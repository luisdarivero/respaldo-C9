//---------------------------------------------------------
// Jose AngelPrado Dupont A01373242
//---------------------------------------------------------

%{

#include <stdio.h>
#include <stdarg.h>

int yylex(void);
void yyerror(char *s, ...);

%}

%union {
    int ival;
}

/* declare tokens */
%token ATOM ILLEGAL EOL OPENB CLOSEB OPENJ CLOSEJ OPENP CLOSEP OPENM CLOSEM COMA

%%

mbdle:
    /* nothing */ { }                              /* Matches at beginning of input */
    | mbdle OPENP mbdle CLOSEP EOL { printf("syntax ok\n> "); }
    | mbdle OPENB mbdle CLOSEB EOL { printf("syntax ok\n> "); }
    | mbdle OPENM mbdle CLOSEM EOL { printf("syntax ok\n> "); }
    | mbdle OPENJ mbdle CLOSEJ EOL { printf("syntax ok\n> "); }
    |ATOM COMA mbdle
    |ATOM
    | mbdle ATOM EOL { printf("syntax ok\n> "); }  /* EOL is end of an expression */
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
    vfprintf(stderr, s, ap);
    fprintf(stderr, "\n");
}
