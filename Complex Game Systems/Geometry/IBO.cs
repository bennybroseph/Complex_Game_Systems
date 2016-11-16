namespace ComplexGameSystems.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OpenTK.Graphics.OpenGL;

    public class IBO
    {
        private readonly int m_Handle;

        private readonly List<uint> m_Indexes;

        public int size => m_Indexes.Count;

        public IBO(IEnumerable<uint> indexes)
        {
            m_Handle = GL.GenBuffer();

            m_Indexes = indexes.ToList();
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_Handle);
        }
        public static void UnBind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void BufferData()
        {
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                (IntPtr)(m_Indexes.Count * sizeof(uint)),
                m_Indexes.ToArray(),
                BufferUsageHint.StaticDraw);
        }
    }
}
