using Microsoft.AspNetCore.Mvc;
using PerfumeryBackend.ApplicationLayer.DTO.Auth;
using PerfumeryBackend.ApplicationLayer.Entities;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.ApplicationLayer.Services;

public class AuthService(
    ICustomerRepository customerRepository,
    IPasswordHasherService passwordHasherService,
    IJwtService jwtService,
    IAvatarService avatarService,
    IBasketService basketService) : IAuthService
{
    public async Task<AccessAndRefreshTokens?> Register(RegisterDto registerDto)
    {
        if (await customerRepository.CustomerAlreadyExist(registerDto.Email))
            return null;

        string salt = passwordHasherService.GenerateSalt();
        string passwordWithSalt = registerDto.Password + salt;
        string passwordHash = passwordHasherService.Generate(passwordWithSalt);

        RefreshToken refreshToken = await jwtService.GenerateRefreshToken();

        string avatarPath = await avatarService.SaveAvatarAsync(registerDto.Image);

        Customer customer = new()
        {
            Name = registerDto.Username,
            Email = registerDto.Email,
            Password = passwordHash,
            PasswordSalt = salt,
            RefreshToken = refreshToken,
            Image = avatarPath,
            Phone = registerDto.Phone
        };

        await customerRepository.AddCustomer(customer);
        await basketService.CreateBasketByCustomerId(customer.Id);

        string accessToken = await jwtService.GenerateAccessToken(customer);

        return new AccessAndRefreshTokens(
            RefreshToken: refreshToken,
            AccessToken: accessToken
        );
    }

    public async Task<AccessAndRefreshTokens?> Login(LoginDto loginDto)
    {
        Customer? customer = await customerRepository.GetByEmailAsync(loginDto.Email);

        if (customer == null) 
        {
            return null;
        }

        if(!passwordHasherService.Verify(loginDto.Password, customer.Password, customer.PasswordSalt))
        {
            return null;
        }

        RefreshToken refreshToken = await jwtService.GenerateRefreshToken();
        string accessToken = await jwtService.GenerateAccessToken(customer);

        await customerRepository.SetRefreshTokenById(customer.Id, refreshToken);

        return new AccessAndRefreshTokens(
            RefreshToken: refreshToken,
            AccessToken: accessToken
        );
    }

    public async Task<AccessAndRefreshTokens?> RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        Customer? customer = await customerRepository.GetByRefreshToken(refreshTokenDto.RefreshToken);

        if (customer == null || customer.RefreshToken.IsExpired)
        {
            return null;
        }

        RefreshToken refreshToken = await jwtService.GenerateRefreshToken();
        string accessToken = await jwtService.GenerateAccessToken(customer);

        await customerRepository.SetRefreshTokenById(customer.Id, refreshToken);

        return new AccessAndRefreshTokens(
            RefreshToken: refreshToken,
            AccessToken: accessToken
        );
    }
}