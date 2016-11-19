namespace Geometry.Shapes
{
    using System;
    using System.Collections.Generic;

    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    public static class Gizmos
    {
        public static void Init()
        {
            Plane.Init();
        }

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
                    start.X - (MyGameWindow.main.Width / 2f),
                    MyGameWindow.main.Height / 2f - start.Y);
            end =
                new Vector2(
                    end.X - MyGameWindow.main.Width / 2f,
                    MyGameWindow.main.Height / 2f - end.Y);

            var size = end - start;

            GL.PushMatrix();
            {
                var matrix =
                    Matrix4.CreateOrthographic(MyGameWindow.main.Width, MyGameWindow.main.Height, -1f, 1f);
                GL.LoadMatrix(ref matrix);

                var translate =
                    Matrix4.CreateTranslation(
                        -MyGameWindow.main.Width / 2f, MyGameWindow.main.Height / 2f, 0f);
                GL.MultMatrix(ref translate);

                GL.MultMatrix(ref trasform);

                var invert = Matrix4.Invert(translate);
                GL.MultMatrix(ref invert);

                GL.Begin(PrimitiveType.Quads);
                {
                    GL.Color4(colorStart);
                    GL.Vertex2(start.X, start.Y);

                    GL.Color4(horizontal ? colorEnd : colorStart);
                    GL.Vertex2(start.X + size.X, start.Y);

                    GL.Color4(colorEnd);
                    GL.Vertex2(start.X + size.X, start.Y + size.Y);

                    GL.Color4(horizontal ? colorStart : colorEnd);
                    GL.Vertex2(start.X, start.Y + size.Y);
                }
                GL.End();
            }
            GL.PopMatrix();
        }

        public static void DrawCustomShape(
            List<Vector2> vertexes, List<Color4> colors, PrimitiveType primitiveType)
        {
            GL.UseProgram(0);

            GL.PushMatrix();
            {
                var matrix =
                    Matrix4.CreateOrthographic(MyGameWindow.main.Width, MyGameWindow.main.Height, -1f, 1f);
                GL.LoadMatrix(ref matrix);

                GL.Begin(primitiveType);
                {
                    for (var i = 0; i < vertexes.Count; ++i)
                    {
                        GL.Color4(colors[i]);

                        var vertex =
                            new Vector2(
                                vertexes[i].X - (MyGameWindow.main.Width / 2f),
                                MyGameWindow.main.Height / 2f - vertexes[i].Y);
                        GL.Vertex2(vertex);
                    }
                }
                GL.End();
            }
            GL.PopMatrix();
        }
    }
}
