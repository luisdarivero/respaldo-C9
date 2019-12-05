# This programme is based on the deeplingo's compiler code, provided by Ariel Ortiz.

# 
#    AST Construction
#    Date: 12-March-2018
#    Authors:
#          A01169073 Aldo Reyna
#          A01375051 Marina Torres
#    File name: Makefile

deeplingo.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs Parser.cs \
	SyntaxError.cs Node.cs SpecificNodes.cs
	mcs -out:deeplingo.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs \
	Parser.cs SyntaxError.cs Node.cs SpecificNodes.cs
			
clean:
	rm deeplingo.exe
