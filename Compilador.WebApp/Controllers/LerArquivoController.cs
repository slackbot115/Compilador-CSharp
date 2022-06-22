using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compilador.Dominio;
using Compilador.WebApp.Models;

namespace Compilador.WebApp.Controllers
{
    public class LerArquivoController : Controller
    {
        private IWebHostEnvironment Environment;

        public LerArquivoController(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }

        public IActionResult LerCodigo(FileViewModel file)
        {
            AnaliseLexica analisadorLexico = new AnaliseLexica();

            string caminhoCodigo2 = file.FileFullPath;

            if (!System.IO.File.Exists(caminhoCodigo2))
            {
                ViewBag.Data = string.Empty;
                return View();
            }

            IEnumerable<string> lines = System.IO.File.ReadLines(caminhoCodigo2);

            List<GeradorItemsLexicos> listAnalisador = new();

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                List<char> linha = line.ToList();

                listAnalisador.AddRange(analisadorLexico.Analisador(linha));

            }

            List<string> textsList = new List<string>();

            for (int i = 0; i < listAnalisador.Count; i++)
            {
                
                textsList.Add(listAnalisador[i].ToString());
            }

            string [] texts = textsList.ToArray();

            ViewBag.Data = texts;
            return View();

        }
    }
}
