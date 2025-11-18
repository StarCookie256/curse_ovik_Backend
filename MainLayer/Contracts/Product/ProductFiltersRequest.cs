using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.MainLayer.Contracts.Product;

public record ProductFiltersRequest(
    [Required] List<string> Brands,
    [Required] List<string> Categories,
    [Required] List<string> Gender,
    [Required] List<decimal> PriceValues,
    [Required] List<int> VolumeValues
);
