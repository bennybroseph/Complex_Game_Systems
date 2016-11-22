using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using FMOD;

using Debug = Utility.Debug;

public static class Audio
{
    [DllImport("kernel32.dll")]
    private static extern IntPtr LoadLibrary(string dllToLoad);

    private static FMOD.System m_FMODSystem;

    public static void Init()
    {
        if (Environment.Is64BitProcess)
        {
            Debug.Log("Loading 64bit FMOD Library");
            LoadLibrary("FMOD\\64\\fmod.dll");
        }
        else
        {
            Debug.Log("Loading 32bit FMOD Library");
            LoadLibrary("FMOD\\32\\fmod.dll");
        }

        Debug.Log("Creating System... Result: " + Factory.System_Create(out m_FMODSystem));

        Debug.Log("Setting DSP Buffer Size... Result: " + m_FMODSystem.setDSPBufferSize(1024, 4));
        Debug.Log(
            "Initializing System... Result: " + m_FMODSystem.init(32, INITFLAGS.NORMAL, (IntPtr)0));
    }

    public static RESULT LoadChannel(Sound sound, out Channel channel)
    {
        var result = m_FMODSystem.playSound(sound, null, true, out channel);
        Debug.Log("Loading channel... Result: " + result);

        return result;
    }

    public static RESULT LoadSound(string path, out Sound sound)
    {
        var result = m_FMODSystem.createStream(path, MODE.DEFAULT, out sound);
        Debug.Log("Loading " + path + "... Result: " + result);

        return result;
    }

    public static RESULT PlaySound(
        ref Channel channel,
        Sound sound,
        uint startTime,
        MODE mode = MODE.DEFAULT,
        int loopCount = -1)
    {
        RESULT result;

        if (channel != null)
        {
            bool isPlaying;
            result = channel.isPlaying(out isPlaying);

            if (isPlaying)
                channel.stop();
        }

        result = m_FMODSystem.playSound(sound, null, false, out channel);

        if (startTime != 0u)
            channel.setPosition(startTime, TIMEUNIT.MS);
        channel.setMode(mode);
        channel.setLoopCount(loopCount);

        return result;

    }
    public static RESULT PlaySound(
        ref Channel channel, Sound sound, MODE mode = MODE.DEFAULT, int loopCount = -1)
    {
        return PlaySound(ref channel, sound, 0u, mode, loopCount);
    }

    public static RESULT Play(Channel channel)
    {
        return channel.setPaused(false);
    }
    public static RESULT Pause(Channel channel)
    {
        return channel.setPaused(true);
    }
    public static RESULT TogglePause(Channel channel)
    {
        bool paused;
        var result = channel.getPaused(out paused);
        if (result != RESULT.OK)
            return result;

        return channel.setPaused(!paused);
    }

    public static RESULT CreateDSP(ref DSP_DESCRIPTION dspDescription, out DSP dsp)
    {
        return m_FMODSystem.createDSP(ref dspDescription, out dsp);
    }
    public static RESULT CreateDSP(DSP_TYPE dspType, out DSP dsp)
    {
        return m_FMODSystem.createDSPByType(dspType, out dsp);
    }

    public static RESULT SetVolume(Channel channel, float newVolume)
    {
        return channel.setVolume(newVolume);
    }

    private static RESULT LogError(
        object obj, RESULT result,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = null,
        [CallerFilePath] string filePath = null)
    {
        Debug.LogError(obj, lineNumber, caller, filePath);

        return result;
    }
}