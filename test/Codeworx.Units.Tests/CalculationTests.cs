using System;
using System.Collections.Generic;
using Codeworx.Units.Defaults;
using Codeworx.Units.Defaults.DistanceDimension;
using Codeworx.Units.Defaults.TemperatureDimension;

namespace Codeworx.Units.Tests
{
    public class CalculationTests
    {
        [Theory]
        [MemberData(nameof(GetAddDistanceMeterTestData))]
        public void InterfaceAddCalculation(IDistance first, IDistance second, IDistance check)
        {
            var calculated = first + second;

            Assert.Equal(check, calculated);
        }

        [Theory]
        [MemberData(nameof(GetSubtractDistanceMeterTestData))]
        public void InterfaceSubtractCalculation(IDistance first, IDistance second, IDistance check)
        {
            var calculated = first - second;

            Assert.Equal(check, calculated);
        }

        [Fact]
        public void StructCalculations()
        {
            Meter meter = 4;
            Centimeter centimeter = 200;
            var calculatedAdd = meter + centimeter;

            Assert.Equal(new Meter(6), calculatedAdd);

            var calculatedSub = meter - centimeter;

            Assert.Equal(new Meter(2), calculatedSub);

            var calculatedDiv = meter / 2;

            Assert.Equal(new Meter(2M), calculatedDiv);

            var calculatedMul = meter * 2;

            Assert.Equal(new Meter(8), calculatedMul);

            var calculatedDivWithoutUnit = meter / centimeter;

            Assert.Equal(2, calculatedDivWithoutUnit);
        }

        public static IEnumerable<object[]> GetAddDistanceMeterTestData()
        {
            yield return new object[] { new Meter(1), new Centimeter(100), new Meter(2) };
            yield return new object[] { new Meter(1), new Kilometer(0.001M), new Meter(2) };
        }

        public static IEnumerable<object[]> GetSubtractDistanceMeterTestData()
        {
            yield return new object[] { new Meter(2), new Centimeter(100), new Meter(1) };
            yield return new object[] { new Meter(2), new Kilometer(0.001M), new Meter(1) };
        }
    }
}