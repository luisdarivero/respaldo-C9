using System;
using System.Collections.Generic;

namespace HelloWorld
{
    class Hello 
    {
        static void Main() 
        {
            string[] arr2 = { "0", "0b0000", "0x0" ,"0o0" };
            
            string a = arr2[2]; 
            string b = arr2[1];
            string cc = arr2[3];
            string c = b.Substring(2, b.Length - 2);
            string d = cc.Substring(2, cc.Length - 2);
            
            int toInt2 = Convert.ToInt32(d, 8); 
            int toInt1 = Convert.ToInt32(c,2); //binario
            int toInt = Convert.ToInt32(a, 16); //hexadecimal
            Console.WriteLine(toInt2);

        }
    }
}
//mcs -out:TEST.exe pruebas.cs
//mono TEST.exe
