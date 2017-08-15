﻿namespace BroEngine
{
    using System.Collections.Generic;
    using System.Linq;

    public class Object
    {
        private static List<Object> s_Objects = new List<Object>();
        private static List<bool> s_IDs = new List<bool> { true };

        private string m_Name;
        private int m_ID;

        public string name { get => m_Name; set => m_Name = value; }

        public Object(string name = "New Object")
        {
            s_Objects.Add(this);

            m_Name = name;

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
            return s_Objects.FirstOrDefault(o => o.GetType() == typeof(T)) as T;
        }
        public static IEnumerable<T> FindObjectsOfType<T>() where T : Object
        {
            return s_Objects.Where(o => o.GetType() == typeof(T)).Cast<T>();
        }

        public static void Destroy(Object o)
        {
            if (o.GetType() == typeof(Transform))
                return;

            s_IDs[o.m_ID] = true;
            s_Objects.Remove(o);
        }

        public override string ToString()
        {
            return name;
        }
    }
}

