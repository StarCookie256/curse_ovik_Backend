using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.ProductVariations;

public record ProductVariationDto(
    [Required] int Id,
    [Required] int ProductId,
    [Required] string? Category,
    [Required] double? Price,
    [Required] double? Volume,
    [Required] int? Stock
);