namespace BroEngineEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Reflection;

    using BroEngine;

    using ImGuiNET;

    [CustomInspector(typeof(GameObject))]
    public class GameObjectInspector : Inspector
    {
        private static Dictionary<Type, Inspector> s_ComponentEditors =
            new Dictionary<Type, Inspector>();
        private static Inspector s_DefaultInspector = new Inspector();

        private static bool s_IsInitialized;

        private static Component s_SelectedComponent;
        private static Component s_HoveredComponent;

        public GameObjectInspector()
        {
            if (s_IsInitialized)
                return;

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var customEditorObjects = type.GetCustomAttributes(typeof(CustomInspectorAttribute), true);
                if (customEditorObjects.Length <= 0)
                    continue;

                var customEditor = customEditorObjects.FirstOrDefault() as CustomInspectorAttribute;
                if (customEditor == null || !typeof(Component).IsAssignableFrom(customEditor.type))
                    continue;

                if (!s_ComponentEditors.ContainsKey(customEditor.type))
                    s_ComponentEditors.Add(
                        customEditor.type,
                        Activator.CreateInstance(type) as Inspector);
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
                ImGui.Columns(2, "Tags and Layers Columns", false);

                var tagIndex = gameObject.tagIndex;
                if (ImGui.Combo("Tag", ref tagIndex, TagLayerManager.instanceTags.ToArray()))
                    gameObject.tagIndex = tagIndex;

                ImGui.SameLine();
                if (ImGui.Button(" + "))
                    TagLayerManager.Select();

                ImGui.NextColumn();

                ImGui.SetColumnOffset(1, ImGui.GetWindowContentRegionWidth() * 0.4f);
                var layerIndex = gameObject.layerIndex;
                if (ImGui.Combo("Layer", ref layerIndex, TagLayerManager.instanceLayers.ToArray()))
                    gameObject.layerIndex = layerIndex;

                ImGui.SameLine();
                if (ImGui.Button(" + "))
                    TagLayerManager.Select();
            }
            ImGui.EndChildFrame();

            var components = gameObject.GetComponents<Component>().ToList();
            foreach (var component in components)
            {
                ImGui.PushID(component.id);
                {
                    if (DrawCollapsingHeader(component))
                    {
                        var componentType = component.GetType();
                        if (s_ComponentEditors.TryGetValue(componentType, out Inspector editor))
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
                                s_DefaultInspector.target = component;
                                s_DefaultInspector.OnInspectorGUI();
                            }
                        }
                        else
                        {
                            s_DefaultInspector.target = component;
                            s_DefaultInspector.OnInspectorGUI();
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
                    "Add Component",
                    new Vector2(ImGui.GetColumnWidth(ImGui.GetColumnIndex()) - 15f, 0)))
                    ImGui.OpenPopup("Add Component Popup");

                if (ImGui.BeginPopup("Add Component Popup"))
                {
                    foreach (var type in Assembly.GetCallingAssembly().GetTypes())
                    {
                        if (!typeof(Component).IsAssignableFrom(type) || type.IsAbstract)
                            continue;

                        if (ImGui.MenuItem(type.Name))
                            gameObject.AddComponent(type);
                    }
                    ImGui.EndPopup();
                }
            }
        }

        private static bool DrawCollapsingHeader(Component component)
        {
            if (s_HoveredComponent == component && s_SelectedComponent != s_HoveredComponent)
            {
                ImGui.Separator();
            }

            var enabledProperty = component.GetType().GetProperty("enabled");

            var label = enabledProperty == null ? component.name : "";
            var flags = TreeNodeFlags.Framed | TreeNodeFlags.DefaultOpen | TreeNodeFlags.AllowOverlapMode;
            var expanded = ImGui.CollapsingHeader(label, flags);

            if (ImGui.IsMouseDragging(0, -1) && s_SelectedComponent != null)
            {
                if (ImGui.IsLastItemHovered())
                    s_HoveredComponent = component;
            }

            if (ImGui.IsLastItemActive() && ImGui.IsMouseDragging(0, -1))
            {
                ImGui.SetTooltip(component.name);
                s_SelectedComponent = component;
            }
            else if (!ImGui.IsMouseDragging(0, -1) &&ImGui.IsLastItemHovered() && s_SelectedComponent != null)
            {
                Console.WriteLine(component.name);
                
                s_SelectedComponent = null;
                s_HoveredComponent = null;
            }

            if (ImGui.BeginPopupContextItem("Component Menu", 1))
            {
                if (ImGui.MenuItem("Reset", false))
                    ;// Reset Component

                ImGui.Separator();
                if (ImGui.MenuItem("Move to Front", false))
                    ;
                if (ImGui.MenuItem("Move to Back", false))
                    ;

                if (ImGui.MenuItem("Remove Component"))
                    BroEngine.Object.Destroy(component);

                if (ImGui.MenuItem("Move Up", false))
                    ;
                if (ImGui.MenuItem("Move Down", false))
                    ;

                if (ImGui.MenuItem("Copy Component", false))
                    ;

                if (ImGui.MenuItem("Paste Component as New", false))
                    ;
                if (ImGui.MenuItem("Paste Component Values", false))
                    ;

                ImGui.EndPopup();
            }

            if (enabledProperty != null)
            {
                ImGui.PushID("Enabled Checkbox");
                {
                    ImGui.SameLine();
                    var enabled = (bool)enabledProperty.GetMethod.Invoke(component, null);
                    if (ImGui.Checkbox("", ref enabled))
                        enabledProperty.SetMethod.Invoke(component, new object[] { enabled });

                    ImGui.SameLine();
                    ImGui.Text(component.name);
                }
                ImGui.PopID();
            }

            return expanded;
        }
    }
}
