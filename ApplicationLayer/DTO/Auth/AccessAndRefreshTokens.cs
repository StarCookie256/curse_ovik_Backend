using PerfumeryBackend.ApplicationLayer.Entities;
using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Auth;

public record AccessAndRefreshTokens(
    [Required] string AccessToken,
    [Required] RefreshToken RefreshToken
);
