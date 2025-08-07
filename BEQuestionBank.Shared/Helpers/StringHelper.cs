using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BEQuestionBank.Shared.Helpers;

public static class StringHelper
{
    public static string GetTextInTag(string input, string tagName)
    {
        string tagRegex = @"<" + tagName + @"[^>]*>(.*?)<\/" + tagName + @">";
        MatchCollection matches = Regex.Matches(input, tagRegex, RegexOptions.Singleline);
        foreach(Match match in matches)
        {
            return match.Value.Replace("<" + tagName + ">", "").Replace("</" + tagName + ">", "");
        }    
        return string.Empty;
    }

    public static string CapitalizeWords(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException("value");
        }

        StringBuilder sb = new StringBuilder(value.Length);
        bool capitalizeNext = true;
        foreach (char c in value)
        {
            if (char.IsWhiteSpace(c) || char.IsPunctuation(c))
            {
                sb.Append(c);
                capitalizeNext = true;
            }
            else if (capitalizeNext)
            {
                sb.Append(char.ToUpper(c));
                capitalizeNext = false;
            }
            else
            {
                sb.Append(char.ToLower(c));
            }
        }
        return sb.ToString();
    }
}

