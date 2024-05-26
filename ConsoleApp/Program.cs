using System.Diagnostics;
using System.Reflection;
using ScrubJay.Reflection.Searching;
using ScrubJay.Reflection.Searching.Criteria;


//var ctors = typeof((int, string)).AllMembers().OfType<ConstructorInfo>().ToList();



/*


var members = Expressions.FindMembers<string>(str => Debug.WriteLine(str));

Console.WriteLine(members);*/


Console.WriteLine("Press Enter to close");
Console.ReadLine();


//static class Tester
//{
//    public static TMember Thing<TMember>(Func<IMemberCriterionBuilderImpl, ICriterion<TMember>> build)
//        where TMember : MemberInfo
//    {
//        IMemberCriterionBuilderImpl builder = new MemberCriterionBuilderImpl();
//        ICriterion<TMember> criterion = build(builder);
//        
//        Debugger.Break();
//        throw new NotImplementedException();
//    }
//}