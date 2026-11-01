using Analizador_Lexico;
using Analizador_Semantico;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;

namespace Analizador_sintactico
{
    //captador de errores clase dante (categoria especial)
    public class Errorsintaxis : Exception
    {
        public int Codigo { get; }
        public int Linea { get; }
        public Errorsintaxis(int error, int linea) : base(Errores.Mensaje(error))
        {
            Codigo = error;
            Linea = linea;
        }
    }

    internal class Sintactico
    {
        Nodo p;
        int linea_antigua = 1;

        // token actual, o 0 si ya no hay tokens para evitar pedir informacion que no existe
        private int Actual
        {
            get { return p == null ? 0 : p.token; }
        }

        // linea del token actual, o la del ultimo consumido para consultar errores
        private int Linea
        {
            get { return p == null ? linea_antigua : p.linea; }
        }


        //se puede hacer con list,arreglo o etc pero pus esta mejor este a gran escala
        //iniciadores de sentencias
        private static readonly HashSet<int> valores = new HashSet<int>
        {
        tokens.Id,
        tokens.INT,
        tokens.STR,
        tokens.REAL,
        tokens.If,
        tokens.While,
        tokens.Repeat,
        tokens.For,
        tokens.Exit,
        tokens.Write,
        tokens.Read,
        tokens.llave_izq
        };

        private static readonly Dictionary<int, int> CodigoDeError = new Dictionary<int, int>
{
    { tokens.Id,             514 },
    { tokens.asignar,        513 },
    { tokens.punto_coma,     508 },
    { tokens.coma,           506 },
    { tokens.parentesis_izq, 515 },
    { tokens.parentesis_der, 515 },
    { tokens.llave_izq,      507 },
    { tokens.llave_der,      507 },
    { tokens.Then,           509 },
    { tokens.Do,             510 },
    { tokens.Until,          511 },
    { tokens.To,             512 }
};

        //constructor 
        public Sintactico(Nodo cabeza)
        {
            p = cabeza;
        }

        private bool Final()
        {
            return p == null;
        }

        private void Avanzar()
        {
            if (p != null)
            {
                linea_antigua = p.linea;
                p = p.sig;
            }
        }

        //Zona segura y sincronizar, partes del boton de panico
        private bool zona_segura(int token)
        {
            return valores.Contains(token)
                || token == tokens.punto_coma
                || token == tokens.llave_der;
        }
        private void Sincronizar()
        {
            //descartara los tokens hasta llegar a un punto seguro un fin de llave o ;
            while (!Final() && !zona_segura(p.token))
            {
                Avanzar();
            }
            if (!Final() && p.token == tokens.punto_coma )
            {
                Avanzar();
            }
        }
        //finalizacion de la funcionalidad de boton de panico

        //errores
        private void Revision(int tokenbien)
        {
            if (Final())
            {
                throw new Errorsintaxis(502, Linea);
            }

            if (p.token != tokenbien)
            {
                int codigo;
                if (!CodigoDeError.TryGetValue(tokenbien, out codigo))
                {
                    codigo = 505;//parche por si acaso tecnicamente nunca pasara ya esta el filtro del sintactico 
                }
                throw new Errorsintaxis(codigo, p.linea);
            }
            Avanzar();
        }

        // diagrama de bloques copia y pega en codiog
        //va de arriba a abajo o de exterior a interior como matrioska
        public void Programa()
        {
            while (!Final())
            {
                Lista_Sentencia();
                if (!Final())
                {
                    // token que no inicia sentencia ('}' suelto, 'else' solo...)
                    Errores.Agregar(505, p.linea);
                    Avanzar();
                }
            }
        }
        private void Lista_Sentencia()
        {
            //si existe algo y ademas es un token iniciador de sentencia prosigue 
            while (!Final() && valores.Contains(p.token))
            {
                Nodo inicio = p;
                try
                {
                    Sentencia();
                }
                catch (Errorsintaxis ex)
                {
                    Errores.Agregar(ex.Codigo, ex.Linea);
                    Sincronizar();

                    // anti bucle infinito: si no avanzo nada, se come un token
                    if (p == inicio)
                    {
                        Avanzar();
                    }
                }
            }
        }

