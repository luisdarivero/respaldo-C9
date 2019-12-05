/* 
    DeepLingo compiler - Semantic analyzer.
    Date: 12-March-2018
    Authors:
          A01169073 Aldo Reyna
          A01375051 Marina Torres
    File name: Values.cs
*/


using System;
using System.Text;

namespace DeepLingo {

    public class Values{
        public string funcType { get; set; }
        public int arity { get; set; }
        public SymbolTableLocal table { get; set; }
        
        public Values(string type, int num, SymbolTableLocal table){
            this.funcType = type;
            this.arity = num;
            this.table = table;
        }
        
        
        
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(String.Format("{0}, {1}", funcType, arity));
            return sb.ToString();
        }
        
    }
}