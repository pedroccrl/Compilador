using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class Lexico
    {
        public Dictionary<string, int> Tabela { get; }
        public StreamReader Reader { get; }
        private int Linha;
        public string LinhaAtual { get; private set; }


        public Lexico(string caminho)
        {
            Tabela = new Dictionary<string, int>();
            Reader = new StreamReader(caminho);
            Linha = 0;
            LinhaAtual = "";
            InicializaTabela();
        }

        private void InicializaTabela()
        {
            Token t;
            Tabela.Add(".", (int)TokenId.Ponto);
            Tabela.Add("var", 2);
            Tabela.Add(",", 3);
            Tabela.Add(";", 4);
            Tabela.Add("begin", 5);
            Tabela.Add("end", 6);
            Tabela.Add("print", 7);
            Tabela.Add("read", 8);
            Tabela.Add("if", 9);
            Tabela.Add("then", 10);
            Tabela.Add("for", 11);
            Tabela.Add("to", 12);
            Tabela.Add("do", 13);
            Tabela.Add("else", 14);
            Tabela.Add(":=", 15);
            Tabela.Add("+", 16);
            Tabela.Add("-", 17);
            Tabela.Add("*", 18);
            Tabela.Add("/", 19);
            Tabela.Add("(", 20);
            Tabela.Add(")", 21);
            Tabela.Add("=", 22);
            Tabela.Add("<>", 23);
            Tabela.Add(">", 24);
            Tabela.Add("<", 25);
            Tabela.Add(">=", 26);
            Tabela.Add("<=", 27);
            Tabela.Add("variavel", 28);
            Tabela.Add("numint", 29);
            Tabela.Add("numfloat", 30);
            Tabela.Add("real", 31);
            Tabela.Add("integer", 32);
            Tabela.Add(":", 33);
            Tabela.Add("const", 34);
            Tabela.Add("EOF", 35);
            Console.WriteLine("Tabela Inicializada");
        }
        private void ErroLexico(char t)
        {
            Console.WriteLine("\nErro Léxico na linha " + Linha + "\nToken Encontrado: " + t);

        }

        private string RemoveInvalidos(string linha)
        {
            linha = linha.Replace("\n", "");
            linha = linha.Replace("\t", "");
            linha = linha.Replace(" ", "");
            return linha;
        }

        private Token CaracterEspecial(string linha)
        {
            Token t = null;
            switch (linha[0])
            {
                //Casos especiais. Simbolos com mais de um caracter.
                case ':':
                    if (linha[1] == '=')
                        t = new Token(":=", ":=", Tabela[":="], Linha);
                    else
                        t = new Token(":", ":", Tabela[":"], Linha);
                    break;
                case '>':
                    if (linha[1] == '=')
                        t = new Token(">=", ">=", Tabela[">="], Linha);
                    else
                        t = new Token(">", ">", Tabela[">"], Linha);
                    break;
                case '<':
                    if (linha[1] == '=')
                        t = new Token("<>", "<>", Tabela["<>"], Linha);
                    else if(linha[1] == '>')
                        t = new Token("<>", "<>", Tabela["<>"], Linha);
                    break;

                default:
                    try
                    {
                        string lex = char.ToString(linha[0]);
                        int n = Tabela[lex];
                        t = new Token(lex, lex, n, Linha);
                    }
                    catch(KeyNotFoundException e)
                    {
                        ErroLexico(linha[0]);
                    }
                    break;
                
            }
            return t;
        }



        public Token NextToken()
        {
            if (Reader.Peek() <= 0 && LinhaAtual == "") //Acaba o arquivo
                return new Token("EOF", "EOF", Tabela["EOF"], Linha++);

            //Falta Implementar o comentário

            while(LinhaAtual.Length == 0 || LinhaAtual == " ")    //Se o linha atual estiver vazio, leio uma linha do arquivo
            {
                LinhaAtual = Reader.ReadLine();
                Linha++;
            }
            while(LinhaAtual[0] == ' ')
            {
                if(LinhaAtual.Length == 1)
                {
                    while (LinhaAtual.Length == 0 || LinhaAtual == " ")    //Se o linha atual estiver vazio, leio uma linha do arquivo
                    {
                        LinhaAtual = Reader.ReadLine();
                        Linha++;
                    }
                }
                else
                {
                    LinhaAtual = LinhaAtual.Substring(1);
                }
            }

            //Agora a ser analisada está na variável LinhaAtual
            if ((LinhaAtual[0] >= 'a' && LinhaAtual[0] <= 'z') || (LinhaAtual[0] >= 'A' && LinhaAtual[0] <= 'Z'))
            {
                int i = 0;
                while((LinhaAtual[i] >= 'a' && LinhaAtual[i] <= 'z') || (LinhaAtual[i] >= 'A' && LinhaAtual[i] <= 'Z'))
                {
                    i++;
                    if (i > LinhaAtual.Length - 1)
                        break;
                }
                string aux = LinhaAtual.Substring(0, i);
                LinhaAtual = LinhaAtual.Remove(0,i);
                try
                {
                    return new Token(aux, aux, Tabela[aux], Linha);
                }
                catch(KeyNotFoundException e)
                {
                    return new Token("variavel", aux, Tabela["variavel"], Linha);
                }
            }
            else if(LinhaAtual[0] >= '0' && LinhaAtual[0] <= '9')
            {
                int i = 0;
                string aux;
                bool isFloat = false;
                while(LinhaAtual[i] >= '0' && LinhaAtual[i] <= '9' || LinhaAtual[i] == '.')
                {
                    if (LinhaAtual[i] == '.' && isFloat == false)
                        isFloat = true;
                    i++;
                    if (i > LinhaAtual.Length - 1)
                        break;
                }
                aux = LinhaAtual.Substring(0, i);
                LinhaAtual = LinhaAtual.Remove(0,i);
                if (isFloat)
                    return new Token("numfloat", aux, Tabela["numfloat"], Linha);
                else
                    return new Token("numint", aux, Tabela["numint"], Linha);
            }
            else
            {
                Token t =  CaracterEspecial(LinhaAtual);
                if(t != null)
                    LinhaAtual = LinhaAtual.Remove(0,t.Lexograma.Length);
                return t;
            }




        }


    }
}
