/*-------------------------------------------------------------------
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

namespace Boolang {
    //---------------------------------------------------------------
    class SyntaxError: Exception {}
    
    //---------------------------------------------------------------
    class lex {
    public static string curlex = "";
    }
   enum Token {
        FLOAT, MAX, DUP, OPEN, CLOSE, SUM, ESPACIO, 
        ILLEGAL_CHAR, EOF
    }
    

      
                   
    //---------------------------------------------------------------
    class Scanner {
        readonly string input;
        static readonly Regex regex = new Regex(
            @"                             
                (?<float>       -?[0-9]+([.][0-9]+)?) 
              | (?<max>                       [!])
              | (?<dup>                       [*])
              | (?<open>                     [[] )
              | (?<close>                    []] )
              | (?<sum>                      [,] )
              | (?<Espacios>      \s  )
              | (?<Other>          .  )
            ", 
            RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.Compiled                
            );
        static readonly IDictionary<string, Token> regexLabels =
            new Dictionary<string, Token>() {
                {"float", Token.FLOAT},
                {"max", Token.MAX},
                {"dup", Token.DUP},
                {"open", Token.OPEN},
                {"close", Token.CLOSE},
                {"sum", Token.SUM},
                //{"ParRight", Token.PAR_RIGHT}
            };
        public Scanner(string input) {
            this.input = input;
        }
       public IEnumerable<Token> Start() {
            foreach (Match m in regex.Matches(input)) {
                //console.writeLine(m);
                if (m.Groups["Espacios"].Length > 0) {
                    // Ignorar espacios.
                } else if (m.Groups["Other"].Length > 0) {
                    yield return Token.ILLEGAL_CHAR;
                } else {
                    foreach (var name in regexLabels.Keys) {
                        if (m.Groups[name].Length > 0) {
                            lex.curlex = m.Value;
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
        public string lexeme = "";
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
             //sb.Append(String.Format(lex.curlex));
            TreeTraversal(this, "", sb);
            return sb.ToString();
        }
        static void TreeTraversal(
                Node node, 
                string indent, 
                StringBuilder sb) {
            sb.Append(indent);
            sb.Append(node+" "+node.lexeme);
            sb.Append('\n');
            foreach (var child in node.children) {
                TreeTraversal(child, indent + "  ", sb);
            }
        }
    }
    
    //---------------------------------------------------------------
    class Program:   Node {}
    class Max:     Node {}
    //class simple:       Node {}
    class MAX_list:    Node {}
    class Dup:        Node{}
    class Sum:        Node{}
    class Float:      Node{}
    
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
        public Node Program() {
            var e = new Program() { Max() };
            //Console.WriteLine("aqui2");
            Expect(Token.EOF);
            return e;
        }
        public Node Max() {      
            var exp1 = Simple();
            //console.writeLine(exp1);
            while (CurrentToken == Token.MAX) {
                var max = new Max();
                //max.AnchorToken = 
                //Console.WriteLine("aqui3");
                Expect(Token.MAX);
                var exp2 = Max();
                max.Add(exp1);
                max.Add(exp2);
                exp1 = max;
                //exp2.Add(exp1);
                //exp2.Add(MasExp());
            }
            //exp1 = exp2;
            return exp1;
        }
        public Node Simple() {
            if(CurrentToken == Token.FLOAT)
            {
            var floa = new Float();
            floa.lexeme = lex.curlex;
            //floa.lexeme = CurrentToken.lexeme;
            Console.WriteLine(floa.lexeme);
            //Console.WriteLine(floa.lexeme);
            //floa.AnchorToken = 
            //Console.WriteLine("aqui4");
            Expect(Token.FLOAT);
            //var exp1 = MaxList();
            ////console.writeLine(CurrentToken);
                return floa;
            }
            if(CurrentToken == Token.DUP){
                var dup = new Dup();
                //dup.AnchorToken = 
                //Console.WriteLine("aqui5");
                Expect(Token.DUP);
                var exp1 = Simple();
                dup.Add(exp1);
                return dup;
            }
            else if(CurrentToken == Token.OPEN){
                Expect(Token.OPEN);
                var dup =  MaxList();
                
                //dup.AnchorToken = 
                //Console.WriteLine("aqui6");
                //xpect(Token.DUP);
                Expect(Token.CLOSE);
                return dup;
            }
            //console.writeLine("aqui");
             throw new SyntaxError();
             return null;
        }
        public Node MaxList()
        {
            var max = Max();
            var sum = new Sum();
            sum.Add(max);
            while(CurrentToken == Token.SUM)
            {
                //Console.WriteLine("aqui7");
                 Expect(Token.SUM);
                // Console.WriteLine("aqui8");
                 var exp1 = MaxList();
                 sum.Add(exp1);
                 //max = sum;
                 
            }
            return sum;
            
        }
            
    }
    //---------------------------------------------------------------
    class CILGenerator {
        public  int GetUnicodeCodePoints(string s)
        {
            int unicodeCodePoint = 0;
            for (int i = 0; i < s.Length; i++){
                unicodeCodePoint = char.ConvertToUtf32(s, i);
                if (unicodeCodePoint > 0xffff){
                    i++;
                }
            }
            return unicodeCodePoint;
        }
        public string Visit(Program node)
        {
            return  ".assembly 'ppt' {}\n\n"
                + ".assembly extern 'pptlib' {}\n\n"
                + ".class public 'salida' extends ['mscorlib']'System'.'Object' {\n"
                + "\t.method public static void 'inicio'() {\n"
                + "\t\t.entrypoint\n"
                + Visit((dynamic) node[0])
                + "\t\tcall void ['mscorlib']'System'.'Console'::'WriteLine'(float64)\n"
                + "\t\tret\n"
                + "\t}\n"
                + "}\n";
        }
        public string Visit(Max node) 
        {
            //Console.WriteLine("aqui");
            var res = "";
            if(GetUnicodeCodePoints(Visit((dynamic) node[0])) >  GetUnicodeCodePoints(Visit((dynamic) node[1])))
            {
                res = Visit((dynamic) node[0]);
            }
            else
            {
                res = Visit((dynamic) node[1]);
            }
            return res;
        }
        public string Visit(Dup node) 
        {
            return Visit((dynamic) node[0])
                + Visit((dynamic) node[0])
                + "\t\tadd\n";  
        }
        
        public string Visit(Sum node) {
            var str = "";
            var i = 0;
            str = str + Visit((dynamic) node[0]);
            try{
                while(node[i+1].GetType() == typeof(Sum)){
                    Console.WriteLine("print aqui");
                    str = str + Visit((dynamic) node[1][0])+"\n\t\tadd\n";
                     i = i + 1;
                }
            }
            catch(System.ArgumentOutOfRangeException e){
                    
            }
                 //str = str +Visit((dynamic) node[0]);
            return str;
        }

        public string Visit(Float node) {
           return "\t\tldc.r8 "+node.lexeme+"\n";
        }       
    }
    //---------------------------------------------------------------
    class Driver {
        public static void Main(string[] args) {
            try {
                var p = new Parser(
                    new Scanner(args[0]).Start().GetEnumerator());                
                var ast = p.Program();
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