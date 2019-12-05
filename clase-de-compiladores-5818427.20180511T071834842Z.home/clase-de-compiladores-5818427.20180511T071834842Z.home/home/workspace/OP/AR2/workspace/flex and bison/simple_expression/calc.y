/* Simple expression */
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
%token ADD MUL NEG NUMBER PAR_LEFT PAR_RIGHT EOL

/* Specify operator precedence and associativity */
%left ADD
%left MUL
%nonassoc NEG

%%

calclist:
    /* nothing */ { } /* matches at beginning of input */
    | calclist exp EOL { printf("%d\n> ", $2); } /* EOL is end of an expression */
;

exp:
      PAR_LEFT ADD explistadd PAR_RIGHT { $$ = $3; }
    | PAR_LEFT MUL explistmul PAR_RIGHT { $$ = $3; }
    | NUMBER
;

explistadd:
    /* nothing */            { $$ = 0; }
    | explistadd exp         { $$ = $1 + $2; }
;
    
explistmul:
    /* nothing */            { $$ = 1; }
    | explistmul exp         { $$ = $1 * $2; }
;

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