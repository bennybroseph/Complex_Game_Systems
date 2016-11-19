namespace UI
{
    using System;
    using System.Collections.Generic;

    using OpenTK;

    public class Canvas
    {
        private readonly List<Element> m_Elements = new List<Element>();

        private Matrix4 m_OrthoProjection;

        public GameWindow gameWindow { get; }

        public Matrix4 projection => m_OrthoProjection;

        public float width { get; private set; }
        public float height { get; private set; }

        public Canvas(GameWindow window)
        {
            gameWindow = window;

            Resize();

            gameWindow.Resize += OnResize;
        }

        public void Update()
        {
            foreach (var element in m_Elements)
                element.Update();
        }

        public void Draw()
        {
            foreach (var element in m_Elements)
                element.Draw();
        }

        public bool AddElement(Element newElement)
        {
            if (m_Elements.Contains(newElement))
                return false;

            m_Elements.Add(newElement);

            return true;
        }

        private void OnResize(object sender, EventArgs eventArgs)
        {
            var window = sender as GameWindow;
            if (window == null || window != gameWindow)
                return;

            Resize();
        }

        private void Resize()
        {
            width = gameWindow.Width;
            height = gameWindow.Height;

            m_OrthoProjection = Matrix4.CreateOrthographicOffCenter(0f, width, height, 0f, -1f, 1f);
        }
    }
}
