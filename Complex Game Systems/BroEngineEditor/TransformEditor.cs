namespace BroEngineEditor
{
    using System.Numerics;

    using BroEngine;

    using Geometry;

    using ImGuiNET;

    [CustomEditor(typeof(Transform))]
    public class TransformEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var t = typeof(Model<>);
            var transform = target as Transform;
            if (transform == null)
                return;

            var localPosition =
                new Vector3(transform.localPosition.X, transform.localPosition.Y, transform.localPosition.Z);

            var localRotation =
                new Vector3(
                    transform.localEulerAngles.X,
                    transform.localEulerAngles.Y,
                    transform.localEulerAngles.Z);

            var localScale = transform.localScale.X;

            if (ImGui.DragVector3(
                "Position", ref localPosition, float.MinValue, float.MaxValue, 0.1f, "%.3f"))
                transform.localPosition =
                    new OpenTK.Vector3(localPosition.X, localPosition.Y, localPosition.Z);

            if (ImGui.DragVector3(
                "Rotation", ref localRotation, float.MinValue, float.MaxValue, 0.1f, "%.3f"))
                transform.localEulerAngles =
                    new OpenTK.Vector3(localRotation.X, localRotation.Y, localRotation.Z);

            if (ImGui.DragFloat(
                "Scale", ref localScale, 0.01f, float.MaxValue, 0.1f, "%.3f"))
                transform.localScale = new OpenTK.Vector3(localScale);
        }
    }
}
