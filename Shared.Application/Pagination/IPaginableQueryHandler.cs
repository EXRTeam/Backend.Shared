using Shared.Application.Mediator;

namespace Shared.Application.Pagination;

public interface IPaginableQueryHandler<TQuery, TDtoObject> : IQueryHandler<TQuery, PaginatedResponse<TDtoObject>>
    where TQuery : class, IPaginableQuery<TDtoObject>;
