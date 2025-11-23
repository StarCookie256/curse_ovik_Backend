
using PerfumeryBackend.ApplicationLayer.DTO.BasketItems;
using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Basket;

public record BasketPageDto(
    [Required] int Id,
    [Required] List<BasketItemDto> BasketItems,
    [Required] double? TotalPrice
);