namespace BroEngineEditor
{
    using BroEngine;

    using ImGuiNET;

    using ImGuiUtility;

    internal static class MainMenu
    {
        public static float menuHeight;

        public static void Init() { ImGuiOpenTK.drawEvent += DrawGui; }

        public static void DrawGui()
        {
            if (!ImGui.BeginMainMenuBar())
                return;

            menuHeight = ImGui.GetWindowHeight();

            if (ImGui.BeginMenu("File"))
            {
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.BeginMenu("Preferences"))
                {
                    if (ImGui.MenuItem(TagLayerManager.displayName))
                    {
                        TagLayerManager.Select();
                    }
                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }
}
