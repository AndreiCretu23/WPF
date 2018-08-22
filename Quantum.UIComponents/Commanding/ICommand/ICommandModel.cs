using Quantum.Metadata;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Quantum.Command
{
    public interface IUICommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
    
    public interface IStaticCommand : IUICommand
    {
    }

    public interface IStaticMetadataOwnerCommand<TMetadataCollection, TMetadataDefinition> : IStaticCommand
        where TMetadataCollection : MetadataCollection<TMetadataDefinition>
        where TMetadataDefinition : IMetadataDefinition
    {
        TMetadataCollection Metadata { get; }
    }
    
    public interface IDependencyCommand : IUICommand
    {
        Type DependencyType { get; }
    }
    
    public interface IDependencyMetadataOwnerCommand<TMetadataCollection, TMetadataDefinition> : IDependencyCommand
        where TMetadataCollection : MetadataCollection<TMetadataDefinition>
        where TMetadataDefinition : IMetadataDefinition
    {
        TMetadataCollection Metadata { get; }
    }
    
    public interface IMultiCommand
    {
    }

    public interface IMultiCommand<TCommand> : IMultiCommand
    {
    }

    public interface IMultiStaticCommand<TCommand> : IMultiCommand<TCommand>
        where TCommand : IStaticCommand
    {
        event Action<IEnumerable<TCommand>> OnCommandsComputed;
        IEnumerable<TCommand> ComputeCommands();
    }

    public interface IMultiStaticMetadataOwnerCommand<TCommand, TMetadataCollection, TMetadataDefinition> : IMultiStaticCommand<TCommand>
        where TCommand : IStaticCommand
        where TMetadataCollection : MetadataCollection<TMetadataDefinition>, new()
        where TMetadataDefinition : IMetadataDefinition
    {
        TMetadataCollection Metadata { get; }
    }

    public interface IMultiDependencyCommand<TCommand> : IMultiCommand<TCommand>
        where TCommand : IDependencyCommand
    {
        event Action<IEnumerable<TCommand>> OnCommandsComputed;
        IEnumerable<TCommand> ComputeCommands(object o);
        Type DependencyType { get; }
    }
    
    public interface IMultiDependencyMetadataOwnerCommand<TCommand, TMetadataCollection, TMetadataDefinition> : IMultiDependencyCommand<TCommand>
        where TCommand : IDependencyCommand
        where TMetadataCollection : MetadataCollection<TMetadataDefinition>, new()
        where TMetadataDefinition : IMetadataDefinition
    {
        TMetadataCollection Metadata { get; }
    }
}
