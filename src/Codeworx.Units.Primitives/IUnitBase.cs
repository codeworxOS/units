namespace Codeworx.Units.Primitives
{
    public interface IUnitBase
    {
        decimal BaseValue { get; }

        string Symbol { get; }

        string Key { get; }

        IUnitBase ToUnit(string symbol);
    }
}
