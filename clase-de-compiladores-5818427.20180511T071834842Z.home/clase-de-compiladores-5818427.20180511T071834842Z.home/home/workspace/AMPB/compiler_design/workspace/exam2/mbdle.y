//---------------------------------------------------------
// Andrea Margarita Pérez Barrera A01373631
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
%token ATOM COMA OPENP CLOSEP OPENSB CLOSESB OPENAB CLOSEAB OPENCB CLOSECB ILLEGAL EOL

%%

mbdle:
    |mbdle list EOL { printf("syntax ok\n> "); }  /* EOL is end of an expression */
    
;

list:
    ATOM
    |OPENP atom_list CLOSEP    
    |OPENSB atom_list CLOSESB   
    |OPENAB atom_list CLOSEAB    
    |OPENCB atom_list CLOSECB      
;

atom_list:
    /* nothing */ { }
    |list
    |atom_list COMA atom_list
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
