namespace Geometry
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    using Utility;

    using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

    public class Texture
    {
        private readonly int m_Handle;

        private readonly string m_Path;

        private Size m_Size;

        protected TextureMinFilter m_MinFilter;
        protected TextureMagFilter m_MagFilter;

        private static readonly Dictionary<string, BitmapData> s_BitmapData =
            new Dictionary<string, BitmapData>();

        public Vector2 position { get; private set; }

        public virtual int width => m_Size.Width;
        public virtual int height => m_Size.Height;

        public static Dictionary<string, Bitmap> bitmaps { get; private set; } =
            new Dictionary<string, Bitmap>();

        protected Texture() { }
        public Texture(string path, TextureMinFilter minFilter, TextureMagFilter magFilter) :
            this(path, minFilter, magFilter, Vector2.Zero, Size.Empty)
        { }
        public Texture(
            string path, TextureMinFilter minFilter, TextureMagFilter magFilter, Vector2 position, Size size)
        {
            m_MinFilter = minFilter;
            m_MagFilter = magFilter;

            m_Path = path;

            this.position = position;
            m_Size = size;

            GL.GenTextures(1, out m_Handle);
        }

        public virtual void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, m_Handle);
        }
        public static void UnBind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public virtual void BufferData()
        {
            var data = GetBitmapData(m_Path);

            m_Size.Width = data.Width;
            m_Size.Height = data.Height;

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                width,
                height,
                0,
                PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0);

            GL.TexParameter(
                TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)m_MinFilter);
            GL.TexParameter(
                TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)m_MagFilter);
        }

        protected static BitmapData GetBitmapData(string path)
        {
            BitmapData bitmapData;

            s_BitmapData.TryGetValue(path, out bitmapData);
            if (bitmapData != null)
                return bitmapData;

            return CreateBitmapData(path);
        }

        protected static BitmapData CreateBitmapData(string path)
        {
            Debug.Log("Loading image at " + path);

            var bitmap = new Bitmap(path);
            var data =
                bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            s_BitmapData.Add(path, data);

            return data;
        }

        protected static string CreateDataKey(string path, Vector2 position, Size size)
        {
            return path + " " + position + " " + size;
        }
    }
}
