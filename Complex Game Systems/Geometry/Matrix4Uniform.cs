﻿using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ComplexGameSystems.Geometry
{
    sealed class Matrix4Uniform
    {
        private readonly string name;
        private Matrix4 matrix;

        public Matrix4 Matrix { get { return matrix; } set { matrix = value; } }

        public Matrix4Uniform(string name)
        {
            this.name = name;
        }

        public void Set(ShaderProgram program)
        {
            // get uniform location
            var i = program.GetUniformLocation(name);

            // set uniform value
            GL.UniformMatrix4(i, false, ref matrix);
        }
    }
}