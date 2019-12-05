//==========================================================
// Aldo Reyna Gómez - A01169073
//==========================================================

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum TokenCategory {
    SYMBOL, SUC, PRED, MAX, COMMA, BRACE_OPEN, BRACE_CLOSE, EOL, ILLEGAL, EOF
}

public class Token {
    TokenCategory category;
    String lexeme;
    public TokenCategory Category {
        get { return category; }
    }
    public String Lexeme {
        get { return lexeme; }
    }
    public Token(TokenCategory category, String lexeme) {
        this.category = category;
        this.lexeme = lexeme;
    }
    public override String ToString() {
        return String.Format("[{0}, \"{1}\"]", Category, Lexeme);
    }
}

public class Scanner {
    readonly String input;
    static readonly Regex regex = new Regex(
        @"
            ([a-z])
          | (\n)
          | (\+)
          | (\-)
          | (\{)
          | (\})
          | (\,)
          | (.)
        ", 
        RegexOptions.IgnorePatternWhitespace 
        | RegexOptions.Multiline
        );
        
    public Scanner(String input) {
        this.input = input;
    }
    
    public IEnumerable<Token> Start() {
        foreach (Match m in regex.Matches(input)) {
            if (m.Groups[1].Success) {
                yield return new Token(TokenCategory.SYMBOL, m.Value);
            } else if (m.Groups[2].Success) {
                yield return new Token(TokenCategory.EOL, m.Value);
            } else if (m.Groups[3].Success) {
                yield return new Token(TokenCategory.SUC, m.Value);
            } else if (m.Groups[4].Success) {
                yield return new Token(TokenCategory.PRED, m.Value);
            } else if (m.Groups[5].Success) {
                yield return new Token(TokenCategory.BRACE_OPEN, m.Value);
            } else if (m.Groups[6].Success) {
                yield return new Token(TokenCategory.BRACE_CLOSE, m.Value);
            } else if (m.Groups[7].Success) {
                yield return new Token(TokenCategory.COMMA, m.Value);
            } else if (m.Groups[8].Success) {
                yield return new Token(TokenCategory.ILLEGAL, m.Value);
            }
        }
        yield return new Token(TokenCategory.EOF, "");
    }
}

class SyntaxError: Exception {
}

public class Parser {
  
    static readonly ISet<TokenCategory> firstOfOperator = 
        new HashSet<TokenCategory>() {
            TokenCategory.SUC,
            TokenCategory.PRED
        };
        
    static readonly ISet<TokenCategory> firstOfMax = 
        new HashSet<TokenCategory>() {
            TokenCategory.BRACE_OPEN,
            TokenCategory.BRACE_CLOSE
        };
        
  
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
    
    public char Prog() {
        var result = Expr();
        Expect(TokenCategory.EOF);
        return result;
    }
    
    public char Expr() {
        var letter = 'a'; //Por default.
        if (Current == TokenCategory.SYMBOL) {
            letter = Symbol();
        }
        else if (firstOfMax.Contains(Current)) {
            return Max();
        }
        while (firstOfOperator.Contains(Current)) {
            if (Current == TokenCategory.SUC) {
                return Succesor(letter);
            }
            else if (Current == TokenCategory.PRED) {
                return Predecesor(letter);
            }
        }
        return letter;
    }
    
    public char Succesor(char letter){
        Expect(TokenCategory.SUC);
        if (letter == 'z') {
            return 'a';
        }
        else {
            return (char) (((int)letter) + 1);
        }
    }
    
    public char Predecesor(char letter){
        Expect(TokenCategory.PRED);
        if (letter == 'a') {
            return 'z';
        }
        else {
            return (char) (((int)letter) - 1);
        }
    }
    
    public char Symbol() {
        if (Current == TokenCategory.SYMBOL) {
            var token = Expect(TokenCategory.SYMBOL);
            return token.Lexeme[0];
        } else {
            throw new SyntaxError();
        }
    }
    
    public char Max() {
        Expect(TokenCategory.BRACE_OPEN);
        var result = ExprList();
        Expect(TokenCategory.BRACE_CLOSE);
        result.Sort();
        return result[0];
    }
    
    public List<char> ExprList() {
        var list = new List<char>();
        list.Add(Expr());
        while (Current == TokenCategory.COMMA) {
            Expect(TokenCategory.COMMA);
            list.Add(Expr());
        }
        return list;
    }
    
}

public class Fook {
    public static void Main(String[] args) {
        try {
            while (true) {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == null) {
                    break;
                }
                var parser = new Parser(new Scanner(line).Start().GetEnumerator());
                var result = parser.Prog();
                Console.WriteLine(result);+
                bn
            }
        } catch (SyntaxError) {
            Console.WriteLine("Syntax Error!");
        }
    }
}
