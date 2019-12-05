# This programme is based on the deeplingo's compiler code, provided by Ariel Ortiz.

# 
#    AST Construction
#    Date: 12-March-2018
#    Authors:
#          A01169073 Aldo Reyna
#          A01375051 Marina Torres
#    File name: Makefile

all: deeplingo.exe deeplingolib.dll

deeplingo.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs Parser.cs \
	SyntaxError.cs Node.cs SpecificNodes.cs SemanticAnalyzer.cs \
	Values.cs SemanticError.cs SymbolTableGlobal.cs SymbolTableLocal.cs Locals.cs CILGenerator.cs
	mcs -out:deeplingo.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs \
	Parser.cs SyntaxError.cs Node.cs SpecificNodes.cs SemanticAnalyzer.cs \
	Values.cs SemanticError.cs SymbolTableGlobal.cs SymbolTableLocal.cs Locals.cs CILGenerator.cs
	

deeplingolib.dll: deeplingolib.cs 
	mcs /t:library deeplingolib.cs
			
clean:
	rm deeplingo.exe deeplingolib.dll
