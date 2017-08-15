namespace BroEngine
{
    using System.Collections.Generic;
    using System.Linq;

    public class GameObject : Object
    {
        private static List<GameObject> s_GameObjects = new List<GameObject>();

        private List<Component> m_Components = new List<Component>();
        private Transform m_Transform;

        private bool m_IsActive = true;
        private bool m_IsStatic;

        private string m_Tag = "UnTagged";

        public Transform transform => m_Transform;

        public bool isActive { get => m_IsActive; set => m_IsActive = value; }
        public bool isStatic { get => m_IsStatic; set => m_IsStatic = value; }

        public string tag { get => m_Tag; set => m_Tag = value; }

        public GameObject(string name = "New GameObject") : base(name)
        {
            m_Transform = AddComponent<Transform>();

            s_GameObjects.Add(this);
        }

        public GameObject(string name, params Component[] components) : this(name)
        {
            m_Components.AddRange(components);
        }

        public static GameObject Find(string otherName)
        {
            return s_GameObjects.FirstOrDefault(gameObject => gameObject.name == otherName);
        }

        public static GameObject FindGameObjectWithTag(string otherTag)
        {
            return s_GameObjects.FirstOrDefault(gameObject => gameObject.tag == otherTag);
        }
        public static IEnumerable<GameObject> FindGameObjectsWithTag(string otherTag)
        {
            return s_GameObjects.Where(gameObject => gameObject.tag == otherTag);
        }

        public T AddComponent<T>() where T : Component, new()
        {
            var newComponent = new T();
            m_Components.Add(newComponent);

            return newComponent;
        }

        public bool CompareTag(string otherTag)
        {
            return m_Tag == otherTag;
        }

        public T GetComponent<T>() where T : Component
        {
            return m_Components.FirstOrDefault(component => component.GetType() == typeof(T)) as T;
        }
        public IEnumerable<T> GetComponents<T>() where T : Component
        {
            return m_Components.Where(component => component.GetType() == typeof(T)).Cast<T>();
        }
    }
}
