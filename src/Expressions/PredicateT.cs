﻿using System.Linq.Expressions;

namespace ScrubJay.Reflection.Expressions;

public static class Predicate<T>
{
    public static readonly Expression<Func<T, bool>> True = item => true;

    public static readonly Expression<Func<T, bool>> False = item => false;
}