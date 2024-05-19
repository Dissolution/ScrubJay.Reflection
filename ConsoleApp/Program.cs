using System.Diagnostics;
using ScrubJay.Reflection.Expressions;





var members = Expressions.FindMembers<string>(str => Debug.WriteLine(str));

Console.WriteLine(members);


Console.WriteLine("Press Enter to close");
Console.ReadLine();