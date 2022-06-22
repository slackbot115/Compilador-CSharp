using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Dominio
{
    public class GeradorItemsLexicos
    {
        public string Lexema { get; set; }
        public string Token { get; set; }
        public string Simbolo { get; set; }

        public GeradorItemsLexicos()
        {
        }

        public GeradorItemsLexicos(string lexema, string token, string simbolo)
        {
            Lexema = lexema;
            Token = token;
            Simbolo = simbolo;
        }

        public override string ToString()
        {
            return ($@"{Lexema}\{Token}\{Simbolo}");
        }
    }
}
