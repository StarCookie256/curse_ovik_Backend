using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Products;

public record PaginationDto(
    [Required] int Page,
    [Required] int PageSize
);