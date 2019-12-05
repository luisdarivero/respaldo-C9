/*-------------------------------------------------------------------
 ilasm salida.il
BNF Grammar for the Boolang language:

    Start     ::= Exp
    Exp       ::= AndExp
    Exp       ::= Exp "|" AndExp
    AndExp    ::= SimpleExp
    AndExp    ::= AndExp "&" SimpleExp
    SimpleExp ::= "(" Exp ")"
    SimpleExp ::= "!" SimpleExp
    SimpleExp ::= "0"
    SimpleExp ::= "1"
     
-------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Boolang {
    
    //---------------------------------------------------------------
    class SyntaxError: Exception {}
    
    //---------------------------------------------------------------
    enum Token {
        PIEDRA, PAPEL, TIJERAS, PAR_LEFT, PAR_RIGHT, MENOS, MAS, 
        ILLEGAL_CHAR, EOF
    }
                   
    //---------------------------------------------------------------
    class Scanner {
        readonly string input;
        static readonly Regex regex = new Regex(
            @"                             
                (?<Piedra>        piedra)
              | (?<Papel>         papel)
              | (?<Tijeras>       tijeras)
              | (?<Menos>         [-] )
              | (?<Mas>           [+] )
              | (?<ParLeft>       [(] )
              | (?<ParRight>      [)] )
              | (?<Espacios>      \s  )
              | (?<Other>          .  )
            ", 
            RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.Compiled                
            );
        static readonly IDictionary<string, Token> regexLabels =
            new Dictionary<string, Token>() {
                {"Piedra", Token.PIEDRA},
                {"Papel", Token.PAPEL},
                {"Tijeras", Token.TIJERAS},
                {"Mas", Token.MAS},
                {"Menos", Token.MENOS},
                {"ParLeft", Token.PAR_LEFT},
                {"ParRight", Token.PAR_RIGHT}
            };
        public Scanner(string input) {
            this.input = input;
        }
       public IEnumerable<Token> Start() {
            foreach (Match m in regex.Matches(input)) {
                Console.WriteLine(m);
                if (m.Groups["Espacios"].Length > 0) {
                    // Ignorar espacios.
                } else if (m.Groups["Other"].Length > 0) {
                    yield return Token.ILLEGAL_CHAR;
                } else {
                    foreach (var name in regexLabels.Keys) {
                        if (m.Groups[name].Length > 0) {
                            yield return regexLabels[name];
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
        IList<Node> children = new List<Node>();
        public Node this[int index] {
            get {
                return children[index];
            }
        }
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
            return GetType().Name;                                 
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
    class Menos:     Node {}
    class Mas:       Node {}
    class Piedra:    Node {}
    class Papel:     Node {}
    class Tijera: Node {}
    
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
            var e = new Program() { Exp() };
            Expect(Token.EOF);
            return e;
        }
        public Node Exp() {            
            var exp1 = MasExp();
            Console.WriteLine(exp1);
            while (CurrentToken == Token.MENOS) {
                Expect(Token.MENOS);
                var exp2 = new Menos() { exp1, Exp() };
              //  exp2.Add(exp1);
            //    exp2.Add(MasExp());
                exp1 = exp2;
            }
            return exp1;
        }
        public Node MasExp() {
            var exp1 = SimpleExp();
            Console.WriteLine(CurrentToken);
            while (CurrentToken == Token.MAS) {
                Expect(Token.MAS);
                var exp2 = new Mas() { exp1, SimpleExp() };                
                exp1 = exp2;
            }
            return exp1;
        }
        public Node SimpleExp() {
            switch (CurrentToken) {                
            case Token.PAR_LEFT:
                Expect(Token.PAR_LEFT);
                var exp = Exp();
                Expect(Token.PAR_RIGHT);
                return exp;
            case Token.PIEDRA:
                Expect(Token.PIEDRA); 
                return new Piedra();  

            case Token.PAPEL:
                Expect(Token.PAPEL);
                return new Papel();
            case Token.TIJERAS:
                Expect(Token.TIJERAS);
                return new Tijera();
            default:
               throw new SyntaxError();
            }
        }        
    }
    
    //---------------------------------------------------------------
    class CILGenerator {
        public string Visit(Program node) {
            return  ".assembly 'ppt' {}\n\n"
                + ".assembly extern 'pptlib' {}\n\n"
                + ".class public 'salida' extends ['mscorlib']'System'.'Object' {\n"
                + "\t.method public static void 'inicio'() {\n"
                + "\t\t.entrypoint\n"
                + Visit((dynamic) node[0])
                + "\t\tcall void ['mscorlib']'System'.'Console'::'WriteLine'(string)\n"
                + "\t\tret\n"
                + "\t}\n"
                + "}\n";
        }
        public string Visit(Mas node) {
            return Visit((dynamic) node[0])
                + Visit((dynamic) node[1])
                + "\t\tcall string class ['pptlib']'ppt'.'Runtime'::'mas'(string, string)\n";               
        }
        public string Visit(Menos node) {
            return Visit((dynamic) node[0])
                + Visit((dynamic) node[1])
                + "\t\tcall string class ['pptlib']'ppt'.'Runtime'::'menos'(string, string)\n";  
        }
        
        public string Visit(Piedra node) {
            return "\t\tldstr \"piedra\"\n";
        }
        
        public string Visit(Papel node) {
            return "\t\tldstr \"papel\"\n";
            
        }        public string Visit(Tijera node) {
            return "\t\tldstr \"tijeras\"\n";
        }
    }
    //---------------------------------------------------------------
    class Driver {
        public static void Main(string[] args) {
            try {
                var p = new Parser(
                    new Scanner(args[0]).Start().GetEnumerator());                
                var ast = p.Start();
                Console.WriteLine(ast.ToStringTree());
                File.WriteAllText(
                    "salida.il", 
                    new CILGenerator().Visit((dynamic) ast));
            } catch (SyntaxError) {
                Console.Error.WriteLine("parse error");
                Environment.Exit(1);
            }
        }
    }   
}