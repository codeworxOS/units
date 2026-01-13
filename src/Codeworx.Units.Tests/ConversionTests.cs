using System;
using System.Collections.Generic;
using Codeworx.Units.Defaults;
using Codeworx.Units.Defaults.DistanceDimension;
using Codeworx.Units.Defaults.TemperatureDimension;

namespace Codeworx.Units.Tests
{
    public class ConversionTests
    {
        [Theory]
        [MemberData(nameof(GetDistanceMeterToOtherConversionData))]
        [MemberData(nameof(GetDistanceYardToOtherConversionData))]
        public void DistanceConversionTests(IDistance original, IDistance check)
        {
            var converted = original.ToUnit(check.Symbol);

            Assert.Equal(Math.Round(converted.Value, 5), Math.Round(check.Value, 5));
        }

        [Theory]
        [MemberData(nameof(GetTemperatureCelsiusToConversionData))]
        [MemberData(nameof(GetTemperatureKelvinToConversionData))]
        [MemberData(nameof(GetTemperatureFahrenheitToConversionData))]
        public void TemperatureConversionTests(ITemperature original, ITemperature check)
        {
            var converted = original.ToUnit(check.Symbol);

            Assert.Equal(Math.Round(converted.Value, 5), Math.Round(check.Value, 5));
        }

        public static IEnumerable<object[]> GetDistanceMeterToOtherConversionData()
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

        public static IEnumerable<object[]> GetDistanceYardToOtherConversionData()
        {
            yield return new object[] { new Yard(1), new Inch(36M) };
            yield return new object[] { new Yard(1), new Feet(3M) };
            yield return new object[] { new Yard(1), new Meter(0.9144M) };
            yield return new object[] { new Yard(1000), new Mile(0.5681806818M) };
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
    }
}