
using System;
using System.IO;
using System.Text;

namespace DeepLingo {

    public class Driver {
        
        const string VERSION = "0.5";

        //-----------------------------------------------------------
        static readonly string[] ReleaseIncludes = {
            "Lexical analysis",
            "Syntactic analysis",
            "AST construction",
            "Semantic analysis",
            "CIL code generation"
        };

        //-----------------------------------------------------------
        void PrintAppHeader() {
            Console.WriteLine("DeepLingo compiler, version " + VERSION);
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
                Console.Error.WriteLine("Please specify the name of the input and output files.");
                Environment.Exit(1);
            }

            try {            
                var inputPath = args[0];
                var outputPath = args[1];
                var input = File.ReadAllText(inputPath);
                
                var parser = new Parser(new Scanner(input).Start().GetEnumerator());
                var program  = parser.Program();
                Console.WriteLine("Syntax OK");
                Console.Write(program.ToStringTree());
                
                //pruebas de la table symboltable (globales)
                var semantic = new SemanticAnalyzer();
                semantic.Visit((dynamic) program);
                semantic.Visit((dynamic) program);
                if(semantic.isSemanticCorrect()){
                    //Console.WriteLine();
                    Console.WriteLine("____________________________________________");
                    Console.WriteLine();
                    Console.WriteLine("Semantic OK");
                    Console.WriteLine("____________________________________________");
                    Console.WriteLine();
                    Console.WriteLine();
                }
                
                var CILgeneration = new CILGenerator();
                string finalCode = CILgeneration.Visit((dynamic) program);
                
                Console.WriteLine(finalCode);
                
                //se guarda el archivo que se desea
                File.WriteAllText(
                    outputPath,
                    finalCode);
                //se termina de guardar el archivo
                
            } catch (Exception e) {

                if (e is FileNotFoundException || e is SyntaxError) {
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
