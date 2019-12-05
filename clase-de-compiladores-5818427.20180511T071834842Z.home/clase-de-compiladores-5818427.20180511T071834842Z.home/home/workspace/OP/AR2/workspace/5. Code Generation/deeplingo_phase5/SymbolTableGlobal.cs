// This programme is based on the Buttercup's compiler code, provided by Ariel Ortiz.

/* 
    DeepLingo compiler - Semantic analyzer.
    Date: 12-March-2018
    Authors:
          A01169073 Aldo Reyna
          A01375051 Marina Torres
    File name: SymbolTableGlobal.cs
*/

using System;
using System.Text;
using System.Collections.Generic;

namespace DeepLingo {

    public class SymbolTableGlobal: IEnumerable<KeyValuePair<string, Values >> {

        IDictionary<string, Values> data = new Dictionary<string, Values>();

        //-----------------------------------------------------------
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("Symbol Table\n");
            sb.Append("====================\n");
            foreach (var entry in data) {
                sb.Append(String.Format("{0} \n", 
                                        entry.Key 
                                        ));
            }
            sb.Append("====================\n");
            return sb.ToString();
        }

        //-----------------------------------------------------------
        public Values this[string key] {
            get {
                return data[key];
            }
            set {
                data[key] = value;
            }
        }

        //-----------------------------------------------------------
        public bool Contains(string key) {
            return data.ContainsKey(key);
        }

        //-----------------------------------------------------------
        public IEnumerator<KeyValuePair<string, Values>> GetEnumerator() {
            return data.GetEnumerator();
        }

        //-----------------------------------------------------------
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
