namespace BroEngine
{
    using System.Collections.Generic;
    using System.Linq;

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

        internal static int GetTagIndex(string tag) { return instanceTags.IndexOf(tag); }
        internal static int GetLayerIndex(string layer) { return instanceLayers.IndexOf(layer); }

        internal static void Select() { InspectorWindow.selectedObject = s_Instance; }

        internal static void RemoveTag(int index)
        {
            foreach (var gameObject in GameObject.FindGameObjectsWithTag(instanceTags[index]))
                gameObject.tagIndex = 0;

            var tags = instanceTags.ToList();
            tags.RemoveAt(index);
            foreach (var gameObject in FindObjectsOfType<GameObject>())
                gameObject.tagIndex = tags.IndexOf(gameObject.tag);
        }
        internal static void RemoveLayer(int index)
        {
            foreach (var gameObject in FindObjectsOfType<GameObject>())
            {
                if (gameObject.layerIndex == index)
                    gameObject.layerIndex = 0;
            }

            var layers = instanceLayers.ToList();
            layers.RemoveAt(index);
            foreach (var gameObject in FindObjectsOfType<GameObject>())
                gameObject.layerIndex = layers.IndexOf(gameObject.layer);
        }
    }
}
