/*
  Compilador de int64
  -- CommentTrimmer:
        Borra todos los comentarios de un string provisto.

  Copyright (C) 2017 Iram Molina & Diego Trujillo bajo WTFPL.
  ITESM CEM
*/

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace Int64 {
    
    public class CommentTrimmer{

        public static string Trim(string file){
            var outfile = "";
            var regex =  new Regex(@"(/[*](.|\n)*?[*]/|//.*)|(.|\n)");

            
            foreach(Match m in regex.Matches(file)){
                if(m.Groups[1].Length > 0){
                    //Checamos cuántos saltos de línea hay en el comentario.
                    int numberOfBreaks = m.Value.Split('\n').Length - 1;
                    // Concatenamos ese número de saltos al string regresado.
                    for(int i = 0; i < numberOfBreaks; i++){
                        outfile += "\n";
                    }
                    continue;
                }
                outfile += m.Value;
            }
            
            return outfile;
        }
        
    }
}