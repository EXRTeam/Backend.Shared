using Microsoft.EntityFrameworkCore;
using Shared.Application.Pagination;

namespace Shared.Infrastructure.Extensions;

public static class QueryableExtensions {
    public static IQueryable<TSource> ApplyPagination<TSource>(
        this IQueryable<TSource> source,
        int page, int size) => source
            .Skip((page - 1) * size)
            .Take(size);

    public static async Task<PaginatedResponse<TObject>> ToPaginatedResponseAsync<TObject>(
        this IQueryable<TObject> query,
        int page, int size,
        CancellationToken token) {
        var totalCount = await query.CountAsync(token);

        var totalPages = (int)Math.Ceiling((double)totalCount / size);

        if (totalCount == 0 || totalCount < (page - 1) * size) {
            return new PaginatedResponse<TObject>(
                Data: [],
                Page: page,
                Size: size,
                TotalCount: 0,
                TotalPages: totalPages);
        }

        var data = await query
            .ApplyPagination(page, size)
            .ToListAsync(token);

        return new PaginatedResponse<TObject>(
            Data: data,
            Page: page,
            Size: size,
            TotalCount: totalCount,
            TotalPages: totalPages);
    }
}
