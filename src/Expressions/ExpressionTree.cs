using System.Linq.Expressions;

namespace ScrubJay.Reflection.Expressions;

public static class ExpressionTree
{
    private static IEnumerable<Expression> EnumerateChildren(Expression expression)
    {
        switch (expression)
        {
            case BinaryExpression binaryExpression:
            {
                yield return binaryExpression.Left;
                yield return binaryExpression.Right;
                yield break;
            }
            case BlockExpression blockExpression:
            {
                foreach (var expr in blockExpression.Expressions)
                    yield return expr;
                yield break;
            }
            case ConditionalExpression conditionalExpression:
            {
                yield return conditionalExpression.Test;
                yield return conditionalExpression.IfTrue;
                yield return conditionalExpression.IfFalse;
                yield break;
            }
            case ConstantExpression constantExpression:
                yield break;
            case DebugInfoExpression debugInfoExpression:
                yield break;
            case DefaultExpression defaultExpression:
                yield break;
            case DynamicExpression dynamicExpression:
                yield break;
            case GotoExpression gotoExpression:
                yield break;
            case IndexExpression indexExpression:
                yield break;
            case InvocationExpression invocationExpression:
            {
                yield return invocationExpression.Expression;
                foreach (var expr in invocationExpression.Arguments)
                    yield return expr;
                yield break;
            }
            case LabelExpression labelExpression:
                yield break;
            case LambdaExpression lambdaExpression:
            {
                foreach (var expr in lambdaExpression.Parameters)
                    yield return expression;
                yield return lambdaExpression.Body;
                yield break;
            }
            case ListInitExpression listInitExpression:
            {
                yield return listInitExpression.NewExpression;
                yield break;
            }
            case LoopExpression loopExpression:
            {
                yield return loopExpression.Body;
                yield break;
            }
            case MemberExpression memberExpression:
            {
                if (memberExpression.Expression is not null)
                    yield return memberExpression.Expression;
                yield break;
            }
            case MemberInitExpression memberInitExpression:
            {
                yield return memberInitExpression.NewExpression;
                yield break;
            }
            case MethodCallExpression methodCallExpression:
            {
                foreach (var expr in methodCallExpression.Arguments)
                    yield return expr;
                yield break;
            }
            case NewArrayExpression newArrayExpression:
            {
                foreach (var expr in newArrayExpression.Expressions)
                    yield return expr;
                yield break;
            }
            case NewExpression newExpression:
            {
                foreach (var expr in newExpression.Arguments)
                    yield return expr;
                yield break;
            }
            case ParameterExpression parameterExpression:
                yield break;
            case RuntimeVariablesExpression runtimeVariablesExpression:
            {
                foreach (var expr in runtimeVariablesExpression.Variables)
                    yield return expr;
                yield break;
            }
            case SwitchExpression switchExpression:
            {
                yield return switchExpression.SwitchValue;
                if (switchExpression.DefaultBody is not null)
                    yield return switchExpression.DefaultBody;
                yield break;
            }
            case TryExpression tryExpression:
            {
                yield return tryExpression.Body;
                if (tryExpression.Fault is not null)
                    yield return tryExpression.Fault;
                if (tryExpression.Finally is not null)
                    yield return tryExpression.Finally;
                yield break;
            }
            case TypeBinaryExpression typeBinaryExpression:
            {
                yield return typeBinaryExpression.Expression;
                yield break;
            }
            case UnaryExpression unaryExpression:
            {
                yield return unaryExpression.Operand;
                yield break;
            }
            default:
                yield break;
        }
    }

    private static IEnumerable<MemberInfo> EnumerateMembers(Expression expression)
    {
        switch (expression)
        {
            case BinaryExpression binaryExpression:
            {
                if (binaryExpression.Method is not null)
                    yield return binaryExpression.Method;
                yield break;
            }
            case BlockExpression blockExpression:
                yield break;
            case ConditionalExpression conditionalExpression:
                yield break;
            case ConstantExpression constantExpression:
                yield break;
            case DebugInfoExpression debugInfoExpression:
                yield break;
            case DefaultExpression defaultExpression:
                yield break;
            case DynamicExpression dynamicExpression:
                yield break;
            case GotoExpression gotoExpression:
                yield break;
            case IndexExpression indexExpression:
            {
                if (indexExpression.Indexer is not null)
                    yield return indexExpression.Indexer;
                yield break;
            }
            case InvocationExpression invocationExpression:
                yield break;
            case LabelExpression labelExpression:
                yield break;
            case LambdaExpression lambdaExpression:
                yield break;
            case ListInitExpression listInitExpression:
                yield break;
            case LoopExpression loopExpression:
                yield break;
            case MemberExpression memberExpression:
            {
                yield return memberExpression.Member;
                yield break;
            }
            case MemberInitExpression memberInitExpression:
                yield break;
            case MethodCallExpression methodCallExpression:
            {
                yield return methodCallExpression.Method;
                yield break;
            }
            case NewArrayExpression newArrayExpression:
                yield break;
            case NewExpression newExpression:
            {
                if (newExpression.Constructor is not null)
                    yield return newExpression.Constructor;
                if (newExpression.Members is not null)
                {
                    foreach (var member in newExpression.Members)
                        yield return member;
                }

                yield break;
            }
            case ParameterExpression parameterExpression:
                yield break;
            case RuntimeVariablesExpression runtimeVariablesExpression:
                yield break;
            case SwitchExpression switchExpression:
            {
                if (switchExpression.Comparison is not null)
                    yield return switchExpression.Comparison;
                yield break;
            }
            case TryExpression tryExpression:
                yield break;
            case TypeBinaryExpression typeBinaryExpression:
            {
                yield return typeBinaryExpression.TypeOperand;
                yield break;
            }
            case UnaryExpression unaryExpression:
            {
                if (unaryExpression.Method is not null)
                    yield return unaryExpression.Method;
                yield break;
            }
            default:
                yield break;
        }
    }


    public static IEnumerable<Expression> Enumerate(Expression? expression, Scope scanDepth)
    {
        if (expression is null || scanDepth == default)
            yield break;

        if (scanDepth.HasFlag(Scope.Self))
            yield return expression;

        if (scanDepth == Scope.Self)
            yield break;

        if (scanDepth == Scope.Children)
        {
            foreach (var expr in EnumerateChildren(expression))
                yield return expr;
            yield break;
        }

        Debug.Assert(scanDepth.HasFlag(Scope.Descendants));

        Scope scan = Scope.Self | Scope.Children | Scope.Descendants;
        foreach (var childExpression in EnumerateChildren(expression))
        {
            foreach (var expr in Enumerate(childExpression, scan))
                yield return expr;
        }

        yield break;
    }

    public static IEnumerable<MemberInfo> EnumerateMembers(Expression? expression, Scope scope)
    {
        return Enumerate(expression, scope).SelectMany(static child => EnumerateMembers(child));
    }
}