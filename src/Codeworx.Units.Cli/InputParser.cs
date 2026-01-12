using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Codeworx.Units.Cli.Data;

namespace Codeworx.Units.Cli
{
    public class InputParser : BaseDimensionProcessor
    {
        public Dictionary<string, JsonDimension> Result { get; private set; } = new Dictionary<string, JsonDimension>();

        public InputParser(Options options) : base(options)
        {
        }

        public async Task<bool> ParseDataAsync()
        {
            WriteVerboseInfo($"Looking for {CurrentOptions.Input}.");

            if (!File.Exists(CurrentOptions.Input))
            {
                WriteErrorOutput($"File {CurrentOptions.Input} not found!");
                return false;
            }

            var json = await File.ReadAllTextAsync(CurrentOptions.Input);
            try
            {
                var result = JsonSerializer.Deserialize<Dictionary<string, JsonDimension>>(json.ToString(), new JsonSerializerOptions { UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow });

                if (result == null)
                    throw new InvalidOperationException();

                Result = result;
            }
            catch (Exception ex)
            {
                WriteErrorOutput($"Error in {CurrentOptions.Input}.{Environment.NewLine}{ex}");
                return false;
            }
            HashSet<string> unitKeys = new HashSet<string>();
            HashSet<string> dimensionNames = new HashSet<string>();
            foreach ((var dimensionName, var data) in Result.ToList())
            {
                data.Name = dimensionName;

                var dimensionClassName = dimensionName.GetClassName();
                if (dimensionNames.Contains(dimensionClassName) || string.IsNullOrWhiteSpace(dimensionClassName))
                {
                    Result.Remove(dimensionName);
                    WriteWarningOutput($"Skipping Dimension {dimensionName}, invalid or duplicated ClassName");
                    continue;
                }

                HashSet<string> unitNames = new HashSet<string>();
                foreach ((var unitName, var unit) in data.Units.ToList())
                {
                    unit.Name = unitName;
                    var unitClassName = unitName.GetClassName();

                    if (unitNames.Contains(unitClassName) || string.IsNullOrWhiteSpace(unitClassName))
                    {
                        data.Units.Remove(unitName);
                        WriteWarningOutput($"Skipping Unit {unitName} in {dimensionName}, invalid or duplicated ClassName");
                        continue;
                    }

                    if (unit.Key == null)
                    {
                        unit.Key = $"{dimensionClassName}_{unitClassName}";
                    }

                    if (unitKeys.Contains(unit.Key))
                    {
                        data.Units.Remove(unitName);
                        WriteWarningOutput($"Skipping Unit {unitName} in {dimensionName}, invalid or duplicated Key");
                    }

                    if (unit.Key.Length > 32)
                    {
                        data.Units.Remove(unitName);
                        WriteWarningOutput($"Skipping Unit {unitName} in {dimensionName}, Key {unit.Key} is too long (<=32)");
                    }

                    if (unit.Symbol.Length > 10)
                    {
                        data.Units.Remove(unitName);
                        WriteWarningOutput($"Skipping Unit {unitName} in {dimensionName}, Symbol {unit.Symbol} is too long (<=10)");
                    }

                    unitKeys.Add(unit.Key);

                    if (unitName != data.BaseUnit)
                    {
                        unit.Conversions[data.BaseUnit] = new JsonUnitConversion { Divisor = unit.Divisor, Factor = unit.Factor, Offset = unit.Offset };
                    }
                }

                if (string.IsNullOrWhiteSpace(data.SIUnit) || !data.Units.ContainsKey(data.SIUnit))
                {
                    Result.Remove(dimensionName);
                    WriteWarningOutput($"Skipping Dimension {dimensionName}, SIUnit not found!");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(data.BaseUnit) || !data.Units.ContainsKey(data.BaseUnit))
                {
                    Result.Remove(dimensionName);
                    WriteWarningOutput($"Skipping Dimension {dimensionName}, BaseUnit not found!");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(data.MetricDefault) || !data.Units.ContainsKey(data.MetricDefault))
                {
                    Result.Remove(dimensionName);
                    WriteWarningOutput($"Skipping Dimension {dimensionName}, MetricDefault not found!");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(data.ImperialDefault) || !data.Units.ContainsKey(data.ImperialDefault))
                {
                    Result.Remove(dimensionName);
                    WriteWarningOutput($"Skipping Dimension {dimensionName}, ImperialDefault not found!");
                    continue;
                }

                if (!data.Units.Any())
                {
                    Result.Remove(dimensionName);
                    WriteWarningOutput($"Skipping Dimension {dimensionName}, Empty Units");
                    continue;
                }
            }

            return Result.Any();
        }
    }
}