namespace UI
{
    using Geometry;
    using Geometry.Shapes;

    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public class Image : Element
    {
        protected Texture m_DefaultTexture;
        protected Texture m_HoveredTexture;
        protected Texture m_PushedTexture;

        protected Mesh<Vertex> m_Mesh;

        protected Texture currentTexture
        {
            get
            {
                var texture = m_State == ElementState.Default ? m_DefaultTexture : m_HoveredTexture;
                if (m_State == ElementState.Selected)
                    texture = m_PushedTexture;

                return texture;
            }
        }

        public Image(
            Canvas canvas,

            Texture defaultTexture,
            Texture highlighedTexture,
            Texture pushedTexture) : base(canvas)
        {
            m_DefaultTexture = defaultTexture;
            m_HoveredTexture = highlighedTexture;
            m_PushedTexture = pushedTexture;

            m_Mesh = Plane.GetMesh();

            Bind();
            BufferData();
            UnBind();
        }

        public void Bind()
        {
            ShaderProgram.texture.Use();

            GL.ActiveTexture(TextureUnit.Texture0);
            if (currentTexture != null)
                currentTexture.Bind();

            m_Mesh.Bind();
        }
        public void UnBind()
        {
            Texture.UnBind();

            m_Mesh.UnBind();

            GL.UseProgram(0);
        }

        public void BufferData()
        {
            if (m_DefaultTexture != null)
            {
                m_DefaultTexture.Bind();
                m_DefaultTexture.BufferData();

                transform.localScale =
                new Vector3(m_DefaultTexture.width, m_DefaultTexture.height, 1f);
            }

            if (m_HoveredTexture != null)
            {
                m_HoveredTexture.Bind();
                m_HoveredTexture.BufferData();
            }

            if (m_PushedTexture != null)
            {
                m_PushedTexture.Bind();
                m_PushedTexture.BufferData();
            }

            var location = ShaderProgram.texture.GetUniformLocation("diffuseMap");
            GL.Uniform1(location, 0);

            m_Mesh.BufferData(ShaderProgram.texture);
        }

        public override void Draw()
        {
            Bind();

            var location = ShaderProgram.texture.GetUniformLocation("diffuseMap");
            GL.Uniform1(location, 0);

            location = ShaderProgram.texture.GetUniformLocation("projectionMatrix");
            var tempMatrix = transform.worldSpaceMatrix * m_Canvas.projection;

            // set uniform value
            GL.UniformMatrix4(location, false, ref tempMatrix);

            m_Mesh.Draw();

            UnBind();
        }
    }
}
