using System;

namespace Quantum.Metadata
{
    /// <summary>
    /// This attribute must be set on every single MetadataDefinition Type. It defines whether the metadata type must be defined within a 
    /// MetadataCollection that supports it or not.
    /// </summary>
    [AttributeUsage(validOn:AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MandatoryAttribute : Attribute
    {
        public bool IsMandatory { get; private set; }
        
        public MandatoryAttribute(bool isMandatory)
        {
            IsMandatory = isMandatory;
        }
    }

    /// <summary>
    /// This attribute must be set on every single MetadataDefinition Type. It defines whether the metadata type supports multiple instances
    /// within a MetadataCollection or not.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SupportsMultipleAttribute : Attribute
    {
        public bool SupportsMultiple { get; private set; }

        public SupportsMultipleAttribute(bool supportsMultiple)
        {
            SupportsMultiple = supportsMultiple;
        }
    }

    /// <summary>
    /// This attribute must be set on every MetadataCollection Type. It defines if whether within an object containing metadata collections, 
    /// the particular collection type MUST be defined or not (let empty).
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MandatoryCollectionAttribute : Attribute
    {
        public bool IsMandatoryCollection { get; private set; }

        public MandatoryCollectionAttribute(bool isMandatoryCollection)
        {
            IsMandatoryCollection = isMandatoryCollection;
        }
    }
}
