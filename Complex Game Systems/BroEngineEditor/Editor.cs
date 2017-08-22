namespace BroEngineEditor
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using System.Windows.Forms;
    using BroEngine;
    using ImGuiNET;

    using ImGuiUtility;
    using OpenTK.Input;
    using Object = BroEngine.Object;

    public enum PopupModalButtons
    {
        OK,
        OK_CANCEL,
        YES_NO,
    }
    internal static class Editor
    {
        public delegate void ButtonAction(bool result);

        private class PopupModal
        {
            public string title;
            public string message;

            public PopupModalButtons popupModalButtons;

            public ButtonAction buttonAction;
        }
        private static List<PopupModal> s_PopupModals = new List<PopupModal>();

        public static Object draggedObject { get; set; }

        private static bool s_HoveredWasSet;
        private static Object s_HoveredObject;

        public static Object hoveredObject
        {
            get => s_HoveredObject;
            set { s_HoveredWasSet = value != null; s_HoveredObject = value; }
        }

        public static Object selectedObject { get; set; }
        private static List<InspectorWindow> s_InspectorWindows = new List<InspectorWindow>();

        private static List<HierarchyWindow> s_HierarchyWindows = new List<HierarchyWindow>();

        private static List<ProjectWindow> s_ProjectWindows = new List<ProjectWindow>();

        public static void Init()
        {
            ImGuiOpenTK.preRender += OnPreRender;
            ImGuiOpenTK.drawGui += OnDrawGui;
            ImGuiOpenTK.postRender += OnPostRender;

            s_InspectorWindows.Add(new InspectorWindow());
            s_HierarchyWindows.Add(new HierarchyWindow());
            s_ProjectWindows.Add(new ProjectWindow());
        }

        public static void ShowMessageBox(
            string title,
            string message,
            PopupModalButtons popupModalButtons,
            ButtonAction buttonAction = null)
        {
            s_PopupModals.Add(
                new PopupModal
                {
                    title = title,
                    message = message,
                    popupModalButtons = popupModalButtons,
                    buttonAction = buttonAction,
                });
        }

        private static void OnPreRender()
        {
            if (ImGui.IsAnyItemActive())
                WrapCursor();

            if (hoveredObject != null && !s_HoveredWasSet)
                hoveredObject = null;

            if (draggedObject != null)
            {
                if (ImGui.IsMouseDragging(0, -1))
                    Cursor.Current = CanCatch() ? Cursors.Default : Cursors.No;
                else
                {
                    Console.WriteLine(
                        draggedObject + " was dropped and " + hoveredObject + " should pick it up");
                    if (hoveredObject is HierarchyWindow)
                        Console.WriteLine(
                            "It should be placed above " +
                            (hoveredObject as HierarchyWindow).placeAbove.name);

                    Catch();
                    hoveredObject = null;
                    draggedObject = null;
                }
            }

            s_HoveredWasSet = false;
        }

        private static void OnDrawGui()
        {
            var popups = s_PopupModals.ToList();
            foreach (var popupModal in popups)
            {
                ImGui.OpenPopup(popupModal.message);

                var flags =
                    WindowFlags.NoTitleBar | WindowFlags.MenuBar | WindowFlags.NoMove | WindowFlags.NoResize;

                ImGui.SetNextWindowPosCenter(SetCondition.Always);
                ImGui.SetNextWindowSize(new Vector2(300, 175), SetCondition.Always);
                if (ImGui.BeginPopupModal(popupModal.message, flags))
                {
                    if (ImGui.BeginMenuBar())
                    {
                        ImGui.Text(popupModal.title);
                        ImGui.EndMenuBar();
                    }

                    ImGui.BeginChild(
                        "Message",
                        new Vector2(0, -ImGuiNative.igGetItemsLineHeightWithSpacing()),
                        false,
                        WindowFlags.Default);
                    {
                        ImGui.Text("");

                        ImGui.TextWrapped(popupModal.message);
                    }
                    ImGui.EndChild();

                    var width = 75;
                    var height = ImGuiNative.igGetTextLineHeightWithSpacing();
                    var size = new Vector2(width, height);

                    switch (popupModal.popupModalButtons)
                    {
                        case PopupModalButtons.OK:
                            if (ImGui.Button("OK", size))
                                ClosePopupModal(popupModal, true);
                            break;

                        case PopupModalButtons.OK_CANCEL:
                            if (ImGui.Button("OK", size))
                                ClosePopupModal(popupModal, true);

                            ImGui.SameLine();
                            ImGuiNative.igSetCursorPosX(ImGui.GetWindowWidth() - size.X - 10);

                            if (ImGui.Button("Cancel", size))
                                ClosePopupModal(popupModal, false);
                            break;

                        case PopupModalButtons.YES_NO:
                            if (ImGui.Button("Yes", size))
                                ClosePopupModal(popupModal, true);

                            ImGui.SameLine();
                            ImGuiNative.igSetCursorPosX(ImGui.GetWindowWidth() - size.X - 10);

                            if (ImGui.Button("No", size))
                                ClosePopupModal(popupModal, false);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    ImGui.EndPopup();
                }
            }
        }

        private static void OnPostRender()
        {

        }

        private static void WrapCursor()
        {
            var cursorState = Mouse.GetCursorState();
            if (MyGameWindow.main.Bounds.Contains(cursorState.X, cursorState.Y))
                return;

            if (cursorState.X > MyGameWindow.main.Bounds.Right)
            {
                Mouse.SetPosition(MyGameWindow.main.Bounds.Left, cursorState.Y);

            }
            else if (cursorState.X < MyGameWindow.main.Bounds.Left)
                Mouse.SetPosition(MyGameWindow.main.Bounds.Right, cursorState.Y);

            if (cursorState.Y > MyGameWindow.main.Bounds.Bottom)
                Mouse.SetPosition(cursorState.X, MyGameWindow.main.Bounds.Top);
            else if (cursorState.Y < MyGameWindow.main.Bounds.Top)
                Mouse.SetPosition(cursorState.X, MyGameWindow.main.Bounds.Bottom);

            cursorState = Mouse.GetCursorState();
            var windowPoint = MyGameWindow.main.PointToClient(new Point(cursorState.X, cursorState.Y));

            var io = ImGui.GetIO();
            var scaledPoint =
                new Vector2(
                    windowPoint.X / io.DisplayFramebufferScale.X,
                    windowPoint.Y / io.DisplayFramebufferScale.Y);

            io.MousePosition = scaledPoint - ImGui.GetMouseDragDelta(0, -1);
            ImGui.ResetMouseDragDelta(0);
            io.MousePosition = scaledPoint;
        }
        private static void ClosePopupModal(PopupModal popupModal, bool result)
        {
            popupModal.buttonAction?.Invoke(result);
            ImGui.CloseCurrentPopup();

            s_PopupModals.Remove(popupModal);
        }

        public static bool CanCatch()
        {
            if (hoveredObject == draggedObject)
                return false;

            if ((hoveredObject is GameObject || hoveredObject is HierarchyWindow) &&
                (draggedObject is GameObject || draggedObject is Transform))
                return true;

            if (hoveredObject is Component && draggedObject is Component)
                return true;

            return false;
        }

        private static void Catch()
        {
            if (!CanCatch())
                return;

            var draggedTransform = draggedObject as Transform;
            var draggedGameObject = draggedObject as GameObject;

            var hierarchyWindow = hoveredObject as HierarchyWindow;
            if (hierarchyWindow != null && (draggedGameObject != null || draggedTransform != null))
            {
                var gameObject = draggedGameObject ?? draggedTransform.gameObject;
                Object.SetInList(gameObject, hierarchyWindow.placeAbove);
                gameObject.transform.SetParent(hierarchyWindow.placeAbove.transform.parent);
            }

            var hoveredTransform = hoveredObject as Transform;

            var hoveredGameObject = hoveredObject as GameObject;

            if ((hoveredGameObject != null || hoveredTransform != null) &&
                (draggedGameObject != null || draggedTransform != null))
            {
                var parent = hoveredTransform ?? hoveredGameObject.transform;
                var child = draggedTransform ?? draggedGameObject.transform;

                child.transform.SetParent(parent);
            }

            var hoveredComponent = hoveredObject as Component;
            var draggedComponent = draggedObject as Component;
            if (hoveredComponent != null && draggedComponent != null &&
                hoveredComponent.gameObject == draggedComponent.gameObject)
                hoveredComponent.gameObject.SetInList(draggedComponent, hoveredComponent);

        }
    }
}
