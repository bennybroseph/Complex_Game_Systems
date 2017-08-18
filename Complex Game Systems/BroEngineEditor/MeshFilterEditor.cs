namespace BroEngineEditor
{
    using BroEngine;
    using Geometry;

    [CustomEditor(typeof(MeshFilter<>))]
    class MeshFilterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var meshFilter = target as MeshFilter<Vertex>;
            if (meshFilter == null)
                return;


        }
    }
}
