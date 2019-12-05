//==========================================================
// Type your name and student ID here.
//==========================================================

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Exam1 {
    public class Problem1 {
        public static void Main(String[] args) {
            //
            var nombreArchivo = args[0];
            var contenidoArchivo = File.ReadAllText(nombreArchivo);
            //Console.WriteLine(contenidoArchivo);
            
            var myRegex = new Regex(@"(^[cC*].*\n)|(\n)|(.)",RegexOptions.Multiline);
            foreach (Match m in myRegex.Matches(contenidoArchivo)) {
                if (m.Groups[1].Success){
                    //Console.WriteLine("\n");
                }
                else if(m.Groups[2].Success){
                    Console.Write("\n");
                }
                else if(m.Groups[3].Success){
                    Console.Write(m);
                }
                
            }
            
        }
    }
}

