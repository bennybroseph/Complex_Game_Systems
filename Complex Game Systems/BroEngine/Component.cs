namespace BroEngine
{
    public class Component : Object
    {
        private GameObject m_GameObject;

        public GameObject gameObject => m_GameObject;

        public string tag => gameObject.tag;
        public Transform transform => gameObject.transform;

        public Component(string name = "New Component") : base(name) { }

        public bool CompareTag(string otherTag) => gameObject.CompareTag(otherTag);

        public T GetComponent<T>() where T : Component => gameObject.GetComponent<T>();
    }
}
