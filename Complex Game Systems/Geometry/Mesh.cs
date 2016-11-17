namespace Geometry
{
    using System;
    using System.Collections.Generic;

    using OpenTK.Graphics.OpenGL;

    public class Mesh<TVertex> where TVertex : struct
    {
        public VBO<TVertex> vbo { get; }
        public VAO<TVertex> vao { get; }
        public IBO ibo { get; }

        public PrimitiveType drawType { get; }

        public Mesh(
            PrimitiveType drawType,
            IEnumerable<TVertex> vertexes, int vertexSize,
            IEnumerable<uint> indexes,
            params VertexAttribute[] attributes)
        {
            this.drawType = drawType;

            vbo = new VBO<TVertex>(vertexes, vertexSize);
            vao = new VAO<TVertex>(vbo, attributes);
            ibo = new IBO(indexes);
        }

        public void Bind()
        {
            vao.Bind();
            vbo.Bind();
            ibo.Bind();
        }
        public void UnBind()
        {
            vao.UnBind();
            vbo.UnBind();
            IBO.UnBind();
        }

        public void BufferData(ShaderProgram shader)
        {
            vbo.BufferData();
            vao.BufferData(shader);
            ibo.BufferData();
        }

        public void Draw()
        {
            GL.DrawElements(drawType, ibo.size, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}
