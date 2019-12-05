/*

    Scanner + Parser for the following simple expression language:

    Expr -> Expr "+" Term
    Expr -> Term
    Term -> Term "*" Pow
    Term -> Pow
    Pow -> Fact "^" Pow
    Pow -> Fact
    Fact -> Int
    Fact -> "(" Expr ")"

*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum TokenCategory {
    PLUS, TIMES, POW, PAR_OPEN, PAR_CLOSE, INT, EOF, ILLEGAL
}

public class Token {
    public TokenCategory Category { get; }
    public String Lexeme { get; }
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
    static readonly Regex regex = new Regex(@"([+])|([*])|([(])|([)])|(\d+)|(\s)|(\^)|(.)");
    public Scanner(String input) {
        this.input = input;
    }
    public IEnumerable<Token> Start() {
        foreach (Match m in regex.Matches(input)) {
            if (m.Groups[1].Length > 0) {
                yield return new Token(TokenCategory.PLUS, m.Value);
            } else if (m.Groups[2].Length > 0) {
                yield return new Token(TokenCategory.TIMES, m.Value);
            } else if (m.Groups[3].Length > 0) {
                yield return new Token(TokenCategory.PAR_OPEN, m.Value);
            } else if (m.Groups[4].Length > 0) {
                yield return new Token(TokenCategory.PAR_CLOSE, m.Value);
            } else if (m.Groups[5].Length > 0) {
                yield return new Token(TokenCategory.INT, m.Value);
            } else if (m.Groups[6].Length > 0) {
                continue;
            } else if (m.Groups[7].Length > 0) {
                yield return new Token(TokenCategory.POW, m.Value);
            } else if (m.Groups[8].Length > 0) {
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
    public int Prog() {
        var x = Expr();
        Expect(TokenCategory.EOF);
        return x;
    }
    public int Expr() {
        var x = Term();
        while (Current == TokenCategory.PLUS) {
            Expect(TokenCategory.PLUS);
            x += Term();
        }
        return x;
    }
    public int Term() {
        var x = Pow();
        while (Current == TokenCategory.TIMES) {
            Expect(TokenCategory.TIMES);
            x *= Pow();
        }
        return x;
    }
    public int Pow() {
        var x = Fact();
        if (Current == TokenCategory.POW) {
            Expect(TokenCategory.POW);
            x = (int) Math.Pow(x, Pow());    
        }
        return x;
    }
    public int Fact() {
        switch (Current) {
        case TokenCategory.INT:
            var token = Expect(TokenCategory.INT);
            return Convert.ToInt32(token.Lexeme);
        case TokenCategory.PAR_OPEN:
            Expect(TokenCategory.PAR_OPEN);
            var x = Expr();
            Expect(TokenCategory.PAR_CLOSE);
            return x;
        default:
            throw new SyntaxError();
            return 0;
        }
    }
}

public class SimpleExpression {
    public static void Main() {
        var line = Console.ReadLine();
        var parser = new Parser(new Scanner(line).Start().GetEnumerator());
        try {
            var result = parser.Prog();
            Console.WriteLine(result);
        } catch (SyntaxError) {
            Console.Error.WriteLine("Found syntax error!");  
        }
    }
}