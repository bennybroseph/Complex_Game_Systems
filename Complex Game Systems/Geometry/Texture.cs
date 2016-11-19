namespace Geometry
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    using OpenTK.Graphics.OpenGL;

    using Utility;

    using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

    public class Texture
    {
        private readonly int m_Handle;

        private string m_Path;

        protected TextureMinFilter m_MinFilter;
        protected TextureMagFilter m_MagFilter;

        private static readonly Dictionary<string, BitmapData> s_BitmapData =
            new Dictionary<string, BitmapData>();

        public virtual int width { get; private set; }
        public virtual int height { get; private set; }

        protected Texture() { }
        public Texture(string path, TextureMinFilter minFilter, TextureMagFilter magFilter)
        {
            m_MinFilter = minFilter;
            m_MagFilter = magFilter;

            m_Path = path;

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

            width = data.Width;
            height = data.Height;

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
    }
}
