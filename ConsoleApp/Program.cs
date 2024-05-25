using System.Diagnostics;
using System.Reflection;
using ScrubJay.Extensions;
using ScrubJay.Reflection.Expressions;
using ScrubJay.Reflection.Searching.Criteria;
using ScrubJay.Reflection.Searching.Scratch;


//var ctors = typeof((int, string)).AllMembers().OfType<ConstructorInfo>().ToList();


IMemberCriterionBuilderImpl builder = new MemberCriterionBuilderImpl();
builder.Instance.NonPublic.IsType.


/*


var members = Expressions.FindMembers<string>(str => Debug.WriteLine(str));

Console.WriteLine(members);*/


Console.WriteLine("Press Enter to close");
Console.ReadLine();