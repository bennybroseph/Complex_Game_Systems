using System.Collections.Generic;

namespace BroEngine
{
    public abstract class Component : Object
    {
        public GameObject gameObject { get; set; }

        public string tag => gameObject.tag;
        public Transform transform => gameObject.transform;

        public override string name => GetType().Name;

        public bool CompareTag(string otherTag) => gameObject.CompareTag(otherTag);

        public T GetComponent<T>() where T : Component => gameObject.GetComponent<T>();
        public IEnumerable<T> GetComponents<T>() where T : Component => gameObject.GetComponents<T>();

        internal bool CanMoveUp() => gameObject.CanMoveUp(this);
        internal void MoveUp() => gameObject.MoveUp(this);

        internal bool CanMoveDown() => gameObject.CanMoveDown(this);
        internal void MoveDown() => gameObject.MoveDown(this);
    }
}
