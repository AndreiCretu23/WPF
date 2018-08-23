using Quantum.Metadata;
using System;
using System.Collections.Generic;

namespace Quantum.Command
{
    public interface ICommandMetadataProcessorService
    {
        void ProcessMetadata<TCommand, TCollection>(TCommand command, Func<TCommand, TCollection> getMetadataCollection)
            where TCommand : IUICommand
            where TCollection : IEnumerable<IMetadataDefinition>;
    }
}
