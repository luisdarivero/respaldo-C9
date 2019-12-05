// This programme is based on the Buttercup's compiler code, provided by Ariel Ortiz.

/* 
    Lexical Analysis
    Date: 29-Jan-2018
    Authors:
          A01169073 Aldo Reyna
          A01375051 Marina Torres
    File name: Scanner.cs
*/


using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DeepLingo {

    class Scanner {

        readonly string input;

        static readonly Regex regex = new Regex(
            @"                             
                (?<And>             [&&]{2}                         )
              | (?<Or>              [||]{2}                         )
              | (?<MultiComment>    [/][*](.|\n)*?[*][/]            )
              | (?<OneComment>      [//]{2}(.)*                     )
              | (?<Comma>           ,                               )
              | (?<SemiColumn>      ;                               )
              | (?<Inc>             [++]{2}                         )
              | (?<Dec>             [--]{2}                         )
              
              
              | (?<Printi>          printi\s?                       )
              | (?<Printc>          printc\s?                       )
              | (?<Prints>          prints\s?                       )
              | (?<Println>         println\(\)                     )
              | (?<Readi>           readi\(\)                       )
              | (?<Reads>           reads\(\)                       )
              
              | (?<True>            42                              )
              
              | (?<CharLiteral>     '([^\n'\\]|\\n|\\r|\\t|\\\\|\\'|\\""|\\u[0-9a-fA-F]{6})'                     )
              
              
              
              | (?<GetArr>          get\s?                          )
              | (?<SetArr>          set\s?                          )
              | (?<AddArr>          add\s?                          )
              | (?<NewArr>          new\s?                          )
              | (?<SizeArr>         size\s?                         )
              
              | (?<IntLiteral>      \d+                             ) 
              | (?<Identifier>      [a-zA-Z0-9_]+                   )
              
              
              
              
              | (?<StrLiteral>      [""].*[""]                      )
              
              | (?<GreatEqual>      >=                              )
              | (?<Equals>          ==                              )
              | (?<NotEqual>        !=                              )
              | (?<LessEqual>       <=                              )
              
              
              | (?<Assign>          [=]                             )
              | (?<Not>             [!]                             )
              
              | (?<Less>            [<]                             )
              | (?<Great>           [>]                             )
              | (?<Add>             [+]                             )
              | (?<Subs>            [-]                             )
              
              | (?<Mul>             [*]                             )
              | (?<Div>             [/]                             )
              | (?<Rem>             [%]                             )
              
              | (?<Neg>             -\s?\d+                         )
              
              
              | (?<Pos>             \+\s?\d+                        )
              
              | (?<Newline>         \n                              )
              | (?<CarrReturn>      \r                              )
              | (?<Tab>             \t                              )
              | (?<SingleQuote>     \'                              )
              | (?<DoubleQuote>     \""                             )
              | (?<BackSlash>       \\                              )
              
              
              
              
              | (?<UniCharacter>    \\u[0-9a-fA-F]{6}               )
              | (?<ParLeft>         [(]                             ) 
              | (?<ParRight>        [)]                             )
              | (?<BraceLeft>       [{]                             )
              | (?<BraceRight>      [}]                             )
              | (?<BracketLeft>     [[]                             )
              | (?<BracketRight>    [\]]                            )
              
              | (?<WhiteSpace>      \s                              )     # Must go anywhere after Newline.
              | (?<Other>           .                               )     # Must be last: match any other character.
            ", 
            RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            );

        static readonly IDictionary<string, TokenCategory> keywords =
            new Dictionary<string, TokenCategory>() {
                {"break", TokenCategory.BREAK},
                {"else", TokenCategory.ELSE},
                {"elseif", TokenCategory.ELSEIF},
                {"if", TokenCategory.IF},
                {"loop", TokenCategory.LOOP},
                {"return", TokenCategory.RETURN},
                {"var", TokenCategory.VAR}
            };

        static readonly IDictionary<string, TokenCategory> nonKeywords =
            new Dictionary<string, TokenCategory>() {
                {"And", TokenCategory.AND},
                {"Add", TokenCategory.ADD},
                {"Assign", TokenCategory.ASSIGN},
                {"BraceRight", TokenCategory.BRACE_CLOSE},
                {"BraceLeft", TokenCategory.BRACE_OPEN},
                {"Comma", TokenCategory.COMMA},
                {"Dec", TokenCategory.DEC},
                {"Div", TokenCategory.DIV},
                {"Equals", TokenCategory.EQUALS},
                {"GetArr", TokenCategory.GET},
                {"Great", TokenCategory.GREAT},
                {"GreatEqual", TokenCategory.GREAT_EQ},
                {"Inc", TokenCategory.INC},
                {"Less", TokenCategory.LESS},
                {"LessEqual", TokenCategory.LESS_EQ},
                {"CharLiteral", TokenCategory.LIT_CHAR},
                {"IntLiteral", TokenCategory.LIT_INT},
                {"StrLiteral", TokenCategory.LIT_STR},
                {"Mul", TokenCategory.MUL},
                {"Neg", TokenCategory.NEG},
                {"New", TokenCategory.NEW},
                {"Not", TokenCategory.NOT},
                {"NotEqual", TokenCategory.NOT_EQUAL},
                {"Or", TokenCategory.OR},
                {"ParLeft", TokenCategory.PARENTHESIS_OPEN},
                {"ParRight", TokenCategory.PARENTHESIS_CLOSE},
                {"Pos", TokenCategory.POS},
                {"Printi", TokenCategory.PRINTI},
                {"Printc", TokenCategory.PRINTC},
                {"Prints", TokenCategory.PRINTS},
                {"Println", TokenCategory.PRINTLN},
                {"Readi", TokenCategory.READI},
                {"Reads", TokenCategory.READS},
                {"Rem", TokenCategory.REM},
                {"SemiColumn", TokenCategory.SEMICOL},
                {"SetArr", TokenCategory.SET},
                {"SizeArr", TokenCategory.SIZE},
                {"FunctionCall", TokenCategory.CALL},
                {"True", TokenCategory.TRUE},
                {"Subs", TokenCategory.SUBS},
                {"BracketLeft", TokenCategory.BRACKET_OPEN},
                {"BracketRight", TokenCategory.BRACKET_CLOSE}
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

                } else if (m.Groups["WhiteSpace"].Success||m.Groups["OneComment"].Success) {
                
                  
                } else if (m.Groups["MultiComment"].Success) {
                  
                    int count = m.Value.Split('\n').Length - 1;
                    
                    if (count > 0) {
                        row += count;
                    }

                    // Skip white space and comments.

                } else if (m.Groups["Identifier"].Success) {

                    if (keywords.ContainsKey(m.Value)) {

                        // Matched string is a DeepLingo keyword.
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



