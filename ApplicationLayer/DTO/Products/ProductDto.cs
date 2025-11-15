using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Products;

public record ProductDto(
    [Required] int Id,
    [Required] string Name,
    [Required] string Brand,
    [Required] List<string> Categories,
    [Required] double FPrice,
    [Required] double SPrice,
    [Required] double FVolume,
    [Required] double SVolume,
    [Required] string Gender,
               string Image
);
