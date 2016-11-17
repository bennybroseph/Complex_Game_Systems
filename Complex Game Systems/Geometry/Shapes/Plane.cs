namespace Geometry.Shapes
{
    using System.Collections.Generic;
    using System.Globalization;

    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    public class Plane
    {
        private const float DEFAULT_SEGMENTS = 1f;

        private static Dictionary<string, Mesh<Vertex>> s_Meshes = new Dictionary<string, Mesh<Vertex>>();

        public static void Init()
        {
            CreateMeshIfNeeded();
        }

        public static Mesh<Vertex> GetMesh(float segments = DEFAULT_SEGMENTS)
        {
            var key = CreateKey(segments);
            CreateMeshIfNeeded(segments);

            return s_Meshes[key];
        }

        private static IEnumerable<Vertex> GenVertexes(float segments)
        {
            for (var x = 0f; x < segments; ++x)
            {
                for (var y = 0f; y < segments; ++y)
                {
                    yield return
                        new Vertex(
                            new Vector3((x - segments / 2f) / segments, 0f, (y - segments / 2f) / segments),
                            new Color4(1f, 1f, 1f, 1f),
                            new Vector2(x / segments, y / segments),
                            new Vector3(0f, 1f, 0f),
                            new Vector3(1f, 0f, 0f));

                    yield return
                        new Vertex(
                            new Vector3(
                                (x + 1f - segments / 2f) / segments, 0f, (y - segments / 2f) / segments),
                            new Color4(1f, 1f, 1f, 1f),
                            new Vector2((x + 1f) / segments, y / segments),
                            new Vector3(0f, 1f, 0f),
                            new Vector3(1f, 0f, 0f));

                    yield return
                        new Vertex(
                            new Vector3(
                                (x + 1f - segments / 2f) / segments, 0f, (y + 1f - segments / 2f) / segments),
                            new Color4(1f, 1f, 1f, 1f),
                            new Vector2((x + 1f) / segments, (y + 1f) / segments),
                            new Vector3(0f, 1f, 0f),
                            new Vector3(1f, 0f, 0f));

                    yield return
                        new Vertex(
                            new Vector3(
                                (x - segments / 2f) / segments, 0f, (y + 1f - segments / 2f) / segments),
                            new Color4(1f, 1f, 1f, 1f),
                            new Vector2(x / segments, (y + 1f) / segments),
                            new Vector3(0f, 1f, 0f),
                            new Vector3(1f, 0f, 0f));
                }
            }
        }
        private static IEnumerable<uint> GenIndexes(float segments)
        {
            for (uint i = 0; i < segments * segments * 4f; i += 4)
            {
                yield return i;
                yield return i + 1;
                yield return i + 2;
                yield return i;
                yield return i + 2;
                yield return i + 3;
            }
        }

        private static string CreateKey(float segments)
        {
            return segments.ToString(CultureInfo.InvariantCulture);
        }

        private static bool CreateMeshIfNeeded(float segments = DEFAULT_SEGMENTS)
        {
            var key = CreateKey(segments);

            if (s_Meshes.ContainsKey(key))
                return false;

            var mesh =
                new Mesh<Vertex>(
                    PrimitiveType.Triangles,
                    GenVertexes(segments), Vertex.size,
                    GenIndexes(segments),
                    new VertexAttribute("inPosition", 3, Vertex.size, 0),
                    new VertexAttribute("inColor", 4, Vertex.size, 3 * 4),
                    new VertexAttribute("inTextureUV", 2, Vertex.size, 3 * 4 + 4 * 4),
                    new VertexAttribute("inNormal", 3, Vertex.size, 3 * 4 + 4 * 4 + 2 * 4),
                    new VertexAttribute("inTangent", 2, Vertex.size, 3 * 4 + 4 * 4 + 2 * 4 + 3 * 4));

            s_Meshes.Add(key, mesh);

            return true;
        }
    }
}
