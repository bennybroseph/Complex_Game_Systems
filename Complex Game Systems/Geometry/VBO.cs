namespace ComplexGameSystems.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OpenTK.Graphics.OpenGL;

    public sealed class VBO<TVertex> where TVertex : struct
    {
        private readonly int m_VertexSize;
        private readonly IEnumerable<TVertex> m_Vertexes;

        private readonly int m_Handle;

        public VBO(IEnumerable<TVertex> vertexes, int vertexSize)
        {
            m_VertexSize = vertexSize;
            m_Vertexes = vertexes;

            GL.GenBuffers(1, out m_Handle);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_Handle);
        }
        public void UnBind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void BufferData()
        {
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(m_Vertexes.Count() * m_VertexSize),
                m_Vertexes.ToArray(),
                BufferUsageHint.StaticDraw);
        }

        public void Draw()
        {
            GL.DrawArrays(PrimitiveType.Triangles, 0, m_Vertexes.Count());
        }
    }
}
