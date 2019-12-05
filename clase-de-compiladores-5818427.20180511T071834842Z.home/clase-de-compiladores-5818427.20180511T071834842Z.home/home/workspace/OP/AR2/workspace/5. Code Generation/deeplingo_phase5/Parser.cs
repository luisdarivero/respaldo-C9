/*
  <program> -> <def-list> EOF
  <def-lst> -> (<var-def> | <fun-def>)*
  <var-def> -> var <id-list>;
  <id-list> -> <id> <id-list-cont>
  <id-list-cont> -> , ‹id› ‹id-list-cont›?
  <fun-def> -> <id> (<id-list>?) { <var-def>* <stmt>* }
  <stmt> -> ‹stmt-assign› | ‹stmt-incr› | ‹stmt-decr› | ‹stmt-fun-call› |
            ‹stmt-if› | ‹stmt-loop› | ‹stmt-break› | ‹stmt-return› |
            ‹stmt-empty›
  
  <stmt-assign> -> <id> = <expr>;
  ‹stmt-incr› -> ‹id› ++ ;
  ‹stmt-decr› -> ‹id› −− ;
  <expr-list> -> <expr> ‹expr-list-cont›?  
  ‹stmt-fun-call› -> <fun-call>;
  ‹fun-call› -> ‹id› ( <expr-list> )
  <expr-lst-cont> -> , <expr> ‹expr-list-cont› ?
  ‹stmt-if› -> if ( ‹expr› ) { ‹stmt>* } <else-if-list>? <else>?
  <else> -> else { ‹stmt›* }
  <else-if-list> -> elseif ( <expr> ) { <stmt>* } *
  ‹stmt-loop› -> loop { ‹stmt›* }
  ‹stmt-break› -> break ;
  ‹stmt-return› -> return ‹expr› ;
  ‹stmt-empty› -> ;
  
  ‹expr› –> ‹expr-or›
  ‹expr-or› –> ‹expr-and› (|| ‹expr-and›)*
  ‹expr-and› –> ‹expr-comp› (&& ‹expr-comp›)*
  ‹expr-comp› –> ‹expr-rel› (‹op-comp› ‹expr-rel›)*
  ‹op-comp› –> == | !=
  ‹expr-rel› –> ‹expr-add› (‹op-rel›  ‹expr-add›)*
  ‹op-rel›  –> < | <= | > | >=
  ‹expr-add› –> ‹expr-mul› (‹op-add›  ‹expr-mul›)*
  ‹op-add› –> + | -
  ‹expr-mul›  –> ‹expr-unary› (‹op-mul› ‹expr-unary›)*
  ‹op-mul› –> * | / | %
  ‹expr-unary› –> (‹op-unary› *) ‹expr-primary›
  ‹op-unary› –> + | - | !
  ‹expr-primary› –> <id> | <func-call> | [‹expr› <expr-list-cont> ?] | <lit-int> |  <lit-double> | <lit-str> | (‹expr›)
*/


