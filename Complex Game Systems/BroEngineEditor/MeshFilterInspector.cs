namespace BroEngineEditor
{
    using BroEngine;
    using Geometry;

    [CustomInspector(typeof(MeshFilter<>))]
    class MeshFilterInspector : Inspector
    {
        public override void OnInspectorGUI()
        {
            var meshFilter = target as MeshFilter<Vertex>;
            if (meshFilter == null)
                return;


        }
    }
}
