namespace ComplexGameSystems.Geometry.Shapes
{
    using System;
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    using GameWindow = GameWindow;

    public static class Gizmos
    {
        public static void DrawRectangle(Vector2 start, Vector2 end, Color4 colorStart, Color4 colorEnd)
        {
            var size = end - start;

            DrawRectangle(
                start, end, colorStart, colorEnd, Math.Abs(size.X) >= Math.Abs(size.Y), Matrix4.Identity);
        }

        public static void DrawRectangle(
            Vector2 start, Vector2 end, Color4 colorStart, Color4 colorEnd, bool horizontal, Matrix4 trasform)
        {
            GL.UseProgram(0);

            start =
                new Vector2(
                    start.X / (GameWindow.main.Width / 2f) - 1f,
                    1 - start.Y / (GameWindow.main.Height / 2f));
            end =
                new Vector2(
                    end.X / (GameWindow.main.Width / 2f) - 1f,
                    1 - end.Y / (GameWindow.main.Height / 2f));

            var size = end - start;

            GL.PushMatrix();
            {
                var translate = Matrix4.CreateTranslation(new Vector3(start));
                GL.MultMatrix(ref translate);

                GL.MultMatrix(ref trasform);

                translate = Matrix4.CreateTranslation(new Vector3(-start));
                GL.MultMatrix(ref translate);

                GL.Begin(PrimitiveType.Quads);
                {
                    GL.Color4(colorStart);
                    GL.Vertex3(start.X, start.Y, -1f);

                    GL.Color4(horizontal ? colorEnd : colorStart);
                    GL.Vertex3(start.X + size.X, start.Y, -1f);

                    GL.Color4(colorEnd);
                    GL.Vertex3(start.X + size.X, start.Y + size.Y, -1f);

                    GL.Color4(horizontal ? colorStart : colorEnd);
                    GL.Vertex3(start.X, start.Y + size.Y, -1f);
                }
                GL.End();
            }
            GL.PopMatrix();
        }
    }
}
