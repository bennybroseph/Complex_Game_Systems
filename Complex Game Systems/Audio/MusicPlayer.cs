namespace ComplexGameSystems
{
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;

    using FMOD;

    public static class MusicPlayer
    {
        private static Channel s_Channel;
        private static List<Sound> s_MusicList = new List<Sound>();

        public static void Init(string path = "Content\\Music\\")
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\" + path);
            var files = directory.GetFiles().ToList();

            foreach (var fileInfo in files)
            {
                Sound newSound;
                var result = Audio.LoadSound(fileInfo.FullName, out newSound);

                if (result == RESULT.OK)
                    s_MusicList.Add(newSound);
            }
        }

        public static RESULT Play()
        {
            if (s_Channel == null)
                return Audio.PlaySound(ref s_Channel, s_MusicList[0], MODE.DEFAULT, 0);

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
    }
}
