
// Authors:
//           A01374527 Luis Daniel Rivero Sosa
//           A01372915 Rodrigo Benavides Villanueva 
//           A01374356 Javier Antonio García Roque 

/*EBNF:

‹program›	→	‹def-list›
‹def-list›	→	(‹def›)*
‹def›	→	‹var-def›
‹def›	→	‹fun-def›
‹var-def›	→	var ‹var-list› ;
‹var-list›	→	‹id-list›
‹id-list›	→	‹id› ‹id-list-cont›
‹id-list-cont›	→	(, ‹id› ‹id-list-cont›)*
‹fun-def›	→	‹id› ( ‹param-list› ) { ‹var-def-list› ‹stmt-list› }
‹param-list›	→	(‹id-list›)+
‹var-def-list›	→	(‹var-def-list› ‹var-def›)*
(‹stmt-list›	→	‹stmt-list› ‹stmt›)*
‹stmt›	→	‹stmt-assign›
‹stmt›	→	‹stmt-incr›
‹stmt›	→	‹stmt-decr›
‹stmt›	→	‹stmt-fun-call›
‹stmt›	→	‹stmt-if›
‹stmt›	→	‹stmt-loop›
‹stmt›	→	‹stmt-break›
‹stmt›	→	‹stmt-return›
‹stmt›	→	‹stmt-empty›
‹stmt-assign›	→	‹id› = ‹expr› ;
‹stmt-incr›	→	‹id› ++ ;
‹stmt-decr›	→	‹id› −− ;
‹stmt-fun-call›	→	‹fun-call› ;
‹fun-call›	→	‹id› ( ‹expr-list› )
‹expr-list›	→	(‹expr› ‹expr-list-cont›)*
‹expr-list-cont›	→	(, ‹expr› ‹expr-list-cont›)*
‹stmt-if›	→	if ( ‹expr› ) { ‹stmt-list› } ‹else-if-list› ‹else›
‹else-if-list›	→	(‹else-if-list› elseif ( ‹expr› ) { ‹stmt-list› })*
‹else›	→	(else { ‹stmt-list› })?
‹stmt-loop›	→	loop { ‹stmt-list› }
‹stmt-break›	→	break ;
‹stmt-return›	→	return ‹expr› ;
‹stmt-empty›	→	;
‹expr›	→	‹expr-or›
‹expr-or›	→	|| ‹expr-and›
‹expr-or›	→	‹expr-and›
‹expr-and›	→	&& ‹expr-comp›
‹expr-and›	→	‹expr-comp›
‹expr-comp›	→	‹op-comp› ‹expr-rel›
‹expr-comp›	→	‹expr-rel›
‹op-comp›	→	==
‹op-comp›	→	!=
‹expr-rel›	→	‹op-rel› ‹expr-add›
‹expr-rel›	→	‹expr-add›
‹op-rel›	→	<
‹op-rel›	→	<=
‹op-rel›	→	>
‹op-rel›	→	>=
‹expr-add›	→	‹op-add› ‹expr-mul›
‹expr-add›	→	‹expr-mul›
‹op-add›	→	+
‹op-add›	→	−
‹expr-mul›	→	‹op-mul› ‹expr-unary›
‹expr-mul›	→	‹expr-unary›
‹op-mul›	→	*
‹op-mul›	→	/
‹op-mul›	→	%
‹expr-unary›	→	‹op-unary› ‹expr-unary›
‹expr-unary›	→	‹expr-primary›
‹op-unary›	→	+
‹op-unary›	→	−
‹op-unary›	→	!
‹expr-primary›	→	‹id›
‹expr-primary›	→	‹fun-call›
‹expr-primary›	→	‹array›
‹expr-primary›	→	‹lit›
‹expr-primary›	→	( ‹expr› )
‹array›	→	[ ‹expr-list› ]
‹lit›	→	‹lit-int›
‹lit›	→	‹lit-char›
‹lit›	→	‹lit-str›

*/

using System;
using System.Collections.Generic;

namespace DeepLingo {

    class Parser {      

        
        
