namespace Geometry.Shapes
{
    using System;
    using System.Collections.Generic;

    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    public static class Gizmos
    {
        private delegate void DrawGizmo(Matrix4 viewProjection);
        private static List<DrawGizmo> s_DrawCalls = new List<DrawGizmo>();

        public static void Init()
        {
            Plane.Init();
        }

        internal static void Render(Matrix4 viewProjection)
        {
            foreach (var drawGizmo in s_DrawCalls)
                drawGizmo(viewProjection);
        }

        internal static void ClearDrawCalls() { s_DrawCalls.Clear(); }

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

        public static void DrawCube(Vector3 center, Vector3 size, bool drawWireFrame = true)
        {
            void NewAction(Matrix4 viewProjection)
            {
                GL.UseProgram(0);

                GL.PushMatrix();
                {
                    GL.LoadMatrix(ref viewProjection);

                    var extents = size / 2f;

                    var vert0 = center + new Vector3(-extents.X, -extents.Y, extents.Z);
                    var vert1 = vert0 + new Vector3(size.X, 0f, 0f);
                    var vert2 = vert0 + new Vector3(0f, size.Y, 0f);
                    var vert3 = vert2 + new Vector3(size.X, 0f, 0f);

                    var vert4 = center - extents;
                    var vert5 = vert4 + new Vector3(size.X, 0f, 0f);
                    var vert6 = vert4 + new Vector3(0f, size.Y, 0);
                    var vert7 = vert6 + new Vector3(size.X, 0f, 0f);

                    if (drawWireFrame)
                    {
                        GL.Begin(PrimitiveType.LineStrip);
                        {
                            GL.Vertex3(vert0);
                            GL.Vertex3(vert1);
                            GL.Vertex3(vert3);
                            GL.Vertex3(vert2);
                            GL.Vertex3(vert0);
                            GL.Vertex3(vert4);
                            GL.Vertex3(vert6);
                            GL.Vertex3(vert2);
                            GL.Vertex3(vert6);
                            GL.Vertex3(vert7);
                            GL.Vertex3(vert5);
                            GL.Vertex3(vert4);
                            GL.Vertex3(vert5);
                            GL.Vertex3(vert1);
                            GL.Vertex3(vert3);
                            GL.Vertex3(vert7);
                        }
                        GL.End();
                    }
                    else
                    {
                        GL.Begin(PrimitiveType.TriangleStrip);
                        {
                            GL.Vertex3(vert7);
                            GL.Vertex3(vert6);
                            GL.Vertex3(vert3);
                            GL.Vertex3(vert2);
                            GL.Vertex3(vert0);
                            GL.Vertex3(vert6);
                            GL.Vertex3(vert4);
                            GL.Vertex3(vert7);
                            GL.Vertex3(vert5);
                            GL.Vertex3(vert3);
                            GL.Vertex3(vert1);
                            GL.Vertex3(vert0);
                            GL.Vertex3(vert5);
                            GL.Vertex3(vert4);
                        }
                        GL.End();
                    }
                }
                GL.PopMatrix();
            }

            s_DrawCalls.Add(NewAction);
        }
    }
}
