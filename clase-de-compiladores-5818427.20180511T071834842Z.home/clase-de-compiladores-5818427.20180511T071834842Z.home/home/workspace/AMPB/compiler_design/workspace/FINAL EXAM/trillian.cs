/*-------------------------------------------------------------------
Andrea Margarita Pérez Barrera A01373631


 ilasm salida.il
BNF Grammar for the Boolang language:



    Start     ::= Max
    Max       ::= simple
    simple    ::= float
    simple    ::= "*" simple
    simple    ::= "[" max-list "]"
    max-list  ::= max-list "," max
    max-list  ::= max

-------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Trillian {
    
    //---------------------------------------------------------------
    class SyntaxError: Exception {}
    
    //---------------------------------------------------------------
    enum Token {
        FLOAT, MAX, DUP, COMA, SRIGHT, SLEFT, ILLEGAL_CHAR, EOF
    }
    
    //---------------------------------------------------------------
    class TokenN {
    
            readonly string lexeme;
            readonly Token category;
            public string Lexeme { 
                get { return lexeme; }
            }
    
            public Token Category {
                get { return category; }          
            }
    
        public TokenN(string lexeme, 
                     Token category) {
            this.lexeme = lexeme;
            this.category = category;
        }

            public override string ToString() {
                return string.Format("{0}",
                                     lexeme);
            }
        }
    //---------------------------------------------------------------
    class Scanner {
        readonly string input;
        static readonly Regex regex = new Regex(
            @"                             
                (?<Float>         -?[0-9]+(\.[0-9])?)
              | (?<Dup>           [*] )
              | (?<Max>           [!] )
              | (?<SLeft>         \[ )
              | (?<SRight>        \] )
              | (?<Coma>          [,] )
              | (?<Espacios>      \s  )
              | (?<Other>          .  )
            ", 
            RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.Compiled                
            );
        static readonly IDictionary<string, Token> regexLabels =
            new Dictionary<string, Token>() {
                {"Float", Token.FLOAT},
                {"Dup", Token.DUP},
                {"Coma", Token.COMA},
                {"Max", Token.MAX},
                {"SLeft", Token.SLEFT},
                {"SRight", Token.SRIGHT},
            };
        public Scanner(string input) {
            this.input = input;
        }
       public IEnumerable<Token> Start() {
           
            Func<Match, Token, TokenN> newTok = (m, tc) =>
                new TokenN(m.Value, tc);
                
            foreach (Match m in regex.Matches(input)) {
                Console.WriteLine(m);
                if (m.Groups["Espacios"].Length > 0) {
                    // Ignorar espacios.
                } else if (m.Groups["Other"].Length > 0) {
                    yield return Token.ILLEGAL_CHAR;
                } else {
                    foreach (var name in regexLabels.Keys) {
                        if (m.Groups[name].Length > 0) {
                            //lexeme.lex = m.Value;
                            yield return regexLabels[name];
                            //yield return newTok(m, regexLabels[m.Value]);
                            break;
                        }
                    }
                }
            }
            yield return Token.EOF; 
        }
    }
    
    //---------------------------------------------------------------
    class Node: IEnumerable<Node> {
        public string lex = "";
        IList<Node> children = new List<Node>();
        public Node this[int index] {
            get {
                return children[index];
            }
        }
        
        public void Add(Node node) {
            children.Add(node);
        }
         //   public Token AnchorToken { get; set; }

        public IEnumerator<Node> GetEnumerator() {
            return children.GetEnumerator();
        }
        System.Collections.IEnumerator
                System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
        public override string ToString() {
            return GetType().Name;
            //return
            //return String.Format("{0} {1}", GetType().Name, AnchorToken);                                 

        }
        public string ToStringTree() {
            var sb = new StringBuilder();
            TreeTraversal(this, "", sb);
            return sb.ToString();
        }
        static void TreeTraversal(
                Node node, 
                string indent, 
                StringBuilder sb) {
            sb.Append(indent);
            sb.Append(node);
            sb.Append('\n');
            foreach (var child in node.children) {
                TreeTraversal(child, indent + "  ", sb);
            }
        }
    }
    //---------------------------------------------------------------
    class Program:   Node {}
    class Max:       Node {}
    class Dup:       Node {}
    class Float:     Node {}
    class Simple:    Node {}
    class Sum:       Node {}
    //---------------------------------------------------------------
    class Parser {
        IEnumerator<Token> tokenStream;
        public Parser(IEnumerator<Token> tokenStream) {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }
        public Token CurrentToken {
            get { return tokenStream.Current; }
        }
        public Token Expect(Token category) {
            if (CurrentToken == category) {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            } else {
                throw new SyntaxError();                
            }
        }
        public Node Start() {
            var e = new Program() { Max() };
            Expect(Token.EOF);
            return e;
        }
        public Node Max() {            
            var exp1 = Simple();
            //Console.WriteLine(exp1);
            while (CurrentToken == Token.MAX) {
                Expect(Token.MAX);
                var exp2 = new Max() {exp1 , Simple()};
                //var exp2 = new Menos() { exp1, Exp() };
              //  exp2.Add(exp1);
            //    exp2.Add(MasExp());
                exp1 = exp2;
            }
            return exp1;
        }
        

        public Node Sum() {
            var exp1 = Max();
            var exp2 = new Sum() {exp1};  
            while (CurrentToken == Token.COMA) {
                Expect(Token.COMA);
                var exp3 = Max();
                exp2.Add(exp3);
            }
            return exp2;
        }
       
        public Node Simple() {
            Console.WriteLine(CurrentToken);

            switch (CurrentToken) {                
            case Token.SLEFT:
                Expect(Token.SLEFT);
                var exp = Sum();
                Expect(Token.SRIGHT);
                return exp;
  
            case Token.FLOAT:
                Expect(Token.FLOAT); 
                return new Float();  
            case Token.DUP:
                Expect(Token.DUP);
                var exp2 = new Dup() {Simple()};

                return exp2;
                
            default:
               throw new SyntaxError();
            }
        }        
    }
/*    
    //---------------------------------------------------------------
    class CILGenerator {
        public string Visit(Program node) {
            return  ".assembly 'ppt' {}\n\n"
                + ".class public 'salida' extends ['mscorlib']'System'.'Object' {\n"
                + "\t.method public static void 'start'() {\n"
                + "\t\t.entrypoint\n"
                + Visit((dynamic) node[0])
                + "\t\tcall void ['mscorlib']'System'.'Console'::'WriteLine'(string)\n"
                + "\t\tret\n"
                + "\t}\n"
                + "}\n";
        }
        public string Visit(Max node) {
            return Visit((dynamic) node[0])
                + Visit((dynamic) node[1])
                + "\t\tcall float ['mscorlib']'System'.'Math'::'Max'(float64, float64)\n";               
        }
        public string Visit(Dup node) 
        {
            return Visit((dynamic) node[0])
                  + Visit((dynamic) node[0])
                  + "\t\tadd\n";  
        }
        
        public string Visit(Sum node) {
            return "\t\tldc.i8."+ node[0].lexeme +"\n";
        }

        public string Visit(Float node) {
           return "\t\tldc.i8."+ node.lexeme +"\n";
            
        }   
    }*/
    //---------------------------------------------------------------
    class Driver {
        public static void Main(string[] args) {
            try {
                var p = new Parser(
                    new Scanner(args[0]).Start().GetEnumerator());                
                var ast = p.Start();
                Console.WriteLine(ast.ToStringTree());
                /*File.WriteAllText(
                    "salida.il", 
                    new CILGenerator().Visit((dynamic) ast));*/
            } catch (SyntaxError) {
                Console.Error.WriteLine("parse error");
                Environment.Exit(1);
            }
        }
    }   
}