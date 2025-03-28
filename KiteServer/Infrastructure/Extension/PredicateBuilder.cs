﻿using System.Linq.Expressions;

namespace Infrastructure.Extension;

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> True<T>()
    {
        return p => true;
    }
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> express, Expression<Func<T, bool>> predicate)
    {
        return express.Compose(predicate, Expression.AndAlso);
    }
    public static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> express, bool condition, Expression<Func<T, bool>> predicate)
    {
        if (!condition) return express;
        return express.Compose(predicate, Expression.AndAlso);
    }
    internal static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
    {
        var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }
}

internal class ParameterRebinder : ExpressionVisitor
{
    private readonly Dictionary<ParameterExpression, ParameterExpression> map;

    public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
    {
        this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
    }

    public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
    {
        return new ParameterRebinder(map).Visit(exp);
    }
    protected override Expression VisitParameter(ParameterExpression p)
    {
        ParameterExpression replacement;
        if (map.TryGetValue(p, out replacement)) p = replacement;
        return base.VisitParameter(p);
    }
}
