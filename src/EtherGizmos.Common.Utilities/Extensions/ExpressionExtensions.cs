using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.Common.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Expression{TDelegate}"/>.
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// Fetches property information from a property expression.
    /// </summary>
    /// <typeparam name="TSource">The type of entity.</typeparam>
    /// <typeparam name="TMember">The type of entity member.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>The property info of the selected property.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static PropertyInfo GetPropertyInfo<TSource, TMember>(this Expression<Func<TSource, TMember>> @this)
    {
        //Cover each possible case of expression body
        MemberExpression? member = null;
        switch (@this.Body)
        {
            case UnaryExpression expression:
                //Handle convert (happens with GUID)
                if (expression.NodeType == ExpressionType.Convert)
                    if (expression.Operand is MemberExpression unaryMember)
                        member = unaryMember;
                break;

            case MemberExpression expression:
                //Handle basic member
                member = expression;
                break;
        }

        //Ensure there is a member being selected
        if (member is null)
        {
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a method, not a property.",
                @this.ToString()));
        }

        //Ensure that member has property information
        if (member.Member is not PropertyInfo property)
        {
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a field, not a property.",
                @this.ToString()));
        }

        return property;
    }

    /// <summary>
    /// Converts the expression into a path, such as Values[2].Id.
    /// </summary>
    /// <typeparam name="TSource">The type of entity.</typeparam>
    /// <param name="this">The expression selecting a property.</param>
    /// <returns>The expression path.</returns>
    public static string GetPath<TSource>(this Expression<Func<TSource, object?>> @this)
    {
        var path = GetExpressionPath(@this.Body);
        return path ?? @this.ToString();
    }

    private static string? GetExpressionPath(Expression? expression)
    {
        return expression switch
        {
            UnaryExpression unaryExpression when unaryExpression.NodeType == ExpressionType.Convert => GetExpressionPath(unaryExpression.Operand),
            MemberExpression member => GetMemberPath(member),
            MethodCallExpression method => GetMethodCallPath(method),
            ParameterExpression => null, //The initial argument of the expression (e.g., e), we don't want this
            _ => expression?.ToString(),
        };
    }

    private static string GetMemberPath(MemberExpression expression)
    {
        var prefix = GetExpressionPath(expression.Expression);
        var postfix = expression.Member.Name;

        return prefix is not null
            ? $"{prefix}.{postfix}"
            : postfix;
    }

    private static string GetMethodCallPath(MethodCallExpression expression)
    {
        //Handle method call (e.g., accessing an element of a collection)
        if (expression.Object is MemberExpression memberExpression)
        {
            var path = GetMemberPath(memberExpression);
            var index = GetIndexFromMethodCall(expression);

            //If the index is a string (e.g., dictionary), escape any double quotes in the accessor
            return index is string stringIndex
                ? $"{path}[\"{stringIndex.Replace("\"", "\\\"")}\"]"
                : $"{path}[{index}]";
        }

        //If the method points to another type of expression, return it as a string, as we do not know how to handle it
        return expression.ToString();
    }

    private static object? GetIndexFromMethodCall(MethodCallExpression methodCallExpression)
    {
        //Assuming the method called is an indexer (e.g., Values[index])
        //Extract the index from the method call arguments
        if (methodCallExpression.Arguments.Count == 1)
        {
            switch (methodCallExpression.Arguments[0])
            {
                case ConstantExpression constant:
                    return constant.Value;

                case MemberExpression member:
                    return GetValueFromExpression(member);
            }
        }

        throw new ArgumentException("Unsupported method call expression.");
    }

    private static object? GetValueFromExpression(MemberExpression expression)
    {
        object? extractValue;
        if (expression.Expression is ConstantExpression constant)
        {
            //Expression is a member of a constant value
            extractValue = constant.Value;
        }
        else if (expression.Expression is MemberExpression member)
        {
            //Expression is a member of another object, so extract the value recursively
            extractValue = GetValueFromExpression(member);
        }
        else
        {
            throw new ArgumentException("Unsupported method call expression.");
        }

        switch (expression.Member)
        {
            case PropertyInfo property:
                //Extract the property from the constant
                return property.GetValue(extractValue);

            case FieldInfo field:
                //Extract the field from the constant
                return field.GetValue(extractValue);

            default:
                throw new ArgumentException("Unsupported method call expression.");
        }
    }
}
