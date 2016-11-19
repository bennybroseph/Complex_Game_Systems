using OpenTK;

public static class Time
{
    public static float deltaTime { get; private set; }
    public static float timeScale { get; set; } = 1f;

    public static float unscaledDeltaTime { get; private set; }

    public static void Init()
    {
        MyGameWindow.main.UpdateFrame += Update;
    }

    private static void Update(object sender, FrameEventArgs frameEventArgs)
    {
        unscaledDeltaTime = (float)frameEventArgs.Time;
        deltaTime = unscaledDeltaTime * timeScale;
    }
}

