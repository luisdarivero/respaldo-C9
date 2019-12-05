/*
  DeepLingo compiler - Program driver.
  Copyright (C) 2013 Ariel Ortiz, ITESM CEM
  
  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.
  
  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  
  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Text;

namespace DeepLingo {

    public class Driver {

        const string VERSION = "0.5";

        //-----------------------------------------------------------
        static readonly string[] ReleaseIncludes = {
            "Lexical analysis",
            "Syntax analysis",
            "AST Construction",
            "Semantic Analysis",
            "CIL code generation"
        };

        //-----------------------------------------------------------
        void PrintAppHeader() {
            Console.WriteLine("DeepLingo compiler, version " + VERSION);
            Console.WriteLine("Copyright \u00A9 2013 by A. Reyna and M. Torres, ITESM CEM."                
            );
            Console.WriteLine("This program is free software; you may "
                + "redistribute it under the terms of");
            Console.WriteLine("the GNU General Public License version 3 or "
                + "later.");
            Console.WriteLine("This programme has absolutely no warranty.");
            Console.WriteLine("This programme is based on the Buttercup's compiler code, provided by Ariel Ortiz.");
        }

        //-----------------------------------------------------------
        void PrintReleaseIncludes() {
            Console.WriteLine("Included in this release:");            
            foreach (var phase in ReleaseIncludes) {
                Console.WriteLine("   * " + phase);
            }
        }

        //-----------------------------------------------------------
        void Run(string[] args) {

            PrintAppHeader();
            Console.WriteLine();
            PrintReleaseIncludes();
            Console.WriteLine();

            if (args.Length != 2) {
                Console.Error.WriteLine(
                    "Please specify the name of the input and output files.");
                Environment.Exit(1);
            }

            try {            
                var inputPath = args[0];
                var outputPath = args[1];
                var input = File.ReadAllText(inputPath);
                var parser = new Parser(new Scanner(input).Start().GetEnumerator());
                var ast = parser.Program();
                //Console.Write(ast.ToStringTree());
                Console.WriteLine("Syntax OK.");
                
                var semantic = new SemanticAnalyzer();
                semantic.Visit((dynamic) ast);
                
                //Console.WriteLine(semantic.FancyPrint());
                
                var codeGenerator = new CILGenerator(semantic);
                File.WriteAllText(
                    outputPath,
                    codeGenerator.Visit((dynamic) ast));
                Console.WriteLine(
                    "Generated CIL code to '" + outputPath + "'.");
                Console.WriteLine();
                
                
            } catch (Exception e) {
                if (e is FileNotFoundException
                    || e is SyntaxError
                    || e is SemanticError) {
                    Console.Error.WriteLine(e.Message);
                    Environment.Exit(1);
                }

                throw;
            }                
        }

        //-----------------------------------------------------------
        public static void Main(string[] args) {
            new Driver().Run(args);
        }
    }
}
