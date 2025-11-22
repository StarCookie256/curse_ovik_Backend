using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Basket;

public record BasketDto(
    [Required] int BasketId,
    [Required] int BasketItemId,
    [Required] int ProductVariationId,
    [Required] int Stock
);