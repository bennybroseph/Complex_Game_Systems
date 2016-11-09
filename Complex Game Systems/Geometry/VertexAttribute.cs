using OpenTK.Graphics.OpenGL;

namespace ComplexGameSystems.Geometry
{
    sealed class VertexAttribute
    {
        private readonly string m_Name;
        private readonly int m_Size;
        private readonly VertexAttribPointerType m_Type;
        private readonly bool m_Normalize;
        private readonly int m_Stride;
        private readonly int m_Offset;

        public VertexAttribute(
            string name,
            int size,
            VertexAttribPointerType type,
            int stride,
            int offset,
            bool normalize = false)
        {
            m_Name = name;
            m_Size = size;
            m_Type = type;
            m_Stride = stride;
            m_Offset = offset;
            m_Normalize = normalize;
        }

        public void Set(ShaderProgram program)
        {
            // get location of attribute from shader program
            var index = program.GetAttributeLocation(m_Name);

            // enable and set attribute
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, m_Size, m_Type, m_Normalize, m_Stride, m_Offset);
        }
    }
}