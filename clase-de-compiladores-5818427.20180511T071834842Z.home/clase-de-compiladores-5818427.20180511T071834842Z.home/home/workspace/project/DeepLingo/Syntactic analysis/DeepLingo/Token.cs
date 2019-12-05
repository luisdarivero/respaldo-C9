// Authors:
//           A01374527 Luis Daniel Rivero Sosa
//           A01372915 Rodrigo Benavides Villanueva 
//           A01374356 Javier Antonio García Roque 

using System;

namespace DeepLingo {

    class Token {

        readonly string lexeme;

        readonly TokenCategory category;

        readonly int row;

        readonly int column;

        public string Lexeme { 
            get { return lexeme; }
        }

        public TokenCategory Category {
            get { return category; }          
        }

        public int Row {
            get { return row; }
        }

        public int Column {
            get { return column; }
        }

        public Token(string lexeme, 
                     TokenCategory category, 
                     int row, 
                     int column) {
            this.lexeme = lexeme;
            this.category = category;
            this.row = row;
            this.column = column;
        }

        public override string ToString() {
            return string.Format("{{{0}, \"{1}\", @({2}, {3})}}",
                                 category, lexeme, row, column);
        }
    }
}

