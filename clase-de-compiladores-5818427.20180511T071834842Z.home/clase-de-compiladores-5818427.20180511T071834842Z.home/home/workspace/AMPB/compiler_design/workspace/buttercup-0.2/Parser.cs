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

namespace INT64 {

    class Parser {      

        static readonly ISet<TokenCategory> EOFile =
            new HashSet<TokenCategory>() {
                TokenCategory.EOF
            };

        static readonly ISet<TokenCategory> firstOfStatement =
            new HashSet<TokenCategory>() {
                TokenCategory.EOF
            };

        static readonly ISet<TokenCategory> firstOfOperator =
            new HashSet<TokenCategory>() {
                TokenCategory.EOF
            };

        static readonly ISet<TokenCategory> firstOfSimpleExpression =
            new HashSet<TokenCategory>() {
                TokenCategory.EOF
            };
                
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

        
// aqui empieza lo nuestro **********************************************************
        public void Program() {            

            while (EOFile.Contains(CurrentToken)) {
                def_list();
            }
            Expect(TokenCategory.EOF);
        }
        
        public void def_list()
        {
         def();   
        }
         public void def()
        {
            if(TokenCategory.VAR == CurrentToken)
            {
                var_def();
            }
            if(TokenCategory.IDENTIFIER == CurrentToken)
            {
                fun_def();
            }
        }
         public void var_def()
        {
         Expect(TokenCategory.VAR);
         var_list();
         Expect(TokenCategory.SEMICOLON);
        }
        public void var_list()
        {
         id_list();
        }
        public void id_list()
        {
         Expect(TokenCategory.IDENTIFIER);
         id_list_cont();
        }
        public void id_list_cont()
        {
         Expect(TokenCategory.COMA);
         Expect(TokenCategory.IDENTIFIER);
        }
        public void fun_def()
        {
         Expect(TokenCategory.IDENTIFIER);
         Expect(TokenCategory.OPENP);
         param_list();
         Expect(TokenCategory.CLOSEP);
         Expect(TokenCategory.OPENB);
         var_def_list();
         stmt_list();
         Expect(TokenCategory.CLOSEB);
        }
         public void param_list()
        {
            id_list();
        }
        public void var_def_list()
        {
            while(CurrentToken.TokenCategory.VAR){
                id_list();
            }
        }
        public void stmt_list()
        {
            while(TokenCategory.IDENTIFIER == CurrentToken){
                stmt();
            }
        }

