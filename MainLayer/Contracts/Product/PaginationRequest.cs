using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.MainLayer.Contracts.Product;

public record PaginationRequest(
    [Required] int Page,
    [Required] int PageSize
);
