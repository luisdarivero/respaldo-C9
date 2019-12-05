//---------------------------------------------------------
// Andrea Margarita Pérez Barrera A01373631
//---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public enum TokenCategory {
    ATOM, COMA, OPENSB, CLOSESB, OPENP, CLOSEP, OPENAB, CLOSEAB, OPENCB, CLOSECB, EOL, ILLEGAL
}

public class Token {

    TokenCategory category;

    public TokenCategory Category {
        get { return category; }
    }

    public Token(TokenCategory category) {
        this.category = category;
    }
}

public class Scanner {

    readonly String input;

    static readonly Regex regex =
        new Regex(
            @"(\w+)|(,)|(\()|(\))|(\[)|(\])|(<)|(>)|({)|(})|(\s)|(.)",
            RegexOptions.Compiled | RegexOptions.Multiline
        );

    public Scanner(String input) {
        this.input = input;
    }

    public IEnumerable<Token> Start() {
        foreach (Match m in regex.Matches(input)) {
            if (m.Groups[1].Success) {
                yield return new Token(TokenCategory.ATOM);
            } else if (m.Groups[2].Success) {
                yield return new Token(TokenCategory.COMA);
            } else if (m.Groups[3].Success) {
                yield return new Token(TokenCategory.OPENP);
            } else if (m.Groups[4].Success) {
                yield return new Token(TokenCategory.CLOSEP);
            } else if (m.Groups[5].Success) {
                yield return new Token(TokenCategory.OPENSB);
            } else if (m.Groups[6].Success) {
                yield return new Token(TokenCategory.CLOSESB);
            } else if (m.Groups[7].Success) {
                yield return new Token(TokenCategory.OPENAB);
            } else if (m.Groups[8].Success) {
                yield return new Token(TokenCategory.CLOSEAB);
            } else if (m.Groups[9].Success) {
                yield return new Token(TokenCategory.OPENCB);
            } else if (m.Groups[10].Success) {
                yield return new Token(TokenCategory.CLOSECB);
            } else if (m.Groups[11].Success) {
                continue;
            } else if (m.Groups[12].Success) {
                yield return new Token(TokenCategory.ILLEGAL);
            }
            
        }
        yield return new Token(TokenCategory.EOL);
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

    public void Mbdle() {
        //while(Current != TokenCategory.EOL){
             first();  
        //}
        Expect(TokenCategory.EOL);
    }
    
    public void first(){
        if(Current == TokenCategory.ATOM){
            Expect(TokenCategory.ATOM);
        }
        else{
            list();
        }
        
    }
    
    public void list(){
        switch(Current){
            case TokenCategory.OPENP:
                Expect(TokenCategory.OPENP);
                atoms_list();
                Expect(TokenCategory.CLOSEP);
                break;
            case TokenCategory.OPENSB:
                Expect(TokenCategory.OPENSB);
                atoms_list();
                Expect(TokenCategory.CLOSESB);
                break;
            case TokenCategory.OPENAB:
                Expect(TokenCategory.OPENAB);
                atoms_list();
                Expect(TokenCategory.CLOSEAB);
                break;
            case TokenCategory.OPENCB:
                Expect(TokenCategory.OPENCB);
                atoms_list();
                Expect(TokenCategory.CLOSECB);
                break;
            default:
                throw new SyntaxError();
                
        }
    }
    
    public void atoms_list(){
        if(Current == TokenCategory.ATOM){
            Expect(TokenCategory.ATOM);
            if(Current == TokenCategory.COMA){
                Expect(TokenCategory.COMA);
                atoms_list();
            }
            
        }
        
      if(Current != TokenCategory.CLOSEAB && Current != TokenCategory.CLOSECB && Current != TokenCategory.CLOSESB && Current != TokenCategory.CLOSEP){
            first();
            if(Current == TokenCategory.COMA){
                Expect(TokenCategory.COMA);
                atoms_list();
            }
      }
       
    }
}

public class MBDLE {

    public static void Main(String[] args) {
        try {
            while (true) {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == null) {
                    break;
                }
                var parser = new Parser(new Scanner(line).Start().GetEnumerator());
                parser.Mbdle();
                Console.WriteLine("syntax ok");
            }
        } catch (SyntaxError) {
            Console.WriteLine("syntax error");
        }
    }
}
