namespace BroEngineEditor
{
    using Geometry;
    using ImGuiNET;

    [CustomInspector(typeof(MeshRenderer<>))]
    public class MeshRendererInspector : Inspector
    {
        public override void OnInspectorGUI()
        {
            var meshRenderer = target as MeshRenderer<Vertex>;
            if (meshRenderer == null)
                return;

            //ImGui.LabelText("Mesh", meshRenderer.mesh.ToString());
        }
    }
}
