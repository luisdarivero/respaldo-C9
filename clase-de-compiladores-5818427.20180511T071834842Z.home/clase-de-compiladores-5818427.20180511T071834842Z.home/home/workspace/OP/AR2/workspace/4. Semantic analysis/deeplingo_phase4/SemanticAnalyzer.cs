// This programme is based on the Buttercup's compiler code, provided by Ariel Ortiz.

/* 
    DeepLingo compiler - Semantic analyzer.
    Date: 12-March-2018
    Authors:
          A01169073 Aldo Reyna
          A01375051 Marina Torres
    File name: SemanticAnalyzer.cs
*/


using System;
using System.Text;
using System.Collections.Generic;

namespace DeepLingo {

    class SemanticAnalyzer {
        
        public HashSet<string> GlobalVars{
            get;
            set;
        }
        string funName="";
        int loopCount = 0;
        
        //-----------------------------------------------------------
        public SymbolTableGlobal Table {
            get;
            private set;
        }

        //-----------------------------------------------------------
        public SemanticAnalyzer() {
            Table = new SymbolTableGlobal();
            Table["printi"] = new Values("p", 1, null);
            Table["printc"] = new Values("p", 1, null);
            Table["prints"] = new Values("p", 1, null);
            Table["println"] = new Values("p", 0, null);
            Table["readi"] = new Values("p", 0, null);
            Table["reads"] = new Values("p", 0, null);
            Table["new"] = new Values("p", 1, null);
            Table["size"] = new Values("p", 1, null);
            Table["add"] = new Values("p", 2, null);
            Table["get"] = new Values("p", 2, null);
            Table["set"] = new Values("p", 3, null);
            GlobalVars = new HashSet<string>();
        }
        
        //-----------------------------------------------------------
        public void Visit(Programme node) {
            //Primera Pasada
            foreach (var n in node) {
                if (n is FunctionDef){
                    var funName = n.AnchorToken.Lexeme;
                    if (Table.Contains(funName)) {
                        throw new SemanticError(
                            "Duplicated function: " + funName,
                            n.AnchorToken);
                    }
                    else {
                        int num = n[0].CountChildren();
                        Table[funName] = new Values ("u",num,new SymbolTableLocal());
                    }
                }else {
                    var size = n.CountChildren();
                    for (int i = 0; i < size; i++) {
                        var varName = n[i].AnchorToken.Lexeme;
                        if (GlobalVars.Contains(varName)) {
                            throw new SemanticError(
                                "Duplicated variable: " + varName,
                                n[i].AnchorToken);
                        }
                        else { 
                            GlobalVars.Add(varName);
                        }
                    }
                }
            }
            
            // Revisar si no existe el método main
            if (!Table.Contains("main")){
                throw new SemanticError(
                    "Te pasaste de lanza, no hay main. \n Atte: Junco");
            }
            
            
            //Segunda Pasada
            foreach (var n in node) {
                if (n is FunctionDef){
                    Visit((dynamic) n);
                }
            }
        }
        
