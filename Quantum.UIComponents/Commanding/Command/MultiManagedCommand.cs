using Quantum.Metadata;
using System;

namespace Quantum.Command
{
    public class MultiManagedCommand : IMultiManagedCommand
    {
        private Func<SubCommandCollection> subCommands;
        public Func<SubCommandCollection> SubCommands
        {
            get => subCommands ?? (() => new SubCommandCollection());
            set => subCommands = value;
        }

        public MultiMenuMetadataCollection MenuMetadata { get; set; } = new MultiMenuMetadataCollection();
    }
}
