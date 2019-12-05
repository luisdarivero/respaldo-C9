/*
  Compilador de int64
  -- Driver:
        Se encarga de llamar a todos los actantes sobre el código.

  Copyright (C) 2017 Iram Molina & Diego Trujillo bajo WTFPL.
  ITESM CEM
*/

using System;
using System.IO;
using System.Text;

namespace Int64 {
    
    public class Driver{
        
        // Control de versiones & info de la aplicación
        static readonly string version = "0.6";
        
        static readonly string[] features = {
            "Lexical Analysis",
            "Syntactic Analyzer",
            "AST Construction"
        };
        
        // Imprime información sobre autores, licencia y características del compilador.
        void PrintCompilerInfo(){
            Console.WriteLine("INT64 PROGRAMMING LANGUAGE COMPILER (Version " + version + ")");
            Console.WriteLine("(C) 2017 Iram Molina & Diego Trujillo. ");
            Console.WriteLine("\nLicensed under WTFPL. \nSee 'License.txt' for more details.");
            Console.WriteLine("\nFeatures:");
            foreach(string feature in features){
                Console.WriteLine("-- " + feature);
            }
        }
        
        // Lógica principal
        void Run(string[] args){
            // Imprime información del compilador.
            PrintCompilerInfo();
            // Checar si nombre de archivo ha sido provisto.
            if (args.Length < 1) {
                Console.Error.WriteLine("Filename not specified.");
                Environment.Exit(1);
            }
            
            
            try{
                // Lee el archivo
                var inputFilename = args[0];
                var inputString = File.ReadAllText(inputFilename);
                
                // Borrar los comentarios
                inputString = CommentTrimmer.Trim(inputString);
    
                // Divide al programa en Tokens y los alimenta a un nuevo Analizador Sintáctico
                var syntacticAnalyzer = new SyntacticAnalyzer(LexicalAnalyzer.Tokenize(inputString).GetEnumerator()); 
                
                var program = syntacticAnalyzer.Program();
                Console.WriteLine(program.ToStringTree());
                Console.WriteLine("Syntax ok!");
            }
            catch(Exception e){
                
                if (e is FileNotFoundException || e is SyntaxError) {
                    Console.Error.WriteLine(e.Message);
                    Environment.Exit(1);
                }

                throw;
            }
            
            
            
        }
        
        // Runner
        public static void Main(string[] args){
            new Driver().Run(args);
        }
    }
}