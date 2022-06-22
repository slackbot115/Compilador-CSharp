using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Dominio
{
    public class GeradorItemsSintaticos
    {
        public string Lexema { get; set; }
        public string Token { get; set; }

        public GeradorItemsSintaticos(string lexema, string token)
        {
            Lexema = lexema;
            Token = token;
        }

        public override string ToString()
        {
            return ($@"{Lexema}\{Token}");
        }
    }
}
