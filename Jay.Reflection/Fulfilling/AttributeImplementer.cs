﻿using Jay.Collections;
using Jay.Reflection.Building;
using Jay.Reflection.Building.Emission;
using Jay.Validation;

namespace Jay.Reflection.Fulfilling;

public static class AttributeImplementer
{
    private static readonly ConcurrentTypeDictionary<Func<Attribute, CustomAttributeBuilder>> _cabCache = new();


    private static CustomAttributeBuilder GetCustomAttributeBuilderFor(Attribute attribute)
    {
        var type = attribute.GetType();
        var createCab = _cabCache.GetOrAdd(type, CreateCabFunc);
        return createCab(attribute);
    }

    private static Func<Attribute, object?[]> CreateGetFieldValuesDelegate(FieldInfo[] fields)
    {
        int len = fields.Length;
        if (len == 0)
            return (_ => Array.Empty<object?>());
        
        return RuntimeBuilder.CreateDelegate<Func<Attribute, object?[]>>("getFieldValues", runtimeBuilder =>
        {
            var emitter = runtimeBuilder.Emitter;
            emitter.DeclareLocal<object?[]>(out var values)
                .Ldc_I4(len)
                .Newarr(typeof(object))
                .Stloc(values);
            for (var i = 0; i < len; i++)
            {
                // values[i] = (object)(attr._fields[i])
                emitter.Ldloc(values)
                    .Ldc_I4(i)
                    .Ldarg(0)
                    .Ldfld(fields[i])
                    .EmitCast(fields[i].FieldType, typeof(object))
                    .Stelem<object>();
            }

            emitter.Ldloc(values)
                .Ret();
        });
    }
    
    private static Func<Attribute, object?[]> CreateGetPropertyValuesDelegate(PropertyInfo[] properties)
    {
        int len = properties.Length;
        if (len == 0)
            return (_ => Array.Empty<object?>());
        
        return RuntimeBuilder.CreateDelegate<Func<Attribute, object?[]>>("getPropertyValues", runtimeBuilder =>
        {
            var emitter = runtimeBuilder.Emitter;
            emitter.DeclareLocal<object?[]>(out var values)
                .Ldc_I4(len)
                .Newarr(typeof(object))
                .Stloc(values);
            for (var i = 0; i < len; i++)
            {
                // values[i] = (object)properties[i].GetValue(attribute)
                var getter = properties[i].GetGetter().ThrowIfNull();
                emitter.Ldloc(values)
                    .Ldc_I4(i)
                    .Ldarg(0)
                    .Call(getter)
                    .EmitCast(properties[i].PropertyType, typeof(object))
                    .Stelem<object>();
            }

            emitter.Ldloc(values)
                .Ret();
        });
    }

    private static (PropertyInfo[] Properties, Func<Attribute, object?[]> GetPropertyValues) FindProperties(Type attributeType)
    {
        var properties = attributeType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.CanRead)
            .Where(property => property.Name != "TypeId")
            .ToArray();
        return (properties, CreateGetPropertyValuesDelegate(properties));
    }
    
    private static (FieldInfo[] Fields, Func<Attribute, object?[]> GetFieldValues) FindFields(Type attributeType)
    {
        var fields = attributeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        return (fields, CreateGetFieldValuesDelegate(fields));
    }

    private static object?[] EmptyValues(Attribute _) => Array.Empty<object?>();

    private static Func<Attribute, CustomAttributeBuilder> CreateCabFunc(Type attributeType)
    {
        var flags = BindingFlags.Public | BindingFlags.Instance;

        var (fields, getFieldValues) = FindFields(attributeType);
        var (properties, getPropertyValues) = FindProperties(attributeType);
        
        // Constructor
        ConstructorInfo? ctor;
        Func<Attribute, object?[]> getCtorArgs;

        // Default?
        ctor = attributeType.GetConstructor(flags, Type.EmptyTypes);
        if (ctor is not null)
        {
            getCtorArgs = EmptyValues;
        }
        else
        {
            // N args = N fields?
            var fieldTypes = Array.ConvertAll(fields, static f => f.FieldType);
            ctor = Searching.MemberSearch.GetConstructor(attributeType, flags, fieldTypes);
            if (ctor is not null)
            {
                getCtorArgs = getFieldValues;
                fields = Array.Empty<FieldInfo>();
                getFieldValues = EmptyValues;
            }
            else
            {
                // N args = N properties?
                var propertyTypes = Array.ConvertAll(properties, static p => p.PropertyType);
                ctor = Searching.MemberSearch.GetConstructor(attributeType, flags, propertyTypes);
                if (ctor is not null)
                {
                    getCtorArgs = getPropertyValues;
                    properties = Array.Empty<PropertyInfo>();
                    getPropertyValues = EmptyValues;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
        
        // Return our delegate now that we've pre-caculated as much as we can
        return (attribute) =>
        {
            var ctorArgs = getCtorArgs(attribute);
            var propertyValues = getPropertyValues(attribute);
            var fieldValues = getFieldValues(attribute);
            return new CustomAttributeBuilder(
                con: ctor,
                constructorArgs: ctorArgs,
                namedProperties: properties,
                propertyValues: propertyValues,
                namedFields: fields,
                fieldValues: fieldValues);
        };
    }


    public static void ImplementAttributes(MemberInfo copyAttributesFrom,
        Action<CustomAttributeBuilder> addAttribute)
    {
        Attribute[] attributes = Attribute.GetCustomAttributes(copyAttributesFrom, true);
        if (attributes.Length == 0) return;
        foreach (var attribute in attributes)
        {
            CustomAttributeBuilder cab = GetCustomAttributeBuilderFor(attribute);
            addAttribute(cab);
        }
    }
}