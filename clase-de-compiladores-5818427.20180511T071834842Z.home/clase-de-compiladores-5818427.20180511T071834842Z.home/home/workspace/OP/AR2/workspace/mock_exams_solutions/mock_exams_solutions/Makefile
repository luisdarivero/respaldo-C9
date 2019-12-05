all: Boolang.exe Ppt.exe

Boolang.exe: boolang.cs
	mcs boolang.cs -out:Boolang.exe

Ppt.exe: ppt.cs
	mcs ppt.cs -out:Ppt.exe

clean:
	rm Boolang.exe Ppt.exe
