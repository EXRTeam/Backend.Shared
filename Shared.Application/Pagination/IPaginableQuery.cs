using Shared.Application.Mediator;

namespace Shared.Application.Pagination;

public interface IPaginableQuery<TDtoObject> : IQuery<PaginatedResponse<TDtoObject>> {
    public int Page { get; }
    public int Size { get; }
}
