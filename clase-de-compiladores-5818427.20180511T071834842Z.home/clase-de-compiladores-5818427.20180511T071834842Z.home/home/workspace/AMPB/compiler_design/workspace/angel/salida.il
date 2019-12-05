.assembly 'ppt' {}

.assembly extern 'pptlib' {}

.class public 'salida' extends ['mscorlib']'System'.'Object' {
	.method public static void 'inicio'() {
		.entrypoint
		ldc.r8 -7.4
		call void ['mscorlib']'System'.'Console'::'WriteLine'(float64)
		ret
	}
}
