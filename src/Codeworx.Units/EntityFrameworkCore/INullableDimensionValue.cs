namespace Codeworx.Units.EntityFrameworkCore
{
    public interface INullableDimensionValue<T>
        where T : IUnitBase
    {
        T? GetDimension();
    }
}
