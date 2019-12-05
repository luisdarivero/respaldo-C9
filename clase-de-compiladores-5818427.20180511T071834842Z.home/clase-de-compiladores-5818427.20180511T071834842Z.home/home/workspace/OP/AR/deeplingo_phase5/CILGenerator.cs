using System;
using System.Text;
using System.Collections.Generic;

namespace DeepLingo {
    class CILGenerator {

        SemanticAnalyzer SymbolTable;
        string funName="";
        private string loopExit;
        int labelCounter = 0;
        
        string GenerateLabel() {
            return String.Format("${0:000000}", labelCounter++);
        }
        
        public CILGenerator(SemanticAnalyzer table) {
            this.SymbolTable = table;
        }
        
        public string Visit(Programme node) {
            var sb = new StringBuilder();
            sb.Append("// Code generated by the DeepLingo compiler\n\n");
            sb.Append(".assembly 'deeplingo' {}\n\n");
            sb.Append(".assembly extern 'deeplingolib' {}\n\n");
            sb.Append(".class public 'DeepLingoProgram' extends ");
            sb.Append("['mscorlib']'System'.'Object'{\n");
            var result="";
            foreach (var n in node) {
                if (n is FunctionDef){
                    funName = n.AnchorToken.Lexeme;
                    result = "\t.method public static void '" + funName + "'() {\n";
                    sb.Append(result);
                    sb.Append("\t\t.entrypoint\n\n");
                    sb.Append(Visit((dynamic) n));
                    sb.Append("\t\tret\n");
                    sb.Append("\t}\n");
                }else {
                    foreach(var j in node){
                        result = "\t\tldsfld '"
                        + j.AnchorToken.Lexeme 
                        + "'\n";
                        sb.Append(result);
                    }
                }
            }
            sb.Append("}\n");
            
            return sb.ToString();
            
        }
        
        
        //-----------------------------------------------------------
        public string Visit(FunctionDef node) {
            var sb = new StringBuilder();
            foreach(var j in node){
                sb.Append(Visit((dynamic) j));
            }
            return sb.ToString();
        }
        
        //-----------------------------------------------------------
        
        public string Visit(ParamList node){
            var sb = new StringBuilder();
            var result = "";
            foreach(var n in node){
                result = "\t\tldarg '"
                + n.AnchorToken.Lexeme 
                + "'\n";
                sb.Append(result);
            }
            
            return sb.ToString();
        }
        
        //-----------------------------------------------------------
        public string Visit(IdList node) {
            var sb = new StringBuilder();
            var result = "";
            foreach(var n in node){
                result = "\t\tldloc '"
                + n.AnchorToken.Lexeme 
                + "'\n";
                sb.Append(result);
            }
            
            return sb.ToString();
        }
        
        
        //-----------------------------------------------------------
        public string Visit(FunctionCall node) {
            // Falta Esto!!!
            var sb = new StringBuilder();
            var result = "";
            if (node.AnchorToken.Lexeme.Equals("prints")){//Es método pre-definido;
                foreach (var n in node[0]) {
                    //Console.WriteLine(n.AnchorToken.Lexeme);
                    foreach(var j in n.AnchorToken.Lexeme){
                        //Console.WriteLine(j+" "+((int)j));
                        result = "\t\tldc.i4 " + Convert.ToInt32(j)+"\n"
                                  +"\t\tcall int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Prints'(int32)\n";
                        sb.Append(result);
                    }
                }
                //result = "\t\tcall int32 class ['deeplingolib']'DeepLingo'.'Utils'::'" + node.AnchorToken.Lexeme +"'(int32)\n";
            }/*else{//es método del programa
                result = "\t\tcall int32 class ['deeplingolib']'DeepLingo'."
                       + "'Utils'::'"+ node.AnchorToken.Lexeme +"'("
                       + Visit((dynamic) node[0])
                       + ")\n";
            }*/
    
            
            
            return sb.ToString();
        }
        
