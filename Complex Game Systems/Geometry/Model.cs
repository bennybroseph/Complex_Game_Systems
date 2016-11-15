namespace ComplexGameSystems.Geometry
{
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    public class Model
    {
        public Mesh<Vertex> mesh { get; set; }

        public ShaderProgram shader { get; set; }

        public Texture normalTexture { get; set; }
        public Texture diffuseTexture { get; set; }
        public Texture specularTexture { get; set; }

        public Color4 materialColor { get; set; }

        public void Bind()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            normalTexture.Bind();
            GL.ActiveTexture(TextureUnit.Texture1);
            diffuseTexture.Bind();
            GL.ActiveTexture(TextureUnit.Texture2);
            specularTexture.Bind();

            mesh.Bind();
        }
        public void UnBind()
        {
            normalTexture.UnBind();
            mesh.UnBind();
        }

        public void Draw()
        {
            mesh.Draw();
        }
    }
}
