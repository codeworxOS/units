using System.Collections.Generic;
using System.Linq;
using Codeworx.Units.Defaults;
using Codeworx.Units.Defaults.DistanceDimension;
using Codeworx.Units.Tests.Data;

namespace Codeworx.Units.Tests
{
    public class ValidationTests
    {
        public static IEnumerable<object[]> DistanceDataOk => GetDistanceData(true).Select(d => new object[] { d });
        public static IEnumerable<object[]> DistanceDataError => GetDistanceData(false).Select(d => new object[] { d });
        public static IEnumerable<object[]> UnitDataOk => GetDistanceData(true).Select(d => new object[] { d.ToMeter() });
        public static IEnumerable<object[]> UnitDataError => GetDistanceData(false).Select(d => new object[] { d.ToMeter() });

        private static IEnumerable<IDistance> GetDistanceData(bool ok)
        {
            if (ok)
            {
                yield return new Meter(5);
                yield return new Meter(7);
                yield return new Meter(10);

                yield return new Centimeter(500);
                yield return new Centimeter(700);
                yield return new Centimeter(1000);
            }
            else
            {
                yield return new Meter(4.9M);
                yield return new Meter(10.1M);

                yield return new Centimeter(490);
                yield return new Centimeter(1001);
            }
        }

        [Theory]
        [MemberData(nameof(DistanceDataOk))]
        public void TestDimensionValidation_ExpectOK(IDistance distance)
        {
            var testObj = new DimensionValidationDummy { Distance = distance };

            var validationResult = testObj.Validate(out var validationResultList);
            Assert.True(validationResult);
            Assert.Empty(validationResultList);
        }

        [Theory]
        [MemberData(nameof(DistanceDataError))]
        public void TestDimensionValidation_ExpectError(IDistance distance)
        {
            var testObj = new DimensionValidationDummy { Distance = distance };

            var validationResult = testObj.Validate(out var validationResultList);
            Assert.False(validationResult);
            Assert.NotEmpty(validationResultList);
        }

        [Theory]
        [MemberData(nameof(UnitDataOk))]
        public void TestUnitValidation_ExpectOK(Meter distance)
        {
            var testObj = new UnitValidationDummy { Distance = distance };

            var validationResult = testObj.Validate(out var validationResultList);
            Assert.True(validationResult);
            Assert.Empty(validationResultList);
        }

        [Theory]
        [MemberData(nameof(UnitDataError))]
        public void TestUnitValidation_ExpectError(Meter distance)
        {
            var testObj = new UnitValidationDummy { Distance = distance };

            var validationResult = testObj.Validate(out var validationResultList);
            Assert.False(validationResult);
            Assert.NotEmpty(validationResultList);
        }
    }
}
