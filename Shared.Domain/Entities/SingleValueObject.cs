using System.Diagnostics.CodeAnalysis;

namespace Shared.Domain.Entities;

public abstract record SingleValueObject<TValue> where TValue : IEquatable<TValue> {
    public TValue Value { get; private set; }

    protected SingleValueObject() {
        Value = default!;
    }

    protected SingleValueObject(TValue value) {
        Value = value;
    }

    [return: NotNullIfNotNull(nameof(valueObject))]
    public static implicit operator TValue?(SingleValueObject<TValue>? valueObject) 
        => valueObject != null ? valueObject.Value : default;
}