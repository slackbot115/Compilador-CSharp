using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Dominio
{
    public class GeradorItemsSintaticos
    {
        public string Token { get; set; }

        public GeradorItemsSintaticos(string token)
        {
            Token = token;
        }

        public override string ToString()
        {
            return ($@"{Token}");
        }
    }
}
