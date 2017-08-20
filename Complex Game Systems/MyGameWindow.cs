using System;
using System.Collections.Generic;
using BroEngine;

using BroEngineEditor;

using Geometry;
using Geometry.Shapes;

using ImGuiUtility;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using UI;

using Utility;

using Object = BroEngine.Object;

public class MyGameWindow : GameWindow
{
    private readonly List<MeshRenderer<Vertex>> m_MeshRenderers = new List<MeshRenderer<Vertex>>();

    private GameObject m_Sun;
    private GameObject m_Earth;
    private GameObject m_Moon;

    private GameObject m_MainCamera = new GameObject("Main Camera");
    private GameObject m_TestObject = new GameObject("Test Object");

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

        GL.Enable(EnableCap.Blend);
        GL.Enable(EnableCap.DepthTest);
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

        Time.Init();
        ShaderProgram.Init();
        Gizmos.Init();

        Audio.Init();
        MusicPlayer.Init("Content\\Music\\Songs");

        ImGuiOpenTK.Init(this);

        //ImGuiOpenTK.drawEvent += new ResizableWindow().DrawGui;

        MainMenu.Init();
        Inspector.Init();
        Hierarchy.Init();

        m_Canvas = new Canvas(this);

        m_Texture =
            new Texture("Content\\Pictures\\Brock.jpg", TextureMinFilter.Nearest, TextureMagFilter.Nearest);

        var duane =
            new Animation(
                TextureMinFilter.Nearest,
                TextureMagFilter.Nearest,
                "Content\\Pictures\\Duane.gif");
        duane.BufferData();

        var sheGotTheMoves =
            new Animation(
                TextureMinFilter.Nearest,
                TextureMagFilter.Nearest,
                "Content\\Pictures\\SheGotTheMoves.gif");
        sheGotTheMoves.BufferData();

        //var dancing = new Image(m_Canvas, duane, null, sheGotTheMoves);
        //dancing.transform.position =
        //    new Vector3(dancing.transform.localScale.X /2f, dancing.transform.localScale.Y / 2f, 0f);

        var playTexture =
            new Texture(
                "Content\\Pictures\\MediaPlayer\\Play.png",
                TextureMinFilter.Linear, TextureMagFilter.Linear);
        var nextTexture =
            new Texture(
                "Content\\Pictures\\MediaPlayer\\Next.png",
                TextureMinFilter.Linear, TextureMagFilter.Linear);
        var previousTexture =
            new Texture(
                "Content\\Pictures\\MediaPlayer\\Previous.png",
                TextureMinFilter.Linear, TextureMagFilter.Linear);

        var play =
            new Button(
                m_Canvas,
                () => MusicPlayer.TogglePause(), playTexture, null, null);
        play.transform.localScale = new Vector3(50f, 50f, 1f);
        play.transform.position =
            new Vector3(Width / 2f, -10f + Height - play.transform.localScale.Y / 2f, 0f);

        var next =
            new Button(
                m_Canvas,
                () => MusicPlayer.NextTrack(), nextTexture, null, null);
        next.transform.localScale = new Vector3(50f, 50f, 1f);
        next.transform.position =
            new Vector3(
                play.transform.position.X + 100f, -10f + Height - next.transform.localScale.Y / 2f, 0f);

        var previous =
            new Button(
                m_Canvas,
                () => MusicPlayer.PreviousTrack(), previousTexture, null, null);
        previous.transform.localScale = new Vector3(50f, 50f, 1f);
        previous.transform.position =
            new Vector3(
                play.transform.position.X - 100f, -10f + Height - previous.transform.localScale.Y / 2f, 0f);

        ShaderProgram.texture.Use();
        var location = ShaderProgram.texture.GetUniformLocation("lightDirection");
        GL.Uniform3(location, 1f, 0.5f, 0f);

        var mesh = Plane.GetMesh();

        m_Sun = new GameObject("Sun");

        var sunMeshFilter = m_Sun.AddComponent<MeshFilter<Vertex>>();
        sunMeshFilter.mesh = mesh;

