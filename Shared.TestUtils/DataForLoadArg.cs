using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;
using System.Reflection;

namespace Shared.TestUtils;

public static class DataForLoadArg {
    /// <summary>
    /// DataForLoadArg.HasProperties<SomeDomainEntity>(nameof(SomeDomainEntity.Id), ...)
    /// </summary>
    public static Expression<Func<TSource, object>> HasProperties<TSource>(
        params string[] properties)
        => Arg.Do<Expression<Func<TSource, object>>>(expression => {
            var dataForLoadType = expression.Body.Type;
            
            for (int i = 0; i < properties.Length; i++) {
                dataForLoadType.GetProperty(properties[i])
                    .Should()
                    .NotBeNull();
            }
        });

    /// <summary>
    /// DataForLoadArg.Equals<SomeDomainEntity>(x => new { x.Id, ...}) 
    /// where SomeDomainEntity has properties Id, ...
    /// </summary>
    public static Expression<Func<TSource, object>> Equals<TSource>(
        Expression<Func<TSource, object>> targetExpression) {
        targetExpression.Body.NodeType
            .Should()
            .Be(ExpressionType.New);

        var targetType = targetExpression.Body.Type;
        var targetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        return Arg.Do<Expression<Func<TSource, object>>>(expression => {
            expression.Body.NodeType
                .Should()
                .Be(ExpressionType.New);

            var dataForLoadType = expression.Body.Type;

            foreach (var targetProperty in targetProperties) {
                dataForLoadType.GetProperty(targetProperty.Name)
                    .Should()
                    .NotBeNull();
            }
        });

    }
}