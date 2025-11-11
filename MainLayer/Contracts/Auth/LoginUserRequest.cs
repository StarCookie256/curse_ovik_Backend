using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.MainLayer.Contracts.Auth;

public record LoginUserRequest(
    [Required] string Email,
    [Required] string Password
);
