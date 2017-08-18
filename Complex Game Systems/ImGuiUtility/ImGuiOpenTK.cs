namespace ImGuiUtility
{
    using System;
    using System.Drawing;

    using ImGuiNET;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;

    public static class ImGuiOpenTK
    {
        private static int s_FontTexture;

        private static NativeWindow s_Window;

        private static float s_ScaleFactor = 1f;

        private static float s_WheelPosition;

        public delegate void DrawEvent();
        public static event DrawEvent drawEvent;

        public static void Init(NativeWindow window)
        {
            s_Window = window;

            s_Window.KeyDown += OnKeyDown;
            s_Window.KeyUp += OnKeyUp;
            s_Window.KeyPress += OnKeyPress;

            ImGui.GetIO().FontAtlas.AddDefaultFont();

            SetOpenTKKeyMappings();

            CreateDeviceObjects();
        }

        private static void SetOpenTKKeyMappings()
        {
            var io = ImGui.GetIO();

            io.KeyMap[GuiKey.Tab] = (int)Key.Tab;
            io.KeyMap[GuiKey.LeftArrow] = (int)Key.Left;
            io.KeyMap[GuiKey.RightArrow] = (int)Key.Right;
            io.KeyMap[GuiKey.UpArrow] = (int)Key.Up;
            io.KeyMap[GuiKey.DownArrow] = (int)Key.Down;
            io.KeyMap[GuiKey.PageUp] = (int)Key.PageUp;
            io.KeyMap[GuiKey.PageDown] = (int)Key.PageDown;
            io.KeyMap[GuiKey.Home] = (int)Key.Home;
            io.KeyMap[GuiKey.End] = (int)Key.End;
            io.KeyMap[GuiKey.Delete] = (int)Key.Delete;
            io.KeyMap[GuiKey.Backspace] = (int)Key.BackSpace;
            io.KeyMap[GuiKey.Enter] = (int)Key.Enter;
            io.KeyMap[GuiKey.Escape] = (int)Key.Escape;
            io.KeyMap[GuiKey.A] = (int)Key.A;
            io.KeyMap[GuiKey.C] = (int)Key.C;
            io.KeyMap[GuiKey.V] = (int)Key.V;
            io.KeyMap[GuiKey.X] = (int)Key.X;
            io.KeyMap[GuiKey.Y] = (int)Key.Y;
            io.KeyMap[GuiKey.Z] = (int)Key.Z;
        }

        private static unsafe void CreateDeviceObjects()
        {
            var io = ImGui.GetIO();

            // Build texture atlas
            var texData = io.FontAtlas.GetTexDataAsAlpha8();

            // Create OpenGL texture
            s_FontTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, s_FontTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Alpha,
                texData.Width,
                texData.Height,
                0,
                PixelFormat.Alpha,
                PixelType.UnsignedByte,
                new IntPtr(texData.Pixels));

            // Store the texture identifier in the ImFontAtlas substructure.
            io.FontAtlas.SetTexID(s_FontTexture);

            // Cleanup (don't clear the input data if you want to append new fonts later)
            //io.Fonts->ClearInputData();
            io.FontAtlas.ClearTexData();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public static unsafe void RenderFrame()
        {
            var io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(s_Window.Width, s_Window.Height);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(s_ScaleFactor);
            io.DeltaTime = (1f / 60f);

            UpdateImGuiInput(io);

            ImGui.NewFrame();

            drawEvent?.Invoke();

            ImGui.Render();

            var data = ImGui.GetDrawData();
            RenderImDrawData(data);
        }

        private static unsafe void RenderImDrawData(DrawData* draw_data)
        {
            // We are using the OpenGL fixed pipeline to make the example code simpler to read!
            // Setup render state: alpha-blending enabled, no face culling, no depth testing,
            // scissor enabled, vertex/texcoord/color pointers.
            GL.GetInteger(GetPName.TextureBinding2D, out int lastTexture);
            GL.PushAttrib(AttribMask.EnableBit | AttribMask.ColorBufferBit | AttribMask.TransformBit);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ScissorTest);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.Enable(EnableCap.Texture2D);

            GL.UseProgram(0);

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            var io = ImGui.GetIO();
            ImGui.ScaleClipRects(draw_data, io.DisplayFramebufferScale);

            // Setup orthographic projection matrix
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(
                0.0f,
                io.DisplaySize.X / io.DisplayFramebufferScale.X,
                io.DisplaySize.Y / io.DisplayFramebufferScale.Y,
                0.0f,
                -1.0f,
                1.0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            // Render command lists

            for (var n = 0; n < draw_data->CmdListsCount; n++)
            {
                var cmd_list = draw_data->CmdLists[n];
                var vtx_buffer = (byte*)cmd_list->VtxBuffer.Data;
                var idx_buffer = (ushort*)cmd_list->IdxBuffer.Data;

                var vert0 = *((DrawVert*)vtx_buffer);
                var vert1 = *((DrawVert*)vtx_buffer + 1);
                var vert2 = *((DrawVert*)vtx_buffer + 2);

                GL.VertexPointer(
                    2, VertexPointerType.Float, sizeof(DrawVert),
                    new IntPtr(vtx_buffer + DrawVert.PosOffset));
                GL.TexCoordPointer(
                    2, TexCoordPointerType.Float, sizeof(DrawVert),
                    new IntPtr(vtx_buffer + DrawVert.UVOffset));
                GL.ColorPointer(
                    4, ColorPointerType.UnsignedByte, sizeof(DrawVert),
                    new IntPtr(vtx_buffer + DrawVert.ColOffset));

                for (var cmd_i = 0; cmd_i < cmd_list->CmdBuffer.Size; cmd_i++)
                {
                    var pcmd = &((DrawCmd*)cmd_list->CmdBuffer.Data)[cmd_i];
                    if (pcmd->UserCallback != IntPtr.Zero)
                        throw new NotImplementedException();

                    GL.BindTexture(TextureTarget.Texture2D, pcmd->TextureId.ToInt32());
                    GL.Scissor(
                        (int)pcmd->ClipRect.X,
                        (int)(io.DisplaySize.Y - pcmd->ClipRect.W),
                        (int)(pcmd->ClipRect.Z - pcmd->ClipRect.X),
                        (int)(pcmd->ClipRect.W - pcmd->ClipRect.Y));

                    var indices = new ushort[pcmd->ElemCount];
                    for (var i = 0; i < indices.Length; i++)
                        indices[i] = idx_buffer[i];

                    GL.DrawElements(
                        PrimitiveType.Triangles,
                        (int)pcmd->ElemCount,
                        DrawElementsType.UnsignedShort,
                        new IntPtr(idx_buffer));

                    idx_buffer += pcmd->ElemCount;
                }
            }

            // Restore modified state
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.BindTexture(TextureTarget.Texture2D, lastTexture);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.PopAttrib();
        }

        private static void UpdateImGuiInput(IO io)
        {
            var cursorState = Mouse.GetCursorState();
            var mouseState = Mouse.GetState();

            // INCORRECT
            // if (s_Window.Bounds.Contains(cursorState.X, cursorState.Y))
            if (s_Window.Focused)
            {
                var windowPoint = s_Window.PointToClient(new Point(cursorState.X, cursorState.Y));
                io.MousePosition =
                    new System.Numerics.Vector2(
                        windowPoint.X / io.DisplayFramebufferScale.X,
                        windowPoint.Y / io.DisplayFramebufferScale.Y);
            }
            else
            {
                io.MousePosition = new System.Numerics.Vector2(-1f, -1f);
            }

            io.MouseDown[0] = mouseState.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouseState.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouseState.MiddleButton == ButtonState.Pressed;

            var newWheelPos = mouseState.WheelPrecise;
            var delta = newWheelPos - s_WheelPosition;
            s_WheelPosition = newWheelPos;
            io.MouseWheel = delta;
        }

        private static void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            //Console.Write("Char typed: " + e.KeyChar);
            ImGui.AddInputCharacter(e.KeyChar);
        }

        private static void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            ImGui.GetIO().KeysDown[(int)e.Key] = true;
            UpdateModifiers(e);
        }

        private static void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            ImGui.GetIO().KeysDown[(int)e.Key] = false;
            UpdateModifiers(e);
        }

        private static void UpdateModifiers(KeyboardKeyEventArgs e)
        {
            var io = ImGui.GetIO();
            io.AltPressed = e.Alt;
            io.CtrlPressed = e.Control;
            io.ShiftPressed = e.Shift;
        }
    }
}
