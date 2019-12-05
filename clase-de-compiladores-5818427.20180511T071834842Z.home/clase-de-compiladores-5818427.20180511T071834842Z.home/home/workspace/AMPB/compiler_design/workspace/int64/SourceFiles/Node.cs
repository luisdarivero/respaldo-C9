using System; 
using System.Collections.Generic;
using System.Text;

namespace Int64{
    class Node: IEnumerable<Node> {
        
        IList<Node> children = new List<Node>();
        
        public Node this[int index]{
            get{ return children[index]; }
        }
    
        public Token AnchorToken { get; set; }
        
        public void Add(Node node){
            children.Add(node);
        }
        
        public IEnumerator<Node> GetEnumerator() {
            return children.GetEnumerator();
        }

        System.Collections.IEnumerator
                System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
        
        public string ToStringTree(){
            var stringBuilder = new StringBuilder();
            TreeTraversal(this, "", stringBuilder);
            return stringBuilder.ToString();
        }
        
        public void TreeTraversal(Node node, string indent, StringBuilder stringBuilder){
            stringBuilder.Append(indent);
            stringBuilder.Append(node.ToString());
            stringBuilder.Append('\n');
            
            foreach(Node child in node.children){ TreeTraversal(child, indent + "  ", stringBuilder); }
        }
        
        public override string ToString() {
            return String.Format("{0} {1}", GetType().Name, AnchorToken);                                 
        }

    }
}