
    // CIL example program.
    //
    // To assemble:
    //                 ilasm salida.il
    
    .assembly 'ppt' { }
    
    .assembly extern 'pptlib' { }
    
    .class public 'salida' extends ['mscorlib']'System'.'Object' {
        .method public static void 'inicio'() {
            .entrypoint
		ldstr "piedra"
		ldstr "papel"
		call string class ['pptlib']'ppt'.'Runtime'::'mas'(string, string)
		ldstr "tijeras"
		call string class ['pptlib']'ppt'.'Runtime'::'mas'(string, string)
		ldstr "piedra"
		ldstr "papel"
		call string class ['pptlib']'ppt'.'Runtime'::'menos'(string, string)
		call string class ['pptlib']'ppt'.'Runtime'::'menos'(string, string)
		call void class ['mscorlib']'System'.'Console'::'WriteLine'(string)
		ret
	}
}
