using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class Program
    {
        static string entrada = "Entrada.txt";
        static Lexico lexico;
        static Sintatico sintatico;
        static void Main(string[] args)
        {
            lexico = new Lexico(entrada);
            sintatico = new Sintatico(lexico);
            Token t;

            sintatico.PROG();
            /*
            do
            {
                t = lexico.NextToken();
                if (t == null)
                    break;  //Erro Léxico
                //Console.WriteLine("Linha: "+ t.Linha + "\tLexema: " + t.Lexograma + "\tDescricao: " + t.Descricao );
            } while (t.Lexograma != "EOF");
            */
            Console.Read();
        }
    }
}
