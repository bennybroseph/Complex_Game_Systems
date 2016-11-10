namespace ComplexGameSystems
{
    using System;
    using System.Runtime.InteropServices;

    using FMOD;

    using Debug = Utility.Debug;

    public static class Audio2
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        private static System m_FMODSystem;

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

        public static RESULT LoadSong(string name, out Sound sound)
        {
            var result = m_FMODSystem.createStream(
                "Content/" + name + ".mp3", MODE.DEFAULT, out sound);

            Debug.Log("Loading " + name + "... Result: " + result);

            return result;
        }

        public static RESULT PlaySound(
            Channel channel,
            Sound sound,
            uint startTime,
            MODE mode = MODE.DEFAULT,
            int loopCount = -1)
        {
            if (!channel.isValid())
            {
                Debug.LogError("Channel is not valid");
                return RESULT.ERR_CHANNEL_ALLOC;
            }

            bool isPlaying;

            channel.isValid();
            var result = channel.isPlaying(out isPlaying);

            if (isPlaying)
                channel.stop();

            m_FMODSystem.playSound(sound, null, false, out channel);

            if (startTime != 0u)
                channel.setPosition(startTime, TIMEUNIT.MS);
            channel.setMode(mode);
            channel.setLoopCount(loopCount);

            return result;
        }
        public static RESULT PlaySound(
            Channel channel, Sound sound, MODE mode = MODE.DEFAULT, int loopCount = -1)
        {
            return PlaySound(channel, sound, 0u, mode, loopCount);
        }
    }
}
