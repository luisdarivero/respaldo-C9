/*
  Compilador de int64
  -- LexicalAnalyzer:
        Divide al programa en tokens.

  Copyright (C) 2017 Iram Molina & Diego Trujillo bajo WTFPL.
  ITESM CEM
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Int64 {

    class LexicalAnalyzer {
        
        static readonly Regex regexMatcher = new Regex(
            @"
                  (?<ExprAnd>         [&][&])
                | (?<ExprOr>          [\|][\|])
                
                | (?<Identifier>      [a-zA-Z_]+)
                
                | (?<Base2Int>        0[bB][01]+)
                | (?<Base8Int>        0[oO][0-7]+)
                | (?<Base16Int>       0[xX][0-9a-fA-F]+)
                | (?<Base10Int>       \d+)
                
                | (?<Plus>            [+])
                | (?<Neg>             [-])
                | (?<Exponentiation>  [*][*])
                | (?<Mul>             [*])
                | (?<Slash>           [/])
                | (?<Pertg>           [%])
                
                
                | (?<BitWiseOr>       [\|])
                | (?<BitWiseOr2>      [\^])
                | (?<BitWiseAnd>      [&])
                | (?<BitShiftLeft>    [<][<])
                | (?<BitShiftRight>   [>][>])
                | (?<DBitShiftRight>  [>][>][>])
                
                | (?<Equal>           [=][=])
                | (?<NotEqual>        [!][=])
                | (?<GEQ>             [>][=])
                | (?<LEQ>             [<][=])
                | (?<GT>              [>])
                | (?<LT>              [<])
                
                | (?<Assign>          [=])
                
                | (?<QtnMark>         [\?])
                | (?<ExclMark>        [!])
                | (?<Tilde>           [~])
                
                | (?<ParOpen>         [(])
                | (?<ParClose>        [)])
                | (?<BrackOpen>       [{])
                | (?<BrackClose>      [}])
                | (?<Comma>           [,])
                | (?<Colon>           [:])
                | (?<Semicolon>       [;])
                
                | (?<String>          [\""](.*?[\""].*?|.*?)[\""])
                | (?<Character>       '(.|[\\]|[\\][n]|[\\][r]|[\\][t]|[\\][']|[\\][\""]|[\\][u][a-fA-F0-9]{6})')
                | (?<SingleQuote>     ['])
                | (?<DoubleQuote>     [\""])
                
                | (?<Newline>         \n)
                | (?<WhiteSpace>      \s)
                | (?<Other>           .)
            
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.Multiline);
        
        static readonly IDictionary<string, TokenCategory> keywords = 
            new Dictionary<string, TokenCategory>() {
                {"var", TokenCategory.VAR},
                {"if", TokenCategory.IF},
                {"else", TokenCategory.ELSE},
                {"continue", TokenCategory.CONTINUE},
                {"true", TokenCategory.TRUE},
                {"false", TokenCategory.FALSE},
                {"while", TokenCategory.WHILE},
                {"do", TokenCategory.DO},
                {"for", TokenCategory.FOR},
                {"in", TokenCategory.IN},
                {"switch", TokenCategory.SWITCH},
                {"case", TokenCategory.CASE},
                {"default", TokenCategory.DEFAULT},
                {"break", TokenCategory.BREAK},
                {"return", TokenCategory.RETURN}
            };
            
        static readonly IDictionary<string, TokenCategory> nonKeywords =
            new Dictionary<string, TokenCategory>() {
                {"Assign", TokenCategory.ASSIGNMENT},
                
                {"ExprAnd", TokenCategory.AND},
                {"ExprOr", TokenCategory.OR},
                
                {"Base2Int", TokenCategory.BASE_2_INT_LITERAL},
                {"Base8Int", TokenCategory.BASE_8_INT_LITERAL},
                {"Base10Int", TokenCategory.BASE_10_INT_LITERAL},
                {"Base16Int", TokenCategory.BASE_16_INT_LITERAL},
                
                {"Plus", TokenCategory.PLUS},
                {"Neg", TokenCategory.NEGATIVE},
                {"Mul", TokenCategory.MULTIPLY},
                {"Exponentiation", TokenCategory.EXPONENTIATION},
                {"Slash", TokenCategory.SLASH},
                {"Pertg", TokenCategory.PERCENTAGE},
                
                {"QtnMark", TokenCategory.QUESTION_MARK},
                {"ExclMark", TokenCategory.EXCLAMATION_MARK},
                {"Tilde", TokenCategory.TILDE},
                
                {"BitWiseAnd", TokenCategory.BITWISE_AND},
                {"BitWiseOr", TokenCategory.BITWISE_OR},
                {"BitWiseOr2", TokenCategory.BITWISE_XOR},
                {"BitShiftLeft", TokenCategory.BIT_SHIFT_LEFT},
                {"BitShiftRight", TokenCategory.BIT_SHIFT_RIGHT},
                {"DBitShiftRight", TokenCategory.DOUBLE_BIT_SHIFT_RIGHT},
                
                {"Equal", TokenCategory.EQUAL},
                {"NotEqual", TokenCategory.NOT_EQUAL},
                {"GT", TokenCategory.GREATER_THAN},
                {"LT", TokenCategory.LESS_THAN},
                {"GEQ", TokenCategory.GREATER_OR_EQUAL_THAN},
                {"LEQ", TokenCategory.LESS_OR_EQUAL_THAN},
                
                {"Character", TokenCategory.CHARACTER_LITERAL},
                {"String", TokenCategory.STRING_LITERAL},
                
                {"ParOpen", TokenCategory.PARENTHESIS_OPEN},
                {"ParClose", TokenCategory.PARENTHESIS_CLOSE},
                {"BrackOpen", TokenCategory.BRACKETS_OPEN},
                {"BrackClose", TokenCategory.BRACKETS_CLOSE},
                {"Comma", TokenCategory.COMMA},
                {"Colon", TokenCategory.COLON},
                {"Semicolon", TokenCategory.SEMICOLON},
                
                {"SingleQuote", TokenCategory.SINGLE_QUOTE},
                {"DoubleQuote", TokenCategory.DOUBLE_QUOTE}
            };
            
        public static IEnumerable<Token> Tokenize(string input){
            
            var row = 1;
            var columnStart = 0;
            
            Func<Match, TokenCategory, Token> newToken = (match, tokenCategory) =>
                new Token(match.Value, tokenCategory, row, match.Index - columnStart + 1);
            
            foreach(Match match in regexMatcher.Matches(input)){
                if(match.Groups["Newline"].Length > 0){
                    row++;
                    columnStart = match.Index + match.Length;
                }
                else if(match.Groups["Other"].Length > 0){
                    yield return newToken(match, TokenCategory.ILLEGAL_CHAR);
                }
                else if(match.Groups["WhiteSpace"].Length > 0){ }
                else if(match.Groups["Identifier"].Length > 0){
                    
                    if(keywords.ContainsKey(match.Value)){
                        yield return newToken(match, keywords[match.Value]);
                    }
                    else{
                        yield return newToken(match, TokenCategory.IDENTIFIER);
                    }
                    
                }
                else{
                    
                    foreach(var name in nonKeywords.Keys){
                        if(match.Groups[name].Length > 0){
                            yield return newToken(match, nonKeywords[name]);
                            break;
                        }
                    }
                    
                }
            }
            
            yield return new Token(null, TokenCategory.EOF, row, input.Length - columnStart + 1);
            
        }
    }
}