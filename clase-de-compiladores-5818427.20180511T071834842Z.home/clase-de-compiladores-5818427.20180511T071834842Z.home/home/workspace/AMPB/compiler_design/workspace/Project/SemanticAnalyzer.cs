/*
  Buttercup compiler - Semantic analyzer.
  Copyright (C) 2013 Ariel Ortiz, ITESM CEM
  
  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.
  
  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  
  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;

namespace INT64 {

    class SemanticAnalyzer {
        
        
        public List<string> global_vars;
        public Dictionary<string,List<string>> localvars;
        public string currentFunc;
        public int number_of_loops;
        //-----------------------------------------------------------
        //-----------------------------------------------------------
        public SymbolTable funcs; //{
        //    get;
        //    private set;
        //}
        
        //-----------------------------------------------------------
        public SemanticAnalyzer() {
            funcs = new SymbolTable();
            funcs["Printi"] = 1;
            funcs["Printc"] = 1;
            funcs["Prints"] = 1;
            funcs["Println"] = 0;
            funcs["Readi"] = 0;
            funcs["Reads"] = 0;
            funcs["New"] = 1;
            funcs["Size"] = 1;
            funcs["Add"] = 2;
            funcs["Get"] = 2;
            funcs["Set"] = 3;
            localvars = new Dictionary<string,List<string>>();
            currentFunc = "";
            global_vars = new List<string>();
            number_of_loops =0;
        }
        
        public void printVars(){
            foreach(string key in this.localvars.Keys){
                ////Console.WriteLine(key);
                List<string> value = this.localvars[key];
                foreach (var val in value) {
                    ////Console.WriteLine("        "+val);
                }
            }
        }
        
        void VisitChildren(Node node, int modo) {
            foreach (var n in node) {
                Visit((dynamic) n, modo);
            }
        }

        //-----------------------------------------------------------
        public void Visit(Program_ node) {
            Visit((dynamic) node[0],1);
            foreach (var entry in this.funcs) {
                    ////Console.WriteLine(entry);                        
            }
            if(!funcs.Contains("main")){
                 throw new SemanticError("no main function found" ,new Token("Program", TokenCategory.BREAK, 0,0));
            }
            ////Console.WriteLine(String.Join(", ",global_vars.ToArray()));
            //printVars();
            Visit((dynamic) node[0],3);
            printVars();
        }
        
    //def_list
        public void Visit(Def_list_ node, int modo) {
             VisitChildren(node,modo);
            }
    // modo sirve para saber el scope en el que se llama esta funcion
    // 1 para ser llamada con variables globales
    // 2 para argumentos de una funcion
        public void Visit(Var_ node, int modo){
             Visit((dynamic) node[0],modo);
        }
    
        public int Visit(Id_list_ node, int modo){
             int n1 = 0;
             foreach (var n in node) {
                if(modo == 1 || modo == 4){
                    Visit((dynamic) n, modo);
                }
                else if (modo == 2){
                    Visit((dynamic) n, modo);
                    n1 = n1+1;
                }
            }
            return n1;
        }
    
        public String Visit(Id_ node,int modo){
            if (modo == 1){
                if (!global_vars.Contains(node.AnchorToken.lexeme)){
                global_vars.Add(node.AnchorToken.lexeme);
                }
                else{
                     throw new SemanticError("repeated variable "+ "in row "+node.AnchorToken.row,node.AnchorToken);
                }
                VisitChildren(node,modo);
                return node.AnchorToken.lexeme;
    
            }
            else if (modo == 2 || modo == 4){
                //Console.WriteLine(node.AnchorToken.lexeme+"    "+modo);
                if(!localvars[this.currentFunc].Contains(node.AnchorToken.lexeme)){
                    localvars[this.currentFunc].Add(node.AnchorToken.lexeme);
                }
                else{
                    //Console.WriteLine("aquihola");
                    //var result = String.Join(", ", localvars[this.currentFunc].ToArray());
                    //Console.WriteLine(result);
                    if(modo == 2){
                        throw new SemanticError("repeated variable "+node.AnchorToken.lexeme,node.AnchorToken);
                    }
                }
                VisitChildren(node,modo);
                return node.AnchorToken.lexeme;
            }
            else if (modo == 5){
                if(!global_vars.Contains(node.AnchorToken.lexeme) && !localvars[currentFunc].Contains(node.AnchorToken.lexeme)){
                    if(!funcs.Contains(node.AnchorToken.lexeme)){
                         throw new SemanticError("variable not found  "+ node.AnchorToken.lexeme,node.AnchorToken);
                    }
                    else if(funcs.Contains(node.AnchorToken.lexeme)){
                        int n1 = 0;
                        ////Console.WriteLine(node.AnchorToken.lexeme);
                        foreach (var n in node[0]) {
                            n1 = n1+1;
                            ////Console.WriteLine(node.AnchorToken.lexeme);
                        }
                        ////Console.WriteLine(n1 + "  " +node.AnchorToken.lexeme);
                        if (funcs[node.AnchorToken.lexeme] != n1){
                             throw new SemanticError("wrong number of arguments" ,node.AnchorToken);
                        }
                        //VisitChildren(node,modo);
                        
                    }
                    else{
                        throw new SemanticError("function not found" ,node.AnchorToken);
                    }
                    
                }
            }
            try{
            VisitChildren(node,modo);
            }
            catch (Exception g){
                        //this.funcs[node.AnchorToken.lexeme] =  n;  
                    
                    if (g is Microsoft.CSharp.RuntimeBinder.RuntimeBinderException || g is System.ArgumentOutOfRangeException)
                        {
                            //WebId = Guid.Empty;
                            return  node.AnchorToken.lexeme;
                        }

                    throw;
                    
                }
            //Console.WriteLine("aqui");
            return node.AnchorToken.lexeme;
        }
    
        public void Visit(Fun_ node, int modo){
            if (modo == 1 || modo == 2){
                int n = 0;
                try{
                    currentFunc = node.AnchorToken.lexeme;
                    if(!funcs.Contains(node.AnchorToken.lexeme)){
                        this.localvars[node.AnchorToken.lexeme] = new List<String>();
                        n = Visit((dynamic) node[0],2);
                        this.funcs[node.AnchorToken.lexeme] =  n; 
                    } 
                    else{
                        throw new SemanticError("repeated function "+ "in row "+node.AnchorToken.row,node.AnchorToken);
                    }
                }
                catch(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException e){
                    this.funcs[node.AnchorToken.lexeme] =  n;  
                }
            }
            else if (modo == 3){
                currentFunc = node.AnchorToken.lexeme;
                try{
                    if (this.funcs[currentFunc] != 0){
                        Visit((dynamic) node[1],4);
                        Visit((dynamic) node[2],5);
                    }
                    else{
                        Visit((dynamic) node[0],4);
                        Visit((dynamic) node[1],5);
                    }
                }
                catch (Exception g){
                        //this.funcs[node.AnchorToken.lexeme] =  n;  
                    
                    if (g is Microsoft.CSharp.RuntimeBinder.RuntimeBinderException || g is System.ArgumentOutOfRangeException)
                        {
                            //WebId = Guid.Empty;
                            return;
                        }

                    throw;
                    
                }
            }
            else if(modo == 5){
                try{
                    if(funcs.Contains(node.AnchorToken.lexeme)){
                        int n1 = 0;
                        ////Console.WriteLine(node.AnchorToken.lexeme);
                        foreach (var n in node[0]) {
                            n1 = n1+1;
                            ////Console.WriteLine(node.AnchorToken.lexeme);
                        }
                        ////Console.WriteLine(n1 + "  " +node.AnchorToken.lexeme);
                        if (funcs[node.AnchorToken.lexeme] != n1){
                             throw new SemanticError("wrong number of arguments" ,node.AnchorToken);
                        }
                        
                    }
                    else{
                        throw new SemanticError("function not found" ,node.AnchorToken);
                    }
                }
                catch (Exception g){
                        //this.funcs[node.AnchorToken.lexeme] =  n;  
                    
                    if (g is Microsoft.CSharp.RuntimeBinder.RuntimeBinderException || g is System.ArgumentOutOfRangeException)
                        {
                            //WebId = Guid.Empty;
                            return;
                        }

                    throw;
                    
                }
        
            }
        }
        
        public void Visit(Var_def_list_ node, int modo){
             VisitChildren(node,modo);
        }
        
        public void Visit(Stmt_list_ node, int modo){
             VisitChildren(node,modo);
             //return 0;
        }
        
        public void Visit(If_ node, int modo){
             VisitChildren(node,modo);
        }
    
        public void Visit(Switch_ node, int modo){
             VisitChildren(node,modo);
        }
        
        public void Visit(While_ node, int modo){
             number_of_loops = number_of_loops + 1;
             VisitChildren(node,modo);
             number_of_loops = number_of_loops - 1;
        }
        
        public void Visit(Do_ node, int modo){
             number_of_loops = number_of_loops + 1;
             VisitChildren(node,modo);
             number_of_loops = number_of_loops - 1;
        }
        
        
         public void Visit(For_ node, int modo){
             number_of_loops = number_of_loops + 1;
             VisitChildren(node,modo);
             number_of_loops = number_of_loops - 1;
        }
        
        public void Visit(Break_ node, int modo){
            if(number_of_loops > 0){
                VisitChildren(node,modo);
            }
            else{
                 throw new SemanticError("break used out of loop" ,node.AnchorToken);
            }
        }
        
         public void Visit(Continue_ node, int modo){
              if(number_of_loops > 0){
                VisitChildren(node,modo);
            }
            else{
                 throw new SemanticError("break used out of loop" ,node.AnchorToken);
            }
        }
        
         public void Visit(Return_ node, int modo){
             VisitChildren(node,modo);
        }
        
        public void Visit(Empty_ node, int modo){
             VisitChildren(node,modo);
        }
        public void Visit(Expr_list_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Expr_ node, int modo){
             VisitChildren(node,modo);
        }
        
        public void Visit(Else_ node, int modo){
             VisitChildren(node,modo);
        }
        
        public string Visit(False_ node, int modo){
             VisitChildren(node,modo);
             return node.AnchorToken.lexeme;
        }
        
        public string Visit(True_ node, int modo){
             VisitChildren(node,modo);
             return node.AnchorToken.lexeme;
        }
        
        public string Visit(Character_ node, int modo){
             string s = node.AnchorToken.lexeme;
             int n = 0;
             string str = s;
             //Console.WriteLine(s);
             //Console.WriteLine("'\\\"'");
             if (s == "'\\n'"){
                 n = 10;
             }
             else if (s == "'\\r'"){
                 n = 13;
             }
             else if (s == "'\\t'"){
                 n = 9;
             }
             else if (s == "'\\'"){
                 n = 92;
             }
             else if (s == "'\\''"){
                 n = 10;
             }
             else if (s =="'\\\"'"){
                 n = 34;
             }
             else if(s.Length == 10){
                 //Console.WriteLine(str+ "aqui");
                 s = str.Substring(3,6);
                 //str = s.Substring(1,9);
                 //Console.WriteLine(s+ " aqui");
                 n = int.Parse(s, System.Globalization.NumberStyles.HexNumber);;
             }
             else{
                 n = GetUnicodeCodePoints(s);
                 //Console.WriteLine(n+" aqui");
             }
             VisitChildren(node,modo);
             Console.WriteLine(n);
             //Console.WriteLine(node.AnchorToken.row);
             return n+"";
        }
        
         public string Visit(Integer_ node, int modo){
             VisitChildren(node,modo);
             return node.AnchorToken.lexeme;
        }
        
        public void Visit(Lit_list_ node, int modo){
             VisitChildren(node,modo);
        }
        
        public void Visit(Default_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Interrogation_ node, int modo){
             VisitChildren(node,modo);
        }
        
        public void Visit(Colon_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Or_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(And_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Comp_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Rel_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Bit_and_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Bit_or_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Bit_shift_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Add_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Mul_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Pow_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Unary_ node, int modo){
             //Console.WriteLine("en unary  "+modo);
             VisitChildren(node,modo);
        }
        
        public string Visit(String_ node, int modo){
             VisitChildren(node,modo);
             return node.AnchorToken.lexeme;
        }
        
         public void Visit(Openp_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Exclamation_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Array_ node, int modo){
             VisitChildren(node,modo);
        }
         public void Visit(Case_ node, int modo){
             Console.WriteLine("aqui");
             List<string> cases = new List<string>();
             var i = 0;
             foreach (var m in node) {
                ////Console.WriteLine(i);
                ////Console.WriteLine("aqui");
                    try{
                        string j = Visit((dynamic) m, modo);
                        //Console.WriteLine(j);
                        if(!cases.Contains(j)){
                            cases.Add(j);
                        }
                        else{
                            throw new SemanticError("repeated case in case list" ,node.AnchorToken);
                        }
                    }
                   catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException h){
                   }
                        //this.funcs[node.AnchorToken.lexeme] =  n;  
                
                i = i + 1;
            }
            foreach (string cas in cases)
            {
                Console.WriteLine(cas);
            }
        }
        
         public void Visit(Expr_or_ node, int modo){
             VisitChildren(node,modo);
        }
        
         public void Visit(Expr_cond_ node, int modo){
             VisitChildren(node,modo);
        }
        
        
        
        public  int GetUnicodeCodePoints(string s){
            int unicodeCodePoint = 0;
            for (int i = 0; i < s.Length; i++){
                unicodeCodePoint = char.ConvertToUtf32(s, i);
                if (unicodeCodePoint > 0xffff){
                    i++;
                }
            }
            return unicodeCodePoint;
        }
        
    }
}
