using System;
using System.Collections.Generic;

public class GeneratorExample{
    //generator method
    public static IEnumerable<int> Start(){
        var c = 1;
        while (c<10000){
            yield return c;
            c *=2;
        }
    }
    
    public static void Main(){
        
        foreach(var x in Start()){
            Console.WriteLine(x);
        }
        /*
        var enumerable = Start();
        var enumerator = enumerable.GetEnumerator();
        enumerator.MoveNext();
        Console.WriteLine(enumerator.Current);
        enumerator.MoveNext();
        Console.WriteLine(enumerator.Current);
        enumerator.MoveNext();
        Console.WriteLine(enumerator.Current);*/
        
        
    }
    
    //to run mcs namefile
    //mono namefile
}