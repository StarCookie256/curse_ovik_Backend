using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.MainLayer.Contracts.Product;

public record ProductSearchRequest(
    [Required] ProductFiltersRequest ProductFilters,
    [Required] PaginationRequest Pagination
);
