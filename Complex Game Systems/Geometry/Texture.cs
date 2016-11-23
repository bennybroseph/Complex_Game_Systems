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

        protected TextureMinFilter m_MinFilter;
        protected TextureMagFilter m_MagFilter;

        private static readonly Dictionary<string, BitmapData> s_BitmapData =
            new Dictionary<string, BitmapData>();

        public Vector2 position { get; private set; }
        public Size size { get; private set; }

        public virtual int width => size.Width;
        public virtual int height => size.Height;

        public static Dictionary<string, Bitmap> bitmaps
        { get; private set; } = new Dictionary<string, Bitmap>();

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
            this.size = size;

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
            var data = GetBitmapData(m_Path, position, size);

            size = new Size(data.Width, data.Height);

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

            // bitmap.UnlockBits(data);

            GL.TexParameter(
                TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)m_MinFilter);
            GL.TexParameter(
                TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)m_MagFilter);
        }

        public static Bitmap GetBitmap(string path)
        {
            Bitmap bitmap;
            bitmaps.TryGetValue(path, out bitmap);
            if (bitmap != null)
                return bitmap;

            return LoadBitmap(path);
        }
        public static BitmapData GetBitmapData(string path, Vector2 position, Size size)
        {
            BitmapData bitmapData;
            s_BitmapData.TryGetValue(path, out bitmapData);
            if (bitmapData != null)
                return bitmapData;

            var bitmap = GetBitmap(path);
            if (size == Size.Empty)
                size = bitmap.Size;

            return CreateBitmapData(bitmap, path, position, size);
        }

        private static Bitmap LoadBitmap(string path)
        {
            Debug.Log("Loading image at " + path);

            var newBitmap = new Bitmap(path);
            bitmaps.Add(path, newBitmap);

            return newBitmap;
        }

        protected static BitmapData CreateBitmapData(Bitmap bitmap, string path, Vector2 position, Size size)
        {
            var data =
                bitmap.LockBits(
                    new Rectangle((int)position.X, (int)position.Y, size.Width, size.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            s_BitmapData.Add(CreateDataKey(path, position, size), data);

            return data;
        }

        private static string CreateBitmapKey(string path)
        {
            return path;
        }
        protected static string CreateDataKey(string path, Vector2 position, Size size)
        {
            return path + " " + position + " " + size;
        }
    }
}
