namespace ComplexGameSystems
{
    using System.Collections.Generic;

    using Geometry;

    using OpenTK.Graphics.OpenGL;

    public class Mesh<TVertex> where TVertex : struct
    {
        public VBO<TVertex> vbo { get; }
        public VAO<TVertex> vao { get; }
        public IBO ibo { get; }

        public BeginMode mode { get; }

        public Mesh(
            BeginMode mode,
            IEnumerable<TVertex> vertexes, int vertexSize,
            IEnumerable<uint> indexes,
            params VertexAttribute[] attributes)
        {
            this.mode = mode;

            vbo = new VBO<TVertex>(vertexes, vertexSize);
            vao = new VAO<TVertex>(vbo, attributes);
            ibo = new IBO(indexes);
        }

        public void Bind()
        {
            vbo.Bind();
            vao.Bind();
            ibo.Bind();
        }
        public void UnBind()
        {
            vbo.UnBind();
            vao.UnBind();
            ibo.UnBind();
        }

        public void BufferData(ShaderProgram shader)
        {
            vbo.BufferData();
            vao.BufferData(shader);
            ibo.BufferData();
        }

        public void Draw()
        {
            GL.DrawElements(mode, ibo.size, DrawElementsType.UnsignedInt, 0);
        }
    }
}
