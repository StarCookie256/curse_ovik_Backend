using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Products;

public record ProductSearchDto(
    [Required] ProductFiltersDto ProductFilters,
    [Required] PaginationDto Pagination
);
