using System;

namespace Quantum.Command
{
    public class Description : IMainMenuMetadata
    {
        public bool SupportsMultiple { get { return false; } }
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
