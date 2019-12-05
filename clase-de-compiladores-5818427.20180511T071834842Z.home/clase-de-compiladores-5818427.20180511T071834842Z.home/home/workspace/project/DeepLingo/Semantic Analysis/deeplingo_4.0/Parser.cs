using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace DeepLingo {
 
     class Parser{
        static readonly ISet<TokenCategory> firstOfStmt = new HashSet<TokenCategory>(){
            TokenCategory.IDENTIFIER,
            TokenCategory.IF,
            TokenCategory.LOOP,
            TokenCategory.BREAK,
            TokenCategory.RETURN,
            TokenCategory.SEMICOLON
        };
        
        static readonly ISet<TokenCategory> firstOfExprRel = new HashSet<TokenCategory>(){
            TokenCategory.LESS,
            TokenCategory.LESSEQUAL,
            TokenCategory.GREATER,
            TokenCategory.GREATEREQUAL
        };
        
        static readonly ISet<TokenCategory> firstOfExprMul = new HashSet<TokenCategory>(){
            TokenCategory.MULTIPLICATION,
            TokenCategory.MODULO,
            TokenCategory.DIVIDE
        };
        
        static readonly ISet<TokenCategory> firstOfExprUnary = new HashSet<TokenCategory>(){
            TokenCategory.PLUS,
            TokenCategory.MINUS,
            TokenCategory.NOT
        };
        
        static readonly ISet<TokenCategory> firstOfExprPrimary = new HashSet<TokenCategory>(){
            TokenCategory.IDENTIFIER,
            TokenCategory.OPENEDBRACKET,
            TokenCategory.STRING,
            TokenCategory.CHAR,
            TokenCategory.INTLITERAL,
            TokenCategory.OPENEDPAR
        };
        
        static readonly ISet<TokenCategory> firstOfExpr = new HashSet<TokenCategory>(){ 
            TokenCategory.IDENTIFIER,
            TokenCategory.OPENEDBRACKET,
            TokenCategory.STRING,
            TokenCategory.CHAR,
            TokenCategory.INTLITERAL,
            TokenCategory.OPENEDPAR,
            TokenCategory.PLUS,
            TokenCategory.MINUS,
            TokenCategory.NOT,
            TokenCategory.MULTIPLICATION,
            TokenCategory.MODULO,
            TokenCategory.DIVIDE,
            TokenCategory.LESS,
            TokenCategory.LESSEQUAL,
            TokenCategory.GREATER,
            TokenCategory.GREATEREQUAL,
            TokenCategory.NOTEQUALS,
            TokenCategory.EQUALS
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
            } 
            else {
                throw new SyntaxError(category, tokenStream.Current);                
            }
        }
        
        public Node Program(){ 
            var node = new Program();
            while(CurrentToken != TokenCategory.EOF){
                if(CurrentToken == TokenCategory.VAR){
                    node.Add(VarDef());
                }else{
                    node.Add(FunDef());
                }
            }
            Expect(TokenCategory.EOF);
            return node;
        }
        
        
        public Node VarDef(){
            var node = new Var(){
                AnchorToken = Expect(TokenCategory.VAR)
            };
            node.Add(VarList());
            Expect(TokenCategory.SEMICOLON);
            return node;
        }
        
        public Node VarList(){
            var node = new VarList();
            node.Add(new Identifier(){
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            });
            while(CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                node.Add(new Identifier(){
                    AnchorToken = Expect(TokenCategory.IDENTIFIER)
                });
            }
            return node;
        }
        
        public Node FunDef(){
            var node = new FunctionDefinition(){
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            };
            Expect(TokenCategory.OPENEDPAR);
            if(CurrentToken != TokenCategory.CLOSEDPAR){
                node.Add(VarList());   
            }
            Expect(TokenCategory.CLOSEDPAR);
            Expect(TokenCategory.OPENEDCURLY);
            while(CurrentToken == TokenCategory.VAR){
                node.Add(VarDef());
            }
            if(firstOfStmt.Contains(CurrentToken)){
                node.Add(StmtList());
            }
            Expect(TokenCategory.CLOSEDCURLY);
            return node;
        }
        
        public Node ElseIf(){
            var node = new ElseIfList();
            while (CurrentToken == TokenCategory.ELSEIF){
                var node_elseif = new ElseIf();
                Expect(TokenCategory.ELSEIF);
                Expect(TokenCategory.OPENEDPAR);
                node_elseif.Add(Expr());
                Expect(TokenCategory.CLOSEDPAR);
                Expect(TokenCategory.OPENEDCURLY);
                node_elseif.Add(StmtList());
                Expect(TokenCategory.CLOSEDCURLY);
                node.Add(node_elseif);
            }
            return node;
        }
        
        public Node Else(){
            var node = new Else();
            if (CurrentToken == TokenCategory.ELSE){
                Expect(TokenCategory.ELSE);
                Expect(TokenCategory.OPENEDCURLY);
                if(CurrentToken != TokenCategory.CLOSEDCURLY){
                    node.Add(StmtList());    
                }
                Expect(TokenCategory.CLOSEDCURLY);
            }
            return node;
        }
        
        public Node StmtList(){
            var node = new StatementList();
            while (firstOfStmt.Contains(CurrentToken)){
                node.Add(Stmt());
            }
            return node;
        }
        
        public Node If(){
            var node = new If() {
              AnchorToken = Expect(TokenCategory.IF)
            };
            Expect(TokenCategory.OPENEDPAR);
            node.Add(Expr());
            Expect(TokenCategory.CLOSEDPAR);
            Expect(TokenCategory.OPENEDCURLY);
            if(CurrentToken != TokenCategory.CLOSEDCURLY){
                node.Add(StmtList());
            }
            Expect(TokenCategory.CLOSEDCURLY);
            if(CurrentToken == TokenCategory.ELSEIF){
                node.Add(ElseIf());    
            }
            if(CurrentToken == TokenCategory.ELSE){
                node.Add(Else());
            }
            return node;
        }
        
        public Node Loop(){
            var node  = new Loop(){
                AnchorToken = Expect(TokenCategory.LOOP)
            };
            Expect(TokenCategory.OPENEDCURLY);
            if(CurrentToken != TokenCategory.CLOSEDCURLY){
                node.Add(StmtList());   
            }
            Expect(TokenCategory.CLOSEDCURLY);
            return node;
        }
        
        public Node Return(){
            var node = new Return() {
                AnchorToken = Expect(TokenCategory.RETURN)
            };
            node.Add(Expr());
            Expect(TokenCategory.SEMICOLON);
            return node;
        }
        
        public Node Stmt(){
            switch (CurrentToken){
                case TokenCategory.IDENTIFIER:
                    var idToken = Expect(TokenCategory.IDENTIFIER);
                    switch (CurrentToken){
                        case TokenCategory.ASSIGN:
                            Expect(TokenCategory.ASSIGN);
                            var node = new Assignment(){
                                AnchorToken = idToken
                            };
                            node.Add(Expr());
                            Expect(TokenCategory.SEMICOLON);
                            return node;

                        case TokenCategory.INCREMENT:
                            Expect(TokenCategory.INCREMENT);
                            var node_inc = new Increment(){
                                AnchorToken = idToken
                            };
                            Expect(TokenCategory.SEMICOLON);
                            return node_inc;

                        case TokenCategory.DECREMENT:
                            Expect(TokenCategory.DECREMENT);
                            var node_dec = new Decrement(){
                                AnchorToken = idToken
                            };
                            Expect(TokenCategory.SEMICOLON);
                            return node_dec;
                            
                        case TokenCategory.OPENEDPAR:
                            Expect(TokenCategory.OPENEDPAR);
                            var node_func = new FunctionCall(){
                                AnchorToken = idToken
                            };
                            if(CurrentToken != TokenCategory.CLOSEDPAR){
                                node_func.Add(ExprList());   
                            }
                            Expect(TokenCategory.CLOSEDPAR);
                            Expect(TokenCategory.SEMICOLON);
                            return node_func;
                    }
                    break;
                case TokenCategory.IF:
                    return If();

                case TokenCategory.LOOP:
                    return Loop();

                case TokenCategory.BREAK:
                    var node_break = new Break(){
                        AnchorToken = Expect(TokenCategory.BREAK)
                    };
                    Expect(TokenCategory.SEMICOLON);
                    return node_break;

                case TokenCategory.RETURN:
                    return Return();

                case TokenCategory.SEMICOLON:
                    return new StatementList(){
                        AnchorToken = Expect(TokenCategory.SEMICOLON)
                    };

                default:
                    throw new SyntaxError(firstOfStmt,tokenStream.Current);
                    
            }
            throw new SyntaxError(firstOfStmt,tokenStream.Current);
        }
        
        public Node ExprList(){
            var node = new ExpressionList();
            node.Add(Expr());
            while (CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                node.Add(Expr());
            }
            return node;
        }
        
        public Node Expr(){
            var node = ExprAnd();
            while(CurrentToken == TokenCategory.OR){
                var node_or = new Or(){
                    AnchorToken = Expect(TokenCategory.OR)
                };
                node_or.Add(node);
                node_or.Add(ExprAnd());
                node = node_or;
            }
            return node;
        }
        
        public Node ExprAnd(){
            var node = ExprComp();
            while(CurrentToken == TokenCategory.AND){
                var node_and = new And(){
                    AnchorToken = Expect(TokenCategory.AND)
                };
                node_and.Add(node);
                node_and.Add(ExprComp());
                node = node_and;
            }
            return node;
        }
        
        public Node ExprComp(){
            var node = ExprRel();
            while(CurrentToken == TokenCategory.EQUALS || CurrentToken == TokenCategory.NOTEQUALS){
                
                if(CurrentToken == TokenCategory.EQUALS){
                    var node_eq = new EqualTo();
                    node_eq.AnchorToken = Expect(TokenCategory.EQUALS);
                    node_eq.Add(node);
                    node_eq.Add(ExprRel());
                    node = node_eq;
                }else{
                    var node_neq = new NotEqualTo();
                    node_neq.AnchorToken = Expect(TokenCategory.NOTEQUALS);
                    node_neq.Add(node);
                    node_neq.Add(ExprRel());
                    node = node_neq;
                }
            }
            return node;
        }
        
        public Node ExprRel(){
            var node = ExprAdd();
            while (firstOfExprRel.Contains(CurrentToken)){
                
                switch (CurrentToken){
                    case TokenCategory.LESS:
                        var node_less = new Less();
                        node_less.AnchorToken = Expect(TokenCategory.LESS);
                        node_less.Add(node);
                        node_less.Add(ExprAdd());
                        node = node_less;
                        break;

                    case TokenCategory.LESSEQUAL:
                        var node_ltet = new LessThanOrEqualTo();
                        node_ltet.AnchorToken = Expect(TokenCategory.LESSEQUAL);
                        node_ltet.Add(node);
                        node_ltet.Add(ExprAdd());
                        node = node_ltet;
                        break;

                    case TokenCategory.GREATER:
                        var node_gt = new GreaterThan();
                        node_gt.AnchorToken = Expect(TokenCategory.GREATER);
                        node_gt.Add(node);
                        node_gt.Add(ExprAdd());
                        node = node_gt;
                        break;

                    case TokenCategory.GREATEREQUAL:
                        var node_gtet = new GreaterThanOrEqualTo();
                        node_gtet.AnchorToken = Expect(TokenCategory.GREATEREQUAL);
                        node_gtet.Add(node);
                        node_gtet.Add(ExprAdd());
                        node = node_gtet;
                        break;

                    default:
                        throw new SyntaxError(firstOfExprRel,tokenStream.Current);
                }
                
            }
            return node;
        }
        
        public Node ExprAdd(){
            var node = ExprMul();
            while (CurrentToken == TokenCategory.PLUS || CurrentToken == TokenCategory.MINUS){
                
                if(CurrentToken == TokenCategory.MINUS){
                    var node_minus = new Minus();
                    node_minus.AnchorToken = Expect(TokenCategory.MINUS);
                    node_minus.Add(node);
                    node_minus.Add(ExprMul());
                    node = node_minus;
                }else{
                    var node_plus = new Plus();
                    node_plus.AnchorToken = Expect(TokenCategory.PLUS);
                    node_plus.Add(node);
                    node_plus.Add(ExprMul());
                    node = node_plus;
                }
                
            }
            return node;
        }
        
        public Node ExprMul(){
            var node = ExprUnary();
            while (firstOfExprMul.Contains(CurrentToken)){

                switch (CurrentToken){
                    case TokenCategory.MULTIPLICATION:
                        var node_mul = new Mul();
                        node_mul.AnchorToken = Expect(TokenCategory.MULTIPLICATION);
                        node_mul.Add(node);
                        node_mul.Add(ExprUnary());
                        node = node_mul;
                        break;

                    case TokenCategory.MODULO:
                        var node_rem = new Rem();
                        node_rem.AnchorToken = Expect(TokenCategory.MODULO);
                        node_rem.Add(node);
                        node_rem.Add(ExprUnary());
                        node = node_rem;
                        break;

                    case TokenCategory.DIVIDE:
                        var node_div = new Div();
                        node_div.AnchorToken = Expect(TokenCategory.DIVIDE);
                        node_div.Add(node);
                        node_div.Add(ExprUnary());
                        node = node_div;
                        break;

                    default:
                        throw new SyntaxError(firstOfExprMul,tokenStream.Current);
                }
            }
            return node;
        }
        
        public Node ExprUnary(){
            var node = new Unary();
            var temp = node; 
             if (firstOfExprUnary.Contains(CurrentToken)){
                while (firstOfExprUnary.Contains(CurrentToken)){
                    switch (CurrentToken){
                        case TokenCategory.PLUS:
                            temp.AnchorToken = Expect(TokenCategory.PLUS);
                            break;

                        case TokenCategory.MINUS:
                            temp.AnchorToken = Expect(TokenCategory.MINUS);
                            break;

                        case TokenCategory.NOT:
                            temp.AnchorToken = Expect(TokenCategory.NOT);
                            break;

                        default:
                            throw new SyntaxError(firstOfExprUnary,tokenStream.Current);
                    }
                    if (!firstOfExprUnary.Contains(CurrentToken)){
                        temp.Add(ExprPrimary());
                    }else{
                        var newNode = new Unary(); 
                        temp.Add(newNode);
                        temp = newNode;
                    }
                    
                }
                
            }
            else{
                return ExprPrimary(); 
            }
            return node; 
        }
        
        public Node ExprPrimary(){
            switch (CurrentToken){
                case TokenCategory.IDENTIFIER: 
                    var idToken = Expect(TokenCategory.IDENTIFIER);
                    if (CurrentToken == TokenCategory.OPENEDPAR){
                        Expect(TokenCategory.OPENEDPAR);
                        var node_func = new FunctionCall(){
                                AnchorToken = idToken
                        };
                        if(CurrentToken != TokenCategory.CLOSEDPAR){
                            node_func.Add(ExprList());   
                        }
                        Expect(TokenCategory.CLOSEDPAR);
                        return node_func;
                    }else{
                        var node_id =  new Identifier(){
                            AnchorToken = idToken
                        };
                        return node_id;
                    }
                case TokenCategory.OPENEDBRACKET:
                    return Arr();

                case TokenCategory.OPENEDPAR: 
                    Expect(TokenCategory.OPENEDPAR);
                    var node_expr = Expr();
                    Expect(TokenCategory.CLOSEDPAR);
                    return node_expr;
                    
                case TokenCategory.STRING:
                    return new StringLiteral(){
                        AnchorToken = Expect(TokenCategory.STRING)
                    };
    
                case TokenCategory.CHAR:
                    return new CharLiteral(){
                        AnchorToken = Expect(TokenCategory.CHAR)
                    };
    
                case TokenCategory.INTLITERAL:
                    return new IntLiteral(){
                        AnchorToken = Expect(TokenCategory.INTLITERAL)
                    };
                    
                default:
                    throw new SyntaxError(firstOfExprPrimary,tokenStream.Current);
            }
        }
        
        public Node Arr(){
            var node = new Array();
            Expect(TokenCategory.OPENEDBRACKET);
            if(CurrentToken != TokenCategory.CLOSEDBRACKET){
                node.Add(ExprList());
            }
            Expect(TokenCategory.CLOSEDBRACKET);
            return node;
        }
    
    }
}