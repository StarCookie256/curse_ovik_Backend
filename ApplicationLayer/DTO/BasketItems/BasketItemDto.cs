using PerfumeryBackend.ApplicationLayer.DTO.ProductVariations;
using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.BasketItems;

public record BasketItemDto(
    [Required] int Id,
    [Required] string Name,
    [Required] string Image,
    [Required] ProductVariationDto ProductVariation,
    [Required] int? Stock
);