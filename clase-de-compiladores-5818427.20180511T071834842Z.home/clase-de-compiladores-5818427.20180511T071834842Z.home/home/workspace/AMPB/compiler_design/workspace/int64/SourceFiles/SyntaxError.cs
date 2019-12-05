/*
  Compilador de int64
  -- SyntaxError:
        Clase que contiene un error de sintáxis.

  Copyright (C) 2017 Iram Molina & Diego Trujillo bajo WTFPL.
  ITESM CEM
*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Int64{
    
    class SyntaxError: Exception {

        public SyntaxError(TokenCategory expectedCategory, Token token): 
          base(String.Format("Syntax Error: Expected {0}, but found {1} ('{2}')  at @({3}, {4})", expectedCategory, token.Category, token.Lexeme, token.Row, token.Column)) { }

        public SyntaxError(ISet<TokenCategory> expectedCategories, Token token):
            base(String.Format("Syntax Error: Expected one of {0}, but found {1} ('{2}') at @({3}, {4}).", Elements(expectedCategories), token.Category, token.Lexeme, token.Row, token.Column)) { }

        static string Elements(ISet<TokenCategory> expectedCategories) {
            var stringBuilder = new StringBuilder("{");
            var first = true;
            foreach (var elem in expectedCategories) {
                if (first) {
                    first = false;
                } else {
                    stringBuilder.Append(", ");
                }
                stringBuilder.Append(elem);
            }
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }
    }

}