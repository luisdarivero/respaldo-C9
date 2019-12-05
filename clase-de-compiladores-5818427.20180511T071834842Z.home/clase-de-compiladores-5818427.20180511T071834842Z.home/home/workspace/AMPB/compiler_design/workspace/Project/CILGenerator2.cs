/*
 Fortran compiler - Common Intermediate Language (CIL) code generator.
 Copyright (C) 2016, ITESM CEM
 Ariel Ortiz
 Kevin Islas
 Sebastián Loredo
 Pamela González
 
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
 
 TO-DO: MANEJO DE ARREGLOS (DATA)
 */

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Fortran {
    
    class CILGenerator {
        
        List<SymbolTable> table;
        
        int labelCounter = 0;
        bool dataInCode = false;
        bool doInCode = false;
        int indexTablas = 0;
        string dataName = "";
        IDictionary<string, List<Object>> data = new SortedDictionary<string, List<Object>>();
        
        //-----------------------------------------------------------
        string GenerateLabel() {
            return String.Format("${0:000000}", labelCounter++);
        }
        
        //-----------------------------------------------------------
        static readonly IDictionary<Type, string> CILTypes =
        new Dictionary<Type, string>() {
            { Type.TYPE_LOGICAL, "bool" },
            { Type.TYPE_INTEGER, "int32" },
            { Type.TYPE_CHARACTER, "string" },
            { Type.TYPE_REAL, "float64" },
            { Type.OBJECT, "object"},
            { Type.VOID, "void"}
        };
        
        public CILGenerator(List<SymbolTable> table) {
            this.table = table.ToList();
        }
        
        public string Visit(Program node) {
            return "// Code generated by the fortran77 compiler.\n\n"
            + ".assembly 'fortran' {}\n\n"
            + ".assembly extern 'fortranlib' {}\n\n"
            + ".class public 'FortranProgram' extends "
            + "['mscorlib']'System'.'Object' {\n"
            + "\t.method public static void 'inicio'() {\n"
            + "\t\t.entrypoint\n"
            + Visit((dynamic) node[0])
            + Visit((dynamic) node[1])
            + "\t\tret\n"
            + "\t}\n"
            + "}\n";
        }
        
        //-----------------------------------------------------------
        public string Visit(DeclarationList node) {
            // The code for the local variable declarations is
            // generated directly from the symbol table, not from
            // the AST nodes.
            var sb = new StringBuilder();
            foreach (var entry in table.ToList()[indexTablas]) {
                var variableName = entry.Key;
                if (entry.Value[0].Equals(Type.TYPE_INTEGER) &&
                (Convert.ToBoolean(table.ToList()[indexTablas][variableName][2]) == true)) {
                    sb.Append(String.Format(
                    "\t\t.locals init ({0} '{1}')\n\t\tldc.i4.s {2}\n\t\tstloc '{3}'\n",
                    CILTypes[Type.TYPE_INTEGER],
                    variableName, table.ToList()[indexTablas][variableName][1],
                    variableName)
                    );
                } else if (entry.Value[0].Equals(Type.TYPE_REAL) &&
                (Convert.ToBoolean(table.ToList()[indexTablas][variableName][2]) == true)) {
                    sb.Append(String.Format(
                    "\t\t.locals init ({0} '{1}')\n\t\tldc.r4 {2}\n\t\tstloc '{3}'\n",
                    CILTypes[Type.TYPE_REAL],
                    variableName, table.ToList()[indexTablas][variableName][1],
                    variableName)
                    );
                } else if (entry.Value[0].Equals(Type.TYPE_INTEGER) &&
                (Convert.ToBoolean(table.ToList()[indexTablas][variableName][3]) == false)) {
                    sb.Append(String.Format(
                    "\t\t.locals init ({0} '{1}')\n",
                    CILTypes[Type.TYPE_INTEGER],
                    variableName)
                    );
                } else if (entry.Value[0].Equals(Type.TYPE_REAL) &&
                (Convert.ToBoolean(table.ToList()[indexTablas][variableName][3]) == false)) {
                    sb.Append(String.Format(
                    "\t\t.locals init ({0} '{1}')\n",
                    CILTypes[Type.TYPE_REAL],
                    variableName)
                    );
                } else if (entry.Value[0].Equals(Type.TYPE_INTEGER) &&
                Convert.ToBoolean(table.ToList()[indexTablas][variableName][3]) == true &&
                Convert.ToString(table.ToList()[indexTablas][variableName][5]) == "-") {
                    sb.Append(String.Format(
                    "\t\t.locals init (class [mscorlib]System.Collections.Generic.List`1<int32> '{0}')\n",
                    variableName)
                    );
                } else if (entry.Value[0].Equals(Type.TYPE_INTEGER) &&
                Convert.ToBoolean(table.ToList()[indexTablas][variableName][3]) == true &&
                Convert.ToString(table.ToList()[indexTablas][variableName][5]) != "-") {
                    sb.Append(String.Format(
                    "\t\t.locals init (int32[0...,0...] '{0}')\n",
                    variableName)
                    );
                    var columns = Convert.ToInt32(table.ToList()[indexTablas][variableName][4]);
                    var rows = Convert.ToInt32(table.ToList()[indexTablas][variableName][5]);
                    sb.Append("\t\tldc.i4 " + columns + "\n");
                    sb.Append("\t\tldc.i4 " + rows + "\n");
                    sb.Append("\t\tnewobj instance void int32[,]::'.ctor'(int32, int32)\n");
                    sb.Append("\t\tstloc '" + variableName + "'\n");
                }
            }
            sb.Append(String.Format(
                    "\t\t.locals init ({0} '{1}')\n",
                    CILTypes[Type.TYPE_INTEGER],
                    "tempo")
                    );
            sb.Append(String.Format(
                    "\t\t.locals init ({0} '{1}')\n",
                    CILTypes[Type.TYPE_INTEGER],
                    "tempor")
                    );
            return sb.ToString();
        }
        
        //-----------------------------------------------------------
        public string Visit(Declaration node) {
            // This method is never called.
            return null;
        }
        
        //-----------------------------------------------------------
        public string Visit(StatementList node) {
            return VisitChildren(node);
        }
        
        //-----------------------------------------------------------
        public string Visit(Do node) {
            var variableName = node[1].AnchorToken.Lexeme;
            var initialValue = 0;
            var finalValue = 0;
            try {
                initialValue = Convert.ToInt32(node[1][0].AnchorToken.Lexeme);
                finalValue = Convert.ToInt32(node[1][1].AnchorToken.Lexeme);
            } catch {
                var x = Convert.ToInt32(table.ToList()[indexTablas][node[1][0][0].AnchorToken.Lexeme][1]);
                var y = x - 1;
                initialValue = y;
                
                finalValue = Convert.ToInt32(node[1][1].AnchorToken.Lexeme);
            }
            var jump = 1;
    
            try {
                table.ToList()[indexTablas][variableName][1] = initialValue;
            } catch {
                
            }
            
            try {
                jump = Convert.ToInt32(node[1][2].AnchorToken.Lexeme);
            } catch {
                try {
                    jump = Convert.ToInt32(node[1][2][0].AnchorToken.Lexeme);
                } catch {
                    jump = 1;
                }
            }
            
            var text = "";
            var label = GenerateLabel();
            var label2 = GenerateLabel();
            
            try {
                table.ToList()[indexTablas][variableName][1] = initialValue;
            } catch {
                
            }
            
            var i = Convert.ToInt32(node[0].AnchorToken.Lexeme);
            
            //Trabajar este codigo
            if ( i == 42 ) {
                text += "\t\tldc.i4 " + (initialValue - 41) + "\n";
            } else {
                text += "\t\tldc.i4 " + initialValue + "\n";
            }
            text += "\t\tstloc '" + variableName + "'\n";
            //---
            
            text += "\t\tbr " + label + "\n";
            text += label2 + ":";
            
            //Do code
            
            //if (dataInCode == true) {
            //    text += "\t\tldloc '" + dataName + "'\n";
            //    text += "\t\tldloc '" + node[1].AnchorToken.Lexeme + "'\n";
            //    text += "\t\tcallvirt instance !0 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Item(int32)\n";
            //}
            
            text += Visit((dynamic) node[2]);
            
            text += "\t\tldloc '" + node[1].AnchorToken.Lexeme + "'\n";
            text += "\t\tldc.i4 " + jump + "\n";
            
            //End of do code
            
            if (initialValue < finalValue) {
                text += "\t\tadd\n";
                text += "\t\tstloc '" + node[1].AnchorToken.Lexeme +"'\n";
                text += label + ": ldloc '" + node[1].AnchorToken.Lexeme + "'\n";
                text += "\t\tldc.i4 " + finalValue + "\n";
                text += "\t\tble " + label2 + "\n";
            } else {
                //No sale esta parte!!!! D:
                text += "\t\tsub\n";
                text += "\t\tstloc '" + node[1].AnchorToken.Lexeme +"'\n";
                text += label + ": ldloc '" + node[1].AnchorToken.Lexeme + "'\n";
                text += "\t\tldc.i4 " + finalValue + "\n";
                text += "\t\tbge " + label2 + "\n";
            }

            return text;
        }
        
        //-----------------------------------------------------------
        public string Visit(GreaterOrEqual node) {
            var sb = new StringBuilder();
            
            sb.Append(VisitBinaryOperator("cgt\n", node));
            sb.Append(VisitBinaryOperator("ceq\n", node));
            sb.Append("\t\t or\n");
            return sb.ToString();
        }
        
        //-----------------------------------------------------------
        public string Visit(Goto node) {
            return "\t\tbr $" +  node[0].AnchorToken.Lexeme + "\n";
        }
        
        //-----------------------------------------------------------
        public string Visit(Stop node) {
            return "";
        }
        //-----------------------------------------------------------
        public string Visit(End node) {
            return "";
        }
        //-----------------------------------------------------------
        public string Visit(True node) {
            return "\t\tldc.i4.1\n";
        }
        
        //-----------------------------------------------------------
        public string Visit(False node) {
            return "\t\tldc.i4.0\n";
        }
        
        //-----------------------------------------------------------
        public string Visit(Neg node) {
            try {
                var x = node[1];
                return VisitBinaryOperator("sub\n", node);
            } catch {
                return Visit((dynamic) node[0])
                + "\t\tneg\n";
            }
            //Console.WriteLine(node + " node");
            // return "\t\tldc.i4.0\n"
            //     + Visit((dynamic) node[0])
            //     + "\t\tsub.ovf\n";
        }
        
        //-----------------------------------------------------------
        public string Visit(And node) {
            return VisitBinaryOperator("and", node);
        }
        
        //-----------------------------------------------------------
        public string Visit(Less node) {
            return VisitBinaryOperator("clt\n", node);
        }
        
        //-----------------------------------------------------------
        public string Visit(Plus node) {
            return VisitBinaryOperator("add\n", node);
        }
        
        //-----------------------------------------------------------
        public string Visit(Mul node) {
            return VisitBinaryOperator("mul\n", node);
        }
        
        //-----------------------------------------------------------
        public string Visit(Div node) {
            return VisitBinaryOperator("div\n", node);
        }
        
        //-----------------------------------------------------------
        public string Visit(Cos node) {
            var text = "";
            if (node[0] is RealLiteral) {
                text += "\t\tldc.r8 " + node[0].AnchorToken.Lexeme + "\n";
            } if (node[0].AnchorToken.Lexeme == "/") {
                text += "\t\tldloc 'pi'\n";
                text += "\t\tldc.i4 " + node[0][1].AnchorToken.Lexeme + "\n";
                text += "\t\tconv.r8\n";
                text += "\t\tdiv\n";
            }
            text += "\t\tcall float64 class [mscorlib]System.Math::Cos(float64)\n";
            return text;
        }
        
        //-----------------------------------------------------------
        public string Visit(Tan node) {
            var text = "";
            if (node[0] is RealLiteral) {
                text += "\t\tldc.r8 " + node[0].AnchorToken.Lexeme + "\n";
            } if (node[0].AnchorToken.Lexeme == "/") {
                text += "\t\tldloc 'pi'\n";
                text += "\t\tldc.i4 " + node[0][1].AnchorToken.Lexeme + "\n";
                text += "\t\tconv.r8\n";
                text += "\t\tdiv\n";
            }
            text += "\t\tcall float64 class [mscorlib]System.Math::Tan(float64)\n";
            return text;
        }
        
        //-----------------------------------------------------------
        public string Visit(Atan node) {
            var text = "";
            if (node[0] is RealLiteral) {
                text += "\t\tldc.r8 " + node[0].AnchorToken.Lexeme + "\n";
            } else if (node[0].AnchorToken.Lexeme == "sqrt") {
                if (node[0][0].AnchorToken.Lexeme == "-") {
                    text += "\t\tldc.r8 " + node[0][0][0].AnchorToken.Lexeme + "\n";
                    text += "\t\tldc.r8 " + node[0][0][1].AnchorToken.Lexeme + "\n";
                    text += "\t\tsub\n";
                    text += "\t\tcall float64 class [mscorlib]System.Math::Sqrt(float64)\n";
                }
            } else if (node[0].AnchorToken.Lexeme == "**") {
                if (node[0][0] is IntLiteral) {
                    text += "\t\tldc.i4 " + node[0][0].AnchorToken.Lexeme + "\n";
                    text += "\t\tconv.r8\n";
                    text += "\t\tldc.r8 " + node[0][1].AnchorToken.Lexeme + "\n";
                    text += "\t\tcall float64 class ['mscorlib']'System'.";
                    text += "'Math'::'Pow'(float64, float64)\n";
                }
            }
            text += "\t\tcall float64 class [mscorlib]System.Math::Atan(float64)\n";
            return text;
        }
    }
}