        private void Sentencia()
        {
            switch (p.token)
            {
                case tokens.Id:
                    Asignacion();
                    break;
                //todos caen al mismo codigo, sino se cierra con break 
                case tokens.INT:
                case tokens.STR:
                case tokens.REAL:
                    Sentencia_Declaracion();
                    break;
                case tokens.If:
                    Sentencia_Si();
                    break;
                case tokens.While:
                    Sentencia_Mientras();
                    break;
                case tokens.Repeat:
                    Sentencia_Repetir();
                    break;
                case tokens.For:
                    Sentencia_Desde();
                    break;
                case tokens.Exit:
                    Sentencia_Salir();
                    break;
                case tokens.Write:
                    Sentencia_Escribir();
                    break;
                case tokens.Read:
                    Sentencia_Leer();
                    break;
                case tokens.llave_izq:
                    Bloque();
                    break;
                default:
                    throw new Errorsintaxis(505, Linea);//por sis acaso aqui se usa parte del lexico igual no lo va a tocar el sintactico nunca
            }
        }
        private void Asignacion()
        {
            int linea = Linea;
            Lista_Variables variable = Usar_variable();//verificaciones de declaracion si no esta declarada
            Revision(tokens.asignar);
            string tipo = expresion_completa();
            if (variable != null && tipo != null && !Compatible(variable.Tipo, tipo))
            {
                Errores.Agregar(519, linea);
            }
            Revision(tokens.punto_coma);
        }

        private void Sentencia_Declaracion()
        {
            string tipo = p.lexema;
            Avanzar();
            Declarar_variable(tipo);//verificacione de duplicadaos
            while (Actual == tokens.coma)
            {
                Avanzar();
                Declarar_variable(tipo);
            }
            Revision(tokens.punto_coma);
        }
        //no es del bnf es apoyo del semantico en el error de variables duplicadas
        private void Declarar_variable(string tipo)
        {
            string nombre = null;
            int linea = Linea;
            if (Actual == tokens.Id) { nombre = p.lexema; }
            Revision(tokens.Id);

            if (!existe_nodo(nombre))
            {
                insertar_nodo(nombre, tipo);
            }
            else
            {//error de variable ya declarada
                Errores.Agregar(518, linea);
            }
        }


        /* este metodo queda obsoleto se fuciono con sentencia declaracion despues de todo ya se valida en el switch no tiene sentido 
        private void Tipo()
        {
            if (p.token == tokens.INT || p.token == tokens.REAL || p.token == tokens.STR)
            {
                Avanzar();
            }
            else
            {
                throw new Errorsintaxis(507, p.linea, p.lexema);
            }
        }*/
        private void Sentencia_Si()
        {
            Revision(tokens.If);
            expresion_completa();
            Revision(tokens.Then);
            Bloque();
            Parte_Sino();
        }
        private void Parte_Sino()
        {
            if (Actual == tokens.Else)
            {
                Avanzar();
                Bloque();
            }
        }
        private void Sentencia_Mientras()
        {
            Revision(tokens.While);
            expresion_completa();
            Revision(tokens.Do);
            Bloque();
        }
        private void Sentencia_Repetir()
        {
            Revision(tokens.Repeat);
            Bloque();
            Revision(tokens.Until);
            expresion_completa();
            Revision(tokens.punto_coma);
        }

        private void Sentencia_Desde()
        {
            Revision(tokens.For);
            Usar_variable();
            Revision(tokens.asignar);
            expresion_completa();
            Revision(tokens.To);
            expresion_completa();

            if (Actual == tokens.Step)
            {
                Avanzar();
                expresion_completa();
            }
            Revision(tokens.Do);
            Bloque();
        }

        private void Sentencia_Salir()
        {
            Revision(tokens.Exit);
            Revision(tokens.punto_coma);
        }

        private void Sentencia_Escribir()
        {
            Revision(tokens.Write);
            Revision(tokens.parentesis_izq);
            expresion_completa();
            Revision(tokens.parentesis_der);
            Revision(tokens.punto_coma);
        }

        private void Sentencia_Leer()
        {
            Revision(tokens.Read);
            Revision(tokens.parentesis_izq);
            Usar_variable();
            Revision(tokens.parentesis_der);
            Revision(tokens.punto_coma);
        }
        private void Bloque()
        {
            Revision(tokens.llave_izq);
            Lista_Sentencia();
            Revision(tokens.llave_der);
        }
        private void Expresion()
        {
            Expresion_And();
            Expresion_Or_Tail();
        }

