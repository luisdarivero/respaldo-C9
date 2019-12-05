
// CIL example program.
//
// To assemble:
//                 ilasm add.il

.assembly 'example' { }

.class public 'Test' extends ['mscorlib']'System'.'Object' {
    .method public static void 'whatever'() {
        .entrypoint
		ldc.i4 20
		ldc.i4 400
		add.ovf
		call void class ['mscorlib']'System'.'Console'::'WriteLine'(int32)
		ret
	}
}
