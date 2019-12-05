//==========================================================
// Aldo Arturo Reyna Gómez - A01169073
//==========================================================

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Exam1 {
    public class Problem1 {
        public static void Main(String[] args) {
            if (args.Length != 1) {
                Console.Error.WriteLine("Please specify the name of the input file.");
                Environment.Exit(1);
            }
            var path = args[0];
            var list = new List<string>();
            var input = "";
            try {
                input = File.ReadAllText(path);
                foreach (string line in File.ReadLines(path)){
                    list.Add(line);
                }
            }
            catch (FileNotFoundException e) {
                Console.Error.WriteLine(e.Message);
                Environment.Exit(1);
            }
            
            var regex = new Regex(@"^[Cc\*].*\n?", RegexOptions.Multiline);
            foreach (Match m in regex.Matches(input)) {
                list.Remove(m.Value);
            }
            list.ForEach(Console.WriteLine);
        }
    }
}

