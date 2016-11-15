namespace ComplexGameSystems.Geometry
{
    using System.Drawing;
    using System.Drawing.Imaging;

    using ComplexGameSystems.Utility;

    using OpenTK.Graphics.OpenGL;

    using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

    public class Texture
    {
        private readonly int m_Handle;

        private string m_Path;

        private TextureMinFilter m_MinFilter;
        private TextureMagFilter m_MagFilter;

        private int m_ImageWidth;
        private int m_ImageHeight;

        public Texture(string path, TextureMinFilter minFilter, TextureMagFilter magFilter)
        {
            m_MinFilter = minFilter;
            m_MagFilter = magFilter;

            Debug.Log("Loading image at " + path);

            var bitmap = new Bitmap(path);
            BitmapData data =
                bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            m_ImageWidth = bitmap.Width;
            m_ImageHeight = bitmap.Height;

            GL.GenTextures(1, out m_Handle);
            Bind();

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                m_ImageWidth,
                m_ImageHeight,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                data.Scan0);

            bitmap.UnlockBits(data);

            GL.TexParameter(
                TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)m_MinFilter);
            GL.TexParameter(
                TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)m_MagFilter);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, m_Handle);
        }
        public void UnBind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
