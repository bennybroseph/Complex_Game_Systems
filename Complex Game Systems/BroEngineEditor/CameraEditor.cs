namespace BroEngineEditor
{
    using BroEngine;

    using ImGuiNET;

    [CustomEditor(typeof(Camera))]
    public class CameraEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var camera = target as Camera;
            if (camera == null)
                return;

            var fieldOfView = camera.fieldOfView;
            if (ImGui.DragFloat("Field of View", ref fieldOfView, 0.001f, 1f, 0.001f, "%.3f"))
                camera.fieldOfView = fieldOfView;

            var near = camera.near;
            if (ImGui.DragFloat("Near", ref near, 0.001f, camera.far - 0.001f, 0.01f, "%.3f"))
                camera.near = near;

            var far = camera.far;
            if (ImGui.DragFloat("Far", ref far, camera.near + 0.001f, float.MaxValue, 0.01f, "%.3f"))
                camera.far = far;
        }
    }
}
