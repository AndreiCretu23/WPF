using System;

namespace Quantum.Metadata
{
    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class Description : IMenuMetadata, ISubMenuMetadata
    {
        public string Value { get; private set; }
        public Description(string description)
        {
            Value = description;
        }
        public Description(Func<string> descriptionGetter)
        {
            Value = descriptionGetter();
        }

    }
}
