namespace BroEngineEditor
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;

    using BroEngine;

    using ImGuiNET;

    using ImGuiUtility;

    using Object = BroEngine.Object;

    internal static class HierarchyWindow
    {
        private static GameObject s_DraggedObject;
        private static GameObject s_HoveringObject;

        private static bool s_Dragging;

        public static void Init() { ImGuiOpenTK.drawEvent += DrawGui; }

        private static void DrawGui()
        {
            ImGui.BeginWindow("Hierarchy", WindowFlags.NoMove | WindowFlags.MenuBar);
            {
                ImGuiNative.igSetWindowPos(new Vector2(0f, MainMenuBar.menuHeight), SetCondition.Always);

                if (ImGuiNative.igBeginPopupContextWindow(false, "Hierarchy Context Menu", 1))
                {
                    MainMenuBar.DrawCreateMenu();

                    ImGui.EndPopup();
                }

                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("Create"))
                    {
                        MainMenuBar.DrawCreateMenu();
                        ImGui.EndMenu();
                    }

                    ImGui.EndMenuBar();
                }

                var indentLevel = 0;
                foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
                {
                    if (gameObject.transform.parent == null)
                    {
                        var tempQueue = new Stack<Queue<GameObject>>();

                        var gameObjectStacks = new Stack<Queue<GameObject>>();
                        var gameObjectQueue = new Queue<GameObject>();
                        gameObjectQueue.Enqueue(gameObject);

                        gameObjectStacks.Push(gameObjectQueue);

                        var currentStack = gameObjectStacks.Peek();
                        var currentObject = gameObjectQueue.Peek();

                        while (gameObjectStacks.Count > 0)
                        {
                            while (currentStack.Count > 0)
                            {
                                var collapsed = false;
                                var selected = InspectorWindow.selectedObject == currentObject;

                                //ImGui.PushStyleColor(ColorTarget.HeaderHovered, Vector4.Zero);
                                //ImGui.PushStyleColor(ColorTarget.HeaderActive, Vector4.Zero);
                                if (selected)
                                    ImGuiNative.igPushStyleColor(
                                        ColorTarget.Text,
                                        new Vector4(143f / 255f, 143f / 255f, 200f / 255f, 1f));

                                var flags =
                                    currentObject.transform.childCount > 0
                                        ? TreeNodeFlags.OpenOnArrow
                                        : TreeNodeFlags.OpenOnArrow | TreeNodeFlags.Leaf;
                                if (selected)
                                    flags |= TreeNodeFlags.Selected;

                                collapsed = ImGui.TreeNodeEx(currentObject.name, flags);
                                if (selected)
                                    ImGui.PopStyleColor();
                                //ImGui.PopStyleColor();

                                if (ImGui.BeginPopupContextItem("GameObject Context Menu"))
                                {
                                    if (ImGui.Selectable("Delete"))
                                        ; // Object.Destroy(currentObject);

                                    ImGui.EndPopup();
                                }

                                if (collapsed)
                                    ImGui.TreePop();

                                if (ImGuiNative.igIsItemHovered())
                                {
                                    s_HoveringObject = currentObject;

                                    if (ImGui.IsMouseClicked(0))
                                        InspectorWindow.selectedObject = currentObject;

                                    if (ImGui.IsMouseDragging(0, -1) && !s_Dragging)
                                        s_DraggedObject = currentObject;
                                }

                                if (collapsed && currentObject.transform.childCount > 0)
                                {
                                    ImGuiNative.igIndent();
                                    indentLevel++;

                                    currentStack.Dequeue();
                                    tempQueue.Push(new Queue<GameObject>());
                                    foreach (var child in currentObject.transform)
                                        tempQueue.Peek().Enqueue(child.gameObject);
                                    gameObjectStacks.Push(tempQueue.Peek());

                                    currentStack = tempQueue.Peek();
                                    currentObject = tempQueue.Peek().Peek();
                                }
                                else
                                {
                                    currentStack.Dequeue();
                                    if (currentStack.Count > 0)
                                        currentObject = currentStack.Peek();
                                }
                            }
                            gameObjectStacks.Pop();
                            if (gameObjectStacks.Count > 0)
                            {
                                if (indentLevel > 0)
                                {
                                    ImGuiNative.igUnindent();
                                    indentLevel--;
                                }

                                currentStack = gameObjectStacks.Peek();
                                if (currentStack.Count > 0)
                                    currentObject = currentStack.Peek();
                            }
                        }
                    }
                    for (var i = 0; i < indentLevel; ++i, --indentLevel)
                        ImGuiNative.igUnindent();
                }

                if (s_DraggedObject != null)
                {
                    s_Dragging = true;
                    ImGui.SetTooltip(s_DraggedObject.name);

                    if (!ImGui.IsMouseDown(0) && s_DraggedObject != s_HoveringObject)
                    {
                        s_DraggedObject.transform.SetParent(s_HoveringObject?.transform);
                        ImGuiNative.igSetWindowFocus();
                        s_Dragging = false;
                    }
                }
                else if(ImGui.IsWindowFocused())
                    ImGui.SetTooltip("");

                if (!s_Dragging)
                    s_DraggedObject = null;
            }
            ImGui.EndWindow();
        }


    }
}
