using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroEngineEditor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomInspectorAttribute : Attribute
    {
        public Type type { get; }

        public CustomInspectorAttribute(Type type) { this.type = type; }
    }
}
