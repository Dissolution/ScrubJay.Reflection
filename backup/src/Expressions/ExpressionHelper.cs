using System.Linq.Expressions;
using ScrubJay.Reflection.Dumping;

namespace ScrubJay.Reflection.Expressions;

public static class ExpressionHelper
{
    private static void WriteCode(TextBuilder text, Expression? expression)
    {
        switch (expression)
        {
            case null:
                break;
            case BinaryExpression binaryExpression:
                break;
            case BlockExpression blockExpression:
                break;
            case ConditionalExpression conditionalExpression:
                break;
            case ConstantExpression constantExpression:
                break;
            case DebugInfoExpression debugInfoExpression:
                break;
            case DefaultExpression defaultExpression:
                break;
            case DynamicExpression dynamicExpression:
                break;
            case GotoExpression gotoExpression:
                break;
            case IndexExpression indexExpression:
                break;
            case InvocationExpression invocationExpression:
                break;
            case LabelExpression labelExpression:
                break;
            case LambdaExpression lambdaExpression:
            {
                var members = Dump.Value(lambdaExpression);

                break;
            }
            case ListInitExpression listInitExpression:
                break;
            case LoopExpression loopExpression:
                break;
            case MemberExpression memberExpression:
                break;
            case MemberInitExpression memberInitExpression:
                break;
            case MethodCallExpression methodCallExpression:
                break;
            case NewArrayExpression newArrayExpression:
                break;
            case NewExpression newExpression:
                break;
            case ParameterExpression parameterExpression:
                break;
            case RuntimeVariablesExpression runtimeVariablesExpression:
                break;
            case SwitchExpression switchExpression:
                break;
            case TryExpression tryExpression:
                break;
            case TypeBinaryExpression typeBinaryExpression:
                break;
            case UnaryExpression unaryExpression:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(expression));
        }

        throw new NotImplementedException();
    }

    public static string ToCode(this Expression expression)
    {
        return TextBuilder.New.Invoke(b => WriteCode(b, expression)).ToStringAndDispose();
    }
}