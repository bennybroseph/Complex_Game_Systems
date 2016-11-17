namespace Geometry
{
    using OpenTK;
    using OpenTK.Graphics;

    public struct Vertex
    {
        public const int size = (3 + 4 + 3 + 3 + 2) * 4;

        public readonly Vector3 position;
        public readonly Color4 colour;

        public readonly Vector2 textureUV;

        public readonly Vector3 normal;
        public readonly Vector3 tangent;

        public Vertex(Vector3 position, Color4 colour)
        {
            this.position = position;
            this.colour = colour;

            textureUV = new Vector2(position.X, position.Y);

            normal = new Vector3(0f, 1f, 0f);
            tangent = new Vector3(1f, 0f, 0f);
        }
        public Vertex(
            Vector3 position, Color4 colour, Vector2 textureUV)
        {
            this.position = position;
            this.colour = colour;

            this.textureUV = textureUV;

            normal = new Vector3(0f, 1f, 0f);
            tangent = new Vector3(1f, 0f, 0f);
        }
        public Vertex(
            Vector3 position, Color4 colour, Vector2 textureUV, Vector3 normal, Vector3 tangent)
        {
            this.position = position;
            this.colour = colour;

            this.normal = normal;
            this.tangent = tangent;

            this.textureUV = textureUV;
        }
    }
}

