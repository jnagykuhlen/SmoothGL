using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmoothGL.Content.Internal;

namespace SmoothGL.Tests;

[TestClass]
public class StringReplaceTest
{
    [TestMethod]
    public void TestReplaceRecursive()
    {
        const string input = """
                             This is some text
                             #include 1
                             #include 2
                             """;

        const string expected = """
                                This is some text
                                This is another text
                                This is another text
                                """;

        const string token = "#include";

        var result = StringReplace.ReplaceRecursive(input, token, argument =>
        {
            return argument switch
            {
                "1" => "#include 2",
                "2" => "#include 3",
                "3" => "This is another text",
                _ => ""
            };
        });

        result.Should().Be(expected);
    }
}