using System;
using System.Runtime.InteropServices;

namespace ComplexGameSystems
{
    public class Audio
    {
        public const int NUM_SONGS = 10;

        private FMOD.System m_FMODSystem;
        private FMOD.Channel m_Channel;
        private FMOD.Sound[] m_Songs;

        private int m_CurrentSongID = -1;

        private static Audio s_Self;

        public static Audio self
        {
            get
            {
                if (s_Self == null)
                {
                    //TODO: Write Debug Class
                    var oldColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Audio was not initialized. Initializing...");
                    Console.ForegroundColor = oldColor;

                    s_Self = new Audio();
                }
                return s_Self;
            }
        }

        public static bool Init()
        {
            if (s_Self == null)
            {
                s_Self = new Audio();
                return true;
            }

            return false;
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        private Audio()
        {
            if (Environment.Is64BitProcess)
            {
                Console.WriteLine("Loading 64bit FMOD Library");
                LoadLibrary("FMOD\\64\\fmod.dll");
            }
            else
            {
                Console.WriteLine("Loading 32bit FMOD Library");
                LoadLibrary("FMOD\\32\\fmod.dll");
            }

            FMOD.Factory.System_Create(out m_FMODSystem);

            m_FMODSystem.setDSPBufferSize(1024, 10);
            m_FMODSystem.init(32, FMOD.INITFLAGS.NORMAL, (IntPtr)0);

            m_Songs = new FMOD.Sound[NUM_SONGS];

            LoadSong(0, "Emergency");
        }

        private void LoadSong(int songID, string name)
        {
            var result = m_FMODSystem.createStream(
                "Content/Music/" + name + ".mp3", FMOD.MODE.DEFAULT, out m_Songs[songID]);
            Console.WriteLine("Loading " + songID + ", got result " + result);
        }

        public static bool IsPlaying()
        {
            var isPlaying = false;

            if (self.m_Channel != null)
                self.m_Channel.isPlaying(out isPlaying);

            return isPlaying;
        }

        public static void Play(int songId)
        {
            Console.WriteLine("Play(" + songId + ")");

            if (self.m_CurrentSongID != songId)
            {
                Stop();

                if (songId >= 0 && songId < NUM_SONGS && self.m_Songs[songId] != null)
                {
                    self.m_FMODSystem.playSound(self.m_Songs[songId], null, false, out self.m_Channel);
                    UpdateVolume();
                    self.m_Channel.setMode(FMOD.MODE.LOOP_NORMAL);
                    self.m_Channel.setLoopCount(-1);

                    self.m_CurrentSongID = songId;
                }
            }
        }

        public static void UpdateVolume()
        {
            if (self.m_Channel != null)
                self.m_Channel.setVolume(1f);
        }

        public static void Stop()
        {
            if (IsPlaying())
                self.m_Channel.stop();

            self.m_CurrentSongID = -1;
        }
    }
}
