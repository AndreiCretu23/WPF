using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows;

namespace Quantum.CoreModule
{
    public interface IFrameworkConfig
    {
        //LongOp
        string LongOpDescription { get; }
        
        //Shell 
        string ShellTitle { get; }
        string ShellIcon { get; }

        double ShellWidth { get; }
        double ShellHeight { get; }

        double ShellMinWidth { get; }
        double ShellMinHeight { get; }

        double ShellMaxWidth { get; }
        double ShellMaxHeight { get; }

        ResizeMode ShellResizeMode { get; }
        WindowState ShellState { get; }
        WindowStartupLocation ShellStartUpLocation { get; }
        
        void OverrideMetadata<TMetadata>(Expression<Func<IFrameworkConfig, TMetadata>> property, Func<TMetadata> value, IEnumerable<Type> invalidators = null);
        IEnumerable<Type> GetPropertyInvalidators<T>(Expression<Func<IFrameworkConfig, T>> property);
    }
}
