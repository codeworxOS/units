using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using Codeworx.Units.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;

namespace Codeworx.Units.EntityFrameworkCore
{
    public class DimensionQueryReplacementPreprocessor : IQueryExpressionInterceptor
    {
        public static readonly DimensionQueryReplacementPreprocessor _instance;

        static DimensionQueryReplacementPreprocessor()
        {
            _instance = new DimensionQueryReplacementPreprocessor();

        }

        private DimensionQueryReplacementPreprocessor()
        {
        }

        public static DimensionQueryReplacementPreprocessor Instance => _instance;
        Expression IQueryExpressionInterceptor.QueryCompilationStarting(Expression queryExpression, QueryExpressionEventData eventData)
        {
            BaseValueExpressionVisitor baseValueVisitor = new BaseValueExpressionVisitor();
            var result = baseValueVisitor.Visit(queryExpression);

            DimensionExpressionVisitor visitor = new DimensionExpressionVisitor(baseValueVisitor.DimensionProperties);
            result = visitor.Visit(result);

            return result;
        }

        private class BaseValueExpressionVisitor : ExpressionVisitor
        {
            private List<PropertyInfo> _dimensionProperties;

            private static readonly MethodInfo _orderByMethod;
            private static readonly MethodInfo _orderByDescendingMethod;
            private static readonly MethodInfo _thenByMethod;
            private static readonly MethodInfo _thenByDescendingMethod;

            public BaseValueExpressionVisitor()
            {
                _dimensionProperties = new List<PropertyInfo>();
            }

            static BaseValueExpressionVisitor()
            {
                Expression<Func<IQueryable<object>, IOrderedQueryable<object>>> exp = p => p.OrderBy(x => 1);
                _orderByMethod = ((MethodCallExpression)exp.Body).Method.GetGenericMethodDefinition();

                Expression<Func<IQueryable<object>, IOrderedQueryable<object>>> exp2 = p => p.OrderByDescending(x => 1);
                _orderByDescendingMethod = ((MethodCallExpression)exp2.Body).Method.GetGenericMethodDefinition();

                Expression<Func<IOrderedQueryable<object>, IOrderedQueryable<object>>> exp3 = p => p.ThenBy(x => 1);
                _thenByMethod = ((MethodCallExpression)exp3.Body).Method.GetGenericMethodDefinition();

                Expression<Func<IOrderedQueryable<object>, IOrderedQueryable<object>>> exp4 = p => p.ThenByDescending(x => 1);
                _thenByDescendingMethod = ((MethodCallExpression)exp4.Body).Method.GetGenericMethodDefinition();
            }

            public IReadOnlyList<PropertyInfo> DimensionProperties => _dimensionProperties.ToImmutableList();

            protected override Expression VisitBinary(BinaryExpression node)
            {
                if (node.Left.Type.IsAssignableTo(typeof(IUnitBase)) && node.Right.Type.IsAssignableTo(typeof(IUnitBase)))
                {
                    var newLeft = CleanupBaseUnitExpression(node.Left);
                    var newRight = CleanupBaseUnitExpression(node.Right);

                    switch (node.NodeType)
                    {
                        case ExpressionType.Equal:
                            return Expression.Equal(newLeft, newRight);
                        case ExpressionType.NotEqual:
                            return Expression.NotEqual(newLeft, newRight);
                        case ExpressionType.GreaterThan:
                            return Expression.GreaterThan(newLeft, newRight);
                        case ExpressionType.GreaterThanOrEqual:
                            return Expression.GreaterThanOrEqual(newLeft, newRight);
                        case ExpressionType.LessThan:
                            return Expression.LessThan(newLeft, newRight);
                        case ExpressionType.LessThanOrEqual:
                            return Expression.LessThanOrEqual(newLeft, newRight);
                    }
                }

                return base.VisitBinary(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.IsGenericMethod)
                {
                    var method = node.Method.GetGenericMethodDefinition();
                    if (method == _orderByMethod || method == _orderByDescendingMethod || method == _thenByMethod || method == _thenByDescendingMethod)
                    {
                        if (node.Arguments[1] is UnaryExpression unary &&
                            unary.Operand is LambdaExpression lambda &&
                            lambda.Body is MemberExpression member &&
                            member.Type.IsAssignableTo(typeof(IUnitBase)))
                        {
                            if (member.Member is PropertyInfo propertyInfo)
                            {
                                _dimensionProperties.Add(propertyInfo);
                            }

                            var newBody = CleanupBaseUnitExpression(member);
                            var newLambda = Expression.Lambda(newBody, lambda.Parameters);
                            var newMethod = method.MakeGenericMethod(lambda.Parameters[0].Type, newBody.Type);
                            return Expression.Call(newMethod, Visit(node.Arguments[0]), Expression.Quote(newLambda));
                        }
                    }
                }

                return base.VisitMethodCall(node);
            }

