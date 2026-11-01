using Analizador_Lexico;
using System;
using System.Collections.Generic;
using System.Text;

namespace Analizador_Semantico
{
    public class Lista_Variables
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public Lista_Variables sig { get; set; }
        public Lista_Variables(string Nombre,string Tipo, Lista_Variables sig)
        {
            this.Nombre = Nombre;
            this.Tipo = Tipo;
            this.sig = sig;
        }

    }//lista de variable

    public class Polish
    {
        public int token { get; set; }
        public string lexema { get; set; }
        public int Linea { get; set; }
        public Polish sig { get; set; }

        public Polish(int token, string lexema, int linea, Polish sig)
        {
            this.token = token;
            this.Linea = linea;
            this.lexema = lexema;
            this.sig = sig;
        }
    }//polish

    //es la pila del polish como aux
    public class Pila_tipos
    {
        public string Tipo { get; set; }
        public Pila_tipos sig { get; set; }

        public Pila_tipos(string Tipo, Pila_tipos sig)
        {
            this.sig = sig;
            this.Tipo = Tipo;
        }

    }
}
