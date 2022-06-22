using Compilador.Dominio;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace Compilador.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            string fileName = @"C:\Users\vinic\Desktop\codigo.txt";

            IEnumerable<string> lines = File.ReadLines(fileName);

            AnaliseLexica analisadorLexico = new AnaliseLexica();
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

            for (int i = 0; i < listAnalisador.Count; i++)
            {
                System.Console.WriteLine($"Lexema: {listAnalisador[i].Lexema}\t\tToken: {listAnalisador[i].Token}\t\tSímbolo: {listAnalisador[i].Simbolo}");
            }
        }
    }
}