            private Expression CleanupBaseUnitExpression(Expression baseExpression)
            {
                var member = typeof(IUnitBase).GetProperty(nameof(IUnitBase.BaseValue));

                Expression result = baseExpression;

                if (result is UnaryExpression unary)
                {
                    result = unary.Operand;
                }

                if (result is ConstantExpression constant)
                {
                    if (constant.Value is IUnitBase unitBase)
                    {
                        return Expression.Constant(unitBase.BaseValue, typeof(decimal));
                    }
                    else if (constant.Value == null)
                    {
                        return Expression.Constant(null, typeof(decimal?));
                    }
                }

                if (result is MemberExpression memberExpression && memberExpression.Member is PropertyInfo propertyInfo)
                {
                    _dimensionProperties.Add(propertyInfo);
                }

                result = Expression.Property(result, result.Type.IsInterface ? typeof(IUnitBase) : result.Type, nameof(IUnitBase.BaseValue));

                return result;
            }
        }

        private class DimensionExpressionVisitor : ExpressionVisitor
        {
            private static readonly MethodInfo _defaultMethod;
            private static readonly MethodInfo _nullableMethod;
            private static ConcurrentDictionary<Type, MethodInfo> _parseMethods = new ConcurrentDictionary<Type, MethodInfo>();
            private readonly IReadOnlyList<PropertyInfo> _baseValueProperties;

            static DimensionExpressionVisitor()
            {
                _nullableMethod = typeof(NullableDimensionValue<>).GetMethod(nameof(NullableDimensionValue<IUnitBase>.GetDimension))!;
                _defaultMethod = typeof(DimensionValue<>).GetMethod(nameof(DimensionValue<IUnitBase>.GetDimension))!;
            }

            public DimensionExpressionVisitor(IEnumerable<PropertyInfo> baseValueProperties)
            {
                _baseValueProperties = baseValueProperties.ToImmutableList();
            }

            protected override MemberBinding VisitMemberBinding(MemberBinding node)
            {
                if (node is MemberAssignment assignment && _baseValueProperties.Contains(node.Member))
                {
                    if (assignment.Expression is MethodCallExpression methodCallExpression)
                    {
                        var expression = VisitMethodCall(methodCallExpression, true);
                        return Expression.Bind(node.Member, expression);
                    }
                }
                return base.VisitMemberBinding(node);
            }

