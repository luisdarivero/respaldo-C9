#  Compilador de int64
#  -- Makefile
#        Facilita la compilación de los archivos.
# 
#Copyright (C) 2017 Iram Molina & Diego Trujillo bajo WTFPL.
#ITESM CEM

int64.exe: ./SourceFiles/Driver.cs ./SourceFiles/CommentTrimmer.cs ./SourceFiles/LexicalAnalyzer.cs ./SourceFiles/Token.cs ./SourceFiles/TokenCategory.cs ./SourceFiles/SyntacticAnalyzer.cs ./SourceFiles/SyntaxError.cs ./SourceFiles/Node.cs ./SourceFiles/SpecificNodes.cs ./TestPrograms/RunTest.cs
	mcs -out:int64.exe ./SourceFiles/Driver.cs ./SourceFiles/CommentTrimmer.cs ./SourceFiles/LexicalAnalyzer.cs ./SourceFiles/Token.cs ./SourceFiles/TokenCategory.cs ./SourceFiles/SyntacticAnalyzer.cs ./SourceFiles/SyntaxError.cs ./SourceFiles/Node.cs ./SourceFiles/SpecificNodes.cs
	mcs -out:RunTest.exe ./TestPrograms/RunTest.cs

clean:
	rm int64.exe