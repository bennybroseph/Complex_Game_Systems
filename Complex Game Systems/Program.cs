using System;

#if DEBUG
using System.Runtime.InteropServices;
#endif

using OpenTK;
using OpenTK.Graphics;

using ComplexGameSystems.Utility;

namespace ComplexGameSystems
{
    internal static class Program
    {

#if DEBUG
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
#endif

        private static void Main(string[] args)
        {
            Debug.Init();

#if DEBUG
            AllocConsole();
#endif

            var window = new GameWindow(
                1600,
                900,
                GraphicsMode.Default,
                "Test Application",
                GameWindowFlags.Default,
                DisplayDevice.Default);
            var context = new GraphicsContext(GraphicsMode.Default, window.WindowInfo);

            window.Run(60, 60);

            Console.WriteLine("Program Terminated...");
        }
    }
}

