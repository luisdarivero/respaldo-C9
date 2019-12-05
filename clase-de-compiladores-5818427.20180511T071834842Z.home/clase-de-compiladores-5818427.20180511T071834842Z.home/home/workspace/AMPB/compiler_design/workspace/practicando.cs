using System;
using System.Text.RegularExpressions;
using System.IO;

class CommentEliminator {
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
        var regex2 = new Regex(@".");
        var regex3 = new Regex(@"(0[b|B]+[0|1]+([_?|0|1]*[0|1]+)?[L|l]?\b)");
            
        var newLineSize = 0;
        var wordSize = 0;
        var charSize = 0;
        
        foreach (Match m in regex.Matches(input)) {
            if (m.Groups["newLine"].Length > 0) {
                newLineSize += 1;
            }
            if (m.Groups["word"].Length > 0) {
                wordSize += 1;
            }
        }
        foreach (Match m in regex2.Matches(input)) {
            if (m.Groups[0].Length > 0) {
               charSize += 1;
            }
        }
        Console.Write(newLineSize + " " + wordSize + " " + charSize);
    }
}