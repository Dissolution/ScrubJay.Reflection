using System.Diagnostics;

Delegate del = () => 4;
Func<int> func = () => 5;

var delType = del.GetType();
var funcType = func.GetType();

Debugger.Break();




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