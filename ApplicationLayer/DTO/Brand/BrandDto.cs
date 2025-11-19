using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Brand;

public record BrandDto(
    [Required] int Id,
    [Required] string Name
);