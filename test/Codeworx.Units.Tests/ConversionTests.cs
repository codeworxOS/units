using System;
using System.Collections.Generic;
using Codeworx.Units.Defaults;
using Codeworx.Units.Defaults.AreaDimension;
using Codeworx.Units.Defaults.DistanceDimension;
using Codeworx.Units.Defaults.MassDimension;
using Codeworx.Units.Defaults.TemperatureDimension;
using Codeworx.Units.Defaults.VolumeDimension;

namespace Codeworx.Units.Tests
{
    public class ConversionTests
    {
        [Theory]
        [MemberData(nameof(GetMetricDistanceConversionData))]
        [MemberData(nameof(GetImperialDistanceConversionData))]
        public void DistanceConversionTests(IDistance original, IDistance check)
        {
            var converted = original.ToUnit(check.Symbol);

            Assert.Equal(Math.Round(check.Value, 5), Math.Round(converted.Value, 5));
        }

        [Theory]
        [MemberData(nameof(GetMetricAreaConversionData))]
        [MemberData(nameof(GetImperialAreaConversionData))]
        public void AreaConversionTests(IArea original, IArea check)
        {
            var converted = original.ToUnit(check.Symbol);

            Assert.Equal(Math.Round(check.Value, 5), Math.Round(converted.Value, 5));
        }

        [Theory]
        [MemberData(nameof(GetMetricVolumeConversionData))]
        [MemberData(nameof(GetImperialVolumeConversionData))]
        public void VolumeConversionTests(IVolume original, IVolume check)
        {
            var converted = original.ToUnit(check.Symbol);

            Assert.Equal(Math.Round(check.Value, 5), Math.Round(converted.Value, 5));
        }

        [Theory]
        [MemberData(nameof(GetTemperatureCelsiusToConversionData))]
        [MemberData(nameof(GetTemperatureKelvinToConversionData))]
        [MemberData(nameof(GetTemperatureFahrenheitToConversionData))]
        public void TemperatureConversionTests(ITemperature original, ITemperature check)
        {
            var converted = original.ToUnit(check.Symbol);

            Assert.Equal(Math.Round(check.Value, 5), Math.Round(converted.Value, 5));
        }

        public static IEnumerable<object[]> GetMetricDistanceConversionData()
        {
            yield return new object[] { new Meter(1), new Centimeter(100) };
            yield return new object[] { new Meter(1), new Kilometer(0.001M) };
            yield return new object[] { new Meter(1000), new Kilometer(1M) };
            yield return new object[] { new Meter(1), new Millimeter(1000) };
            yield return new object[] { new Meter(1), new Yard(1.0936132983M) };
            yield return new object[] { new Meter(1), new Feet(3.280839895M) };
            yield return new object[] { new Meter(1), new Inch(39.37007874M) };
            yield return new object[] { new Meter(1000), new Mile(0.6213711922M) };
        }

        public static IEnumerable<object[]> GetImperialDistanceConversionData()
        {
            yield return new object[] { new Yard(1), new Inch(36M) };
            yield return new object[] { new Yard(1), new Feet(3M) };
            yield return new object[] { new Yard(1), new Meter(0.9144M) };
            yield return new object[] { new Yard(1000), new Mile(0.5681806818M) };
            yield return new object[] { new Mile(1), new Yard(1760) };
            yield return new object[] { new Mile(1), new Feet(5280) };
            yield return new object[] { new Mile(1), new Inch(63360) };
        }

        public static IEnumerable<object[]> GetTemperatureCelsiusToConversionData()
        {
            yield return new object[] { new Fahrenheit(0), new Kelvin(255.37222222M) };
            yield return new object[] { new Fahrenheit(0), new Celsius(-17.777777778M) };
            yield return new object[] { new Fahrenheit(100), new Kelvin(310.92777778M) };
            yield return new object[] { new Fahrenheit(100), new Celsius(37.777777778M) };
            yield return new object[] { new Fahrenheit(-10), new Kelvin(249.81666667M) };
            yield return new object[] { new Fahrenheit(-10), new Celsius(-23.333333333M) };
        }

