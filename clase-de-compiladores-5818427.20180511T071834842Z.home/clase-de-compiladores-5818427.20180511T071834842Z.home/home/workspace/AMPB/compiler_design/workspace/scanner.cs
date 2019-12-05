// A first example of how to use regular expressions in C#.
//
// To compile:
//                   mcs scanner.cs
// To run:
//                   mono scanner.exe

using System;
using System.Text.RegularExpressions;

namespace SimpleScanner {
    
    class Scanner {
        public static void Main() {
            var regex = new Regex(
                @"
                ([a-zA-Z_]\w*)   # Group 1: Identifier
            |   (\d+)            # Group 2: Integer
            |   ([+])            # Group 3: Plus symbol
            |   (.)              # Group 4: Anything else
            ",
                  RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Singleline);
            var str = "1234 abc + 0_hello_ + _123;";
            foreach (Match m in regex.Matches(str)) {
                if (m.Groups[1].Length > 0) {
                    Console.Write("Identifier: ");
                } else if (m.Groups[2].Length > 0) {
                    Console.Write("Integer: ");
                } else if (m.Groups[3].Length > 0) {
                    Console.Write("Plus: ");
                } else {
                    // Console.Write("Ignoring: ");
                    continue;
                }
                
                var x = m.Value;
                var i = m.Index;
                Console.WriteLine(x + " " + i);
            }
        }
    }  
}