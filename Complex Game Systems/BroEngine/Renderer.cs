using Geometry.Shapes;
using OpenTK;
using OpenTK.Graphics;

namespace BroEngine
{
    public abstract class Renderer : Component
    {
        private Bounds m_Bounds = new Bounds(Vector3.Zero, new Vector3(1f, 1f, 1f));

        public Bounds bounds => m_Bounds;
        public bool enabled { get; set; } = true;

        internal abstract void Render(Matrix4 viewProjection);

        public void DrawGizmos()
        {
            //Gizmos.DrawRectangle(
            //    new Vector2(m_Bounds.min.X, m_Bounds.min.Y),
            //    new Vector2(m_Bounds.max.X + 10f, m_Bounds.min.Y +  10f),
            //    Color4.LimeGreen, Color4.LimeGreen, true, transform.worldSpaceMatrix);
        }
    }
}
