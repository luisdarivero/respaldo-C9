

using System;
using System.Collections.Generic;

namespace DeepLingo {

    class SemanticAnalyzer {
        
        public bool isSemanticCorrect(){
            if(!Functions.Contains("main")){
                throw new SemanticError(
                            "Not main function in code ");
            }
            else if(Functions["main"].Item2 != 0){
                throw new SemanticError(
                            "Main function must have 0 args");
            }
            return true;
        }

        //-----------------------------------------------------------
        public SymbolTable GlobalSymbols {
            get;
            private set;
        }
        
        public FunctionTable Functions {
            get;
            private set;
        }
        
        public Tuple<string,int,LocalFunctionTable> newFunc(
                    string type, int arity, LocalFunctionTable tableV){
            return new Tuple<string,int,LocalFunctionTable>(
                type, arity, tableV);
        }
        
        public LocalFunctionTable newlocalT(){
            return new LocalFunctionTable();;
        }
        //-----------------------------------------------------------
        public void addPredefinedFunctions(){
            Functions["printi"] = newFunc("P",1,null);
            Functions["printc"] = newFunc("P",1,null);
            Functions["prints"] = newFunc("P",1,null);
            Functions["println"] = newFunc("P",0,null);
            Functions["readi"] = newFunc("P",0,null);
            Functions["reads"] = newFunc("P",0,null);
            Functions["new"] = newFunc("P",1,null);
            Functions["size"] = newFunc("P",1,null);
            Functions["add"] = newFunc("P",2,null);
            Functions["get"] = newFunc("P",2,null);
            Functions["set"] = newFunc("P",3,null);
        }
        
        public SemanticAnalyzer() {
            GlobalSymbols = new SymbolTable();
            Functions = new FunctionTable();
            addPredefinedFunctions();
            /*
            GlobalSymbols["uno"] = 0;
            GlobalSymbols["dos"] = 2;
            Console.WriteLine(GlobalSymbols.Contains("uno"));
            Console.WriteLine(GlobalSymbols.Contains("cero"));
            LocalFunctionVariables = new LocalFunctionTable();
            LocalFunctionVariables["1"] = new Tuple<string,int>("P",1);
            Functions = new FunctionTable();
            Functions["hola"] = new Tuple<string, int, LocalFunctionTable>(
                    "P",1, null);
            Functions["adios"] = newFunc("U",2,newlocalT());
            Functions.getLocalFT("adios").addVar("a","L",-1);
            Functions.getLocalFT("adios").addVar("b","P",0);
            
            Console.WriteLine(Functions.Contains("hola"));
            Console.WriteLine(Functions.Contains("holacara"));
            Console.WriteLine(Functions.getLocalFT("adios").Contains("b"));
            Console.WriteLine(Functions.getLocalFT("adios").Contains("c"));*/
        }
        
        //-----------------------------------------------------------
        private string currentFunc = "";//bandera que indica 
        //a que función se está haciendo referencia actualmente
        private int loopCount = 0;
        private int visitCount = 0;

        //-----------------------------------------------------------
        
        public void Visit(Program node) {
            VisitChildren(node);
            visitCount++;
            return;
        }
        //------------------------------------------------------------
        public void Visit(Var node){
            Visit((dynamic) node[0]);
            return;
        }
        
        public void Visit(VarList node){
            foreach (var n in node) {
                //Visit((dynamic) n);
                var variableName = n.AnchorToken.Lexeme;
            
                if(currentFunc == ""){
                    if(visitCount > 0){
                        return;
                    }
                    if (GlobalSymbols.Contains(variableName)) {
                        throw new SemanticError(
                            "Duplicated variable: " + variableName,
                            n.AnchorToken);
                    }
                    else{
                        GlobalSymbols[variableName] = 0;
                    }
                
                }else{
                    if(Functions.getLocalFT(currentFunc).Contains(variableName)){
                        throw new SemanticError(
                            "Duplicated variable: " + variableName,
                            n.AnchorToken);
                    }
                    else{
                        Functions.getLocalFT(currentFunc).addVar(variableName,"L",-1);
                    }
                }
            }
            
            return;
        }
        //----------------------------------------------------------
        
        public void Visit(StatementList node){
            VisitChildren(node);
        }
        
        public void Visit(Assignment node){
            var variableName = node.AnchorToken.Lexeme;
            if(Functions.getLocalFT(currentFunc).Contains(variableName) ||
                GlobalSymbols.Contains(variableName)){
                VisitChildren(node);
            }
            else{
                throw new SemanticError(
                            "Unknown Variable: " + variableName,
                            node.AnchorToken);
            }
        }
        
