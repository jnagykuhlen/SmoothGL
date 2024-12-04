using System.Collections;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmoothGL.Content.Internal;

namespace SmoothGL.Tests;

[TestClass]
public class EnumerableExtensionTest
{
    [TestMethod]
    public void TestZipWithRemainderFirstShorter()
    {
        var first = new List<int> { 1, 2 };
        var second = new List<int> { 3, 4, 5, 6 };

        first.ZipWithRemainder(second, out var firstRemainder, out var secondRemainder)
            .Should().Equal((1, 3), (2, 4));

        firstRemainder.Should().BeEmpty();
        secondRemainder.Should().Equal(5, 6);
    }

    [TestMethod]
    public void TestZipWithRemainderFirstLonger()
    {
        var first = new List<int> { 1, 2 };
        var second = new List<int> { 3 };

        first.ZipWithRemainder(second, out var firstRemainder, out var secondRemainder)
            .Should().Equal((1, 3));

        firstRemainder.Should().Equal(2);
        secondRemainder.Should().BeEmpty();
    }

    [TestMethod]
    public void TestGetItemTypeGenericCollection()
    {
        var enumerable = (IEnumerable<string>) ["A", "B", "C"];
        enumerable.GetItemType().Should().Be(typeof(string));
    }

    [TestMethod]
    public void TestGetItemTypeNonGenericCollection()
    {
        var stack = new Stack();
        stack.Push("A");

        stack.GetItemType().Should().BeNull();
    }
}