            protected Expression VisitMethodCall(MethodCallExpression node, bool withBaseValue)
            {
                if (node.Object is MemberExpression property)
                {
                    if (node.Method.HasSameMetadataDefinitionAs(_defaultMethod))
                    {
                        var expression = GetDefaultInitExpression(property, withBaseValue);
                        return expression;
                    }
                    else if (node.Method.HasSameMetadataDefinitionAs(_nullableMethod))
                    {
                        var expression = GetNullableInitExpression(property, withBaseValue);
                        return expression;
                    }
                }

                return base.VisitMethodCall(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                return VisitMethodCall(node, false);
            }

            private Expression GetDefaultInitExpression(MemberExpression unitPropertyExpression, bool withBaseValue)
            {
                var dimensionType = unitPropertyExpression.Type.GenericTypeArguments[0];
                var attribute = dimensionType.GetCustomAttribute<GeneralImplementationAttribute>();

                if (attribute != null)
                {
                    var bindings = new List<MemberBinding>()
                    {
                         Expression.Bind(
                            attribute.ImplementationType.GetProperty(nameof(IUnitBase.Key))!,
                            Expression.Property(unitPropertyExpression, nameof(DimensionValue<IUnitBase>.UnitId))),
                        Expression.Bind(
                            attribute.ImplementationType.GetProperty(nameof(IUnitBase.Value))!,
                            Expression.Property(unitPropertyExpression, nameof(DimensionValue<IUnitBase>.Value))),
                    };


                    if (withBaseValue)
                    {
                        Expression baseExpression = Expression.Property(
                                                                Expression.Property(unitPropertyExpression, nameof(DimensionValue<IUnitBase>.Unit)),
                                                                nameof(UnitInformation.ConversionOffset));

                        baseExpression = Expression.Add(baseExpression, Expression.Property(unitPropertyExpression, nameof(DimensionValue<IUnitBase>.Value)));
                        baseExpression = Expression.Multiply(baseExpression, Expression.Property(Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Unit)), nameof(UnitInformation.ConversionFactor)));
                        baseExpression = Expression.Divide(baseExpression, Expression.Property(Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Unit)), nameof(UnitInformation.ConversionDivisor)));

                        bindings.Add(Expression.Bind(
                            attribute.ImplementationType.GetProperty(nameof(IUnitBase.BaseValue))!,
                            baseExpression));
                    }

                    var init = Expression.MemberInit(
                        Expression.New(attribute.ImplementationType),
                        bindings);

                    return Expression.Convert(init, dimensionType);
                }

                return Expression.Constant(null, dimensionType);
            }

            private Expression GetNullableInitExpression(MemberExpression unitPropertyExpression, bool withBaseValue)
            {
                var unitPropertyValueExpression = Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Value));
                var unitPropertyUnitIdExpression = Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.UnitId));

                var unitPropertyValueValueExpression = Expression.Property(unitPropertyValueExpression, nameof(Nullable<decimal>.Value));
                var unitPropertyValueHasValueExpression = Expression.NotEqual(unitPropertyValueExpression, Expression.Constant(null, typeof(decimal?)));

                var dimensionType = unitPropertyExpression.Type.GenericTypeArguments[0];
                var attribute = dimensionType.GetCustomAttribute<GeneralImplementationAttribute>();

                if (attribute != null)
                {
                    var bindings = new List<MemberBinding>()
                    {
                        Expression.Bind(
                            attribute.ImplementationType.GetProperty(nameof(IUnitBase.Key))!,
                            Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.UnitId))),
                        Expression.Bind(
                            attribute.ImplementationType.GetProperty(nameof(IUnitBase.Value))!,
                            Expression.Property(Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Value)), nameof(Nullable<decimal>.Value))),
                    };

                    if (withBaseValue)
                    {
                        Expression baseExpression = Expression.Property(
                                                                Expression.Property(unitPropertyExpression, nameof(DimensionValue<IUnitBase>.Unit)),
                                                                nameof(UnitInformation.ConversionOffset));

                        baseExpression = Expression.Add(baseExpression, Expression.Property(Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Value)), nameof(Nullable<decimal>.Value)));
                        baseExpression = Expression.Multiply(baseExpression, Expression.Property(Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Unit)), nameof(UnitInformation.ConversionFactor)));
                        baseExpression = Expression.Divide(baseExpression, Expression.Property(Expression.Property(unitPropertyExpression, nameof(NullableDimensionValue<IUnitBase>.Unit)), nameof(UnitInformation.ConversionDivisor)));

                        bindings.Add(Expression.Bind(
                           attribute.ImplementationType.GetProperty(nameof(IUnitBase.BaseValue))!,
                           baseExpression)
                        );
                    }

                    var init = Expression.MemberInit(
                        Expression.New(attribute.ImplementationType),
                        bindings
                        );

                    var body = Expression.Condition(unitPropertyValueHasValueExpression, Expression.Convert(init, dimensionType), Expression.Constant(null, dimensionType));

                    return body;
                }

                return Expression.Constant(null, dimensionType);
            }
        }
    }
}