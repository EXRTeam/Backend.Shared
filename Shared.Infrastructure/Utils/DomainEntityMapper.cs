﻿using System.Linq.Expressions;
using System.Reflection;

namespace Shared.Infrastructure.Utils;

public static class DomainEntityMapper {
    private static readonly MethodInfo createMapFunctionMethod =
        typeof(DomainEntityMapper)
            .GetMethod(nameof(CreateMapFunction), BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new NullReferenceException();

    private static readonly MethodInfo enumerableSelectMethod =
        typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .FirstOrDefault(m =>
                m.Name == "Select"
                && m.IsGenericMethodDefinition
                && m.GetParameters().Length == 2)!;

    private static readonly Dictionary<(Type, Type), Delegate> mappers = [];

    public static TDestination Map<TDestination>(object source) {
        var sourceType = source.GetType();
        var destinationType = typeof(TDestination);

        var key = (sourceType, destinationType);

        if (mappers.TryGetValue(key, out var existingMapper)) {
            var result = ((Func<object, TDestination>)existingMapper)(source);
            return result;
        }

        var mapper = CreateMapFunction<TDestination>(sourceType);
        mappers[key] = mapper;

        return mapper(source);
    }

    private static Func<object, TDestination> CreateMapFunction<TDestination>(Type sourceType) {
        var destinationType = typeof(TDestination);

        var objectSourceParameter = Expression.Parameter(typeof(object), $"source_{sourceType}");
        var sourceParameter = Expression.Convert(objectSourceParameter, sourceType);

        var destinationPrivateConstructor = destinationType.GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic, Type.EmptyTypes)
            ?? throw new NullReferenceException($"Private constructor not found for aggregate {sourceType}");

        var createExpression = Expression.New(destinationPrivateConstructor);

        var createdDestinationObjectVariable = Expression.Variable(destinationType, $"destination_{destinationType}");

        var creationDestinationObject = Expression.Assign(createdDestinationObjectVariable, createExpression);

        var destinationProperties = destinationType.GetProperties(
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

        var propertyAssignExpressions = new List<BinaryExpression>();

        for (int i = 0; i < destinationProperties.Length; i++) {
            PropertyInfo destinationProperty = destinationProperties[i];
            var sourceProperty = sourceType.GetProperty(destinationProperty.Name);

            if (sourceProperty == null) {
                continue;
            }

            Expression assignRightExpression;

            var destinationField = (destinationType.GetField($"<{destinationProperty.Name}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)
                ?? destinationType.GetField(
                    string.Concat(
                        char.ToLower(destinationProperty.Name[0]),
                        destinationProperty.Name[1..]),
                    BindingFlags.NonPublic | BindingFlags.Instance)
                )
                ?? throw new NullReferenceException(
                    $"Field of property {destinationProperty.Name} at destination type {destinationType} not found");

            if (sourceProperty.PropertyType != destinationField.FieldType) {
                assignRightExpression = CreateMemberMapExpression(
                    sourceParameter,
                    sourceProperty,
                    destinationField);
            } else {
                assignRightExpression = Expression.Property(sourceParameter, sourceProperty);
            }

            var assignPropertyExpression = Expression.Assign(
                Expression.Field(createdDestinationObjectVariable, destinationField),
                assignRightExpression);

            propertyAssignExpressions.Add(assignPropertyExpression);
        }

        var bodyExpressions = new List<Expression> {
        creationDestinationObject
    };

        bodyExpressions.AddRange(propertyAssignExpressions);
        bodyExpressions.Add(createdDestinationObjectVariable);

        var body = Expression.Block([createdDestinationObjectVariable], bodyExpressions);

        var lambdaExpression = Expression.Lambda<Func<object, TDestination>>(body, objectSourceParameter);
        return lambdaExpression.Compile();
    }

    private static Expression CreateMemberMapExpression(
        Expression sourceParameter,
        PropertyInfo sourceProperty,
        FieldInfo destinationField) {
        var sourcePropertyType = sourceProperty.PropertyType;
        var destinationPropertyType = destinationField.FieldType;

        if (sourcePropertyType.IsGenericType && destinationPropertyType.IsGenericType) {
            Type? sourceEnumerableInterface = null;
            Type? destinationEnumerableInterface = null;

            if (sourcePropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
                sourceEnumerableInterface = sourcePropertyType;
            }

            if (sourceEnumerableInterface == null) {
                sourceEnumerableInterface = sourcePropertyType.GetInterfaces()
                    .FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            }

            if (destinationPropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
                destinationEnumerableInterface = destinationPropertyType;
            }

            if (destinationEnumerableInterface == null) {
                destinationEnumerableInterface = destinationPropertyType.GetInterfaces()
                    .FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            }

            if (sourceEnumerableInterface == null || destinationEnumerableInterface == null) {
                throw new NullReferenceException($"Can not map {sourcePropertyType} to {destinationPropertyType}");
            }

            var sourceGenericArgumentType = sourceEnumerableInterface.GetGenericArguments()[0];
            var destinationGenericArgumentType = destinationEnumerableInterface.GetGenericArguments()[0];

            var collectionConstructor = destinationPropertyType
                .GetConstructor(
                    BindingFlags.Public | BindingFlags.Instance,
                    [typeof(IEnumerable<>).MakeGenericType(destinationGenericArgumentType)])!;

            if (sourceGenericArgumentType == destinationGenericArgumentType) {
                // collection of same types (for example List<T> maps to IEnumerable<T>)
                return Expression.New(
                    collectionConstructor,
                    Expression.Property(sourceParameter, sourceProperty));
            } else {
                // collections of different types, creating mapping with Select method at IEnumerable<>
                return Expression.New(
                    collectionConstructor,
                    Expression.Call(
                        null,
                        enumerableSelectMethod
                            .MakeGenericMethod(sourceGenericArgumentType, destinationGenericArgumentType)!,
                        Expression.Property(sourceParameter, sourceProperty),
                        Expression.Constant(
                            createMapFunctionMethod
                                .MakeGenericMethod(destinationGenericArgumentType)
                                .Invoke(null, [sourcePropertyType]))
                    )
                );
            }
        }

        return Expression.Invoke(
            Expression.Constant(
                createMapFunctionMethod
                    .MakeGenericMethod(destinationPropertyType)
                    .Invoke(null, [sourcePropertyType])),
            Expression.Property(sourceParameter, sourceProperty));
    }
}