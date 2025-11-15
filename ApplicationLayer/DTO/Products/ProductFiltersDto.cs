using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Products;

public record ProductFiltersDto(
    [Required] List<string> Gender,
    [Required] List<decimal> PriceValues,
    [Required] List<string> Brands,
    [Required] List<string> Categories,
    [Required] List<int> VolumeValues
);
