using System;
using System.Diagnostics;

using OpenTK;

using Debug = Utility.Debug;

public static class Time
{
    private static Stopwatch s_Stopwatch = new Stopwatch();

    public static float deltaTime { get; private set; }
    public static float timeScale { get; set; } = 1f;

    public static float unscaledDeltaTime { get; private set; }

    public static float time => s_Stopwatch.ElapsedMilliseconds;
    public static TimeSpan timeSpan => s_Stopwatch.Elapsed;

    public static void Init()
    {
        Debug.Log("Time initialized");
        s_Stopwatch.Start();

        MyGameWindow.main.UpdateFrame += Update;
    }

    private static void Update(object sender, FrameEventArgs frameEventArgs)
    {
        unscaledDeltaTime = (float)frameEventArgs.Time;
        deltaTime = unscaledDeltaTime * timeScale;
    }
}

