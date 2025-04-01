using System.Reflection;

namespace ScrubJay.Reflection.Tests.Demos;

public class TestClass
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public List<BindingFlags> BindingFlags { get; set; } = [];

    public TestClass(int id)
    {
        this.Id = id;
    }

    public TestClass(string name)
    {
        this.Name = name;
    }

    public TestClass(List<BindingFlags> bindingFlags)
    {
        this.BindingFlags = bindingFlags;
    }
}

public struct TestStruct
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public List<BindingFlags> BindingFlags { get; set; } = [];

    public TestStruct(int id)
    {
        this.Id = id;
    }

    public TestStruct(string name)
    {
        this.Name = name;
    }

    public TestStruct(List<BindingFlags> bindingFlags)
    {
        this.BindingFlags = bindingFlags;
    }
}
