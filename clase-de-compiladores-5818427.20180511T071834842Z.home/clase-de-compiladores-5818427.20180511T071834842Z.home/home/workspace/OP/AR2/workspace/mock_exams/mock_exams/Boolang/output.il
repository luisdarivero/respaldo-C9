
    // CIL example program.
    //
    // To assemble:
    //                 ilasm output.il
    
    .assembly 'example' { }
    
    .class public 'Test' extends ['mscorlib']'System'.'Object' {
        .method public static void 'start'() {
            .entrypoint
		ldc.i4.1
		ldc.i4.0
		ldc.i4.0
		or
		xor
		ldc.i4.1
		and
		call void class ['mscorlib']'System'.'Console'::'WriteLine'(int32)
		ret
	}
}
