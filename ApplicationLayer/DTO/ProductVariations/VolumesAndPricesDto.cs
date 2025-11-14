using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.ProductVariations;

public record VolumesAndPricesDto(
  [Required] double FVolume,
  [Required] double SVolume,
  [Required] double FPrice,
  [Required] double SPrice
);
