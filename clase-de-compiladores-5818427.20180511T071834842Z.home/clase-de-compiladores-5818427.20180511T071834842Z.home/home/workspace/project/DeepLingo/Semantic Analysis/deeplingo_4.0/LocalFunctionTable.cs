using System;
using System.Text;
using System.Collections.Generic;

namespace DeepLingo {

    public class LocalFunctionTable: IEnumerable<KeyValuePair<string, Tuple<string,int>>> {

        IDictionary<string, Tuple<string,int>> data = 
                        new SortedDictionary<string, Tuple<string,int>>();

        //-----------------------------------------------------------
        public override string ToString() {
            var sb = new StringBuilder();
            //sb.Append("\tLocal Symbol Table\n");
            sb.Append("\t====================\n");
            sb.Append("\t|name\t|type\t|pos\t|\n");
            
            foreach (var entry in data) {
                sb.Append(String.Format("\t|{0}\t|{1}\t|{2}\t|\n", 
                                        entry.Key, entry.Value.Item1,
                                        entry.Value.Item2));
            }
            sb.Append("\t====================\n");
            return sb.ToString();
        }

        //-----------------------------------------------------------
        public Tuple<string,int> this[string key] {
            get {
                return data[key];
            }
            set {
                data[key] = value;
            }
        }
        
        public void addVar(string name, string type, int pos){
            var tupla = new Tuple<string, int>(
                type,pos);
            this[name] = tupla;
        }

        //-----------------------------------------------------------
        public bool Contains(string key) {
            return data.ContainsKey(key);
        }

        //-----------------------------------------------------------
        public IEnumerator<KeyValuePair<string, Tuple<string,int>>> GetEnumerator() {
            return data.GetEnumerator();
        }

        //-----------------------------------------------------------
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
