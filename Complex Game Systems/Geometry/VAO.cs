namespace ComplexGameSystems.Geometry
{
    using System.Collections.Generic;
    using System.Linq;

    using OpenTK.Graphics.OpenGL;

    public sealed class VAO<TVertex> where TVertex : struct
    {
        private readonly int m_Handle;

        private readonly VBO<TVertex> m_VBO;

        private readonly List<VertexAttribute> m_VertexAttributes;

        public VAO(VBO<TVertex> vbo, params VertexAttribute[] attributes)
        {
            // create new vertex array object
            m_Handle = GL.GenVertexArray();

            m_VBO = vbo;

            m_VertexAttributes = attributes.ToList();
        }

        public void Bind()
        {
            // bind for usage (modification or rendering)
            GL.BindVertexArray(m_Handle);
        }
        public void UnBind()
        {
            GL.BindVertexArray(0);
        }

        public void BufferData(ShaderProgram program)
        {
            // set all attributes
            foreach (var attribute in m_VertexAttributes)
                attribute.Set(program);
        }
    }
}
