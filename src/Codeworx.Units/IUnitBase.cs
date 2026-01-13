namespace Codeworx.Units
{
    public interface IUnitBase
    {
        decimal BaseValue { get; }

        string Symbol { get; }

        string Key { get; }

        UnitSystem System { get; }

        IUnitBase ToUnit(string symbol);
    }
}
