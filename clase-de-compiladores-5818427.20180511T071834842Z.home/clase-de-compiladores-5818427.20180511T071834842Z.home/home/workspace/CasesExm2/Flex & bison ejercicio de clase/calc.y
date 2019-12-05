/* simplest version of calculator */
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
%token ADD MUL NUMBER PAR_LEFT PAR_RIGHT EOL

/* Specify operator precedence and associativity */
%left ADD
%left MUL


%%

calclist:
    /* nothing */ { } /* matches at beginning of input */
    | calclist exp EOL { printf("%d\n> ", $2); } /* EOL is end of an expression */
;

exp:
      PAR_LEFT ADD PAR_RIGHT            { $$ = 0; }
    | PAR_LEFT MUL PAR_RIGHT            { $$ = 1; }
    | PAR_LEFT ADD sumExpr PAR_RIGHT    { $$ = $3; }
    | PAR_LEFT MUL mulExpr PAR_RIGHT    { $$ = $3; }
    | NUMBER
;

sumExpr:
     sumExpr sumExpr     {$$ = $1 + $2;}
    |exp
    
mulExpr:
     mulExpr mulExpr     {$$ = $1 * $2;}
    |exp


%%
int main(int argc, char **argv) {
    printf("> ");
    yyparse();
    return 0;
}
void yyerror(char *s, ...) {
    va_list ap;
    va_start(ap, s);
    fprintf(stderr, "%d: error: ", yylineno);
    vfprintf(stderr, s, ap);
    fprintf(stderr, "\n");
    exit(1);
}