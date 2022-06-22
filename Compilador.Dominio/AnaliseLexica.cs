using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Compilador.Dominio
{
    public class AnaliseLexica
    {
        
        public static Regex regexInt = new Regex("^[0-9]+$");
        public static Regex regexIds = new Regex("^[a-zA-Z_]([a-zA-Z_]|[0-9])*$");
        public static Regex regexDouble = new Regex(@"^[0-9]+\.[0-9]+$");

        public static char operadorAtribuicao = '=';

        public  Dictionary<int, string> tabelaDeSimbolos = new Dictionary<int, string>();

        public static Dictionary<string, string> palavrasReservadas = new Dictionary<string, string>()
                {
                    { "void", "VOID" },
                    { "main", "MAIN" },
                    { "int", "INT" },
                    { "float", "FLOAT" },
                    { "char", "CHAR" },
                    { "boolean", "BOOLEAN" },
                    { "if", "IF" },
                    { "else", "ELSE" },
                    { "for", "FOR" },
                    { "while", "WHILE" },
                    { "scanf", "SCANF" },
                    { "println", "PRINTLN" },
                    { "return", "RETURN" }
                };

        public static List<string> simbolosEspeciais = new List<string>() { "(", ")", "{", "}", "[", "]", ",", ";" };

        public static List<string> operadoresAritmeticos = new List<string>() { "+", "-", "*", "/", "%"};

        public static List<string> operadoresLogicos = new List<string>() {"&&", "||", "!"};

        public static List<string> operadoresComparacao = new List<string>() { ">", ">=", "<=", "<", "!=", "==" };

        public  int contadorTabelaSimbolos = 1;

        public List<GeradorItemsSintaticos> Analisador(List<char> codigoChar)
        {
            codigoChar = AdicionaEspacoNoFinalCasoNecessario(codigoChar);

            string lexema = "";

            List<GeradorItemsLexicos> lexemaTokenSimbolo = new List<GeradorItemsLexicos>();
            List<GeradorItemsSintaticos> itensSintaticos = new List<GeradorItemsSintaticos>();

            for (int i = 0; i < codigoChar.Count; i++)
            {
                
                if (Char.IsLetter(codigoChar[i]))
                {
                    lexema += codigoChar[i];
                }
                else if (Char.IsDigit(codigoChar[i]))
                {
                    lexema+= codigoChar[i];
                }
                else if (codigoChar[i] == '.')
                {
                    lexema += codigoChar[i];
                }
                else if (VerificarExistenciaOperadorLogico(codigoChar[i].ToString()) && codigoChar[i] != '!')
                {
                    lexema += codigoChar[i];

                    //adiciona && ||
                    string operadorLogico = VerificarOperadorLogico(lexema);
                    if (lexema.Equals(operadorLogico))
                    {
                        GeradorItemsLexicos novoItem = new GeradorItemsLexicos(lexema, lexema, "Operador Lógico");
                        lexemaTokenSimbolo.Add(novoItem);
                        lexema = "";

                        GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos(lexema);
                        itensSintaticos.Add(itemSintatico);
                    }
                }
                else if (codigoChar[i] == '!' && codigoChar[i + 1] != '=')
                {
                    //adiciona !
                    GeradorItemsLexicos novoItem = new GeradorItemsLexicos("!", "!", "Operador Lógico");
                    lexemaTokenSimbolo.Add(novoItem);
                    lexema = "";

                    GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos("!");
                    itensSintaticos.Add(itemSintatico);
                }
                else
                {
                    //adicionar no token
                    string token = VerificarPalavraReservada(lexema);
                    if (token != null)
                    {
                        GeradorItemsLexicos novoItem = new GeradorItemsLexicos(lexema, token, "palavra reservada");
                        lexemaTokenSimbolo.Add(novoItem);
                        lexema = "";

                        GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos("if");
                        itensSintaticos.Add(itemSintatico);
                    }

                    string simbolo = VerificarSimboloEspecial(codigoChar[i].ToString());

                    //adciona identificador
                    if ((lexema != "") && (lexema != simbolo))
                    {
                        if (regexIds.IsMatch(lexema))
                        {
                            AdicionaNaTabelaDeSimbolos(lexema, ref contadorTabelaSimbolos, lexemaTokenSimbolo);
                            
                            GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos("ID");
                            itensSintaticos.Add(itemSintatico);
                        }
                    }

                    //adiciona numero double 
                    if (regexDouble.IsMatch(lexema))
                    {
                        GeradorItemsLexicos novoItem = new GeradorItemsLexicos(lexema, "NUM_DEC, " + lexema, "Numero decimal");
                        lexemaTokenSimbolo.Add(novoItem);

                        GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos("NUM_DEC");
                        itensSintaticos.Add(itemSintatico);
                    }

                    //adiciona numero inteiro 
                    if (regexInt.IsMatch(lexema))
                    {
                        GeradorItemsLexicos novoItem = new GeradorItemsLexicos(lexema, "NUM_INT, " + lexema, "Numero inteiro");
                        lexemaTokenSimbolo.Add(novoItem);

                        GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos("NUM_INT");
                        itensSintaticos.Add(itemSintatico);
                    }

                    if ((!string.IsNullOrEmpty(lexema)) && (!regexIds.IsMatch(lexema)) && (!regexInt.IsMatch(lexema)) && (!regexDouble.IsMatch(lexema)))
                    {
                        GeradorItemsLexicos erroItem = new GeradorItemsLexicos(lexema, "ERRO", "ERRO");
                        lexemaTokenSimbolo.Add(erroItem);
                    }

                    //adicionar no simbolo especial
                    if (simbolo != null)
                    {
                        GeradorItemsLexicos novoItem = new GeradorItemsLexicos(simbolo, simbolo, "símbolo especial");
                        lexemaTokenSimbolo.Add(novoItem);

                        GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos(simbolo);
                        itensSintaticos.Add(itemSintatico);
                    }

                    if (VerificaExistenciaCaracterComparacao(codigoChar[i]))
                    {
                        //adiciona <= >= etc
                        if (codigoChar[i + 1] == '=')
                        {
                            string operadorComparacao = codigoChar[i] + "" + codigoChar[i + 1];

                            string comp = VerificaOperadorComparcao(operadorComparacao);

                            GeradorItemsLexicos novoItem = new GeradorItemsLexicos(comp, "COMP", "Operador comparação");
                            lexemaTokenSimbolo.Add(novoItem);
                            lexema = "";

                            GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos("COMP");
                            itensSintaticos.Add(itemSintatico);

                            continue;
                        }
                        //adiciona > < etc...
                        else
                        {
                            GeradorItemsLexicos novoItem = new GeradorItemsLexicos(codigoChar[i].ToString(), "COMP", "Operador comparação");
                            lexemaTokenSimbolo.Add(novoItem);

                            GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos("COMP");
                            itensSintaticos.Add(itemSintatico);
                        }
                    }

                    if (codigoChar[i] == operadorAtribuicao)
                    {
                        //adiciona ==
                        if(codigoChar[i+1] == operadorAtribuicao)
                        {
                            GeradorItemsLexicos novoItem = new GeradorItemsLexicos("==", "COMP", "Operador comparação");
                            lexemaTokenSimbolo.Add(novoItem);

                            GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos("COMP");
                            itensSintaticos.Add(itemSintatico);
                        }

                        //adiciona =
                        else if(codigoChar[i-1] != operadorAtribuicao && !VerificaExistenciaCaracterComparacao(codigoChar[i - 1]))
                        {
                            GeradorItemsLexicos novoItem = new GeradorItemsLexicos(operadorAtribuicao.ToString(), operadorAtribuicao.ToString(), "Operador de Atribuição");
                            lexemaTokenSimbolo.Add(novoItem);

                            GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos("=");
                            itensSintaticos.Add(itemSintatico);
                        }
                        
                    }

                    //adiciona textos
                    if (codigoChar[i] == '"')
                    {
                        string txt = RealizaTexto(ref i, codigoChar, lexema);

                        if (txt == "Texto")
                        {
                            GeradorItemsLexicos novoItem = new GeradorItemsLexicos("txt", "Texto ", "Constantes de texto");
                            lexemaTokenSimbolo.Add(novoItem);

                            GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos("Texto");
                            itensSintaticos.Add(itemSintatico);
                        }
                        else
                        {
                            GeradorItemsLexicos novoItem = new GeradorItemsLexicos(txt, "ERRO", "ERRO");
                            lexemaTokenSimbolo.Add(novoItem);
                        }
                            
                        
                    }

                    string operadorAritimetico = VerificarOperadorAritimetico(codigoChar[i].ToString());

                    //adiciona + - etc...
                    if (codigoChar[i].ToString() == operadorAritimetico)
                    {
                        //adiciona comentário
                        if (codigoChar[i] == '/' && codigoChar[i + 1] == '/')
                        {
                            GeradorItemsLexicos novoItem = new GeradorItemsLexicos("//", " ", "Comentário");
                            lexemaTokenSimbolo.Add(novoItem);
                            RealizaComentario(ref i, codigoChar, lexema);
                        }
                        //adiciona operador aritimético /
                        else if(codigoChar[i] == '/' && codigoChar[i-1] != '/')
                        {
                            GeradorItemsLexicos novoItem = new GeradorItemsLexicos(codigoChar[i].ToString(), codigoChar[i].ToString(), "Operador Aritmético");
                            lexemaTokenSimbolo.Add(novoItem);

                            GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos(codigoChar[i].ToString());
                            itensSintaticos.Add(itemSintatico);
                        }
                        //adiciona operador aritimético diferente de barra
                        else if(codigoChar[i] != '/')
                        {
                            GeradorItemsLexicos novoItem = new GeradorItemsLexicos(codigoChar[i].ToString(), codigoChar[i].ToString(), "Operador Aritmético");
                            lexemaTokenSimbolo.Add(novoItem);

                            GeradorItemsSintaticos itemSintatico = new GeradorItemsSintaticos(codigoChar[i].ToString());
                            itensSintaticos.Add(itemSintatico);
                        }
                    }

                    
                    if (codigoChar[i] != ' ' && codigoChar[i] != '\t' && simbolo == null && operadorAritimetico == null && codigoChar[i] != operadorAtribuicao && codigoChar[i] != '"'&& !operadoresComparacao.Contains(codigoChar[i].ToString()))
                    {
                        GeradorItemsLexicos erroItem = new GeradorItemsLexicos(codigoChar[i].ToString(), "ERRO", "ERRO");
                        lexemaTokenSimbolo.Add(erroItem);
                    }
                        

                    lexema = "";
                }
            }

            return itensSintaticos;
        }

        private string RealizaTexto(ref int i, List<char> codigoChar, string lexema)
        {
            i = i + 1;

            while (i < codigoChar.Count)
            {
                lexema += "";

                if (codigoChar[i] == '"')
                {
                    return "Texto";
                }

                if (i == codigoChar.Count - 1)
                {

                    return "ERRO_TEXTO";
                }

                i++;
            }

            return null;
        }

        private void RealizaComentario(ref int i, List<char> codigoChar, string lexema)
        {
            i = i + 2;
            while(i < codigoChar.Count)
            {
                lexema += "";
                
                i++;
            }

            i--;
        }

        private bool VerificaExistenciaCaracterComparacao(char op)
        {
            if(op == '>')
            {
                return true;
            }
            if (op == '<')
            {
                return true;
            }
            if (op == '!')
            {
                return true;
            }

            return false;
        }

        
        private string VerificaOperadorComparcao(string lexema)
        {
            foreach(var op in operadoresComparacao)
            {
                if(op == lexema)
                {
                    return op;
                }
            }

            return null;
        }

        private bool VerificarExistenciaOperadorLogico(string operador)
        {
            foreach(var op in operadoresLogicos)
            {
                if (op.Contains(operador))
                {
                    return true;
                }

            }

            return false;
        }

        private string VerificarOperadorLogico(string operador)
        {
            //&&x

            foreach (var op in operadoresLogicos)
            {
                if (op.Equals(operador))
                {
                    return operador;
                }
            }

            return null;
        }

        private string VerificarOperadorAritimetico(string opereador)
        {
            foreach (var op in operadoresAritmeticos)
            {
                if (op.Equals(opereador))
                {
                    return opereador;
                }
            }

            return null;
        }

        private List<char> AdicionaEspacoNoFinalCasoNecessario(List<char> codigoChar)
        {
            int i = codigoChar[codigoChar.Count - 1];

            if ( i != ' ')
            {
                codigoChar.Add(' ');
            }

            return codigoChar;
        }

        private void AdicionaNaTabelaDeSimbolos(string lexema, ref int contadorTabelaSimbolos, List<GeradorItemsLexicos> lexemaTokenSimbolo)
        {
            int key = 0;
            string value = "";

            ////se o value for igual ao lexema retorna o item do dicionario e passa o value no lexema e a key no contador
            if (tabelaDeSimbolos.ContainsValue(lexema))
            {
                foreach (KeyValuePair<int, string> item in tabelaDeSimbolos)
                {
                    if (item.Value.Equals(lexema)){
                        value = item.Value;
                        key = item.Key;
                    }
                }

                GeradorItemsLexicos novoItem = new GeradorItemsLexicos(value, $"ID, {key}", "identificador");
                lexemaTokenSimbolo.Add(novoItem);
            }
            //caso contrario adiciona o lexema no value e a key no contador
            else
            {
                tabelaDeSimbolos.Add(contadorTabelaSimbolos, lexema);

                foreach (KeyValuePair<int, string> item in tabelaDeSimbolos)
                {
                    if (item.Value.Equals(lexema))
                    {
                        value = item.Value;
                        key = item.Key;
                    }
                }

                GeradorItemsLexicos novoItem = new GeradorItemsLexicos(value, $"ID, {key}", "identificador");
                lexemaTokenSimbolo.Add(novoItem);

                contadorTabelaSimbolos++;
            }
        }

        private static string VerificarSimboloEspecial(string lexema)
        {
            foreach (var simbolo in simbolosEspeciais)
            {
                if (simbolo.Equals(lexema))
                {
                    return simbolo;
                }
            }

            return null;
        }

        private static string VerificarPalavraReservada(string lexema)
        {
            for (int i = 0; i < palavrasReservadas.Count; i++)
            {
                if (palavrasReservadas.ContainsKey(lexema))
                {
                    return palavrasReservadas[lexema];
                }
            }

            return null;

        }
    }
}
