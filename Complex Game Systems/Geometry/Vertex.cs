using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

public struct Vertex
{
    public const int size = (3 + 4 + 3 + 3 + 2) * 4;

    public readonly Vector3 position;
    public readonly Color4 colour;

    public readonly Vector3 normal;
    public readonly Vector3 tangent;

    public readonly Vector2 textureUV;

    public Vertex(Vector3 position, Color4 colour)
    {
        this.position = position;
        this.colour = colour;

        normal = new Vector3(0f, 1f, 0f);
        tangent = Vector3.Zero;

        textureUV = Vector2.Zero;
    }
    public Vertex(
        Vector3 position, Color4 colour, Vector3 normal, Vector3 tangent, Vector2 textureUV)
    {
        this.position = position;
        this.colour = colour;

        this.normal = normal;
        this.tangent = tangent;

        this.textureUV = textureUV;
    }
}

