using OpenTK.Graphics.OpenGL;

namespace ComplexGameSystems.Geometry
{
    sealed class VAO<TVertex> where TVertex : struct
    {
        private readonly int handle;

        public VAO(VBO<TVertex> vertexBuffer, ShaderProgram program, params VertexAttribute[] attributes)
        {
            // create new vertex array object
            GL.GenVertexArrays(1, out handle);

            // bind the object so we can modify it
            Bind();

            // bind the vertex buffer object
            vertexBuffer.Bind();

            // set all attributes
            foreach (var attribute in attributes)
                attribute.Set(program);

            // unbind objects to reset state
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Bind()
        {
            // bind for usage (modification or rendering)
            GL.BindVertexArray(handle);
        }
    }
}
