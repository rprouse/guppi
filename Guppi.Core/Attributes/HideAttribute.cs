using System;

namespace Guppi.Core.Attributes
{
    /// <summary>
    /// Causes a property to not be shown in the configuration process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HideAttribute : Attribute
    {
        public HideAttribute() { }
    }
}
