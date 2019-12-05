// Print to the standard output the contents of the input file
// (which should be a C source program) but without comments.

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
        var regex = new Regex(@"(/[*](.|\n)*?[*]/|//.*)|(.|\n)");
        foreach (Match m in regex.Matches(input)) {
            if (m.Groups[1].Length > 0) {
                continue;
            }
            Console.Write(m.Value);    
        }
    }
}