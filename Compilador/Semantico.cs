using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class Semantico
    {
        //lexograma, tipo, constante, escopo, valor
        private List<string> lexograma;
        private List<string> tipo;
        private List<int> constante;
        private List<int> escopo;

        public Semantico()
        {
            this.lexograma = new List<string>();
            this.tipo = new List<string>();
            this.constante = new List<int>();
            this.escopo = new List<int>();
        }

        public bool AddNaTabela(string lex, string tip, int con, int esc)
        {
            if (!lexograma.Contains(lex))
            {
                //System.out.println("add na tab "+lex);
                lexograma.Add(lex);
                tipo.Add(tip);
                constante.Add(con);
                escopo.Add(esc);
                return true;
            }
            return false;
        }
        public int ChecaTabela(string lex)
        {
            if (lexograma.Contains(lex) && lexograma.Count > 0)
            {
                
                for (int i = lexograma.Count - 1; i > -1; i--)
                {
                    if (lexograma[i] == lex)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        public bool retiraTabela(string lex, int esc)
        {

            int i = ChecaTabela(lex);
            if (i > -1)
            {
                lexograma.RemoveAt(i);
                escopo.Remove(i);
                constante.Remove(i);
                tipo.RemoveAt(i);
                return true;
            }
            return false;
        }
        public void imprimeTabela()
        {
            for (int i = 0; i < escopo.Count; i++)
            {
                Console.WriteLine(lexograma[i] + "  " + tipo[i] + "  " + escopo[i] + "   " + constante[i]);
            }
        }
        public void limpaEscopo(int esc)
        {
            int i = escopo.Count - 1;
            while (escopo.Contains(esc) && i > 0)
            {
                if (escopo[i] == esc)
                {
                    lexograma.RemoveAt(i);
                    escopo.Remove(i);
                    constante.Remove(i);
                    tipo.RemoveAt(i);
                }
                i = i - 1;
            }
        }

    }
}