class Hello {
    public static void Main() {
        var result = 0;
        var m = 3;
        switch (m) {
        case 2:
            result = 2;
            break;
        case 1:
            result = 3;
            break;
        default:
            result = 4;
            break;
        }
    }
}