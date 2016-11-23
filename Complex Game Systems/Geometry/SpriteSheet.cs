using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    using OpenTK;
    using OpenTK.Graphics.ES10;

    class SpriteSheet
    {
        private readonly List<Texture> m_Textures = new List<Texture>();

        private Vector2 m_GridSize;

        public SpriteSheet(
            string path, TextureMinFilter minFilter, TextureMagFilter magFilter, Vector2 gridSize)
        {
            m_GridSize = gridSize;


        }


    }
}
