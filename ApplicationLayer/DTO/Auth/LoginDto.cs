using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Auth;
public record LoginDto(
    [Required] string Username,
    [Required] string Password
);
