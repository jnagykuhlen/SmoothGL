using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using OpenTK;

using SmoothGL.Graphics;


namespace SmoothGL.Content
{
    /// <summary>
    /// Reader class which loads mesh data from a stream, in accordance with the wavefront OBJ specification.
    /// </summary>
    public class WavefrontOBJReader : IContentReader<MeshData>
    {
        /// <summary>
        /// Reads mesh data from a stream.
        /// </summary>
        /// <param name="stream">Stream to read data from.</param>
        /// <param name="requestedType">The concrete type requested. Should be the specified type or subtypes.</param>
        /// <param name="contentManager">Content manager used to load additional data.</param>
        /// <returns>The read object.</returns>
        public MeshData Read(Stream stream, Type requestedType, ContentManager contentManager)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> textureCoordinates = new List<Vector2>();
            List<uint> indices = new List<uint>();

            StreamReader reader = new StreamReader(stream);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("vt"))
                {
                    textureCoordinates.Add(ParseTextureCoordinate(line.Substring(2, line.Length - 2).Trim()));
                }
                else if (line.StartsWith("vn"))
                {
                    normals.Add(ParseNormal(line.Substring(2, line.Length - 2).Trim()));
                }
                else if (line.StartsWith("v"))
                {
                    vertices.Add(ParseVertex(line.Substring(1, line.Length - 1).Trim()));
                }
                else if (line.StartsWith("f"))
                {
                    indices.AddRange(ParseFace(line.Substring(1, line.Length - 1).Trim()));
                }
            }

            return new MeshData(vertices.ToArray(), normals.ToArray(), textureCoordinates.ToArray(), indices.ToArray());
        }

        private float[] ParseVector(string text, int numberOfComponents)
        {
            string[] componentStrings = text.Split(' ');
            if (componentStrings.Length < numberOfComponents)
                throw new InvalidDataException("Wrong Wavefront OBJ file format.");

            try
            {
                return componentStrings.Where(s => !string.IsNullOrWhiteSpace(s)).Take(numberOfComponents)
                                       .Select(s => float.Parse(s.Trim(), CultureInfo.InvariantCulture))
                                       .ToArray();
            }
            catch (FormatException formatException)
            {
                throw new InvalidDataException(string.Format("Unable to parse vector string \"{0}\".", text), formatException);
            }
        }

        private Vector3 ParseVertex(string text)
        {
            float[] components = ParseVector(text, 3);
            return new Vector3(components[0], components[1], components[2]);
        }

        private Vector3 ParseNormal(string text)
        {
            float[] components = ParseVector(text, 3);
            return Vector3.Normalize(new Vector3(components[0], components[1], components[2]));
        }

        private Vector2 ParseTextureCoordinate(string text)
        {
            float[] components = ParseVector(text, 2);
            return new Vector2(components[0], components[1]);
        }

        private IEnumerable<uint> ParseFace(string text)
        {
            string[] indexStrings = text.Split(' ');
            if (indexStrings.Length < 3)
                throw new InvalidDataException("Wrong Wavefront OBJ file format.");

            return Triangulate(indexStrings.Select(s => ParseIndex(s.Trim())));
        }

        private IEnumerable<uint> Triangulate(IEnumerable<uint> indices)
        {
            uint first = indices.ElementAt(0);
            uint last = indices.ElementAt(1);

            foreach (uint current in indices.Skip(2))
            {
                yield return first;
                yield return last;
                yield return current;

                last = current;
            }
        }

        private uint ParseIndex(string text)
        {
            uint[] indices = text.Split('/').Where(s => !string.IsNullOrWhiteSpace(s))
                                            .Select(s => uint.Parse(s.Trim(), CultureInfo.InvariantCulture))
                                            .ToArray();

            if (indices.Any(i => i != indices[0]))
                throw new InvalidDataException("Wavefront OBJ loader does not support different indices for vertex components.");

            return indices[0] - 1;
        }

        /// <summary>
        /// Indicates whether this class can also read subtypes of the specified type.
        /// </summary>
        public bool CanReadSubtypes
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the name of this reader.
        /// </summary>
        public string ReaderName
        {
            get
            {
                return "MeshDataReader";
            }
        }
    }
}
