using System;
using System.Collections.Generic;

class YieldExample {
    public static IEnumerable<int> Pow2(int max) {
        var n = 1;
        while (n<= max) {
            yield return n;
            n *= 2;
        }
    }
    
    public static vid Main() {
        /*foreach (var x in Pow2(128)) {
            Console.WriteLine(x);
        }*/
        IEnumerable<int> a = Pow2(128);
        IEnumerable<int> b = a.GetEnumerator();
        
        while (b.MoveNext()){
            int x = b.Current;
            Console.WriteLine(x);
        }
    }
}

//mono buttercup.exe