using OpenTK.Graphics.OpenGL;



public sealed class Shader
{
    private readonly int handle;

    public int Handle { get { return handle; } }

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
    private readonly int handle;

    public ShaderProgram(params Shader[] shaders)
    {
        // create program object
        handle = GL.CreateProgram();

        // assign all shaders
        foreach (var shader in shaders)
            GL.AttachShader(handle, shader.Handle);

        // link program (effectively compiles it)
        GL.LinkProgram(handle);

        // detach shaders
        foreach (var shader in shaders)
            GL.DetachShader(handle, shader.Handle);
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