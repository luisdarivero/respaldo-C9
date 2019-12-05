
deepLingo.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs Parser.cs \
	SyntaxError.cs Node.cs SpecificNodes.cs SymbolTable.cs SemanticAnalyzer.cs \
	LocalFunctionTable.cs FunctionTable.cs SemanticError.cs
	mcs -out:deeplingo.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs \
	Parser.cs SyntaxError.cs Node.cs SpecificNodes.cs SymbolTable.cs SemanticAnalyzer.cs \
	LocalFunctionTable.cs FunctionTable.cs SemanticError.cs
			
clean:
	rm deeplingo.exe
