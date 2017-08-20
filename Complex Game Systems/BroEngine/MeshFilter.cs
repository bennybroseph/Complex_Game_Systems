namespace BroEngine
{
    using Geometry;

    public class MeshFilter<TVertex> : Component where TVertex : struct
    {
        public Mesh<TVertex> mesh { get; set; }
    }
}
