

//para compilar el achivo.il se tiene que usar:
//ilasm myTestFile.il
using System;
using System.Text;
using System.Collections.Generic;


namespace DeepLingo{
    class CILGenerator{
        
        SymbolTable table;

        int labelCounter = 0;
        
        string GenerateLabel() {
            return String.Format("${0:000000}", labelCounter++);
        } 
        
        public CILGenerator(SymbolTable table) {
            this.table = table;
        }
        
        public CILGenerator(){
            //Vacio
        }
        
        public string putS(string cad){
            return cad + "\n";
        }
        
        int identSpaces = 1;
        public string Indentar(){
            var ident = "";
            for(int i =0;i<identSpaces;i++){
                ident = ident + "\t";
            }
            return ident;
        }
        
        static readonly IDictionary<string, int> specials = new Dictionary<string, int>() {
                { "\\r", 13},
                { "\\t", 9},
                { "\\\"", 34},
                { "\\", 92},
                { "\\n", 10},
                { "\\'", 39},
        };
        int specialCode(string cadena){
            int codeSpecial = 0;
            try{
                codeSpecial = (int)Convert.ToInt64(cadena, 16);
            }
            catch(Exception e){
                Console.WriteLine("Error trying convert '"+cadena+"' to char: " + e);
            }
            return codeSpecial;
        }
        
        bool isGlobalVar = true;
        List<string> globalVar = new List<string>();
        List<string> globalFun = new List<string>();
        List<string> localVar = new List<string>();
        List<string> statementLst = new List<string>(); 
        List<string> libFunc = new List<string>();
        
        
        
        public void printLst(List<string> lista){
            string result = "";
            foreach (string item in lista) { result+=  item + ", ";};
            Console.WriteLine(result);
        }
        
        string endIfLabel = "";
        string oldExitLoop = "";
        string currentExitLoop = "";
        bool isAssigment = false;
        //Empiezan la visita de los nodos--------------------------------
        
        //check
        public string Visit(Program node) {
            libFunc.Add("printi");
            libFunc.Add("printc");
            libFunc.Add("prints");
            libFunc.Add("println");
            libFunc.Add("readi");
            libFunc.Add("reads");
            libFunc.Add("new");
            libFunc.Add("size");
            libFunc.Add("add");
            libFunc.Add("get");
            libFunc.Add("set");
            
            string programResult = "";
            programResult += "// Code generated by the deeplingo compiler.\n\n" 
                + ".assembly 'deeplingo' {}\n\n"
                + ".assembly extern 'deeplingolib' {}\n\n"
                + ".class public 'DeepLingoProgram' extends " 
                + "['mscorlib']'System'.'Object' {\n";
            programResult += VisitChildren(node);
            programResult += "}";
            /*Console.WriteLine("prueba listas-------------");
            printLst(globalVar);
            printLst(globalFun);
            printLst(localVar);
            printLst(statementLst);
            Console.WriteLine("termina------------------");*/
            return programResult;
        }
        
        //check
        public string Visit(Var node){
            return Visit((dynamic) node[0]);
        }
        
        //check
        public string Visit(VarList node){
            string result = "";
            //se declara como una variable global
            if(isGlobalVar){
                foreach (var n in node) {
                    result += putS(Indentar() + ".field  public static  int32 '" +
                            n.AnchorToken.Lexeme+"'");
                    globalVar.Add(n.AnchorToken.Lexeme);
                }
                return result;
            }
            //se declara como una variable local de una funcion
            foreach (var n in node) {
				result += putS(Indentar() + ".locals init (int32 '"+
				            n.AnchorToken.Lexeme+"')");
				localVar.Add(n.AnchorToken.Lexeme);
			}
			return result;
        }
        
        //check
        public string Visit(StatementList node){
            
            var temp = "";
			foreach(var n in node){
				if (n is FunctionCall && !libFunc.Contains(n.AnchorToken.Lexeme)){
					var temp2 = "";
					var result = "";
					if(n.Lenght()>0){
					    foreach(var x in n[0]){
						temp2 += "int32,";
						result += Visit((dynamic) x);
    					}
    					//Console.WriteLine("llego aqui" + n.AnchorToken.Lexeme);
    					temp += result
    					+ putS(Indentar() + "call int32 class 'DeepLingoProgram'::'" + n.AnchorToken.Lexeme + "'("+ temp2.Substring(0, temp2.Length-1)+")") 
    					+ putS(Indentar() + "pop");
					}
					else{
					    //Console.WriteLine("llego aqui" + n.AnchorToken.Lexeme);
					    temp += putS(Indentar() + 
					    "call int32 class 'DeepLingoProgram'::'" + 
            			n.AnchorToken.Lexeme + "'()\n" + putS(Indentar() + "pop"));
					}
					
				}
				else{
					temp += Visit((dynamic) n);	
				}
			}
			return temp;
        }
        