        private void Expresion_And()
        {
            Expresion_Not();
            Expresion_And_Tail();
        }

        private void Expresion_Or_Tail()
        {
            if (Actual == tokens.or)
            {
                Avanzar();
                Expresion_And();
                Expresion_Or_Tail();
            }
        }

        private void Expresion_And_Tail()
        {
            if (Actual == tokens.and)
            {
                Avanzar();
                Expresion_Not();
                Expresion_And_Tail();
            }
        }

        private void Expresion_Not()
        {
            if (Actual == tokens.not)
            {
                Avanzar();
                Expresion_Not();
            }
            else
            {
                Expresion_Relacional();
            }
        }

        private void Expresion_Relacional()
        {
            Expresion_Simple();
            Expresion_Relacional_Tail();
        }

        private void Expresion_Relacional_Tail()
        {
            if (Actual == tokens.igual || Actual == tokens.diferente ||
                Actual == tokens.menor || Actual == tokens.menor_igual ||
                Actual == tokens.mayor || Actual == tokens.mayor_igual)
            {
                int token = Actual;
                string lexema = p.lexema;
                int linea = Linea;
                Avanzar();
                Expresion_Simple();
                insertar_polish(token, lexema, linea);
            }
        }

        private void Expresion_Simple()
        {
            Termino();
            Expresion_Simple_Tail();
        }

        private void Expresion_Simple_Tail()
        {
            if (Actual == tokens.mas || Actual == tokens.menos)
            {
                int token = Actual;
                string lexema = p.lexema;
                int linea = Linea;
                Avanzar();
                Termino();
                insertar_polish(token, lexema, linea);
                Expresion_Simple_Tail();
            }
        }

        private void Termino()
        {
            Factor();
            Termino_Tail();
        }

        private void Termino_Tail()
        {
            if (Actual == tokens.por || Actual == tokens.div)
            {
                int token = Actual;
                string lexema = p.lexema;
                int linea = Linea;
                Avanzar();
                Factor();
                insertar_polish(token, lexema, linea);
                Termino_Tail();
            }
        }

        private void Factor()
        {
            if (Actual == tokens.parentesis_izq)
            {
                Avanzar();
                Expresion();
                Revision(tokens.parentesis_der);
            }
            else if (Actual == tokens.Id)
            {
                int token = Actual;
                string lexema = p.lexema;
                int linea = Linea;
                Usar_variable();
                insertar_polish(token, lexema, linea);
            }
            else if (Actual == tokens.NR || Actual == tokens.NE || Actual == tokens.Cadena)
            {
                int token = Actual;
                string lexema = p.lexema;
                int linea = Linea;
                Avanzar();
                insertar_polish(token, lexema, linea);
            }
            else if (Final())
            {
                throw new Errorsintaxis(502, Linea);
            }
            else
            {
                throw new Errorsintaxis(516, Linea);
            }
        }



        //Funciones semanticas
        //error variable no asignada y variable duplicada
        Lista_Variables cabeza_variable = null, p_variable = null;
        public void insertar_nodo(string Nombre, string Tipo)
        {
            Lista_Variables nodo = new Lista_Variables(Nombre, Tipo, null);

            if (cabeza_variable == null)
            {
                cabeza_variable = nodo;
                p_variable = cabeza_variable;
            }
            else
            {
                p_variable.sig = nodo;
                p_variable = nodo;
            }
        }//funcion

        //primer error dice si se declaro la variable, el existe nodo de abajo es el mismo pero sin resumir o optimizar
        public bool existe_nodo(string Nombre)
        {
            return buscar_nodo(Nombre) != null;
        }
        /*
        public bool existe_nodo(string Nombre) {
            Lista_Variables aux = cabeza_variable;
            while (aux != null)
            {
                if(aux.Nombre == Nombre) {  return true; }

                 aux = aux.sig;
            }
            return false;
        }//funcion 
        */
        public Lista_Variables buscar_nodo(string Nombre) {
            Lista_Variables aux = cabeza_variable;
            while (aux != null)
            {
                if (aux.Nombre == Nombre) { return aux; }

                aux = aux.sig;
            }
            return null;
        }//funcion

