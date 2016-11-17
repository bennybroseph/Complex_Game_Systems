using ComplexGameSystems.Utility;

using OpenTK.Graphics.OpenGL;

namespace ComplexGameSystems
{
    public sealed class Shader
    {
        public int handle { get; }

        public Shader(ShaderType type, string code)
        {
            // create shader object
            handle = GL.CreateShader(type);

            // set source and compile shader
            GL.ShaderSource(handle, code);
            GL.CompileShader(handle);
        }
    }

    public sealed class ShaderProgram
    {
        public int handle { get; }

        public ShaderProgram(params Shader[] shaders)
        {
            // create program object
            handle = GL.CreateProgram();

            // assign all shaders
            foreach (var shader in shaders)
                GL.AttachShader(handle, shader.handle);

            // link program (effectively compiles it)
            GL.LinkProgram(handle);

            int status;
            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out status);
            if (status == 0)
                Debug.Log(
                    string.Format("Error linking program: {0}", GL.GetProgramInfoLog(handle)));

            // detach shaders
            foreach (var shader in shaders)
                GL.DetachShader(handle, shader.handle);
        }

        public void Use()
        {
            // activate this program to be used
            GL.UseProgram(handle);
        }

        public int GetAttributeLocation(string name)
        {
            // get the location of a vertex attribute
            return GL.GetAttribLocation(handle, name);
        }

        public int GetUniformLocation(string name)
        {
            // get the location of a uniform variable
            return GL.GetUniformLocation(handle, name);
        }
    }
}