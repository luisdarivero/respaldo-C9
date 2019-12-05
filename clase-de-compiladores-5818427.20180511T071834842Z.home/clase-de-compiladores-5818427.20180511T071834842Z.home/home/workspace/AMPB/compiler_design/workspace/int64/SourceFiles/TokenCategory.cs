/*
  Compilador de int64
  -- TokenCategory
        Define las categorías de Tokens leídos por el compilador.

  Copyright (C) 2017 Iram Molina & Diego Trujillo bajo WTFPL.
  ITESM CEM
*/

namespace Int64{
    
    public enum TokenCategory{
        VAR,
        IDENTIFIER,
        IF,
        ELSE,
        ELSE_IF,
        CONTINUE,
        TRUE,
        FALSE,
        WHILE,
        DO,
        FOR,
        IN,
        SWITCH,
        CASE,
        DEFAULT,
        BREAK,
        RETURN,
        ASSIGNMENT,
        BASE_2_INT_LITERAL,
        BASE_8_INT_LITERAL,
        BASE_10_INT_LITERAL,
        BASE_16_INT_LITERAL,
        PLUS,
        NEGATIVE,
        MULTIPLY,
        EXPONENTIATION,
        PERCENTAGE, // %
        SLASH, // /
        EQUAL,
        NOT_EQUAL,
        GREATER_THAN,
        LESS_THAN,
        GREATER_OR_EQUAL_THAN,
        LESS_OR_EQUAL_THAN,
        AND,
        OR,
        BITWISE_AND,
        BITWISE_OR,
        BITWISE_XOR,
        BIT_SHIFT_LEFT,
        BIT_SHIFT_RIGHT,
        DOUBLE_BIT_SHIFT_RIGHT,
        CHARACTER_LITERAL,
        STRING_LITERAL,
        PARENTHESIS_OPEN,
        PARENTHESIS_CLOSE,
        BRACKETS_OPEN,
        BRACKETS_CLOSE,
        SINGLE_QUOTE, // "
        DOUBLE_QUOTE, // '
        COMMA,
        COLON,
        SEMICOLON,
        QUESTION_MARK,
        EXCLAMATION_MARK,
        TILDE,
        EOF,
        ILLEGAL_CHAR
    }
}