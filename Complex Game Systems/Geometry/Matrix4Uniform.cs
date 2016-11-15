using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ComplexGameSystems.Geometry
{
    sealed class Matrix4Uniform
    {
        private readonly string m_Name;
        private Matrix4 m_Matrix;

        public Matrix4 matrix { get { return m_Matrix; } set { m_Matrix = value; } }

        public Matrix4Uniform(string name)
        {
            this.m_Name = name;
        }

        public void Set(ShaderProgram program)
        {
            // get uniform location
            var location = program.GetUniformLocation(m_Name);

            var mvp = m_Matrix * Matrix4.CreateRotationX(-45f);

            // set uniform value
            GL.UniformMatrix4(location, false, ref mvp);
        }
    }
}