/*
  Buttercup compiler - This class performs the syntactic analysis,
  (a.k.a. parsing).
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

namespace DeepLingo {

    class Parser {      
        
        // ------------------------ And here ------------------------- 

        static readonly ISet<TokenCategory> firstOfDeclaration =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.VAR
            };

        static readonly ISet<TokenCategory> firstOfStatement =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.IF,
                TokenCategory.LOOP,
                TokenCategory.BREAK,
                TokenCategory.RETURN
            };
            
        static readonly ISet<TokenCategory> firstOfAssign =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.ASSIGN,
                TokenCategory.INC,
                TokenCategory.DEC,
                TokenCategory.PARENTHESIS_OPEN
            };
            
        static readonly ISet<TokenCategory> firstOfOpComp =
            new HashSet<TokenCategory>() {
                TokenCategory.EQUALS,
                TokenCategory.NOT_EQUAL
            };
            
        static readonly ISet<TokenCategory> firstOfOpRel =
            new HashSet<TokenCategory>() {
                TokenCategory.LESS,
                TokenCategory.LESS_EQ,
                TokenCategory.GREAT,
                TokenCategory.GREAT_EQ
            };
            
        static readonly ISet<TokenCategory> firstOfOpAdd =
            new HashSet<TokenCategory>() {
                TokenCategory.ADD,
                TokenCategory.SUBS
            };

        static readonly ISet<TokenCategory> firstOfOpMul =
            new HashSet<TokenCategory>() {
                TokenCategory.MUL,
                TokenCategory.DIV,
                TokenCategory.REM
            };
            
        static readonly ISet<TokenCategory> firstOfOpUnary =
            new HashSet<TokenCategory>() {
                TokenCategory.POS,
                TokenCategory.SUBS,
                TokenCategory.NOT
            };

        static readonly ISet<TokenCategory> firstOfExpressionPrimary =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.BRACKET_OPEN,
                TokenCategory.LIT_INT,
                TokenCategory.LIT_CHAR,
                TokenCategory.LIT_STR,
                TokenCategory.PARENTHESIS_OPEN,
                
            };
            
        // ---------------- HERE ENDS THE MAGIC SOMEHOW ----------------
        //Y yo aqui perdiendo el tiempo      
        IEnumerator<Token> tokenStream;

        public Parser(IEnumerator<Token> tokenStream) {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }

        public TokenCategory CurrentToken {
            get { return tokenStream.Current.Category; }
        }

        public Token Expect(TokenCategory category) {
            if (CurrentToken == category) {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            } else {
                throw new SyntaxError(category, tokenStream.Current);                
            }
        }
        
        
        //------------------THE MAGIC STARTS HERE------------------------------
        

        public Node Program() {            
            var n1=new Programme();
            while (firstOfDeclaration.Contains(CurrentToken)) {
                if(CurrentToken == TokenCategory.IDENTIFIER){
                    n1.Add(FunDef());
                }
                else if(CurrentToken == TokenCategory.VAR){
                    n1.Add(VarDef());
                }
            }
            Expect(TokenCategory.EOF);
            return n1;
        }

        public Node FunDef() {
            var n1=new FunctionDef(){
                AnchorToken=Expect(TokenCategory.IDENTIFIER)
            };
            Expect(TokenCategory.PARENTHESIS_OPEN);
            if(CurrentToken == TokenCategory.IDENTIFIER){
                n1.Add(IDList(new ParamList()));
            }else{
                n1.Add(new ParamList());
            }
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.BRACE_OPEN);
            while(CurrentToken == TokenCategory.VAR){
                n1.Add(VarDef());
            }
            if (firstOfStatement.Contains(CurrentToken)) {
                var listStmt = new ListStatements();
                while(firstOfStatement.Contains(CurrentToken)){
                    listStmt.Add(Statement());
                }
                n1.Add(listStmt);
            }
            Expect(TokenCategory.BRACE_CLOSE);
            return n1;
        }
        
        public Node VarDef() {
            var token=Expect(TokenCategory.VAR);
            var n1=IDList(new IdList());
            n1.AnchorToken=token;
            Expect(TokenCategory.SEMICOL);
            return n1;
        }
        
        public Node IDList(Node n1){
            n1.Add(new Identifier() {
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            });
            if (CurrentToken == TokenCategory.COMMA) {
                IDListCont(n1);
            }
            return n1;
        }
        
        public Node IDListCont(Node idList){
            if (CurrentToken == TokenCategory.COMMA) {
                Expect(TokenCategory.COMMA);
                idList.Add(new Identifier() {
                    AnchorToken = Expect(TokenCategory.IDENTIFIER)
                });
                IDListCont(idList);
            }
            return idList;
        }
        
        
        
        
        // --------------------- ESTO VA TOMANDO FORMA ------------------
        
        public Node Statement() {
            if (CurrentToken==TokenCategory.IDENTIFIER){
                var token=Expect(TokenCategory.IDENTIFIER);
                switch (CurrentToken) {

                case TokenCategory.ASSIGN:
                    var n1=StatementAssignment();
                    n1.AnchorToken=token;
                    return n1;
    
                case TokenCategory.INC:
                    var n2 = StatementInc();
                    n2.AnchorToken = token;
                    return n2;
                    
                case TokenCategory.DEC:
                    var n3 = StatementDec();
                    n3.AnchorToken=token;
                    return n3;
                    
                case TokenCategory.PARENTHESIS_OPEN:
                    var n4 =StatementFunCall();
                    n4.AnchorToken = token;
                    return n4;
                    
                default:
                    throw new SyntaxError(firstOfAssign, 
                                          tokenStream.Current);
                }
            }
            else {
                switch (CurrentToken) {
                    
                case TokenCategory.IF:
                    return StatementIf();
                    
                case TokenCategory.LOOP:
                    return StatementLoop();
                    
                case TokenCategory.BREAK:
                    return StatementBreak();
                    
                case TokenCategory.RETURN:
                    return StatementReturn();
                
                default:
                    throw new SyntaxError(firstOfStatement, 
                                          tokenStream.Current);
                }
                
            }
           
        }

        public Node StatementAssignment() {
            Expect(TokenCategory.ASSIGN);
            var expr = Expression();
            var assign = new Assignment { expr };
            Expect(TokenCategory.SEMICOL);
            return assign;
        }
        
        public Node StatementInc() {
            Expect(TokenCategory.INC);
            var inc = new Inc();
            Expect(TokenCategory.SEMICOL);
            return inc;
        }
    
        public Node StatementDec() {
            Expect(TokenCategory.DEC);
            var dec=new Dec();
            Expect(TokenCategory.SEMICOL);
            return dec;
        }
        
        public Node StatementFunCall() {
            var result = new FunctionCall();
            result.Add(FunctionCall());
            Expect(TokenCategory.SEMICOL);
            return result;
        }
        
        public Node FunctionCall(){
            Expect(TokenCategory.PARENTHESIS_OPEN);
            var result = ExpressionList();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            return result;
        }

        // ---------------- ALL MAKE SENSE   ----------------
        public Node StatementIf() {
            var n1=new If(){
                AnchorToken=Expect(TokenCategory.IF)
            };
            Expect(TokenCategory.PARENTHESIS_OPEN);
            n1.Add(Expression());//Esta es la condición
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.BRACE_OPEN);
            var list=new ListStatements();
            while (firstOfStatement.Contains(CurrentToken)) {
                list.Add(Statement());
            }
            n1.Add(list); // Statements IF
            Expect(TokenCategory.BRACE_CLOSE);
            n1.Add(ElseIfList()); // Elseif list
            n1.Add(Else()); // Else
            return n1;
        }
        
        public Node ExpressionList(){
            var result=new ExprList();
            if (firstOfExpressionPrimary.Contains(CurrentToken) ||
                firstOfOpUnary.Contains(CurrentToken)){
                result.Add(Expression());
                if (CurrentToken == TokenCategory.COMMA) {
                    ExpressionListCont(result);
                }
            }
            return result;
        }
        
        public Node ExpressionListCont(Node exprList){
            if (CurrentToken == TokenCategory.COMMA) {
                Expect(TokenCategory.COMMA);
                var result = Expression();
                exprList.Add(result);
                ExpressionListCont(exprList);
                
            }
            return exprList;
        }
        
        public Node ElseIfList(){
            var elseIfList = new ListElseIf();
            while (CurrentToken == TokenCategory.ELSEIF){
                var elseIfToken = new ElseIf() {
                    AnchorToken = Expect(TokenCategory.ELSEIF)
                };
                Expect(TokenCategory.PARENTHESIS_OPEN);
                elseIfToken.Add(Expression());
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                var n1 = new ListStatements();
                Expect(TokenCategory.BRACE_OPEN);
                while(firstOfStatement.Contains(CurrentToken)){
                    n1.Add(Statement());
                }
                elseIfToken.Add(n1);
                Expect(TokenCategory.BRACE_CLOSE);
                elseIfList.Add(elseIfToken);
            }
            return elseIfList;
        }
        
        public Node Else(){
            var n1 = new ListStatements();
            if (CurrentToken == TokenCategory.ELSE){
                Expect(TokenCategory.ELSE);
                Expect(TokenCategory.BRACE_OPEN);
                while(firstOfStatement.Contains(CurrentToken)){
                    n1.Add(Statement());
                }
                Expect(TokenCategory.BRACE_CLOSE);
            }
            return n1;
        }
        
        public Node StatementLoop() {
            var n1 =new Loop() { 
                AnchorToken=Expect(TokenCategory.LOOP)
            }; 
            Expect(TokenCategory.BRACE_OPEN);
            while (firstOfStatement.Contains(CurrentToken)) {
                n1.Add(Statement());
            }
            Expect(TokenCategory.BRACE_CLOSE);
            return n1;
        }
        
        public Node StatementBreak() {
            var n1 = new Break() {
                AnchorToken = Expect(TokenCategory.BREAK)
            };
            Expect(TokenCategory.SEMICOL);
            return n1;
        }
        
        public Node StatementReturn() {
            var n1 = new Return() {
                AnchorToken = Expect(TokenCategory.RETURN)
            };
            var result = Expression();
            n1.Add(result);
            Expect(TokenCategory.SEMICOL);
            return n1;
        }
        
        
        // ------------------ Hasta aquí, todo chido ------------------------
            
        public Node Expression() {
            return ExpressionOr();
        }
    
        public Node ExpressionOr() {
            var n1 = ExpressionAnd();
            while (CurrentToken == TokenCategory.OR) {
                var n2 = new Or(){
                    AnchorToken=Expect(TokenCategory.OR)
                };
                n2.Add(n1);
                n2.Add(ExpressionAnd());
                n1=n2;
            }
            return n1;
        }
        
        public Node ExpressionAnd() {
            var n1 = ExpressionComp();
            while (CurrentToken == TokenCategory.AND) {
                var n2 = new And() {
                    AnchorToken=Expect(TokenCategory.AND)
                };
                n2.Add(n1);
                n2.Add(ExpressionComp());
                n1 = n2;
            }
            return n1;
        }
        
        public Node ExpressionComp() {
            var n1 = ExpressionRel();
            while (firstOfOpComp.Contains(CurrentToken)) {
                var n2 = OpComp();
                n2.Add(n1);
                n2.Add(ExpressionRel());
                n1 = n2;
            }
            return n1;
        }
        
        // ------------------ Ya tiene sentido TODA la vida ------------------------
        public Node OpComp(){
            switch (CurrentToken) {
                
            case TokenCategory.EQUALS:
                return new Equals() {
                    AnchorToken = Expect(TokenCategory.EQUALS)
                };
                
            case TokenCategory.NOT_EQUAL:
                return new NotEq() {
                    AnchorToken = Expect(TokenCategory.NOT_EQUAL)
                };
                
            default:
                throw new SyntaxError(firstOfOpComp, 
                                      tokenStream.Current);
            }
        }
        
        public Node ExpressionRel() {
            var n1 = ExpressionAdd();
            while (firstOfOpRel.Contains(CurrentToken)) {
                var n2 = OpRel();
                n2.Add(n1);
                n2.Add(ExpressionAdd());
                n1 = n2;
            }
            return n1;
        }
        
        public Node OpRel(){
            switch (CurrentToken) {
                
            case TokenCategory.LESS:
                return new Less() {
                    AnchorToken = Expect(TokenCategory.LESS)
                };
                
            case TokenCategory.LESS_EQ:
                return new LessEq() {
                    AnchorToken = Expect(TokenCategory.LESS_EQ)
                };
                
            case TokenCategory.GREAT:
                return new Great() {
                    AnchorToken = Expect(TokenCategory.GREAT)
                };
            
            case TokenCategory.GREAT_EQ:
                return new GreatEq() {
                    AnchorToken = Expect(TokenCategory.GREAT_EQ)
                };
                
            default:
                throw new SyntaxError(firstOfOpRel, 
                                      tokenStream.Current);
            }
        }
        
        public Node ExpressionAdd() {
            var n1=ExpressionMul();
            while (firstOfOpAdd.Contains(CurrentToken)) {
                var n2 = OpAdd();
                n2.Add(n1);
                n2.Add(ExpressionMul());
                n1 = n2;
            }
            return n1;
        }
        
        public Node OpAdd(){
            switch (CurrentToken) {
                
            case TokenCategory.ADD:
                return new Add() {
                        AnchorToken = Expect(TokenCategory.ADD)
                    };
                
            case TokenCategory.SUBS:
                return new Subs() {
                    AnchorToken = Expect(TokenCategory.SUBS)
                };
                
            default:
                throw new SyntaxError(firstOfOpAdd, 
                                      tokenStream.Current);
            }
        }
        
        public Node ExpressionMul() {
            var n1=ExpressionUnary();
            while (firstOfOpMul.Contains(CurrentToken)) {
                var result=OpMul();
                result.Add(n1);
                result.Add(ExpressionUnary());
                n1=result;
            }
            return n1;
        }
        
        public Node OpMul(){
            switch (CurrentToken) {
                
            case TokenCategory.MUL:
                return new Mult() {
                    AnchorToken = Expect(TokenCategory.MUL)
                };
                
            case TokenCategory.DIV:
                return new Div() {
                    AnchorToken = Expect(TokenCategory.DIV)
                };
                
            case TokenCategory.REM:
                return new Rem() {
                    AnchorToken = Expect(TokenCategory.REM)
                };
                
            default:
                throw new SyntaxError(firstOfOpMul, 
                                      tokenStream.Current);
            }
        }
        
        public Node ExpressionUnary() {
            
            if (firstOfOpUnary.Contains(CurrentToken)) {
                var result=OpUnary();
                result.Add(ExpressionUnary());
                return result;
            }
            else if (firstOfExpressionPrimary.Contains(CurrentToken)) {
                return ExpressionPrimary();
            }
            else {
                throw new SyntaxError(firstOfExpressionPrimary, 
                                      tokenStream.Current);
            }
        }
        
        public Node OpUnary(){
            switch (CurrentToken) {
                
            case TokenCategory.POS:
                return new Pos() {
                    AnchorToken = Expect(TokenCategory.POS)
                };
                
            case TokenCategory.SUBS:
                return new Neg() {
                    AnchorToken = Expect(TokenCategory.SUBS)
                };
                
            case TokenCategory.NOT:
                return new Not() {
                    AnchorToken = Expect(TokenCategory.NOT)
                };
                
            default:
                throw new SyntaxError(firstOfOpUnary, 
                                      tokenStream.Current);
            }
        }
        
        
        public Node ExpressionPrimary(){
            
            switch (CurrentToken) {
                
            case TokenCategory.IDENTIFIER:
                var idToken = Expect(TokenCategory.IDENTIFIER);
                if (CurrentToken == TokenCategory.PARENTHESIS_OPEN){
                    var funCall = new FunctionCall() {
                        AnchorToken = idToken
                    };
                    funCall.Add(FunctionCall());
                    return funCall;
                }
                else {
                    return new Identifier() {
                        AnchorToken = idToken
                    };
                }

                    
            case TokenCategory.BRACKET_OPEN:
                return Array();
                
            case TokenCategory.LIT_INT:
                return new IntLiteral() {
                    AnchorToken = Expect(TokenCategory.LIT_INT)
                };
     
                
            case TokenCategory.LIT_CHAR:
                return new CharLiteral() {
                    AnchorToken = Expect(TokenCategory.LIT_CHAR)
                };
                
            case TokenCategory.LIT_STR:
                return new StringLiteral() {
                    AnchorToken = Expect(TokenCategory.LIT_STR)
                };
                
            case TokenCategory.PARENTHESIS_OPEN:
                Expect(TokenCategory.PARENTHESIS_OPEN);
                var result = Expression();
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                return result;

            default:
                throw new SyntaxError(firstOfStatement, 
                                      tokenStream.Current);
            }
            
        }
        
        public Node Array() {
            Expect(TokenCategory.BRACKET_OPEN);
            var result = ExpressionList();
            Expect(TokenCategory.BRACKET_CLOSE);
            return result;
        }
    }
}
