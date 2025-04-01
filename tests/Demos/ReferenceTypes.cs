using System.Diagnostics;

namespace ScrubJay.Reflection.Tests.Demos;

public static class ReferenceTypes
{
    [Fact]
    public static void ClassesPassedByRef()
    {
        var test = new TestClass(147);
        Assert.Equal(147, test.Id);

        test.Id = 3;
        Assert.Equal(3, test.Id);

        ChangeIdTo(test, 40);
        Assert.Equal(40, test.Id);

        ChangeIdTo_In(in test, 41);
        Assert.Equal(41, test.Id);

        ChangeIdTo_Ref(ref test, 42);
        Assert.Equal(42, test.Id);

    }

    private static void ChangeIdTo(TestClass pt, int newY)
    {
        pt.Id = newY;
    }

    private static void ChangeIdTo_In(in TestClass pt, int newY)
    {
        pt.Id = newY;
    }

    private static void ChangeIdTo_Ref(ref TestClass pt, int newY)
    {
        pt.Id = newY;
    }


    [Fact]
    public static void StructsPassedByValue()
    {
        var test = new TestStruct(147);
        Assert.Equal(147, test.Id);

        test.Id = 3;
        Assert.Equal(3, test.Id);

        ChangeIdTo(test, 40);
        Assert.Equal(3, test.Id);

        /*ChangeIdTo_In(in test, 41);
        Assert.Equal(41, test.Id);*/

        ChangeIdTo_Ref(ref test, 42);
        Assert.Equal(42, test.Id);

    }

    private static void ChangeIdTo(TestStruct pt, int newY)
    {
        pt.Id = newY;
    }

    private static void ChangeIdTo_Ref(ref TestStruct pt, int newY)
    {
        pt.Id = newY;
    }
}

