namespace BroEngine
{
    using System.Collections.Generic;
    using System.Linq;

    public class Object
    {
        private static List<Object> s_Objects = new List<Object>();
        private static List<bool> s_IDs = new List<bool> { true };

        protected int m_ID;

        public virtual string name { get; set; }
        public int id => m_ID;

        public Object(string name = "New Object")
        {
            s_Objects.Add(this);

            this.name = name;

            var newID = -1;
            for (var i = 0; i < s_IDs.Count; i++)
            {
                if (!s_IDs[i])
                    continue;

                newID = i;
                break;
            }

            if (newID != -1)
            {
                m_ID = newID;
                s_IDs[m_ID] = false;
            }
            else
            {
                m_ID = s_IDs.Count;
                s_IDs.Add(false);
            }
        }

        public static T FindObjectOfType<T>() where T : Object
        {
            return s_Objects.FirstOrDefault(o => o is T) as T;
        }
        public static T[] FindObjectsOfType<T>() where T : Object
        {
            return s_Objects.Where(o => o is T).Cast<T>().ToArray();
        }

        public static void Destroy(Object o)
        {
            if (o.GetType() == typeof(Transform))
                return;

            if (o is Component component)
            {
                if (component.gameObject.IsComponentRequired(component))
                    return;

                component.gameObject.RemoveComponent(component);
            }

            s_IDs[o.m_ID] = true;
            s_Objects.Remove(o);
        }

        public override string ToString() { return name; }
    }
}

