using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class Sintatico
    {
        private Token tk;
        private Lexico lx;
        private Semantico sem;
        private bool inVar;
        private bool inConst;
        private int k;
        private List<string> tempVars;

        public Sintatico(Lexico lx)
        {
            this.inVar = false;
            this.inConst = false;
            this.tempVars = new List<string>();
            this.sem = new Semantico();
            this.lx = lx;
            this.tk = lx.NextToken();
            if (tk == null)
                Console.WriteLine("Arquivo vazio");
        }

        private void ErroSint(Token tk, String esperado)
        {
            Console.WriteLine("Erro sintatico na linha: " + tk.Linha + "\nSimbolo " + tk.Lexograma + " Inesperado.");
            Console.WriteLine("token \"" + esperado + "\" esperado\n");
            //System.exit(1);
        }
        private void ErroSint(Token tk)
        {
            Console.WriteLine("Erro sintatico na linha: " + tk.Linha + "\nSimbolo " + tk.Lexograma + " Inesperado.");
            //System.exit(1);
        }
        private bool Reconhece(String token)
        {
            if (tk.Descricao == token)
            {
                tk = lx.NextToken();
                return true;
            }
            else
            {
                return false;
            }
        }
        public void PROG()
        {
            CONST();
            BLOCO();
            if (!Reconhece("."))
            {
                ErroSint(tk, ".");
            }
            else
            {
                if (!Reconhece("EOF"))
                {
                    ErroSint(tk, "Fim de arquivo");
                }
            }
            while (tk.Descricao != "EOF")
            {
                tk = lx.NextToken();
            }
        }
        public void CONST()
        {
            if (tk.Descricao == "const")
            {
                Reconhece("const");
                inConst = true;
                LISTA_CONST();
                inConst = false;
            }
        }
        public void LISTA_CONST()
        {
            DEF_CONST();
            LISTA_CONST2();
        }
        public void DEF_CONST()
        {
            if (tk.Descricao == "variavel")
            {
                String tempLex = tk.Lexograma;
                String tempTipo;
                IDENT();
                if (Reconhece("="))
                {
                    if (tk.Descricao == "numfloat" || tk.Descricao == "numint")
                    {
                        if (tk.Descricao == "numfloat") tempTipo = "real";
                        else tempTipo = "integer";
                        NUM();
                        if (!Reconhece(";"))
                        {
                            while (tk.Descricao != "EOF" && tk.Descricao != "begin" && tk.Descricao != "var")
                            {
                                tk = lx.NextToken();
                            }
                        }
                        sem.AddNaTabela(tempLex, tempTipo, 1, k);
                        //sem.imprimeTabela();
                    }
                    else
                    {
                        ErroSint(tk, "numero");
                        while (tk.Descricao != "EOF" && tk.Descricao != "begin" && tk.Descricao != "var")
                        {
                            tk = lx.NextToken();
                        }
                    }
                }
                else
                {
                    ErroSint(tk, "=");
                    while (tk.Descricao != "EOF" && tk.Descricao != "begin" && tk.Descricao != "var")
                    {
                        tk = lx.NextToken();
                    }
                }
            }
            else
            {
                ErroSint(tk, "variavel");
                while (tk.Descricao != "EOF" && tk.Descricao != "begin" && tk.Descricao != "var")
                {
                    tk = lx.NextToken();
                }
            }
        }
        public void LISTA_CONST2()
        {
            if (tk.Descricao == "variavel")
            {
                LISTA_CONST();
            }
        }
        public void VARS()
        {
            inVar = true;
            LISTAS_IDENT();
            inVar = false;
            if (!Reconhece(";"))
            {
                ErroSint(tk, ";");
                while (tk.Descricao != "EOF" && tk.Descricao != "begin")
                {
                    tk = lx.NextToken();
                }
            }
            if (tk.Descricao == "variavel")
            {
                VARS();
            }
        }
        public void BLOCO()
        {
            if (Reconhece("var"))
            {
                VARS();
            }
            int j = k;
            k = k + 1;
            if (Reconhece("begin"))
            {
                COMS();
                if (Reconhece("end"))
                {
                    //sem.imprimeTabela();
                    sem.limpaEscopo(j);
                    return;
                }
                else
                {
                    ErroSint(tk, "end");
                    while (tk.Descricao != "EOF" && tk.Descricao != ";" && tk.Descricao != "else" && tk.Descricao != ".")
                    {
                        tk = lx.NextToken();
                    }
                }
            }
            else
            {
                ErroSint(tk, "begin");
                while (tk.Descricao != "EOF" && tk.Descricao != ";" && tk.Descricao != "else" && tk.Descricao != ".")
                {
                    tk = lx.NextToken();
                }
            }
            //sem.imprimeTabela();
            sem.limpaEscopo(j);
        }
        public void COMS()
        {
            if (COM())
            {
                if (Reconhece(";"))
                {
                    COMS();
                }
                else
                {
                    ErroSint(tk, ";");
                    while (tk.Descricao != "EOF" && tk.Descricao != "end")
                    {
                        tk = lx.NextToken();
                    }
                }
            }
        }
        public bool COM()
        {
            switch (tk.Descricao)
            {
                case "print":
                    tk = lx.NextToken();
                    if (Reconhece("("))
                    {
                        LISTA_IDENT();
                        if (Reconhece(")"))
                        {
                            return true;
                        }
                        else
                        {
                            ErroSint(tk, ")");
                            while (tk.Descricao != "EOF" && tk.Descricao != ";")
                            {
                                tk = lx.NextToken();
                            }
                        }
                    }
                    else
                    {
                        ErroSint(tk, "(");
                        while (tk.Descricao != "EOF" && tk.Descricao != ";")
                        {
                            tk = lx.NextToken();
                        }
                    }
                    return true;
                case "if":
                    tk = lx.NextToken();
                    EXP_REL();
                    if (Reconhece("then"))
                    {
                        BLOCO();
                        ELSE_OPC();
                        return true;
                    }
                    else
                    {
                        ErroSint(tk, "then");
                        while (tk.Descricao != "EOF" && tk.Descricao != ";")
                        {
                            tk = lx.NextToken();
                        }
                    }
                    return true;
                case "for":
                    tk = lx.NextToken();
                    IDENT();
                    if (Reconhece(":="))
                    {
                        EXP();
                        if (Reconhece("to"))
                        {
                            EXP();
                            if (Reconhece("do"))
                            {
                                BLOCO();
                                return true;
                            }
                            else
                            {
                                ErroSint(tk, "do");
                                while (tk.Descricao != "EOF" && tk.Descricao != ";")
                                {
                                    tk = lx.NextToken();
                                }
                            }
                        }
                        else
                        {
                            ErroSint(tk, "to");
                            while (tk.Descricao != "EOF" && tk.Descricao != ";")
                            {
                                tk = lx.NextToken();
                            }
                        }
                    }
                    else
                    {
                        ErroSint(tk, ":=");
                        while (tk.Descricao != "EOF" && tk.Descricao != ";")
                        {
                            tk = lx.NextToken();
                        }
                    }
                    return true;
                case "read":
                    tk = lx.NextToken();
                    if (Reconhece("("))
                    {
                        LISTA_IDENT();
                        if (Reconhece(")"))
                        {
                            return true;
                        }
                        else
                        {
                            ErroSint(tk, ")");
                            while (tk.Descricao != "EOF" && tk.Descricao != ";")
                            {
                                tk = lx.NextToken();
                            }
                        }
                    }
                    else
                    {
                        ErroSint(tk, "(");
                        while (tk.Descricao != "EOF" && tk.Descricao != ";")
                        {
                            tk = lx.NextToken();
                        }

                    }
                    return true;
                case "variavel":
                    IDENT();
                    if (Reconhece(":="))
                    {
                        EXP();
                    }
                    else
                    {
                        ErroSint(tk, ":=");
                        while (tk.Descricao != "EOF" && tk.Descricao != ";")
                        {
                            tk = lx.NextToken();
                        }
                    }
                    return true;
            }
            return false;
        }
        public void ELSE_OPC()
        {
            if (Reconhece("else"))
            {
                BLOCO();
            }
        }
        public void LISTAS_IDENT()
        {
            DEF_LISTAS_IDENT();
            LISTAS_IDENT2();
        }
        public void LISTAS_IDENT2()
        {
            if (tk.Descricao == "variavel")
            {
                LISTAS_IDENT();
            }
        }
        public void DEF_LISTAS_IDENT2()
        {
            if (tk.Descricao == "integer")
            {
                for (int i = 0; i < this.tempVars.Capacity - 1; i++)
                {
                    sem.AddNaTabela(this.tempVars[i], "integer", 0, k);
                }
                // sem.imprimeTabela();
                this.tempVars.Clear();
                Reconhece("integer");
            }
            else
            {
                if (tk.Descricao == "real")
                {
                    Reconhece("real");
                    for (int i = 0; i < this.tempVars.Capacity - 1; i++)
                    {
                        sem.AddNaTabela(this.tempVars[i], "real", 0, k);
                    }
                    this.tempVars.Clear();
                }
                else
                {
                    ErroSint(tk);
                    while (tk.Descricao != "EOF" && tk.Descricao != "begin" && tk.Descricao != ")")
                    {
                        tk = lx.NextToken();
                    }
                }
            }
        }
        public void DEF_LISTAS_IDENT()
        {
            LISTA_IDENT();
            if (Reconhece(":"))
            {
                DEF_LISTAS_IDENT2();
            }
            else
            {
                ErroSint(tk, ":");
                while (tk.Descricao != "EOF" && tk.Descricao != "begin" && tk.Descricao != ")")
                {
                    tk = lx.NextToken();
                }
            }
        }
        public void LISTA_IDENT1()
        {
            if (Reconhece(","))
            {
                LISTA_IDENT();
            }
        }
        public void LISTA_IDENT()
        {
            IDENT();
            LISTA_IDENT1();
        }
        public void EXP1()
        {
            if (Reconhece("+"))
            {
                EXP();
            }
            else
            {
                if (Reconhece("-"))
                {
                    EXP();
                }
            }
        }
        public void EXP()
        {
            TERMO();
            EXP1();
        }
        public void TERMO()
        {
            FATOR();
            TERMO1();
        }
        public void TERMO1()
        {
            if (Reconhece("*"))
            {
                TERMO();
            }
            else
            {
                if (Reconhece("/"))
                {
                    TERMO();
                }
            }

        }
        public void FATOR()
        {
            if (Reconhece("("))
            {
                EXP();
                if (Reconhece(")"))
                {
                    return;
                }
                else
                {
                    ErroSint(tk, ")");// *  |  /  |  + | -  | to | do |  ; |   = | <> | < | > | <= | >=
                    while (tk.Descricao != "EOF" && tk.Descricao != "*" && tk.Descricao != "/" && tk.Descricao != "+" && tk.Descricao != "-"
                             && tk.Descricao != "to" && tk.Descricao != "do" && tk.Descricao != ";" && tk.Descricao != "=" && tk.Descricao != "<>"
                             && tk.Descricao != "<" && tk.Descricao != ">" && tk.Descricao != "<=" && tk.Descricao != ">=" && tk.Descricao != "then")
                    {
                        tk = lx.NextToken();
                    }
                    return;
                }
            }
            else
            {
                if (tk.Descricao == "variavel")
                {
                    IDENT();
                }
                else
                {
                    if (tk.Descricao == "numint" || tk.Descricao == "numfloat")
                    {
                        NUM();
                    }
                    else
                    {
                        ErroSint(tk);
                        while (tk.Descricao != "EOF" && tk.Descricao != "*" && tk.Descricao != "/" && tk.Descricao != "+" && tk.Descricao != "-"
                                 && tk.Descricao != "to" && tk.Descricao != "do" && tk.Descricao != ";" && tk.Descricao != "=" && tk.Descricao != "<>"
                                 && tk.Descricao != "<" && tk.Descricao != ">" && tk.Descricao != "<=" && tk.Descricao != ">=" && tk.Descricao != "then")
                        {
                            tk = lx.NextToken();
                        }
                    }
                }
            }

        }
        public void EXP_REL()
        {
            EXP();
            OP_REL();
            EXP();
        }
        public void OP_REL()
        {
            switch (tk.Descricao)
            {
                case "=":
                    tk = lx.NextToken();
                    break;
                case "<>":
                    tk = lx.NextToken();
                    break;
                case "<":
                    tk = lx.NextToken();
                    break;
                case ">":
                    tk = lx.NextToken();
                    break;
                case "<=":
                    tk = lx.NextToken();
                    break;
                case ">=":
                    tk = lx.NextToken();
                    break;
                default:
                    ErroSint(tk);
                    while (tk.Descricao != "EOF" && tk.Descricao != "variavel")
                    {
                        tk = lx.NextToken();
                    }
                    break;
            }
        }
        public void IDENT()
        {
            CARACTER();
            IDENT1();
        }
        public void IDENT1()
        {
            Token temp = tk;
            if (Reconhece("variavel"))
            {
                if (inVar)
                {
                    this.tempVars.Add(temp.Lexograma);
                }
                IDENT();
                // sem.imprimeTabela();
                if (temp.Linha > 26)
                {
                    int a;
                    a = 10;
                }
                if (!inVar && !inConst)
                {
                    if (sem.ChecaTabela(temp.Lexograma) == -1)
                    {
                        Console.WriteLine("Erro semantico na linha " + temp.Linha + "\nVariavel: " + temp.Lexograma + "  nao declarada\n");
                    }
                }
            }
        }
        public void CARACTER()
        {
        }
        public void NUM()
        {
            DIGITO();
            NUM1();
        }
        public void NUM1()
        {
            if (Reconhece("numint"))
            {
                NUMINT();
                NUM();
            }
            else
            {
                if (Reconhece("numfloat"))
                {
                    NUMFLOAT();
                    NUM();
                }

            }
        }
        public void NUMINT()
        {

        }
        public void NUMFLOAT()
        {

        }


        public void DIGITO()
        {
        }

    }
}
