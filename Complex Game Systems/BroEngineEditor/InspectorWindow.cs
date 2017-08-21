namespace BroEngineEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    using ImGuiNET;

    using Object = BroEngine.Object;

    internal class InspectorWindow : ResizableWindow
    {
        private Dictionary<Type, Inspector> s_Editors =
            new Dictionary<Type, Inspector>();

        private Inspector s_DefaultInspector = new Inspector();

        private Object m_SelectedObject;
        private Object selectedObject => m_SelectedObject ?? Editor.selectedObject;

        public override string name => "Inspector";

        public InspectorWindow()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var customEditorObjects = type.GetCustomAttributes(typeof(CustomInspectorAttribute), true);
                if (customEditorObjects.Length <= 0)
                    continue;

                var customEditor = customEditorObjects.FirstOrDefault() as CustomInspectorAttribute;
                if (customEditor != null && !s_Editors.ContainsKey(customEditor.type))
                    s_Editors.Add(
                        customEditor.type,
                        Activator.CreateInstance(type) as Inspector);
            }

            m_AllowResizeDirections = Direction.South | Direction.West;
        }

        protected override void PreTryResize()
        {
            ImGuiNative.igSetWindowPos(
                new Vector2(MyGameWindow.main.Width - ImGui.GetWindowSize().X, MainMenuBar.menuHeight),
                SetCondition.Always);

            ImGui.SetWindowSize(
                new Vector2(MyGameWindow.main.Width * 0.3f, MyGameWindow.main.Height * 0.66f),
                SetCondition.Once);
        }
        protected override unsafe void DrawWindowElements()
        {
            ImGui.PushID(selectedObject.id);
            ImGuiNative.igBeginGroup();
            {
                var buffer = Marshal.StringToHGlobalAuto(selectedObject.name);
                var bufferSize = (uint)Encoding.Unicode.GetByteCount(selectedObject.name);
                ImGui.InputText(
                    "Name", buffer, bufferSize, InputTextFlags.EnterReturnsTrue, OnEditName);

                if (s_Editors.TryGetValue(selectedObject.GetType(), out Inspector editor))
                {
                    editor.target = selectedObject;
                    editor.OnInspectorGUI();
                }
                else
                {
                    s_DefaultInspector.target = selectedObject;
                    s_DefaultInspector.OnInspectorGUI();
                }
            }
            ImGuiNative.igEndGroup();
            ImGui.PopID();
        }

        private unsafe int OnEditName(TextEditCallbackData* data)
        {
            selectedObject.name = Marshal.PtrToStringAuto(data->Buf);
            return 1;
        }
    }
}
