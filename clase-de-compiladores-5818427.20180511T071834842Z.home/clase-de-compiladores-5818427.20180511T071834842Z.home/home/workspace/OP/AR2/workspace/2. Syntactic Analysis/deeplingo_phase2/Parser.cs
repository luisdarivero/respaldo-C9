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
                TokenCategory.NEG,
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
        

        public void Program() {            

            while (firstOfDeclaration.Contains(CurrentToken)) {
                if(CurrentToken == TokenCategory.IDENTIFIER){
                    FunDef();
                }
                else if(CurrentToken == TokenCategory.VAR){
                    VarDef();
                }
            }
            Expect(TokenCategory.EOF);
        }

        public void FunDef() {
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            if(CurrentToken == TokenCategory.IDENTIFIER){
                IDList();
            }
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.BRACE_OPEN);
            
            while(CurrentToken == TokenCategory.VAR){
                VarDef();
            }
            while(firstOfStatement.Contains(CurrentToken)){
                Statement();
            }
            Expect(TokenCategory.BRACE_CLOSE);
        }
        
        public void IDList() {
            Expect(TokenCategory.IDENTIFIER);
            IDListCont();
            
        }
        
        public void IDListCont(){
            if (CurrentToken == TokenCategory.COMMA) {
                Expect(TokenCategory.COMMA);
                Expect(TokenCategory.IDENTIFIER);
                IDListCont();
            }
        }
        
        public void VarDef() {
            Expect(TokenCategory.VAR);
            IDList();
            Expect(TokenCategory.SEMICOL);
        }
        
        // --------------------- ESTO VA TOMANDO FORMA ------------------
        

        public void Statement() {
            if (CurrentToken==TokenCategory.IDENTIFIER){
                Expect(TokenCategory.IDENTIFIER);
                switch (CurrentToken) {

                case TokenCategory.ASSIGN:
                    StatementAssignment();
                    break;
    
                case TokenCategory.INC:
                    StatementInc();
                    break;
                    
                case TokenCategory.DEC:
                    StatementDec();
                    break;
                    
                case TokenCategory.PARENTHESIS_OPEN:
                    StatementFunCall();
                    break;
                    
                default:
                    throw new SyntaxError(firstOfAssign, 
                                          tokenStream.Current);
                }
            }
            else {
                switch (CurrentToken) {
                    
                case TokenCategory.IF:
                    StatementIf();
                    break;
                    
                case TokenCategory.LOOP:
                    StatementLoop();
                    break;
                    
                case TokenCategory.BREAK:
                    StatementBreak();
                    break;
                    
                case TokenCategory.RETURN:
                    StatementReturn();
                    break;
                
                default:
                    throw new SyntaxError(firstOfStatement, 
                                          tokenStream.Current);
                }
                
            }
           
        }

        public void StatementAssignment() {
            Expect(TokenCategory.ASSIGN);
            Expression();
            Expect(TokenCategory.SEMICOL);
        }
        
        public void StatementInc() {
            Expect(TokenCategory.INC);
            Expect(TokenCategory.SEMICOL);
        }
        
        public void StatementDec() {
            Expect(TokenCategory.DEC);
            Expect(TokenCategory.SEMICOL);
        }
        
        public void StatementFunCall() {
            FunctionCall();
            Expect(TokenCategory.SEMICOL);
        }
        
        public void FunctionCall(){
            Expect(TokenCategory.PARENTHESIS_OPEN);
            ExpressionList();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
        }

        // ---------------- ALL MAKE SENSE   ----------------
        public void StatementIf() {
            Expect(TokenCategory.IF);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Expression();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.BRACE_OPEN);
            while (firstOfStatement.Contains(CurrentToken)) {
                Statement();
            }
            Expect(TokenCategory.BRACE_CLOSE);
            if(CurrentToken == TokenCategory.ELSEIF) {
                ElseIfList();
            }
            if(CurrentToken == TokenCategory.ELSE) {
                Else();
            }
        }
        
        public void ExpressionList(){
            
            if (firstOfExpressionPrimary.Contains(CurrentToken) ||
                firstOfOpUnary.Contains(CurrentToken)){
                Expression();
                ExpressionListCont();
            }
        }
        
        public void ExpressionListCont(){
            if (CurrentToken == TokenCategory.COMMA) {
                Expect(TokenCategory.COMMA);
                Expression();
                ExpressionListCont();
            }
        }
        
        public void ElseIfList(){
            while (CurrentToken == TokenCategory.ELSEIF){
                Expect(TokenCategory.ELSEIF);
                Expect(TokenCategory.PARENTHESIS_OPEN);
                Expression();
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                Expect(TokenCategory.BRACE_OPEN);
                while(firstOfStatement.Contains(CurrentToken)){
                    Statement();
                }
                Expect(TokenCategory.BRACE_CLOSE);    
            }
        }
        
        public void Else(){
            Expect(TokenCategory.ELSE);
            Expect(TokenCategory.BRACE_OPEN);
            while(firstOfStatement.Contains(CurrentToken)){
                Statement();
            }
            Expect(TokenCategory.BRACE_CLOSE);
        }
        
        public void StatementLoop() {
            Expect(TokenCategory.LOOP);
            Expect(TokenCategory.BRACE_OPEN);
            while (firstOfStatement.Contains(CurrentToken)) {
                Statement();
            }
            Expect(TokenCategory.BRACE_CLOSE);
        }
        
        public void StatementBreak() {
            Expect(TokenCategory.BREAK);
            Expect(TokenCategory.SEMICOL);
        }
        
        public void StatementReturn() {
            Expect(TokenCategory.RETURN);
            Expression();
            Expect(TokenCategory.SEMICOL);
        }
        
        
        // ------------------ Hasta aquí, todo chido ------------------------
            
        public void Expression() {
            ExpressionOr();
        }
    
        public void ExpressionOr() {
            ExpressionAnd();
            while (CurrentToken == TokenCategory.OR) {
                Expect(TokenCategory.OR);
                ExpressionAnd();
            }
        }
        
        public void ExpressionAnd() {
            ExpressionComp();
            while (CurrentToken == TokenCategory.AND) {
                Expect(TokenCategory.AND);
                ExpressionComp();
            }
        }
        
        public void ExpressionComp() {
            ExpressionRel();
            while (firstOfOpComp.Contains(CurrentToken)) {
                OpComp();
                ExpressionRel();
            }
        }
        
        // ------------------ Ya tiene sentido TODA la vida ------------------------
        public void OpComp(){
            switch (CurrentToken) {
                
            case TokenCategory.EQUALS:
                Expect(TokenCategory.EQUALS);
                break;
                
            case TokenCategory.NOT_EQUAL:
                Expect(TokenCategory.NOT_EQUAL);
                break;
                
            default:
                throw new SyntaxError(firstOfOpComp, 
                                      tokenStream.Current);
            }
        }
        
        public void ExpressionRel() {
            ExpressionAdd();
            while (firstOfOpRel.Contains(CurrentToken)) {
                OpRel();
                ExpressionAdd();
            }
        }
        
        public void OpRel(){
            switch (CurrentToken) {
                
            case TokenCategory.LESS:
                Expect(TokenCategory.LESS);
                break;
                
            case TokenCategory.LESS_EQ:
                Expect(TokenCategory.LESS_EQ);
                break;
                
            case TokenCategory.GREAT:
                Expect(TokenCategory.GREAT);
                break;
            
            case TokenCategory.GREAT_EQ:
                Expect(TokenCategory.GREAT_EQ);
                break;
                
            default:
                throw new SyntaxError(firstOfOpRel, 
                                      tokenStream.Current);
            }
        }
        
        public void ExpressionAdd() {
            ExpressionMul();
            while (firstOfOpAdd.Contains(CurrentToken)) {
                OpAdd();
                ExpressionMul();
            }
        }
        
        public void OpAdd(){
            switch (CurrentToken) {
                
            case TokenCategory.ADD:
                Expect(TokenCategory.ADD);
                break;
                
            case TokenCategory.SUBS:
                Expect(TokenCategory.SUBS);
                break;
                
            default:
                throw new SyntaxError(firstOfOpAdd, 
                                      tokenStream.Current);
            }
        }
        
        public void ExpressionMul() {
            ExpressionUnary();
            while (firstOfOpMul.Contains(CurrentToken)) {
                OpMul();
                ExpressionUnary();
            }
        }
        
        public void OpMul(){
            switch (CurrentToken) {
                
            case TokenCategory.MUL:
                Expect(TokenCategory.MUL);
                break;
                
            case TokenCategory.DIV:
                Expect(TokenCategory.DIV);
                break;
                
            case TokenCategory.REM:
                Expect(TokenCategory.REM);
                break;
                
            default:
                throw new SyntaxError(firstOfOpMul, 
                                      tokenStream.Current);
            }
        }
        
        public void ExpressionUnary() {
            
            if (firstOfOpUnary.Contains(CurrentToken)) {
                OpUnary();
                
                ExpressionUnary();
            }
            else if (firstOfExpressionPrimary.Contains(CurrentToken)) {
                ExpressionPrimary();
            }
            else {
                throw new SyntaxError(firstOfExpressionPrimary, 
                                      tokenStream.Current);
            }
        }
        
        public void OpUnary(){
            switch (CurrentToken) {
                
            case TokenCategory.POS:
                Expect(TokenCategory.POS);
                break;
                
            case TokenCategory.NEG:
                Expect(TokenCategory.NEG);
                break;
                
            case TokenCategory.NOT:
                Expect(TokenCategory.NOT);
               
                break;
                
            default:
                throw new SyntaxError(firstOfOpUnary, 
                                      tokenStream.Current);
            }
        }
        
        
        public void ExpressionPrimary(){
            
            switch (CurrentToken) {
                
            case TokenCategory.IDENTIFIER:
                Expect(TokenCategory.IDENTIFIER);
                if (CurrentToken == TokenCategory.PARENTHESIS_OPEN){
                    FunctionCall();
                }
                break;
                    
            case TokenCategory.BRACKET_OPEN:
                Array();
                break;
                
            case TokenCategory.LIT_INT:
                Expect(TokenCategory.LIT_INT);
                break;
                
            case TokenCategory.LIT_CHAR:
                Expect(TokenCategory.LIT_CHAR);
                break;
                
            case TokenCategory.LIT_STR:
                Expect(TokenCategory.LIT_STR);
                break;
                
            case TokenCategory.PARENTHESIS_OPEN:
                Expect(TokenCategory.PARENTHESIS_OPEN);
                Expression();
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                break;
                
            

            default:
                throw new SyntaxError(firstOfStatement, 
                                      tokenStream.Current);
            }
            
        }
        
        public void Array(){
            Expect(TokenCategory.BRACKET_OPEN);
            ExpressionList();
            Expect(TokenCategory.BRACKET_CLOSE);
        }
    }
}
