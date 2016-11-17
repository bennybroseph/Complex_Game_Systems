namespace Geometry
{
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    public class Model<TVertex> where TVertex : struct
    {
        public Transform transform { get; set; } = new Transform();

        public Mesh<TVertex> mesh { get; set; }

        public ShaderProgram shader { get; set; }

        public Texture normalTexture { get; set; }
        public Texture diffuseTexture { get; set; }
        public Texture specularTexture { get; set; }

        public Color4 materialColor { get; set; }

        public void Bind()
        {
            shader.Use();

            if (normalTexture != null)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                normalTexture.Bind();
            }
            if (diffuseTexture != null)
            {
                GL.ActiveTexture(TextureUnit.Texture1);
                diffuseTexture.Bind();
            }
            if (specularTexture != null)
            {
                GL.ActiveTexture(TextureUnit.Texture2);
                specularTexture.Bind();
            }

            mesh.Bind();
        }
        public void UnBind()
        {
            Texture.UnBind();

            mesh.UnBind();

            GL.UseProgram(0);
        }

        public void BufferData()
        {
            shader.Use();

            if (normalTexture != null)
            {
                normalTexture.BufferData();

                var location = shader.GetUniformLocation("normalMap");
                GL.Uniform1(location, 0);
            }
            if (diffuseTexture != null)
            {
                diffuseTexture.BufferData();

                var location = shader.GetUniformLocation("diffuseMap");
                GL.Uniform1(location, 1);
            }
            if (specularTexture != null)
            {
                specularTexture.BufferData();

                var location = shader.GetUniformLocation("specularMap");
                GL.Uniform1(location, 2);
            }

            mesh.BufferData(shader);
        }

        public void Draw()
        {
            shader.Use();

            // get uniform location
            var location = shader.GetUniformLocation("projectionMatrix");
            var tempMatrix = transform.worldSpaceMatrix * Camera.main.viewProjection; // why though

            // set uniform value
            GL.UniformMatrix4(location, false, ref tempMatrix);

            mesh.Draw();
        }
    }
}
