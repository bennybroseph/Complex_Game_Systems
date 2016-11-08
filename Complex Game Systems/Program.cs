using System;

using OpenTK;
using OpenTK.Graphics;

public class Program
{
    private static void Main(string[] args)
    {
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

