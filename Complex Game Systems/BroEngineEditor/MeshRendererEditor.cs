﻿namespace BroEngineEditor
{
    using Geometry;
    using ImGuiNET;

    [CustomEditor(typeof(MeshRenderer<>))]
    public class MeshRendererEditor : Editor
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
