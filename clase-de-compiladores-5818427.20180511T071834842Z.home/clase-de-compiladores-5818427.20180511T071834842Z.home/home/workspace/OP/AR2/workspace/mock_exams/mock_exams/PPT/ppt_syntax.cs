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
        public void Inicio() {
            Exp();
            Expect(TokenCategory.EOF);
        }
        public void Exp() {
            ExpMas();
            if (Current == TokenCategory.MENOS) {
                Expect(TokenCategory.MENOS);
                Exp();
            }
        }
        public void ExpMas(){
            ExpSimple();
            while (Current == TokenCategory.MAS) {
                Expect(TokenCategory.MAS);
                ExpSimple();
            }
        }
        public void ExpSimple() {
            switch (Current) {
            case TokenCategory.PIEDRA:
                Expect(TokenCategory.PIEDRA);
                break;
            case TokenCategory.PAPEL:
                Expect(TokenCategory.PAPEL);
                break;
            case TokenCategory.TIJERAS:
                Expect(TokenCategory.TIJERAS);
                break;
            case TokenCategory.PAR_OPEN:
                Expect(TokenCategory.PAR_OPEN);
                ExpSimple();
                Expect(TokenCategory.PAR_CLOSE);
                break;
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
                parser.Inicio();
                Console.WriteLine("syntax ok");
            } catch (SyntaxError) {
                Console.WriteLine("Parse error!");
            }
        }
    }
}