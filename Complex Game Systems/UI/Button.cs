namespace UI
{
    using Geometry;
    using OpenTK.Input;

    public class Button : Image
    {
        public delegate void OnPush();

        private OnPush m_OnPushDelegate;

        public Button(
            Canvas canvas,

            OnPush onPushDelegate,

            Texture defaultTexture,
            Texture highlighedTexture,
            Texture pushedTexture) : base(canvas, defaultTexture, highlighedTexture, pushedTexture)
        {
            m_OnPushDelegate = onPushDelegate;
        }

        protected override void OnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (m_State == ElementState.Selected)
                m_OnPushDelegate();

            base.OnMouseUp(sender, mouseButtonEventArgs);
        }
    }
}
