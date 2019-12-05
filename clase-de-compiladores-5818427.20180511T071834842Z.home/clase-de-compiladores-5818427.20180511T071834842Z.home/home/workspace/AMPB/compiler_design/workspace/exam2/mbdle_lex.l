/*---------------------------------------------------------
  Andrea Margarita Pérez Barrera A01373631
-----------------------------------------------------------*/

%{

#include "mbdle_tokens.h"

%}

%%

[a-zA-Z0-9_]+  { return ATOM; }
[,]            { return COMA; }
[\(]           { return OPENP;}
[\)]           { return CLOSEP;}
[\[]           { return OPENSB;}
[\]]           { return CLOSESB;}
[<]            { return OPENAB;}
[>]            { return CLOSEAB;}
[{]            { return OPENCB;}
[}]            { return CLOSECB;}
[ \t]          { /* ignore whitespace */ }
\n             { return EOL; }
.              { return ILLEGAL; }

%%
