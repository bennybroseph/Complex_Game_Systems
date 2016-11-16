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
            GL.UseProgram(0);

            start =
                new Vector2(
                    start.X / (GameWindow.main.Width / 2f) - 1f,
                    1-start.Y / (GameWindow.main.Height / 2f));
            end =
                new Vector2(
                    end.X / (GameWindow.main.Width / 2f) - 1f,
                    1-end.Y / (GameWindow.main.Height / 2f));

            var size = end - start;

            GL.Begin(PrimitiveType.Quads);
            {
                GL.Color4(colorStart);
                GL.Vertex3(start.X, start.Y, -1f);

                GL.Color4(Math.Abs(size.X) >= Math.Abs(size.Y) ? colorEnd : colorStart);
                GL.Vertex3(start.X + size.X, start.Y, -1f);

                GL.Color4(colorEnd);
                GL.Vertex3(start.X + size.X, start.Y + size.Y, -1f);

                GL.Color4(Math.Abs(size.X) >= Math.Abs(size.Y) ? colorStart : colorEnd);
                GL.Vertex3(start.X, start.Y + size.Y, -1f);
            }
            GL.End();
        }
    }
}