        //-----------------------------------------------------------
        public string Visit(ExprList node) {
            return VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public string Visit(Assignment node) {
            var sb = new StringBuilder();
            //sb.Append("\t\t");
            sb.Append(Visit((dynamic) node[0]));
            sb.Append("\n\t\t");
            var result = "";
            if (SymbolTable.GlobalVars.Contains(node.AnchorToken.Lexeme)){//Es variable Global
                result="stsfld '" 
                + node.AnchorToken.Lexeme 
                + "'\n";
            }else if(SymbolTable.Table[funName].table[node.AnchorToken.Lexeme].funcType.Equals("local")){ //Es variable local
                result="stloc '" 
                + node.AnchorToken.Lexeme 
                + "'\n";
            }
            
            sb.Append(result);
            
            return sb.ToString();
        }
        
        //-----------------------------------------------------------
        public string Visit(Inc node) {
            return "\t\tldloc '" + node.AnchorToken.Lexeme + "'\n"
            + "\t\tldc.i4.1\n"
            + "\t\tadd.ovf\n";
        }
        
        //-----------------------------------------------------------
        public string Visit(Dec node) {
            return "\t\tldloc '" + node.AnchorToken.Lexeme + "'\n"
            + "\t\tldc.i4.1\n"
            + "\t\tsub.ovf\n";
            
        }
        
        //-----------------------------------------------------------
        public string Visit(If node) {
            var label = GenerateLabel();

            return String.Format(
                "{0}\t\tldc.i4 42\n\t\tbne.un '{1}'\n\t\t{2}\n\t'{1}':\n\t\t{3}\n\t\t{4}",
                Visit((dynamic) node[0]),
                label,
                Visit((dynamic) node[1]),
                Visit((dynamic) node[2]),
                Visit((dynamic) node[3])
            );
        }
        
        //-----------------------------------------------------------
        public string Visit(ListStatements node) {
            return VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public string Visit(ListElseIf node) {
            return VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public string Visit(ElseIf node) {
            var label = GenerateLabel();

            return String.Format(
                "\t\t{0}\n\t\tldc.i4 42\n\t\tbne.un '{1}'\n\t\t{2}\n\t'{1}':\n",
                Visit((dynamic) node[0]),
                label,
                Visit((dynamic) node[1])
            );
            
        }
        
        //-----------------------------------------------------------
        public string Visit(Loop node) {
            var label = GenerateLabel();
            var oldExitLoop = loopExit;
            loopExit = GenerateLabel();
            var result = "\t'"+label+"':\n"
                         + VisitChildren(node) 
                         + "\tbr " + label + "\n"
                         + "\t" + loopExit + ":\n";
            loopExit = oldExitLoop;
            return result;
        }
        
        //-----------------------------------------------------------
        public string Visit(Break node) {
            return "br " + loopExit + "\n";
        }
        
        //-----------------------------------------------------------
        public string Visit(Return node) {
            return "\t\t" + Visit((dynamic) node[0])+ "\n";
        }
        
        //-----------------------------------------------------------
        public string Visit(And node) {
            var label = GenerateLabel();
            var result = "\t\tldc.i4.0\n" 
                       + Visit((dynamic) node[0]) + "\n"
                       + "\t\tldc.i4 42\n"
                       + "\t\tbne.un " + label + "\n"
                       + Visit((dynamic) node[1]) + "\n"
                       + "\t\tldc.i4 42\n"
                       + "\t\tbne.un " + label + "\n"
                       + "\t\tpop\n"
                       + "\t\tldc.i4 42\n"
                       + "\t" +label + ":\n";
            return result;
        }
        
        //-----------------------------------------------------------
        public string Visit(Or node) {
            var label = GenerateLabel();
            var result = Visit((dynamic) node[1]) + "\n"
                       + Visit((dynamic) node[0]) + "\n"
                       + "\t\tldc.i4 42\n"
                       + "\t\tbne.un " + label + "\n"
                       + "\t\tpop\n"
                       + "\t\tldc.i4 42\n"
                       + "\t" + label + ":\n";
            return result;
        }
        
        //-----------------------------------------------------------
        public string Visit(Identifier node) {
            var sb = new StringBuilder();
            var result = "";
            if (SymbolTable.GlobalVars.Contains(node.AnchorToken.Lexeme)){//Es variable Global
                result="ldsfld '" 
                + node.AnchorToken.Lexeme + "'";
            }else if(SymbolTable.Table[funName].table[node.AnchorToken.Lexeme].funcType.Equals("local")){ //Es variable local
                result="ldloc '" 
                + node.AnchorToken.Lexeme+ "'";
            }else{//es parametro
                result="ldarg '" 
                + node.AnchorToken.Lexeme+ "'";
            }
            
            sb.Append(result);
            
            return sb.ToString();
            
        }
        
        //-----------------------------------------------------------
        public string Visit(IntLiteral node) {
            var number = Convert.ToInt32(node.AnchorToken.Lexeme);
            return "ldc.i4 " + number;
        }
        
        
        //-----------------------------------------------------------
        public string Visit(CharLiteral node) {
            return node.AnchorToken.Lexeme;
        }
        
        //-----------------------------------------------------------
        public string Visit(StringLiteral node) {
            var sb = new StringBuilder();
            var result = "";
            var number = 0;
            foreach(var j in node.AnchorToken.Lexeme){
                number = Convert.ToInt32(j);
                result = "\t\tldc.i4 " + number+ "\n";
                sb.Append(result);
            }
            return sb.ToString();
        }
        
        //-----------------------------------------------------------
        public string Visit(Less node) {
            var label = GenerateLabel();

            return String.Format(
                "\t\tldc.i4 42\n\t\t{0}\n\t\t{1}\n\t\tblt '{2}'\n\t\tpop\n\t\tldc.i4.0\n\t'{2}':\n",
                Visit((dynamic) node[0]),
                Visit((dynamic) node[1]),
                label
            );
        }
        
        //-----------------------------------------------------------
        public string Visit(LessEq node) {
            var label = GenerateLabel();

            return String.Format(
                "\t\tldc.i4 42\n\t\t{0}\n\t\t{1}\n\t\tble '{2}'\n\t\tpop\n\t\tldc.i4.0\n\t'{2}':\n",
                Visit((dynamic) node[0]),
                Visit((dynamic) node[1]),
                label
            );
        }
        
        //-----------------------------------------------------------
        public string Visit(Great node) {
            var label = GenerateLabel();

            return String.Format(
                "\t\tldc.i4 42\n\t\t{0}\n\t\t{1}\n\t\tbgt '{2}'\n\t\tpop\n\t\tldc.i4.0\n\t'{2}':\n",
                Visit((dynamic) node[0]),
                Visit((dynamic) node[1]),
                label
            );
        }
        
        //-----------------------------------------------------------
        public string Visit(GreatEq node) {
            var label = GenerateLabel();

            return String.Format(
                "\t\tldc.i4 42\n\t\t{0}\n\t\t{1}\n\t\tbge '{2}'\n\t\tpop\n\t\tldc.i4.0\n\t'{2}':\n",
                Visit((dynamic) node[0]),
                Visit((dynamic) node[1]),
                label
            );
        }
        
        //-----------------------------------------------------------
        public string Visit(Equals node) {
            var label = GenerateLabel();

            return String.Format(
                "\t\tldc.i4 42\n\t\t{0}\n\t\t{1}\n\t\tbeq '{2}'\n\t\tpop\n\t\tldc.i4.0\n\t'{2}':\n",
                Visit((dynamic) node[0]),
                Visit((dynamic) node[1]),
                label
            );
        }
        
        //-----------------------------------------------------------
        public string Visit(NotEq node) {
            var label = GenerateLabel();

            return String.Format(
                "\t\tldc.i4 42\n\t\t{0}\n\t\t{1}\n\t\tbne.un '{2}'\n\t\tpop\n\t\tldc.i4.0\n\t'{2}':\n",
                Visit((dynamic) node[0]),
                Visit((dynamic) node[1]),
                label
            );
        }
        
        //-----------------------------------------------------------
        public string Visit(Pos node) {
            return "\t\tldc.i4 " + Visit((dynamic) node[0]) + "\n";
                
        }
        
        //-----------------------------------------------------------
        public string Visit(Neg node) {
            
            return"\t\tldc.i4.0\n\t\t"+Visit((dynamic) node[0])+"\n\t\tsub.ov\n";
                
        }
        
        
        
        
        //-----------------------------------------------------------
        public string Visit(Not node) {
            var label = GenerateLabel();

            return String.Format(
                "\t\tldc.i4.0\n\t\t{0}\n\t\tldc.i4 42\n\t\tble '{1}'\n\t\tpop\n\t\tldc.i4 42\n\t'{1}':\n",
                Visit((dynamic) node[0]),
                label
            );
        }
        
        //-----------------------------------------------------------
        public string Visit(Add node) {
            return Visit((dynamic) node[0])
                +"\n\t\t" + Visit((dynamic) node[1])
                + "\n\t\tadd.ovf\n";
        }
        
        //-----------------------------------------------------------
        public string Visit(Subs node) {
            return Visit((dynamic) node[0])
                +"\n\t\t" + Visit((dynamic) node[1])
                + "\n\t\tsub.ovf\n";
        }
        
        //-----------------------------------------------------------
        public string Visit(Mult node) {
            return Visit((dynamic) node[0])
                +"\n\t\t" + Visit((dynamic) node[1])
                + "\n\t\tmul.ovf\n";
        }
        
        public string Visit(Div node) {
            return Visit((dynamic) node[0])
                +"\n\t\t" + Visit((dynamic) node[1])
                + "\n\t\tdiv.un\n";
        }
        
        public string Visit(Rem node) {
            return Visit((dynamic) node[0])
                +"\n\t\t" + Visit((dynamic) node[1])
                + "\n\t\trem.un\n";
        }
        
        string VisitChildren(Node node) {
            var sb = new StringBuilder();
            foreach (var n in node) {
                sb.Append(Visit((dynamic) n));
            }
            return sb.ToString();
        }
        
    }
}