        //check
        public string Visit(Assignment node){
            isAssigment = true;
            string result = "";
            if(statementLst.Contains(node.AnchorToken.Lexeme)){
                result = VisitChildren(node) + putS(Indentar() + "starg.s "+ 
                        statementLst.IndexOf(node.AnchorToken.Lexeme));
                isAssigment = false;
                return result;
            }
            else if(localVar.Contains(node.AnchorToken.Lexeme)){
                result =  VisitChildren(node) + putS(Indentar() + 
                        "stloc '"+ node.AnchorToken.Lexeme+ "'");
                isAssigment = false;
                return result;
            }
            //entonces es una variable global
            result =  VisitChildren(node) + putS(Indentar() + 
                    "stsfld int32 'DeepLingoProgram'::'"+
                        node.AnchorToken.Lexeme+"'");
            isAssigment = false;
            return result;
        }
        
        //check
        public string Visit(FunctionCall node){
            
            switch(node.AnchorToken.Lexeme){
				case "printi":
				    
					return VisitChildren(node) 
					+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Printi'(int32)")
				    + putS(Indentar() + "pop");
				case "printc":
					return VisitChildren(node) 
					+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Printc'(int32)")
				    + putS(Indentar() + "pop");
				case "prints":
					return VisitChildren(node) 
					+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Prints'(int32)")
				    + putS(Indentar() + "pop");
				case "println":
					return VisitChildren(node) 
					+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Println'()")
				    + putS(Indentar() + "pop");
				case "readi":
					return VisitChildren(node) 
					+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Readi'()");
				case "reads":
					return VisitChildren(node) 
					+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Reads'()");
				case "new":
					return VisitChildren(node) 
					+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'New'(int32)");
				case "size":
					return VisitChildren(node) 
					+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Size'(int32)");
				case "add":
					return VisitChildren(node) 
					+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Add'(int32,int32)")
					+ putS(Indentar() + "pop");
				case "get":
					return VisitChildren(node) 
					+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Get'(int32,int32)");
				case "set":
					return VisitChildren(node) 
					+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Set'(int32,int32,int32)")
					+ putS(Indentar() + "pop");	
				case "pow":
					return VisitChildren(node) 
					+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Pow'(int32,int32)");	
				
			}
			//default -> es una funcion definida por el usuario
			var assi = "";
			if(false){
			    assi = putS(Indentar() + "pop");
			}
			isAssigment = false;
			var temp = "";
			var temp2 = "";
			var result = "";
			var t = "";
			//si la funcion tiene argumentos
			if(node.Lenght()>0){
			    foreach(var n in node[0]){
    				temp2 += "int32,";
    				result += Visit((dynamic) n);
			    }
			    temp += result
            			+ putS(Indentar() + "call int32 class 'DeepLingoProgram'::'" + 
            			node.AnchorToken.Lexeme + "'("+ 
            			temp2.Substring(0, temp2.Length-1)+")") + assi ;
    			return temp;
			}
			t = result
    			+ putS(Indentar() + "call int32 class 'DeepLingoProgram'::'" + 
    			node.AnchorToken.Lexeme + "'()") + assi;
    		return t;
			
        }
        //check
        public string Visit(Identifier node){
            string idName = node.AnchorToken.Lexeme;
            if(statementLst.Contains(idName)){
                return putS(Indentar() + "ldarg."+ statementLst.IndexOf(idName));
            }
            else if(localVar.Contains(idName)){
                return putS(Indentar() + "ldloc '"+ idName + "'");
            }
            //caso en el que sea una variable global
            return putS(Indentar() + "ldsfld int32 'DeepLingoProgram'::'"
                    + idName+"'");
        }
        
        //check
        public string Visit(IntLiteral node){
            var intValue = Convert.ToInt32(node.AnchorToken.Lexeme);
            if (intValue <= 8) {
                return putS(Indentar() + "ldc.i4." + intValue);
            } else if (intValue <= 127) {
                return putS(Indentar() + "ldc.i4.s " + intValue);
            }
            return putS(Indentar() + "ldc.i4 " + intValue);
        }
       
