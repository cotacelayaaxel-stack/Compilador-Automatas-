using Analizador_sintactico;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Analizador_Lexico
{
    internal class Lexico
    {
        //x apuntador, edo estado, MT matriz [col][fila],caracter pus el char de la posicion x
        //lexema secuencia de texto que cumple un patron para asignarle un token
        /*
         el x apuntara la posicion en la que el archivo va para analizarla, el char tomara el valor de dicha posicion
         y depues se alamacena en un lexema el cual ira aumentando su tamano mientras x avance hasta terminar con una palabra o un 
        simbolo unico
         */
        // el eo termina el lexema, el nl tambien y aumenta la linea++ y el enter 
        int edo = 0, col, MT, x = 0, linea = 1;
        string lexema = "", archivo = "";
        char caracter;


        int[,] matriz = new int[11, 25]{
//  letra dig  _    .    +    -    *    /    >    <    =    :    ;    "    (    )    {    }    ,    eb   tab  newL re   eof  oc
    { 1,   2,   505, 505, 201, 202, 203, 5,   7,   8,   209, 9,   301, 10,  302, 303, 304, 305, 307, 0,   0,   0,   0,   0,   505 },
    { 1,   1,   1,   101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101 },
    { 102, 2,   102, 3,   102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102 },
    { 501, 4,   501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501 },
    { 103, 4,   103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103 },
    { 204, 204, 204, 204, 204, 204, 204,   6, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204 },
    { 6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   6,   0,   0,   502, 6   },
    { 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 208, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205 },
    { 206, 206, 206, 206, 206, 206, 206, 206, 206, 210, 207, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206 },
    { 503, 503, 503, 503, 503, 503, 503, 503, 503, 503, 214, 503, 503, 503, 503, 503, 503, 503, 503, 503, 503, 503, 503, 503, 503 },
    { 10,  10,  10,  10,  10,  10,  10,  10,  10,  10,  10,  10,  10, 104,  10,  10,  10,  10,  10,  10,  10,  504, 504, 502, 505 }
};

        //palabras reservadas
        Dictionary<string, int> PalabrasResevadas = new Dictionary<string, int>
        {
            {"str", 401 },
            {"int", 402 },
            {"real", 403 },
            {"if", 404 },
            {"then", 405 },
            {"else", 406 },
            {"while", 407 },
            {"do", 408 },
            {"repeat", 409 },
            {"until", 410 },
            {"for", 411 },
            {"to", 412 },
            {"step", 413 },
            {"exit", 414 },
            {"write", 415 },
            {"read", 416 },
            {"and", 211 },
            {"or", 212 },
            {"not", 213 }
        };

        //constructor 
        public Lexico(string texto)
        {
            archivo = texto;
        }

        public void Analizar()
        {

            while (x < archivo.Length)
            {
                caracter = archivo[x];
                int edo_antiguo = edo;
                //edoantiguo anterior estado por correccion a simbolos unitarios 

                if (Char.IsLetter(caracter))
                {
                    col = 0;
                }
                else
                    if (Char.IsDigit(caracter))
                    {
                        col = 1;
                    }
                    else
                    {
                        switch (caracter)
                        {
                            case '_':
                                col = 2;
                                break;
                            case '.':
                                col = 3;
                                break;
                            case '+':
                                col = 4;
                                break;
                            case '-':
                                col = 5;
                                break;
                            case '*':
                                col = 6;
                                break;
                            case '/':
                                col = 7;
                                break;
                            case '>':
                                col = 8;
                                break;
                            case '<':
                                col = 9;
                                break;
                            case '=':
                                col = 10;
                                break;
                            case ':':
                                col = 11;
                                break;
                            case ';':
                                col = 12;
                                break;
                            case '"':
                                col = 13;
                                break;
                            case '(':
                                col = 14;
                                break;
                            case ')':
                                col = 15;
                                break;
                            case '{':
                                col = 16;
                                break;
                            case '}':
                                col = 17;
                                break;
                            case ',':
                                col = 18;
                                break;
                            case ' ':
                                col = 19;
                                break;
                            case '\t':
                                col = 20;
                                break;
                            case '\n':
                                col = 21;
                                linea++;
                                break;
                            case '\r':
                                col = 22;
                                break;
                            //el eof no, es una condicion
                            default:
                                col = 24;
                                break;
                        }//switch
                    }

                MT = matriz[edo, col];
                //transicion o estado

                if (MT == 0)
                {
                    edo = 0;
                    lexema = "";
                }
                else if (MT > 0 && MT < 100)
                {
                    edo = MT;
                    lexema = lexema + caracter;
                }
                else if (MT > 100 && MT < 500)//token
                {
                    bool final = MT == 104 || MT == 207 || MT == 208 || MT == 214;

                    if (edo_antiguo == 0 || final)
                    {
                        lexema = lexema + caracter;
                    }
                    else
                    {
                        x = x - 1;
                    }

                    int token;
                    if (PalabrasResevadas.TryGetValue(lexema, out token))
                    {
                        insertar_nodo(lexema, token, linea);
                    }
                    else
                    {
                        insertar_nodo(lexema, MT, linea);
                    }

                    edo = 0;
                    lexema = "";
                }
                else if (MT > 500)
                {//error
                    Errores.Agregar(MT, linea);

                    if (edo_antiguo != 0)
                    {
                        x = x - 1;
                        if (caracter == '\n') linea--;
                    }
                    edo = 0;
                    lexema = "";
                }
                x++;//avance
            }//while

            //por si eof y aun hay algo
            if (lexema != "")
            {
                MT = matriz[edo, 23];

                int token;
                if (MT > 500)
                {
                    Errores.Agregar(MT, linea);
                }
                if (PalabrasResevadas.TryGetValue(lexema, out token))
                {
                    insertar_nodo(lexema, token, linea);
                }
                else
                {
                    insertar_nodo(lexema, MT, linea);
                }
            }

        }//funcion

        //apuntadores de cabeza y cola
        Nodo cabeza = null, p = null;
        public void insertar_nodo(string lexema, int token, int linea)
        {
            Nodo nodo = new Nodo(lexema, token, linea, null);

            if (cabeza == null)
            {
                cabeza = nodo;
                p = cabeza;
            }
            else
            {
                p.sig = nodo;
                p = nodo;
            }
        }//funcion

        public Nodo Cabeza
        {
            get { return cabeza; }
        }
        /*
         * solo usado en la clase lexico para comprobar sus resultados
        public void imprimir()
        {
            Nodo actual = cabeza;
            while (actual != null)
            {
                if (actual.token > 500)
                {
                    string error = Errores[actual.token];
                    Console.WriteLine($"Error :{actual.token} con : {actual.lexema} en linea: {actual.linea}, razon: {error}  ");
                }
                else
                {
                    Console.WriteLine($"Lexema: {actual.lexema}      Token: {actual.token}       linea: {actual.linea} ");
                }
                actual = actual.sig;
            }
        }
        */



    }//clase lexico


}
