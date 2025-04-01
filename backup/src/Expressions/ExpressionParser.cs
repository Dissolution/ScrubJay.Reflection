using System.Linq.Expressions;

namespace ScrubJay.Reflection.Expressions;

public static class ExpressionParser
{
    public static Option<(ConstructorInfo, object?[])> TryParseConstructor(Expression? expression)
    {
        if (expression is null)
            return None();
        if (expression is NewExpression newExpression)
        {
            var ctor = newExpression.Constructor;
            var args = newExpression.Arguments
                .Select(a => (a as ConstantExpression)!.Value).ToArray();
            return Some((ctor, args))!;
        }
        else if (expression is LambdaExpression lambdaExpression)
        {
            return TryParseConstructor(lambdaExpression.Body);
        }

        Debugger.Break();
        return None();
    }

}
