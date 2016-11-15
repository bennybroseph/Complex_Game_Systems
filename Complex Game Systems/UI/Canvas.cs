

namespace ComplexGameSystems.UI
{
    using System;

    public class Canvas
    {
        public float width;
        public float height;

        public Canvas(GameWindow window)
        {
            width = window.Width;
            height = window.Height;

            window.OnResizeEvent += OnResize;
        }

        private void OnResize(GameWindow window, EventArgs eventArgs)
        {
            width = window.Width;
            height = window.Height;
        }
    }
}
