﻿namespace BroEngineEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Reflection;

    using BroEngine;

    using ImGuiNET;

    [CustomEditor(typeof(GameObject))]
    public class GameObjectEditor : Editor
    {
        private static Dictionary<Type, Editor> s_ComponentEditors =
            new Dictionary<Type, Editor>();
        private static Editor s_DefaultEditor = new Editor();

        private static bool s_IsInitialized;

        public GameObjectEditor()
        {
            if (s_IsInitialized)
                return;

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var customEditorObjects = type.GetCustomAttributes(typeof(CustomEditorAttribute), true);
                if (customEditorObjects.Length <= 0)
                    continue;

                var customEditor = customEditorObjects.FirstOrDefault() as CustomEditorAttribute;
                if (customEditor == null || !typeof(Component).IsAssignableFrom(customEditor.type))
                    continue;

                if (!s_ComponentEditors.ContainsKey(customEditor.type))
                    s_ComponentEditors.Add(
                        customEditor.type,
                        Activator.CreateInstance(type) as Editor);
            }

            s_IsInitialized = true;
        }

        public override void OnInspectorGUI()
        {
            var gameObject = target as GameObject;
            if (gameObject == null)
                return;

            ImGui.BeginChildFrame(0, new Vector2(ImGui.GetWindowWidth(), 25f), WindowFlags.NoScrollbar);
            {
                ImGui.Columns(2, "Test", false);
                var t = 1;
                ImGui.Combo("Tag Test", ref t, new[] { "Tag 1", "Tag 2" });
                ImGui.NextColumn();
                var f = 1;
                ImGui.Combo("Layer Test", ref f, new[] { "Layer 1", "Layer 2" });
            }
            ImGui.EndChildFrame();

            var components = gameObject.GetComponents<Component>().ToList();
            foreach (var component in components)
            {
                ImGui.PushID(component.id);
                {
                    if (ImGui.CollapsingHeader(component.name, component.id.ToString(), true, true))
                    {
                        if (ImGui.BeginPopupContextItem("Component Menu", 1))
                        {
                            if (ImGui.Selectable("Reset"))
                            {
                                // Reset Component
                            }
                            if (ImGui.Selectable("Remove Component"))
                            {
                                BroEngine.Object.Destroy(component);
                            }
                            ImGui.EndPopup();
                        }

                        var componentType = component.GetType();
                        if (s_ComponentEditors.TryGetValue(componentType, out Editor editor))
                        {
                            editor.target = component;
                            editor.OnInspectorGUI();
                        }
                        else if (componentType.IsGenericType)
                        {
                            var foundEditor = false;
                            foreach (var keyValuePair in s_ComponentEditors)
                            {
                                var underlyingType = componentType.GetGenericTypeDefinition();
                                if (underlyingType != keyValuePair.Key)
                                    continue;

                                keyValuePair.Value.target = component;
                                keyValuePair.Value.OnInspectorGUI();

                                foundEditor = true;
                                break;
                            }

                            if (!foundEditor)
                            {
                                s_DefaultEditor.target = component;
                                s_DefaultEditor.OnInspectorGUI();
                            }
                        }
                        else
                        {
                            s_DefaultEditor.target = component;
                            s_DefaultEditor.OnInspectorGUI();
                        }
                    }
                }
                ImGui.PopID();
            }

            ImGui.LabelText("", "");

            ImGui.Columns(3, "Add Component Column", false);
            {
                ImGui.NextColumn();
                if (ImGui.Button(
                    "Add Component", new Vector2(ImGui.GetColumnWidth(ImGui.GetColumnIndex()) - 15f, 0)))
                {
                    // Add Component
                }
            }
        }
    }
}
