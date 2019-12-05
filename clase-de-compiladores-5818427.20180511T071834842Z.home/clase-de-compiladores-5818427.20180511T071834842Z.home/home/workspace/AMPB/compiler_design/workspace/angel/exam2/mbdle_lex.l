/*---------------------------------------------------------
  Jose AngelPrado Dupont A01373242
-----------------------------------------------------------*/

%{

#include "mbdle_tokens.h"

%}

%%

[a-zA-Z0-9_]+  { return ATOM; }
"{"              { return OPENB; }
"}"              { return CLOSEB; }
"<"              { return OPENM; }
">"              { return CLOSEM; }
"("             { return OPENP; }
")"             { return CLOSEP; }
"["             { return OPENJ; }
"]"             { return CLOSEJ; }
","              { return COMA; }
[ \t]          { /* ignore whitespace */ }
\n             { return EOL; }
.              { return ILLEGAL; }

%%
