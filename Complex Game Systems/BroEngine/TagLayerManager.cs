namespace BroEngine
{
    using System.Collections.Generic;

    using BroEngineEditor;

    internal class TagLayerManager : Object
    {
        private static TagLayerManager s_Instance = new TagLayerManager();

        internal static string displayName => s_Instance.name;

        internal static List<string> instanceTags => s_Instance.tags;
        internal static List<string> instanceLayers => s_Instance.layers;

        public override string name => "Tags & Layers";

        internal readonly List<string> tags =
            new List<string>
            {
                "Untagged",

                "Main Camera",
                "Player",
                "Editor Only",
                "Re-spawn",
                "Finish",
                "Game Controller",
            };

        internal readonly List<string> layers =
            new List<string>
            {
                "Default",

                "TransparentFX",
                "Ignore Raycast",
                "Water",
            };

        internal static void Init()
        {
            if (s_Instance == null)
                s_Instance = new TagLayerManager();
        }

        internal static int GetTagIndex(string tag) { return s_Instance.tags.IndexOf(tag); }
        internal static int GetLayerIndex(string layer) { return s_Instance.layers.IndexOf(layer); }

        internal static void Select() { Inspector.selectedObject = s_Instance; }
    }
}
