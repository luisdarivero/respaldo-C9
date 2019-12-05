//---------------------------------------------------------
// Type your name and student ID here.
//---------------------------------------------------------

%{

#include <stdio.h>
#include <stdarg.h>

void yyerror(char *s, ...);
extern int yylineno;

%}

%union {
    int ival;
}

/* declare tokens */
%token ADD MUL PAR_LEFT PAR_RIGHT EOL
%token<ival> INTEGER
%type<ival> calclist exp exp_sum exp_mul

%%

calclist:
    /* nothing */ { }                            /* matches at beginning of input */
    | calclist exp EOL { printf("%d\n> ", $2); } /* EOL is end of an expression */
;


exp:
      PAR_LEFT ADD PAR_RIGHT         { $$ = 0; }
    | PAR_LEFT MUL PAR_RIGHT         { $$ = 1; }
    | PAR_LEFT ADD exp_sum PAR_RIGHT { $$ = $3; }
    | PAR_LEFT MUL exp_mul PAR_RIGHT { $$ = $3; }
    | INTEGER
;

exp_sum:
      PAR_LEFT ADD PAR_RIGHT        { $$ = 0; }
    | PAR_LEFT MUL PAR_RIGHT        { $$ = 1; }
    | exp_sum exp_sum               { $$ = $1 + $2; }
    | PAR_LEFT ADD exp_sum PAR_RIGHT { $$ = $3; }
    | PAR_LEFT MUL exp_mul PAR_RIGHT { $$ = $3; }
    | INTEGER

;

exp_mul:
      PAR_LEFT ADD PAR_RIGHT        { $$ = 0; }
    | PAR_LEFT MUL PAR_RIGHT        { $$ = 1; }
    | exp_mul exp_mul               { $$ = $1 * $2; }
    | PAR_LEFT ADD exp_sum PAR_RIGHT { $$ = $3; }
    | PAR_LEFT MUL exp_mul PAR_RIGHT { $$ = $3; }
    | INTEGER


%%

int main(int argc, char **argv) {
    printf("> ");
    yyparse();
    return 0;
}

void yyerror(char *s, ...) {
    va_list ap;
    va_start(ap, s);
    fprintf(stderr, "Line %d: ", yylineno);
    vfprintf(stderr, s, ap);
    fprintf(stderr, "\n");
}