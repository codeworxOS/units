using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Codeworx.Units.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Codeworx.Units.EntityFrameworkCore
{
    public class DimensionQueryReplacementPreprocessor : IQueryExpressionInterceptor
    {
        public static readonly DimensionQueryReplacementPreprocessor _instance;

        public static DimensionQueryReplacementPreprocessor Instance => _instance;

        static DimensionQueryReplacementPreprocessor()
        {
            _instance = new DimensionQueryReplacementPreprocessor();
        }

        Expression IQueryExpressionInterceptor.QueryCompilationStarting(Expression queryExpression, QueryExpressionEventData eventData)
        {
            DimensionExpressionVisitor visitor = new DimensionExpressionVisitor();

            var tmp = visitor.Visit(queryExpression);
            return tmp;
        }

        private class DimensionExpressionVisitor : ExpressionVisitor
        {
            private static ConcurrentDictionary<Type, MethodInfo> _parseMethods = new ConcurrentDictionary<Type, MethodInfo>();

            private static readonly MethodInfo _nullableMethod;
            private static readonly MethodInfo _defaultMethod;

            static DimensionExpressionVisitor()
            {
                _nullableMethod = typeof(NullableDimensionValue<>).GetMethod(nameof(NullableDimensionValue<IUnitBase>.GetDimension))!;
                _defaultMethod = typeof(DimensionValue<>).GetMethod(nameof(DimensionValue<IUnitBase>.GetDimension))!;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Object is MemberExpression property)
                {
                    if (node.Method.HasSameMetadataDefinitionAs(_defaultMethod))
                    {
                        var expression = GetDefaultParseExpression(property);

                        return expression;
                    }
                    else if (node.Method.HasSameMetadataDefinitionAs(_nullableMethod))
                    {
                        var expression = GetNullableParseExpression(property);

                        return expression;
                    }
                }

                return base.VisitMethodCall(node);
            }

            private Expression GetNullableParseExpression(MemberExpression unitPropertyExpression)
            {
                var unitPropertyValueExpression = Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Value));
                var unitPropertyUnitIdExpression = Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.UnitId));

                var unitPropertyValueValueExpression = Expression.Property(unitPropertyValueExpression, nameof(Nullable<Decimal>.Value));
                var unitPropertyValueHasValueExpression = Expression.Property(unitPropertyValueExpression, nameof(Nullable<decimal>.HasValue));

                var dimensionType = unitPropertyExpression.Type.GenericTypeArguments[0];
                var parseMethod = _parseMethods.GetOrAdd(dimensionType, GetParseMethod);

                var body = Expression.Condition(unitPropertyValueHasValueExpression, Expression.Call(null, parseMethod, unitPropertyUnitIdExpression, unitPropertyValueValueExpression), Expression.Convert(Expression.Constant(null), dimensionType));

                return body;
            }

            private Expression GetDefaultParseExpression(MemberExpression unitPropertyExpression)
            {
                var unitPropertyValueProperty = Expression.Property(unitPropertyExpression, nameof(DimensionValue<IUnitBase>.Value));
                var unitPropertyUnitIdExpression = Expression.Property(unitPropertyExpression, nameof(DimensionValue<IUnitBase>.UnitId));

                var dimensionType = unitPropertyExpression.Type.GenericTypeArguments[0];
                var parseMethod = _parseMethods.GetOrAdd(dimensionType, GetParseMethod);

                return Expression.Call(null, parseMethod, unitPropertyUnitIdExpression, unitPropertyValueProperty);
            }

            private MethodInfo GetParseMethod(Type type)
            {
                return type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, [typeof(string), typeof(decimal)])!;
            }
        }
    }
}