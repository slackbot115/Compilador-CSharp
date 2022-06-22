using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Dominio
{
    public class GeradorObjetosCompilados
    {
        List<GeradorItemsLexicos> itemsLexicos;
        List<GeradorItemsSintaticos> itemsSintaticos;

        public GeradorObjetosCompilados()
        {
        }

        public GeradorObjetosCompilados(List<GeradorItemsLexicos> itemsLexicos, List<GeradorItemsSintaticos> itemsSintaticos)
        {
            this.itemsLexicos = itemsLexicos;
            this.itemsSintaticos = itemsSintaticos;
        }
    }
}
