namespace Geometry
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;

    using OpenTK.Graphics.OpenGL;

    using Utility;

    using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

    public class Animation : Texture
    {
        private class TextureInfo
        {
            public int handle;
            public string path;

            public int width;
            public int height;
        }

        private readonly List<TextureInfo> m_TextureInfo = new List<TextureInfo>();
        public IEnumerable<string> totalPaths { get; }

        private int m_CurrentIndex;
        private float m_DeltaIndexTime;

        private static readonly Dictionary<string, List<BitmapData>> s_BitmapData =
            new Dictionary<string, List<BitmapData>>();

        public override int width => m_TextureInfo[m_CurrentIndex].width;
        public override int height => m_TextureInfo[m_CurrentIndex].height;

        public float animationTimeScale { get; set; } = 1f;

        public Animation(
            TextureMinFilter minFilter, TextureMagFilter magFilter, string firstPath, params string[] paths)
        {
            m_MinFilter = minFilter;
            m_MagFilter = magFilter;

            totalPaths = new[] { firstPath }.Concat(paths);
        }

        public override void Bind()
        {
            m_DeltaIndexTime += Time.deltaTime;
            if (m_DeltaIndexTime > 0.03f)
            {
                m_CurrentIndex++;
                m_DeltaIndexTime = 0f;
            }

            if (m_CurrentIndex >= m_TextureInfo.Count)
                m_CurrentIndex = 0;

            GL.BindTexture(TextureTarget.Texture2D, m_TextureInfo[m_CurrentIndex].handle);
        }

        public override void BufferData()
        {
            foreach (var path in totalPaths)
            {
                foreach (var bitmapData in GetAnimationBitmapData(path))
                {
                    int newHandle;
                    GL.GenTextures(1, out newHandle);

                    var newTextureInfo = new TextureInfo
                    {
                        handle = newHandle,
                        path = path,

                        width = bitmapData.Width,
                        height = bitmapData.Height,
                    };

                    GL.BindTexture(TextureTarget.Texture2D, newTextureInfo.handle);

                    GL.TexImage2D(
                        TextureTarget.Texture2D,
                        0,
                        PixelInternalFormat.Rgba,
                        newTextureInfo.width,
                        newTextureInfo.height,
                        0,
                        PixelFormat.Bgra,
                        PixelType.UnsignedByte,
                        bitmapData.Scan0);

                    // bitmap.UnlockBits(data);

                    GL.TexParameter(
                        TextureTarget.Texture2D,
                        TextureParameterName.TextureMinFilter,
                        (int)m_MinFilter);
                    GL.TexParameter(
                        TextureTarget.Texture2D,
                        TextureParameterName.TextureMagFilter,
                        (int)m_MagFilter);

                    m_TextureInfo.Add(newTextureInfo);
                }
            }
        }

        private static IEnumerable<BitmapData> GetAnimationBitmapData(string path)
        {
            List<BitmapData> bitmapData;

            s_BitmapData.TryGetValue(path, out bitmapData);
            if (bitmapData != null)
                return bitmapData;

            return CreateBitmapData(path);
        }

        private static IEnumerable<BitmapData> CreateBitmapData(string path)
        {
            Debug.Log("Loading image at " + path);

            var newList = new List<BitmapData>();
            using (var gif = Image.FromFile(path))
            {
                var dimension = new FrameDimension(gif.FrameDimensionsList[0]); // gets the GUID
                var frameCount = gif.GetFrameCount(dimension); // total frames in the animation

                for (var index = 0; index < frameCount; ++index)
                {
                    gif.SelectActiveFrame(dimension, index); // find the frame

                    var bitmap = new Bitmap(gif.Width, gif.Height); // make a copy of it

                    Graphics.FromImage(bitmap).DrawImage(gif, 0f, 0f);

                    newList.Add(
                        bitmap.LockBits(
                            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                            ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb));
                }

                s_BitmapData.Add(path, newList);
            }
            return newList;
        }
    }
}