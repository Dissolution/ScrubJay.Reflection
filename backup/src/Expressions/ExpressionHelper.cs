using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace ScrubJay.Reflection.Expressions;

/// <summary>
/// 
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.linq.expressions"/>
public static class ExpressionHelper
{
    private static void AddTo<T>(T? value, List<T> list)
    {
        if (value is not null)
        {
            list.Add(value);
        }
    }
    
    private static void AddTo<T>(ReadOnlyCollection<T?>? values, List<T> list)
    {
        if (values is not null)
        {
            foreach (T? value in values)
            {
                if (value is not null)
                {
                    list.Add(value);
                }
            }
        }
    }

    private static void ExtractMembersTo<E>(ReadOnlyCollection<E> expressions, List<MemberInfo> members)
        where E : Expression
    {
        foreach (var expression in expressions)
        {
            ExtractMembersTo(expression, members);
        }
    }
    
    private static void ExtractMembersTo(Expression? expression, List<MemberInfo> members)
    {
        switch (expression)
        {
            case null:
                return;
            case BinaryExpression binaryExpression:
            {
                ExtractMembersTo(binaryExpression.Left, members);
                ExtractMembersTo(binaryExpression.Right, members);
                AddTo(binaryExpression.Method, members);
                return;
            }
            case BlockExpression blockExpression:
            {
                ExtractMembersTo(blockExpression.Variables, members);
                ExtractMembersTo(blockExpression.Expressions, members);
                return;
            }
            case ConditionalExpression conditionalExpression:
            {
                ExtractMembersTo(conditionalExpression.Test, members);
                ExtractMembersTo(conditionalExpression.IfTrue, members);
                ExtractMembersTo(conditionalExpression.IfFalse, members);
                return;
            }
            case ConstantExpression constantExpression:
            {
                // spoiler: it isn't
                if (constantExpression.Value is MemberInfo member)
                {
                    AddTo(member, members);
                }
                return;
            }
            case DebugInfoExpression debugInfoExpression:
                // nothing
                return;
            case DefaultExpression defaultExpression:
                // nothing
                return;
            case DynamicExpression dynamicExpression:
                throw new NotSupportedException();
//            case Expression<TODO> expression1:
//                break;
            case GotoExpression gotoExpression:
                // nothing
                return;
            case IndexExpression indexExpression:
            {
                ExtractMembersTo(indexExpression.Object, members);
                ExtractMembersTo(indexExpression.Arguments, members);
                AddTo(indexExpression.Indexer, members);
                return;
            }
            case InvocationExpression invocationExpression:
            {
                ExtractMembersTo(invocationExpression.Arguments, members);
                ExtractMembersTo(invocationExpression.Expression, members);
                return;
            }
            case LabelExpression labelExpression:
                // nothing
                return;
            case LambdaExpression lambdaExpression:
            {
                ExtractMembersTo(lambdaExpression.Parameters, members);
                ExtractMembersTo(lambdaExpression.Body, members);
                AddTo(lambdaExpression.ReturnType, members);
                return;
            }
            case ListInitExpression listInitExpression:
            {
                var initializers = listInitExpression.Initializers;
                foreach (ElementInit elementInit in initializers)
                {
                    AddTo(elementInit.AddMethod, members);
                    ExtractMembersTo(elementInit.Arguments, members);
                }
                ExtractMembersTo(listInitExpression.NewExpression, members);
                return;
            }
            case LoopExpression loopExpression:
            {
                ExtractMembersTo(loopExpression.Body, members);
                return;
            }
            case MemberExpression memberExpression:
            {
                ExtractMembersTo(memberExpression.Expression, members);
                AddTo(memberExpression.Member, members);
                return;
            }
            case MemberInitExpression memberInitExpression:
            {
                ExtractMembersTo(memberInitExpression.NewExpression, members);
                return;
            }
            case MethodCallExpression methodCallExpression:
            {
                ExtractMembersTo(methodCallExpression.Object, members);
                ExtractMembersTo(methodCallExpression.Arguments, members);
                AddTo(methodCallExpression.Method, members);
                return;
            }
            case NewArrayExpression newArrayExpression:
            {
                ExtractMembersTo(newArrayExpression.Expressions, members);
                return;
            }
            case NewExpression newExpression:
            {
                ExtractMembersTo(newExpression.Arguments, members);
                AddTo(newExpression.Constructor, members);
                AddTo(newExpression.Members, members);
                return;
            }
            case ParameterExpression parameterExpression:
                // nothing
                return;
            case RuntimeVariablesExpression runtimeVariablesExpression:
            {
                ExtractMembersTo(runtimeVariablesExpression.Variables, members);
                return;
            }
            case SwitchExpression switchExpression:
            {
                AddTo(switchExpression.Comparison, members);
                ExtractMembersTo(switchExpression.SwitchValue, members);
                foreach (SwitchCase sc in switchExpression.Cases)
                {
                    ExtractMembersTo(sc.TestValues, members);
                    ExtractMembersTo(sc.Body, members);
                }
                ExtractMembersTo(switchExpression.DefaultBody, members);
                return;
            }
            case TryExpression tryExpression:
            {
                ExtractMembersTo(tryExpression.Body, members);
                foreach (CatchBlock cb in tryExpression.Handlers)
                {
                    ExtractMembersTo(cb.Variable, members);
                    ExtractMembersTo(cb.Filter, members);
                    ExtractMembersTo(cb.Body, members);
                }
                ExtractMembersTo(tryExpression.Fault, members);
                ExtractMembersTo(tryExpression.Finally, members);
                return;
            }
            case TypeBinaryExpression typeBinaryExpression:
            {
                AddTo(typeBinaryExpression.TypeOperand, members);
                ExtractMembersTo(typeBinaryExpression.Expression, members);
                return;
            }
            case UnaryExpression unaryExpression:
            {
                ExtractMembersTo(unaryExpression.Operand, members);
                AddTo(unaryExpression.Method, members);
                return;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(expression));
        }
    }

    public static List<MemberInfo> ExtractMembers(this Expression? expression)
    {
        List<MemberInfo> members = new();
        ExtractMembersTo(expression, members);
        return members;
    }

    public static Result<(ConstructorInfo Ctor, object?[] Arguments)> TryParseConstructor(Expression? expression)
    {
        if (expression is null)
            return new ArgumentNullException(nameof(expression));
        if (expression is NewExpression newExpression)
        {
            var ctor = newExpression.Constructor;
            var arguments = newExpression.Arguments;
            object?[] args = new object?[arguments.Count];
            for (var i = 0; i < arguments.Count; i++)
            {
                if (arguments[i] is ConstantExpression constantExpression)
                {
                    args[i] = constantExpression.Value;
                }
                else
                {
                    Debugger.Break();
                    return new ArgumentException("Expression did not contain a parsable Constructor", nameof(expression));
                }
            }
            return (ctor, args);
        }
        else if (expression is LambdaExpression lambdaExpression)
        {
            return TryParseConstructor(lambdaExpression.Body);
        }
        else
        {

            Debugger.Break();
            return new ArgumentException("Expression did not contain a parsable Constructor", nameof(expression));
        }
    }
}