        static readonly ISet<TokenCategory> firstOfStatement =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.IF,
                TokenCategory.LOOP,
                TokenCategory.BREAK,
                TokenCategory.RETURN,
                TokenCategory.SEMICOLON
            };
            
        static readonly ISet<TokenCategory> firstOfAssing =
            new HashSet<TokenCategory>() {
                TokenCategory.PARENTHESIS_OPEN,
                TokenCategory.LESS,
                TokenCategory.INCREMENTATION,
                TokenCategory.ASSIGN
            };
        //define las funciones con palabras reservadas
        static readonly ISet<TokenCategory> firstOfFunc =
            new HashSet<TokenCategory>(){
                TokenCategory.PRINT_I,
                TokenCategory.PRINT_C,
                TokenCategory.PRINT_S,
                TokenCategory.PRINT_LN,
                TokenCategory.READ_I,
                TokenCategory.READ_S,
                TokenCategory.NEW,
                TokenCategory.SIZE,
                TokenCategory.ADD,
                TokenCategory.GET,
                TokenCategory.SET
            };
            
        static readonly ISet<TokenCategory> firstOfExpression = 
            new HashSet<TokenCategory>(){
                TokenCategory.IDENTIFIER,
                TokenCategory.SQUARE_BRACKET_OPEN,
                TokenCategory.INT_LITERAL,
                TokenCategory.CHAR_LITERAL,
                TokenCategory.STR_LITERAL,
                TokenCategory.PARENTHESIS_OPEN
            };

