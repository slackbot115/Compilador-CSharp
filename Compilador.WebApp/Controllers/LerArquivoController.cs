using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compilador.Dominio;
using Compilador.WebApp.Models;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Compilador.Dominio.Parser;

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
            List<GeradorItemsSintaticos> itemsSintaticos = new List<GeradorItemsSintaticos>();

            for (int i = 0; i < listAnalisador.Count; i++)
            {
                textsList.Add(listAnalisador[i].ToString());
                if (listAnalisador[i].Token.Contains(',') && listAnalisador[i].Token != ",") {
                    var tokenFiltrado = listAnalisador[i].Token.Split(',')[0];
                    itemsSintaticos.Add(new GeradorItemsSintaticos(tokenFiltrado));
                }
                else
                    itemsSintaticos.Add(new GeradorItemsSintaticos(listAnalisador[i].Token));
            }

            string stream = "";

            foreach (var item in itemsSintaticos)
            {
                stream += item + "\r\n";
            }

            var inputStream = new AntlrInputStream(stream);

            var languageLexer = new LanguageLexer(inputStream);

            var commonTokenStream = new CommonTokenStream(languageLexer);

            var languageParser = new LanguageParser(commonTokenStream);

            LanguageErrorListener errorListener;
            
            StringWriter writer = new StringWriter();
            errorListener = new LanguageErrorListener(writer);
            
            languageParser.RemoveErrorListeners();
            languageParser.AddErrorListener(errorListener);

            languageParser.program();

            string retornoSintatico = "";
            if(languageParser.NumberOfSyntaxErrors == 0)
                retornoSintatico += "Código válido";
            else
            {
                retornoSintatico += "Erro: " + errorListener.Writer + "\r\n";
            }
            
            string[] listaTokens = textsList.ToArray();
            List<object> retornos = new List<object>();
            retornos.Add(listaTokens);
            retornos.Add(retornoSintatico);

            ViewBag.Data = retornos;
            return View();

        }
    }
}
