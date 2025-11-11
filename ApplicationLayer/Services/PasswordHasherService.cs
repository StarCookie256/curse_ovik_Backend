using PerfumeryBackend.ApplicationLayer.Interfaces;

namespace PerfumeryBackend.ApplicationLayer.Services;

public class PasswordHasherService : IPasswordHasherService
{
    public string GenerateSalt() =>
        BCrypt.Net.BCrypt.GenerateSalt();

    public string Generate(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hashedPassword, string salt) =>
        BCrypt.Net.BCrypt.Verify(password + salt, hashedPassword);
}
