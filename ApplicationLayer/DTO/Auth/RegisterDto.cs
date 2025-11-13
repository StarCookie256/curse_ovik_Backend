using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Auth;
public record RegisterDto(
    [Required] string Username,
    [Required] string Email,
    [Required] string Password,
    [Required] string Phone,
    [Required] IFormFile Image
);
