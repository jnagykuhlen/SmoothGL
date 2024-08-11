using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmoothGL.Content;
using SmoothGL.Content.Readers;
using SmoothGL.Graphics.Geometry;

namespace SmoothGL.Tests;

[TestClass]
public class WavefrontObjReaderTest
{
    [TestMethod]
    public void TestLoadMeshData()
    {
        var contentManager = new ContentManager("");
        contentManager.SetContentReader(new WavefrontObjReader());
        
        const string contents = """
                                vn 0.0 0.0 -1.0
                                v 1.0 0.9 0.0
                                vn 0.0 0.0 -1.0
                                v -1.0 0.9 0.0
                                vn 0.0 0.0 -1.0
                                v 0.0 -0.9 0.0
                                f 2//2 1//1 3//3
                                """;

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents));

        var meshData = contentManager.Load<MeshData>(stream);
        meshData.NumberOfVertices.Should().Be(3);
        meshData.NumberOfIndices.Should().Be(3);
    }
}