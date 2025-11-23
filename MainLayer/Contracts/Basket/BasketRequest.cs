using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.MainLayer.Contracts.Basket;

public record BasketRequest(
    [Required] int ProductVariationId
);