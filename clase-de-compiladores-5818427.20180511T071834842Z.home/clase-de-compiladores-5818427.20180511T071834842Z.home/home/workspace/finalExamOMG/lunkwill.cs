/*-------------------------------------------------------------------

Luis Daniel Rivero Sosa A01374527
     
-------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace lunkwill {
    
    //---------------------------------------------------------------
    class SyntaxError: Exception {}
    
    //---------------------------------------------------------------
    enum Token {
        SYMBOL, SUCC, PRED, MAX, COROPEN, CORCLOSE,
        SEMICOLON, EOF, ILEGAL
    }
    
     class TokenMax {

        readonly string lexeme;

        readonly Token category;

        public string Lexeme { 
            get { return lexeme; }
        }
        public Token Category {
            get { return category; }          
        }
        public TokenMax(string lexeme, 
                     Token category) {
            this.lexeme = lexeme;
            this.category = category;
        }
        public override string ToString() {
            return string.Format("{0} , {1}",
                                 category, lexeme);
        }
    }
                   
    //---------------------------------------------------------------
    class Scanner {
        readonly string input;
        static readonly Regex regex = new Regex(
            @"      
                (?<Succ>        succ    )
              | (?<Pred>        pred    )
              | (?<Max>         max     )
              | (?<Symbol>      [a-z]   )
              | (?<CorOpen>     [[]     )
              | (?<CorClose>    []]     )
              | (?<Semicolon>   [;]     )
              | (?<Espacios>    \s      )
              | (?<Otro>        .       )
            ", 
            RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.Compiled                
            );
        static readonly IDictionary<string, Token> regexLabels =
            new Dictionary<string, Token>() {
                {"Succ",  Token.SUCC},
                {"Pred",   Token.PRED},
                {"Max", Token.MAX},
                {"Symbol",     Token.SYMBOL},
                {"CorOpen",   Token.COROPEN},
                {"CorClose",  Token.CORCLOSE},
                {"Semicolon",  Token.SEMICOLON}
            };
        public Scanner(string input) {
            this.input = input;
        }
        public IEnumerable<TokenMax> Start() {
            foreach (Match m in regex.Matches(input)) {
                if (m.Groups["Espacios"].Success) {
                    // Ignorar espacios.
                } else if (m.Groups["Otro"].Success) {
                    yield return (new TokenMax(m.Value,Token.ILEGAL));
                } else {
                    foreach (var name in regexLabels.Keys) {
                        if (m.Groups[name].Success) {
                            yield return (new TokenMax(m.Value,regexLabels[name]));
                            break;
                        }
                    }
                }
            }
            yield return (new TokenMax("",Token.EOF)); 
        }
    }
    
    //---------------------------------------------------------------
    class Node: IEnumerable<Node> {
        IList<Node> children = new List<Node>();
        
        public string myLexeme = "";
        public void modifyLexeme(string l){
            myLexeme = l;
        }
        public string getMyLexeme(){
            return myLexeme;
        }
        public Node this[int index] {
            get {
                return children[index];
            }
        }
        //public Token AnchorToken { get; set; }
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
            string t =  GetType().Name;  
            if(myLexeme != ""){
                t+= " " + myLexeme;
            }
            return t;
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
    class Programa: Node {}
    class Exp:      Node {}
    class Symbol:   Node {}
    class Succ:     Node {}
    class Pred:     Node {}
    class Max:      Node {}
    class ExprLst:  Node {}

    //---------------------------------------------------------------
    class Parser {
        IEnumerator<TokenMax> tokenStream;
        public Parser(IEnumerator<TokenMax> tokenStream) {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }
        public string currentLexeme{
            get { return tokenStream.Current.Lexeme; }
        }
        public Token CurrentToken {
            get { return tokenStream.Current.Category; }
        }
        public Token Expect(Token category) {
            if (CurrentToken == category) {
                Token current = tokenStream.Current.Category;
                tokenStream.MoveNext();
                return current;
            } else {
                throw new SyntaxError();                
            }
        }
        public Node Inicio(){
            var p = new Programa(){Exp()};
            Expect(Token.EOF);
            return p;
        }
        public Node Exp() {  
            if(CurrentToken == Token.SYMBOL){
                return Symbol();
            }
            else if(CurrentToken == Token.SUCC){
                return Succ();
            }
            else if(CurrentToken == Token.PRED){
                return Pred();
            }
            //se considera que es un max
            else{
                return Max();
            }
                       
        }
        public Node Symbol(){
            
            var p = new Symbol();
            p.modifyLexeme(currentLexeme);
            Expect(Token.SYMBOL);
            return p;
        }
        public Node Succ(){
            Expect(Token.SUCC);
            Expect(Token.COROPEN);
            var p = new Succ() { Exp() };
            Expect(Token.CORCLOSE);
            return p;
        }
        public Node Pred(){
            Expect(Token.PRED);
            Expect(Token.COROPEN);
            var p = new Pred() { Exp() };
            Expect(Token.CORCLOSE);
            return p;
        }
        public Node Max(){
            Expect(Token.MAX);
            Expect(Token.COROPEN);
            var p = new Max() { ExprLst() };
            Expect(Token.CORCLOSE);
            return p;
        }
        public Node ExprLst(){
            var p = new ExprLst(){Exp()};
            while(CurrentToken == Token.SEMICOLON){
                Expect(Token.SEMICOLON);
                p.Add(Exp());
            }
            return p;
        }
        
    }
    
    //---------------------------------------------------------------
    class CILGenerator {
        public string Visit(Programa node) {
            return ".assembly 'lunkwill' {}\n\n"
                + ".assembly extern 'lunkwill' {}\n\n"
                + ".class public 'LegendaryExam' extends ['mscorlib']'System'.'Object' {\n"
                + "\t.method public static void 'start'() {\n"
                + "\t\t.entrypoint\n"
                + Visit((dynamic) node[0])
                + "\t\tcall void ['mscorlib']'System'.'Console'::'WriteLine'(string)\n"
                + "\t\tret\n"
                + "\t}\n"
                + "}\n";
        }
        
        public string Visit(Symbol node){
            return "ldstr\"" + node.getMyLexeme() + "\"\n";
        }
        public string Visit(Succ node){
            return Visit((dynamic)node[0]) +
            "call string class ['lunklib']'LibLunk'.'Utils'::'Successor'()\n" ;
        }
        public string Visit(Pred node){
            return Visit((dynamic)node[0]) +
            "call string class ['lunklib']'LibLunk'.'Utils'::'Predecessor'()\n";
        }
        public string Visit(Max node){
            string result = "";
            result += Visit((dynamic) node[0]);
            result +=  "call string class ['lunklib']'LibLunk'.'Utils'::'Maximum'(string,string)\n";
            return result;
        }
        public string Visit(ExprLst node){
            var sb = new StringBuilder();
            foreach (var n in node) {
                //Console.WriteLine(n.AnchorToken.Lexeme+" : "+n.GetType().Name);
                sb.Append(Visit((dynamic) n));
                
            }
            return sb.ToString();
        }
        
    }
        
        /*
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
        }
        public string Visit(Tijeras node) {
            return "\t\tldstr \"tijeras\"\n";
        }*/
    
    
    //---------------------------------------------------------------
    class Driver {
        public static void Main(string[] args) {
            try {
                /*foreach (var tok in new Scanner(args[0]).Start()) {
                    Console.WriteLine(String.Format("{0}\n", 
                                                     tok)
                    );
                }*/
                //ToStringTree()
                var p = new Parser(new Scanner(args[0]).Start().GetEnumerator()); 
                
                var ast = p.Inicio();
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