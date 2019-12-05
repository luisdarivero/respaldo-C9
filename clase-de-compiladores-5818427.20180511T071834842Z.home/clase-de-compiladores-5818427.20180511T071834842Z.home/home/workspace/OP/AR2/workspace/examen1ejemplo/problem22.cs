//==========================================================
// Type your name and student ID here.
//==========================================================

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Exam1 {
    public class Problem2 {
        public static void Main(String[] args) {
            
            var path = args[0];
            string input = File.ReadAllText(path);
            var regex = new Regex(
                @"      
                        (?<Rank>        \d\.\d              )
                        (?<Title>       [a-zA-Z\' \d:-]+    )
                        (?<Year>        \(\d+\)             )
                        (?<Extension>   \.\w{2,3}           )
                ", 
                RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            
                );
        
            var lst = new List<string>();
            foreach (Match m in regex.Matches(input)) {
                lst.Add(m.Groups[3].Value + m.Groups[2].Value + m.Groups[1].Value + m.Groups[4].Value);
            }
            lst.Sort();
            lst.ForEach(Console.WriteLine);

        }
    }
}
