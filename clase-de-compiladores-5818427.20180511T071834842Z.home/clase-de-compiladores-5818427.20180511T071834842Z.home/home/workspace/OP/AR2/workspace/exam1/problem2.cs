//==========================================================
// Aldo Arturo Reyna Gómez - A01169073
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
            var path = args[0];
            var list = new List<string>();
            try {
                foreach (string line in File.ReadLines(path)){
                    list.Add(line);
                }
            }
            catch (FileNotFoundException e) {
                Console.Error.WriteLine(e.Message);
                Environment.Exit(1);
            }
            
            var regex = new Regex(@"(#x)([0-9a-fA-F]+)");
            for (var n = 0; n < list.Count; n++) {
                string s = list[n];
                foreach (Match m in regex.Matches(list[n])) {
                    if (m.Groups[2].Success) {
                        int value = Convert.ToInt32(m.Groups[2].Value, 16);
                        s = s.Replace(m.Value, "#"+value);
                    }
                }
                Console.WriteLine(s);
            }
        }
    }
}