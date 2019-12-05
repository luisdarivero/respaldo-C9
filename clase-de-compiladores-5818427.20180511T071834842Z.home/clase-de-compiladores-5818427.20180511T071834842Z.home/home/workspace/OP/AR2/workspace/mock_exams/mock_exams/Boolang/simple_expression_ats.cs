/*
    Scanner + Parser for the following simple expression language:
    
    exp -> and_exp ("|" and_exp)*
    and_exp -> simple_exp ("&" simple_exp)*
    simple_exp -> "(" exp ")" | "!" simple_exp | "0" | "1"

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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

class SyntaxError: Exception {
}

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
        var node = new Prog {
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

public class Prog : Node { }
public class And : Node { }
public class Or : Node { }
public class Not : Node { }
public class Literal_0 : Node { }
public class Literal_1 : Node { }

public class SimpleExpression {
    public static void Main() {
        var line = Console.ReadLine();
        var parser = new Parser(new Scanner(line).Start().GetEnumerator());
        try {
            //parser.Prog();
            //Console.WriteLine("syntax ok");
            var abstractSyntaxTree = parser.Prog();
            Console.WriteLine(abstractSyntaxTree.ToStringTree());
        } catch (SyntaxError) {
            Console.WriteLine("Parse error!");
        }
    }
}