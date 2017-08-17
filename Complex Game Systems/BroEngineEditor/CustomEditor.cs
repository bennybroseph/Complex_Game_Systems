using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroEngineEditor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomEditorAttribute : Attribute
    {
        public Type type { get; }
        public CustomEditorAttribute(Type type)
        {
            this.type = type;
        }
    }
}