       //check
        public string Visit(FunctionDefinition node){
            isGlobalVar = false;
            var functionName = node.AnchorToken.Lexeme;
            globalFun.Add(functionName);
            localVar = new List<string>();
            statementLst = new List<string>(); 
            int nodesUsed = 0;
            var temp1 = " ";
			var temp2 = " ";
            //verifica el numero de nodos hijos
            if(node.Lenght()>0){
                if(node[0].GetType().Name == "VarList"){
                    foreach (var n in node[0]) {
                        temp1 += "int32,";
        			    temp2 += putS(Indentar() + ".locals init (int32 '"+
        			    n.AnchorToken.Lexeme+"')");
        			    statementLst.Add(n.AnchorToken.Lexeme);
                    }
                    nodesUsed++;
                }
            }
            
            
            string result = putS(Indentar() + ".method public static int32 '" +
                            functionName +"'(" +
                            temp1.Substring(0,temp1.Length-1) + ") {");
            identSpaces++;
            if(functionName == "main"){
				result += putS(Indentar() + ".entrypoint");
			}
			result += temp2;
			for(var i = nodesUsed; i< node.Lenght(); i++){
			    result += Visit((dynamic)node[i]);
			}
			
			result += putS(Indentar() + "ldc.i4.0") + putS(Indentar() + "ret");
			identSpaces--;
	        result += putS(Indentar() + "}") + putS("");
			
			return result;
        }
        
        public string Visit(Loop node){
            oldExitLoop = currentExitLoop;
			var startLoop = GenerateLabel();
			currentExitLoop = GenerateLabel();
			
			var result = putS(Indentar() + startLoop + ":") +
				VisitChildren(node) +
				putS(Indentar() + "br " + startLoop) +
				putS(Indentar() + "'"+currentExitLoop + "':");
			
			currentExitLoop = oldExitLoop;	
			return result;
        }
        
        public string Visit(Break node){
            return putS(Indentar() + "br " + currentExitLoop);  
        }
        
        //check
        public string Visit(Plus node){
            return VisitChildren(node) + putS(Indentar() + "add.ovf");
        }
        
        //check
        public string Visit(Unary node){
            //si es un signo not
            if(node.AnchorToken.Lexeme == "!"){
				var label = GenerateLabel();
				return 
					putS(Indentar() + "ldc.i4.0")+
					Visit((dynamic)node[0]) + 
					putS(Indentar() + "ldc.i4 42")+
					putS(Indentar() + "beq '"+label+"'")+
					putS(Indentar() + "pop")+
					putS(Indentar() + "ldc.i4 42")+ 
					putS(Indentar() + "'"+label+"':");
			}
			//si es un menos
			else if(node.AnchorToken.Lexeme == "-"){
				return 
					putS(Indentar() + "ldc.i4.0")+
					Visit((dynamic)node[0]) + 
				    putS(Indentar() + "sub");
			}
			//se asume que es un mas
			else{
				return Visit((dynamic)node[0]);
			}
			
        }
        
        //check
        public string Visit(Increment node){
            return 
			putS(Indentar() + "ldloc '"+node.AnchorToken.Lexeme+"'") + 
			putS(Indentar() + "ldc.i4.1") +
			putS(Indentar() + "add") +
			putS(Indentar() + "stloc '"+node.AnchorToken.Lexeme+"'");
        }
        
        //check
        public string Visit(Decrement node){
            return 
			putS(Indentar() + "ldloc '"+node.AnchorToken.Lexeme+"'") + 
			putS(Indentar() + "ldc.i4.1") +
			putS(Indentar() + "sub") +
			putS(Indentar() + "stloc '"+node.AnchorToken.Lexeme+"'");
        }
        
        //check
        public string Visit(Return node){
            return VisitChildren(node) + putS(Indentar() + "ret");
        }
        
        //check
        public string Visit(CharLiteral node){
            var s = node.AnchorToken.Lexeme;
			if(s.Length > 3){
				if(specials.ContainsKey(s.Substring(1,2))){
					return putS(Indentar() + "ldc.i4.s " + specials[s.Substring(1,2)])
					+ VisitChildren(node);	
				}
				else if(s.Substring(1,2) == @"\u"){
					var codePoint = specialCode(s.Substring(3,6));
                    if (codePoint <= 8) {
		                return putS(Indentar() + "ldc.i4." + codePoint)
						+ VisitChildren(node);	
		
		            } else if (codePoint <= 127) {
		                return putS(Indentar() + "ldc.i4.s " + codePoint)
						+ VisitChildren(node);	
		
		            } else {
		                return putS(Indentar() + "ldc.i4 " + codePoint)
						+ VisitChildren(node);	
		            } 
				}
			}
			return putS(Indentar() + "ldc.i4.s " + (int)node.AnchorToken.Lexeme[1])
			+ VisitChildren(node);
        }
        
