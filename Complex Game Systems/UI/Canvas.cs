namespace UI
{
    using System;

    public class Canvas
    {
        private OpenTK.GameWindow gameWindow;

        public float width;
        public float height;

        public Canvas(OpenTK.GameWindow window)
        {
            gameWindow = window;

            width = gameWindow.Width;
            height = gameWindow.Height;

            gameWindow.Resize += OnResize;
        }

        private void OnResize(object sender, EventArgs eventArgs)
        {
            var window = sender as GameWindow;
            if (window == null || window != gameWindow)
                return;

            width = gameWindow.Width;
            height = gameWindow.Height;
        }
    }
}
