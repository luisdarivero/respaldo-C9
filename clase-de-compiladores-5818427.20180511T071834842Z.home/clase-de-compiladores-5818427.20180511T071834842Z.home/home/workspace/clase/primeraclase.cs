using System;
using System.Text.RegularExpressions;
/// <summary>
/// Summary description for Class1
/// </summary>
public class Class1
{
	public Class1()
	{
        var regex = new Regex(@"([_A-Za_z]\w*)|(\d+)");
        var str = "hello 123 hi_234 0001";
        foreach(Match m in regex.Matches(str))
        {
            if (m.Groups[1].Success)
            {
                Console.WriteLine($"found an identifier: {m.Value}") + $"({m.Index},{m.Length})";
            } else if (m.Groups[2].Success)
            {
                Console.WriteLine($"found an integer literal: {m.Value}") + $"({m.Index},{m.Length})");
            }
        }
		//
		// TODO: Add constructor logic here
		//
	}
}