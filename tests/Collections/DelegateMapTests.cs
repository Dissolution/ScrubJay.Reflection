using ScrubJay.Reflection.Collections;
using ScrubJay.Utilities;

namespace ScrubJay.Reflection.Tests.Collections;

public class DelegateMapTests
{
    [Fact]
    public void ActionReturnsVoidKey()
    {
        Type[] key = DelegateMap.GetKey<Action>();
        Assert.NotNull(key);
        Assert.Single(key);
        Assert.Equal(typeof(void), key[0]);
    }
    
    [Fact]
    public void FuncReturnsKeyOfReturnType()
    {
        Type[] key = DelegateMap.GetKey<Func<Guid>>();
        Assert.NotNull(key);
        Assert.Single(key);
        Assert.Equal<Type>([typeof(Guid)], key);
    }

    [Fact]
    public void GenericTypesMatch()
    {
        Type[] key = DelegateMap.GetKey<Fn<int, Guid, string?>>();
        Assert.NotNull(key);
        Assert.Equal(3, key.Length);
        Assert.Equal(typeof(int), key[0]);
        Assert.Equal(typeof(Guid), key[1]);
        Assert.Equal(typeof(string), key[2]);
    }
}