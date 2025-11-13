using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.MainLayer.Contracts.Auth;

public record RegisterUserRequest(
    [Required] string Username,
    [Required] string Email,
    [Required] string Password,
    [Required] string Phone,
    [Required] IFormFile Image
);
