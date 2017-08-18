namespace BroEngine
{
    using Geometry;

    public class MeshFilter<TVertex> : Renderer where TVertex : struct
    {
        public Mesh<TVertex> mesh { get; set; }
    }
}
