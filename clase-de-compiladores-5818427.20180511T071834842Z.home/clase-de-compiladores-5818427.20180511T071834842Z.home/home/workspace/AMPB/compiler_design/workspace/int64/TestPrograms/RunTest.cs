using System;
using System.IO;
using System.Text;
using System.Diagnostics;


namespace Int64Tests{
    class RunTest{
        public static string[] availablePrograms = {
            "hello.int64",
            "binary.int64",
            "palindrome.int64",
            "factorial.int64",
            "arrays.int64",
            "next_day.int64"
        };
        
        public static void Main(string[] args){
            Console.WriteLine("==== INT64 Compiler Tester ====");
            Console.WriteLine("Available programs:");
            
            for(var i = 0; i < availablePrograms.Length; i++){
                Console.WriteLine("(" + i + ") " + availablePrograms[i]);
            }
            
            Console.Write("Please type number of the program to test:");
            var userChoice = Console.ReadLine();
            
            Console.WriteLine("");

            try{
                int numberedChoice = Int32.Parse(userChoice);
                if(numberedChoice >= 0 && numberedChoice < availablePrograms.Length){
                    Console.WriteLine(availablePrograms[numberedChoice] + " selected.");
                    
                    var info = new ProcessStartInfo();
                    info.FileName = "mono";
                    info.Arguments = "int64.exe ./TestPrograms/" + availablePrograms[numberedChoice];
                    
                    info.UseShellExecute = false;
                    info.CreateNoWindow = true;
                    
                    info.RedirectStandardOutput = true;
                    info.RedirectStandardError = true;
                    
                    var process = Process.Start(info);
                    process.WaitForExit();
                }
                else{
                    Console.WriteLine("Non-valid choice, bye bye.");
                }
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
            
        }
    }
}