        public void stmt(){
            switch (CurrentToken) {
                case TokenCategory.IDENTIFIER:
                Expect(TokenCategory.IDENTIFIER);
                if(CurrentToken.TokenCategory.EQUAL){
                    Expect(TokenCategory.EQUAL);  
                    expr();
                    Expect(TokenCategory.SEMICOLON);
                }
                 else if(CurrentToken.TokenCategory.OPENP) {
                    Expect(TokenCategory.OPENP); 
                    expr_list();
                    Expect(TokenCategory.CLOSEP); 
                    Expect(TokenCategory.SEMICOLON);
                }
                else {
                     throw new SyntaxError(firstOfSimpleExpression, 
                                      tokenStream.Current);
                }
                    
               
                break;

            case TokenCategory.IF:
                Expect(TokenCategory.IF);
                Expect(TokenCategory.OPENP);
                expr();
                Expect(TokenCategory.CLOSEP);
                Expect(TokenCategory.OPENB);
                srmt_list();
                Expect(TokenCategory.CLOSEB); 
                else_if_list();
                else_();
                break;

            case TokenCategory.SWITCH:
                Expect(TokenCategory.SWITCH);
                Expect(TokenCategory.OPENP);
                expr();
                Expect(CLOSEP);
                Expect(TokenCategory.OPENB);
                case_list();
                default_();
                Expect(CLOSEB);
                break;
                

            case TokenCategory.WHILE:
                Expect(TokenCategory.WHILE);
                Expect(TokenCategory.OPENP);
                expr();
                Expect(TokenCategory.CLOSEP);
                break;
    /*
            case TokenCategory.PARENTHESIS_OPEN:
                Expect(TokenCategory.PARENTHESIS_OPEN);
                Expression();
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                break;
        */

            case TokenCategory.DO: //OYE AQUI NO ES DO_WHILE???
                Expect(TokenCategory.DO); 
                Expect(TokenCategory.OPENB);
                stmt_list();
                Expect(TokenCategory.CLOSEB);
                Expect(TokenCategory.WHILE);
                Expect(TokenCategory.OPENP);
                expr();
                Expect(TokenCategory.CLOSEP);
                Expect(TokenCategory.SEMICOLON);
                break;

            case TokenCategory.FOR:
                Expect(TokenCategory.FOR);
                Expect(TokenCategory.OPENP);
                Expect(TokenCategory.IDENTIFIER);
                Expect(TokenCategory.IN);
                expr();
                Expect(TokenCategory.CLOSEP);
                Expect(TokenCategory.OPENB);
                stmt_list();
                Expect(TokenCategory.CLOSEB);
                break;
            
            case TokenCategory.BREAK:
                Expect(TokenCategory.BREAK);
                Expect(TokenCategory.SEMICOLON);
                break;
            
            case TokenCategory.CONTINUE:
                Expect(TokenCategory.CONTINUE);
                Expect(TokenCategory.SEMICOLON);
                break;
                
            case TokenCategory.RETURN:
                Expect(TokenCategory.RETURN);
                expr();
                Expect(TokenCategory.SEMICOLON);
                break;
                
            case TokenCategory.EMPTY:
                Expect(TokenCategory.SEMICOLON);
                break;
                
            default:
                throw new SyntaxError(firstOfSimpleExpression, 
                                      tokenStream.Current);
                
                
            }
        
        }
        public void expr_list()
        {
            expr();
            if(CurrentToken.TokenCategory.COMA){
                expr_list_cont();
            }
            
        } 
        
        
        public void expr_list_cont(){
            while(CurrentToken.TokenCategory.COMA){
            expr();
            expr_list_cont();
            }
        }
        
        public void else_if_list(){
            while(CurrentToken.Category.ELSE)
            {
                Expect(TokenCategory.ELSE);
                if(CurrentToken.Category.IF)
                {
                Expect(TokenCategory.IF);
                Expect(TokenCategory.OPENP);
                expr();
                Expect(TokenCategory.CLOSEP);
                Expect(TokenCategory.OPENB);
                stmt_list();
                Expect(TokenCategory.CLOSEB);
                }
                else if(CurrentToken.Category.OPENB)
                {
                    else_();
                }
            }
            
        }
        
        public void else_(){
            Expect(TokenCategory.ELSE);
            Expect(TokenCategory.OPENB);
            stmt_list();
            Expect(TokenCategory.CLOSEB);
        }
        
        public void case_list()
        {
            while(CurrentToken.TokenCategory.CASE)
            {
                Expect(TokenCategory.CASE);
                lit_list(); 
                Expect(TokenCategory.COLON);
                stmt_list();
            }
        }
        
        public void lit_list()
        {
            if(CurrentToken.TokenCategory.FALSE)
            {
                Expect(TokenCategory.FALSE);
            }
            else if(CurrentToken.TokenCategory.TRUE)
            {
                Expect(TokenCategory.TRUE);
            }
            else if(CurrentToken.TokenCategory.CHARACTER)
            {
                Expect(TokenCategory.CHARACTER);
            }
            else if(CurrentToken.TokenCategory.INTEGER)
            {
                Expect(TokenCategory.INTEGER);
            }
            else if(CurrentToken.TokenCategory.BINARYINT)
            {
                Expect(TokenCategory.BINARYINT);
            }
            else if(CurrentToken.TokenCategory.OCTALINT)
            {
                Expect(TokenCategory.OCTALINT);
            }
            else if(CurrentToken.TokenCategory.HEXAINT)
            {
                Expect(TokenCategory.HEXAINT);
            }
            else
            {
                throw new SyntaxError(firstOfOperator, 
                                      tokenStream.Current);//hay que arreglar estos diccionarios al final
            }
            lit_list_cont();
        }
        
