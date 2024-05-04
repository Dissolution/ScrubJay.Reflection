﻿using Jay.Reflection.Utilities;

namespace Jay.Reflection.Fulfilling;

internal class BackingFieldImplementer : Implementer, IBackingFieldImplementer
{
    public BackingFieldImplementer(TypeBuilder typeBuilder) 
        : base(typeBuilder)
    {
    }

    public FieldBuilder ImplementBackingField(PropertyInfo property)
    {
        string fieldName = MemberNaming.CreateBackingFieldName(property);
        FieldAttributes fieldAttributes = FieldAttributes.Private;
        
        if (!property.CanWrite && property.GetSetter() is null)
        {
            fieldAttributes |= FieldAttributes.InitOnly;
        }

        if (property.IsStatic())
        {
            fieldAttributes |= FieldAttributes.Static;
        }

        return _typeBuilder.DefineField(
            fieldName,
            property.PropertyType,
            fieldAttributes);
    }
}