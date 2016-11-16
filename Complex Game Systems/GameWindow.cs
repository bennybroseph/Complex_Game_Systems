namespace ComplexGameSystems
{
    using System;
    using System.IO;

    using Geometry;
    using Geometry.Shapes;

    using Utility;

    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;

    public class GameWindow : OpenTK.GameWindow
    {
        private Model<Vertex> m_Model;

        private Texture m_Texture;

        private VBO<Vertex> m_VBO;
        private VAO<Vertex> m_VAO;

        private ShaderProgram m_ShaderProgram;

        private StaticCamera m_Camera;
        private Matrix4Uniform m_ProjectionMatrixUniform;

        public delegate void OnEvent(GameWindow window, EventArgs eventArgs);

        public event OnEvent OnResizeEvent;

        public static GameWindow main { get; protected set; }

        public GameWindow() :
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
            if (main == null)
                main = this;
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
                "Content\\Pictures\\Brock.jpg", TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            m_Texture.Bind();

            var location = m_ShaderProgram.GetUniformLocation("diffuseMap");
            GL.ProgramUniform1(m_ShaderProgram.handle, location, 0);

            location = m_ShaderProgram.GetUniformLocation("lightDirection");
            GL.ProgramUniform3(m_ShaderProgram.handle, location, 0, -1, 0);

            var mesh = Plane.GetMesh();

            m_Model = new Model<Vertex>
            {
                matrix = Matrix4.Identity,
                mesh = mesh,
                shader = m_ShaderProgram,
                diffuseTexture = m_Texture
            };
            m_Model.Bind();
            m_Model.BufferData();
            m_Model.UnBind();

            m_Camera = new StaticCamera();
            m_Camera.SetPerspective(MathHelper.Pi * 0.25f, 16f / 9f, 0.1f, 75f);
            m_Camera.SetLookAt(new Vector3(0f, 5f, 10f), Vector3.Zero, new Vector3(0f, 1f, 0f));

            MusicPlayer.Play();
        }

        protected override void OnUpdateFrame(FrameEventArgs eventArgs)
        {
            m_Camera.Update();
            MusicPlayer.Update();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            m_Camera.OnKeyDown(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            MusicPlayer.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            MusicPlayer.OnMouseMove(e);
        }

        private void OnRenderFrameEvent(object o, FrameEventArgs eventArgs)
        {
            GL.ClearColor(Color4.DimGray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //m_ShaderProgram.Use();

            //GL.ActiveTexture(TextureUnit.Texture0);
            //m_Texture.Bind();
            //{
            //    m_Model.Bind();
            //    {
            //        m_Model.Draw();
            //    }
            //    m_Model.UnBind();
            //}
            //Texture.UnBind();

            MusicPlayer.Draw();

            SwapBuffers();
        }
    }
}

