using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Auth;

public record CustomerData(
    [Required] int Id,
    [Required] string Image,
    [Required] string Name,
    [Required] string Email,
    [Required] string Phone,
               string Address
);