using System.Diagnostics;
using System.Reflection;
using ScrubJay.Extensions;
using ScrubJay.Reflection.Expressions;
using ScrubJay.Reflection.Searching.Criteria;
using ScrubJay.Reflection.Searching.Scratch;


//var ctors = typeof((int, string)).AllMembers().OfType<ConstructorInfo>().ToList();

FieldInfo thing = Tester.Thing(b => b.Instance.IsField.Name("blah"));

/*


var members = Expressions.FindMembers<string>(str => Debug.WriteLine(str));

Console.WriteLine(members);*/


Console.WriteLine("Press Enter to close");
Console.ReadLine();


static class Tester
{
    public static TMember Thing<TMember>(Func<IMemberCriterionBuilderImpl, ICriterion<TMember>> build)
        where TMember : MemberInfo
    {
        IMemberCriterionBuilderImpl builder = new MemberCriterionBuilderImpl();
        ICriterion<TMember> criterion = build(builder);
        
        Debugger.Break();
        throw new NotImplementedException();
    }
}