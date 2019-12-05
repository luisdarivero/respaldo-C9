.assembly 'lunkwill' {}

.assembly extern 'lunkwill' {}

.class public 'LegendaryExam' extends ['mscorlib']'System'.'Object' {
	.method public static void 'start'() {
		.entrypoint
ldstr"a"
ldstr"b"
call string class ['lunklib']'LibLunk'.'Utils'::'Maximum'(string,string)
		call void ['mscorlib']'System'.'Console'::'WriteLine'(string)
		ret
	}
}