        public void lit_list_cont(){
            while(CurrentToken.TokenCategory.COMA)
            {
            Expect(TokenCategory.COMA);
            lit_list();
            }
        }
        
        public void lit_simple(){
            switch (CurrentToken){
                case TokenCategory.BOOL:
                    Expect(TokenCategory.BOOL);
                    break;
                case TokenCategory.INT:
                    Expect(TokenCategory.INT);
                    break;
                case TokenCategory.CHAR:
                    Expect(TokenCategory.CHAR);
                    break;
                default:
                    throw new SyntaxError(firstOfSimpleExpression, 
                                      tokenStream.Current);
            }
        }
        
        public void default_(){
            Expect(TokenCategory.DEFAULT);
            Expect(TokenCategory.COLON);
            stmt_list();
        }
        
        public void expr(){
            expr_cond();
        }
        
        public void expr_cond(){
            expr_or();
            while(CurrentToken.TokenCategory.INTERROGATION)
            {
                Expect(TokenCategory.INTERROGATION);
                expr();
                Expect(TokenCategory.COLON);
                expr();
            }
        }
        
        public void expr_or()
        {
            expr_and();
            while(CurrentToken.TokenCategory.OR)
            {
                Expect(TokenCategory.OR);
                expr_and();
            }
        }
        public void expr_and()
        {
            expr_comp();
            while(CurrentToken.TokenCategory.AND)
            {
                Expect(TokenCategory.AND);
                expr_comp(); 
            }
        }
        
        public void expr_comp()
        {
            expr_rel();
            while(CurrentToken.TokenCategory.EQUAL || CurrentToken.TokenCategory.NOTEQUAL)
            {
                op_comp();
                expr_rel();
            }
        }
        
        public void expr_rel()
        {
            expr_bit_or();
            while(CurrentToken.TokenCategory.LESSEQUAL || CurrentToken.TokenCategory.MOREEQUAL ||CurrentToken.TokenCategory.MORE||CurrentToken.TokenCategory.LESS)
            {
                op_rel();
                expr_bit_or();
                
            }
        }
         public void expr_bit_or()
        {
            expr_bit_and();
            while(CurrentToken.TokenCategory.ORR || CurrentToken.TokenCategory.POWERR)
            {
                op_bit_or();
                expr_bit_and();
            }
        }
         public void expr_bit_and()
        {
            expr_bit_shift();
            while(CurrentToken.TokenCategory.ANDD)
            {
                Expect(TokenCategory.ANDD);
                expr_bit_shift();
            }
        }
        
         public void expr_bit_shift()
        {
            expr_add();
            while(CurrentToken.TokenCategory.SHIFTL||CurrentToken.TokenCategory.SHIFTR||CurrentToken.TokenCategory.SHIFTT)
            {
                op_bit_shift();
                expr_add();
            }
        }
        public void expr_add()
        {
            expr_mul();
            while(CurrentToken.TokenCategory.PLUS||CurrentToken.TokenCategory.MINUS)
            {
                op_add();
                expr_mul();
            }
        }
         public void expr_mul()
        {
            expr_pow();
            while(CurrentToken.TokenCategory.TIMES||CurrentToken.TokenCategory.DIV)
            {
                op_mul();
                expr_pow();
            }
        }
        public void expr_pow()
        {
            expr_unary();
            while(CurrentToken.TokenCategory.POWERR)
            {
                expr_unary();
                Expect(TokenCategory.POWERR);
            }
        }
         public void expr_unary()
        {
            expr_primary();
            while(CurrentToken.TokenCategory.PLUS||CurrentToken.TokenCategory.MINUS||CurrentToken.TokenCategory.EXCLAMATION||CurrentToken.TokenCategory.FLOW)
            {
                expr_primary();
            }
        }
        
       
        
        public void op_comp(){
            if(CurrentToken.TokenCategory.EQUAL){
                Expect(TokenCategory.EQUAL);
            }
            else if (CurrentToken.TokenCategory.NOTEQUAL){
                Expect.TokenCategory.NOTEQUAL;
            }
        }
        
