namespace Codeworx.Units
{
    public interface IUnitBase
    {
        decimal Value { get; }

        string Symbol { get; }

        string Key { get; }

        decimal BaseValue { get; }

        UnitSystem System { get; }

        IUnitBase ToUnit(string symbol);
    }
}
