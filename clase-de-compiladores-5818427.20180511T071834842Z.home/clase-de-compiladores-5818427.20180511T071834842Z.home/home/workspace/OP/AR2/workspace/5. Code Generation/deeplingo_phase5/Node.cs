// This programme is based on the Buttercup's compiler code, provided by Ariel Ortiz.

/* 
    DeepLingo compiler - Specific node subclasses for the AST (Abstract Syntax Tree).
    Date: 12-March-2018
    Authors:
          A01169073 Aldo Reyna
          A01375051 Marina Torres
    File name: Node.cs
*/

using System; 
using System.Collections.Generic;
using System.Text;

namespace DeepLingo {

    class Node: IEnumerable<Node> {

        IList<Node> children = new List<Node>();

        public Node this[int index] {
            get {
                return children[index];
            }
        }

        public Token AnchorToken { get; set; }

        public void Add(Node node) {
            children.Add(node);
        }

        public IEnumerator<Node> GetEnumerator() {
            return children.GetEnumerator();
        }

        System.Collections.IEnumerator
                System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return String.Format("{0} {1}", GetType().Name, AnchorToken);                                 
        }

        public string ToStringTree() {
            var sb = new StringBuilder();
            TreeTraversal(this, "", sb);
            return sb.ToString();
        }
        
        public int CountChildren() {
            int i=0;
            foreach (var n in this) {
                i++;
            }
            return i;
        }

        static void TreeTraversal(Node node, string indent, StringBuilder sb) {
            sb.Append(indent);
            sb.Append(node);
            sb.Append('\n');
            foreach (var child in node.children) {
                TreeTraversal(child, indent + "  ", sb);
            }
        }
    }
}