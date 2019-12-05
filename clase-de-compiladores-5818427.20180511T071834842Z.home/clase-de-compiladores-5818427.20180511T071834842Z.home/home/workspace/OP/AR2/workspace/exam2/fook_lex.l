%{
// Aldo Reyna Gómez - A01169073
#include "fook_tokens.h"

void yyerror(char *s, ...);

%}

%%

[a-z] { yylval = yytext[0];
        return SYMBOL; }
"+"   { return SUC; }
"-"   { return PRED; }
"{"   { return BRACE_OPEN; }
"}"   { return BRACE_CLOSE; }
","   { return COMMA; }
[ \t]  { /* ignore whitespace */ }
\n    { return EOL; }
.     { return ILLEGAL; }
%%
