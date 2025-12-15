using System;
using System.Collections.Generic;
using Codeworx.Units.Defaults;
using Codeworx.Units.Defaults.DistanceDimension;
using Codeworx.Units.Primitives;

namespace Codeworx.Units.Tests
{
    public class ParserTests
    {
        [Theory]
        [MemberData(nameof(GetDistanceFullStringParseData))]
        public void DistanceFullConversionTests(string input, Type targetType, IUnitBase check)
        {
            var parsed = IDistance.Parse(input);

            Assert.Equal(Math.Round(parsed.BaseValue, 5), Math.Round(check.BaseValue, 5));
            Assert.IsType(targetType, parsed);
        }

        [Theory]
        [MemberData(nameof(GetDistanceParseData))]
        public void DistanceConversionTests(decimal value, string symbol, Type targetType, IUnitBase check)
        {
            var parsed = IDistance.Parse(symbol, value);

            Assert.Equal(Math.Round(parsed.BaseValue, 5), Math.Round(check.BaseValue, 5));
            Assert.IsType(targetType, parsed);
        }

        public static IEnumerable<object[]> GetDistanceFullStringParseData()
        {
            yield return new object[] { "100 m", typeof(Meter), new Meter(100) };
            yield return new object[] { "100 cm", typeof(Centimeter), new Centimeter(100) };
            yield return new object[] { "100 mi", typeof(Mile), new Mile(100) };
        }

        public static IEnumerable<object[]> GetDistanceParseData()
        {
            yield return new object[] { 100M, "m", typeof(Meter), new Meter(100) };
            yield return new object[] { 100M, "cm", typeof(Centimeter), new Centimeter(100) };
            yield return new object[] { 100M, "mi", typeof(Mile), new Mile(100) };
        }
    }
}