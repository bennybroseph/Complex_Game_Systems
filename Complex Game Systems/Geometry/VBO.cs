using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK.Graphics.OpenGL;

namespace ComplexGameSystems.Geometry
{
    public sealed class VBO<TVertex> where TVertex : struct
    {
        private readonly int m_VertexSize;
        private readonly List<TVertex> m_Vertexes;

        private readonly int m_Handle;

        public VBO(IEnumerable<TVertex> vertexes, int vertexSize)
        {
            m_VertexSize = vertexSize;
            m_Vertexes = vertexes.ToList();

            m_Handle = GL.GenBuffer();
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_Handle);
        }

        public void BufferData()
        {
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(m_VertexSize * m_Vertexes.Count),
                m_Vertexes.ToArray(),
                BufferUsageHint.StreamDraw);
        }

        public void Draw()
        {
            GL.DrawArrays(PrimitiveType.Triangles, 0, m_Vertexes.Count);
        }
    }
}
