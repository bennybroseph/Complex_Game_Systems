﻿namespace BroEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class RequireComponentAttribute : Attribute
    {
        public Type[] types { get; }

        public RequireComponentAttribute(params Type[] types) { this.types = types; }
    }
}
