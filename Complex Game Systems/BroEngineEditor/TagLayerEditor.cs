namespace BroEngineEditor
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using System.Text;

    using BroEngine;

    using ImGuiNET;

    using Object = BroEngine.Object;

    [CustomEditor(typeof(TagLayerManager))]
    public class TagLayerEditor : Editor
    {
        private delegate void Removed(int removedIndex);

        private byte[] m_CurrentBuffer;

        private int m_SelectedTag = -1;
        private int m_SelectedLayer = -1;

        public override void OnInspectorGUI()
        {
            var tagLayerManager = target as TagLayerManager;
            if (tagLayerManager == null)
                return;

            DrawList("Tag", tagLayerManager.tags, ref m_SelectedTag, OnRemovedTag);
            DrawList("Layer", tagLayerManager.layers, ref m_SelectedLayer, OnRemovedLayer);
        }

        private void DrawList(string name, IList<string> strings, ref int selectedIndex, Removed onRemoved)
        {
            if (ImGui.TreeNodeEx(name + "s", TreeNodeFlags.DefaultOpen))
            {
                for (var i = 0; i < strings.Count; i++)
                {
                    var flags =
                        TreeNodeFlags.Leaf |
                        TreeNodeFlags.NoTreePushOnOpen |
                        (selectedIndex == i ? TreeNodeFlags.Selected : 0);

                    ImGui.TreeNodeEx(strings[i], flags);
                    if (ImGuiNative.igIsItemClicked(0))
                        selectedIndex = i;
                }

                if (ImGui.Button(" + "))
                {
                    m_CurrentBuffer = Encoding.Default.GetBytes("New " + name);
                    ImGui.OpenPopup("Enter New " + name);
                }

                if (ImGui.BeginPopupModal("Enter New " + name, WindowFlags.AlwaysAutoResize))
                {
                    var flags = InputTextFlags.AutoSelectAll;
                    ImGui.InputText("", m_CurrentBuffer, 35, flags, null);

                    if (ImGui.Button("OK"))
                    {
                        strings.Add(Encoding.Default.GetString(m_CurrentBuffer));
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("Cancel"))
                        ImGui.CloseCurrentPopup();

                    ImGui.EndPopup();
                }

                ImGui.SameLine();
                if (ImGui.Button(" - "))
                {
                    onRemoved(selectedIndex);
                    strings.RemoveAt(selectedIndex);
                }
            }
            ImGui.TreePop();
        }

        private static void OnRemovedTag(int index)
        {
            foreach (var gameObject in GameObject.FindGameObjectsWithTag(TagLayerManager.instanceTags[index]))
                gameObject.tagIndex = 0;
        }

        private static void OnRemovedLayer(int index)
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                if (gameObject.layerIndex == index)
                    gameObject.layerIndex = 0;
            }
        }
    }
}
