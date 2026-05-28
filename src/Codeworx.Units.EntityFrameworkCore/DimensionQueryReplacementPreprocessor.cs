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

        private DimensionQueryReplacementPreprocessor()
        {
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

            protected override Expression VisitBinary(BinaryExpression node)
            {
                if (node.Left.Type.IsAssignableTo(typeof(IUnitBase)) && node.Right.Type.IsAssignableTo(typeof(IUnitBase)))
                {
                    var member = typeof(IUnitBase).GetProperty(nameof(IUnitBase.BaseValue))!;

                    var newLeft = Expression.Property(node.Left, member);
                    var newRight = Expression.Property(node.Right, member);

                    switch (node.NodeType)
                    {
                        case ExpressionType.Equal:
                            return Expression.Equal(newLeft, newRight);
                        case ExpressionType.GreaterThan:
                            return Expression.GreaterThan(newLeft, newRight);
                        case ExpressionType.GreaterThanOrEqual:
                            return Expression.GreaterThanOrEqual(newLeft, newRight);
                        case ExpressionType.LessThan:
                            return Expression.LessThan(newLeft, newRight);
                        case ExpressionType.LessThanOrEqual:
                            return Expression.LessThanOrEqual(newLeft, newRight);
                        case ExpressionType.NotEqual:
                            return Expression.NotEqual(newLeft, newRight);
                    }
                }

                return base.VisitBinary(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Object is MemberExpression property)
                {
                    if (node.Method.HasSameMetadataDefinitionAs(_defaultMethod))
                    {
                        var expression = GetDefaultInitExpression(property);

                        return expression;
                    }
                    else if (node.Method.HasSameMetadataDefinitionAs(_nullableMethod))
                    {
                        var expression = GetNullableInitExpression(property);

                        return expression;
                    }
                }

                return base.VisitMethodCall(node);
            }

            private Expression GetNullableInitExpression(MemberExpression unitPropertyExpression)
            {
                var unitPropertyValueExpression = Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Value));
                var unitPropertyUnitIdExpression = Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.UnitId));

                var unitPropertyValueValueExpression = Expression.Property(unitPropertyValueExpression, nameof(Nullable<decimal>.Value));
                var unitPropertyValueHasValueExpression = Expression.Property(unitPropertyValueExpression, nameof(Nullable<decimal>.HasValue));

                var dimensionType = unitPropertyExpression.Type.GenericTypeArguments[0];
                var attribute = dimensionType.GetCustomAttribute<GeneralImplementationAttribute>();

                if (attribute != null)
                {
                    Expression baseExpression = Expression.Property(
                                                            Expression.Property(unitPropertyExpression, nameof(DimensionValue<IUnitBase>.Unit)),
                                                            nameof(UnitInformation.ConversionOffset));

                    baseExpression = Expression.Add(baseExpression, Expression.Property(Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Value)), nameof(Nullable<decimal>.Value)));
                    baseExpression = Expression.Multiply(baseExpression, Expression.Property(Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Unit)), nameof(UnitInformation.ConversionFactor)));
                    baseExpression = Expression.Divide(baseExpression, Expression.Property(Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Unit)), nameof(UnitInformation.ConversionDivisor)));

                    var init = Expression.MemberInit(
                        Expression.New(attribute.ImplementationType),
                        Expression.Bind(
                            attribute.ImplementationType.GetProperty(nameof(IUnitBase.Symbol))!,
                            Expression.Property(Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Unit)), nameof(UnitInformation.Symbol))),
                        Expression.Bind(
                            attribute.ImplementationType.GetProperty(nameof(IUnitBase.Value))!,
                            Expression.Property(Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Value)), nameof(Nullable<decimal>.Value))),
                        Expression.Bind(
                            attribute.ImplementationType.GetProperty(nameof(IUnitBase.BaseValue))!,
                            baseExpression)
                        );

                    var body = Expression.Condition(unitPropertyValueHasValueExpression, Expression.Convert(init, dimensionType), Expression.Constant(null, dimensionType));

                    return body;
                }

                return Expression.Constant(null, dimensionType);
            }

            private Expression GetDefaultInitExpression(MemberExpression unitPropertyExpression)
            {
                var dimensionType = unitPropertyExpression.Type.GenericTypeArguments[0];
                var attribute = dimensionType.GetCustomAttribute<GeneralImplementationAttribute>();

                if (attribute != null)
                {
                    Expression baseExpression = Expression.Property(
                                                            Expression.Property(unitPropertyExpression, nameof(DimensionValue<IUnitBase>.Unit)),
                                                            nameof(UnitInformation.ConversionOffset));

                    baseExpression = Expression.Add(baseExpression, Expression.Property(unitPropertyExpression, nameof(DimensionValue<IUnitBase>.Value)));
                    baseExpression = Expression.Multiply(baseExpression, Expression.Property(Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Unit)), nameof(UnitInformation.ConversionFactor)));
                    baseExpression = Expression.Divide(baseExpression, Expression.Property(Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Unit)), nameof(UnitInformation.ConversionDivisor)));

                    var init = Expression.MemberInit(
                        Expression.New(attribute.ImplementationType),
                        Expression.Bind(
                            attribute.ImplementationType.GetProperty(nameof(IUnitBase.Symbol))!,
                            Expression.Property(Expression.Property(unitPropertyExpression, nameof(DimensionValue<IUnitBase>.Unit)), nameof(UnitInformation.Symbol))),
                        Expression.Bind(
                            attribute.ImplementationType.GetProperty(nameof(IUnitBase.Value))!,
                            Expression.Property(unitPropertyExpression, nameof(DimensionValue<IUnitBase>.Value))),
                        Expression.Bind(
                            attribute.ImplementationType.GetProperty(nameof(IUnitBase.BaseValue))!,
                            baseExpression)
                        );

                    return Expression.Convert(init, dimensionType);
                }

                return Expression.Constant(null, dimensionType);
            }
        }
    }
}