namespace BroEngineEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;

    using ImGuiNET;

    using ImGuiUtility;

    using Object = BroEngine.Object;

    public static class Inspector
    {
        private static Dictionary<Type, Editor> s_Editors =
            new Dictionary<Type, Editor>();

        private static Editor s_DefaultEditor = new Editor();

        public static Object selectedObject { get; set; }

        private static bool m_IsHoveringNS;
        private static bool m_IsHoveringWE;

        private static bool m_IsResizingNS;
        private static bool m_IsResizingWE;

        public static void Init()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var customEditorObjects = type.GetCustomAttributes(typeof(CustomEditorAttribute), true);
                if (customEditorObjects.Length <= 0)
                    continue;

                var customEditor = customEditorObjects.FirstOrDefault() as CustomEditorAttribute;
                if (customEditor != null && !s_Editors.ContainsKey(customEditor.type))
                    s_Editors.Add(
                        customEditor.type,
                        Activator.CreateInstance(type) as Editor);
            }


            ImGuiOpenTK.drawEvent += DrawGui;
        }

        private static void DrawGui()
        {
            //var t = true;
            //ImGuiNative.igShowTestWindow(ref t);
            //return;

            if (ImGui.BeginWindow("Inspector", WindowFlags.NoMove | WindowFlags.NoResize))
            {
                ImGui.SetWindowSize(
                    new Vector2(MyGameWindow.main.Width * 0.3f, MyGameWindow.main.Height * 0.66f),
                    SetCondition.Once);

                ImGuiNative.igSetWindowPos(
                    new Vector2(MyGameWindow.main.Width - ImGui.GetWindowSize().X, MainMenu.menuHeight),
                    SetCondition.Always);

                TryResizeNSWE();

                ImGui.PushID(selectedObject.id);
                ImGuiNative.igBeginGroup();
                {
                    var buffer = Encoding.Default.GetBytes(selectedObject.name);
                    if (ImGui.InputText(
                        "Name", buffer, 35, InputTextFlags.EnterReturnsTrue, null))
                        selectedObject.name = Encoding.Default.GetString(buffer);

                    if (s_Editors.TryGetValue(selectedObject.GetType(), out Editor editor))
                    {
                        editor.target = selectedObject;
                        editor.OnInspectorGUI();
                    }
                    else
                    {
                        s_DefaultEditor.target = selectedObject;
                        s_DefaultEditor.OnInspectorGUI();
                    }
                }
                ImGuiNative.igEndGroup();
                ImGui.PopID();
            }
            ImGui.EndWindow();
        }

        private static unsafe void TryResizeNSWE()
        {
            if (m_IsHoveringNS && m_IsHoveringWE)
                Cursor.Current = Cursors.SizeNESW;

            if (m_IsResizingNS && m_IsResizingWE)
            {
                Cursor.Current = Cursors.SizeNESW;

                var io = *ImGuiNative.igGetIO();

                ImGuiNative.igSetWindowPos(
                    ImGui.GetWindowPosition() + new Vector2(io.MouseDelta.X, 0f), SetCondition.Always);
                ImGui.SetWindowSize(ImGui.GetWindowSize() + new Vector2(-io.MouseDelta.X, io.MouseDelta.Y));

                if (!ImGui.IsMouseDragging(0, 0))
                    m_IsResizingWE = false;
            }
            else
            {
                    TryResizeNS();
                    TryResizeWE();
            }
        }

        private static unsafe void TryResizeNS()
        {
            if (m_IsResizingNS)
            {
                Cursor.Current = Cursors.SizeNS;

                var io = *ImGuiNative.igGetIO();

                var mouseDelta = new Vector2(0f, io.MouseDelta.Y);
                ImGui.SetWindowSize(ImGui.GetWindowSize() + mouseDelta);

                if (!ImGui.IsMouseDragging(0, 0))
                    m_IsResizingNS = false;
            }

            var min = ImGui.GetWindowPosition() + new Vector2(0f, ImGui.GetWindowHeight() - 5f);
            var max = min + new Vector2(ImGui.GetWindowWidth(), 10f);
            if (ImGui.IsMouseHoveringRect(min, max, false))
            {
                if (!m_IsHoveringWE)
                    Cursor.Current = Cursors.SizeNS;

                m_IsHoveringNS = true;
                if (ImGui.IsMouseDragging(0, 0))
                    m_IsResizingNS = true;
            }
            else if (!m_IsResizingNS)
                m_IsHoveringNS = false;
        }
        private static unsafe void TryResizeWE()
        {
            if (m_IsResizingWE)
            {
                Cursor.Current = Cursors.SizeWE;

                var io = *ImGuiNative.igGetIO();

                var mouseDelta = new Vector2(io.MouseDelta.X, 0f);
                ImGuiNative.igSetWindowPos(ImGui.GetWindowPosition() + mouseDelta, SetCondition.Always);
                ImGui.SetWindowSize(ImGui.GetWindowSize() - mouseDelta);

                if (!ImGui.IsMouseDragging(0, 0))
                    m_IsResizingWE = false;
            }

            var min = ImGui.GetWindowPosition() - new Vector2(5f, 0f);
            var max = min + new Vector2(10, ImGui.GetWindowHeight());
            if (ImGui.IsMouseHoveringRect(min, max, false))
            {
                if (!m_IsHoveringNS)
                    Cursor.Current = Cursors.SizeWE;

                m_IsHoveringWE = true;
                if (ImGui.IsMouseDragging(0, 0))
                    m_IsResizingWE = true;
            }
            else if (!m_IsResizingWE)
                m_IsHoveringWE = false;
        }
    }
}
