﻿namespace ScrubJay.Reflection;

[Flags]
public enum Visibility
{
    None = 0,
    Private = 1 << 0,
    Protected = 1 << 1,
    Internal = 1 << 2,
    Public = 1 << 3,
    
    NonPublic = Private | Protected | Internal,
    Any = Public | NonPublic,
}