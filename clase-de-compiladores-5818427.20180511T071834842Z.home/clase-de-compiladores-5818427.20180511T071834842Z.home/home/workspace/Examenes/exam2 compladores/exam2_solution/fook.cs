//==========================================================
// luis daniel rivero sosa A01374527
//==========================================================

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum TokenCategory {
    SYMBOL, EOL, ILLEGAL, PLUS, NEG, CI, CD, COMMA, EOF
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
    static readonly Regex regex = new Regex(@"([a-z])|(\+)|(\-)|({)|(})|(,)|(\n)|(\s)|(.)");
    public Scanner(String input) {
        this.input = input;
    }
    public IEnumerable<Token> Start() {
        foreach (Match m in regex.Matches(input)) {
            if (m.Groups[1].Success) {
                yield return new Token(TokenCategory.SYMBOL, m.Value);
            } else if (m.Groups[2].Success) {
                yield return new Token(TokenCategory.PLUS, m.Value);
            } else if (m.Groups[3].Success) {
                yield return new Token(TokenCategory.NEG, m.Value);
            } else if (m.Groups[4].Success) {
                yield return new Token(TokenCategory.CI, m.Value);
            } else if (m.Groups[5].Success) {
                yield return new Token(TokenCategory.CD, m.Value);
            } else if (m.Groups[6].Success) {
                yield return new Token(TokenCategory.COMMA, m.Value);
            } else if (m.Groups[7].Success || m.Groups[8].Success) {
                continue;
            } else if (m.Groups[9].Success) {
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
    
    public List<char> listaCaracteres(){
        List<char> caracteres = new List<char>()
        {
            'a','b','c','d','e','f','g','h','i','j','k','l','m',
            'n','o','p','q','r','s','t','u','v','w','x','y','z'
        };
        return caracteres;
    }
    
    public int getLetterIndex(char letter){
        return listaCaracteres().IndexOf(letter);
    }
    
    public char getLetterByNumber(int number){
        var lista = listaCaracteres();
        return lista[number];
    }
    
    public int rest (int number){
        if(number == 0){
            return ((listaCaracteres().Count) - 1);
        }
        return number-1;
    }
    
    public int sum (int number){
        if(number >= listaCaracteres().Count - 1){
            return 0;
        }
        return number+1;
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
        if(Current == TokenCategory.SYMBOL){
            return Symbol();
        }
        else if(Current == TokenCategory.CI){
            return Max();
        }
        else{
            throw new SyntaxError();
        }
        
    }
    
    public char Max(){
        Expect(TokenCategory.CI);
        char letter;
        if(Current == TokenCategory.SYMBOL){
            letter = Symbol();
        }
        else if(Current == TokenCategory.CI){
            letter = Max();
        }
        else{
            throw new SyntaxError();
        }
        
        
        
        while(Current ==TokenCategory.COMMA){
            Expect(TokenCategory.COMMA);
            if(Current == TokenCategory.SYMBOL){
                char letter2 = Symbol();
                if(getLetterIndex(letter) < getLetterIndex(letter2)){
                    letter = letter2;
                }
            }
            else if(Current == TokenCategory.CI){
                char letter2 = Max();
                if(getLetterIndex(letter) < getLetterIndex(letter2)){
                    letter = letter2;
                }
            }
            else{
                throw new SyntaxError();
            }
        }
        
        Expect(TokenCategory.CD);
        while(Current == TokenCategory.PLUS ||
                    Current == TokenCategory.NEG){
            if(Current == TokenCategory.PLUS){
                Expect(TokenCategory.PLUS);
                int letterIndex = getLetterIndex(letter);
                
                letterIndex = sum(letterIndex);
                
                letter = getLetterByNumber(letterIndex);
                
            }
            else{
                Expect(TokenCategory.NEG);
                int letterIndex = getLetterIndex(letter);
                letterIndex = rest(letterIndex);
                letter = getLetterByNumber(letterIndex);
            }
        }
        
        return letter;
    }
    
    public char Symbol() {
        if (Current == TokenCategory.SYMBOL) {
            var token = Expect(TokenCategory.SYMBOL);
            var letter =  token.Lexeme[0];
            while(Current == TokenCategory.PLUS ||
                    Current == TokenCategory.NEG){
                if(Current == TokenCategory.PLUS){
                    Expect(TokenCategory.PLUS);
                    int letterIndex = getLetterIndex(letter);
                    
                    letterIndex = sum(letterIndex);
                    
                    letter = getLetterByNumber(letterIndex);
                    
                }
                else{
                    Expect(TokenCategory.NEG);
                    int letterIndex = getLetterIndex(letter);
                    letterIndex = rest(letterIndex);
                    letter = getLetterByNumber(letterIndex);
                }
            }
            return letter;
        } else {
            throw new SyntaxError();
        }
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
                Console.WriteLine(result);
            }
        } catch (SyntaxError) {
            Console.WriteLine("Syntax Error!");
        }
    }
}
