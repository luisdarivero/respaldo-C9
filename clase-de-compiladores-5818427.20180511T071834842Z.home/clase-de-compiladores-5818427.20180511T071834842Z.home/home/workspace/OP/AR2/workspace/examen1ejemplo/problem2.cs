//==========================================================
// Solution to problem 2.
//==========================================================

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Exam1 {
    public class Problem2 {
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
            var lst = new List<String>();
            var regex = new Regex(@"(\d\.\d) (.+) (\(\d{4}\))(\..{2,3})");
            foreach (Match m in regex.Matches(input)) {
                lst.Add(m.Groups[3].Value + " " + m.Groups[2].Value + " " + m.Groups[1].Value + m.Groups[4].Value);
            }
            lst.Sort();
            lst.ForEach(Console.WriteLine);
        }
    }
}
