namespace BroEngineEditor
{
    using System.Numerics;
    using System.Windows.Forms;

    using ImGuiNET;

    internal static class DockWindowTest
    {
        private static bool m_IsResizingWE;

        public static unsafe void DrawGui()
        {
            if (ImGui.BeginWindow("Window 1", WindowFlags.NoMove | WindowFlags.MenuBar))
            {
                ImGui.Text("Other Text");
            }
            ImGui.EndWindow();
        }
    }
}
