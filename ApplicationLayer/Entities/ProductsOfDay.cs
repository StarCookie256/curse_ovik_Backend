using PerfumeryBackend.ApplicationLayer.DTO.Products;

namespace PerfumeryBackend.ApplicationLayer.Entities;

public record ProductsOfDay(
    List<ProductDto> ProductDtos,
    DateTime LastUpdated
);
