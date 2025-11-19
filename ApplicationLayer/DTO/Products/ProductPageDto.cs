using PerfumeryBackend.ApplicationLayer.DTO.Brand;
using PerfumeryBackend.ApplicationLayer.DTO.ProductVariations;
using PerfumeryBackend.DatabaseLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Products;

public record ProductPageDto(
    [Required] int Id,
    [Required] string Name,
    [Required] BrandDto Brand,
    [Required] List<string> Categories,
    [Required] string Gender,
               int? ManufactureYear,
               string? Image,
               string? ExpirationDate,
               string? Country,
               List<ProductVariationDto> ProductVariations
);