using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Basket;

public record BasketDto(
    [Required] int CustomerId,
    [Required] int ProductVariationId
);