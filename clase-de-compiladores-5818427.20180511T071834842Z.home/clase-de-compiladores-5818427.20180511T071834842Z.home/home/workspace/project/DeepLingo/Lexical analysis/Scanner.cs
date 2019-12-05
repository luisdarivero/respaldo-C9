
// Authors:
//           A01374527 Luis Daniel Rivero Sosa
//           A01372915 Rodrigo Benavides Villanueva 
//           A01374356 Javier Antonio García Roque 

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DeepLingo {

    class Scanner {

        readonly string input;
        
        // char-prev: '(\\[nrt""\\']|\\u[0-9a-fA-F]{6})'
        // str-prev: ""(\\[nrt""\\']|(\\u [a-fA-F0-9]{6})|([^\n\r\t\""\\']))*?""

        static readonly Regex regex = new Regex(
            @"                             
                (?<And>                 &&          )   #
              | (?<EqualTo>             [=][=]      )   #
              | (?<NotEqualTo>          [!][=]      )   #
              | (?<Assign>              [=]         )   #
              | (?<Or>                  \|\|        )   #
              | (?<SingleLnComment>     \/\/.*      )   #single line comment.
              | (?<MultiLnComment>      \/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*\/         )   #Multi line comment ERROR.
              | (?<id>                  [a-zA-Z_]+  )   #
              | (?<LitInt>              -?\d+       )   #
              | (?<LitChar>             '((\\[nrt""\\'])|(\\u [a-fA-F0-9]{6})|[^\n\r\t\'\""\\])'        )
              | (?<LitStr>              ""(\\[nrt""\\']|(\\u [a-fA-F0-9]{6})|([^\n\r\t]))*?""          )
              | (?<GreaterEqual>        [>][=]      )   #
              | (?<LessEqual>           [<][=]      )   #
              | (?<Less>                [<]         )   #
              | (?<GreaterThan>         [>]         )   #
              | (?<Mul>                 [*]         )   #
              | (?<Decrementation>      [-][-]      )   #
              | (?<Neg>                 [-]         )   #
              | (?<Newline>             \n          )   #
              | (?<ParLeft>             [(]         )   #
              | (?<ParRight>            [)]         )   #
              | (?<BraceOpen>           [{]         )   #
              | (?<BraceClose>          [}]         )   #
              | (?<Semicolon>           [;]         )   #
              | (?<Comma>               [,]         )   #
              | (?<Incrementation>      [+][+]      )   #
              | (?<Plus>                [+]         )   #
              | (?<Div>                 \/          )   #
              | (?<Remainder>           [%]         )   #
              | (?<Not>                 [!]         )   #
              | (?<SquareBOpen>         \[          )
              | (?<SquareBClose>        \]          )
              | (?<WhiteSpace>          \s          )   # Must go anywhere after Newline.
              | (?<Other>               .           )   # Must be last: match any other character.
            ", 
            RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            );

        static readonly IDictionary<string, TokenCategory> keywords =
            new Dictionary<string, TokenCategory>() {
                {"if", TokenCategory.IF}, //comma separated
                {"break", TokenCategory.BREAK},
                {"else", TokenCategory.ELSE},
                {"elseif", TokenCategory.ELSEIF},
                {"loop", TokenCategory.LOOP},
                {"return", TokenCategory.RETURN},
                {"var" , TokenCategory.VAR},
                {"printi" , TokenCategory.PRINT_I},
                {"printc" , TokenCategory.PRINT_C},
                {"prints" , TokenCategory.PRINT_S},
                {"println" , TokenCategory.PRINT_LN},
                {"readi" , TokenCategory.READ_I},
                {"reds" , TokenCategory.READ_S},
                {"new" , TokenCategory.NEW},
                {"size" , TokenCategory.SIZE},
                {"add" , TokenCategory.ADD},
                {"get" , TokenCategory.GET},
                {"set" , TokenCategory.SET}
                
                
            };

        static readonly IDictionary<string, TokenCategory> nonKeywords =
            new Dictionary<string, TokenCategory>() {
                {"And", TokenCategory.AND},
                {"Assign", TokenCategory.ASSIGN},
                {"LitInt", TokenCategory.INT_LITERAL},
                {"Less", TokenCategory.LESS},
                {"Mul", TokenCategory.MUL},
                {"Neg", TokenCategory.NEG},
                {"ParLeft", TokenCategory.PARENTHESIS_OPEN},
                {"ParRight", TokenCategory.PARENTHESIS_CLOSE},
                {"Plus", TokenCategory.PLUS},
                {"Div" , TokenCategory.DIV},
                {"Remainder" , TokenCategory.REMAINDER},
                {"Not" , TokenCategory.NOT},
                {"Or" , TokenCategory.OR},
                {"EqualTo" , TokenCategory.EQUAL_TO},
                {"NotEqualTo" , TokenCategory.NOT_EQUAL_TO},
                {"Incrementation" , TokenCategory.INCREMENTATION},
                {"Decrementation" , TokenCategory.DECREMENTATION},
                {"GreaterThan" , TokenCategory.GREATER_THAN},
                {"GreaterEqual" , TokenCategory.GREATER_THAN_OR_EQUAL_TO},
                {"LessEqual" , TokenCategory.LESS_THAN_OR_EQUAL_TO},
                {"BraceOpen" , TokenCategory.BRACE_OPEN},
                {"BraceClose" , TokenCategory.BRACE_CLOSE},
                {"Semicolon" , TokenCategory.SEMICOLON},
                {"Comma" , TokenCategory.COMMA},
                {"SquareBOpen" , TokenCategory.SQUARE_BRACKET_OPEN},
                {"SquareBClose" , TokenCategory.SQUARE_BRACKET_CLOSE},
                {"LitChar" , TokenCategory.CHAR_LITERAL},
                {"LitStr" , TokenCategory.STR_LITERAL}
            };

        public Scanner(string input) {
            this.input = input;
        }

        public IEnumerable<Token> Start() {

            var row = 1;
            var columnStart = 0;

            Func<Match, TokenCategory, Token> newTok = (m, tc) =>
                new Token(m.Value, tc, row, m.Index - columnStart + 1);

            foreach (Match m in regex.Matches(input)) {

                if (m.Groups["Newline"].Success) {

                    // Found a new line.
                    row++;
                    columnStart = m.Index + m.Length;

                } else if (m.Groups["WhiteSpace"].Success 
                    || m.Groups["SingleLnComment"].Success ) {

                    // Skip white space and single-comments.
                  

                } else if (m.Groups["MultiLnComment"].Success) {
                    foreach (var c in m.Value){
                        if (c == '\n'){
                            row++;
                        }
                    }
                    
                } else if (m.Groups["id"].Success) {

                    if (keywords.ContainsKey(m.Value)) {

                        // Matched string is a Buttercup keyword.
                        yield return newTok(m, keywords[m.Value]);                                               

                    } else { 

                        // Otherwise it's just a plain identifier.
                        yield return newTok(m, TokenCategory.IDENTIFIER);
                    }

                } else if (m.Groups["Other"].Success) {

                    // Found an illegal character.
                    yield return newTok(m, TokenCategory.ILLEGAL_CHAR);

                } else {

                    // Match must be one of the non keywords.
                    foreach (var name in nonKeywords.Keys) {
                        if (m.Groups[name].Success) {
                            yield return newTok(m, nonKeywords[name]);
                            break;
                        }
                    }
                }
            }

            yield return new Token(null, 
                                   TokenCategory.EOF, 
                                   row, 
                                   input.Length - columnStart + 1);
        }
    }
}
