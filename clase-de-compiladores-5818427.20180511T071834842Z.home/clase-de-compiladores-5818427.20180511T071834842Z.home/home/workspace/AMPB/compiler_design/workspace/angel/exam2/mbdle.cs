//---------------------------------------------------------
// Jose AngelPrado Dupont A01373242
//---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public enum TokenCategory {
    ATOM, EOL, ILLEGAL, OPENB, CLOSEB, OPENM, CLOSEM, OPENP, CLOSEP, OPENJ, CLOSEJ, COMA
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
            @"(\w+)|({)|(})|(<)|(>)|(\[)|(\])|(\()|(\))|(\s)|(,)|(.)",
            RegexOptions.Compiled | RegexOptions.Multiline
        );

    public Scanner(String input) {
        this.input = input;
    }

    public IEnumerable<Token> Start() {
        foreach (Match m in regex.Matches(input)) {
            if (m.Groups[1].Success) {
                //console.writeLine("atom");
                yield return new Token(TokenCategory.ATOM);
            } 
            else if (m.Groups[2].Success) {
                 //console.writeLine("{");
                yield return new Token(TokenCategory.OPENB);
            } 
            else if (m.Groups[3].Success) {
                //console.writeLine("}");
                yield return new Token(TokenCategory.CLOSEB);
            }
            else if (m.Groups[4].Success) {
                //console.writeLine("<");
                yield return new Token(TokenCategory.OPENM);
            }
            else if (m.Groups[5].Success) {
                //console.writeLine(">");
                yield return new Token(TokenCategory.CLOSEM);
            }
            else if (m.Groups[6].Success) {
                //console.writeLine("[");
                yield return new Token(TokenCategory.OPENJ);
            }
            else if (m.Groups[7].Success) {
                 //console.writeLine("]");
                yield return new Token(TokenCategory.CLOSEJ);
            }
            else if (m.Groups[8].Success) {
                 //console.writeLine("(");
                yield return new Token(TokenCategory.OPENP);
            }
            else if (m.Groups[9].Success) {
                 //console.writeLine(")");
                yield return new Token(TokenCategory.CLOSEP);
            }
            else if (m.Groups[11].Success) {
                 //console.writeLine(",");
                yield return new Token(TokenCategory.COMA);
            }else if (m.Groups[10].Success) {
                 //console.writeLine("Space");
                continue;
            } else if (m.Groups[12].Success) {
                //console.writeLine("error");
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
            //console.writeLine(category);
            throw new SyntaxError();
        }
    }

    public void Mbdle() {
         if (Current == TokenCategory.ATOM){
             //console.writeLine("aqui1");
            singleAtom();
        }
        else{
            //console.writeLine("aqui2");
            list();
        }
        Expect(TokenCategory.EOL);
    }
    
    public void singleAtom(){
            Expect(TokenCategory.ATOM);
    }
    
    public void list(){
         if (Current == TokenCategory.OPENB){
             //console.writeLine("aqui3");
            Expect(TokenCategory.OPENB);
            list2();
            Expect(TokenCategory.CLOSEB);
        }
        
         else if (Current == TokenCategory.OPENP){
             //console.writeLine("aqui4");
            Expect(TokenCategory.OPENP);
            list2();
            Expect(TokenCategory.CLOSEP);
        }
        else if (Current == TokenCategory.OPENJ){
            //console.writeLine("aqui5");
            Expect(TokenCategory.OPENJ);
            list2();
            Expect(TokenCategory.CLOSEJ);
        }
        else if (Current == TokenCategory.OPENM){
            //console.writeLine("aqui6");
            Expect(TokenCategory.OPENM);
            list2();
            Expect(TokenCategory.CLOSEM);
        }
        else if (Current == TokenCategory.COMA){
            //console.writeLine("aquiNUEVO");
            atom_list_cont();
        }
        
        else {
            //console.writeLine("aqui7");
            throw new SyntaxError();
        }
    }
    
     public void list2()
     {
         //while (Current == TokenCategory.COMA){
           //  //console.writeLine("aqui9");
            //Expect(TokenCategory.COMA);
            if (Current == TokenCategory.OPENB){
                //console.writeLine("aqui10");
                Expect(TokenCategory.OPENB);
                atom_list();
                Expect(TokenCategory.CLOSEB);
                list2();
            }
        
            else if (Current == TokenCategory.OPENP){
                //console.writeLine("aqui11");
                Expect(TokenCategory.OPENP);
                atom_list();
                Expect(TokenCategory.CLOSEP);
                list2();
            }
            else if (Current == TokenCategory.OPENJ){
                //console.writeLine("aqui12");
                Expect(TokenCategory.OPENJ);
                atom_list();
                Expect(TokenCategory.CLOSEJ);
                list2();
            }
            else if (Current == TokenCategory.OPENM){
                //console.writeLine("aqui13");
                Expect(TokenCategory.OPENM);
                atom_list();
                Expect(TokenCategory.CLOSEM);
                list2();
            }
            else if (Current == TokenCategory.ATOM){
                //console.writeLine("aqui13");
                atom_list();
            }
            else if (Current == TokenCategory.COMA){
                Expect(TokenCategory.COMA);
                list2();
            }
            //else {
            //    throw new SyntaxError();
            //}
         }
    //}
    
    public void atom_list(){
        if (Current == TokenCategory.ATOM){
            //console.writeLine("aqui14");
            Expect(TokenCategory.ATOM);
            atom_list_cont();
        }
        else if (Current == TokenCategory.OPENP || Current == TokenCategory.OPENB || Current == TokenCategory.OPENM || Current == TokenCategory.OPENJ ){
            //console.writeLine("aqui15");
            list2();
        }
        else if (Current == TokenCategory.COMA)
        {
            //console.writeLine("aqui16");
            list2();
        }
    }
    
    public void  atom_list_cont(){
        while (Current == TokenCategory.COMA){
            //console.writeLine("aqui17");
            Expect(TokenCategory.COMA);
            list2();
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