        //-----------------------------------------------------------
        public void Visit(FunctionDef node) {
            funName = node.AnchorToken.Lexeme;
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        
        public void Visit(ParamList node){
            var size = node.CountChildren();
            for (int i=0; i<size; i++){
                var varName = node[i].AnchorToken.Lexeme;
                Table[funName].table[varName] = new Locals("param", i);
            }
        }
        
        //-----------------------------------------------------------
        public void Visit(IdList node) {
            var size = node.CountChildren();
            for (int i=0; i<size; i++){
                var varName = node[i].AnchorToken.Lexeme;
                Table[funName].table[varName] = new Locals("local", -1);
            }
            
        }
        
        
        //-----------------------------------------------------------
        public void Visit(FunctionCall node) {
            var funName = node.AnchorToken.Lexeme;
            if (!Table.Contains(funName)) {
                throw new SemanticError(
                    "variable:Missing function: " + funName,
                    node.AnchorToken);
            }else{
                var numParams = node[0].CountChildren();
                if (numParams!=Table[funName].arity){
                    throw new SemanticError(
                    "Parameters: Incorrect number of parameters recived in " + funName,
                    node.AnchorToken);
                }else{
                    Visit((dynamic)node[0]);
                }
            }
        }
        
        //-----------------------------------------------------------
        public void Visit(ExprList node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(Assignment node) {
            var variableName = node.AnchorToken.Lexeme;
            if (Table[funName].table.Contains(variableName) || GlobalVars.Contains(variableName)) {
                VisitChildren(node);
            } else {
                throw new SemanticError(
                    "Undeclared variable: " + variableName,
                    node.AnchorToken);
            }
        }
        
        //-----------------------------------------------------------
        public void Visit(Inc node) {
            var variableName = node.AnchorToken.Lexeme;
            if (Table[funName].table.Contains(variableName) || GlobalVars.Contains(variableName)) {
                VisitChildren(node);
            } else {
                throw new SemanticError(
                    "Undeclared variable: " + variableName,
                    node.AnchorToken);
            }
        }
        
        //-----------------------------------------------------------
        public void Visit(Dec node) {
            var variableName = node.AnchorToken.Lexeme;
            if (Table[funName].table.Contains(variableName) || GlobalVars.Contains(variableName)) {
                VisitChildren(node);
            } else {
                throw new SemanticError(
                    "Undeclared variable: " + variableName,
                    node.AnchorToken);
            }
            
        }
        
        //-----------------------------------------------------------
        public void Visit(If node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(ListStatements node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(ListElseIf node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(ElseIf node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(Loop node) {
            loopCount++;
            VisitChildren(node);
            loopCount--;
        }
        
        //-----------------------------------------------------------
        public void Visit(Break node) {
            if (loopCount == 0){
                throw new SemanticError(
                    "Break is outside the body of loop statement.", node.AnchorToken);
            }
            else{
                VisitChildren(node);
            }
        }
        
        //-----------------------------------------------------------
        public void Visit(Return node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(And node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(Or node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(Identifier node) {
            var variableName = node.AnchorToken.Lexeme;
            if (!(Table[funName].table.Contains(variableName)) && !(GlobalVars.Contains(variableName))) {
                throw new SemanticError(
                    "Undeclared variable: " + variableName,
                    node.AnchorToken);
            }
            
        }
        
        //-----------------------------------------------------------
        public void Visit(IntLiteral node) {
            var number = node.AnchorToken.Lexeme;
            try {
                Convert.ToInt32(number);
            }
            catch (OverflowException){
                throw new SemanticError("Value was either too large or too small for an Int32.");
            }
        }
        
        
        //-----------------------------------------------------------
        public void Visit(CharLiteral node) {
            
        }
        
        //-----------------------------------------------------------
        public void Visit(StringLiteral node) {
            
        }
        
        //-----------------------------------------------------------
        public void Visit(Less node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(LessEq node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(Great node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(GreatEq node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(Equals node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(NotEq node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(Pos node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(Neg node) {
            VisitChildren(node);
        }
        
        
        
        
        //-----------------------------------------------------------
        public void Visit(Not node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(Add node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(Subs node) {
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public void Visit(Mult node) {
            VisitChildren(node);
        }
        
        
        public void Visit(Div node) {
            VisitChildren(node);
        }
        
        public void Visit(Rem node) {
            VisitChildren(node);
        }
        
        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }
        
        //-----------------------------------------------------------
        public string FancyPrint() {
            var sb = new StringBuilder();
            sb.Append("Symbol Table: Global Variables\n");
            sb.Append("==============================\n");
            foreach (var entry in GlobalVars) {
                sb.Append(String.Format("{0} \n", 
                                        entry
                                        ));
            }
            sb.Append("==============================\n\n\n");
            sb.Append("Symbol Table: Global Function\n");
            sb.Append("==============================\n");
            foreach (var entry in Table) {
                sb.Append(String.Format("{0} \n", 
                                        entry
                                        ));
            }
            sb.Append("==============================\n\n\n");
            foreach (var entry in Table) {
                
                if (entry.Value.table!=null){
                    sb.Append("Symbol Table: "+entry.Key+"\n");
                    sb.Append("==============================\n");
                    sb.Append(String.Format("{0} \n",
                        entry.Value.table));
                    sb.Append("==============================\n\n\n");
                }
            }
            
            return sb.ToString();
        }
    }
}
