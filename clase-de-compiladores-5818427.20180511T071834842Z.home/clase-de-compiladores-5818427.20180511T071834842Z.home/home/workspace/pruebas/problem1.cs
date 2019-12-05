//==========================================================
// Solution to problem 1.
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
            var inputPath = args[0];
            var input = "";
            try {
                input = File.ReadAllText(inputPath);
            } catch (FileNotFoundException e) {
                Console.Error.WriteLine(e.Message);
                Environment.Exit(1);
            }
            var regex = new Regex(@"([a-zA-Z]+)|(\n)");
            var lastWord = "";
            var lastRow = 0;
            var lastColumn = 0;
            var row = 1;
            var columnStart = 0;
            foreach (Match m in regex.Matches(input)) {
                if (m.Groups[1].Success) {
                    var currentWord = m.Value;
                    var currentRow = row;
                    var currentColumn = m.Index - columnStart + 1;
                    if (lastWord.ToLower() == currentWord.ToLower()) {
                        Console.WriteLine(String.Format(
                            "\"{0}\" in row {1}, column {2} is immediately repeated in row {3}, column {4}.",
                            lastWord.ToLower(), lastRow, lastColumn, currentRow, currentColumn));
                    }
                    lastWord = currentWord;
                    lastRow = currentRow;
                    lastColumn = currentColumn;
                } else if (m.Groups[2].Success) {
                    row++;
                    columnStart = m.Index + m.Length;
                }
            }
        }
    }
}
