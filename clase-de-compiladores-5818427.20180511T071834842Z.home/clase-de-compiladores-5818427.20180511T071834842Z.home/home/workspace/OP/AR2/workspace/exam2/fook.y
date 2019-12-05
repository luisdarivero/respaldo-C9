//==========================================================
// Aldo Reyna Gómez - A01169073
//==========================================================

%{

#include <stdio.h>
#include <stdarg.h>
#include <stdlib.h>
#include <math.h>

int yylex(void);
void yyerror(char *s, ...);
extern int yylineno;

%}

/* Declare tokens */
%token SYMBOL SUC PRED MAX COMMA BRACE_OPEN BRACE_CLOSE EOL ILLEGAL

/* Specify operator precedence and associativity */
%left SUC
%left PRED
%left BRACE_OPEN
%nonassoc BRACE_CLOSE
%nonassoc COMMA

%%

fooklist:
    /* nothing */ { } /* matches at beginning of input */
    | fooklist expr EOL { printf("%c\n> ", $2 + 'a'); } /* EOL is end of an expression */
;

expr:
      SYMBOL { $$ = (int)$1; }
    | expr SUC { 
        if ($1 == 'z')
            $$ = 'a';
        else    
            $$ = (char) (((int)$1) + 1); 
      }
    | expr PRED { $$ = (char) (int)$1; }
    | max { $$ = $1; }
;

max:
    BRACE_OPEN exprList BRACE_CLOSE {$$ = 0; }
;
    
exprList:
    exprList COMMA expr { $$ = 0; }
;

%%

int main(int argc, char **argv) {
    printf("> ");
    yyparse();
    return 0;
}

void yyerror(char * s, ...) {
    fprintf(stderr, "Syntax Error!\n");
    exit(1);
}
