using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Codeworx.Units.Cli.Data;
using Codeworx.Units.Cli.Typescript;
using HandlebarsDotNet;

namespace Codeworx.Units.Cli
{
    public class TypescriptDimensionCreator : BaseDimensionProcessor
    {
        private readonly List<TSDimension> _dimensionData;

        public TypescriptDimensionCreator(Options options, Dictionary<string, JsonDimension> data) : base(options)
        {
            List<TSDimension> dimensions = new List<TSDimension>();

            foreach ((string dimensionName, JsonDimension dimensionData) in data)
            {

                List<TSUnit> units = new List<TSUnit>();
                foreach ((var unitName, JsonUnit unitData) in dimensionData.Units)
                {
                    List<TSConversion> conversions = new List<TSConversion>();
                    foreach ((var toUnitName, var toUnitData) in dimensionData.Units)
                    {
                        string? conversion = null;
                        if (unitName != toUnitName)
                        {
                            var conversionPath = dimensionData.GetConversionPath(unitData, toUnitData);
                            if (conversionPath == null)
                            {
                                WriteWarningOutput($"Invalid Conversion {dimensionData.Name}");
                            }
                            var str = (conversionPath ?? []).GetConversionExpression().ToFullString();

                            str = str.Replace("Value", "this.value");
                            str = str.Replace("M", string.Empty);

                            conversion = str;
                        }

                        conversions.Add(new TSConversion
                        {
                            Conversion = conversion,
                            ConversionUnitName = toUnitName.GetClassName(),
                            DimensionName = dimensionName.GetClassName()
                        });
                    }

                    units.Add(new TSUnit
                    {
                        DimensionName = dimensionName.GetClassName(),
                        UnitName = unitName.GetClassName(),
                        Symbol = unitData.Symbol,
                        SystemString = string.Join(",", GetSystemString(unitData.System).Select(d => $"'{d}'")),
                        Conversion = conversions
                    });
                }

                dimensions.Add(new TSDimension
                {
                    DimensionName = dimensionName.GetClassName(),
                    DefaultImperial = dimensionData.Units[dimensionData.ImperialDefault].Symbol,
                    DefaultMetric = dimensionData.Units[dimensionData.MetricDefault].Symbol,
                    DefaultUnit = dimensionData.Units[dimensionData.BaseUnit].Symbol,
                    SIUnit = dimensionData.Units[dimensionData.SIUnit].Symbol,
                    Units = units
                });
            }

            _dimensionData = dimensions;
        }

        private IEnumerable<string> GetSystemString(UnitSystem? system)
        {
            List<string> systems = new List<string>();

            if (system.HasValue)
            {
                var flagValues = Enum.GetValues(typeof(UnitSystem)).Cast<UnitSystem>().Where(x => system.Value.HasFlag(x));

                foreach (var item in flagValues)
                {
                    systems.Add(item.ToString());
                }
            }

            if (systems.Count == 0)
            {
                return [nameof(UnitSystem.Metric), nameof(UnitSystem.Imperial)];
            }

            return systems;
        }

        public async Task<bool> ProcessAsync()
        {
            WriteVerboseInfo($"Updating File {CurrentOptions.Typescript}");

            var template = Handlebars.Compile(GetTypeScriptTemplate());

            var names = _dimensionData.Select(t => "'" + t.DimensionName + "'");
            var result = template(new { Dimensions = _dimensionData, Names = string.Join(" | ", names), NameList = string.Join(", ", names) });

            try
            {
                await File.WriteAllTextAsync(CurrentOptions.Typescript!, result);
            }
            catch (Exception ex)
            {
                WriteErrorOutput($"Could not write file: {CurrentOptions.Typescript}");
                WriteErrorOutput(ex.ToString());
                return false;
            }

            return true;
        }

        private string GetTypeScriptTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Codeworx.Units.Cli.Typescript.Template.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}