        //funcion para checar si la varible existe o no en la lista
        private Lista_Variables Usar_variable()
        {
            string nombre = null;
            int linea = Linea;
            if (Actual == tokens.Id)
            {
                nombre = p.lexema;
            }
                Revision(tokens.Id);

            Lista_Variables variable = buscar_nodo(nombre);
            if (variable == null)
            {
                // variable no declarasa 
                Errores.Agregar(517, linea);   
            }
            return variable;   
        }




        //tercer error hacia abajo incompatibilidad de tipos
        //primero funciones del polish 
        Polish cabeza_polish = null, p_polish = null;

        private void insertar_polish (int token, string lexema, int linea)
        {
            Polish nodo = new Polish(token,lexema, linea, null);

            if (cabeza_polish == null)
            {
                cabeza_polish = nodo;
                p_polish = cabeza_polish;
            }
            else
            {
                p_polish.sig = nodo;
                p_polish = nodo;
            }
        }

        private void Vaciar_polish()
        {
            cabeza_polish = null;
            p_polish = null;

        }

        //funciones del axuiliar la lista de tipos
        Pila_tipos cabeza_tipos = null;
        private void Push(string Tipo)
        {
            cabeza_tipos = new Pila_tipos(Tipo, cabeza_tipos);
        }

        private string Pop()
        {
            if (cabeza_tipos == null) return null;   

            string tipo = cabeza_tipos.Tipo;
            cabeza_tipos = cabeza_tipos.sig;
            return tipo;
        }
        private void Vaciar_pila()
        {
            cabeza_tipos = null;
        }

        private string expresion_completa()
        {
            Vaciar_polish();
            Expresion();
            return Evaluar_polish();
        }

        private string Evaluar_polish()
        {
            Vaciar_pila();
            Polish aux = cabeza_polish;
            while (aux != null)
            {
                if(aux.token == tokens.NE) { Push("int"); } else
                if (aux.token == tokens.NR) { Push("real"); } else
                if (aux.token == tokens.Cadena) { Push("str"); } else
                if(aux.token == tokens.Id)
                {
                    Lista_Variables variable = buscar_nodo(aux.lexema);
                    if(variable != null) { Push(variable.Tipo); } else { Push(null); }
                }
                else
                {
                string der = Pop();
                string izq = Pop();
                string res = Resultado_Tipo(Verificacion_Tipo(aux.token), izq, der);
                 if (izq != null && der != null && res == null)
                 {
                 Errores.Agregar(519, aux.Linea);
                 }
                 Push(res);
                 }
                aux = aux.sig;
            }
            return Pop();

        }

        // matrices de tipos 
        string[,] suma = new string[3,3]
        {//el orden de todos es int str real de izq a der y de up a down int str real
            {"int",null,"real" },
            {null,"str",null },
            {"real",null,"real" }
        };

        string[,] resta_por = new string[3, 3]
        {
            {"int",null,"real" },
            {null,null,null },
            {"real",null,"real" }
        };
        string[,] div = new string[3, 3]
        {
            {"real",null,"real" },
            {null,null,null },
            {"real",null,"real" }
        };
        string[,] comparaciones = new string[3, 3]
        {
            {"bool",null,null },
            {null,"bool",null },
            {null,null,"bool" }
        };
        private int Indice(string tipo)
        {
            switch (tipo)
            {
                case "int": return 0;
                case "str": return 1;
                case "real": return 2;
                default: return -1;   // null o "bool": no están en las matrices
            }
        }
        private bool Compatible(string tipo, string valor)
        {
            if (tipo == "int") return valor == "int";
            if (tipo == "real") return valor == "real" || valor == "int";
            if (tipo == "str") return valor == "real" || valor == "int" || valor == "str";

            return false;
        }
        private string Resultado_Tipo(string[,] matriz, string izq, string der)
        {
            int i = Indice(izq);
            int j = Indice(der);
            if (i < 0 || j < 0) return null;
            return matriz[i, j];
        }

        private string[,] Verificacion_Tipo(int operador)
        {
            switch (operador)
            {
                case tokens.mas: return suma;
                case tokens.menos:
                case tokens.por: return resta_por;
                case tokens.div: return div;
                default: return comparaciones;   
            }
        }

        //nota no se puso la conversion implicita entonces el real y el int no pueden ser semejantes
    }//clase sintactico 
}//namespace
