using System;
using System.Collections.Generic;
using System.IO;

using ComplexGameSystems.Geometry;
using ComplexGameSystems.Utility;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ComplexGameSystems
{
    public class GameWindow : OpenTK.GameWindow
    {
        private VBO<Vertex> m_VBO;
        private VAO<Vertex> m_VAO;

        private ShaderProgram m_ShaderProgram;

        private Matrix4Uniform m_ProjectionMatrixUniform;

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
        }

        protected override void OnLoad(EventArgs eventArgs)
        {
            Audio.Init();

            RenderFrame += OnRenderFrameEvent;

            var vertexes = new List<Vertex>
            {
                new Vertex(new Vector3(-1f, -1f, -1.5f), new Color4(1f, 0f, 0f, 1f)),
                new Vertex(new Vector3(1, 1, -1.5f), new Color4(1f, 0f, 0f, 1f)),
                new Vertex(new Vector3(1, -1, -1.5f), new Color4(1f, 0f, 0f, 1f))
            };

            m_VBO = new VBO<Vertex>(vertexes, Vertex.size);

            var vertShaderCode = File.ReadAllText("Shaders/Default.vert");
            var vertShader = new Shader(ShaderType.VertexShader, vertShaderCode);

            var fragShaderCode = File.ReadAllText("Shaders/Default.frag");
            var fragShader = new Shader(ShaderType.FragmentShader, fragShaderCode);

            m_ShaderProgram = new ShaderProgram(vertShader, fragShader);

            m_VAO = new VAO<Vertex>(
                m_VBO, m_ShaderProgram,
                new VertexAttribute("vPosition", 3, VertexAttribPointerType.Float, Vertex.size, 0),
                new VertexAttribute("vColor", 4, VertexAttribPointerType.Float, Vertex.size, 3 * 4));

            m_ProjectionMatrixUniform =
                new Matrix4Uniform("projectionMatrix")
                {
                    Matrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 16f / 9, 0.1f, 100f)
                };
        }

        protected override void OnUpdateFrame(FrameEventArgs eventArgs)
        {
            //Console.WriteLine(eventArgs.Time);
            Audio.Play(0);
        }

        private void OnRenderFrameEvent(object o, FrameEventArgs eventArgs)
        {
            GL.ClearColor(Color4.DimGray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            m_ShaderProgram.Use();
            m_ProjectionMatrixUniform.Set(m_ShaderProgram);

            m_VBO.Bind();
            m_VAO.Bind();

            m_VBO.BufferData();
            m_VBO.Draw();

            SwapBuffers();
        }
    }
}

