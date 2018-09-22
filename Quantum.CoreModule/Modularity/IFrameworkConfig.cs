using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows;

namespace Quantum.CoreModule
{
    /// <summary>
    /// Contains information used by various components of the framework. The configuration properties' metadata can be
    /// overriden in the bootstrapper of the local application.
    /// </summary>
    public interface IFrameworkConfig
    {
        /// <summary>
        /// Returns the text displayed in the center of the loading animation that pops up when the UIThread is busy.<para></para>
        /// Default value is : "Loading..."
        /// </summary>
        string LongOpDescription { get; }

        /// <summary>
        /// Returns the title of the MainWindow of the application. <para></para>
        /// Default value is the application's name.
        /// </summary>
        string ShellTitle { get; }

        /// <summary>
        /// Returns the path to the ".png" resource used as the icon for the MainWindow of the application.<para></para>
        /// Default value is null.
        /// </summary>
        string ShellIcon { get; }

        /// <summary>
        /// Returns the initial width of the MainWindow of the application.<para></para>
        /// Default value is Double.Nan (auto). Warning, overriding the metadata for this property is not safe.
        /// </summary>
        [UnsafeSet]
        double ShellWidth { get; }

        /// <summary>
        /// Returns the initial height of the MainWindow of the application.<para></para>
        /// Default value is Double.Nan (auto). Warning, overriding the metadata for this property is not safe.
        /// </summary>
        [UnsafeSet]
        double ShellHeight { get; }

        /// <summary>
        /// Returns the minimum width of the MainWindow of the application.<para></para>
        /// Default value is 700.
        /// </summary>
        double ShellMinWidth { get; }

        /// <summary>
        /// Returns the minimum height of the MainWindow of the application.<para></para>
        /// Default value is 600.
        /// </summary>
        double ShellMinHeight { get; }

        /// <summary>
        /// Returns the maximum width of the MainWindow of the application.<para></para>
        /// Default value is Double.NaN (auto). Warning, overriding the metadata for this property is not safe.
        /// </summary>
        [UnsafeSet]
        double ShellMaxWidth { get; }

        /// <summary>
        /// Returns the maximum height of the MainWindow of the application.<para></para>
        /// Default value is Double.NaN (auto). Warning, overriding the metadata for this property is not safe.
        /// </summary>
        [UnsafeSet]
        double ShellMaxHeight { get; }

        /// <summary>
        /// Returns the resize mode the the MainWindow of the application.<para></para>
        /// Default value is CanResizeWithGrip.
        /// </summary>
        ResizeMode ShellResizeMode { get; }
        
        /// <summary>
        /// Returns the state of the MainWindow of the application.<para></para>
        /// Default value is Maximized. Warning, overring the metadata for this property is not safe.
        /// </summary>
        [UnsafeSet]
        WindowState ShellState { get; }

        /// <summary>
        /// Returns the StartUp Location of the MainWindow of the application.<para></para>
        /// Default value is CenterScreen.
        /// </summary>
        WindowStartupLocation ShellStartUpLocation { get; }
        
        /// <summary>
        /// Overrides the metadata for a specified framework configuration property.
        /// </summary>
        /// <typeparam name="TMetadata">The type of the property</typeparam>
        /// <param name="property">A lambda expression indicating the configuration property.</param>
        /// <param name="value">A getter returning the value of the property.</param>
        /// <param name="invalidators">A collection of event types on which the configuration property must be re-evaluated</param>
        void OverrideMetadata<TMetadata>(Expression<Func<IFrameworkConfig, TMetadata>> property, Func<TMetadata> value, IEnumerable<Type> invalidators = null);

        /// <summary>
        /// Returns a collection of event types associated with a configuration property on which the value of the property must be invalidated. 
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="property">A lambda expression indicating the configuration property.</param>
        /// <returns></returns>
        IEnumerable<Type> GetPropertyInvalidators<T>(Expression<Func<IFrameworkConfig, T>> property);
    }
}
