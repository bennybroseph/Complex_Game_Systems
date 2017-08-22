namespace Geometry.Shapes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    internal class Sphere
    {
        private const float DEFAULT_RADIUS = 0.5f;
        private const float DEFAULT_SEGMENTS = 27f;

        private static readonly Dictionary<string, Mesh<Vertex>> s_Meshes =
            new Dictionary<string, Mesh<Vertex>>();

        public static void Init()
        {
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.PrimitiveRestartIndex(0xFFFF);

            CreateMesh(DEFAULT_RADIUS, DEFAULT_SEGMENTS);
        }

        public static Mesh<Vertex> GetMesh(float radius = DEFAULT_RADIUS, float segments = DEFAULT_SEGMENTS)
        {
            var key = CreateKey(radius, segments);

            return s_Meshes.TryGetValue(key, out Mesh<Vertex> mesh) ? mesh : CreateMesh(radius, segments);
        }

        private static IEnumerable<Vertex> GenVertexes(float radius, float segments)
        {
            var vertexes = new List<Vertex>();
            for (var i = 0d; i <= segments; i++)
            {
                var phi = 2 * Math.PI * (i / segments);

                foreach (var baseVertex in GenHalfCircle(radius, segments))
                {
                    var vertex =
                        new Vertex(
                            new Vector3(
                                baseVertex.position.X * (float)Math.Cos(phi) +
                                    baseVertex.position.Z * -(float)Math.Sin(phi),
                                baseVertex.position.Y,
                                baseVertex.position.X * (float)Math.Sin(phi) +
                                    baseVertex.position.Z * (float)Math.Cos(phi)),
                            baseVertex.color);

                    vertex.normal = vertex.position.Normalized();

                    //vertex.textureUV.X = (float)Math.Asin(vertex.normal.X) / (float)Math.PI + 0.5f;
                    //vertex.textureUV.X = (float)Math.Asin(vertex.normal.Y) / (float)Math.PI + 0.5f;

                    var d = vertex.position.Normalized();
                    vertex.textureUV.X = 0.5f + (float)Math.Atan2(d.Z, d.X) / (2 * (float)Math.PI);
                    vertex.textureUV.Y = 0.5f - (float)Math.Asin(d.Y) / (float)Math.PI;

                    vertexes.Add(vertex);
                }
            }

            for (var i = 0; i < vertexes.Count; i += 3)
            {
                var v0 = vertexes[i + 0];
                var v1 = vertexes[i + 1];
                var v2 = vertexes[i + 2];

                var pos0 = v0.position;
                var pos1 = v1.position;
                var pos2 = v2.position;

                var uv0 = v0.textureUV;
                var uv1 = v1.textureUV;
                var uv2 = v2.textureUV;

                var deltaPos1 = pos1 - pos0;
                var deltaPos2 = pos2 - pos0;

                var deltaUV1 = uv1 - uv0;
                var deltaUV2 = uv2 - uv0;

                var r = 1f / (deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X);
                var tangent = (deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) * r;
                var biTangent = (deltaPos2 * deltaUV1.X - deltaPos1 * deltaUV2.X) * r;

                v0.tangent = tangent;
                v1.tangent = tangent;
                v2.tangent = tangent;

                v0.biTangent = biTangent;
                v1.biTangent = biTangent;
                v2.biTangent = biTangent;

                yield return v0;
                yield return v1;
                yield return v2;
            }
        }

        private static IEnumerable<Vertex> GenHalfCircle(float radius, float segments)
        {
            for (var i = 0d; i < segments; ++i)
            {
                var theta = Math.PI * i / (segments - 1d);
                yield return
                    new Vertex(
                        new Vector3((float)Math.Sin(theta) * radius, (float)Math.Cos(theta) * radius, 0f),
                        new Color4(1f, 1f, 1f, 1f));
            }
        }

        private static IEnumerable<uint> GenIndexes(float segments, float points)
        {
            for (var i = 0u; i < segments; ++i)
            {
                var start = i * (uint)points;
                for (var j = 0u; j < points; j++)
                {
                    var botR = start + (uint)points + j;
                    var botL = start + j;

                    yield return botL;
                    yield return botR;
                }
                yield return 0xFFFF;
            }
        }

        public static string CreateKey(float radius, float segments) { return radius + " " + segments; }

        public static Mesh<Vertex> CreateMesh(float radius, float segments)
        {
            var key = CreateKey(radius, segments);

            if (s_Meshes.TryGetValue(key, out Mesh<Vertex> mesh))
                return mesh;

            var newMesh =
                new Mesh<Vertex>(
                    PrimitiveType.TriangleStrip,
                    GenVertexes(radius, segments), Vertex.size,
                    GenIndexes(segments, segments),
                    new VertexAttribute("inPosition", 3, Vertex.size, 0),
                    new VertexAttribute("inColor", 4, Vertex.size, 3 * 4),
                    new VertexAttribute("inNormal", 3, Vertex.size, 3 * 4 + 4 * 4),
                    new VertexAttribute("inTextureUV", 2, Vertex.size, 3 * 4 + 4 * 4 + 3 * 4),
                    new VertexAttribute("inTangent", 3, Vertex.size, 3 * 4 + 4 * 4 + 3 * 4 + 2 * 4),
                    new VertexAttribute("inBiTangent", 3, Vertex.size, 3 * 4 + 4 * 4 + 3 * 4 + 2 * 4 + 3 * 4));

            s_Meshes.Add(key, newMesh);
            return newMesh;
        }
    }
}
