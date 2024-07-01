using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Corathing.Organizer.WPF.Utils;

public static class JsonUtils
{
    public static void PopulateObject<T>(T target, string jsonSource) where T : class =>
        PopulateObject(target, jsonSource, typeof(T));

    public static void OverwriteProperty<T>(T target, JsonProperty updatedProperty) where T : class =>
        OverwriteProperty(target, updatedProperty, typeof(T));

    private static void PopulateObject(object target, string jsonSource, Type type)
    {
        var json = JsonDocument.Parse(jsonSource).RootElement;

        foreach (var property in json.EnumerateObject())
        {
            OverwriteProperty(target, property, type);
        }
    }

    private static void PopulateCollection(object target, string jsonSource, Type elementType)
    {
        var json = JsonDocument.Parse(jsonSource).RootElement;
        var addMethod = target.GetType().GetMethod("Add", new[] { elementType });
        var containsMethod = target.GetType().GetMethod("Contains", new[] { elementType });

        foreach (var property in json.EnumerateArray())
        {
            object? element;

            if (elementType.IsValueType || elementType == typeof(string))
            {
                element = JsonSerializer.Deserialize(jsonSource, elementType);
            }
            else if (IsCollection(elementType))
            {
                var nestedElementType = elementType.GenericTypeArguments[0];
                element = Instantiate(elementType);

                PopulateCollection(element, property.GetRawText(), nestedElementType);
            }
            else
            {
                element = Instantiate(elementType);

                PopulateObject(element, property.GetRawText(), elementType);
            }

            var contains = containsMethod.Invoke(target, new[] { element });
            if (contains is false)
            {
                addMethod.Invoke(target, new[] { element });
            }
        }
    }

    private static void OverwriteProperty(object target, JsonProperty updatedProperty, Type type)
    {
        var propertyInfo = type.GetProperty(updatedProperty.Name);

        if (propertyInfo == null)
        {
            return;
        }

        if (updatedProperty.Value.ValueKind == JsonValueKind.Null)
        {
            propertyInfo.SetValue(target, null);
            return;
        }

        var propertyType = propertyInfo.PropertyType;
        object? parsedValue;

        if (propertyType.IsValueType || propertyType == typeof(string))
        {
            parsedValue = JsonSerializer.Deserialize(
                updatedProperty.Value.GetRawText(),
                propertyType);
        }
        else if (IsCollection(propertyType))
        {
            var elementType = propertyType.GenericTypeArguments[0];
            parsedValue = propertyInfo.GetValue(target);
            parsedValue ??= Instantiate(propertyType);

            PopulateCollection(parsedValue, updatedProperty.Value.GetRawText(), elementType);
        }
        else
        {
            parsedValue = propertyInfo.GetValue(target);
            parsedValue ??= Instantiate(propertyType);

            PopulateObject(
                parsedValue,
                updatedProperty.Value.GetRawText(),
                propertyType);
        }

        propertyInfo.SetValue(target, parsedValue);
    }

    private static object Instantiate(Type type)
    {
        var ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Array.Empty<Type>());

        if (ctor is null)
        {
            throw new InvalidOperationException($"Type {type.Name} has no parameterless constructor.");
        }

        return ctor.Invoke(Array.Empty<object?>());
    }

    private static bool IsCollection(Type type) =>
        type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));
}
