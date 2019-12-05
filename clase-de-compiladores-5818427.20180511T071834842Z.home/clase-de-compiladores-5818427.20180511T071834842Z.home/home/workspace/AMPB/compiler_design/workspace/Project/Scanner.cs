/*
Jose Angel Prado Dupont A01373243
Andrea Margarita Pérez Barrera A01373631
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace INT64
{

    class Scanner
    {

        readonly string input;

        static readonly Regex regex = new Regex(@"
               (?<comment> (//.*\n)             )
                |(?<string> \""(.)*(\n)*\"")
                | (?<longComment>(\/[\*](.*?|\n)*?\*\/))
                |(?<newLine>    \n                )             
                |(?<identifier> [a-zA-Z]+\w*      )   
                |(?<binary>     0[bB][01]+        )    
                |(?<octal>      0[oO][01234567]+  )
                |(?<hexal>      0[xX][0-9a-fA-F]+ ) 
                |(?<integer>    -?\d+              )          
                |(?<character>  '\\''|'.?'|'\\\""'|'\\n'|'\\r'|'\\t'|'\\\\'|'\\'|'\\\\'|'\\u[a-fA-F0-9]{6}')
                |(?<plus>       [\+]                )            
                |(?<minus>      [-]                 )             
                |(?<openB>      [{]                 )             
                |(?<closeB>     [}]                 )             
                |(?<openP>      [\(]                )            
                |(?<closeP>     [\)]                )           
                |(?<semicolon>  [;]                 )            
                |(?<coma>       [,]                 )            
                |(?<or>         \|\|              )          
                |(?<and>        &&                  )            
                |(?<colon>      [:]                 )             
                |(?<interrogation>[\?]              )      
                |(?<equal>       ==                ) 
                |(?<asignation> [=]                 ) 
                |(?<notEqual>   !=                )          
                |(?<lessEqual>  <=                )         
                |(?<moreEqual>  >=                )         
                |(?<orr>        \|                )               
                |(?<power>      [\^]              )              
                |(?<andd>       [&]               ) 
                |(?<shiftt>     >>>               )
                |(?<shiftL>     <<                )              
                |(?<shiftR>     >>                ) 
                |(?<less>       [<]                 )             
                |(?<more>       [>]                 )
                |(?<div>        [/]                 )              
                |(?<rem>        [%]                 )              
                |(?<powerr>     \*\*                )
                |(?<times>      [\*]                 )
                |(?<flow>      [~]                 )
                |(?<exclamation>[!]                 )        
                |(?<error>      .                 )             

            ",
            RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            );

        static readonly IDictionary<string, TokenCategory> keywords =
                  new Dictionary<string, TokenCategory>() {
                {"break", TokenCategory.BREAK},
                {"case", TokenCategory.CASE},
                {"continue", TokenCategory.CONTINUE},
                {"default", TokenCategory.DEFAULT},
                {"do", TokenCategory.DO},
                {"else", TokenCategory.ELSE},
                {"false", TokenCategory.FALSE},
                {"for", TokenCategory.FOR},
                {"if", TokenCategory.IF},
                {"in", TokenCategory.IN},
                {"return", TokenCategory.RETURN},
                {"switch", TokenCategory.SWITCH},
                {"true", TokenCategory.TRUE},
                {"while", TokenCategory.WHILE},
                {"var", TokenCategory.VAR}
                  };

        static readonly IDictionary<string, TokenCategory> nonKeywords =
            new Dictionary<string, TokenCategory>() {
                {"string", TokenCategory.STRING},
                {"identifier", TokenCategory.IDENTIFIER},
                {"binary", TokenCategory.BINARYINT},
                {"octal", TokenCategory.OCTALINT},
                {"hexal", TokenCategory.HEXAINT},
                {"integer", TokenCategory.INTEGER},
                {"character", TokenCategory.CHARACTER},
                {"plus", TokenCategory.PLUS},
                {"minus", TokenCategory.MINUS},
                {"less", TokenCategory.LESS},
                {"more", TokenCategory.MORE},
                {"openB", TokenCategory.OPENB},
                {"closeB", TokenCategory.CLOSEB},
                {"openP", TokenCategory.OPENP},
                {"closeP", TokenCategory.CLOSEP},
                {"semicolon", TokenCategory.SEMICOLON},
                {"coma", TokenCategory.COMA},
                {"asignation", TokenCategory.ASIGNATION},
                {"or", TokenCategory.OR},
                {"and", TokenCategory.AND},
                {"colon", TokenCategory.COLON},
                 {"interrogation", TokenCategory.INTERROGATION},
                {"equal", TokenCategory.EQUAL},
                {"notEqual", TokenCategory.NOTEQUAL},
                {"lessEqual", TokenCategory.LESSEQUAL},
                {"moreEqual", TokenCategory.MOREEQUAL},
                {"orr", TokenCategory.ORR},
                {"power", TokenCategory.POWER},
                {"andd", TokenCategory.ANDD},
                {"shiftL", TokenCategory.SHIFTL},
                {"shiftR", TokenCategory.SHIFTR},
                {"shiftt", TokenCategory.SHIFTT},
                {"times", TokenCategory.TIMES},
                {"div", TokenCategory.DIV},
                {"rem", TokenCategory.REM},
                {"powerr", TokenCategory.POWER},
                {"exclamation", TokenCategory.EXCLAMATION},
                {"flow", TokenCategory.FLOW},
                {"error", TokenCategory.ERROR}
            };

        public Scanner(string input)
        {
            this.input = input;
        }

        public IEnumerable<Token> Start()
        {

            var row = 1;
            var columnStart = 0;

            Func<Match, TokenCategory, Token> newTok = (m, tc) =>
                new Token(m.Value, tc, row, m.Index - columnStart + 1);

            foreach (Match m in regex.Matches(input))
            {

                if (m.Groups["newLine"].Length > 0)
                {
                    // Found a new line.
                    row++;
                    columnStart = m.Index + m.Length;

                }
                else if (m.Groups["comment"].Length > 0
                  || m.Groups["longComment"].Length > 0)
                {
                         string str = m.Value;
                         for (int i = 0; i < str.Length ; i++)
                         {
                             if(str[i]== '\n'){
                            row++;
                             }
                        
                    }

                }
                else if (m.Groups["identifier"].Length > 0)
                {

                    if (keywords.ContainsKey(m.Value))
                    {

                        yield return newTok(m, keywords[m.Value]);

                    }
                    else
                    {

                        yield return newTok(m, TokenCategory.IDENTIFIER);
                    }

                }
                else if (m.Groups["error"].Length > 0)
                {
                    
                    if(m.Value != " ")
                    {
                     yield return newTok(m, TokenCategory.ERROR);
                        
                    }

                }
                else
                {
                    foreach (var name in nonKeywords.Keys)
                    {
                        if (m.Groups[name].Length > 0)
                        {
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
