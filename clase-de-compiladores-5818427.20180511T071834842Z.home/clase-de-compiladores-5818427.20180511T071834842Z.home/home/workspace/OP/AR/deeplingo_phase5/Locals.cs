/* 
    DeepLingo compiler - Semantic analyzer.
    Date: 12-March-2018
    Authors:
          A01169073 Aldo Reyna
          A01375051 Marina Torres
    File name: Locals.cs
*/


using System;
using System.Text;

namespace DeepLingo {

    public class Locals{
        public string funcType { get; set; }
        public int position { get; set; }
        
        public Locals(string type, int num){
            this.funcType = type;
            this.position = num;
        }
        
        public override string ToString() {
            var sb = new StringBuilder();
            if (position!=-1){
                sb.Append(String.Format("{0}, {1}", funcType, position));
            }else{
                sb.Append(String.Format("{0}, -", funcType, position));
            }
            
            return sb.ToString();
        }
        
    }
}