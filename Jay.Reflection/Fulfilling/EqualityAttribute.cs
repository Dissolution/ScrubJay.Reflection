﻿namespace Jay.Reflection.Fulfilling;

[AttributeUsage(validOn: AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EqualityAttribute : Attribute
{
    public bool ParticipatesInEquality { get; init; }

    public EqualityAttribute(bool participatesInEquality = true)
    {
        ParticipatesInEquality = participatesInEquality;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return ParticipatesInEquality ? "Participates" : "Doesn't";
    }
}