namespace Geometry.Shapes
{
    using System.Collections.Generic;

    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    public class Quad : Plane
    {
        private static readonly Dictionary<string, Mesh<Vertex>> s_Meshes =
            new Dictionary<string, Mesh<Vertex>>();

        public new static void Init()
        {
            CreateMesh(DEFAULT_SEGMENTS);
        }

        public new static Mesh<Vertex> GetMesh(float segments = DEFAULT_SEGMENTS)
        {
            var key = CreateKey(segments);

            Mesh<Vertex> mesh;

            s_Meshes.TryGetValue(key, out mesh);
            if (mesh != null)
                return mesh;

            return CreateMesh(segments);
        }

        private static IEnumerable<Vertex> GenVertexes(float segments)
        {
            for (var x = 0f; x < segments; ++x)
            {
                for (var y = 0f; y < segments; ++y)
                {
                    // Top Left
                    yield return
                        new Vertex(
                            new Vector3((x - segments / 2f) / segments, (y - segments / 2f) / segments, 0f),
                            new Color4(1f, 1f, 1f, 1f),
                            new Vector3(0f, 0f, 1f),
                            new Vector2(x / segments, y / segments),
                            new Vector3(1f, 0f, 0f),
                            new Vector3(0f, 1f, 0f));

                    // Top Right
                    yield return
                        new Vertex(
                            new Vector3(
                                (x + 1f - segments / 2f) / segments, (y - segments / 2f) / segments, 0f),
                            new Color4(1f, 1f, 1f, 1f),
                            new Vector3(0f, 0f, 1f),
                            new Vector2((x + 1f) / segments, y / segments),
                            new Vector3(1f, 0f, 0f),
                            new Vector3(0f, 1f, 0f));

                    // Bottom Right
                    yield return
                        new Vertex(
                            new Vector3(
                                (x + 1f - segments / 2f) / segments, (y + 1f - segments / 2f) / segments, 0f),
                            new Color4(1f, 1f, 1f, 1f),
                            new Vector3(0f, 0f, 1f),
                            new Vector2((x + 1f) / segments, (y + 1f) / segments),
                            new Vector3(1f, 0f, 0f),
                            new Vector3(0f, 1f, 0f));

                    yield return
                        new Vertex(
                            new Vector3(
                                (x - segments / 2f) / segments, (y + 1f - segments / 2f) / segments, 0f),
                            new Color4(1f, 1f, 1f, 1f),
                            new Vector3(0f, 0f, 1f),
                            new Vector2(x / segments, (y + 1f) / segments),
                            new Vector3(1f, 0f, 0f),
                            new Vector3(0f, 1f, 0f));
                }
            }
        }

        private static Mesh<Vertex> CreateMesh(float segments)
        {
            var key = CreateKey(segments);

            var mesh =
                new Mesh<Vertex>(
                    PrimitiveType.Triangles,
                    GenVertexes(segments), Vertex.size,
                    GenIndexes(segments),
                    new VertexAttribute("inPosition", 3, Vertex.size, 0),
                    new VertexAttribute("inColor", 4, Vertex.size, 3 * 4),
                    new VertexAttribute("inNormal", 3, Vertex.size, 3 * 4 + 4 * 4),
                    new VertexAttribute("inTextureUV", 2, Vertex.size, 3 * 4 + 4 * 4 + 3 * 4),
                    new VertexAttribute("inTangent", 3, Vertex.size, 3 * 4 + 4 * 4 + 3 * 4 + 2 * 4),
                    new VertexAttribute("inBiTangent", 3, Vertex.size, 3 * 4 + 4 * 4 + 3 * 4 + 2 * 4 + 3 * 4));

            s_Meshes.Add(key, mesh);

            return mesh;
        }
    }
}
