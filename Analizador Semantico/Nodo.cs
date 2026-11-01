using System;
using System.Collections.Generic;
using System.Text;

namespace Analizador_Lexico
{
    //perteneciente al analizador lexico
    public class Nodo
    {
        public string lexema { get; set; }
        public int token { get; set; }
        public int linea { get; set; }
        public Nodo sig { get; set; }

        public Nodo(string lexema, int token, int linea, Nodo sig)
        {
            this.lexema = lexema;
            this.token = token;
            this.linea = linea;
            this.sig = sig;
        }
    }//clase

}
