using Quantum.Events;
using Quantum.Utils;
using System;

namespace Quantum.UIComponents
{
    internal class DefaultDockingConfiguration : IDockingConfiguration
    {
        public bool SerializesLayout => true;

        public string LayoutSerializationDirectory => AppInfo.ApplicationConfigRepository;
        public string LayoutSerializationFileName => "DockingLayout";

        public Type LayoutSerializationEvent => typeof(ShutdownEvent);
        public Type LayoutDeserializationEvent => typeof(PanelsLoadedEvent);
    }
}
