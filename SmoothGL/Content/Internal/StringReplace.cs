namespace SmoothGL.Content.Internal;

public static class StringReplace
{
    public static string ReplaceRecursive(string input, string token, Func<string, string> replacementFunction)
    {
        var startIndex = 0;
        while ((startIndex = input.IndexOf(token, startIndex, StringComparison.InvariantCulture)) >= 0)
        {
            var endIndex = input.IndexOfAny(['\n', '\r'], startIndex);
            if (endIndex < 0)
                endIndex = input.Length;

            var argumentIndex = startIndex + token.Length;
            var argument = input.Substring(argumentIndex, endIndex - argumentIndex).Trim();
            
            var replacement = replacementFunction(argument);
            input = input.Remove(startIndex, endIndex - startIndex).Insert(startIndex, replacement);
        }

        return input;
    }
}