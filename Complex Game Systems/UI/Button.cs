namespace UI
{
    using Geometry;
    using Geometry.Shapes;

    using OpenTK;
    using OpenTK.Graphics;

    public class Button : Image
    {
        public Button(
            Canvas canvas,

            Texture defaultTexture,
            Texture highlighedTexture,
            Texture pushedTexture) : base(canvas, defaultTexture, highlighedTexture, pushedTexture) { }

        public override void Update()
        {
            base.Update();
        }

        //public override void Draw()
        //{
        //    var color = m_State == ElementState.Hovered ? Color4.Blue : Color4.White;
        //    if (m_State == ElementState.Selected)
        //        color = Color4.Black;

        //    Gizmos.DrawRectangle(
        //        transform.position.Xy - new Vector2(0.5f),
        //        transform.position.Xy + new Vector2(0.5f),
        //        color, color, true, transform.worldSpaceMatrix * m_Canvas.projection);
        //}
    }
}
