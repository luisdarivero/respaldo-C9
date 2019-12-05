// This programme is based on the Buttercup's compiler code, provided by Ariel Ortiz.

/* 
    Lexical Analysis
    Date: 29-Jan-2018
    Authors:
          A01169073 Aldo Reyna
          A01375051 Marina Torres
    File name: TokenCategory.cs
*/


namespace DeepLingo {

    enum TokenCategory {
        ADD,
        AND,
        ASSIGN,
        BRACE_CLOSE,
        BRACE_OPEN,
        BRACKET_CLOSE,
        BRACKET_OPEN,
        BREAK,
        CALL,
        COMMA,
        DEC,
        DIV,
        ELSE,
        ELSEIF,
        EOF,
        EQUALS,
        GET,
        GREAT,
        GREAT_EQ,
        IDENTIFIER,
        IF,
        INC,
        LESS,
        LESS_EQ,
        LIT_CHAR,
        LIT_INT,
        LIT_STR,
        LOOP,
        MUL,
        NEG,
        NEW,
        NOT,
        NOT_EQUAL,
        OR,
        PARENTHESIS_CLOSE,
        PARENTHESIS_OPEN,
        POS,
        PRINTI,
        PRINTC,
        PRINTS,
        PRINTLN,
        READI,
        READS,
        REM,
        RETURN,
        SEMICOL,
        SET,
        SIZE,
        SUBS,
        TRUE,
        VAR,
        ILLEGAL_CHAR
    }
}

