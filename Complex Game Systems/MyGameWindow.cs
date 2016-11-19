using System;
using System.Collections.Generic;
using System.IO;

using Geometry;
using Geometry.Shapes;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using UI;

using Utility;

public class MyGameWindow : GameWindow
{
    private readonly List<Model<Vertex>> m_Models = new List<Model<Vertex>>();

    private Model<Vertex> m_Sun;
    private Model<Vertex> m_Earth;
    private Model<Vertex> m_Moon;

    private Canvas m_Canvas;

    private Texture m_Texture;

    private Camera m_Camera;

    public static MyGameWindow main { get; protected set; }

    public MyGameWindow() :
        this(
        1600,
        900,
        GraphicsMode.Default,
        "Test Window",
        GameWindowFlags.Default,
        DisplayDevice.Default,
        3,
        0,
        GraphicsContextFlags.Default)
    {
        if (main == null)
            main = this;
    }
    public MyGameWindow(
        int width, int height,
        GraphicsMode mode,
        string title,
        GameWindowFlags flags,
        DisplayDevice displayDevice,
        int major = 3, int minor = 0,
        GraphicsContextFlags contextFlags = GraphicsContextFlags.ForwardCompatible) :

            base(width, height, mode, title, flags, displayDevice, major, minor, contextFlags)
    {
        Debug.Log("OpenGL Version: " + GL.GetString(StringName.Version));
        if (main == null)
            main = this;
    }

    protected override void OnResize(EventArgs eventArgs)
    {
        base.OnResize(eventArgs);

        GL.Viewport(0, 0, Width, Height);
    }

    protected override void OnLoad(EventArgs eventArgs)
    {
        base.OnLoad(eventArgs);

        GL.ClearColor(Color4.DimGray);
        GL.DebugMessageCallback(OnDebugMessage, IntPtr.Zero);

        Time.Init();
        ShaderProgram.Init();
        Gizmos.Init();

        Audio.Init();
        MusicPlayer.Init("Content\\Music\\Bomberman 64");

        m_Canvas = new Canvas(this);

        m_Texture = new Texture(
            "Content\\Pictures\\Brock.jpg", TextureMinFilter.Nearest, TextureMagFilter.Nearest);

        var animation =
            new Animation(
                TextureMinFilter.Nearest,
                TextureMagFilter.Nearest,
                "Content\\Pictures\\CartoonDancing.gif");
        animation.BufferData();

        var button = new Button(m_Canvas, animation, null, null);
        button.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
        //button.transform.localScale = new Vector3(100f, 100f, 100f);
        button.transform.position = new Vector3(500f, 500f, 0f);

        ShaderProgram.texture.Use();
        var location = ShaderProgram.texture.GetUniformLocation("lightDirection");
        GL.Uniform3(location, 1f, 0.5f, 0f);

        var mesh = Plane.GetMesh();
        m_Sun = new Model<Vertex>
        {
            mesh = mesh,
            shader = ShaderProgram.texture,
            diffuseTexture = m_Texture
        };
        m_Sun.Bind();
        m_Sun.BufferData();
        m_Sun.UnBind();

        m_Models.Add(m_Sun);

        m_Earth = new Model<Vertex>
        {
            mesh = mesh,
            shader = ShaderProgram.texture,
            diffuseTexture = m_Texture
        };
        m_Earth.Bind();
        m_Earth.BufferData();
        m_Earth.UnBind();

        m_Earth.transform.SetParent(m_Sun.transform);

        m_Earth.transform.Translate(new Vector3(1.5f, 0f, 0f));

        m_Models.Add(m_Earth);

        m_Moon = new Model<Vertex>
        {
            mesh = mesh,
            shader = ShaderProgram.texture,
            diffuseTexture = m_Texture
        };
        m_Moon.Bind();
        m_Moon.BufferData();
        m_Moon.UnBind();

        m_Moon.transform.Translate(new Vector3(1.5f, 0f, 0f));
        m_Moon.transform.Scale(new Vector3(0.5f, 0.5f, 0.5f));
        m_Moon.transform.SetParent(m_Earth.transform);

        m_Models.Add(m_Moon);

        m_Camera = new StaticCamera();
        m_Camera.SetLookAt(new Vector3(0f, 5f, 10f), Vector3.Zero, new Vector3(0f, 1f, 0f));
        m_Camera.SetPerspective(MathHelper.PiOver4, Width / (float)Height, 0.1f, 75f);

        MusicPlayer.Play();

        RenderFrame += OnRenderFrameEvent;
    }

    protected override void OnUpdateFrame(FrameEventArgs eventArgs)
    {
        base.OnUpdateFrame(eventArgs);

        m_Camera.Update();
        MusicPlayer.Update();

        m_Sun.transform.position =
            new Vector3(
                m_Sun.transform.position.X - 0.01f, m_Sun.transform.position.Y, m_Sun.transform.position.Z);
        m_Sun.transform.eulerAngles =
            new Vector3(
                m_Sun.transform.eulerAngles.X + 1f, m_Sun.transform.eulerAngles.Y, m_Sun.transform.eulerAngles.Z);

        m_Moon.transform.localScale = new Vector3(2.0f, 1.1f, 1.1f);
        m_Moon.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        m_Moon.transform.position = new Vector3(2f, 2f, 0f);

        m_Canvas.Update();
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        MusicPlayer.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);
        MusicPlayer.OnMouseMove(e);
    }

    protected void OnDebugMessage(
        DebugSource source, DebugType type,
        int num1, DebugSeverity severity, int num2, IntPtr ptr1, IntPtr ptr2)
    {
        Debug.Log("OpenGL Debug Message: " + source + " " + type);
    }

    private void OnRenderFrameEvent(object o, FrameEventArgs eventArgs)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        foreach (var model in m_Models)
        {
            model.Bind();
            {
                model.Draw();
            }
            model.UnBind();
        }

        MusicPlayer.Draw();

        m_Canvas.Draw();

        SwapBuffers();
    }
}