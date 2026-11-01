using System;
using System.Collections.Generic;

namespace Analizador_sintactico
{
    internal static class Errores
    {
        private static readonly Dictionary<int, string> lista_errores = new Dictionary<int, string>
        {
            // Léxicos
            { 501, "Se espera 'digito'" },
            { 502, "eof inesperado" },
            { 503, "Se espera '='" },
            { 504, "Se espera \"" },
            { 505, "Simbolo no valido" },

            // Sintácticos
            { 506, "Se espera ',' al agregar mas variables a la vez" },
            { 507, "Se espera abrir o cerrar llaves" },
            { 508, "Se espera ';'" },
            { 509, "Se espera 'then'" },
            { 510, "Se espera 'do'" },
            { 511, "Se espera 'until'" },
            { 512, "Se espera 'to'" },
            { 513, "Se espera ':='" },
            { 514, "Se espera 'id'" },
            { 515, "Se espera abrir o cerrar '()'" },
            { 516, "Se espera factor (ID, NE, NR, Cadena (exp)" },

            //semantico
            { 517, "Variable no declarada" },
            { 518, "Variable ya declarada" },
            { 519, "Incoherencia de tipos" },
        };

        private static readonly List<(int token, int linea, string descripcion)> errores_acumulados
            = new List<(int, int, string)>();

        public static bool Hay_Errores
        {
            get { return errores_acumulados.Count > 0; }
        }

        public static string Mensaje(int error)
        {
            return lista_errores.TryGetValue(error, out string descripcion_error)
                ? descripcion_error : "Error desconocido";//en teoria no debe de existir porque nunca existira un error desconocido con token asignado
        }

        public static void Agregar(int token, int linea)
        {
            errores_acumulados.Add((token, linea, Mensaje(token)));
        }

        public static void Mostrar()
        {
            foreach (var parte in errores_acumulados)
            {
                Console.WriteLine($"Error {parte.token} | Linea: {parte.linea} | {parte.descripcion}");
            }
        }
    }
}