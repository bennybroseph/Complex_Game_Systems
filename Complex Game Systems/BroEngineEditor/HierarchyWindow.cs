namespace BroEngineEditor
{
    using System.Collections.Generic;
    using System.Numerics;

    using BroEngine;
    using Geometry.Shapes;
    using ImGuiNET;
    using OpenTK.Graphics;

    internal class HierarchyWindow : ResizableWindow
    {
        public override string name => "Hierarchy";

        public GameObject placeAbove { get; private set; }

        public HierarchyWindow()
        {
            m_AllowResizeDirections = Direction.South | Direction.East;
        }

        protected override void PreTryResize()
        {
            ImGuiNative.igSetWindowPos(new Vector2(0f, MainMenuBar.menuHeight), SetCondition.Always);

            ImGui.SetWindowSize(
                new Vector2(MyGameWindow.main.Width * 0.15f, MyGameWindow.main.Height * 0.6f),
                SetCondition.Once);
        }

        protected override void DrawWindowElements()
        {
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
            foreach (var gameObject in FindObjectsOfType<GameObject>())
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
                            var min =
                                new Vector2(
                                    ImGui.GetLastItemRectMin().X,
                                    ImGui.GetLastItemRectMax().Y);
                            var max =
                                new Vector2(
                                    ImGui.GetWindowContentRegionWidth(),
                                    ImGui.GetLastItemRectMax().Y + 5f);

                            if (ImGui.IsMouseDragging(0, -1) && ImGui.IsMouseHoveringRect(min, max, false))
                            {
                                Editor.hoveredObject = this;
                                placeAbove = currentObject;
                                ImGui.Separator();
                                //Gizmos.DrawRectangle(
                                //    new OpenTK.Vector2(min.X, min.Y),
                                //    new OpenTK.Vector2(max.X, max.Y), Color4.White, Color4.White);
                            }

                            var collapsed = false;
                            var selected =
                                Editor.selectedObject == currentObject ||
                                Editor.hoveredObject == currentObject;

                            //ImGui.PushStyleColor(ColorTarget.HeaderHovered, Vector4.Zero);
                            //ImGui.PushStyleColor(ColorTarget.HeaderActive, Vector4.Zero);
                            //ImGui.PushStyleColor(ColorTarget.h, Vector4.Zero);
                            if (selected)
                                ImGuiNative.igPushStyleColor(
                                    ColorTarget.Text,
                                    new Vector4(143f / 255f, 143f / 255f, 200f / 255f, 1f));

                            var flags = TreeNodeFlags.OpenOnArrow;

                            if (currentObject.transform.childCount <= 0)
                                flags |= TreeNodeFlags.Leaf;
                            if (selected)
                                flags |= TreeNodeFlags.Selected;

                            collapsed = ImGui.TreeNodeEx(currentObject.name, flags);
                            if (selected)
                                ImGui.PopStyleColor();
                            //ImGui.PopStyleColor();

                            if (ImGui.IsMouseDragging(0, -1) &&
                                Editor.draggedObject != null &&
                                ImGui.IsLastItemHoveredRect())
                                Editor.hoveredObject = currentObject;

                            if (ImGui.IsLastItemActive() && ImGui.IsMouseDragging(0, -1))
                            {
                                ImGui.SetTooltip(currentObject.name);
                                Editor.draggedObject = currentObject;
                            }

                            if (ImGui.IsLastItemHovered() && ImGui.IsMouseClicked(0))
                                Editor.selectedObject = currentObject;

                            if (ImGui.BeginPopupContextItem("GameObject Context Menu"))
                            {
                                if (ImGui.Selectable("Delete"))
                                    ; // Object.Destroy(currentObject);

                                ImGui.EndPopup();
                            }

                            if (collapsed)
                                ImGui.TreePop();

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

            //if (m_DraggedObject != null)
            //{
            //    m_Dragging = true;
            //    ImGui.SetTooltip(m_DraggedObject.name);

            //    if (!ImGui.IsMouseDown(0) && m_DraggedObject != m_HoveringObject)
            //    {
            //        m_DraggedObject.transform.SetParent(m_HoveringObject?.transform);
            //        ImGuiNative.igSetWindowFocus();
            //        m_Dragging = false;
            //    }
            //}
            //else if (ImGui.IsWindowFocused())
            //    ImGui.SetTooltip("");

            //if (!m_Dragging)
            //    m_DraggedObject = null;
        }
    }
}
