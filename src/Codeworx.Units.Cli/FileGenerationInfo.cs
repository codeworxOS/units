using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Codeworx.Units.Cli
{
    internal record struct FileGenerationInfo(string Folder, string ClassName, NamespaceDeclarationSyntax Code)
    {
    }
}
