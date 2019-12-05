//==========================================================
// Andrea Margarita Pérez Barrera A01373631
//==========================================================

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Exam1 {
    public class Problem2 {
        public static void Main(String[] args) {
            if (args.Length != 1) {
                Console.Error.WriteLine("Please indicate the input file!");
                Environment.Exit(1);
            }
            var inputFile = args[0];
            var input = File.ReadAllText(inputFile);
            var regex = new Regex(@"
                 (?<newLine>      \n)
                |(?<year>        \d{4}   )
                |(?<version>    (\d\.\d)    )
                |(?<name>        ([a-zA-Z].+\x20))
                |(?<extension>  (\.(avi|mov|mp4|qt|wmv)))
            ",
            RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            );
            
            
            List<List<string>> BigArr = new List<List<string>>();;
            List<string> movieArr;
            var val1 = "";
            var val2 = "";
            var val3 = "";
            var val4 = "";
            
            foreach (Match m in regex.Matches(input)) {
                
                if (m.Groups["newLine"].Length > 0) {
                    movieArr = new List<string>(){val1,val2,val3,val4};
                    BigArr.Add(movieArr);
                    val1 = ""; val2 = ""; val3 = ""; val4 = "";
                }
                if (m.Groups["year"].Length > 0) {
                    val1 = m.Value;
                }if (m.Groups["name"].Length > 0) {
                    val2 = m.Value;
                }if (m.Groups["version"].Length > 0) {
                    val3 = m.Value;
                }
                if (m.Groups["extension"].Length > 0) {
                    val4 = m.Value;
                }
                
            }

            BigArr.Sort((a, b) => a[0].CompareTo(b[0]));
            foreach (List<string> subList in BigArr){
                foreach (string item in subList){
                    Console.Write(item);
                }
                Console.WriteLine(" ");
            }
            
        }
    }
}

