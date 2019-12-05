/*
  Compilador de int64
  -- Token
        Contiene información sobre un Token.

  Copyright (C) 2017 Iram Molina & Diego Trujillo bajo WTFPL.
  ITESM CEM
*/

using System;
using System.IO;
using System.Text;

namespace Int64 {
    
    public class Token{
        // Atributos de un Token
        private readonly string lexeme;
        readonly TokenCategory category;
        readonly int row;
        readonly int column;
        
        // Getters
        public string Lexeme { get {return lexeme;}}
        public TokenCategory Category { get {return category;}}
        public int Row { get {return row;}}
        public int Column { get {return column;}}
        
        // Constructor
        public Token(string lexeme, TokenCategory category, int row, int column){
            this.lexeme = lexeme;
            this.category = category;
            this.row = row;
            this.column = column;
        }
        
        public override string ToString() {
            //return string.Format("{{{0}, \"{1}\", @({2}, {3})}}", category, lexeme, row, column);
            
            return string.Format("{0} @({2}, {3}): \"{1}\" ", category, lexeme, row, column);
        }
    }
}