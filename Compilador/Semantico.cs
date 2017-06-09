using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    public class Semantico
    {
        private class TokenSemantico 
        {
            public string Lexograma;
            public string Tipo;
            public bool isConstante;
            public int Escopo;

            public TokenSemantico(string Lexograma, string Tipo, bool isConstante, int Escopo)
            {
                this.Lexograma = Lexograma;
                this.Tipo = Tipo;
                this.isConstante = isConstante;
                this.Escopo = Escopo;
            }
            public override bool Equals(object obj)
            {
                TokenSemantico aux = (TokenSemantico)obj;
                if (this.Lexograma == aux.Lexograma)
                    return true;
                else
                    return false;
            }
        }
        //lexograma, tipo, constante, escopo, valor
        private List<TokenSemantico> tkSem;

        public Semantico()
        {
            this.tkSem = new List<TokenSemantico>();
        }

        public bool AddNaTabela(string lex, string tip, bool con, int esc)
        {
            TokenSemantico aux = new TokenSemantico(lex, tip, con, esc);
            if(!tkSem.Contains(aux))
            {
                tkSem.Add(aux);
                return true;
            }
            return false;
        }

        public int ChecaTabela(string lex)
        {
            return tkSem.FindIndex(t => t.Lexograma == lex);
            /*
            for(int  i = tkSem.Count - 1; i >=0; i--)
            {
                if (tkSem[i].Lexograma == lex)
                    return i;
            }
            return -1;
            */
        }

        public bool RetiraTabela(string lex, int esc)
        {
            int i = ChecaTabela(lex);
            if (i > -1)
            {
                tkSem.RemoveAt(i);
                return true;
            }
            return false;
        }
        public void ImprimeTabela()
        {
            for (int i = 0; i < tkSem.Count; i++)
            {
                Console.WriteLine(tkSem[i].Lexograma + "  " + tkSem[i].Tipo + "  " + tkSem[i].Escopo + "   " + tkSem[i].isConstante);
            }
            Console.WriteLine("-------------------");
        }
        public void LimpaEscopo(int esc)
        {
            tkSem.RemoveAll(x => x.Escopo == esc);
            //ImprimeTabela();
        }

    }
}