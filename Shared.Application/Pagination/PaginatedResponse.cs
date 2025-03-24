namespace Shared.Application.Pagination;

public record struct PaginatedResponse<TObject>(
    IEnumerable<TObject> Data,
    int Page,
    int Size,
    int TotalCount,
    int TotalPages);