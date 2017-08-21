namespace BroEngineEditor
{
    using System;

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
                if (ImGui.MenuItem("New Scene", false))
                    ;
                if (ImGui.MenuItem("Open Scene", false))
                    ;

                ImGui.Separator();
                if (ImGui.MenuItem("Save Scenes", false))
                    ;
                if (ImGui.MenuItem("Save Scene as..", false))
                    ;

                ImGui.Separator();
                if (ImGui.MenuItem("New Project..", false))
                    ;
                if (ImGui.MenuItem("Open Project..", false))
                    ;
                if (ImGui.MenuItem("Save Project", false))
                    ;

                ImGui.Separator();
                if (ImGui.MenuItem("Exit"))
                    MyGameWindow.main.Exit();

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.BeginMenu("Project Settings"))
                {
                    if (ImGui.MenuItem(TagLayerManager.displayName))
                        TagLayerManager.Select();

                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("GameObject"))
            {
                DrawCreateMenu();

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }

        public static void DrawCreateMenu()
        {
            if (ImGui.MenuItem("Create Empty"))
                new GameObject();

            if (ImGui.BeginMenu("3D"))
            {
                foreach (PrimitiveType name in Enum.GetValues(typeof(PrimitiveType)))
                    if (ImGui.MenuItem(name.ToString()))
                        GameObject.CreatePrimitive(name);

                ImGui.EndMenu();
            }
        }
    }
}
