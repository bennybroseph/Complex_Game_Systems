namespace ComplexGameSystems
{
    using OpenTK;
    using OpenTK.Input;

    public class StaticCamera : Camera
    {
        public override void Update()
        {
            m_PositionMatrix *= Matrix4.CreateRotationY(-0.001f);
        }

        public void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Q)
                m_PositionMatrix *= Matrix4.CreateTranslation(1f, 0f, 0f);
            if (e.Key == Key.E)
                m_PositionMatrix *= Matrix4.CreateTranslation(-1f, 0f, 0f);

            if (e.Key == Key.A)
                m_PositionMatrix *= Matrix4.CreateTranslation(0f, 0f, 1f);
            if (e.Key == Key.D)
                m_PositionMatrix *= Matrix4.CreateTranslation(0f, 0f, -1f);

            if (e.Key == Key.W)
                m_PositionMatrix *= Matrix4.CreateTranslation(0f, 1f, 0f);
            if (e.Key == Key.S)
                m_PositionMatrix *= Matrix4.CreateTranslation(0f, -1f, 0f);
        }
    }
}
