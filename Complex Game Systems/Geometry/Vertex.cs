namespace Geometry
{
    using OpenTK;
    using OpenTK.Graphics;

    public struct Vertex
    {
        public const int size = (3 + 4 + 3 + 2 + 3 + 3) * 4;

        public Vector3 position;
        public Color4 color;

        public Vector3 normal;
        public Vector2 textureUV;

        public Vector3 tangent;
        public Vector3 biTangent;

        public Vertex(Vector3 position, Color4 color)
        {
            this.position = position;
            this.color = color;

            textureUV = new Vector2(position.X, position.Y);

            normal = new Vector3(0f, 1f, 0f);
            tangent = new Vector3(1f, 0f, 0f);
            biTangent = new Vector3(0f, 0f, 1f);
        }

        public Vertex(Vector3 position, Color4 color, Vector2 textureUV)
        {
            this.position = position;
            this.color = color;
            this.textureUV = textureUV;

            normal = new Vector3(0f, 1f, 0f);
            tangent = new Vector3(1f, 0f, 0f);
            biTangent = new Vector3(0f, 0f, 1f);
        }
        public Vertex(Vector3 position, Color4 color, Vector3 normal)
        {
            this.position = position;
            this.color = color;
            this.normal = normal;

            textureUV = new Vector2(position.X, position.Y);
            tangent = new Vector3(1f, 0f, 0f);
            biTangent = new Vector3(0f, 0f, 1f);
        }
        public Vertex(Vector3 position, Color4 color, Vector3 normal, Vector2 textureUV)
        {
            this.position = position;
            this.color = color;
            this.normal = normal;
            this.textureUV = textureUV;

            tangent = new Vector3(1f, 0f, 0f);
            biTangent = new Vector3(0f, 0f, 1f);
        }
        public Vertex(
            Vector3 position,
            Color4 color,
            Vector3 normal,
            Vector2 textureUV,
            Vector3 tangent,
            Vector3 biTangent)
        {
            this.position = position;
            this.color = color;

            this.normal = normal;
            this.textureUV = textureUV;

            this.tangent = tangent;
            this.biTangent = biTangent;
        }
    }
}

