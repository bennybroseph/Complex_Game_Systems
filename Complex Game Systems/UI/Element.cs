namespace UI
{
    using OpenTK;
    using OpenTK.Input;

    public abstract class Element
    {
        protected enum ElementState
        {
            Default,
            Hovered,
            Selected,
        }

        protected Canvas m_Canvas;

        protected ElementState m_State = ElementState.Default;
        protected ElementState m_PreviouState = ElementState.Default;

        public Transform transform { get; } = new Transform();

        protected Element(Canvas canvas)
        {
            m_Canvas = canvas;
            m_Canvas.AddElement(this);

            m_Canvas.gameWindow.MouseMove += OnMouseMoveEvent;
            m_Canvas.gameWindow.MouseDown += OnMouseDown;
            m_Canvas.gameWindow.MouseUp += OnMouseUp;
        }

        protected virtual void OnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (m_State == ElementState.Selected)
                m_State = ElementState.Hovered;
        }

        protected virtual void OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (m_State == ElementState.Hovered)
                m_State = ElementState.Selected;
        }

        protected virtual void OnMouseMoveEvent(object sender, MouseMoveEventArgs mouseMoveEventArgs)
        {
            m_PreviouState = m_State;

            var cursorState = mouseMoveEventArgs.Mouse;

            var mouseX = cursorState.X;
            var mouseY = cursorState.Y;

            var t = Matrix4.CreateTranslation(new Vector3(mouseX, mouseY, 0f)) * m_Canvas.projection;
            var f = Matrix4.CreateTranslation(transform.position) * m_Canvas.projection;

            if (mouseX >= transform.position.X - transform.localScale.X / 2f &&
                mouseX <= transform.position.X + transform.localScale.X / 2f &&
                mouseY <= transform.position.Y + transform.localScale.Y / 2f &&
                mouseY >= transform.position.Y - transform.localScale.Y / 2f)
            {
                if (m_State == ElementState.Default)
                    m_State = ElementState.Hovered;
            }
            else
                m_State = ElementState.Default;
        }

        public virtual void Update()
        {

        }

        public abstract void Draw();
    }
}
