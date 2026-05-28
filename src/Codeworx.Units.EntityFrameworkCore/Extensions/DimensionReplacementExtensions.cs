using Microsoft.EntityFrameworkCore;

namespace Codeworx.Units.EntityFrameworkCore
{
    public static class DimensionReplacementExtensions
    {
        public static DbContextOptionsBuilder AddDimensionQueryReplacement(this DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.AddInterceptors(DimensionQueryReplacementPreprocessor.Instance);
    }
}
