// This programme is based on the Buttercup's compiler code, provided by Ariel Ortiz.

/* 
    DeepLingo compiler - Semantic analyzer.
    Date: 12-March-2018
    Authors:
          A01169073 Aldo Reyna
          A01375051 Marina Torres
    File name: SymbolTableLocal.cs
*/

using System;
using System.Text;
using System.Collections.Generic;

namespace DeepLingo {

    public class SymbolTableLocal: IEnumerable<KeyValuePair<string, Locals >> {

        IDictionary<string, Locals> data = new Dictionary<string, Locals>();

        //-----------------------------------------------------------
        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var entry in data) {
                sb.Append(String.Format("{0}, {1}\n", 
                                        entry.Key, entry.Value 
                                        ));
            }
            
            return sb.ToString();
        }

        //-----------------------------------------------------------
        public Locals this[string key] {
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
        public IEnumerator<KeyValuePair<string, Locals>> GetEnumerator() {
            return data.GetEnumerator();
        }

        //-----------------------------------------------------------
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
