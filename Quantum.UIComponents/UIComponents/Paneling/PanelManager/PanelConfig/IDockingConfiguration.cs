using System;

namespace Quantum.UIComponents
{
    public interface IDockingConfiguration
    {
        /// <summary>
        /// Returns a value indicating whether the PanelManager should serialize the layout or not.
        /// </summary>
        bool SerializesLayout { get; }

        /// <summary>
        /// Returns the path of the directory in which the layout should be serialized.
        /// </summary>
        string LayoutSerializationDirectory { get; }

        /// <summary>
        /// Returns the name of the XML file in which the layout should be serialized (Just the name, no paths / file extensions -> That is handled automatically).
        /// </summary>
        string LayoutSerializationFileName { get; }

        /// <summary>
        /// Returns the type of the event on which the layout of the panels should be serialized.
        /// </summary>
        Type LayoutSerializationEvent { get; }

        /// <summary>
        /// Returns the type of the event on which the layout of the panels should be deserialized.
        /// </summary>
        Type LayoutDeserializationEvent { get; }
    }
}
