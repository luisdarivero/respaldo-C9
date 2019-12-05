//==========================================================
// Luis Daniel Rivero Sosa A01374527
//==========================================================

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Exam1 {
    public class Problem2 {
        public static void Main(String[] args) {
            var nombreArchivo = args[0];
            var contenidoArchivo = File.ReadAllText(nombreArchivo);
            //Console.WriteLine(contenidoArchivo);
            var mySecondRegex = new Regex(@"[0-9a-fA-F]+");
            var myRegex = new Regex(@"(&#x[0-9a-fA-F]+;)|(\n)|(.)");
            foreach (Match m in myRegex.Matches(contenidoArchivo)) {
                
                if (m.Groups[1].Success){
                    foreach(Match y in mySecondRegex.Matches(m.Value)){
                    Console.Write("&#");
                    Console.Write(Convert.ToInt32(y.Value, 16));
                    Console.Write(";");
                    }
                }
                else if(m.Groups[3].Success){
                    Console.Write(m);
                }
                else if(m.Groups[2].Success){
                    Console.WriteLine("");
                }
                /*
                else if(m.Groups[2].Success){
                    Console.Write("\n");
                }
                else if(m.Groups[3].Success){
                    Console.Write(m);
                }*/
                
            }
        }
    }
}

