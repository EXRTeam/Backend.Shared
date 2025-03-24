using FluentValidation;

namespace Shared.Application.Pagination;

internal record struct PaginationOptions(
    int Page,
    int Size);

internal class PaginationOptionsValidator : AbstractValidator<PaginationOptions> {
    public PaginationOptionsValidator() {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page should be greater than zero");

        RuleFor(x => x.Size)
            .GreaterThan(0)
                .WithMessage("Page size should be greater than zero");
    }
}
