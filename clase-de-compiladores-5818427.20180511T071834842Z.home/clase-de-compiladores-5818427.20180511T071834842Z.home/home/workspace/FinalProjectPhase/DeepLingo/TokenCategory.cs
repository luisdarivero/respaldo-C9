
namespace DeepLingo {

    enum TokenCategory {
        AND,            // &&
        ASSIGN,         // =
        BREAK,          // break
        CLOSEDCURLY,     // }
        CLOSEDPAR,       // )
        CLOSEDBRACKET,    // ]
        CHAR,      //      'AAA'
        COMMA,           // ,
        DECREMENT,      // --
        NOTEQUALS,      // !=
        DIVIDE,         // /
        EQUALS,         // ==
        ELSE,           // else
        ELSEIF,         // elseif
        EOF,            // End of Line
        GREATER,        // >
        GREATEREQUAL,   // >=
        IF,             // if
        IDENTIFIER,     // Common Identifier
        INCREMENT,      // ++
        INTLITERAL,     // Int Literal
        LESS,           // <
        LESSEQUAL,      // <=
        LOOP,           // loop
        MINUS,          // -
        MODULO,         // %
        MULTIPLICATION,   // *
        NEGATION,       // -x
        NOT,            // !
        OPENEDPAR,        // (
        OPENEDCURLY,      // {
        OPENEDBRACKET,     // [
        OR,             // ||
        PLUS,           // +
        RETURN,         // return
        SAME,           // +x
        SEMICOLON,      // ;
        STRING,         // Any string
        VAR,            // var
        ILLEGAL_CHAR    // 
    }
}