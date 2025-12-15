using System.Linq;

namespace System
{
    public static class StringExtensions
    {
        public static string GetClassName(this string name)
        {
            var splittedNamesToUpper = name.Split([" ", "(", ")", ".", "-"], StringSplitOptions.RemoveEmptyEntries).Select(d => d.Substring(0, 1).ToUpper() + d.Substring(1));

            return string.Join(string.Empty, splittedNamesToUpper);
        }
    }
}
