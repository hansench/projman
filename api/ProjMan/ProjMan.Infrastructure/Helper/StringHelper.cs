namespace ProjMan.Infrastructure.Helper;

public static class StringHelper
{
    public static int ToInt(this string numberString)
    {
        if (int.TryParse(numberString, out int numValue))
        {
            return numValue;
        }

        return 0;
    }

    public static string Enquote(this string input)
    {
        return $@"""{input}""";
    }


    public static string CapitalizeFirstChar(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        input = input.Trim();
        return input[0].ToString().ToUpperInvariant() + input.Substring(1);
    }
}