        public void Visit(FunctionCall node){
            var functionName = node.AnchorToken.Lexeme;
            if(!Functions.Contains(functionName)){
                throw new SemanticError(
                            "Unknown Function: " + functionName,
                            node.AnchorToken);
            }
            int argsCount = 0;
            if(node.Lenght() > 0){
                argsCount = node[0].Lenght();
            }
            var realArgsCount = Functions[functionName].Item2;
            if(argsCount != realArgsCount){
                throw new SemanticError(
                            "Wrong number of arguments calling function: " + functionName,
                            node.AnchorToken);
                
            }
            VisitChildren(node);
        }
        
        public void Visit(Identifier node){
            var variableName = node.AnchorToken.Lexeme;
            if(!Functions.getLocalFT(currentFunc).Contains(variableName) &&
                !GlobalSymbols.Contains(variableName)){
                    throw new SemanticError(
                            "Unknown Variable: " + variableName,
                            node.AnchorToken);
            }
            return;
        }
        
        public void Visit(IntLiteral node){
            var intStr = node.AnchorToken.Lexeme;

            try {
                Convert.ToInt32(intStr);

            } catch (OverflowException) {
                throw new SemanticError(
                    "Integer literal too large: " + intStr, 
                    node.AnchorToken);
            }
            return;
        }
        //-----------------------------------------------------------
        public void Visit(FunctionDefinition node){
            var functionName = node.AnchorToken.Lexeme;
            if(Functions.Contains(functionName) && visitCount == 0){
                throw new SemanticError(
                            "Duplicated function: " + functionName,
                            node.AnchorToken);
            }
            currentFunc = functionName;
            int startCount = 0;
            if(visitCount == 0){
                if(node.Lenght()>0){
                    if(node[0].GetType().Name == "VarList"){
                        //startCount++;
                        Functions[functionName] = newFunc("U",node[0].Lenght(),newlocalT());
                        int contador = 0;
                        foreach (var n in node[0]) {
                            var variableName = n.AnchorToken.Lexeme;
                            if(Functions.getLocalFT(functionName).Contains(variableName)){
                                throw new SemanticError(
                                "Duplicated variable: " + variableName,
                                n.AnchorToken);
                            }
                            Functions.getLocalFT(functionName).addVar(variableName,"P",contador);
                            contador++;
                        }
                    }else{
                        Functions[functionName] = newFunc("U",0,newlocalT());
                    }
                    
                }else{
                    Functions[functionName] = newFunc("U",0,newlocalT());
                }
            }else{
                if(node.Lenght()>0){
                    if(node[0].GetType().Name == "VarList"){
                        startCount++;
                    }
                }
                for(int i = startCount; i< node.Lenght(); i++){
                    Visit((dynamic) node[i]);
                }
            }
        }
        
        public void Visit(Loop node){
            loopCount++;
            VisitChildren(node);
            loopCount--;
        }
        
        public void Visit(Break node){
            if(loopCount < 1){
                throw new SemanticError(
                            "Break without Loop declared: ",
                            node.AnchorToken);
            }
        }
        
        public void Visit(Plus node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        public void Visit(Unary node){
            Visit((dynamic) node[0]);
        }
        public void Visit(Increment node){
            var variableName = node.AnchorToken.Lexeme;
            if(!Functions.getLocalFT(currentFunc).Contains(variableName) &&
                !GlobalSymbols.Contains(variableName)){
                    throw new SemanticError(
                            "Unknown Variable: " + variableName,
                            node.AnchorToken);
            }
            return;
        }
        public void Visit(Decrement node){
            var variableName = node.AnchorToken.Lexeme;
            if(!Functions.getLocalFT(currentFunc).Contains(variableName) &&
                !GlobalSymbols.Contains(variableName)){
                    throw new SemanticError(
                            "Unknown Variable: " + variableName,
                            node.AnchorToken);
            }
            return;
        }
        
        public void Visit(Return node){
            Visit((dynamic) node[0]);
        }
        
        public void Visit(CharLiteral node){
            return;
        }
        
        public void Visit(Array node){
            if(node.Lenght() == 0){
                return;
            }
            VisitChildren(node);
        }
        
        public void Visit(ExpressionList node){
            VisitChildren(node);
        }
        
        public void Visit(Rem node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        public void Visit(Minus node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        public void Visit(Mul node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        public void Visit(Div node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        public void Visit(StringLiteral node){
            return;
        }
        
        public void Visit(EqualTo node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        public void Visit(NotEqualTo node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        public void Visit(Less node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        public void Visit(LessThanOrEqualTo node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        public void Visit(GreaterThan node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        public void Visit(GreaterThanOrEqualTo node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        public void Visit(And node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        public void Visit(Or node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        public void Visit(If node){
            VisitChildren(node);
        }
        public void Visit(ElseIfList node){
            VisitChildren(node);
        }
        public void Visit(ElseIf node){
            VisitChildren(node);
        }
        public void Visit(Else node){
            VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }
        
    }
}
