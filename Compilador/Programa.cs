using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Compilador
{
    public class Programa
    {
        string path = "programa.c";
        string ident = "";
        public bool AcabouLinha { get; set; }
        public Token TokenEscopo { get; set; }
        public Token TokenIO { get; set; }
        public Token TokenVarFor { get; set; }
        public Token TokenTipoVar { get; set; }
        public List<Token> VariaveisEscopo { get; set; }
        private List<Token> VariaveisSemantico { get; set; }

        private static Programa _codigo;

        public static Programa Codigo
        {
            get { if (_codigo == null) _codigo = new Programa(); return _codigo; }
            set { _codigo = value; }
        }

        private Programa()
        {
            string lib = "// Alunos: Pedro Camara, Victor Brito, Caio Falco\n// Compiladores 2017.1\n\n";
            lib += "#include <stdlib.h>\n#include <stdio.h>\n";
            File.WriteAllText(path, lib);
            VariaveisEscopo = new List<Token>();
            VariaveisSemantico = new List<Token>();
            AcabouLinha = true;
        }

        public void Escreve (Token token)
        {
            string linha = string.Empty;

            switch (token.Id)
            {
                case TokenId.Ponto:
                    break;
                case TokenId.Var:
                    TokenEscopo = token;
                    break;
                case TokenId.Virgula:
                    break;
                case TokenId.PontoEVirgula:
                    if (TokenEscopo.Id != TokenId.Const && TokenEscopo.Id != TokenId.End) linha = token.Lexograma;
                    linha += "\n";
                    TokenIO = null;
                    AcabouLinha = true;
                    break;
                case TokenId.Begin:
                    if (ident.Length < 4)
                    {
                        linha = "int main()\n{\n";
                        ident = "    ";
                    }
                    TokenEscopo = token;
                    break;
                case TokenId.End:
                    TokenEscopo = token;
                    if (ident.Length>=4) ident = ident.Remove(0, 4);
                    linha = $"{ident}" + "}\n";
                    break;
                case TokenId.Print:
                    linha = $"{ident}printf";
                    TokenIO = token;
                    break;
                case TokenId.Read:
                    linha = $"{ident}scanf";
                    TokenIO = token;
                    break;
                case TokenId.If:
                    TokenEscopo = token;
                    linha = $"{ident}if (";
                    break;
                case TokenId.Then:
                    if (TokenEscopo.Id == TokenId.If)
                    {
                        linha = $")\n{ident}" + "{\n";
                        ident += "    ";
                    }
                    break;
                case TokenId.For:
                    TokenEscopo = token;
                    linha = $"{ident}for (";
                    break;
                case TokenId.To:
                    linha = $"; {TokenVarFor.Lexograma} < ";
                    break;
                case TokenId.Do:
                    linha = $"; {TokenVarFor.Lexograma}++)\n{ident}" + "{\n";
                    ident += "    ";
                    TokenTipoVar = null;
                    break;
                case TokenId.Else:
                    linha = $"{ident}else\n{ident}" + "{\n";
                    ident += "    ";
                    break;
                case TokenId.Atribuicao:
                    linha = " = ";
                    break;
                case TokenId.Soma:
                    linha = token.Lexograma;
                    break;
                case TokenId.Subtracao:
                    linha = token.Lexograma;
                    break;
                case TokenId.Multiplicacao:
                    linha = token.Lexograma;
                    break;
                case TokenId.Divisao:
                    linha = token.Lexograma;
                    break;
                case TokenId.AbreParentese:
                    //if (TokenIO?.Id == TokenId.Read || TokenIO?.Id == TokenId.Print)
                    linha = "(";
                    break;
                case TokenId.FechaParentese:
                    linha = ")";
                    break;
                case TokenId.Igual:
                    if (TokenEscopo.Id == TokenId.Const) linha = " ";
                    else linha = "==";
                    break;
                case TokenId.Diferente:
                    linha = "!=";
                    break;
                case TokenId.Maior:
                    linha = ">";
                    break;
                case TokenId.Menor:
                    linha = "<";
                    break;
                case TokenId.MaiorIgual:
                    linha = ">=";
                    break;
                case TokenId.MenorIgual:
                    linha = "<=";
                    break;
                case TokenId.Variavel:
                    if (TokenEscopo.Id == TokenId.Const)
                    {
                        linha = $"#define {token.Lexograma}";
                    }
                    else if (TokenEscopo.Id == TokenId.Var) VariaveisEscopo.Add(token);
                    else if (TokenIO != null)
                    {
                        var v = VariaveisSemantico.Find(var => var.Lexograma == token.Lexograma);
                        if (v.Id == TokenId.Integer) linha = $"\"%d\"";
                        else linha = $"\"%f\"";

                        if (TokenIO.Id == TokenId.Read) linha += $", &{v.Lexograma}";
                        else linha += $", {v.Lexograma}";
                    }
                    else if (TokenEscopo.Id == TokenId.Begin)
                    {
                        if (AcabouLinha)
                        {
                            AcabouLinha = false;
                            linha = ident;
                        }
                        linha += token.Lexograma;
                    }
                    else linha = token.Lexograma;
                    if (TokenEscopo.Id == TokenId.For) TokenVarFor = token;
                    break;
                case TokenId.NumInt:
                    linha = token.Lexograma;
                    break;
                case TokenId.NumFloat:
                    linha = token.Lexograma;
                    break;
                case TokenId.Real:
                    if (TokenEscopo.Id == TokenId.Var)
                    {
                        linha = $"{ident}float ";
                        foreach (var v in VariaveisEscopo)
                            linha += $"{v.Lexograma}, ";
                        linha = linha.Remove(linha.Length - 2, 2);
                        VariaveisEscopo.Clear();
                    }
                    break;
                case TokenId.Integer:
                    if (TokenEscopo.Id == TokenId.Var)
                    {
                        linha = $"{ident}int ";
                        foreach (var v in VariaveisEscopo)
                            linha += $"{v.Lexograma}, ";
                        linha = linha.Remove(linha.Length - 2, 2);
                        VariaveisEscopo.Clear();
                    }
                    break;
                case TokenId.DoisPontos:
                    break;
                case TokenId.Const:
                    TokenEscopo = token;
                    break;
                case TokenId.EOF:
                    
                    break;
                default:
                    break;
            }

            File.AppendAllText(path, linha);
        }

        public void AddSemantico(string lex, string tip)
        {
            TokenId id;
            if (tip == "integer") id = TokenId.Integer;
            else id = TokenId.Real;
            var token = new Token(null, lex, (int)id, -1);
            VariaveisSemantico.Add(token);
        }
    }
}
