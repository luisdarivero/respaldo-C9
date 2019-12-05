using System;
using System.IO;
using System.Text.RegularExpressions;
namespace Daniel {
    public class csv {
        public static void Main(String[] args) {
            var nombreArchivo = args[0];
            var contenidoArchivo = File.ReadAllText(nombreArchivo);
            var contador = 0;
            string[] valores = new string[7]{"","","","","","",""};
            var myRegex = new Regex(@"
                (?<ignore>          ([ ][-][ ][-][ ])                   )
              | (?<ipClient>        [0-9]+[.][0-9]+[.][0-9]+[.][0-9]+   )
              | (?<ipClientDos>     ([0-9a-z]+[:]+)+[0-9a-z]+           )
              | (?<date>            ([[].+?[]])                         )
              | (?<number>          ([0-9]+)                            )
              | (?<quotesInside>    ([""].+[\\][""].+[\\][""].+?[""])   )
              | (?<quoteInside>    ([""].+[\\][""].+?[""])              )
              | (?<quotes>          ([""].+?[""])                       )
              | (?<Newline>         \n                                  )
              | (?<ignoreDos>       \s                                  )", 
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled
                | RegexOptions.Multiline);
                
            using (System.IO.StreamWriter file = 
                new System.IO.StreamWriter(@"resultado.csv"))
            {
               
                foreach (Match m in myRegex.Matches(contenidoArchivo)) {
                    if (m.Groups["Newline"].Success){
                        var resultado = "";
                        resultado += valores[0]+"," + (valores[1].Substring(1,11)) +"," + (valores[1].Substring(12,9)) +",";
                        var methodUrl = valores[2].Split(' ');
                        if(methodUrl.Length > 2){
                            resultado += methodUrl[0].Substring(1,3) +"," + methodUrl[1] +",";
                        }
                        else{
                            resultado += "\"-\"," + methodUrl[0] + ",";
                        }
                        resultado += valores[3] +"," + valores [4] +"," + valores[6] +"," + valores [5];
                        file.WriteLine(resultado);
                        //Console.WriteLine(resultado); 
                        //Console.WriteLine("\n");
                        contador = 0;
                    }
                    else if(m.Groups["ignore"].Success || m.Groups["ignoreDos"].Success){
                    }
                    else if (m.Groups["ipClient"].Success || m.Groups["ipClientDos"].Success ||
                        m.Groups["date"].Success || m.Groups["number"].Success ||
                        m.Groups["quotes"].Success || m.Groups["quotesInside"].Success || m.Groups["quoteInside"].Success){
                        valores[contador] = m.Value;
                        contador ++;
                    }
                };
                    
            }
            
        }
    }
}