        public void op_rel(){
            switch(CurrentToken){
                case TokenCategory.LESS:
                    Expect(TokenCategory.LESS);
                    break;
                case TokenCategory.LESSEQUAL:
                    Expect(TokenCategory.LESSEQUAL);
                    break;
                case TokenCategory.MORE:
                    Expect(TokenCategory.MORE);
                    break;
                case TokenCategory.MOREEQUAL:
                    Expect(TokenCategory.MOREEQUAL);
                    break;
                default:
                    throw new SyntaxError(firstOfSimpleExpression,tokenStream.Current);
            }
        }
        
        public void op_bit_or(){
            if(CurrentToken.TokenCategory.ORR){
                Expect(TokenCategory.ORR);
            }
            else if(CurrentToken.TokenCategory.POWER){
                Expect(TokenCategory.POWER);
            }
        }
        
        public void op_bit_shift(){
            if(CurrentToken.TokenCategory.SHIFTL){
                Expect(TokenCategory.SHIFTL);
            }
            else if(CurrentToken.TokenCategory.SHIFTR){
                Expect(TokenCategory.SHIFTR);
            }
            else if(CurrentToken.TokenCategory.SHIFTT){
                Expect(TokenCategory.SHIFTT);
            }
        }
        
        public void opp_add(){
            if(CurrentToken.TokenCategory.PLUS){
                Expect(TokenCategory.PLUS);
            }
            else if(CurrentToken.TokenCategory.MINUS){
                Expect(TokenCategory.MINUS);
            }
        }

        
        public void op_mul(){
            if(CurrentToken.TokenCategory.TIMES){
                Expect(TokenCategory.TIMES);
            }
            else if(CurrentToken.TokenCategory.DIV){
                Expect(TokenCategory.DIV);
            }
        }
        
        public void op_unary(){
            switch(CurrentToken){
                case TokenCategory.PLUS:
                    Expect(TokenCategory.PLUS);
                    break;
                case TokenCategory.MINUS:
                    Expect(TokenCategory.MINUS);
                    break;
                case TokenCategory.EXCLAMATION:
                    Expect(TokenCategory.EXCLAMATION);
                    break;
                case TokenCategory.FLOW:
                    Expect(TokenCategory.FLOW);
                    break;
                default:
                    throw new SyntaxError(firstOfSimpleExpression,tokenStream.Current);
            }
        }
        
        public void expr_primary()
        {
            if(currentToken.TokenCategory.IDENTIFIER)
            {
                Expect(TokenCategory.IDENTIFIER);
                if(CurrentToken.TokenCategory.OPENP) {
                    Expect(TokenCategory.OPENP); 
                    expr_list();
                    Expect(TokenCategory.CLOSEP); 
                    Expect(TokenCategory.SEMICOLON);
                }
            }
            else if(CurrentToken.TokenCategory.FALSE)
            {
                Expect(TokenCategory.FALSE);
            }
            else if(CurrentToken.TokenCategory.TRUE)
            {
                Expect(TokenCategory.TRUE);
            }
            else if(CurrentToken.TokenCategory.CHARACTER)
            {
                Expect(TokenCategory.CHARACTER);
            }
            else if(CurrentToken.TokenCategory.INTEGER)
            {
                Expect(TokenCategory.INTEGER);
            }
            else if(CurrentToken.TokenCategory.BINARYINT)
            {
                Expect(TokenCategory.BINARYINT);
            }
            else if(CurrentToken.TokenCategory.OCTALINT)
            {
                Expect(TokenCategory.OCTALINT);
            }
            else if(CurrentToken.TokenCategory.HEXAINT)
            {
                Expect(TokenCategory.HEXAINT);
            }
             else if(CurrentToken.TokenCategory.STRING)
            {
                Expect(TokenCategory.STRING);
            }
            else if(CurrentToken.TokenCategory.OPENP)
            {
                Expect(TokenCategory.OPENP);
                expr();
                Expect(TokenCategory.CLOSEP);
            }
            else if(CurrentToken.TokenCategory.OPENB)
            {
              array_list();
            }
        }
        
        
        public void array_list(){
                Expect(TokenCategory.OPENB);
                lit_list();
                Expect(TokenCategory.CLOSEP);
        }
        
    }
}
