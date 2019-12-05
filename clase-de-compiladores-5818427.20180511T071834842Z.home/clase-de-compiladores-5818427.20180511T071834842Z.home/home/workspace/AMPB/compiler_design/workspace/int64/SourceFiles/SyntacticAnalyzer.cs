/*
  Compilador de int64
  -- SyntacticAnalyzer:
        Se encarga de realizar el análisis sintáctico sobre el archivo fuente.

  Copyright (C) 2017 Iram Molina & Diego Trujillo bajo WTFPL.
  ITESM CEM
*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Int64{
    
    class SyntacticAnalyzer{
        
        // Atributos
        IEnumerator<Token> tokenStream;
        
        // Categorías
        static readonly ISet<TokenCategory> firstOfDeclaration =
            new HashSet<TokenCategory>() {
              TokenCategory.VAR,
              TokenCategory.IDENTIFIER
            };

        static readonly ISet<TokenCategory> firstOfStatement =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.IF,
                TokenCategory.SWITCH,
                TokenCategory.WHILE,
                TokenCategory.DO,
                TokenCategory.FOR,
                TokenCategory.BREAK,
                TokenCategory.CONTINUE,
                TokenCategory.RETURN
            };
            

            
        static readonly ISet<TokenCategory> firstOfListContinuation = 
            new HashSet<TokenCategory>() {
              TokenCategory.COMMA
            };
            
        static readonly ISet<TokenCategory> simpleLiterals = 
            new HashSet<TokenCategory>() {
              TokenCategory.TRUE,
              TokenCategory.FALSE,
              TokenCategory.BASE_2_INT_LITERAL,
              TokenCategory.BASE_8_INT_LITERAL,
              TokenCategory.BASE_10_INT_LITERAL,
              TokenCategory.BASE_16_INT_LITERAL,
              TokenCategory.CHARACTER_LITERAL
            };
            
        static readonly ISet<TokenCategory> comparisonOperators = 
            new HashSet<TokenCategory>() {
              TokenCategory.EQUAL,
              TokenCategory.NOT_EQUAL
            };
        
        static readonly ISet<TokenCategory> relationshipOperators = 
            new HashSet<TokenCategory>() {
              TokenCategory.GREATER_THAN,
              TokenCategory.GREATER_OR_EQUAL_THAN,
              TokenCategory.LESS_THAN,
              TokenCategory.LESS_OR_EQUAL_THAN
            };
        static readonly ISet<TokenCategory> orBitOperators = 
            new HashSet<TokenCategory>() {
              TokenCategory.BITWISE_OR,
              TokenCategory.BITWISE_XOR
            };
            
        static readonly ISet<TokenCategory> bitShiftOperators = 
            new HashSet<TokenCategory>() {
              TokenCategory.BIT_SHIFT_RIGHT,
              TokenCategory.BIT_SHIFT_LEFT,
              TokenCategory.DOUBLE_BIT_SHIFT_RIGHT
            };
        
        static readonly ISet<TokenCategory> additionOperators = 
            new HashSet<TokenCategory>() {
              TokenCategory.PLUS,
              TokenCategory.NEGATIVE
            };
            
        static readonly ISet<TokenCategory> multiplicationOperators = 
            new HashSet<TokenCategory>() {
              TokenCategory.MULTIPLY,
              TokenCategory.SLASH,
              TokenCategory.PERCENTAGE
            };
          
        static readonly ISet<TokenCategory> unaryOperators = 
            new HashSet<TokenCategory>() {
              TokenCategory.PLUS,
              TokenCategory.NEGATIVE,
              TokenCategory.EXCLAMATION_MARK,
              TokenCategory.TILDE
            };
            
        static readonly ISet<TokenCategory> firstOfAllLiterals = 
            new HashSet<TokenCategory>() {
              TokenCategory.TRUE,
              TokenCategory.FALSE,
              TokenCategory.BASE_2_INT_LITERAL,
              TokenCategory.BASE_8_INT_LITERAL,
              TokenCategory.BASE_10_INT_LITERAL,
              TokenCategory.BASE_16_INT_LITERAL,
              TokenCategory.CHARACTER_LITERAL,
              TokenCategory.STRING_LITERAL,
              TokenCategory.BRACKETS_OPEN
            };
            

        // Constructor
        public SyntacticAnalyzer(IEnumerator<Token> tokenStream) {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }
        
        // Getters & Setters
        public TokenCategory CurrentToken { get {return tokenStream.Current.Category;} }
        
        // Expect
        public Token Expect(TokenCategory category){
          if(CurrentToken == category){
            Token current = tokenStream.Current;
            tokenStream.MoveNext();
            return current;
          }
          else{
            throw new SyntaxError(category, tokenStream.Current);
          }
        }
        
        // Parse Tree
        public Node Program(){
          
          Node defList = null;
          
          while(CurrentToken != TokenCategory.EOF){
            defList = DefList();
          }
          Expect(TokenCategory.EOF);
          
          return new Program(){
            defList
          };
        }
        
        public Node DefList(){
          var defList = new DefinitionList(){};
          
          while(firstOfDeclaration.Contains(CurrentToken)){
            defList.Add(Def());
          }
          
          return defList;
        }
        
        public Node Def(){
          Node definition = null;
          
          switch(CurrentToken){
            case TokenCategory.VAR:
              definition = VarDef();
              break;
            case TokenCategory.IDENTIFIER:
              definition = FunDef();
              break;
            default:
              throw new SyntaxError(firstOfDeclaration, tokenStream.Current);
          }
          
          return definition;
        }
        
                
        public Node VarDefList(){
          var varDefList =  new VariableDefinitionList();
          while(CurrentToken == TokenCategory.VAR){
            varDefList.Add(VarDef());
          }
          return varDefList;
        }
        
        public Node VarDef(){
          var varDef = new VariableDefinition(){
              AnchorToken = Expect(TokenCategory.VAR)
          };
          varDef.Add(IdList());
          Expect(TokenCategory.SEMICOLON);
          return varDef;
        }
        
        public Node IdList(){
          var idList = new IDList();
          idList.Add(Id());
          
          while(firstOfListContinuation.Contains(CurrentToken)){
            Expect(TokenCategory.COMMA);
            idList.Add(Id());
          }
          
          return idList;
        }
        
        public Node Id(){
          return new Identifier(){
            AnchorToken = Expect(TokenCategory.IDENTIFIER)
          };
        }
        
        public Node Id(Token identifier){
          return new Identifier(){
            AnchorToken = identifier
          };
        }
        
        public Node FunDef(){
          var funDef = new FunctionDefinition(){
            AnchorToken = Expect(TokenCategory.IDENTIFIER)
          };
          
          Expect(TokenCategory.PARENTHESIS_OPEN);
          funDef.Add(ParamList());
          Expect(TokenCategory.PARENTHESIS_CLOSE);
          
          Expect(TokenCategory.BRACKETS_OPEN);
          funDef.Add(VarDefList());
          funDef.Add(StmtList());
          Expect(TokenCategory.BRACKETS_CLOSE);
          
          return funDef;
        }
        
        public Node ParamList(){
          var parameterList =  new ParameterList(){};
          if(CurrentToken != TokenCategory.PARENTHESIS_CLOSE){
            parameterList.Add(IdList());
          }
          return parameterList;
        }

        
        public Node StmtList(){
          var statementList = new StatementList();
          while(firstOfStatement.Contains(CurrentToken)){
            statementList.Add(Stmt());
          }
          return statementList;
        }
        
        public Node Stmt(){
          Node statement = new Statement();
          switch (CurrentToken){
              case TokenCategory.IDENTIFIER:
                statement = StmtIdentifier();
                break;
              case TokenCategory.IF:
                statement = StmtIf();
                break;
              case TokenCategory.SWITCH:
                statement = StmtSwitch();
                break;
              case TokenCategory.WHILE:
                statement = StmtWhile();
                break;
              case TokenCategory.DO:
                statement = StmtDoWhile();
                break;
              case TokenCategory.FOR:
                statement = StmtFor();
                break;
              case TokenCategory.BREAK:
                statement = StmtBreak();
                break;
              case TokenCategory.CONTINUE:
                statement = StmtContinue();
                break;
              case TokenCategory.RETURN:
                statement = StmtReturn();
                break;
              case TokenCategory.SEMICOLON:
                Expect(TokenCategory.SEMICOLON);
                break;
              default:
                throw new SyntaxError(firstOfStatement, tokenStream.Current);
            }
            return statement;
        }
        
        public Node StmtIdentifier(){
          var identifierToken = Expect(TokenCategory.IDENTIFIER);
          Node identifier = new Identifier();
          
          if(CurrentToken == TokenCategory.ASSIGNMENT){
            identifier = new Assignment(){
              AnchorToken = identifierToken
            };
            Expect(TokenCategory.ASSIGNMENT);
            identifier.Add(Expr());
            Expect(TokenCategory.SEMICOLON);
          }
          if(CurrentToken == TokenCategory.PARENTHESIS_OPEN){
            identifier = FunCall(identifierToken);
            Expect(TokenCategory.SEMICOLON);
          }
          
          return identifier;
        }

        
        public Node FunCall(Token identifierToken){
            var functionCall = new FunctionCall(){
              AnchorToken = identifierToken
            };
          
          Expect(TokenCategory.PARENTHESIS_OPEN);
          if(CurrentToken != TokenCategory.PARENTHESIS_CLOSE){
            functionCall.Add(ExprList());
          }
          Expect(TokenCategory.PARENTHESIS_CLOSE);
          
          return functionCall;
        }
        
        public Node StmtIf(){
          var statementIf = new If(){
            AnchorToken = Expect(TokenCategory.IF)
          };
          Expect(TokenCategory.PARENTHESIS_OPEN);
          statementIf.Add(Expr());
          Expect(TokenCategory.PARENTHESIS_CLOSE);
          
          Expect(TokenCategory.BRACKETS_OPEN);
          statementIf.Add(StmtList());
          Expect(TokenCategory.BRACKETS_CLOSE);
          
          if(CurrentToken == TokenCategory.ELSE){
            statementIf.Add(ElseIfList());
          }
          //ElseIfList();
          //Else();
          return statementIf;
        }
        
        public Node ElseIfList(){
          var elseIfList = new ElseIfList();
          
          while(CurrentToken == TokenCategory.ELSE){
            var elseToken = Expect(TokenCategory.ELSE);
            if(CurrentToken == TokenCategory.IF){
              elseIfList.Add(ElseIf(elseToken));
            }
            else if(CurrentToken == TokenCategory.BRACKETS_OPEN){
              elseIfList.Add(Else(elseToken));
            }
          }
          
          return elseIfList;
        }
        
        public Node ElseIf(Token elseToken){
          var statementElseIf = new ElseIf(){
            AnchorToken = elseToken
          };
          Expect(TokenCategory.IF);
          
          Expect(TokenCategory.PARENTHESIS_OPEN);
          statementElseIf.Add(Expr());
          Expect(TokenCategory.PARENTHESIS_CLOSE);
          
          Expect(TokenCategory.BRACKETS_OPEN);
          statementElseIf.Add(StmtList());
          Expect(TokenCategory.BRACKETS_CLOSE);
          
          return statementElseIf;
        }
        
        
        public Node Else(Token elseToken){
          var statementElse = new Else(){
            AnchorToken = elseToken
          };
          Expect(TokenCategory.BRACKETS_OPEN);
          statementElse.Add(StmtList());
          Expect(TokenCategory.BRACKETS_CLOSE);
          return statementElse;
        
        }
        
        public Node StmtSwitch(){
          var statementSwitch =  new Switch(){
            AnchorToken = Expect(TokenCategory.SWITCH)
          };
          
          Expect(TokenCategory.PARENTHESIS_OPEN);
          statementSwitch.Add(Expr());
          Expect(TokenCategory.PARENTHESIS_CLOSE);
          
          Expect(TokenCategory.BRACKETS_OPEN);
          while(CurrentToken == TokenCategory.CASE){
            statementSwitch.Add(Case());
          }
          if(CurrentToken == TokenCategory.DEFAULT){
            statementSwitch.Add(DefaultCase());
          }
          Expect(TokenCategory.BRACKETS_CLOSE);
          
          return statementSwitch;
        }
        
        public Node Case(){ 
          var singleCase = new Case(){
            AnchorToken = Expect(TokenCategory.CASE)
          };
          singleCase.Add(LitList());
          Expect(TokenCategory.COLON);
          singleCase.Add(StmtList());
          return singleCase;
        }
        
        
        
        public Node DefaultCase(){
          var defaultCase = new Default(){
            AnchorToken = Expect(TokenCategory.DEFAULT)
          };
          Expect(TokenCategory.COLON);
          defaultCase.Add(StmtList());
          
          return defaultCase;
        }
        
        public Node StmtWhile(){
          var statementWhile = new While(){
            AnchorToken = Expect(TokenCategory.WHILE)
          };
          
          Expect(TokenCategory.PARENTHESIS_OPEN);
          statementWhile.Add(Expr());
          Expect(TokenCategory.PARENTHESIS_CLOSE);
          
          Expect(TokenCategory.BRACKETS_OPEN);
          statementWhile.Add(StmtList());
          Expect(TokenCategory.BRACKETS_CLOSE);
          
          return statementWhile;
        }
        
        public Node StmtDoWhile(){
          var statementDo = new Do(){
            AnchorToken = Expect(TokenCategory.DO)
          };
          
          Expect(TokenCategory.BRACKETS_OPEN);
          statementDo.Add(StmtList());
          Expect(TokenCategory.BRACKETS_CLOSE);
          
          var statementWhile = new Do_While(){
            AnchorToken = Expect(TokenCategory.WHILE)
          };
          statementDo.Add(statementWhile);
          Expect(TokenCategory.PARENTHESIS_OPEN);
          statementWhile.Add(Expr());
          Expect(TokenCategory.PARENTHESIS_CLOSE);
          Expect(TokenCategory.SEMICOLON);
          
          return statementDo;
          
        }
        
        public Node StmtFor(){
          var statementFor = new For(){
            AnchorToken = Expect(TokenCategory.FOR)
          };
      
          Expect(TokenCategory.PARENTHESIS_OPEN);
          statementFor.Add(Id());
          Expect(TokenCategory.IN);
          statementFor.Add(Expr());
          Expect(TokenCategory.PARENTHESIS_CLOSE);
          
          Expect(TokenCategory.BRACKETS_OPEN);
          statementFor.Add(StmtList());
          Expect(TokenCategory.BRACKETS_CLOSE);
          
          return statementFor;
        }
        
        public Node StmtBreak(){
          var statementBreak =  new Break(){
            AnchorToken = Expect(TokenCategory.BREAK)
          };
          
          Expect(TokenCategory.SEMICOLON);
          
          return statementBreak;
        }
        
        public Node StmtContinue(){
          var statementContinue = new Continue(){
            AnchorToken = Expect(TokenCategory.CONTINUE)
          };
          Expect(TokenCategory.SEMICOLON);
          return statementContinue;
        }
        
        public Node StmtReturn(){
          var statementReturn =  new Return(){
            AnchorToken = Expect(TokenCategory.RETURN)
          };
          statementReturn.Add(Expr());
          Expect(TokenCategory.SEMICOLON);
          return statementReturn;
        }
        
        
        public Node ExprList(){
          var expressionList = new ExpressionList();
          expressionList.Add(Expr());
          while(firstOfListContinuation.Contains(CurrentToken)){
            Expect(TokenCategory.COMMA);
            expressionList.Add(Expr());
          }
          return expressionList;
        }

        public Node Expr(){
          return ExprCond();
        }
        
        public Node ExprCond(){
          Node expressionOr = ExprOr();
          if(CurrentToken == TokenCategory.QUESTION_MARK){
            Node expressionCond = new ShortHandCondition(){
              AnchorToken = Expect(TokenCategory.QUESTION_MARK)
            };
            expressionCond.Add(expressionOr);
            expressionCond.Add(Expr());
            Expect(TokenCategory.COLON);
            expressionCond.Add(Expr());
            
            return expressionCond;
          }
          
          return expressionOr;
        }
        
        public Node ExprOr(){
          Node expressionAnd = ExprAnd();
          if(CurrentToken == TokenCategory.OR){
            Node expressionOr = new Or(){
              AnchorToken = Expect(TokenCategory.OR)
            };
            expressionOr.Add(expressionAnd);
            expressionOr.Add(ExprAnd());
            return expressionOr;
          }
          
          return expressionAnd;
        }
        
        public Node ExprAnd(){
          Node expressionComp  = ExprComp();
          if(CurrentToken == TokenCategory.AND){
            Node expressionAnd = new And(){
              AnchorToken = Expect(TokenCategory.AND)
            };
            expressionAnd.Add(expressionComp);
            expressionAnd.Add(ExprComp());
            return expressionAnd;
          }
          return expressionComp;
        }
        
        public Node ExprComp(){
          Node expressionRel = ExprRel();
          if(comparisonOperators.Contains(CurrentToken)){
            Node expressionComp = new Comparison(){
              AnchorToken = OpComp()
            };
            expressionComp.Add(expressionRel);
            expressionComp.Add(ExprRel());
            return expressionComp;
          }
          return expressionRel;
        }
        
        public Token OpComp(){
          Token operatorComparison = null;
          switch(CurrentToken){
            case TokenCategory.EQUAL:
              operatorComparison = Expect(TokenCategory.EQUAL);
              break;
            case TokenCategory.NOT_EQUAL:
              operatorComparison = Expect(TokenCategory.NOT_EQUAL);
              break;
            default:
              throw new SyntaxError(comparisonOperators, tokenStream.Current);
          }
          return operatorComparison;
        }
        
        public Node ExprRel(){
          Node expressionBitOr= ExprBitOr();
          while(relationshipOperators.Contains(CurrentToken)){
            Node expressionRel = new Relationship(){
              AnchorToken = OpRel()
            };
            expressionRel.Add(expressionBitOr);
            expressionRel.Add(ExprBitOr());
            return expressionRel;
          }
          return expressionBitOr;
        }
        
        public Token OpRel(){
          Token operatorRelationship = null;
          switch(CurrentToken){
            case TokenCategory.GREATER_THAN:
              operatorRelationship = Expect(TokenCategory.GREATER_THAN);
              break;
            case TokenCategory.GREATER_OR_EQUAL_THAN:
              operatorRelationship = Expect(TokenCategory.GREATER_OR_EQUAL_THAN);
              break;
            case TokenCategory.LESS_THAN:
              operatorRelationship = Expect(TokenCategory.LESS_THAN);
              break;
            case TokenCategory.LESS_OR_EQUAL_THAN:
              operatorRelationship = Expect(TokenCategory.LESS_OR_EQUAL_THAN);
              break;
            default:
              throw new SyntaxError(relationshipOperators, tokenStream.Current);
          }
          return operatorRelationship;
        }
        
        public Node ExprBitOr(){
          Node expressionBitAnd = ExprBitAnd();
          while(orBitOperators.Contains(CurrentToken)){
            Node expressionBitOr = new BitwiseOr(){
              AnchorToken = OpBitOr()
            };
            expressionBitOr.Add(expressionBitAnd);
            expressionBitOr.Add(ExprBitAnd());
            return expressionBitOr;
          }
          return expressionBitAnd;
        }
        
        public Token OpBitOr(){
          Token operatorBitOr = null;
          switch(CurrentToken){
            case TokenCategory.BITWISE_OR:
              operatorBitOr = Expect(TokenCategory.BITWISE_OR);
              break;
            case TokenCategory.BITWISE_XOR:
              operatorBitOr = Expect(TokenCategory.BITWISE_XOR);
              break;
            default:
              throw new SyntaxError(orBitOperators, tokenStream.Current);
          }
          return operatorBitOr;
        }
        
        public Node ExprBitAnd(){
          Node expressionBitShift = ExprBitShift();
          while(CurrentToken == TokenCategory.BITWISE_AND){
            Node expressionBitAnd = new BitwiseAnd(){
              AnchorToken = Expect(TokenCategory.BITWISE_AND)
            };
            expressionBitAnd.Add(expressionBitShift);
            expressionBitAnd.Add(ExprBitShift());
            return expressionBitAnd;
          }
          return expressionBitShift;
        }
        
        public Node ExprBitShift(){
          Node expressionAdd = ExprAdd();
          while(bitShiftOperators.Contains(CurrentToken)){
            Node expressionBitShift = new BitShift(){
              AnchorToken = OpBitShift()
            };
            expressionBitShift.Add(expressionAdd);
            expressionBitShift.Add(ExprAdd());
            return expressionBitShift;
          }
          return expressionAdd;
        }
        
        public Token OpBitShift(){
          Token operatorBitShift = null;
          switch(CurrentToken){
            case TokenCategory.BIT_SHIFT_LEFT:
              operatorBitShift = Expect(TokenCategory.BIT_SHIFT_LEFT);
              break;
            case TokenCategory.BIT_SHIFT_RIGHT:
              operatorBitShift = Expect(TokenCategory.BIT_SHIFT_RIGHT);
              break;
            case TokenCategory.DOUBLE_BIT_SHIFT_RIGHT:
              operatorBitShift = Expect(TokenCategory.DOUBLE_BIT_SHIFT_RIGHT);
              break;
            default:
              throw new SyntaxError(bitShiftOperators, tokenStream.Current);
          }
          return operatorBitShift;
        }
        
        public Node ExprAdd(){
          Node expressionMul = ExprMul();
          while(additionOperators.Contains(CurrentToken)){
            Node expressionAdd = new Addition(){
              AnchorToken = OpAdd()
            };
            expressionAdd.Add(expressionMul);
            expressionAdd.Add(ExprMul());
            return expressionAdd;
          }
          return expressionMul;
        }
        
        public Token OpAdd(){
          Token operatorAdd = null;
          switch(CurrentToken){
            case TokenCategory.PLUS:
              operatorAdd = Expect(TokenCategory.PLUS);
              break;
            case TokenCategory.NEGATIVE:
              operatorAdd = Expect(TokenCategory.NEGATIVE);
              break;
            default:
              throw new SyntaxError(additionOperators, tokenStream.Current);
          }
          return operatorAdd;
        }
        
        public Node ExprMul(){
          Node expressionPow = ExprPow();
          while(multiplicationOperators.Contains(CurrentToken)){
            Node expressionMul = new Multiplication(){
              AnchorToken = OpMul()
            };
            expressionMul.Add(expressionPow);
            expressionMul.Add(ExprPow());
            return expressionMul;
          }
          return expressionPow;
        }
        
        public Token OpMul(){
          Token operatorMultiplication = null;
          switch(CurrentToken){
            case TokenCategory.MULTIPLY:
              operatorMultiplication = Expect(TokenCategory.MULTIPLY);
              break;
            case TokenCategory.SLASH:
              operatorMultiplication = Expect(TokenCategory.SLASH);
              break;
            case TokenCategory.PERCENTAGE:
              operatorMultiplication = Expect(TokenCategory.PERCENTAGE);
              break;
            default:
              throw new SyntaxError(multiplicationOperators, tokenStream.Current);
          }
          return operatorMultiplication;
        }
        
        public Node ExprPow(){
          Node expressionUnary = ExprUnary();
          while(CurrentToken == TokenCategory.EXPONENTIATION){
            Node expressionPow = new Pow(){
              AnchorToken = Expect(TokenCategory.EXPONENTIATION)
            };
            expressionPow.Add(expressionUnary);
            expressionPow.Add(ExprUnary());
            return expressionPow;
          }
          return expressionUnary;
        }
        
        public Node ExprUnary(){
          while(unaryOperators.Contains(CurrentToken)){
            Node expressionUnary = new UnaryOperation(){
              AnchorToken = OpUnary()
            };
            expressionUnary.Add(ExprPrimary());
            return expressionUnary;
          }
          return ExprPrimary();
        }
        
        public Token OpUnary(){
          Token operatorUnary = null;
          switch(CurrentToken){
            case TokenCategory.PLUS:
              operatorUnary = Expect(TokenCategory.PLUS);
              break;
            case TokenCategory.NEGATIVE:
              operatorUnary = Expect(TokenCategory.NEGATIVE);
              break;
            case TokenCategory.EXCLAMATION_MARK:
              operatorUnary = Expect(TokenCategory.EXCLAMATION_MARK);
              break;
            case TokenCategory.TILDE:
              operatorUnary = Expect(TokenCategory.TILDE);
              break;
            default:
              throw new SyntaxError(unaryOperators, tokenStream.Current);
          }
          return operatorUnary;
        }
        
        public Node ExprPrimary(){
          Node primaryExpression = null;
          if(CurrentToken == TokenCategory.IDENTIFIER){
            var identifierToken = Expect(TokenCategory.IDENTIFIER);
            if(CurrentToken == TokenCategory.PARENTHESIS_OPEN){
              primaryExpression = FunCall(identifierToken);
            }
            else{
              primaryExpression = Id(identifierToken);
            }
          }
          else if(CurrentToken == TokenCategory.PARENTHESIS_OPEN){
            Expect(TokenCategory.PARENTHESIS_OPEN);
            primaryExpression = Expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
          }
          else if(firstOfAllLiterals.Contains(CurrentToken)){
            if(simpleLiterals.Contains(CurrentToken)){
              primaryExpression = LitSimple();
            }
            else if(CurrentToken == TokenCategory.STRING_LITERAL){
              primaryExpression = LitStr();
            }
            else if(CurrentToken == TokenCategory.BRACKETS_OPEN){
              primaryExpression = ArrayList();
            }
            else{
              throw new SyntaxError(firstOfAllLiterals, tokenStream.Current);
            }
          }
          else{
            throw new SyntaxError(firstOfAllLiterals, tokenStream.Current);
          }
          return primaryExpression;
        }
        
        public Node LitList(){
          var literalList = new LiteralList();
          literalList.Add(LitSimple());
          while(firstOfListContinuation.Contains(CurrentToken)){
            Expect(TokenCategory.COMMA);
            literalList.Add(LitSimple());
          }
          return literalList;
        }
        
        public Node LitSimple(){
          Node simpleLiteral = null;
          switch(CurrentToken){
            case TokenCategory.TRUE:
              simpleLiteral = new True(){
                AnchorToken = Expect(TokenCategory.TRUE)
              };
              break;
            case TokenCategory.FALSE:
              simpleLiteral = new False(){
                AnchorToken = Expect(TokenCategory.FALSE)
              };
              break;
            case TokenCategory.BASE_2_INT_LITERAL:
              simpleLiteral = new Base2IntegerLiteral(){
                AnchorToken = Expect(TokenCategory.BASE_2_INT_LITERAL)
              };
              break;
            case TokenCategory.BASE_8_INT_LITERAL:
              simpleLiteral = new Base8IntegerLiteral(){
                AnchorToken = Expect(TokenCategory.BASE_8_INT_LITERAL)
              };
              break;
            case TokenCategory.BASE_10_INT_LITERAL:
              simpleLiteral = new Base10IntegerLiteral(){
                AnchorToken = Expect(TokenCategory.BASE_10_INT_LITERAL)
              };
              break;
            case TokenCategory.BASE_16_INT_LITERAL:
              simpleLiteral = new Base16IntegerLiteral(){
                AnchorToken = Expect(TokenCategory.BASE_16_INT_LITERAL)
              };
              break;
            case TokenCategory.CHARACTER_LITERAL:
              simpleLiteral = new CharacterLiteral(){
                AnchorToken = Expect(TokenCategory.CHARACTER_LITERAL)
              };
              break;
            default:
              throw new SyntaxError(simpleLiterals, tokenStream.Current);
            
          }
          return simpleLiteral;
        }
        
        public Node LitStr(){
          return new StringLiteral()
          { AnchorToken = Expect(TokenCategory.STRING_LITERAL) };
        }
        
        public Node ArrayList(){
          var arrayListLiteral = new ArrayListLiteral();
          Expect(TokenCategory.BRACKETS_OPEN);
          if(CurrentToken != TokenCategory.BRACKETS_CLOSE){
            arrayListLiteral.Add(LitList());
          }
          Expect(TokenCategory.BRACKETS_CLOSE);
          return arrayListLiteral;
        }
        

        
    }
    
}