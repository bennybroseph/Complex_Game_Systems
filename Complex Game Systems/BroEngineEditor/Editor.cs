﻿namespace BroEngineEditor
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;

    using BroEngine;

    using ImGuiNET;

    using ImGuiUtility;

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

        public static Object s_DraggedObject;

        public static Object selectedObject { get; set; }
        private static List<InspectorWindow> s_InspectorWindows = new List<InspectorWindow>();

        private static List<HierarchyWindow> s_HierarchyWindows = new List<HierarchyWindow>();

        public static void Init()
        {
            ImGuiOpenTK.preRender += OnPreRender;
            ImGuiOpenTK.drawGui += OnDrawGui;
            ImGuiOpenTK.postRender += OnPostRender;

            s_InspectorWindows.Add(new InspectorWindow());
            s_HierarchyWindows.Add(new HierarchyWindow());
        }

        public static void ShowMessageBox(
            string title,
            string message,
            PopupModalButtons popupModalButtons,
            ButtonAction buttonAction = null)
        {
            if (buttonAction == null)
                buttonAction = result => { };

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
                            throw new System.ArgumentOutOfRangeException();
                    }

                    ImGui.EndPopup();
                }
            }
        }

        private static void OnPostRender()
        {

        }

        private static void ClosePopupModal(PopupModal popupModal, bool result)
        {
            popupModal.buttonAction(result);
            ImGui.CloseCurrentPopup();

            s_PopupModals.Remove(popupModal);
        }
    }
}
