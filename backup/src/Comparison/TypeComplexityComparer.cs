namespace ScrubJay.Reflection.Comparison;

public sealed class TypeComplexityComparer :
    IEqualityComparer<Type>,
    IComparer<Type>,
    IHasDefault<TypeComplexityComparer>
{
    public static TypeComplexityComparer Default { get; } = new();
    
    private static int RootTypeComplexity(Type type)
    {
        // struct < class < interface
        if (type.IsValueType) return 1;
        if (type.IsClass) return 2;
        if (type.IsInterface) return 3;
        if (type.IsGenericParameter) return 4;
        throw new ArgumentException(null, nameof(type));
    }

    private static int OpennessComplexity(Type type)
    {
        // static < sealed < 'normal' < abstract
        if (type.IsSealed)
        {
            if (type.IsAbstract)
            {
                // static
                return 11;
            }

            // sealed
            return 12;
        }
        if (!type.IsAbstract) 
            return 13;
        // abstract
        return 14;
    }

    private static int GenericTypesComplexity(Type type)
    {
        // `IList` < `IList<int>` < `IList<>` < `IDictionary<string, string>` < `IDictionary<,>` < ... n type args
        var genericTypes = type.GenericTypeArguments;
        var typeCount = genericTypes.Length;
        int complexity = (101 * (typeCount + 1));

        if (!type.IsGenericType)
            return complexity;
        
        foreach (var gt in genericTypes)
        {
            complexity += Complexity(gt);
            
        }
        return complexity;
    }

    public static int Complexity(Type? type)
    {
        if (type is null)
            return 0;
        int complexity = RootTypeComplexity(type) +
            OpennessComplexity(type) +
            GenericTypesComplexity(type);
        return complexity;
    }
    
    public int Compare(Type? left, Type? right)
    {
        if (ReferenceEquals(left, right)) 
            return 0;
        if (left is null) 
            return -1;
        if (right is null) 
            return 1;
        
        int c = RootTypeComplexity(left).CompareTo(RootTypeComplexity(right));
        if (c != 0) return c;
        c = OpennessComplexity(left).CompareTo(OpennessComplexity(right));
        if (c != 0) return c;
        c = GenericTypesComplexity(left).CompareTo(GenericTypesComplexity(right));
        return c;
    }

    public bool Equals(Type? left, Type? right) => Compare(left, right) == 0;

    public int GetHashCode(Type? type) => Complexity(type);
}