        public static IEnumerable<object[]> GetTemperatureFahrenheitToConversionData()
        {
            yield return new object[] { new Celsius(0), new Kelvin(273.15M) };
            yield return new object[] { new Celsius(0), new Fahrenheit(32M) };
            yield return new object[] { new Celsius(100), new Kelvin(373.15M) };
            yield return new object[] { new Celsius(100), new Fahrenheit(212M) };
            yield return new object[] { new Celsius(-10), new Kelvin(263.15M) };
            yield return new object[] { new Celsius(-10), new Fahrenheit(14M) };
        }

        public static IEnumerable<object[]> GetTemperatureKelvinToConversionData()
        {
            yield return new object[] { new Kelvin(0), new Celsius(-273.15M) };
            yield return new object[] { new Kelvin(0), new Fahrenheit(-459.67M) };
            yield return new object[] { new Kelvin(10), new Celsius(-263.15M) };
            yield return new object[] { new Kelvin(10), new Fahrenheit(-441.67M) };
            yield return new object[] { new Kelvin(100), new Celsius(-173.15M) };
            yield return new object[] { new Kelvin(100), new Fahrenheit(-279.67M) };
            yield return new object[] { new Kelvin(300), new Celsius(26.85M) };
            yield return new object[] { new Kelvin(300), new Fahrenheit(80.33M) };
        }

        public static IEnumerable<object[]> GetMetricAreaConversionData()
        {
            yield return new object[] { new SquareMeter(0), new SquareCentimeter(0) };
            yield return new object[] { new SquareMeter(0), new SquareInch(0) };
            yield return new object[] { new SquareMeter(10), new SquareMillimeter(10000000) };
            yield return new object[] { new SquareMeter(10), new SquareCentimeter(100000) };
            yield return new object[] { new SquareMeter(10), new SquareKilometer(0.00001M) };
            yield return new object[] { new SquareMeter(10), new SquareInch(15500.031M) };
            yield return new object[] { new SquareMeter(10), new SquareFeet(107.63910M) };
            yield return new object[] { new SquareMeter(10), new Acre(0.00247105M) };
        }

        public static IEnumerable<object[]> GetImperialAreaConversionData()
        {
            yield return new object[] { new SquareFeet(0), new SquareCentimeter(0) };
            yield return new object[] { new SquareFeet(0), new SquareInch(0) };
            yield return new object[] { new SquareFeet(10), new SquareMillimeter(929030.4M) };
            yield return new object[] { new SquareFeet(10), new SquareCentimeter(9290.304M) };
            yield return new object[] { new SquareFeet(10), new SquareMeter(0.9290304M) };
            yield return new object[] { new SquareFeet(10), new SquareKilometer(0.000000929030399M) };
            yield return new object[] { new SquareFeet(10), new SquareInch(1440) };
            yield return new object[] { new SquareFeet(10), new Acre(0.0002295684M) };
        }

        public static IEnumerable<object[]> GetMetricVolumeConversionData()
        {
            yield return new object[] { new CubicMeter(0), new Gallon(0) };
            yield return new object[] { new CubicMeter(0), new CubicCentimeter(0) };
            yield return new object[] { new CubicMeter(10), new CubicCentimeter(10000000) };
            yield return new object[] { new CubicMeter(10), new Liter(10000) };
            yield return new object[] { new CubicMeter(10), new CubicFoot(353.14666721M) };
            yield return new object[] { new CubicMeter(10), new CubicInch(610237.44095M) };
            yield return new object[] { new CubicMeter(10), new Gallon(2641.7205236M) };
            yield return new object[] { new CubicMeter(10), new Quart(10566.882094M) };
        }

        public static IEnumerable<object[]> GetImperialVolumeConversionData()
        {
            yield return new object[] { new Gallon(0), new Quart(0) };
            yield return new object[] { new Gallon(0), new CubicMeter(0) };
            yield return new object[] { new Gallon(10), new CubicCentimeter(37854.11784M) };
            yield return new object[] { new Gallon(10), new CubicMeter(0.0378541178M) };
            yield return new object[] { new Gallon(10), new Liter(37.85411784M) };
            yield return new object[] { new Gallon(10), new CubicInch(2310) };
            yield return new object[] { new Gallon(10), new CubicFoot(1.3368055556M) };
            yield return new object[] { new Gallon(10), new Quart(40) };
        }
    }
}