        //check
        public string Visit(Array node){
            var temp = "";
			foreach(var n in node[0]){
				temp = temp
				+ putS(Indentar() + "dup")
				+ Visit((dynamic) n)
				+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Add'(int32,int32)")
				+ putS(Indentar() + "pop");
			}
			return 
			putS(Indentar() + "ldc.i4.0")
			+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'New'(int32)")
			+ temp;
        }
        
        //check
        public string Visit(ExpressionList node){
            return VisitChildren(node);
        }
        
        //check
        public string Visit(Rem node){
            return VisitChildren(node) + putS(Indentar() + "rem");
        }
        
        //check
        public string Visit(Minus node){
            return VisitChildren(node) + putS(Indentar() + "sub");
        }
        
        //check
        public string Visit(Mul node){
            return VisitChildren(node) + putS(Indentar() + "mul.ovf");
        }
        
        //check
        public string Visit(Div node){
            return VisitChildren(node) + putS(Indentar() + "div");
        }
        
        //check
        public string Visit(StringLiteral node){
            string val = node.AnchorToken.Lexeme;
			int size = val.Length;
			string temp = "";
			for(var i = 1;i<size-1;i++){
				var toadd = (int)val[i];
				if(i+1<size-1 && specials.ContainsKey(val.Substring(i,2))){
                    toadd = specials[val.Substring(i,2)];
					i += 1;
				}
				else if(i+7<size-1 && val.Substring(i,2) == @"\u"){
					toadd = specialCode(val.Substring(i+2,6));
					i += 7;
				}
				temp = temp
				+ putS(Indentar() + "dup")
				+ putS(Indentar() + "ldc.i4 "+ toadd)
				+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Add'(int32,int32)")
				+ putS(Indentar() + "pop");	
			}
			
			return putS(Indentar() + "ldc.i4.0")
			+ putS(Indentar() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'New'(int32)")
			+ temp;
        }
        
        //check
        public string Visit(EqualTo node){
            var label = GenerateLabel();
            return putS(Indentar() + "ldc.i4.0")
				+ VisitChildren(node)
				+ putS(Indentar() + "bne.un '"+label+"'")
				+ putS(Indentar() + "pop")
				+ putS(Indentar() + "ldc.i4 42")
				+ putS(Indentar() + label + ":");
        }
        
        //check
        public string Visit(NotEqualTo node){
            var label = GenerateLabel();
            return putS(Indentar() + "ldc.i4 42")
				+ VisitChildren(node)
				+ putS(Indentar() + "bne.un '"+label+"'")
				+ putS(Indentar() + "pop")
				+ putS(Indentar() + "ldc.i4.0")
				+ putS(Indentar() + label + ":");
        }
        
        //check
        public string Visit(Less node){
            var label = GenerateLabel();
            return putS(Indentar() + "ldc.i4.0")
				+ VisitChildren(node)
				+ putS(Indentar() + "bge '"+label+"'")
				+ putS(Indentar() + "pop")
				+ putS(Indentar() + "ldc.i4 42")
				+ putS(Indentar() + label + ":");
        }
        
        //check
        public string Visit(LessThanOrEqualTo node){
            var label = GenerateLabel();
            return putS(Indentar() + "ldc.i4 42")
				+ VisitChildren(node)
				+ putS(Indentar() + "ble '"+label+"'")
				+ putS(Indentar() + "pop")
				+ putS(Indentar() + "ldc.i4.0")
				+ putS(Indentar() + label + ":");
        }
        
        //check
        public string Visit(GreaterThan node){
            var label = GenerateLabel();
            return putS(Indentar() + "ldc.i4.0")
				+ VisitChildren(node)
				+ putS(Indentar() + "ble '"+label+"'")
				+ putS(Indentar() + "pop")
				+ putS(Indentar() + "ldc.i4 42")
				+ putS(Indentar() + label + ":");
        }
        
        //check
        public string Visit(GreaterThanOrEqualTo node){
            var label = GenerateLabel();
            return putS(Indentar() + "ldc.i4 42")
				+ VisitChildren(node)
				+ putS(Indentar() + "bge '"+label+"'")
				+ putS(Indentar() + "pop")
				+ putS(Indentar() + "ldc.i4.0")
				+ putS(Indentar() + label + ":");
        }
        
        //check
        public string Visit(And node){
            var result = "";
			var lbl = GenerateLabel();
			var lbl2 = GenerateLabel();
			
			foreach(var n in node){
				result += 
				Visit((dynamic) n)
				+ putS(Indentar() + "ldc.i4 42")
				+ putS(Indentar() + "bne.un '"+lbl+"'");
			}
			return result
			+ putS(Indentar() + "ldc.i4 42")
			+ putS(Indentar() + "br "+lbl2)
			+ putS(Indentar() + lbl + ":")
			+ putS(Indentar() + "ldc.i4.0 ")
			+ putS(Indentar() + lbl2 + ":");
        }
        
        //check
        public string Visit(Or node){
            var result = "";
			var truecondition = GenerateLabel();
			var endcondition = GenerateLabel();
			foreach(var n in node){
				result += 
				Visit((dynamic) n)
				+ putS(Indentar() + "ldc.i4.0")
				+ putS(Indentar() + "bne.un '"+truecondition+"'");
			}
			return result
			+ putS(Indentar() + "ldc.i4.0")
			+ putS(Indentar() + "br "+endcondition)
			+ putS(Indentar() + truecondition + ":")
			+ putS(Indentar() + "ldc.i4 42")
			+ putS(Indentar() + endcondition + ":");
        }
        
        public string Visit(If node){
            var elsesbody = GenerateLabel();
			var oldEndIfLaber = endIfLabel;
			endIfLabel = GenerateLabel();
			var result  = "";
			int numberOfNodesChildren = node.Lenght();
			int startCountNodes = 0;
			//para procesar los argumentos
			result += Visit(((dynamic) node[0]))
			+ putS(Indentar() + "ldc.i4 42")
			+ putS(Indentar() + "bne.un '"+elsesbody+"'");
			numberOfNodesChildren--;
			startCountNodes++;
			//en caso de que se tenga un statementList
			if(numberOfNodesChildren > 0 &&
			    node[startCountNodes].GetType().Name == "StatementList"){
			        //Console.WriteLine("entro a statementLst con: " + startCountNodes);
			        identSpaces ++;
			        result += Visit(((dynamic) node[startCountNodes]));
			        identSpaces--;
			        numberOfNodesChildren--;
			        
			        startCountNodes++;
			}
					
			result += putS(Indentar() + "br "+endIfLabel+"")
			+ putS(Indentar() + "'"+elsesbody+"':");
			//si existe un elseif
			if(numberOfNodesChildren > 0 &&
			    node[startCountNodes].GetType().Name == "ElseIfList"){
			        //Console.WriteLine("entro a elseif con: " + startCountNodes);
			        identSpaces ++;
			        result += Visit(((dynamic) node[startCountNodes]));
			        identSpaces--;
			        numberOfNodesChildren--;
			        
			        startCountNodes++;
			}
			
			if(numberOfNodesChildren > 0 &&
			    node[startCountNodes].GetType().Name == "Else"){
			        //Console.WriteLine("entro a else con: " + startCountNodes);
			        //Console.WriteLine("el tipo de nodo: " + node[startCountNodes].GetType().Name);
			        identSpaces ++;
			        result += Visit(((dynamic) node[startCountNodes]));
			        identSpaces--;
			        
			}		
			result += putS(Indentar() + "'"+endIfLabel+"':");
			endIfLabel = oldEndIfLaber;
			return  result;
        }
        
        //check
        public string Visit(ElseIfList node){
            return VisitChildren(node);
        }
        
        
        public string Visit(ElseIf node){
            identSpaces ++;
            var lbl = GenerateLabel();
			string result =  Visit(((dynamic) node[0]))
			+ putS(Indentar() + "ldc.i4 42")
			+ putS(Indentar() + "bne.un '"+lbl+"'")
			+ Visit(((dynamic) node[1]))
			+ putS(Indentar() + "'"+lbl + "':");
			identSpaces --;
			return result;
        }
        public string Visit(Else node){
            identSpaces ++;
            string result = "";
            result +=  Visit((dynamic)node[0]);
            identSpaces --;
            return result;
        }
        
        
        public string VisitChildren(Node node) {
            var sb = new StringBuilder();
            foreach (var n in node) {
                sb.Append(Visit((dynamic) n));
            }
            return sb.ToString();
        }
        //Termina la visita de los nodos---------------------------------
        
        
    }
}