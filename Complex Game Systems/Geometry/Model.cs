namespace ComplexGameSystems.Geometry
{
    using System;

    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    public class Model<TVertex> where TVertex : struct
    {
        public Matrix4 matrix { get; set; }

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
            if (normalTexture != null)
                normalTexture.BufferData();
            if (diffuseTexture != null)
                diffuseTexture.BufferData();
            if (specularTexture != null)
                specularTexture.BufferData();

            mesh.BufferData(shader);
        }

        public void Draw()
        {
            // get uniform location
            var location = shader.GetUniformLocation("ProjectionMatrix");

            var tempMatrix = Camera.main.modelViewProjection * matrix;
            //matrix *= Matrix4.CreateRotationX(0.01f);

            // set uniform value
            GL.UniformMatrix4(location, false, ref tempMatrix);

            mesh.Draw();
        }
    }
}
