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
        private Lexico Lex;
        private Semantico Sem;
        private bool inVar;
        private bool inConst;
        private bool intRecebendoEsq;
        private bool intEsq;
        private int k;
        private List<string> tempVars;

        public Sintatico(Lexico lx)
        {
            this.inVar = false;
            this.inConst = false;
            this.tempVars = new List<string>();
            this.Sem = new Semantico();
            this.Lex = lx;
            this.tk = lx.NextToken();
            if (tk == null)
                Console.WriteLine("Arquivo vazio");
        }

        private void ErroSint(Token tk, String esperado)
        {
            Console.WriteLine("Erro sintatico na linha: " + tk.Linha + "\nSimbolo " + tk.Lexograma + " Inesperado.");
            Console.WriteLine("token \"" + esperado + "\" esperado\n");
            
        }
        private void ErroSint(Token tk)
        {
            Console.WriteLine("Erro sintatico na linha: " + tk.Linha + "\nSimbolo " + tk.Lexograma + " Inesperado.");
            
        }
        private bool Reconhece(String token)
        {
            if (tk.Descricao == token)
            {
                tk = Lex.NextToken();
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
                tk = Lex.NextToken();
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
                string tempLex = tk.Lexograma;
                string tempTipo;
                IDENT(false);
                if (Reconhece("="))
                {
                    if (tk.Descricao == "numfloat" || tk.Descricao == "numint")
                    {
                        if (tk.Descricao == "numfloat")
                            tempTipo = "real";
                        else
                            tempTipo = "integer";
                        NUM();
                        if (!Reconhece(";"))
                        {
                            while (tk.Descricao != "EOF" && tk.Descricao != "begin" && tk.Descricao != "var")
                            {
                                tk = Lex.NextToken();
                            }
                        }
                        Sem.AddNaTabela(tempLex, tempTipo, true, k);
                        //sem.imprimeTabela();
                    }
                    else
                    {
                        ErroSint(tk, "numero");
                        while (tk.Descricao != "EOF" && tk.Descricao != "begin" && tk.Descricao != "var")
                        {
                            tk = Lex.NextToken();
                        }
                    }
                }
                else
                {
                    ErroSint(tk, "=");
                    while (tk.Descricao != "EOF" && tk.Descricao != "begin" && tk.Descricao != "var")
                    {
                        tk = Lex.NextToken();
                    }
                }
            }
            else
            {
                ErroSint(tk, "variavel");
                while (tk.Descricao != "EOF" && tk.Descricao != "begin" && tk.Descricao != "var")
                {
                    tk = Lex.NextToken();
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
                    tk = Lex.NextToken();
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
                    Sem.LimpaEscopo(j);
                    return;
                }
                else
                {
                    ErroSint(tk, "end");
                    while (tk.Descricao != "EOF" && tk.Descricao != ";" && tk.Descricao != "else" && tk.Descricao != ".")
                    {
                        tk = Lex.NextToken();
                    }
                }
            }
            else
            {
                ErroSint(tk, "begin");
                while (tk.Descricao != "EOF" && tk.Descricao != ";" && tk.Descricao != "else" && tk.Descricao != ".")
                {
                    tk = Lex.NextToken();
                }
            }
            //sem.imprimeTabela();
            Sem.LimpaEscopo(j);
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
                        tk = Lex.NextToken();
                    }
                }
            }
        }
        public bool COM()
        {
            switch (tk.Descricao)
            {
                case "print":
                    tk = Lex.NextToken();
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
                                tk = Lex.NextToken();
                            }
                        }
                    }
                    else
                    {
                        ErroSint(tk, "(");
                        while (tk.Descricao != "EOF" && tk.Descricao != ";")
                        {
                            tk = Lex.NextToken();
                        }
                    }
                    return true;
                case "if":
                    tk = Lex.NextToken();
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
                            tk = Lex.NextToken();
                        }
                    }
                    return true;
                case "for":
                    tk = Lex.NextToken();
                    IDENT(true);
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
                                    tk = Lex.NextToken();
                                }
                            }
                        }
                        else
                        {
                            ErroSint(tk, "to");
                            while (tk.Descricao != "EOF" && tk.Descricao != ";")
                            {
                                tk = Lex.NextToken();
                            }
                        }
                    }
                    else
                    {
                        ErroSint(tk, ":=");
                        while (tk.Descricao != "EOF" && tk.Descricao != ";")
                        {
                            tk = Lex.NextToken();
                        }
                    }
                    return true;
                case "read":
                    tk = Lex.NextToken();
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
                                tk = Lex.NextToken();
                            }
                        }
                    }
                    else
                    {
                        ErroSint(tk, "(");
                        while (tk.Descricao != "EOF" && tk.Descricao != ";")
                        {
                            tk = Lex.NextToken();
                        }

                    }
                    return true;
                case "variavel":
                    IDENT(true);
                    if (Reconhece(":="))
                    {
                        EXP();
                    }
                    else
                    {
                        ErroSint(tk, ":=");
                        while (tk.Descricao != "EOF" && tk.Descricao != ";")
                        {
                            tk = Lex.NextToken();
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
                for (int i = 0; i < this.tempVars.Count; i++)
                {
                    Sem.AddNaTabela(this.tempVars[i], "integer", false, k);
                }
                this.tempVars.Clear();
                Reconhece("integer");
            }
            else
            {
                if (tk.Descricao == "real")
                {
                    Reconhece("real");
                    for (int i = 0; i < this.tempVars.Count; i++)
                    {
                        Sem.AddNaTabela(this.tempVars[i], "real", false, k);
                    }
                    this.tempVars.Clear();
                }
                else
                {
                    ErroSint(tk);
                    while (tk.Descricao != "EOF" && tk.Descricao != "begin" && tk.Descricao != ")")
                    {
                        tk = Lex.NextToken();
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
                    tk = Lex.NextToken();
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
            IDENT(false);
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
                        tk = Lex.NextToken();
                    }
                    return;
                }
            }
            else
            {
                if (tk.Descricao == "variavel")
                {
                    
                    if(Sem.ChecaTabela(tk.Lexograma) != -1)
                    {
                        intEsq = Sem.TokenInteger(tk.Lexograma);
                        if(intRecebendoEsq == true && intEsq == false)
                        {
                            Console.WriteLine("WARNING: Atribuição entre tipos diferentes na linha " + tk.Linha + ".\n");

                        }
                    }
                    IDENT(false);
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
                            tk = Lex.NextToken();
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
            if(tk.Descricao == "=" || tk.Descricao == "<>" || tk.Descricao == "<" ||
                tk.Descricao == ">" || tk.Descricao == "<=" || tk.Descricao == ">=")
            {
                tk = Lex.NextToken();
                if(tk.Descricao == "variavel")
                {
                    bool intDir = Sem.TokenInteger(tk.Lexograma);
                    if (intEsq != intDir) //Comparação entre tipos diferentes.
                    {
                        Console.WriteLine("WARNING: Comparação entre tipos diferentes na linha " + tk.Linha + ".\n");
                    }
                }
            }
            else
            {
                ErroSint(tk);
                    while (tk.Descricao != "EOF" && tk.Descricao != "variavel")
                    {
                        tk = Lex.NextToken();
                    }
            }
        }
        public void IDENT(bool estaRecebendo)
        {
            CARACTER();
            IDENT1(estaRecebendo);
        }
        public void IDENT1(bool estaRecebendo)
        {
            Token temp = tk;
            if (Reconhece("variavel"))
            {
                if (inVar)
                {
                    this.tempVars.Add(temp.Lexograma);
                }
                IDENT(estaRecebendo);
                /*
                if (temp.Linha > 26)
                {
                    int a;
                    a = 10;
                }
                */
                if (!inVar && !inConst)
                {
                    if (Sem.ChecaTabela(temp.Lexograma) == -1)
                    {
                        Console.WriteLine("Erro semantico na linha " + temp.Linha);
                        Console.WriteLine("Variavel: " + temp.Lexograma + "  nao declarada\n");
                    }
                    else if (estaRecebendo == true)
                    {
                        if (Sem.TokenConstante(temp.Lexograma))
                        {
                            Console.WriteLine("Erro semantico na linha " + temp.Linha);
                            Console.WriteLine("Tentativa de atribuição a constante " + temp.Lexograma + "\n");
                        }
                        else if (Sem.TokenInteger(temp.Lexograma))
                        {
                            intRecebendoEsq = true; //Variavel global para informar sem tem inteiro recebendo a esq
                        }
                        else
                        {
                            intRecebendoEsq = false;
                        }
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
