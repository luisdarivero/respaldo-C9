// This programme is based on the Buttercup's compiler code, provided by Ariel Ortiz.

/* 
    DeepLingo compiler - Specific node subclasses for the AST (Abstract Syntax Tree).
    Date: 12-March-2018
    Authors:
          A01169073 Aldo Reyna
          A01375051 Marina Torres
    File name: SpecificNodes.cs
*/


namespace DeepLingo {    

    class Programme: Node {} //Inherit from Node
    
    class FunctionDef: Node{}
    
    class IdList: Node {}
    
    class ParamList: Node {}
    
    class FunctionCall: Node {}

    class ExprList: Node {}
    
    class Assignment: Node {}
    
    class Inc: Node {}
  
    class Dec: Node {}

    class If: Node {}
    
    class ListStatements: Node {}
    
    class ListElseIf: Node {}
    
    class ElseIf: Node {}
    
    class Loop: Node {}
    
    class Break: Node {}
    
    class Return: Node {}
    
    class And: Node {}
    
    class Or: Node {}
    
    class Identifier: Node {}

    class IntLiteral: Node {}
    
    class CharLiteral: Node {}
    
    class StringLiteral: Node {}
    
    class Less: Node {}
    
    class LessEq: Node {}
    
    class Great: Node {}
    
    class GreatEq: Node {}
    
    class Equals: Node {}
    
    class NotEq: Node {}
    
    class Pos: Node {}

    class Neg: Node {}
    
    class Not: Node {}
    
    class Add: Node {}
    
    class Subs: Node {}

    class Mult: Node {}
    
    class Div: Node {}
    
    class Rem: Node {}
}