namespace Geometry
{
    using System.Collections.Generic;

    using OpenTK.Graphics.OpenGL;

    public sealed class VAO<TVertex> where TVertex : struct
    {
        private readonly int m_Handle;

        private readonly VBO<TVertex> m_VBO;

        private readonly IEnumerable<VertexAttribute> m_VertexAttributes;

        public VAO(VBO<TVertex> vbo, params VertexAttribute[] attributes)
        {
            // create new vertex array object
            GL.GenVertexArrays(1, out m_Handle);

            m_VBO = vbo;

            m_VertexAttributes = attributes;
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
