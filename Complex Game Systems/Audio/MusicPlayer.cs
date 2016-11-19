using System.Collections.Generic;
using System.IO;
using System.Linq;

using FMOD;

using Geometry.Shapes;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

public static class MusicPlayer
{
    private static Channel s_Channel;
    private static Equalizer s_Equalizer;
    private static List<Sound> s_MusicList = new List<Sound>();

    private static int s_CurrentIndex = -1;

    private static bool s_IsSelected;

    public static void Init(string path = "Content\\Music")
    {
        var directory = new DirectoryInfo(path);
        var files = directory.GetFiles().ToList();

        foreach (var fileInfo in files)
        {
            Sound newSound;
            var result = Audio.LoadSound(path + "\\" + fileInfo.Name, out newSound);

            if (result == RESULT.OK)
                s_MusicList.Add(newSound);
        }
    }

    public static RESULT Play()
    {
        if (s_Channel == null)
            return CreateChannel();

        bool isPlaying;
        var result = s_Channel.isPlaying(out isPlaying);

        return isPlaying ? result : Audio.Play(s_Channel);
    }
    public static RESULT Pause()
    {
        if (s_Channel == null)
            return RESULT.ERR_CHANNEL_ALLOC;

        return s_Channel.setPaused(true);
    }
    public static RESULT TogglePause()
    {
        if (s_Channel == null)
            Play();

        return Audio.TogglePause(s_Channel);
    }

    public static RESULT NextTrack()
    {
        if (s_CurrentIndex + 1 < s_MusicList.Count)
            return PlaySound(s_MusicList[++s_CurrentIndex]);

        return PlaySound(s_MusicList[s_CurrentIndex = 0]);
    }
    public static RESULT PreviousTrack()
    {
        if (s_CurrentIndex - 1 >= 0)
            return PlaySound(s_MusicList[--s_CurrentIndex]);

        return PlaySound(s_MusicList[s_CurrentIndex = s_MusicList.Count - 1]);
    }

    private static RESULT PlaySound(Sound sound)
    {
        var result = Audio.PlaySound(ref s_Channel, sound, MODE.DEFAULT, 0);
        if (s_Equalizer == null)
            s_Equalizer = new Equalizer(s_Channel);
        else
            s_Equalizer.LinkChannel(s_Channel);

        return result;
    }

    private static RESULT CreateChannel()
    {
        var result = PlaySound(s_MusicList[0]);
        s_CurrentIndex = 0;

        return result;
    }

    public static void Update()
    {
        uint position;
        s_Channel.getPosition(out position, TIMEUNIT.MS);

        Sound sound;
        s_Channel.getCurrentSound(out sound);

        uint length;
        sound.getLength(out length, TIMEUNIT.MS);

        if (position >= length)
            NextTrack();

        s_Equalizer.Update();
    }

    public static void OnMouseDown(MouseButtonEventArgs e)
    {
        if (!s_IsSelected || e.Button != MouseButton.Left)
            return;

        Sound sound;
        s_Channel.getCurrentSound(out sound);

        uint length;
        sound.getLength(out length, TIMEUNIT.MS);

        uint f = (uint)((float)e.X / MyGameWindow.main.Width * length);
        s_Channel.setPosition(f, TIMEUNIT.MS);
    }
    public static void OnMouseMove(MouseMoveEventArgs e)
    {
        s_IsSelected = e.Y <= MyGameWindow.main.Height - 70f && e.Y >= MyGameWindow.main.Height - 87f;
    }

    public static void Draw()
    {
        uint position;
        s_Channel.getPosition(out position, TIMEUNIT.MS);

        Sound sound;
        s_Channel.getCurrentSound(out sound);

        uint length;
        sound.getLength(out length, TIMEUNIT.MS);

        var proportion = (float)position / length;

        var extraSpace = s_IsSelected ? 5 : 0;

        Gizmos.DrawRectangle(
            new Vector2(0f, MyGameWindow.main.Height - 75f),
            new Vector2(MyGameWindow.main.Width, MyGameWindow.main.Height - 77f - extraSpace),
            Color4.BlueViolet,
            Color4.BlueViolet);

        Gizmos.DrawRectangle(
            new Vector2(0f, MyGameWindow.main.Height - 75f),
            new Vector2(proportion * MyGameWindow.main.Width, MyGameWindow.main.Height - 77f - extraSpace),
            Color4.White,
            Color4.White);

        s_Equalizer.Draw();
    }
}