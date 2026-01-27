using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Codeworx.Units.Cli.Data
{
    public static class DimensionExtensions
    {
        public static JsonUnit[]? GetConversionPath(this JsonDimension dimension, JsonUnit sourceUnit, JsonUnit targetUnit)
        {
            List<JsonUnit[]> conversionPaths = new List<JsonUnit[]>
            {
                new[] { sourceUnit },
            };

            var units = dimension.Units.Values.ToList();

            while (conversionPaths.Any() && !conversionPaths.Any(c => c.Last() == targetUnit))
            {
                List<JsonUnit[]> newPaths = new List<JsonUnit[]>();
                var validLookups = units.Except(conversionPaths.SelectMany(c => c)).ToList();

                foreach (var path in conversionPaths)
                {
                    var lastEntry = path.Last();

                    foreach (var possibleConversion in validLookups.Where(d => (d.Conversions?.ContainsKey(lastEntry.Name) ?? false) || (lastEntry.Conversions?.ContainsKey(d.Name) ?? false)))
                    {
                        var newPath = path.ToList();
                        newPath.Add(possibleConversion);
                        newPaths.Add(newPath.ToArray());
                    }
                }

                conversionPaths = newPaths;
            }

            conversionPaths = conversionPaths.Where(c => c.Last() == targetUnit).ToList();

            return conversionPaths.OrderBy(d =>
            {
                var flag = d.Select(c => c.System ?? UnitSystem.Both).Aggregate((f, s) => f | s);
                return flag;
            }
            ).FirstOrDefault();
        }

        public static ExpressionSyntax GetConversionExpression(this JsonUnit[] conversionPath)
        {
            ExpressionSyntax conversionExpression = SyntaxFactory.IdentifierName("Value");

            for (int idx = 1; idx < conversionPath.Length; idx++)
            {
                var before = conversionPath[idx - 1];
                var current = conversionPath[idx];

                if (before.Conversions != null && before.Conversions.ContainsKey(current.Name))
                {
                    if (before.Conversions[current.Name].Offset != 0)
                    {
                        if (conversionExpression is not IdentifierNameSyntax && conversionExpression is not ParenthesizedExpressionSyntax)
                        {
                            conversionExpression = SyntaxFactory.ParenthesizedExpression(conversionExpression);
                        }

                        var offset = before.Conversions[current.Name].Offset;
                        if (offset < 0)
                        {
                            conversionExpression = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, conversionExpression, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(offset * -1)));
                        }
                        else
                        {
                            conversionExpression = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, conversionExpression, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(offset)));
                        }
                        conversionExpression = SyntaxFactory.ParenthesizedExpression(conversionExpression);
                    }

                    if (before.Conversions[current.Name].Factor != 1)
                    {
                        conversionExpression = SyntaxFactory.BinaryExpression(SyntaxKind.MultiplyExpression, conversionExpression, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(before.Conversions[current.Name].Factor)));
                    }

                    if (before.Conversions[current.Name].Divisor != 1)
                    {
                        conversionExpression = SyntaxFactory.BinaryExpression(SyntaxKind.DivideExpression, conversionExpression, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(before.Conversions[current.Name].Divisor)));
                    }
                }
                else
                {
                    if (current.Conversions[before.Name].Factor != 1)
                    {
                        conversionExpression = SyntaxFactory.BinaryExpression(SyntaxKind.DivideExpression, conversionExpression, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(current.Conversions[before.Name].Factor)));
                    }

                    if (current.Conversions[before.Name].Divisor != 1)
                    {
                        conversionExpression = SyntaxFactory.BinaryExpression(SyntaxKind.MultiplyExpression, conversionExpression, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(current.Conversions[before.Name].Divisor)));
                    }

                    if (current.Conversions[before.Name].Offset != 0)
                    {
                        if (conversionExpression is not IdentifierNameSyntax && conversionExpression is not ParenthesizedExpressionSyntax)
                        {
                            conversionExpression = SyntaxFactory.ParenthesizedExpression(conversionExpression);
                        }

                        var offset = current.Conversions[before.Name].Offset;
                        if (offset < 0)
                        {
                            conversionExpression = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, conversionExpression, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(offset * -1)));
                        }
                        else
                        {
                            conversionExpression = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, conversionExpression, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(offset)));
                        }

                        conversionExpression = SyntaxFactory.ParenthesizedExpression(conversionExpression);
                    }
                }
            }

            while (GetValidSimplification(conversionExpression) is BinaryExpressionSyntax exp && exp != null)
            {
                if (exp.Left is BinaryExpressionSyntax exp_leftPart &&
                  exp_leftPart.Right is LiteralExpressionSyntax expt_lit_left && expt_lit_left.Token.Value is decimal valLeft &&
                  exp.Right is LiteralExpressionSyntax exp_lit_Right && exp_lit_Right.Token.Value is decimal valRight)
                {
                    if (exp_leftPart.Kind() == SyntaxKind.MultiplyExpression)
                    {
                        if (exp.Kind() == SyntaxKind.MultiplyExpression)
                        {
                            var value = valLeft * valRight;

                            conversionExpression = conversionExpression.ReplaceNode(exp, SyntaxFactory.BinaryExpression(SyntaxKind.MultiplyExpression, exp_leftPart.Left, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value))));
                        }
                        else
                        {
                            var value = valLeft / valRight;

                            conversionExpression = conversionExpression.ReplaceNode(exp, SyntaxFactory.BinaryExpression(SyntaxKind.MultiplyExpression, exp_leftPart.Left, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value))));
                        }
                    }
                    else
                    {
                        if (exp.Kind() == SyntaxKind.MultiplyExpression)
                        {
                            var value = valLeft / valRight;

                            conversionExpression = conversionExpression.ReplaceNode(exp, SyntaxFactory.BinaryExpression(SyntaxKind.DivideExpression, exp_leftPart.Left, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value))));
                        }
                        else
                        {
                            var value = valLeft * valRight;

                            conversionExpression = conversionExpression.ReplaceNode(exp, SyntaxFactory.BinaryExpression(SyntaxKind.DivideExpression, exp_leftPart.Left, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value))));
                        }
                    }
                }
            }

            return conversionExpression;
        }

        private static BinaryExpressionSyntax? GetValidSimplification(ExpressionSyntax conversionExpression)
        {
            var validEntries = conversionExpression.DescendantNodesAndSelf().OfType<BinaryExpressionSyntax>()
              .Where(d => IsValidBinaryExpression(d))
              .Where(d => IsValidExpression(d.Left) && IsLiteral(d.Right));

            return validEntries.LastOrDefault();

            bool IsValidBinaryExpression(BinaryExpressionSyntax d)
            {
                return d.IsKind(SyntaxKind.DivideExpression) || d.IsKind(SyntaxKind.MultiplyExpression);
            }

            bool IsValidExpression(ExpressionSyntax exp)
            {
                if (exp is BinaryExpressionSyntax binaryExp && IsValidBinaryExpression(binaryExp) && IsLiteral(binaryExp.Right))
                {
                    return true;
                }

                return false;
            }

            bool IsLiteral(ExpressionSyntax exp)
            {
                if (exp is LiteralExpressionSyntax literal)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
