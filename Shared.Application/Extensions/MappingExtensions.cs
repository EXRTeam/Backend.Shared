using Shared.Domain.Results;

namespace Shared.Application;

public static class MappingExtensions {
    public static Result<TDestination[]> Map<TDestination, TSource>(
        this TSource[] source,
        Func<TSource, Result<TDestination>> mapper) {
        var array = new TDestination[source.Length];

        for (int i = 0; i < source.Length; i++) {
            var ingredient = mapper(source[i]);
            if (ingredient.IsFailure) return ingredient.Error;
            array[i] = ingredient.Value;
        }

        return array;
    }
}
