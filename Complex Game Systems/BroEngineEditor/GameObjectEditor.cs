namespace BroEngineEditor
{
    using System.Numerics;

    using BroEngine;

    using ImGuiNET;

    [CustomEditor(typeof(GameObject))]
    public class GameObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var gameObject = target as GameObject;
            if (gameObject == null)
                return;

            ImGui.BeginChildFrame(0, new Vector2(ImGui.GetWindowSize().X, 25f), WindowFlags.NoScrollbar);
            {
                ImGui.Columns(2, "Test", false);
                var t = 1;
                ImGui.Combo("Tag Test", ref t, new[] { "Tag 1", "Tag 2" });
                ImGui.NextColumn();
                var f = 1;
                ImGui.Combo("Layer Test", ref f, new[] { "Layer 1", "Layer 2" });
            }
            ImGui.EndChildFrame();

            foreach (var component in gameObject.GetComponents<Component>())
            {
                ImGui.PushID(component.id);
                {
                    ImGui.CollapsingHeader(component.name, component.id.ToString(), true, true);
                }
                ImGui.PopID();
            }
        }
    }
}
