using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Domain.Entities;
using System.Linq.Expressions;
using System.Reflection;

namespace Shared.Infrastructure;

public class SingleValueObjectConversion<TValueObject, TValue> : ValueConverter<TValueObject, TValue>
    where TValueObject : SingleValueObject<TValue>
    where TValue : IEquatable<TValue> {
    private static readonly Expression<Func<TValue, TValueObject>> fromValueToObjectConversion;
    private static readonly Expression<Func<TValueObject, TValue>> fromObjectToValueConversion;

    public SingleValueObjectConversion()
        : base(fromObjectToValueConversion, fromValueToObjectConversion) { }

    static SingleValueObjectConversion() {
        fromValueToObjectConversion = FromValueToObject();
        fromObjectToValueConversion = valueObject => valueObject.Value;
    }

    private static Expression<Func<TValue, TValueObject>> FromValueToObject() {
        var parameter = Expression.Parameter(typeof(TValue), "value");

        var valueObjectConstructor = typeof(TValueObject).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic, [typeof(TValue)]);

        var valueObjectCreation = Expression.New(valueObjectConstructor!, parameter);

        // value => new TValueObject(value);

        var lambda = Expression.Lambda<Func<TValue, TValueObject>>(valueObjectCreation, parameter);
        return lambda;
    }
}