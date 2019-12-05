.assembly 'ppt' {}

.class public 'salida' extends ['mscorlib']'System'.'Object' {
	.method public static void 'start' () {
		.entrypoint
		ldc.i4.1
		call void ['mscorlib']'System'.'Console'::'WriteLine'(int32)
		ret
	}
}
