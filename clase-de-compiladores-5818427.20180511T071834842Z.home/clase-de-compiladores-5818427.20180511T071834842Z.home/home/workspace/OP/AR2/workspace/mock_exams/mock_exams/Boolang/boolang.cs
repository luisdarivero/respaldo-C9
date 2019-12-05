/*
    LL(1) Grammar for the Boolang language:
    Prog -> Expr Eof
    e1xp -> and_exp ("|" and_exp)*
    and_exp -> simple_exp ("&" simple_exp)*
    simple_exp -> "(" exp ")" | "!" simple_exp | "0" | "1"

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Boolang {
    
    //---------------------------------------------------------------
    class SyntaxError: Exception {}
    
    //---------------------------------------------------------------
    public enum TokenCategory {
        AND, OR, NOT, PAR_OPEN, PAR_CLOSE, LITERAL_0, LITERAL_1, EOF, ILLEGAL
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
        static readonly Regex regex = new Regex(@"([&])|([|])|([!])|([(])|([)])|([0])|([1])|(\s)|(.)");
        public Scanner(String input) {
            this.input = input;
        }
        public IEnumerable<Token> Start() {
            foreach (Match m in regex.Matches(input)) {
                if (m.Groups[1].Length > 0) {
                    yield return new Token(TokenCategory.AND, m.Value);
                } else if (m.Groups[2].Length > 0) {
                    yield return new Token(TokenCategory.OR, m.Value);
                } else if (m.Groups[3].Length > 0) {
                    yield return new Token(TokenCategory.NOT, m.Value);
                } else if (m.Groups[4].Length > 0) {
                    yield return new Token(TokenCategory.PAR_OPEN, m.Value);
                } else if (m.Groups[5].Length > 0) {
                    yield return new Token(TokenCategory.PAR_CLOSE, m.Value);
                } else if (m.Groups[6].Length > 0) {
                    yield return new Token(TokenCategory.LITERAL_0, m.Value);
                } else if (m.Groups[7].Length > 0) {
                    yield return new Token(TokenCategory.LITERAL_1, m.Value);
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
    public class Prog : Node { }
    public class And : Node { }
    public class Or : Node { }
    public class Not : Node { }
    public class Literal_0 : Node { }
    public class Literal_1 : Node { }
    
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
        public Node Prog() {
            var node = new Prog() {
                Expr()
            };
            Expect(TokenCategory.EOF);
            return node;
        }
        public Node Expr() {
            var node1 = AndExpr();
            while (Current == TokenCategory.OR) {
                var node2 = new Or() {
                    AnchorToken = Expect(TokenCategory.OR)
                };
                node2.Add(node1);
                node2.Add(AndExpr());
                node1 = node2;
            }
            return node1;
        }
        public Node AndExpr(){
            var node1 = SimpleExpr();
            while (Current == TokenCategory.AND) {
                var node2 = new And() {
                    AnchorToken = Expect(TokenCategory.AND)
                };
                node2.Add(node1);
                node2.Add(SimpleExpr());
                node1 = node2;
            }
            return node1;
        }
        public Node SimpleExpr() {
            switch (Current) {
            case TokenCategory.PAR_OPEN:
                Expect(TokenCategory.PAR_OPEN);
                var n = Expr();
                Expect(TokenCategory.PAR_CLOSE);
                return n;
            case TokenCategory.NOT:
                var node1 = new Not() {
                    AnchorToken = Expect(TokenCategory.NOT)
                };
                node1.Add(SimpleExpr());
                return node1;
            case TokenCategory.LITERAL_0:
                return new Literal_0() {
                    AnchorToken = Expect(TokenCategory.LITERAL_0)
                };
            case TokenCategory.LITERAL_1:
                return new Literal_1() {
                    AnchorToken = Expect(TokenCategory.LITERAL_1)
                };
            default:
                throw new SyntaxError();
            }
        }
    }
    
    //---------------------------------------------------------------
    public class CILGenerator {
        
        public String Visit(Prog node) {
            return
    @"
    // CIL example program.
    //
    // To assemble:
    //                 ilasm output.il
    
    .assembly 'example' { }
    
    .class public 'Test' extends ['mscorlib']'System'.'Object' {
        .method public static void 'start'() {
            .entrypoint"
        + "\n"
        + Visit((dynamic) node[0])
        + "\t\tcall void class ['mscorlib']'System'.'Console'::'WriteLine'(int32)\n"
        + "\t\tret\n"
        + "\t}\n"
        + "}\n";
        }
        
        public String Visit(And node) {
            return Visit((dynamic) node[0])
            + Visit((dynamic) node[1])
            + "\t\tand\n";
        }
        
        public String Visit(Or node) {
            return Visit((dynamic) node[0])
            + Visit((dynamic) node[1])
            + "\t\tor\n";
        }
        
        public String Visit(Not node) {
            return "\t\tldc.i4.1\n"
            + Visit((dynamic) node[0])
            + "\t\txor\n";
        }
        
        public String Visit(Literal_0 node) {
            return "\t\tldc.i4.0\n";
        }
        
        public String Visit(Literal_1 node) {
            return "\t\tldc.i4.1\n";
        }
        
    }
    
    //---------------------------------------------------------------
    public class Driver {
        public static void Main(string[] args) {
            try {
                var parser = new Parser(new Scanner(args[0]).Start().GetEnumerator());
                var tree = parser.Prog();
                Console.WriteLine(tree.ToStringTree());
                File.WriteAllText(
                    "output.il",
                    new CILGenerator().Visit((dynamic) tree));
            } catch (SyntaxError) {
                Console.WriteLine("Parse error!");
            }
        }
    }
    
}

