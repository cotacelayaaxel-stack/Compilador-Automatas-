//Version Lexico
//Version Sintactico
//Version semantica


using Analizador_Lexico;
using Analizador_sintactico;
using static System.Runtime.InteropServices.JavaScript.JSType;

string texto = "", ruta = "";
try
{
    ruta = @"C:\Users\axela\OneDrive\Documentos escolares\Sistemas computacionales\semestre 7\Lenguaje y automatas ll\Codigo_ver.txt";
    texto = File.ReadAllText(ruta);
    Lexico lexico = new Lexico(texto);

    Console.WriteLine("\n Texto ingresado: \n");
    Console.WriteLine(texto);

    lexico.Analizar();
    //lexico.imprimir();


    Sintactico sinaxis = new Sintactico(lexico.Cabeza);
    sinaxis.Programa();

    if (Errores.Hay_Errores)
    {
        Errores.Mostrar();
        Environment.Exit(1);
    }
    Console.WriteLine("Analisis Terminado");


}
catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
{
    Console.WriteLine("Archivo no encontrado");
    Environment.Exit(1);
}







// cuadno sintactico este listo poner un if para ver si no hay errores seguir con el sinta

/*
 ruta actual de seguimiento o analisis del compilador
1. Program.cs busca el archivo segun la ruta dada y copia el texto para mandarlo a la clase 
Lexico en donde es copiada en el constructor, para esto se crea el objeto lexico de la clase Lexico
y se inicializa el archivo pasandole el parametro de texto 
2. Program.cs utiliza la funcion Analizar() de la clase Lexico para revisar el codigo 
3. con ayuda de la matriz de estados, los diccionarios de palabras reservadas y errores, 
Lexico valida las sentencias escritas para ver si pertenecen sus simbolos a la gramatica, al 
hacer esto tambien se les asigna su propio token y se agregan sus lexemas en la lista de la clase
Nodo junto con su linea
4.

 
 
 */

/*
 Falta cambiar el diseño de errores de como acciona el compilador a la hora de encontrar uno panico por ejemplo
cambiar los tokens de los erroes primero los de lexico-sintactico-semanticosy asi
 */