namespace Codeworx.Units.EntityFrameworkCore
{
    public interface IDimensionValue<T>
        where T : IUnitBase
    {
        T GetDimension();
    }
}
