using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.MainLayer.Contracts.Basket;

public record BasketRequest(
    [Required] int BasketId,
    [Required] int BasketItemId,
    [Required] int ProductVariationId,
    [Required] int Stock
);