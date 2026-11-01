using System;
using System.Collections.Generic;
using System.Text;

namespace Analizador_sintactico
{
    public static class tokens
    {
        //tipos de variables menos id
        public const int Id = 101;
        public const int NE = 102;
        public const int NR = 103;
        public const int Cadena = 104;

        //operadores aritmeticos
        public const int mas = 201;
        public const int menos = 202;
        public const int por = 203;
        public const int div = 204;

        //operadores relacionales
        public const int mayor = 205;
        public const int menor = 206;
        public const int menor_igual = 207;
        public const int mayor_igual = 208;
        public const int igual = 209;
        public const int diferente = 210;

        //operadores logicos
        public const int and = 211;
        public const int or = 212;
        public const int not = 213;

        //asignacion
        public const int asignar = 214;

        //simbolos de puntuacion
        public const int punto_coma = 301;
        public const int coma = 307;

        //simbolos de agrupacion
        public const int parentesis_izq = 302;
        public const int parentesis_der = 303;
        public const int llave_izq = 304;
        public const int llave_der = 305;

        //palabras reservadas
        public const int STR = 401;
        public const int INT = 402;
        public const int REAL = 403;
        public const int If = 404;
        public const int Then = 405;
        public const int Else = 406;
        public const int While = 407;
        public const int Do = 408;
        public const int Repeat = 409;
        public const int Until = 410;
        public const int For = 411;
        public const int To = 412;
        public const int Step = 413;
        public const int Exit = 414;
        public const int Write = 415;
        public const int Read = 416;
    }
}
