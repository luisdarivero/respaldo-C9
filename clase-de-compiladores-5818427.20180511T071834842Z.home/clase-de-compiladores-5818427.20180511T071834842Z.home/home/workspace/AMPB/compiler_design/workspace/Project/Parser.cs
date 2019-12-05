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

       

       /* static readonly ISet<TokenCategory> firstOfOperator =
            new HashSet<TokenCategory>() {
                TokenCategory.EOF
            };*/

        static readonly ISet<TokenCategory> repeats =
            new HashSet<TokenCategory>() {
                TokenCategory.PLUS,
                TokenCategory.MINUS,
                TokenCategory.EQUAL,
                TokenCategory.ASIGNATION,
                TokenCategory.POWER,
                TokenCategory.POWERR,
                TokenCategory.FLOW,
                TokenCategory.LESS,
                TokenCategory.LESSEQUAL,
                TokenCategory.MOREEQUAL,
                TokenCategory.NOTEQUAL,
                TokenCategory.ORR,
                TokenCategory.LESSEQUAL,
                TokenCategory.MOREEQUAL,
                TokenCategory.NOTEQUAL,
                TokenCategory.SHIFTL,
                TokenCategory.SHIFTR,
                TokenCategory.SHIFTT,
                TokenCategory.TIMES,
                TokenCategory.DIV,
                TokenCategory.REM,
                TokenCategory.EXCLAMATION
            };
        static readonly ISet<TokenCategory> firstOfSimpleExpression =
            new HashSet<TokenCategory>() {
                TokenCategory.INTEGER,
                TokenCategory.TRUE,
                TokenCategory.CHARACTER,
                TokenCategory.FALSE,
                TokenCategory.HEXAINT,
                TokenCategory.BINARYINT,
                TokenCategory.OCTALINT
            };
            
            static readonly ISet<TokenCategory> firstOfSimpleExpression1 =
            new HashSet<TokenCategory>() {
                TokenCategory.INTEGER,
                TokenCategory.TRUE,
                TokenCategory.CHARACTER,
                TokenCategory.FALSE,
                TokenCategory.HEXAINT,
                TokenCategory.BINARYINT,
                TokenCategory.OCTALINT,
                TokenCategory.IDENTIFIER,
                TokenCategory.STRING,
                TokenCategory.OPENB,
                TokenCategory.OPENP,
                TokenCategory.IDENTIFIER,
                TokenCategory.OPENB,
                TokenCategory.OPENP
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

// export  variable=blabla        
// aqui empieza lo nuestro **********************************************************
        public Node Program() 
        {  // //Console.WriteLine("Program");
            var program_ = new Program_();
            if (CurrentToken != TokenCategory.EOF) {
               program_.Add(def_list());
            }
            Expect(TokenCategory.EOF);
            //Console.WriteLine("aqui4");
            return program_;
        }
    
        public Node def_list()
        {
            //Console.WriteLine("def_list");
            var def_list = new Def_list_();
            while(CurrentToken != TokenCategory.EOF){
                //Console.WriteLine("aqui1");
                def_list.Add(def());
                //Console.WriteLine("aqui2");
            }
            //Console.WriteLine("aqui3");
            return def_list;
        }
         public Node def()
        {
            //Console.WriteLine("Def(");
            //var def_ = new Node();
            if(CurrentToken == TokenCategory.VAR)
            {
                 // //Console.WriteLine("Def1");
                return var_def();
            }
            else if(CurrentToken == TokenCategory.IDENTIFIER)
            {
                  //Console.WriteLine("Def2");
                 return  fun_def();
                 //Console.WriteLine("Def3");
            }
            else
            {
                  //Console.WriteLine("De3");
                //Console.WriteLine(CurrentToken);
                 throw new SyntaxError(TokenCategory.IDENTIFIER,tokenStream.Current);
            }
           // return def_;
        }
         public Node var_def()
        {
            //Console.WriteLine("var_def");
            var Var = new Var_();
            var r = Expect(TokenCategory.VAR);
            Var.AnchorToken = r;
            Var.Add(var_list());
            //Console.WriteLine("semi1");
            Expect(TokenCategory.SEMICOLON);
            return Var;
        }
        public Node var_list()
        {
            //Console.WriteLine("var_list");
            return id_list();
        }
        public Node id_list()
        {
            //Console.WriteLine("id_list");
            var id_list = new Id_list_();
            var id = new Id_();
            var r = Expect(TokenCategory.IDENTIFIER);
            id_list.Add(id);
            id.AnchorToken = r;
            while(CurrentToken == TokenCategory.COMA){
                id_list.Add(id_list_cont());
            }
            return id_list;
        }
        
        public Node id_list_cont()
        {
             //Console.Write("id_list_cont");
            var id = new Id_();
            Expect(TokenCategory.COMA);
            var r =Expect(TokenCategory.IDENTIFIER);
            id.AnchorToken = r;
            return id;
        }
        
        public Node fun_def()
        {
             //Console.WriteLine("fun_def");
            var fun = new Fun_();
            var r = Expect(TokenCategory.IDENTIFIER);
            fun.AnchorToken = r;
            Expect(TokenCategory.OPENP);
            if(CurrentToken != TokenCategory.CLOSEP){
             fun.Add(param_list());
         }
         Expect(TokenCategory.CLOSEP);
         Expect(TokenCategory.OPENB);
         if(CurrentToken == TokenCategory.VAR){
             //Console.WriteLine("fun_def2");
            fun.Add(var_def_list());
         }
         //Console.WriteLine("aqui estoy");
         fun.Add(stmt_list());
          //Console.WriteLine("fun_def3");
         Expect(TokenCategory.CLOSEB);
         //Console.WriteLine("fun_def4");
         return fun;
        }
         public Node param_list()
        {
             //Console.WriteLine("param_list");
            return id_list();
        }
        public Node var_def_list()
        {
             //Console.WriteLine("var_def_list");
            var var_def_list = new Var_def_list_();
            while(CurrentToken == TokenCategory.VAR){
                var_def_list.Add(var_def());
            }
            return var_def_list;
        }
        
        public Node stmt_list()
        {
             //Console.WriteLine("stmt_list");
            var stmt_list = new Stmt_list_();
            while(CurrentToken == TokenCategory.IDENTIFIER 
            ||CurrentToken == TokenCategory.IF 
            ||CurrentToken == TokenCategory.SWITCH
            ||CurrentToken == TokenCategory.WHILE
            ||CurrentToken == TokenCategory.FOR
            ||CurrentToken == TokenCategory.DO
            ||CurrentToken == TokenCategory.BREAK
            ||CurrentToken == TokenCategory.CONTINUE
            ||CurrentToken == TokenCategory.RETURN
            ||CurrentToken == TokenCategory.SEMICOLON){
                stmt_list.Add(stmt_());
                //Console.WriteLine("stmt_list2");
            }
            return stmt_list;
        }

        public Node stmt_(){
             //Console.WriteLine("stmt");
            Token r;
            switch (CurrentToken) {

                case TokenCategory.IF:
                    //Console.WriteLine("stmt1");
                    Node If = new If_();
                    r = Expect(TokenCategory.IF);
                    If.AnchorToken = r;
                    Expect(TokenCategory.OPENP);
                    If.Add(expr());
                    Expect(TokenCategory.CLOSEP);
                    Expect(TokenCategory.OPENB);
                    If.Add(stmt_list());
                     //Console.WriteLine("CLOSEB3");
                    Expect(TokenCategory.CLOSEB); 
                    If.Add(else_if_list());
                    If.Add(else_());
                    return If;
                case TokenCategory.SWITCH:
                    //Console.WriteLine("stmt2");
                    var Switch = new Switch_();
                     r = Expect(TokenCategory.SWITCH);
                    Switch.AnchorToken = r;
                    Expect(TokenCategory.OPENP);
                    Switch.Add(expr());
                    Expect(TokenCategory.CLOSEP);
                    Expect(TokenCategory.OPENB);
                    Switch.Add(case_list());
                    Switch.Add(default_());
                     //Console.WriteLine("CLOSEB4");
                    Expect(TokenCategory.CLOSEB);
                    return Switch;
    
                case TokenCategory.WHILE:
                    //Console.WriteLine("stmt3");
                    var While = new While_();
                    r = Expect(TokenCategory.WHILE);
                    While.AnchorToken = r;
                    Expect(TokenCategory.OPENP);
                    While.Add(expr());
                    Expect(TokenCategory.CLOSEP);
                    Expect(TokenCategory.OPENB);
                    While.Add(stmt_list());
                    //Console.WriteLine("CLOSEB11");
                    Expect(TokenCategory.CLOSEB);
                    return While;
        
        /*       case TokenCategory.OPENP:
                    Expect(TokenCategory.OPENP);
                    expr();
                    Expect(TokenCategory.CLOSEP);
                    break;*/
            
    
                case TokenCategory.DO: //OYE AQUI NO ES DO_WHILE???
                    //Console.WriteLine("stmt4");
                    var Do = new Do_();
                    r = Expect(TokenCategory.DO);
                    Do.AnchorToken = r;
                    Expect(TokenCategory.OPENB);
                    Do.Add(stmt_list());
                    //Console.WriteLine("CLOSEB5");
                    Expect(TokenCategory.CLOSEB);
                    var While_ = new While_();
                    var m = Expect(TokenCategory.WHILE);
                    While_.AnchorToken = m;
                    Expect(TokenCategory.OPENP);
                    While_.Add(expr());
                    Do.Add(While_);
                    Expect(TokenCategory.CLOSEP);
                    //Console.WriteLine("semi4");
                    Expect(TokenCategory.SEMICOLON);
                    return Do;
    
                case TokenCategory.FOR:
                    //Console.WriteLine("stmt5");
                    var For = new For_();
                     r = Expect(TokenCategory.FOR);
                    For.AnchorToken = r;
                    Expect(TokenCategory.OPENP);
                    var Id = new Id_();
                    var n = Expect(TokenCategory.IDENTIFIER);
                    Id.AnchorToken = n;
                    For.Add(Id);
                    Expect(TokenCategory.IN);
                    For.Add(expr());
                    Expect(TokenCategory.CLOSEP);
                    Expect(TokenCategory.OPENB);
                    For.Add(stmt_list());
                     //Console.WriteLine("CLOSEB6");
                    Expect(TokenCategory.CLOSEB);
                    return For;
                
                case TokenCategory.BREAK:
                    //Console.WriteLine("stmt6");
                    var Break = new Break_();
                     r = Expect(TokenCategory.BREAK);
                    Break.AnchorToken = r;
                    //Console.WriteLine("semi5");
                    Expect(TokenCategory.SEMICOLON);
                    return Break;
                
                case TokenCategory.CONTINUE:
                    //Console.WriteLine("stmt7");
                    var Continue = new Continue_();
                    r = Expect(TokenCategory.CONTINUE);
                    Continue.AnchorToken = r;
                    //Console.WriteLine("semi6");
                    Expect(TokenCategory.SEMICOLON);
                    return Continue;
                    
                case TokenCategory.RETURN:
                    //Console.WriteLine("stmt8");
                    var Return = new Return_();
                    r =Expect(TokenCategory.RETURN);
                    Return.AnchorToken = r;
                    Return.Add(expr());
                    //Console.WriteLine("semi7");
                    Expect(TokenCategory.SEMICOLON);
                    return Return;
                    
                case TokenCategory.SEMICOLON:
                    //Console.WriteLine("stmt9");
                    var empty = new Empty_();
                    //Console.WriteLine("semi8");
                    r = Expect(TokenCategory.SEMICOLON);
                    empty.AnchorToken = r;
                    return empty;
                    
                case TokenCategory.IDENTIFIER:
                    //Console.WriteLine("stmt10");
                    r = Expect(TokenCategory.IDENTIFIER);
                    if(CurrentToken == TokenCategory.ASIGNATION){
                        //Console.WriteLine("stmt11");
                        var id = new Id_();
                        id.AnchorToken = r;
                        Expect(TokenCategory.ASIGNATION); 
                        id.Add(expr());
                        //Console.WriteLine("semi2");
                        Expect(TokenCategory.SEMICOLON);
                        return id;
                    }
                     else if(CurrentToken == TokenCategory.OPENP) {
                         //Console.WriteLine("stmt12");
                        var Fun = new Fun_();
                        Fun.AnchorToken = r;
                        Expect(TokenCategory.OPENP); 
                        if(CurrentToken != TokenCategory.CLOSEP){
                            //Console.WriteLine("stmt13");////////////MODIFICADO//////////////////////////////////////////
                            Fun.Add(expr_list());
                        }
                        //Console.WriteLine("stmt14");
                        Expect(TokenCategory.CLOSEP); 
                        //Console.WriteLine("semi3");
                        Expect(TokenCategory.SEMICOLON);
                        return Fun;
                    }
                    else {
                         throw new SyntaxError(TokenCategory.EQUAL,tokenStream.Current);
                    }
                    
                default:
                    throw new SyntaxError(TokenCategory.IDENTIFIER,tokenStream.Current);
                }
        }
        public Node expr_list()
        {
            //Console.WriteLine("expr_list");
            var expr_list = new Expr_list_();
            expr_list.Add(expr());
            if(CurrentToken == TokenCategory.COMA){
                 //Console.WriteLine("expr_list_cont");
                while(CurrentToken == TokenCategory.COMA)
                {
                    Expect(TokenCategory.COMA);
                    expr_list.Add(expr());
                }
            }
            return expr_list;
        } 
        
//weoifjooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooowef
        public Node expr_list_cont(Node expr_list){
             //Console.WriteLine("expr_list_cont");
            while(CurrentToken == TokenCategory.COMA){
            Expect(TokenCategory.COMA);
            expr_list.Add(expr());
            expr_list_cont(expr_list);
            }
            return expr_list;
        }
//jsjdnfJDDDDDDDDDDDDDDDDDDDDDDDDWIOFEWFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
        
        public Node else_if_list(){
             //Console.WriteLine("else_if_list");
            var Else_ = new Else_();
            while(CurrentToken == TokenCategory.ELSE)
            {
                var r = Expect(TokenCategory.ELSE);
                Else_.AnchorToken = r;
                if(CurrentToken == TokenCategory.IF)
                {
                    var If = new If_();
                    var m = Expect(TokenCategory.IF);
                    If.AnchorToken = m;
                    Else_.Add(If);
                    Expect(TokenCategory.OPENP);
                    If.Add(expr());
                    Expect(TokenCategory.CLOSEP);
                    Expect(TokenCategory.OPENB);
                    If.Add(stmt_list());
                 //Console.WriteLine("CLOSEB7");
                    Expect(TokenCategory.CLOSEB);
                }
                else if(CurrentToken == TokenCategory.OPENB)
                {
                     //Console.WriteLine("else_");
                    if(CurrentToken == TokenCategory.OPENB){
                        Expect(TokenCategory.OPENB);
                        Else_.Add(stmt_list());
                        //Console.WriteLine("CLOSEB9");
                        Expect(TokenCategory.CLOSEB);
                    }
                }
            }
            return Else_;
        }
//nejnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnd
        public Node else_(){
            Node Else = new Else_();
            if(CurrentToken == TokenCategory.OPENB){
                Expect(TokenCategory.OPENB);
                Else.Add(stmt_list());
             //Console.WriteLine("CLOSEB9");
                Expect(TokenCategory.CLOSEB);
            }
            return Else;
        }
//kdcmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
        public Node case_list()
        {
             //Console.WriteLine("case_list");
            var Case = new Case_();
            while(CurrentToken == TokenCategory.CASE)
            {
                var r = Expect(TokenCategory.CASE);
                Case.AnchorToken = r;
                Case.Add(lit_list()); 
                Expect(TokenCategory.COLON);
                Case.Add(stmt_list());
            }
            return Case;
        }
        
        public Node lit_list()
        {
             //Console.WriteLine("lit_list");
            //Node lit_list = new Lit_list_();
            Node lit = new Lit_list_();
            if(CurrentToken == TokenCategory.FALSE)
            {
                lit = new False_();
                var r = Expect(TokenCategory.FALSE);
                lit.AnchorToken = r;
            }
            else if(CurrentToken == TokenCategory.TRUE)
            {
                lit = new True_();
                var r = Expect(TokenCategory.TRUE);
                lit.AnchorToken = r;
            }
            else if(CurrentToken == TokenCategory.CHARACTER)   //y si le ponemos que espera un valor
            {
                lit = new Character_();
                var r =Expect(TokenCategory.CHARACTER);
                lit.AnchorToken = r;
            }
            else if(CurrentToken == TokenCategory.INTEGER)
            {
                lit = new Integer_();
                var r = Expect(TokenCategory.INTEGER);
                lit.AnchorToken = r;
            }
            else if(CurrentToken == TokenCategory.BINARYINT)
            {
                lit = new Integer_();
                var r = Expect(TokenCategory.BINARYINT);
                lit.AnchorToken = r;
            }
            else if(CurrentToken == TokenCategory.OCTALINT)
            {
                lit = new Integer_();
                var r = Expect(TokenCategory.OCTALINT);
                lit.AnchorToken = r;
            }
            else if(CurrentToken == TokenCategory.HEXAINT)
            {
                lit = new Integer_();
                var r = Expect(TokenCategory.HEXAINT);
                lit.AnchorToken = r;
            }
            else
            {
                throw new SyntaxError(firstOfSimpleExpression, tokenStream.Current);//hay que arreglar estos diccionarios al final
            }
            //lit_list.Add(lit);
            while(CurrentToken == TokenCategory.COMA)
            {
                Expect(TokenCategory.COMA);
                lit.Add(lit_list());
            }
            return lit;
        }
        
        public Node lit_list_cont(Node lit_list_){
             //Console.WriteLine("lit_list_cont");
            while(CurrentToken == TokenCategory.COMA)
            {
                Expect(TokenCategory.COMA);
                lit_list_.Add(lit_list());
            }
            return lit_list_;
        }
        
        public Node lit_simple(){
             //Console.WriteLine("lit_simple");
            Node True = null;
            Token r;
            switch (CurrentToken){
               case TokenCategory.TRUE:
                    True = new True_();
                    r = Expect(TokenCategory.TRUE);
                    True.AnchorToken = r;
                    return True;
                case TokenCategory.FALSE:
                    True = new  False_();
                    r = Expect(TokenCategory.FALSE);
                    True.AnchorToken = r;
                    return True;
                case TokenCategory.INTEGER:
                    True = new Integer_();
                    r = Expect(TokenCategory.INTEGER);
                    True.AnchorToken = r;
                    return True;
                case TokenCategory.CHARACTER:
                    True = new Character_();
                    r = Expect(TokenCategory.CHARACTER);
                    True.AnchorToken = r;
                    return True;
                default:
                    throw new SyntaxError(TokenCategory.CHARACTER, tokenStream.Current);
            }
        }
        
        public Node default_(){
             //Console.WriteLine("default");
            var Default = new Default_();
            var r = Expect(TokenCategory.DEFAULT);
            Default.AnchorToken = r;
            Expect(TokenCategory.COLON);
            Default.Add(stmt_list());
            return Default;
        }
        
        public Node expr(){
             //Console.WriteLine("expr");
            var expr = new Expr_();
            expr.Add(expr_cond());
            return expr;
        }
        
        public Node expr_cond(){
             //Console.WriteLine("expr_cond");
            var expr_or_ = expr_or();
            while(CurrentToken == TokenCategory.INTERROGATION)
            {
                //Console.WriteLine("while1");
                var interrogation = new Interrogation_();
                var r = Expect(TokenCategory.INTERROGATION);
                interrogation.AnchorToken = r; 
                interrogation.Add(expr());
                var Colon = new Colon_(); 
                var m = Expect(TokenCategory.COLON);
                Colon.AnchorToken = m;
                Colon.Add(expr());
                interrogation.Add(Colon);
                return interrogation;
            }
            return expr_or_;
        }
        
        public Node expr_or()
        {
            //Console.WriteLine("expr_or");
            Node expr_and_ = expr_and();
            if(CurrentToken == TokenCategory.OR)
            {
                //Console.WriteLine("while2");
                var Or = new Or_();
                var r = Expect(TokenCategory.OR);
                Or.AnchorToken = r;
                Or.Add(expr_and_);
                Or.Add(expr_and());
                return Or;
            }
            return expr_and_;
        }
        public Node expr_and()
        {
             //Console.WriteLine("expr_and");
            //Console.WriteLine("expr_and");
            Node expr_comp_ = expr_comp();
            while(CurrentToken == TokenCategory.AND)
            {
                //Console.WriteLine("while3");
                var And = new And_();
                var r = Expect(TokenCategory.AND);
                And.AnchorToken = r;
                And.Add(expr_comp_);
                And.Add(expr_comp());
                return And;
            }
            return expr_comp_;
        }
        
        public Node expr_comp()
        {
            //Console.WriteLine("expr_comp");
            //Console.WriteLine("expr_comp");
            Node expr_rel_ = expr_rel();
            if(CurrentToken == TokenCategory.EQUAL || CurrentToken == TokenCategory.NOTEQUAL)
            {
                //Console.WriteLine("while4");
                var comp = new Comp_();
                var r = op_comp();
                comp.AnchorToken = r;
                comp.Add(expr_rel_);
                comp.Add(expr_rel());
                return comp;
            }
            return expr_rel_;
        }
        
        public Node expr_rel()
        {
             //Console.WriteLine("expr_rel");
            //Console.WriteLine("expr_rel");
            Node expr_bit_or_ = expr_bit_or();
            if(CurrentToken == TokenCategory.LESSEQUAL || CurrentToken == TokenCategory.MOREEQUAL ||CurrentToken == TokenCategory.MORE||CurrentToken == TokenCategory.LESS)
            {
                //Console.WriteLine("while5");
                var rel = new Rel_();
                var r = op_rel();
                rel.AnchorToken = r;
                rel.Add(expr_bit_or_);
                rel.Add(expr_bit_or());
                return rel;
            }
            return expr_bit_or_;
        }
         public Node expr_bit_or()
        {
             //Console.WriteLine("expr_bit_or");
            //Console.WriteLine("expr_bit_or");
            Node expr_bit_and_ = expr_bit_and();
            if(CurrentToken == TokenCategory.ORR || CurrentToken == TokenCategory.POWERR)
            {
                //Console.WriteLine("while6");
                var bit_or = new Bit_or_();
                var r = op_bit_or();
                bit_or.AnchorToken = r;
                bit_or.Add(expr_bit_and_);
                bit_or.Add(expr_bit_and());
                return bit_or;
            }
            return expr_bit_and_;
        }
         public Node expr_bit_and()
        {
            //Console.WriteLine("expr_bit_and");
            Node expr_bit_shift_ = expr_bit_shift();
            if(CurrentToken == TokenCategory.ANDD)
            {
                //Console.WriteLine("while7");
                var bit_and = new Bit_and_();
                var r = Expect(TokenCategory.ANDD);
                bit_and.AnchorToken = r;
                bit_and.Add(expr_bit_shift_);
                bit_and.Add(expr_bit_shift());
                return bit_and;
            }
            return expr_bit_shift_;
        }
        
         public Node expr_bit_shift()
        {
            //Console.WriteLine("expr_bit_shift");
            Node expr_add_ = expr_add();
            if(CurrentToken == TokenCategory.SHIFTL||CurrentToken == TokenCategory.SHIFTR||CurrentToken == TokenCategory.SHIFTT)
            {
                //Console.WriteLine("while8");
                var bit_shift = new Bit_shift_();
                var r = op_bit_shift();
                bit_shift.AnchorToken = r;
                bit_shift.Add(expr_add_);
                bit_shift.Add(expr_add());
                return bit_shift;
            }
            return expr_add_;
        }
        public Node expr_add()
        {
            //Console.WriteLine("expr_add");
            Node expr_mul_ = expr_mul();
            if(CurrentToken == TokenCategory.PLUS||CurrentToken == TokenCategory.MINUS)
            {
                //Console.WriteLine("while9");
                var add = new Add_();
                var r = op_add();
                add.AnchorToken = r;
                add.Add(expr_mul_);
                add.Add(expr_mul());
                return add;
            }
            return expr_mul_;
        }
         public Node expr_mul()
        {
            //Console.WriteLine("expr_mul");
            Node expr_pow_ = expr_pow();
            if(CurrentToken == TokenCategory.TIMES||CurrentToken == TokenCategory.DIV||CurrentToken == TokenCategory.REM)
            {
                //Console.WriteLine("while10");
                var mul = new Mul_();
                var r = op_mul();
                mul.AnchorToken = r;
                mul.Add(expr_pow_);
                mul.Add(expr_pow());
                return mul;
            }
            return expr_pow_;
        }
        public Node expr_pow()
        {
             //Console.WriteLine("expr_pow");
            Node expr_unary_ = expr_unary();
            if(CurrentToken == TokenCategory.POWERR)
            {
                //Console.WriteLine("while11");
                var pow = new  Pow_();
                var r = Expect(TokenCategory.POWERR);
                pow.AnchorToken = r;
                pow.Add(expr_unary());
                pow.Add(expr_unary_);
                return pow;
            }
            return expr_unary_;
        }
         public Node expr_unary()
        {
            //Console.WriteLine("expr_unitary");
            Node expr_primary_ = expr_primary();
            if(CurrentToken == TokenCategory.PLUS||CurrentToken == TokenCategory.MINUS||CurrentToken == TokenCategory.EXCLAMATION||CurrentToken == TokenCategory.FLOW)
            {
                //Console.WriteLine("while12");
                var unary = new Unary_();
                var r = op_unary();
                unary.AnchorToken = r;
                unary.Add(expr_primary_);
                unary.Add(expr_primary());
                return unary;
            }
            return expr_primary_;
        }
        
       
        
        public Token op_comp(){
             //Console.WriteLine("op_cond");
            if(CurrentToken == TokenCategory.EQUAL){
                var r = Expect(TokenCategory.EQUAL);
                repeat();
                return r;
            }
            else if (CurrentToken == TokenCategory.NOTEQUAL){
                var r = Expect(TokenCategory.NOTEQUAL);
                repeat();
                return r;
            }
            return null;
        }
        
        public Token op_rel(){
             //Console.WriteLine("op_rel");
            Token r;
            switch(CurrentToken){
                case TokenCategory.LESS:
                     r = Expect(TokenCategory.LESS);
                    repeat();
                    return r;
                case TokenCategory.LESSEQUAL:
                    r = Expect(TokenCategory.LESSEQUAL);
                    repeat();
                    return r;
                case TokenCategory.MORE:
                    r = Expect(TokenCategory.MORE);
                    repeat();
                    return r;
                case TokenCategory.MOREEQUAL:
                    r = Expect(TokenCategory.MOREEQUAL);
                    repeat();
                    return r;
                default:
                    throw new SyntaxError(TokenCategory.MOREEQUAL,tokenStream.Current);
            }
        }
        
        public Token op_bit_or(){
            //Console.WriteLine("op_bit_or");
            if(CurrentToken == TokenCategory.ORR){
                var r = Expect(TokenCategory.ORR);
                repeat();
                return r;
            }
            else if(CurrentToken == TokenCategory.POWER){
                var r = Expect(TokenCategory.POWER);
                repeat();
                return r;
            }
            return null;
        }
        
        public Token op_bit_shift(){
            //Console.WriteLine("op_bit_shift");
            if(CurrentToken == TokenCategory.SHIFTL){
                var r = Expect(TokenCategory.SHIFTL);
                repeat();
                return r;
            }
            else if(CurrentToken == TokenCategory.SHIFTR){
                var r = Expect(TokenCategory.SHIFTR);
                repeat();
                return r;
            }
            else if(CurrentToken == TokenCategory.SHIFTT){
                var r = Expect(TokenCategory.SHIFTT);
                repeat();
                return r;
            }
            return null;
        }
        
        public Token op_add(){
            //Console.WriteLine("op_add");
            if(CurrentToken == TokenCategory.PLUS){
                var r = Expect(TokenCategory.PLUS);
                repeat();
                return r;
            }
            else if(CurrentToken == TokenCategory.MINUS){
                var r = Expect(TokenCategory.MINUS);
                repeat();
                return r;
            }
            return null;
        }

        
        public Token op_mul(){
            //Console.WriteLine("op_mul");
            if(CurrentToken == TokenCategory.TIMES){
                var r = Expect(TokenCategory.TIMES);
                repeat();
                return r;
            }
            else if(CurrentToken == TokenCategory.DIV){
                var r = Expect(TokenCategory.DIV);
                repeat();
                return r;
            }
            else if(CurrentToken == TokenCategory.REM){
                var r = Expect(TokenCategory.REM);
                repeat();
                return r;
            }
            return null;
        }
        
        public Token op_unary(){
            //Console.WriteLine("op_unitary");
            Token r;
            switch(CurrentToken){
                case TokenCategory.PLUS:
                    r = Expect(TokenCategory.PLUS);
                    repeat();
                    return r;
                case TokenCategory.MINUS:
                    r = Expect(TokenCategory.MINUS);
                    repeat();
                    return r;
                case TokenCategory.EXCLAMATION:
                    r = Expect(TokenCategory.EXCLAMATION);
                    repeat();
                    return r;
                case TokenCategory.FLOW:
                    r = Expect(TokenCategory.FLOW);
                    repeat();
                    return r;
                default:
                    throw new SyntaxError(TokenCategory.FLOW,tokenStream.Current);
            }
        }
        public void repeat()
        {
            if(repeats.Contains(CurrentToken))
            {
                throw new SyntaxError(firstOfSimpleExpression, tokenStream.Current);
            }
            if(!firstOfSimpleExpression1.Contains(CurrentToken))
            {
                throw new SyntaxError(firstOfSimpleExpression1, tokenStream.Current);
            }
        }
        public Node expr_primary()
        {
            //Console.WriteLine("expr_primary");
            if(CurrentToken == TokenCategory.IDENTIFIER)
            {
                //Console.WriteLine("expr_primary1");
                var id = new Id_();
                var r = Expect(TokenCategory.IDENTIFIER);
                id.AnchorToken = r;
                if(CurrentToken == TokenCategory.OPENP) {
                     //Console.WriteLine("expr_primary2");
                    Expect(TokenCategory.OPENP); 
                    if(CurrentToken != TokenCategory.CLOSEP){ /////////////////////////////MODIFICADO///////////////////////////
                        id.Add(expr_list());
                    }
                    Expect(TokenCategory.CLOSEP); 
                    //Console.WriteLine("semi9");
                    //Expect(TokenCategory.SEMICOLON);
                }
                return id;
            }
            else if(CurrentToken == TokenCategory.FALSE)
            {
                 //Console.WriteLine("expr_primary3");
                var False = new False_();
                var r = Expect(TokenCategory.FALSE);
                False.AnchorToken = r;
                return False;
            }
            else if(CurrentToken == TokenCategory.TRUE)
            {
                 //Console.WriteLine("expr_primary4");
                var False = new True_();
                var r = Expect(TokenCategory.TRUE);
                False.AnchorToken = r;
                return False;
            }
            else if(CurrentToken == TokenCategory.CHARACTER)
            {
                 //Console.WriteLine("expr_primary5");
                var False = new Character_();
                var r = Expect(TokenCategory.CHARACTER);
                False.AnchorToken = r;
                return False;
            }
            else if(CurrentToken == TokenCategory.INTEGER)
            {
                 //Console.WriteLine("expr_primary6");
                var False = new Integer_();
                var r = Expect(TokenCategory.INTEGER);
                False.AnchorToken = r;
                return False;
            }
            else if(CurrentToken == TokenCategory.BINARYINT)
            {
                 //Console.WriteLine("expr_primary7");
                var False = new Integer_();
                var r = Expect(TokenCategory.BINARYINT);
                False.AnchorToken = r;
               return False;
            }
            else if(CurrentToken == TokenCategory.OCTALINT)
            {
                 //Console.WriteLine("expr_primary8");
                var False = new Integer_();
                var r = Expect(TokenCategory.OCTALINT);
                False.AnchorToken = r;
                return False;
            }
            else if(CurrentToken == TokenCategory.HEXAINT)
            {
                 //Console.WriteLine("expr_primary9");
                var False = new Integer_();
                var r = Expect(TokenCategory.HEXAINT);
                False.AnchorToken = r;
                return False;
            }
             else if(CurrentToken == TokenCategory.STRING)
            {
                 //Console.WriteLine("expr_primary10");
                var False = new String_();
                var r = Expect(TokenCategory.STRING);
                False.AnchorToken = r;
                return False;
            }
            else if(CurrentToken == TokenCategory.OPENP)
            {
                 //Console.WriteLine("expr_primary11");
                var openp = new Openp_();
                var r = Expect(TokenCategory.OPENP);
                openp.AnchorToken = r;
                //Console.WriteLine("CURRRRRRRREEEEENT" + CurrentToken);
                if(CurrentToken != TokenCategory.CLOSEP)
                {
                    openp.Add(expr());
                }
                
                Expect(TokenCategory.CLOSEP);
                return openp;
            }
            else if(CurrentToken == TokenCategory.OPENB)
            {
                 //Console.WriteLine("expr_primary12");
                var r =  array_list();
                return r;
            }
            else if(CurrentToken == TokenCategory.EXCLAMATION)
            {
                 //Console.WriteLine("expr_primary13");
                var exclamation = new Exclamation_();
                var r =Expect(TokenCategory.EXCLAMATION);
                exclamation.AnchorToken = r;
                if(CurrentToken == TokenCategory.IDENTIFIER){
                    var id = new Id_();
                    var m = Expect(TokenCategory.IDENTIFIER);
                    id.AnchorToken = m;
                    exclamation.Add(id);
                }
                return exclamation;
            }
            else
            {
                throw new SyntaxError(firstOfSimpleExpression, tokenStream.Current);
            }
        }
        
        
        public Node array_list(){
             //Console.WriteLine("array_list");
                var array = new Array_();
                var r = Expect(TokenCategory.OPENB);
                array.AnchorToken = r ;
                while(CurrentToken != TokenCategory.CLOSEB )
                array.Add(lit_list());
                //Console.WriteLine("CLOSEB1");
                Expect(TokenCategory.CLOSEB);
                return array;
        }
        
    }
}
