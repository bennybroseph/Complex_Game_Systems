namespace ComplexGameSystems
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Geometry;
    using Geometry.Shapes;

    using Utility;

    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    public class GameWindow : OpenTK.GameWindow
    {
        private Mesh<Vertex> m_Mesh;

        private Texture m_Texture;

        private VBO<Vertex> m_VBO;
        private VAO<Vertex> m_VAO;

        private ShaderProgram m_ShaderProgram;

        private Matrix4Uniform m_ProjectionMatrixUniform;

        public delegate void OnEvent(GameWindow window, EventArgs eventArgs);

        public event OnEvent OnResizeEvent;

        public GameWindow() :
            this(
            1600, 900,
            GraphicsMode.Default,
            "Test Window",
            GameWindowFlags.Default,
            DisplayDevice.Default,
            3, 0,
            GraphicsContextFlags.Default)
        { }
        public GameWindow(
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
        }

        protected override void OnResize(EventArgs eventArgs)
        {
            GL.Viewport(0, 0, Width, Height);

            if (OnResizeEvent != null)
                OnResizeEvent.Invoke(this, eventArgs);
        }

        protected override void OnLoad(EventArgs eventArgs)
        {
            Audio.Init();
            MusicPlayer.Init("Content\\Music\\Bomberman 64\\");

            RenderFrame += OnRenderFrameEvent;

            var vertShaderCode = File.ReadAllText("Shaders/Texture.vert");
            var vertShader = new Shader(ShaderType.VertexShader, vertShaderCode);

            var fragShaderCode = File.ReadAllText("Shaders/Texture.frag");
            var fragShader = new Shader(ShaderType.FragmentShader, fragShaderCode);

            m_ShaderProgram = new ShaderProgram(vertShader, fragShader);

            m_Texture = new Texture(
                "Content\\Pictures\\gradient.png", TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            m_Texture.Bind();

            var location = m_ShaderProgram.GetUniformLocation("diffuseMap");
            GL.ProgramUniform1(m_ShaderProgram.handle, location, 0);

            location = m_ShaderProgram.GetUniformLocation("lightDirection");
            GL.ProgramUniform3(m_ShaderProgram.handle, location, 0, -1, 0);

            m_Mesh = Plane.GetMesh();

            m_Mesh.Bind();
            m_Mesh.BufferData(m_ShaderProgram);
            m_Mesh.UnBind();

            m_ProjectionMatrixUniform =
                new Matrix4Uniform("ProjectionMatrix")
                {
                    matrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 16f / 9, 0.1f, 100f)
                };

            MusicPlayer.Play();
        }

        protected override void OnUpdateFrame(FrameEventArgs eventArgs)
        {
            //Console.WriteLine(eventArgs.Time);
            //Audio.Play();
        }

        private void OnRenderFrameEvent(object o, FrameEventArgs eventArgs)
        {
            GL.ClearColor(Color4.DimGray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            m_ShaderProgram.Use();
            m_ProjectionMatrixUniform.Set(m_ShaderProgram);

            GL.ActiveTexture(TextureUnit.Texture0);
            m_Texture.Bind();
            {
                m_Mesh.Bind();
                {
                    m_Mesh.Draw();
                }
                m_Mesh.UnBind();
            }
            m_Texture.UnBind();

            SwapBuffers();
        }
    }
}