        static readonly ISet<TokenCategory> firstOfOperator =
            new HashSet<TokenCategory>() {
                TokenCategory.AND,
                TokenCategory.OR,
                TokenCategory.EQUAL_TO,
                TokenCategory.NOT_EQUAL_TO,
                TokenCategory.LESS,
                TokenCategory.LESS_THAN_OR_EQUAL_TO,
                TokenCategory.GREATER_THAN,
                TokenCategory.GREATER_THAN_OR_EQUAL_TO,
                TokenCategory.PLUS,
                TokenCategory.NEG,
                TokenCategory.MUL,
                TokenCategory.DIV,
                TokenCategory.REMAINDER,
                TokenCategory.NOT
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

        public void Program() {  
            //metodo que invoca manda a llamar toda la funcionalidad
            Def_List();
            //Espera el final del archivo - EOF
            Expect(TokenCategory.EOF);
        }
        //Replica la funcionalidad de Def()
        public void Def_List(){
            //manda a llamar Var_Def()
            while(CurrentToken == TokenCategory.VAR){//si var existe, se sigue ejecutando
                Var_Def();
            }
            while(CurrentToken == TokenCategory.IDENTIFIER){//si se encuentra un ID
                Expect(TokenCategory.IDENTIFIER);//Consume un ID
                Expect(TokenCategory.PARENTHESIS_OPEN);//consume un (
                Var_List();//manda a llamar una lista de parametros, sustituye a param_list()
                Expect(TokenCategory.PARENTHESIS_CLOSE);//consume un )
                Expect(TokenCategory.BRACE_OPEN);//consume un {
                //define una lista de variables, sustituye a var-def-list()
                while(CurrentToken == TokenCategory.VAR){//si var existe, se sigue ejecutando
                    Var_Def();
                }
                Stmt_List();
                Expect(TokenCategory.BRACE_CLOSE);//consume un }
            }
        }
        
        public void Stmt_List(){
            //se ejecuta siempre y cuando encuentre un statement
            while(firstOfStatement.Contains(CurrentToken) || firstOfFunc.Contains(CurrentToken)){
                //si el token actual se encuentra en la lista firstOfStatement
                if(firstOfStatement.Contains(CurrentToken)){
                    //posibles casos:
                    //TokenCategory.IDENTIFIER,TokenCategory.IF,TokenCategory.LOOP,
                    //TokenCategory.BREAK,TokenCategory.RETURN,TokenCategory.SEMICOLON
                    switch (CurrentToken) {
                        //si se encuentra un ID
                        case TokenCategory.IDENTIFIER:
                            Expect(TokenCategory.IDENTIFIER);//consume un ID
                            //Identifica que hay despues del ID
                            switch(CurrentToken){
                                //Si hay un signo de =
                                case TokenCategory.ASSIGN:
                                    Expect(TokenCategory.ASSIGN);//consume un signo de =
                                    Expr();
                                    Expect(TokenCategory.SEMICOLON);//consume un ;
                                    break;
                                //si se encuentra un ++
                                case TokenCategory.INCREMENTATION:
                                    Expect(TokenCategory.INCREMENTATION);//consume un ++
                                    Expect(TokenCategory.SEMICOLON);//consume un ;
                                    break;
                                //SI se encuentra un --
                                case TokenCategory.DECREMENTATION:
                                    Expect(TokenCategory.DECREMENTATION);//consume un --
                                    Expect(TokenCategory.SEMICOLON);//consume un ;
                                    break;
                                case TokenCategory.PARENTHESIS_OPEN:
                                    Fun_Call();//consume los parametros de la funcion
                                    Expect(TokenCategory.SEMICOLON);//consume un ;
                                    break;
                                default:
                                    throw new SyntaxError(firstOfAssing, 
                                          tokenStream.Current);
                            }
                            break;
                        case TokenCategory.IF: //equivalente a stmt-if()
                            If();//consume un if
                            break;
                        case TokenCategory.LOOP:
                            Loop();//consume un Loop
                            break;
                        case TokenCategory.BREAK:
                            Break();//consume el break
                            break;
                        case TokenCategory.RETURN:
                            Return();
                            break;
                        case TokenCategory.SEMICOLON://equivalente a stmt-empty()
                            Expect(TokenCategory.SEMICOLON);//consume un ;
                            break;
                    }
                }
                //Si el token actual se encuentra en la lista firstOfFunc
                else if(firstOfFunc.Contains(CurrentToken)){
                    Fun_Palabra_Reservada();//consume la funcion con la palabra reservada
                }
            }
        }
        //consume una funcion con una papabra reservada
        public void Fun_Palabra_Reservada(){
            //posibles casos:
            //TokenCategory.PRINT_I,TokenCategory.PRINT_C,TokenCategory.PRINT_S,
            //TokenCategory.PRINT_LN,TokenCategory.READ_I,TokenCategory.READ_S,TokenCategory.NEW,
            //TokenCategory.SIZE,TokenCategory.ADD,TokenCategory.GET,TokenCategory.SET
            Expect(CurrentToken);//consume el toquen actual, es alguno de los posibles casos
            Fun_Call();//consume los parametros de la funcion
            Expect(TokenCategory.SEMICOLON);//consume un ;
        }
        //funcion que consume un return
        public void Return(){
            Expect(TokenCategory.RETURN);//consume un "return"
            Expr();//consume una expresion
            Expect(TokenCategory.SEMICOLON);//consume un ;
        }
        //funcion que consume un break
        public void Break(){
            Expect(TokenCategory.BREAK);//consume un "break"
            Expect(TokenCategory.SEMICOLON);//consume un ;
        }
        //funcion que consume un loop
        public void Loop(){
            Expect(TokenCategory.LOOP);//consume un "loop"
            Expect(TokenCategory.BRACE_OPEN);//CONSUME {
            Stmt_List();//consume una lista de stmt
            Expect(TokenCategory.BRACE_CLOSE);//consume un }
        }
        //funcion que consume un if
        public void If(){
            Expect(TokenCategory.IF);//consume un if
            Expect(TokenCategory.PARENTHESIS_OPEN);//CONSUME UN (
            Expr();//consume una expresion
            Expect(TokenCategory.PARENTHESIS_CLOSE);//consume un )
            Expect(TokenCategory.BRACE_OPEN);//CONSUME {
            Stmt_List();//consume una lista de stmt
            Expect(TokenCategory.BRACE_CLOSE);//consume un }
            //consume todos los "elseif"
            while(CurrentToken == TokenCategory.ELSEIF){
                Expect(TokenCategory.ELSEIF);//consume un "elseif"
                Expect(TokenCategory.PARENTHESIS_OPEN);//CONSUME UN (
                Expr();//consume una expresion
                Expect(TokenCategory.PARENTHESIS_CLOSE);//consume un )
                Expect(TokenCategory.BRACE_OPEN);//CONSUME {
                Stmt_List();//consume una lista de stmt
                Expect(TokenCategory.BRACE_CLOSE);//consume un }
            }
            if(CurrentToken == TokenCategory.ELSE){
                Expect(TokenCategory.ELSE);//consume un "else"
                Expect(TokenCategory.BRACE_OPEN);//CONSUME {
                Stmt_List();//consume una lista de stmt
                Expect(TokenCategory.BRACE_CLOSE);//consume un }
            }
        }
        //funcion que valida los parametros de una funcion
        public void Fun_Call(){
            Expect(TokenCategory.PARENTHESIS_OPEN);//consume un parentesis abierto
            Expr_List();
            Expect(TokenCategory.PARENTHESIS_CLOSE);//consume un parentesis cerrado
        }
        
        public void Expr_List(){
            //ciclo que revisa las expresiones dentro de una funcion
            //Error de !0 se puede arreglar aqui
            if(firstOfExpression.Contains(CurrentToken) || CurrentToken == TokenCategory.NEG ||
            CurrentToken == TokenCategory.PLUS || CurrentToken == TokenCategory.NOT || 
            firstOfFunc.Contains(CurrentToken)){
                Expr();//consume una expresion
                //SIEMPRE QUE ENCUENTRE UNA ,
                while(CurrentToken == TokenCategory.COMMA){
                    Expect(TokenCategory.COMMA);//CONSUME UNA ,
                    Expr();//consume una expresion
                }
            }
        }
        
        //Se encarga de revisar la declaracion de variables
        public void Var_Def(){
            Expect(TokenCategory.VAR);//consume el identificador var
            Var_List();
            Expect(TokenCategory.SEMICOLON);//consume un ;
        }
        //Consume una lista de variables separadas por comas
        public void Var_List(){
            if(CurrentToken == TokenCategory.IDENTIFIER){
                Expect(TokenCategory.IDENTIFIER);//consume un ID
                while(CurrentToken == TokenCategory.COMMA){
                    Expect(TokenCategory.COMMA);//consume una coma
                    Expect(TokenCategory.IDENTIFIER);//consume un ID
                    //Var_List();//llamada recursiva, caso base donde ya no hay más comas
                    //para separar las variables
                }
            }
            
        }
        
        //funcion que valida si hay una expresion
        public void Expr(){
            Expr_Or();
            while(firstOfOperator.Contains(CurrentToken)){
                Expr_Or();
            }
        }
        //funcion que verifica un or
        public void Expr_Or(){
            if(CurrentToken == TokenCategory.OR){
                Expect(TokenCategory.OR);
            }
            Expr_And();
        }
        //funcion que verifica un and
        public void Expr_And(){
            if(CurrentToken == TokenCategory.AND){
                Expect(TokenCategory.AND);
            }
            Expr_Comp();
        }
        public void Expr_Comp(){
            //sustituye a op_comp()
            if(CurrentToken == TokenCategory.EQUAL_TO || CurrentToken == TokenCategory.NOT_EQUAL_TO){
                switch(CurrentToken){
                    case TokenCategory.EQUAL_TO:
                        Expect(TokenCategory.EQUAL_TO);
                        break;
                    case TokenCategory.NOT_EQUAL_TO:
                        Expect(TokenCategory.NOT_EQUAL_TO);
                        break;
                }
            }
            Expr_Rel();
        }
        public void Expr_Rel(){
            //sustituye a op_rel
            if(CurrentToken == TokenCategory.GREATER_THAN || 
            CurrentToken == TokenCategory.GREATER_THAN_OR_EQUAL_TO ||
            CurrentToken == TokenCategory.LESS ||
            CurrentToken == TokenCategory.LESS_THAN_OR_EQUAL_TO){
                switch(CurrentToken){
                    case TokenCategory.GREATER_THAN:
                        Expect(TokenCategory.GREATER_THAN);
                        break;
                    case TokenCategory.GREATER_THAN_OR_EQUAL_TO:
                        Expect(TokenCategory.GREATER_THAN_OR_EQUAL_TO);
                        break;
                    case TokenCategory.LESS:
                        Expect(TokenCategory.LESS);
                        break;
                    case TokenCategory.LESS_THAN_OR_EQUAL_TO:
                        Expect(TokenCategory.LESS_THAN_OR_EQUAL_TO);
                        break;
                }
            }
            Expr_Add();
        }
        
        public void Expr_Add(){
            if(CurrentToken == TokenCategory.NEG || CurrentToken == TokenCategory.PLUS){
                switch(CurrentToken){
                    case TokenCategory.NEG:
                        Expect(TokenCategory.NEG);
                        break;
                    case TokenCategory.PLUS:
                        Expect(TokenCategory.PLUS);
                        break;
                }
            }
            Expr_Mul();
        }
        public void Expr_Mul(){
            if(CurrentToken == TokenCategory.MUL || CurrentToken == TokenCategory.DIV ||
            CurrentToken == TokenCategory.REMAINDER){
                switch(CurrentToken){
                    case TokenCategory.MUL:
                        Expect(TokenCategory.MUL);
                        break;
                    case TokenCategory.DIV:
                        Expect(TokenCategory.DIV);
                        break;
                    case TokenCategory.REMAINDER:
                        Expect(TokenCategory.REMAINDER);
                        break;
                }
            }
            Expr_Unary();
        }
        public void Expr_Unary(){
            while(CurrentToken == TokenCategory.NEG || CurrentToken == TokenCategory.PLUS ||
            CurrentToken == TokenCategory.NOT){
                switch(CurrentToken){
                    case TokenCategory.NEG:
                        Expect(TokenCategory.NEG);
                        break;
                    case TokenCategory.PLUS:
                        Expect(TokenCategory.PLUS);
                        break;
                    case TokenCategory.NOT:
                        Expect(TokenCategory.NOT);
                        break;
                }
            }
            Expr_Primary();
        }
        //es el caso base
        public void Expr_Primary(){
            if(firstOfFunc.Contains(CurrentToken)){
                Expect(CurrentToken);//consume el toquen actual, es alguno de los posibles casos
                Fun_Call();//consume los parametros de la funcion
            }
            else{
                switch (CurrentToken){
            //posibles casos:
            //TokenCategory.IDENTIFIER,TokenCategory.SQUARE_BRACKET_OPEN,TokenCategory.INT_LITERAL,
            //TokenCategory.CHAR_LITERAL,TokenCategory.STR_LITERAL,TokenCategory.PARENTHESIS_OPEN
                    case TokenCategory.IDENTIFIER:
                        Expect(TokenCategory.IDENTIFIER);
                        if(CurrentToken == TokenCategory.PARENTHESIS_OPEN){
                            Fun_Call();//consume los parametros de la funcion
                        }
                        break;
                    case TokenCategory.SQUARE_BRACKET_OPEN://ARRAY
                        Expect(TokenCategory.SQUARE_BRACKET_OPEN);
                        Expr_List();
                        Expect(TokenCategory.SQUARE_BRACKET_CLOSE);
                        break;
                    case TokenCategory.INT_LITERAL:
                        Expect(TokenCategory.INT_LITERAL);
                        break;
                    case TokenCategory.CHAR_LITERAL:
                        Expect(TokenCategory.CHAR_LITERAL);
                        break;
                    case TokenCategory.STR_LITERAL:
                        Expect(TokenCategory.STR_LITERAL);
                        break;
                    case TokenCategory.PARENTHESIS_OPEN:
                        Expect(TokenCategory.PARENTHESIS_OPEN);
                        Expr();
                        Expect(TokenCategory.PARENTHESIS_CLOSE);
                        break;
                        
                    default:
                        throw new SyntaxError(firstOfExpression, 
                                          tokenStream.Current);
                }
            }
        }
        
        
        

    }
}
