using System;

namespace MyDay.Core.Attributes
{
    /// <summary>
    /// Sets an alternate display name for a property in the configuration process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DisplayAttribute : Attribute
    {
        public string Description { get; set; }

        public DisplayAttribute(string description)
        {
            Description = description;
        }
    }
}
