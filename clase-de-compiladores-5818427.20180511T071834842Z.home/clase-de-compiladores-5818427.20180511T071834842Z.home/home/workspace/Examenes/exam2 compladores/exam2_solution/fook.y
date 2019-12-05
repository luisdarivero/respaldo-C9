//==========================================================
// luis daniel rivero sosa A01374527.
//==========================================================

%{

#include <stdio.h>
#include <stdlib.h>
#include <math.h>

int yylex(void);
void yyerror(char *s, ...);

%}

/* Declare tokens */
%token SYMBOL EOL ILLEGAL PLUS NEG CI CD COMMA

%%

fooklist:
    /* nothing */ { } /* matches at beginning of input */
    | fooklist expr EOL { printf("%c\n> ", $2 + 'a'); } /* EOL is end of an expression */
;

expr:
    CI exprList CD              {$$ = $2;}
    | expr signList             {$$ = (($1 - '0')+ $2) + '0';}
    | SYMBOL                    
;


exprList:
    expr COMMA expr             {if($1 - '0'>$3 - '0'){$$ = $1;}else{$$ = $2}}
    | expr


signList:
    PLUS signList               {$$ = $1 + $2;}
    | NEG signList              {$$ = $1 + $2;}
    | PLUS                      {$$ = 1;}
    | NEG                       {$$ = -1;}


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
