using System.IO;

using OpenTK.Graphics.OpenGL;

using Utility;

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
    public static ShaderProgram basic { get; private set; }
    public static ShaderProgram texture { get; private set; }

    public int handle { get; }

    public static void Init()
    {
        var vertShaderCode = File.ReadAllText("Shaders/Default.vert");
        var vertShader = new Shader(ShaderType.VertexShader, vertShaderCode);

        var fragShaderCode = File.ReadAllText("Shaders/Default.frag");
        var fragShader = new Shader(ShaderType.FragmentShader, fragShaderCode);

        basic = new ShaderProgram(vertShader, fragShader);

        vertShaderCode = File.ReadAllText("Shaders/Texture.vert");
        vertShader = new Shader(ShaderType.VertexShader, vertShaderCode);

        fragShaderCode = File.ReadAllText("Shaders/Texture.frag");
        fragShader = new Shader(ShaderType.FragmentShader, fragShaderCode);

        texture = new ShaderProgram(vertShader, fragShader);
    }

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