using System.Text.RegularExpressions;

namespace InBodyPDFExtractor.Extensions;

public static class RegexExtension
{
    public static bool IsMatch(this Regex regex, string input, out Match match)
    {
        match = regex.Match(input);
        return match.Success;
    }
}