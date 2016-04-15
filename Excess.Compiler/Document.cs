using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excess.Compiler
{
	public interface IMappingService<TNode>
	{
		TNode LexicalTree { get; set; }
		TNode SyntacticalTree { get; set; }
		TNode SemanticalTree { get; set; }

		void LexicalChange(SourceSpan oldSpan, int newLength);
		void SemanticalChange(TNode oldNode, TNode newNode);

		TNode SemanticalMap(SourceSpan src);
		TNode SemanticalMap(TNode node);
		SourceSpan SourceMap(TNode node);
	}
}
