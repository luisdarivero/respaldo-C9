#A01374527 luis daniel rivero sosa
%{

#include "fook_tokens.h"

void yyerror(char *s, ...);

%}

%%



[a-z] { return SYMBOL; }
\+    { return PLUS;}
\-    { return NEG;}
\{    { return CI;}
\}    { return CD;}
,     { return COMMA;}
\n    { return EOL; }
[ \t]  { }
.     { return ILLEGAL; }

%%
