//==========================================================
// Andrea Margarita Pérez Barrera A01373631
//==========================================================

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Exam1 {
    public class Problem1 {
        public static void Main(String[] args) {
            if (args.Length != 1) {
            Console.Error.WriteLine("Please indicate the input file!");
            Environment.Exit(1);
        }
        var inputFile = args[0];
        var input = File.ReadAllText(inputFile);
        var regex = new Regex(@"
                 (?<newLine>    \n          )
                |(?<word> [a-zA-Z]+\w*      )
            ",
            RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            );

        var row = 1;
        var columnStart = 0;
        var wordCheck = "";
        var wordTemp = "";
        var colWord= 0;
        var rowWord= 0;
       // bool flag= false;
        
        foreach (Match m in regex.Matches(input)) {
             if (m.Groups["newLine"].Length > 0){
                    row++;
                    columnStart = m.Index + m.Length;
                }
             if (m.Groups["word"].Length > 0){
                 wordCheck = m.Value;
                 
                 //Console.WriteLine(m.Value +"-"+ row +"-" +(m.Index - columnStart + 1));
                 //Console.WriteLine(m.Value[1]);
                 foreach (Match n in regex.Matches(input)) {
                     if (n.Groups["word"].Length > 0){
                         //Console.WriteLine(m.Index + " " + n.Index);
                         if(m.Index != n.Index){
                            if(wordCheck.Equals(wordTemp, StringComparison.OrdinalIgnoreCase)){
                                //Console.WriteLine(wordTemp +"-"+ rowWord +"-" +colWord);
                                //Console.WriteLine(m.Value +"-"+ row +"-" +(m.Index - columnStart + 1));
                                Console.WriteLine('"'+wordTemp+'"'+' '+ "in row "+rowWord+", column "+colWord+" is inmediately repeated in row "+row+", column "+ (m.Index - columnStart + 1));
                                wordTemp = "";
                                rowWord = 0;
                                colWord = 0;
                                
                                break;
                            }
                            if(wordCheck.Equals(n.Value, StringComparison.OrdinalIgnoreCase)){
                                wordTemp = m.Value;
                                rowWord = row;
                                colWord = m.Index - columnStart + 1;
                               // Console.WriteLine();
                                break;
                         }}
                 }
                 }  
             }
        }
        }
    }
}

