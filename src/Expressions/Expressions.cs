using System.Diagnostics;
using System.Linq.Expressions;
using ScrubJay.Reflection.Comparison;

namespace ScrubJay.Reflection.Expressions;

public static partial class Expressions
{
    public static Func<TValue, TResult> CompileUnaryExpression<TValue, TResult>(
        Func<Expression, UnaryExpression> body)
    {
        ParameterExpression arg = Expression.Parameter(typeof(TValue), "arg");
        try
        {
            var lambda = Expression.Lambda<Func<TValue, TResult>>(body(arg), arg);
            return lambda.Compile();
        }
        catch (Exception ex)
        {
            // Return a function that throws the exception
            return _ => throw new InvalidOperationException(ex.Message);
        }
    }

    public static Func<T1, T2, TResult> CompileBinaryExpression<T1, T2, TResult>(Func<Expression, Expression, BinaryExpression> body)
    {
        ParameterExpression arg1 = Expression.Parameter(typeof(T1), "arg1");
        ParameterExpression arg2 = Expression.Parameter(typeof(T2), "arg2");
        try
        {
            var lambda = Expression.Lambda<Func<T1, T2, TResult>>(body(arg1, arg2), arg1, arg2);
            return lambda.Compile();
        }
        catch (Exception ex)
        {
            // Return a function that throws the exception
            return (_, _) => throw new InvalidOperationException(ex.Message);
        }
    }
}

    public sealed class ExpressionMembers : IReadOnlyCollection<MemberInfo>
    {
        private readonly HashSet<MemberInfo> _members = new(MemberInfoEqualityComparer.Default);

        public int Count => _members.Count;

        public void Add(MemberInfo? member)
        {
            if (member is not null)
            {
                _members.Add(member);
            }
        }

        public void AddFrom(IEnumerable<Expression?>? expressions)
        {
            if (expressions is not null)
            {
                foreach (Expression? expression in expressions)
                {
                    AddFrom(expression);
                }
            }
        }

        public void AddFrom(Expression? expression)
        {
            switch (expression)
            {
                case null:
                    return;
                case BinaryExpression binaryExpression:
                    AddFrom(binaryExpression.Left);
                    Add(binaryExpression.Method);
                    AddFrom(binaryExpression.Right);
                    return;
                case BlockExpression blockExpression:
                    AddFrom(blockExpression.Expressions);
                    return;
                case ConditionalExpression conditionalExpression:
                    AddFrom(conditionalExpression.Test);
                    AddFrom(conditionalExpression.IfTrue);
                    AddFrom(conditionalExpression.IfFalse);
                    return;
                case ConstantExpression constantExpression:
                case DebugInfoExpression debugInfoExpression:
                case DefaultExpression defaultExpression:
                case DynamicExpression dynamicExpression:
                case GotoExpression gotoExpression:
                    return;
                case IndexExpression indexExpression:
                    Add(indexExpression.Indexer);
                    return;
                case InvocationExpression invocationExpression:
                    AddFrom(invocationExpression.Expression);
                    return;
                //case Expression<TDelegate> delegateExpression:
                case LabelExpression labelExpression:
                    return;
                case LambdaExpression lambdaExpression:
                    AddFrom(lambdaExpression.Body);
                    return;
                case ListInitExpression listInitExpression:
                    AddFrom(listInitExpression.NewExpression);
                    return;
                case LoopExpression loopExpression:
                    return;
                case MemberExpression memberExpression:
                    Add(memberExpression.Member);
                    return;
                case MemberInitExpression memberInitExpression:
                    AddFrom(memberInitExpression.NewExpression);
                    return;
                case MethodCallExpression methodCallExpression:
                    Add(methodCallExpression.Method);
                    return;
                case NewArrayExpression newArrayExpression:
                    return;
                case NewExpression newExpression:
                    Add(newExpression.Constructor);
                    return;
                case ParameterExpression parameterExpression:
                case RuntimeVariablesExpression runtimeVariablesExpression:
                case SwitchExpression switchExpression:
                case TryExpression tryExpression:
                case TypeBinaryExpression typeBinaryExpression:
                    return;
                case UnaryExpression unaryExpression:
                    Add(unaryExpression.Method);
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(expression));
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<MemberInfo> GetEnumerator() => _members.GetEnumerator();
    }

partial class Expressions
{
    public static IReadOnlyCollection<MemberInfo> FindMembers(Expression expression)
    {
        var members = new ExpressionMembers();
        members.AddFrom(expression);
        return members;
    }
    
    public static IReadOnlyCollection<MemberInfo> FindMembers<T>(Expression<Action<T>> expression)
    {
        return FindMembers((Expression)expression);
    }

    public static IReadOnlyCollection<MemberInfo> FindMembers<T>(Expression<Func<T>> expression)
    {
        return FindMembers((Expression)expression);
    }

    public static IReadOnlyCollection<MemberInfo> FindMembers<T>(Expression<Func<T, object?>> expression)
    {
        return FindMembers((Expression)expression);
    }
}