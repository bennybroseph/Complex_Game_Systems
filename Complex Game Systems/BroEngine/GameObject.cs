namespace BroEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using FMOD;

    public class GameObject : Object
    {
        private List<Component> m_Components = new List<Component>();

        public Transform transform { get; }

        public bool isActive { get; set; } = true;
        public bool isStatic { get; set; }

        public string tag { get; set; } = "UnTagged";

        public GameObject(string name = "New GameObject") : base(name)
        {
            transform = AddComponent<Transform>();
            transform.gameObject = this;
        }

        public GameObject(string name, params Type[] types) : this(name)
        {
            foreach (var type in types)
                AddComponent(type);
        }

        public static GameObject Find(string otherName)
        {
            return FindObjectsOfType<GameObject>().FirstOrDefault(gameObject => gameObject.name == otherName);
        }

        public static GameObject FindGameObjectWithTag(string otherTag)
        {
            return FindObjectsOfType<GameObject>().FirstOrDefault(gameObject => gameObject.tag == otherTag);
        }

        public static IEnumerable<GameObject> FindGameObjectsWithTag(string otherTag)
        {
            return FindObjectsOfType<GameObject>().Where(gameObject => gameObject.tag == otherTag);
        }

        public T AddComponent<T>() where T : Component, new()
        {
            var newComponent = new T();
            return AddComponent(newComponent);
        }
        public Component AddComponent(Type type)
        {
            if (!typeof(Component).IsAssignableFrom(type))
                return null;

            var newComponent = Activator.CreateInstance(type) as Component;
            return AddComponent(newComponent);
        }

        public bool CompareTag(string otherTag)
        {
            return tag == otherTag;
        }

        public T GetComponent<T>() where T : Component
        {
            return m_Components.FirstOrDefault(component => component is T) as T;
        }
        public Component GetComponent(Type type)
        {
            return m_Components.FirstOrDefault(type.IsInstanceOfType);
        }

        public IEnumerable<T> GetComponents<T>() where T : Component
        {
            return m_Components.Where(component => component is T).Cast<T>();
        }
        public IEnumerable<Component> GetComponents(Type type)
        {
            return m_Components.Where(type.IsInstanceOfType);
        }

        internal void RemoveComponent(Component component)
        {
            component.gameObject = null;
            m_Components.Remove(component);
        }

        private T AddComponent<T>(T component) where T : Component
        {
            var requireComponent = typeof(T).GetCustomAttribute<RequireComponentAttribute>();
            if (requireComponent != null)
            {
                foreach (var type in requireComponent.types)
                {
                    if (!typeof(Component).IsAssignableFrom(type))
                        continue;

                    // If there is already a component on the object
                    if (GetComponent(type) != null)
                        continue;

                    var newComponent = Activator.CreateInstance(type) as Component;
                    if (newComponent == null)
                        continue;

                    AddComponent(newComponent);
                }
            }

            var disallowMultiple = typeof(T).GetCustomAttribute<DisallowMultipleComponentAttribute>();
            if (disallowMultiple != null && GetComponent<T>() != null)
            {
                Console.WriteLine(
                    "The component of type " + typeof(T).Name + " does not allow multiple instances");
                return null;
            }

            component.gameObject = this;
            m_Components.Add(component);

            return component;
        }
    }
}
