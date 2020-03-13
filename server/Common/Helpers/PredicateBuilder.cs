using System;
using System.Linq.Expressions;

namespace Common.Helpers
{
	public static class PredicateBuilder
	{
		public static Expression<Func<T, bool>> True<T>() => f => true;

		public static Expression<Func<T, bool>> False<T>() => f => false;

		public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> currentExpression, Expression<Func<T, bool>> otherExpression) =>
			Expression.Lambda<Func<T, bool>>(Expression.OrElse(currentExpression.Body, Expression.Invoke(otherExpression, currentExpression.Parameters)), currentExpression.Parameters);

		public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> currentExpression, Expression<Func<T, bool>> otherExpression) =>
			Expression.Lambda<Func<T, bool>>(Expression.AndAlso(currentExpression.Body, Expression.Invoke(otherExpression, currentExpression.Parameters)), currentExpression.Parameters);
	}
}
