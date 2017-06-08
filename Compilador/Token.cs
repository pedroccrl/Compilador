using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class Token
    {
        public int Numero { get; }
        public int Linha { get; }
        public string Lexograma { get; }
        public string Descricao { get; }

        public Token(string Descricao, string Lexograma,  int Numero, int Linha) 
        {
            this.Descricao = Descricao;
            this.Lexograma = Lexograma;
            this.Numero = Numero;
            this.Linha = Linha;
        }
    }
}
