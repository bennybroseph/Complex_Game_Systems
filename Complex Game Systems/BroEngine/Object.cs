namespace BroEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BroEngineEditor;

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
            {
                Editor.ShowMessageBox(
                    "Can't Remove a Transform Component",
                    "You cannot remove a transform component from a GameObject",
                    PopupModalButtons.OK);

                return;
            }

            if (o is Component component)
            {
                if (component.gameObject.IsComponentRequired(component, out Type dependentType))
                {
                    Editor.ShowMessageBox(
                        "Can't Remove Component",
                        "Can't remove " + component.GetType().Name + " because " + dependentType.Name + " " +
                        "depends on it",
                        PopupModalButtons.OK);
                    return;
                }

                component.gameObject.RemoveComponent(component);
            }

            s_IDs[o.m_ID] = true;
            s_Objects.Remove(o);
        }

        public override string ToString() { return name; }
    }
}

