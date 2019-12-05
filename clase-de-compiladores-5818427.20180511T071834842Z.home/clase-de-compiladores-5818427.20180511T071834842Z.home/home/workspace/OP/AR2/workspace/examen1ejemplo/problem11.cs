//==========================================================
// Type your name and student ID here.
//==========================================================

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Exam1 {
    public class Problem1 {
        public static void Main(String[] args) {
            if (args.Length != 1) {
                Console.Error.WriteLine("Please specify the name of the input file.");
                Environment.Exit(1);
            }
            var path = args[0];
            string input = "";
            
            try {
                input = File.ReadAllText(path);
            }
            catch (FileNotFoundException e) {
                Console.Error.WriteLine(e.Message);
                Environment.Exit(1);
            }
            var regex = new Regex(
                @"      
                        (?<Newline>     \n          )
                        |(?<Word>        [a-zA-Z]+   )
                ", 
                RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            
                );
            var currentWord = "";
            var lastWord = "";
            
            var currentRow = 1;
            var lastRow = 1;
            
            var columnStart = 0;
            
            var currentColumn = 0;
            var lastColumn = 0;
            
            foreach (Match m in regex.Matches(input)) {
                
                if (m.Groups["Newline"].Success) {
                    currentRow++;
                    columnStart = m.Index;
                }
                else if (m.Groups["Word"].Success) {
                    currentWord = m.Value;
                    currentColumn = m.Index - columnStart;
                    if (lastWord.ToLower() == currentWord.ToLower()) {
                        Console.WriteLine(String.Format(
                            "\"{0}\" in row {1}, column {2} is immediately repeated in row {3}, column {4}.",
                            lastWord.ToLower(), lastRow, lastColumn, currentRow, currentColumn));
                    }
                    lastWord = currentWord;
                    lastColumn = currentColumn;
                    lastRow = currentRow;
                }
                
            }
        }
    }
}