using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.Editor;
using Microsoft.CodeAnalysis.Editor.Extensibility.NavigationBar;

namespace Microsoft.CodeAnalysis.Remote.Asset
{
    public static class SymbolGetter
    {
        public static Location GetSymbol(
            Compilation compilation, SyntaxTree syntaxTree, NavigationBarItem item, CancellationToken cancellationToken)
        {
            var symbolItem = item as NavigationBarSymbolItem;
            if (symbolItem == null)
            {
                return null;
            }

            var symbols = symbolItem.NavigationSymbolId.Resolve(compilation, cancellationToken: cancellationToken);
            var symbol = symbols.Symbol;

            if (symbol == null)
            {
                if (symbolItem.NavigationSymbolIndex < symbols.CandidateSymbols.Length)
                {
                    symbol = symbols.CandidateSymbols[symbolItem.NavigationSymbolIndex.Value];
                }
                else
                {
                    return null;
                }
            }

            var location = symbol.Locations.FirstOrDefault(l => l.SourceTree.Equals(syntaxTree));
            if (location == null)
            {
                location = symbol.Locations.FirstOrDefault();
            }

            return location;
        }
    }
}
