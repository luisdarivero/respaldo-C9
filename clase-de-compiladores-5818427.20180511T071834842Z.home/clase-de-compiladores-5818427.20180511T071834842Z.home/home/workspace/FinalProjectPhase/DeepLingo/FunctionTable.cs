using System;
using System.Text;
using System.Collections.Generic;

namespace DeepLingo {

    public class FunctionTable: IEnumerable<KeyValuePair<string, 
                        Tuple<string, int, LocalFunctionTable>>> {

        IDictionary<string, Tuple<string, int, LocalFunctionTable>> data = 
                    new SortedDictionary<string, Tuple<string, int, LocalFunctionTable>>();

        //-----------------------------------------------------------
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("Global Function Table\n");
            sb.Append("====================\n");
            sb.Append("|name\t|type\t|arity\t|\n");
            foreach (var entry in data) {
                sb.Append(String.Format("|{0}\t|{1}\t|{2}\t|\n", 
                                        entry.Key, entry.Value.Item1,
                                        entry.Value.Item2));
                if(entry.Value.Item1 == "U"){
                    sb.Append(entry.Value.Item3.ToString());
                }
            }
            sb.Append("====================\n");
            return sb.ToString();
        }

        //-----------------------------------------------------------
        public Tuple<string, int, LocalFunctionTable> this[string key] {
            get {
                return data[key];
            }
            set {
                data[key] = value;
            }
        }
        
        public LocalFunctionTable getLocalFT(string name){
            return data[name].Item3;
        }
        
        

        //-----------------------------------------------------------
        public bool Contains(string key) {
            return data.ContainsKey(key);
        }

        //-----------------------------------------------------------
        public IEnumerator<KeyValuePair<string, 
                Tuple<string, int, LocalFunctionTable>>> GetEnumerator() {
            return data.GetEnumerator();
        }

        //-----------------------------------------------------------
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
