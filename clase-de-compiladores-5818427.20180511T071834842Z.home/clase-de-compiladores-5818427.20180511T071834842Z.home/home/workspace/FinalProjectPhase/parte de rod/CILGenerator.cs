using System;
using System.Text;
using System.Collections.Generic;

namespace DeepLingo {
    class CILGenerator {

        string GenerateLabel() {return String.Format("${0:000000}", labelCounter++);}    
        public CILGenerator(){
        	
        }
        
        static readonly IDictionary<string, int> encodings = new Dictionary<string, int>() {
                { "\\n", 10},
                { "\\r", 13},
                { "\\t", 9},
                { "\\", 92},
                { "\\'", 39},
                { "\\\"", 34},
                
            };
        
        public string Indent(){
            var s = "";
            for(var i =0;i<indentCounter;i++){
                s = s + "\t";
            }
            return s;
        }
        public string Line(string s){
        	return s + "\n";
        }
        
        
        public string Visit (Arr node){
			var temp = "";
			foreach(var n in node[0]){
				temp = temp
				+ Line(Indent() + "dup")
				+ Visit((dynamic) n)
				+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Add'(int32,int32)")
				+ Line(Indent() + "pop");	
			}
			
			return 
			Line(Indent() + "ldc.i4.0")
			+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'New'(int32)")
			+ temp;
				
		}
		public string Visit (Program node){
			return 
				Line(Line("// Code generated by the deeplingo compiler.")) 
                + Line(Line(".assembly 'deeplingo' {}"))
                + Line(Line(".assembly extern 'deeplingolib' {}"))
                + ".class public 'DeepLingoProgram' extends " 
                + Line("['mscorlib']'System'.'Object' {") 
                + VisitChildren(node)
                + Line("}");
		}
		public string Visit (Def node){
			return node.ToString() + "\n" +VisitChildren(node);
		}
		public string Visit (Char node){
			var s = node.AnchorToken.Lexeme;
			if(s.Length > 3){
				if(encodings.ContainsKey(s.Substring(1,2))){
					return Line(Indent() + "ldc.i4.s " + encodings[s.Substring(1,2)])
					+ VisitChildren(node);	
				}
				else if(s.Substring(1,2) == @"\u"){
					var codePoint = getSpecialCharCodePoint(s.Substring(3,6));
                    if (codePoint <= 8) {
		                return Line(Indent() + "ldc.i4." + codePoint)
						+ VisitChildren(node);	
		
		            } else if (codePoint <= 127) {
		                return Line(Indent() + "ldc.i4.s " + codePoint)
						+ VisitChildren(node);	
		
		            } else {
		                return Line(Indent() + "ldc.i4 " + codePoint)
						+ VisitChildren(node);	
		            } 
                    
					
				}
				
			}
			return Line(Indent() + "ldc.i4.s " + (int)node.AnchorToken.Lexeme[1])
			+ VisitChildren(node);
			
		}
		public string Visit (VarDef node){
			var result = "";
			if(insideFunction){
				foreach (var n in node[0]) {
					result = result + Line(Indent() + ".locals init (int32 '"+n.AnchorToken.Lexeme+"')");
					localVariables.Add(n.AnchorToken.Lexeme);
				}
			}
			else{
				foreach (var n in node[0]) {
					result = result + Line(Indent() + ".field  public static  int32 '"+n.AnchorToken.Lexeme+"'");
					globalVariables.Add(n.AnchorToken.Lexeme);
				}
			}
			return result;	
		}
		public string Visit (IdList node){
			return VisitChildren(node);
		}
		public string Visit (Identifier node){
			if(parameters.Contains(node.AnchorToken.Lexeme)){
				return Line(Indent() + "ldarg."+ parameters[node.AnchorToken.Lexeme]);
			}
			else if(localVariables.Contains(node.AnchorToken.Lexeme)){
				return Line(Indent() + "ldloc '"+ node.AnchorToken.Lexeme + "'");	
			}
			else{
				return Line(Indent() + "ldsfld int32 'DeepLingoProgram'::'"+ node.AnchorToken.Lexeme+"'");	
			}
			
		}
		public string Visit (If node){
			var elsesbody = GenerateLabel();
			var oldEndIfLaber = endIfLabel;
			endIfLabel = GenerateLabel();
			
			var result = Visit(((dynamic) node[0]))
			+ Line(Indent() + "ldc.i4 42")
			+ Line(Indent() + "bne.un '"+elsesbody+"'")		
			+ Visit(((dynamic) node[1]))			
			+ Line(Indent() + "br "+endIfLabel+"")
			+ Line(Indent() + "'"+elsesbody+"':")	
			+ Visit(((dynamic) node[2]))			
			+ Visit(((dynamic) node[3]))			
			+ Line(Indent() + "'"+endIfLabel+"':");
			
			endIfLabel = oldEndIfLaber;
			return  result;
		}
		public string Visit (VarDefList node){
			return VisitChildren(node);
		}
		public string Visit (FunDef node){
			insideFunction = true;
			localVariables = new SymbolTable();
			var temp1 = " ";
			var temp2 = " ";
			parameters = new FunctionTable();
			var ind = 0;
			foreach(var n in node[0]){
				parameters[n.AnchorToken.Lexeme] = ind;
				temp1 += "int32,";
				temp2 += Line(Indent() + ".locals init (int32 '"+n.AnchorToken.Lexeme+"')");
				localVariables.Add(n.AnchorToken.Lexeme);
				ind++;
			}
			
			var result = Line(Indent() + ".method public static int32 '"+node.AnchorToken.Lexeme+"'("+temp1.Substring(0,temp1.Length-1)+") {");
			indentCounter++;
			if(node.AnchorToken.Lexeme == "main"){
				result += Line(Indent() + ".entrypoint");
			}
			result += temp2 
			+ Visit((dynamic) node[1]) 
			+ Visit((dynamic) node[2]) 
			+ Line(Indent() + "ldc.i4.0")
			+ Line(Indent() + "ret");
			indentCounter--;             
			insideFunction = false;
			return result + Line(Indent() + "}") + Line(Indent());
                
			
		}
		public string Visit (ElseIfList node){
			return VisitChildren(node);
		}
		public string Visit (ElseIf node){
			var lbl = GenerateLabel();
			return 
			Visit(((dynamic) node[0]))
			+ Line(Indent() + "ldc.i4 42")
			+ Line(Indent() + "bne.un '"+lbl+"'")
			+ Visit(((dynamic) node[1]))
			+ Line(Indent() + "'"+lbl + "':");
			
		}
		public string Visit (Else node){
			return VisitChildren(node);
		}
		public string Visit (StmtList node){
			var temp = "";
			foreach(var n in node){
				if (n is FunCall && !globalfunctions.Contains(n.AnchorToken.Lexeme)){
					var temp2 = "";
					var result = "";
					foreach(var x in n[0]){
						temp2 += "int32,";
						result += Visit((dynamic) x);
					}
					
					temp += result
					+ Line(Indent() + "call int32 class 'DeepLingoProgram'::'" + n.AnchorToken.Lexeme + "'("+ temp2.Substring(0, temp2.Length-1)+")") 
					+ Line(Indent() + "pop");
				}
				else{
					temp += Visit((dynamic) n);	
				}
			}
			return temp;
		}
		public string Visit (Loop node){
			oldExitLoop = currentExitLoop;
			
			var startLoop = GenerateLabel();
			currentExitLoop = GenerateLabel();
			
			var result = Line(Indent() + startLoop + ":") +
				VisitChildren(node) +
				Line(Indent() + "br " + startLoop) +
				Line(Indent() + "'"+currentExitLoop + "':");
			
			currentExitLoop = oldExitLoop;	
			return result;
		}
		public string Visit (Return node){
			return 
			VisitChildren(node)
			+ Line(Indent() + "ret");
		}
		public string Visit (Assignment node){
			if(parameters.Contains(node.AnchorToken.Lexeme)){
				return VisitChildren(node) + Line(Indent() + "starg.s "+ parameters[node.AnchorToken.Lexeme]);
			}
			else if(localVariables.Contains(node.AnchorToken.Lexeme)){
				return VisitChildren(node) + Line(Indent() + "stloc '"+node.AnchorToken.Lexeme+"'");	
			}
			else{
				return VisitChildren(node) + Line(Indent() + "stsfld int32 'DeepLingoProgram'::'"+node.AnchorToken.Lexeme+"'");
			}
		}
		public string Visit (Increment node){
			return 
			Line(Indent() + "ldloc '"+node.AnchorToken.Lexeme+"'") + 
			Line(Indent() + "ldc.i4.1") +
			Line(Indent() + "add") +
			Line(Indent() + "stloc '"+node.AnchorToken.Lexeme+"'");
		}
		public string Visit (IntLiteral node){
			var intValue = Convert.ToInt32(node.AnchorToken.Lexeme);

            if (intValue <= 8) {
                return Line(Indent() + "ldc.i4." + intValue);

            } else if (intValue <= 127) {
                return Line(Indent() + "ldc.i4.s " + intValue);

            } else {
                return Line(Indent() + "ldc.i4 " + intValue);
            } 
		}
		public string Visit (Decrement node){
			return 
			Line(Indent() + "ldloc '"+node.AnchorToken.Lexeme+"'") + 
			Line(Indent() + "ldc.i4.1") +
			Line(Indent() + "sub") +
			Line(Indent() + "stloc '"+node.AnchorToken.Lexeme+"'");
		}
		public string Visit (FunCall node){
			switch(node.AnchorToken.Lexeme){
				case "printi":
					return VisitChildren(node) 
					+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Printi'(int32)")
				    + Line(Indent() + "pop");
				case "printc":
					return VisitChildren(node) 
					+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Printc'(int32)")
				    + Line(Indent() + "pop");
				case "prints":
					return VisitChildren(node) 
					+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Prints'(int32)")
				    + Line(Indent() + "pop");
				case "println":
					return VisitChildren(node) 
					+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Println'()")
				    + Line(Indent() + "pop");
				case "readi":
					return VisitChildren(node) 
					+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Readi'()");
				case "reads":
					return VisitChildren(node) 
					+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Reads'()");
				case "new":
					return VisitChildren(node) 
					+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'New'(int32)");
				case "size":
					return VisitChildren(node) 
					+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Size'(int32)");
				case "add":
					return VisitChildren(node) 
					+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Add'(int32,int32)")
					+ Line(Indent() + "pop");
				case "get":
					return VisitChildren(node) 
					+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Get'(int32,int32)");
				case "set":
					return VisitChildren(node) 
					+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Set'(int32,int32,int32)")
					+ Line(Indent() + "pop");	
				case "pow":
					return VisitChildren(node) 
					+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Pow'(int32,int32)");	
				default:
					var temp2 = "";
					if(addpop){
						addpop = false;
						temp2 = Line(Indent() + "pop");
					}
					
					var temp = "";
					var result = "";
					foreach(var n in node[0]){
						temp += "int32,";
						result += Visit((dynamic) n);
					}
					
					var t = result
					+ Line(Indent() + "call int32 class 'DeepLingoProgram'::'" + node.AnchorToken.Lexeme + "'("+ temp.Substring(0, temp.Length-1)+")") 
					+ temp2;
					return t;
			}
		}
		public string Visit (Stmt node){
			if(node.AnchorToken.Category == TokenCategory.BREAK){
			    return Line(Indent() + "br " + currentExitLoop);    
			}
			return node.ToString() + "\n" +VisitChildren(node);
		}
		public string Visit (ExprList node){
			return VisitChildren(node);
		}
		public string Visit (ExprOr node){
			var result = "";
			var truecondition = GenerateLabel();
			var endcondition = GenerateLabel();
			if(node.AnchorToken.Category == TokenCategory.OR){
				foreach(var n in node){
					result += 
					Visit((dynamic) n)
					+ Line(Indent() + "ldc.i4.0")
					+ Line(Indent() + "bne.un '"+truecondition+"'");
				}
				return result
				+ Line(Indent() + "ldc.i4.0")
				+ Line(Indent() + "br "+endcondition)
				+ Line(Indent() + truecondition + ":")
				+ Line(Indent() + "ldc.i4 42")
				+ Line(Indent() + endcondition + ":");
			}
			return node.ToString() + "\n" +VisitChildren(node);
			
		}
		public string Visit (ExprAnd node){
			var result = "";
			var lbl = GenerateLabel();
			var lbl2 = GenerateLabel();
			if(node.AnchorToken.Category == TokenCategory.AND){
				foreach(var n in node){
					result += 
					Visit((dynamic) n)
					+ Line(Indent() + "ldc.i4 42")
					+ Line(Indent() + "bne.un '"+lbl+"'");
				}
				return result
				+ Line(Indent() + "ldc.i4 42")
				+ Line(Indent() + "br "+lbl2)
				+ Line(Indent() + lbl + ":")
				+ Line(Indent() + "ldc.i4.0 ")
				+ Line(Indent() + lbl2 + ":");
					
			}
			return node.ToString() + "\n" +VisitChildren(node);
		}
		public string Visit (ExprAdd node){
		    if(node.AnchorToken.Category == TokenCategory.PLUS){
		        return VisitChildren(node) + Line(Indent() + "add.ovf");
		    }
		    else if(node.AnchorToken.Category == TokenCategory.MINUS){
		        return VisitChildren(node) + Line(Indent() + "sub");
		    }
			return node.ToString() + "\n" +VisitChildren(node);
		}
		public string Visit (ExprComp node){
			var lbl = GenerateLabel();
			if(node.AnchorToken.Category == TokenCategory.EQUALS){
				return 
				Line(Indent() + "ldc.i4.0")
				+ VisitChildren(node)
				+ Line(Indent() + "bne.un '"+lbl+"'")
				+ Line(Indent() + "pop")
				+ Line(Indent() + "ldc.i4 42")
				+ Line(Indent() + lbl + ":");
			}
			else if(node.AnchorToken.Category == TokenCategory.NOTEQUALS){
				return 
				Line(Indent() + "ldc.i4 42")
				+ VisitChildren(node)
				+ Line(Indent() + "bne.un '"+lbl+"'")
				+ Line(Indent() + "pop")
				+ Line(Indent() + "ldc.i4.0")
				+ Line(Indent() + lbl + ":");
			}
			return node.ToString() + "\n" +VisitChildren(node);
		}
		public string Visit (ExprRel node){
			var lbl = GenerateLabel();
			if(node.AnchorToken.Category == TokenCategory.GREATER){
				return 
				Line(Indent() + "ldc.i4.0")
				+ VisitChildren(node)
				+ Line(Indent() + "ble '"+lbl+"'")
				+ Line(Indent() + "pop")
				+ Line(Indent() + "ldc.i4 42")
				+ Line(Indent() + lbl + ":");	
			}
			if(node.AnchorToken.Category == TokenCategory.LESSEQUAL){
				return 
				Line(Indent() + "ldc.i4 42")
				+ VisitChildren(node)
				+ Line(Indent() + "ble '"+lbl+"'")
				+ Line(Indent() + "pop")
				+ Line(Indent() + "ldc.i4.0")
				+ Line(Indent() + lbl + ":");	
			}
			if(node.AnchorToken.Category == TokenCategory.GREATEREQUAL){
				return 
				Line(Indent() + "ldc.i4 42")
				+ VisitChildren(node)
				+ Line(Indent() + "bge '"+lbl+"'")
				+ Line(Indent() + "pop")
				+ Line(Indent() + "ldc.i4.0")
				+ Line(Indent() + lbl + ":");	
			}
			if(node.AnchorToken.Category == TokenCategory.LESS){
				return 
				Line(Indent() + "ldc.i4.0")
				+ VisitChildren(node)
				+ Line(Indent() + "bge '"+lbl+"'")
				+ Line(Indent() + "pop")
				+ Line(Indent() + "ldc.i4 42")
				+ Line(Indent() + lbl + ":");	
			}
			return node.ToString() + "\n" +VisitChildren(node);
			
			
		}
		public string Visit (ExprMul node){
			switch(node.AnchorToken.Category){
				case TokenCategory.MODULO:
					return VisitChildren(node) + 
					Line(Indent() + "rem");
				case TokenCategory.MULTIPLICATION:
					return VisitChildren(node) + 
					Line(Indent() + "mul.ovf");
				case TokenCategory.DIVIDE:
					return VisitChildren(node) + 
					Line(Indent() + "div");
				default:
					return null;
			}
		}
		public string Visit (ExprUnary node){
			if(node.AnchorToken.Lexeme.Equals("!")){
				var lbl = GenerateLabel();
				return 
					Line(Indent() + "ldc.i4.0")+
					VisitChildren(node) + 
					Line(Indent() + "ldc.i4 42")+
					Line(Indent() + "beq '"+lbl+"'")+
					Line(Indent() + "pop")+
					Line(Indent() + "ldc.i4 42")+ 
					Line(Indent() + "'"+lbl+"':");
			}
			if(node.AnchorToken.Lexeme.Equals("-")){
				return 
					Line(Indent() + "ldc.i4.0")+
					VisitChildren(node) + 
					Line(Indent() + "sub");
			}
			if(node.AnchorToken.Lexeme.Equals("+")){
				return VisitChildren(node);
			}
			return VisitChildren(node);
		}
		public string Visit (ExprPrimary node){
			return node.ToString() + "\n" +VisitChildren(node);
		}
		public string Visit (Str node){
			var val = node.AnchorToken.Lexeme;
			var size = val.Length;
			var temp = "";
			for(var i = 1;i<size-1;i++){
				var toadd = (int)val[i];
				if(i+1<size-1 && encodings.ContainsKey(val.Substring(i,2))){
                    toadd = encodings[val.Substring(i,2)];
					i += 1;
				}
				else if(i+7<size-1 && val.Substring(i,2) == @"\u"){
					toadd = getSpecialCharCodePoint(val.Substring(i+2,6));
					i += 7;
				}
				temp = temp
				+ Line(Indent() + "dup")
				+ Line(Indent() + "ldc.i4 "+ toadd)
				+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Add'(int32,int32)")
				+ Line(Indent() + "pop");	
			}
			
			return Line(Indent() + "ldc.i4.0")
			+ Line(Indent() + "call int32 class ['deeplingolib']'DeepLingo'.'Utils'::'New'(int32)")
			+ temp;
		}
        
        string VisitChildren(Node node) {
            var sb = new StringBuilder();
            foreach (var n in node) {
                sb.Append(Visit((dynamic) n));
            }
            return sb.ToString();
        }
        int getSpecialCharCodePoint(string hexCode){
            int codePoint = 0;
            try{
                codePoint = (int)Convert.ToInt64(hexCode, 16);
            }
            catch(Exception e){
                Console.WriteLine("Could not convert "+hexCode+" hex to character, using 0: " + e);
            }
            return codePoint;
        }
    }
}