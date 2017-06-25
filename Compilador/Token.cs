using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    public class Token
    {
        public int Numero { get; }
        public int Linha { get; }
        public string Lexograma { get; }
        public string Descricao { get; }
        public TokenId Id { get; private set; }
        public static int Tkid = 0;

        public Token(string Descricao, string Lexograma,  int Numero, int Linha) 
        {
            this.Descricao = Descricao;
            this.Lexograma = Lexograma;
            this.Numero = Numero;
            this.Linha = Linha;
            this.Id = (TokenId)Numero;
            Tkid++;
            if (Linha != -1) Programa.Codigo.Escreve(this);
        }
    }

    public enum TokenId
    {
        Ponto = 1,
        Var,
        Virgula,
        PontoEVirgula,
        Begin,
        End,
        Print,
        Read,
        If,
        Then,
        For,
        To,
        Do,
        Else,
        Atribuicao,
        Soma,
        Subtracao,
        Multiplicacao,
        Divisao,
        AbreParentese,
        FechaParentese,
        Igual,
        Diferente,
        Maior,
        Menor,
        MaiorIgual,
        MenorIgual,
        Variavel,
        NumInt,
        NumFloat,
        Real,
        Integer,
        DoisPontos,
        Const,
        EOF
    }
}
