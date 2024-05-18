using System.Diagnostics;
using ScrubJay.Reflection.Expressions;





var member = Expressions
    .FirstMember(() => Debug.WriteLine("ABC"));

Console.WriteLine(member);


Console.WriteLine("Press Enter to close");
Console.ReadLine();