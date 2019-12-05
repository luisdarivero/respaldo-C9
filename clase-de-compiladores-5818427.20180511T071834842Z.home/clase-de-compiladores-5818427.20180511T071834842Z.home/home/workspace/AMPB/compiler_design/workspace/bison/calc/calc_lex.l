%{

#include "calc_tokens.h"
#include <math.h>

void yyerror(char *s, ...);

%}

%%

"+"    { return ADD; }
"-"    { return NEG; }
"*"    { return MUL; }
"^"    {return POW;}
"("    { return PAR_LEFT; }
")"    { return PAR_RIGHT; }
[01]+b  {yytext[strlen(yytext)-1] = 0; 
        yylval = convertBinaryToDecimal(atoi(yytext));
        return NUMBER;
        }
[0-9]+ {
        yylval = atoi(yytext);
        return NUMBER;
}
\n     { yylineno++; return EOL; }
[ \t]  { /* ignore whitespace */ }
.      { yyerror("Mystery character %c\n", *yytext); }

%%
int power(int a,int b){
        int j ;
        int c = a;
        if(b<1){
            return 1;
        }
        for(j=0; j<b-1;j++){
            a = a*c;
        }
        return a;
}
int convertBinaryToDecimal(int n)
{
    int decimalNumber = 0, i = 0, remainder = 0, dos=2;
    while (n!=0)
    {
        remainder = n%10;
        n /= 10;
        decimalNumber += remainder*power(dos,i);
        ++i;
    }
    return decimalNumber;
}
