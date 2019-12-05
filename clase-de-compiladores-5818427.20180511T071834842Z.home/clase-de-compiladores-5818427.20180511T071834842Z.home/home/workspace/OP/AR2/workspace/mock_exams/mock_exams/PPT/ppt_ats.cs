/*-------------------------------------------------------------------

Gramática BNF del lenguaje Ppt:

    Inicio    ::= Exp
    Exp       ::= ExpMas 
    Exp       ::= ExpMas "-" Exp
    ExpMas    ::= ExpSimple
    ExpMas    ::= ExpMas "+" ExpSimple
    ExpSimple ::= "piedra"
    ExpSimple ::= "papel"
    ExpSimple ::= "tijeras"
    ExpSimple ::= "(" Exp ")"
    
Convertido a LL(1):
    
    Inicio -> Exp
    Exp -> ExpMas | "-" Exp
    ExpMas -> ExpSimple ("+" ExpSimple)*
    ExpSimple -> "piedra" | "papel" | "tijeras" | "(" Exp ")"
 
     
-------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PPT {
    
    //---------------------------------------------------------------
    class SyntaxError: Exception {
    }
    
    public enum TokenCategory {
        MAS, MENOS, PAR_OPEN, PAR_CLOSE, PIEDRA, PAPEL, TIJERAS, EOF, ILLEGAL
    }
    
    public class Token {
        public TokenCategory Category;
        public String Lexeme;
        public Token(TokenCategory category, String lexeme) {
            Category = category;
            Lexeme = lexeme;
        }
        public override String ToString() {
            return String.Format("[{0}, \"{1}\"]", Category, Lexeme);
        }
    }
    
    //---------------------------------------------------------------
    public class Scanner {
        readonly String input;
        static readonly Regex regex = new Regex(@"([+])|([-])|([(])|([)])|(piedra)|(papel)|(tijeras)|(\s)|(.)");
        public Scanner(String input) {
            this.input = input;
        }
        public IEnumerable<Token> Start() {
            foreach (Match m in regex.Matches(input)) {
                if (m.Groups[1].Length > 0) {
                    yield return new Token(TokenCategory.MAS, m.Value);
                } else if (m.Groups[2].Length > 0) {
                    yield return new Token(TokenCategory.MENOS, m.Value);
                } else if (m.Groups[3].Length > 0) {
                    yield return new Token(TokenCategory.PAR_OPEN, m.Value);
                } else if (m.Groups[4].Length > 0) {
                    yield return new Token(TokenCategory.PAR_CLOSE, m.Value);
                } else if (m.Groups[5].Length > 0) {
                    yield return new Token(TokenCategory.PIEDRA, m.Value);
                } else if (m.Groups[6].Length > 0) {
                    yield return new Token(TokenCategory.PAPEL, m.Value);
                } else if (m.Groups[7].Length > 0) {
                    yield return new Token(TokenCategory.TIJERAS, m.Value);
                } else if (m.Groups[8].Length > 0) {
                    continue;
                } else if (m.Groups[9].Length > 0) {
                    yield return new Token(TokenCategory.ILLEGAL, m.Value);
                }
            }
            yield return new Token(TokenCategory.EOF, "");
        }
    }
    
    //---------------------------------------------------------------
    public class Node : IEnumerable<Node> {
    
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
            return String.Format("{0}", GetType().Name);
        }
    
        public string ToStringTree() {
            var sb = new StringBuilder();
            TreeTraversal(this, "", sb);
            return sb.ToString();
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
    
    //---------------------------------------------------------------
    public class Programa : Node { }
    public class Mas : Node { }
    public class Menos : Node { }
    public class Piedra : Node { }
    public class Papel : Node { }
    public class Tijeras : Node { }
    
    //---------------------------------------------------------------
    public class Parser {
        IEnumerator<Token> tokenStream;
        public Parser(IEnumerator<Token> tokenStream) {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }
        public TokenCategory Current {
            get { return tokenStream.Current.Category; }
        }
        public Token Expect(TokenCategory category) {
            if (Current == category) {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            } else {
                throw new SyntaxError();
            }
        }
        public Node Inicio() {
            var node = new Programa() {
                Exp()    
            };
            Expect(TokenCategory.EOF);
            return node;
        }
        public Node Exp() {
            var node1 = ExpMas();
            if (Current == TokenCategory.MENOS) {
                Expect(TokenCategory.MENOS);
                var node2 = new Menos() {
                    node1, Exp()
                };
                node1 = node2;
            }
            return node1;
        }
        public Node ExpMas(){
            var node1 = ExpSimple();
            while (Current == TokenCategory.MAS) {
                Expect(TokenCategory.MAS);
                var node2 = new Mas() {
                    node1, ExpSimple()
                };
                node1 = node2;
            }
            return node1;
        }
        public Node ExpSimple() {
            switch (Current) {
            case TokenCategory.PIEDRA:
                Expect(TokenCategory.PIEDRA);
                return new Piedra();
            case TokenCategory.PAPEL:
                Expect(TokenCategory.PAPEL);
                return new Papel();
            case TokenCategory.TIJERAS:
                Expect(TokenCategory.TIJERAS);
                return new Tijeras();
            case TokenCategory.PAR_OPEN:
                Expect(TokenCategory.PAR_OPEN);
                var n = ExpSimple();
                Expect(TokenCategory.PAR_CLOSE);
                return n;
            default:
                throw new SyntaxError();
            }
        }
    }
    
    //---------------------------------------------------------------
    public class Driver {
        public static void Main(string[] args){
            var line = Console.ReadLine();
            var parser = new Parser(new Scanner(line).Start().GetEnumerator());
            try {
                var abstractSyntaxTree = parser.Inicio();
                Console.WriteLine(abstractSyntaxTree.ToStringTree());
            } catch (SyntaxError) {
                Console.WriteLine("Parse error!");
            }
        }
    }
}