using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Dominio.Parser
{
    public class LanguageErrorListener : BaseErrorListener
    {
        public string Symbol { get; private set; }
        public StringWriter Writer { get; private set; }

        public LanguageErrorListener(StringWriter writer)
        {
            Writer = writer;
        }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            Writer.WriteLine(msg);

            Symbol = offendingSymbol.Text;
        }
    }
}
