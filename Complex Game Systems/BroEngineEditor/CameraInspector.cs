namespace BroEngineEditor
{
    using System.Numerics;

    using BroEngine;

    using ImGuiNET;

    [CustomInspector(typeof(Camera))]
    public class CameraInspector : Inspector
    {
        public override void OnInspectorGUI()
        {
            var camera = target as Camera;
            if (camera == null)
                return;
            
            var index = camera.isOrthographic ? 1 : 0;
            if (ImGui.Combo("Projection", ref index, new[] { "Perspective", "Orthographic" }))
                camera.isOrthographic = index == 1;

            var fieldOfView = camera.fieldOfView;
            if (ImGui.DragFloat("Field of View", ref fieldOfView, 0.001f, 1f, 0.001f, "%.3f"))
                camera.fieldOfView = fieldOfView;

            var near = camera.near;
            if (ImGui.DragFloat("Near", ref near, 0.001f, camera.far - 0.001f, 0.01f, "%.3f"))
                camera.near = near;

            var far = camera.far;
            if (ImGui.DragFloat("Far", ref far, camera.near + 0.001f, float.MaxValue, 0.01f, "%.3f"))
                camera.far = far;

            ImGui.Text("");
            ImGui.Text("Viewport Rect");

            var position = new Vector2(camera.rect.position.X, camera.rect.position.Y);
            var size = new Vector2(camera.rect.size.X, camera.rect.size.Y);

            var changedPosition =
                ImGui.DragVector2(
                    "Position", ref position, float.MinValue, float.MaxValue, 0.001f, "%.3f");
            var changedSize =
                ImGui.DragVector2(
                    "Size", ref size, float.MinValue, float.MaxValue, 0.001f, "%.3f");
            if (changedPosition || changedSize)
                camera.rect = new Rect(position.X, position.Y, size.X, size.Y);

        }
    }
}
