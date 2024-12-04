using System.Collections.ObjectModel;
using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmoothGL.Content.Internal;

namespace SmoothGL.Tests;

[TestClass]
public class JsonElementExtensionTest
{
    [TestMethod]
    public void TestPopulateObject()
    {
        var target = new TestObject("Test Name", 3);

        var objectElement = JsonDocument.Parse("""{"name":"Another Test Name","unused":42}""").RootElement;
        objectElement.Populate(target);

        target.Should().Be(new TestObject("Another Test Name", 3));
    }

    [TestMethod]
    public void TestPopulateNestedObject()
    {
        var inner = new TestObject("Test Name", 3);
        var target = new NestedTestObject(inner, true);

        var objectElement = JsonDocument.Parse("""{"inner":{"name":"Another Test Name","value":5},"flag":true}""").RootElement;
        objectElement.Populate(target);

        target.Should().Be(new NestedTestObject(new TestObject("Another Test Name", 5), true));
        target.Inner.Should().BeSameAs(inner);
    }
    
    [TestMethod]
    public void TestPopulateReadOnlyCollection()
    {
        var target = new ReadOnlyCollection<TestObject>(new List<TestObject> { new("Test Name", 3) });

        var arrayElement = JsonDocument.Parse("""[{"name":"Another Test Name","value":5}, {"name":"Ignored","value":1}]""").RootElement;
        arrayElement.Populate(target);

        target.Should().Equal(new TestObject("Another Test Name", 5));
    }

    [TestMethod]
    public void TestPopulateListWithMoreElements()
    {
        var target = new List<TestObject> { new("Test Name", 3) };

        var arrayElement = JsonDocument.Parse("""[{"name":"Another Test Name","value":5}, {"name":"Yet Another Test Name","value":1}]""").RootElement;
        arrayElement.Populate(target);

        target.Should().Equal(new TestObject("Another Test Name", 5), new TestObject("Yet Another Test Name", 1));
    }

    [TestMethod]
    public void TestPopulateListWithLessElements()
    {
        var target = new List<TestObject> { new("Test Name", 3), new("Another Test Name", 5) };

        var arrayElement = JsonDocument.Parse("""[{"name":"Yet Another Test Name","value":1}]""").RootElement;
        arrayElement.Populate(target);

        target.Should().Equal(new TestObject("Yet Another Test Name", 1));
    }

    private record TestObject(string Name, int Value);

    private record NestedTestObject(TestObject Inner, bool Flag);
}