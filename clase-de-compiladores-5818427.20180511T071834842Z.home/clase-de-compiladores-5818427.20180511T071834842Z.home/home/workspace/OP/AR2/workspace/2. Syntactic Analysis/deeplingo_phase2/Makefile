# This programme is based on the Buttercup's compiler code, provided by Ariel Ortiz.

# 
#    Lexical Analysis
#    Date: 29-Jan-2018
#    Authors:
#          A01169073 Aldo Reyna
#          A01375051 Marina Torres
#    File name: Makefile


deeplingo.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs Parser.cs \
	SyntaxError.cs
	mcs -out:deeplingo.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs \
	Parser.cs SyntaxError.cs
			
clean:
	rm deeplingo.exe