namespace Geometry
{
    using BroEngine;

    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    [DisallowMultipleComponent]
    public class MeshRenderer<TVertex> : Renderer where TVertex : struct
    {
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

                var location = shader.GetUniformLocation("normalMap");
                GL.Uniform1(location, 0);
            }
            if (diffuseTexture != null)
            {
                GL.ActiveTexture(TextureUnit.Texture1);
                diffuseTexture.Bind();

                var location = shader.GetUniformLocation("diffuseMap");
                GL.Uniform1(location, 1);
            }
            if (specularTexture != null)
            {
                GL.ActiveTexture(TextureUnit.Texture2);
                specularTexture.Bind();

                var location = shader.GetUniformLocation("specularMap");
                GL.Uniform1(location, 2);
            }

            GetComponent<MeshFilter<TVertex>>()?.mesh.Bind();
        }
        public void UnBind()
        {
            Texture.UnBind();

            GetComponent<MeshFilter<TVertex>>()?.mesh.UnBind();

            GL.UseProgram(0);
        }

        public void BufferData()
        {
            shader.Use();

            if (normalTexture != null)
                normalTexture.BufferData();
            if (diffuseTexture != null)
                diffuseTexture.BufferData();
            if (specularTexture != null)
                specularTexture.BufferData();

            GetComponent<MeshFilter<TVertex>>()?.mesh.BufferData(shader);
        }

        internal override void Render(Camera camera)
        {
            if (!enabled)
                return;

            Bind();
            {
                // get uniform location
                var location = shader.GetUniformLocation("projectionMatrix");
                var tempMatrix = transform.worldSpaceMatrix * camera.viewProjection;

                // set uniform value
                GL.UniformMatrix4(location, false, ref tempMatrix);

                GetComponent<MeshFilter<TVertex>>()?.mesh.Draw();
            }
            UnBind();
        }
    }
}