        var sunMeshRenderer = m_Sun.AddComponent<MeshRenderer<Vertex>>();
        sunMeshRenderer.shader = ShaderProgram.texture;
        sunMeshRenderer.diffuseTexture = m_Texture;

        sunMeshRenderer.Bind();
        sunMeshRenderer.BufferData();
        sunMeshRenderer.UnBind();

        m_MeshRenderers.Add(sunMeshRenderer);

        m_Earth = new GameObject("Earth");

        var earthMeshFilter = m_Earth.AddComponent<MeshFilter<Vertex>>();
        earthMeshFilter.mesh = mesh;

        var earthMeshRenderer = m_Earth.AddComponent<MeshRenderer<Vertex>>();
        earthMeshRenderer.shader = ShaderProgram.texture;
        earthMeshRenderer.diffuseTexture = duane;

        earthMeshRenderer.Bind();
        earthMeshRenderer.BufferData();
        earthMeshRenderer.UnBind();

        m_Earth.transform.SetParent(m_Sun.transform);

        m_Earth.transform.Translate(new Vector3(1.5f, 0f, 0f));

        m_MeshRenderers.Add(earthMeshRenderer);

        m_Moon = new GameObject("Moon");

        var moonMeshFilter = m_Moon.AddComponent<MeshFilter<Vertex>>();
        moonMeshFilter.mesh = mesh;

        var moonMeshRenderer = m_Moon.AddComponent<MeshRenderer<Vertex>>();
        moonMeshRenderer.shader = ShaderProgram.texture;
        moonMeshRenderer.diffuseTexture = sheGotTheMoves;

        moonMeshRenderer.Bind();
        moonMeshRenderer.BufferData();
        moonMeshRenderer.UnBind();

        m_Moon.transform.Translate(new Vector3(1.5f, 0f, 0f));
        m_Moon.transform.Scale(new Vector3(0.5f, 0.5f, 0.5f));
        m_Moon.transform.SetParent(m_Earth.transform);

        m_MeshRenderers.Add(moonMeshRenderer);

        m_Camera = new StaticCamera();
        m_Camera.SetLookAt(new Vector3(0f, 5f, 10f), Vector3.Zero, new Vector3(0f, 1f, 0f));
        m_Camera.SetPerspective(MathHelper.PiOver4, Width / (float)Height, 0.1f, 75f);

        var camera = m_MainCamera.AddComponent<BroEngine.Camera>();
        camera.SetLookAt(new Vector3(0f, 5f, 10f), Vector3.Zero, new Vector3(0f, 1f, 0f));
        m_MainCamera.tag = "Main Camera";

        Inspector.selectedObject = m_MainCamera;

        RenderFrame += OnRenderFrameEvent;
    }

    protected override void OnUpdateFrame(FrameEventArgs eventArgs)
    {
        base.OnUpdateFrame(eventArgs);

        m_Camera.Update();
        MusicPlayer.Update();

        //m_Sun.transform.position =
        //    new Vector3(
        //        m_Sun.transform.position.X - 0.01f, m_Sun.transform.position.Y, m_Sun.transform.position.Z);
        m_Sun.transform.eulerAngles =
            new Vector3(
                m_Sun.transform.eulerAngles.X + 1f,
                m_Sun.transform.eulerAngles.Y,
                m_Sun.transform.eulerAngles.Z);

        m_Moon.transform.localScale = new Vector3(7.0f, 1.1f, 1.1f);
        m_Moon.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        m_Moon.transform.position = new Vector3(2f, 2f, 0f);

        m_Canvas.Update();
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

        Gizmos.DrawCube(Vector3.Zero, Vector3.One);

        foreach (var camera in BroEngine.Camera.allCameras)
        {
            camera.Render();

            if (!camera.enabled)
                continue;

            Gizmos.Render(camera.viewProjection);

            MusicPlayer.Draw();
            m_Canvas.Draw();
        }

        GL.Viewport(0, 0, Width, Height);
        ImGuiOpenTK.RenderFrame();

        SwapBuffers();

        Gizmos.ClearDrawCalls();
    }
}