namespace BroEngineEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Reflection;
    using System.Text;

    using ImGuiNET;

    using Object = BroEngine.Object;

    public static class Inspector
    {
        private static Dictionary<Type, Editor> s_Editors =
            new Dictionary<Type, Editor>();

        private static Editor s_DefaultEditor = new Editor();

        public static Object selectedObject { get; set; }

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
                        Activator.CreateInstance(type, null) as Editor);
            }
        }

        public static void DrawGui()
        {
            if (ImGui.BeginWindow("Inspector", WindowFlags.NoMove))
            {
                ImGui.SetWindowSize(
                    new Vector2(ImGui.GetWindowSize().X, MyGameWindow.main.Height * 0.66f),
                    SetCondition.Always);

                ImGuiNative.igSetWindowPos(
                    new Vector2(MyGameWindow.main.Width - ImGui.GetWindowSize().X, 0f),
                    SetCondition.Always);

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
    }
}
