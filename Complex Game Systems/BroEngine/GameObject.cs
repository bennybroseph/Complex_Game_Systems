namespace BroEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Geometry;
    using Geometry.Shapes;

    using JetBrains.Annotations;

    public enum PrimitiveType
    {
        Sphere,
        Capsule,
        Cylinder,
        Cube,
        Plane,
        Quad,
    }

    public class GameObject : Object
    {
        private readonly List<Component> m_Components = new List<Component>();

        internal int tagIndex { get; set; }
        public int layerIndex { get; set; }

        public Transform transform { get; }

        public bool isActive { get; set; } = true;
        public bool isStatic { get; set; }

        public string tag
        {
            get => TagLayerManager.instanceTags[tagIndex];
            set => tagIndex = TagLayerManager.GetTagIndex(value);
        }
        internal string layer
        {
            get => TagLayerManager.instanceLayers[layerIndex];
            set => layerIndex = TagLayerManager.GetLayerIndex(value);
        }

        public GameObject(string name = "New GameObject") : base(name)
        {
            transform = AddComponent<Transform>();
            if (transform == null)
            {
                Console.WriteLine("A transform could not be added to " + name + "?...");
                return;
            }

            transform.gameObject = this;
        }

        public GameObject(string name, params Type[] types) : this(name)
        {
            foreach (var type in types)
                AddComponent(type);
        }

        public static GameObject CreatePrimitive(PrimitiveType primitiveType)
        {
            var newGameObject = new GameObject(primitiveType.ToString());
            var newMeshFilter = newGameObject.AddComponent<MeshFilter<Vertex>>();
            var newMeshRenderer = newGameObject.AddComponent<MeshRenderer<Vertex>>();

            newMeshRenderer.shader = ShaderProgram.basic;

            switch (primitiveType)
            {
                case PrimitiveType.Sphere:
                    newMeshFilter.mesh = Sphere.GetMesh();
                    break;
                case PrimitiveType.Capsule:
                    break;
                case PrimitiveType.Cylinder:
                    break;
                case PrimitiveType.Cube:
                    break;
                case PrimitiveType.Plane:
                    newMeshFilter.mesh = Plane.GetMesh();
                    break;
                case PrimitiveType.Quad:
                    newMeshFilter.mesh = Quad.GetMesh();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(primitiveType), primitiveType, null);
            }

            newMeshRenderer.Bind();
            newMeshRenderer.BufferData();
            newMeshRenderer.UnBind();

            return newGameObject;
        }

        [Pure, CanBeNull]
        public static GameObject Find(string otherName)
        {
            return
                FindObjectsOfType<GameObject>().FirstOrDefault(
                    gameObject => gameObject.name == otherName);
        }

        [Pure, CanBeNull]
        public static GameObject FindGameObjectWithTag(string otherTag)
        {
            return
                FindObjectsOfType<GameObject>().FirstOrDefault(
                    gameObject => gameObject.CompareTag(otherTag));
        }
        [Pure]
        public static GameObject[] FindGameObjectsWithTag(string otherTag)
        {
            return
                FindObjectsOfType<GameObject>().Where(
                    gameObject => gameObject.CompareTag(otherTag)).ToArray();
        }

        [CanBeNull]
        public T AddComponent<T>() where T : Component, new()
        {
            return AddComponent(typeof(T)) as T;
        }
        [CanBeNull]
        public Component AddComponent(Type type)
        {
            if (!typeof(Component).IsAssignableFrom(type))
                return null;

            var requireComponentAttribute = type.GetCustomAttribute<RequireComponentAttribute>();
            if (requireComponentAttribute != null)
            {
                foreach (var requiredComponentType in requireComponentAttribute.types)
                {
                    if (!typeof(Component).IsAssignableFrom(requiredComponentType))
                        continue;

                    // If there is already a component on the object
                    if (GetComponent(requiredComponentType) != null)
                        continue;

                    // Recursive call to attempt to add the new component
                    AddComponent(requiredComponentType);
                }
            }

            var disallowMultiple = type.GetCustomAttribute<DisallowMultipleComponentAttribute>();
            if (disallowMultiple != null && GetComponent(type) != null)
            {
                Console.WriteLine(
                    "The component of type " + type.Name + " does not allow multiple instances");

                return null;
            }

            var newComponent = Activator.CreateInstance(type) as Component;
            if (newComponent == null)
                return null;

            newComponent.gameObject = this;
            m_Components.Add(newComponent);

            return newComponent;
        }

        [MustUseReturnValue]
        public bool CompareTag(string otherTag)
        {
            return tagIndex == TagLayerManager.GetTagIndex(otherTag);
        }

        [Pure, CanBeNull]
        public T GetComponent<T>() where T : Component
        {
            return m_Components.FirstOrDefault(component => component is T) as T;
        }
        [Pure, CanBeNull]
        public Component GetComponent(Type type)
        {
            return m_Components.FirstOrDefault(type.IsInstanceOfType);
        }

        [Pure]
        public T[] GetComponents<T>() where T : Component
        {
            return m_Components.Where(component => component is T).Cast<T>().ToArray();
        }
        [Pure]
        public Component[] GetComponents(Type type)
        {
            return m_Components.Where(type.IsInstanceOfType).ToArray();
        }

        internal void RemoveComponent(Component component)
        {
            component.gameObject = null;
            m_Components.Remove(component);
        }

        internal void MoveComponent()
        {

        }

        internal bool IsComponentRequired(Component requiredComponent, out Type dependentType)
        {
            var componentType = requiredComponent.GetType();
            foreach (var component in m_Components)
            {
                var requireComponentAttribute =
                    component.GetType().GetCustomAttribute<RequireComponentAttribute>();
                if (requireComponentAttribute == null)
                    continue;

                if (requireComponentAttribute.types.All(type => type != componentType))
                    continue;

                dependentType = component.GetType();
                return true;
            }
            dependentType = null;
            return false;
        }
    }
}
