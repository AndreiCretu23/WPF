using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.CoreModule;
using Quantum.ResourceLibrary;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;

namespace Quantum.Core
{
    internal class FrameworkConfig : IFrameworkConfig
    {
        public string LongOpDescription => GetValue(() => LongOpDescription);


        public string ShellTitle => GetValue(() => ShellTitle);
        public string ShellIcon => GetValue(() => ShellIcon);

        public double ShellWidth => GetValue(() => ShellWidth);
        public double ShellHeight => GetValue(() => ShellHeight);

        public double ShellMinWidth => GetValue(() => ShellMinWidth);
        public double ShellMinHeight => GetValue(() => ShellMinHeight);

        public double ShellMaxWidth => GetValue(() => ShellMaxWidth);
        public double ShellMaxHeight => GetValue(() => ShellMaxHeight);

        public ResizeMode ShellResizeMode => GetValue(() => ShellResizeMode);
        public WindowState ShellState => GetValue(() => ShellState);
        public WindowStartupLocation ShellStartUpLocation => GetValue(() => ShellStartUpLocation);

        public FrameworkConfig()
        {
            SetDefaultMetadata();
        }
        
        private Dictionary<string, ConfigPropertyDefinition> ConfigDefinitions = new Dictionary<string, ConfigPropertyDefinition>();
        private ScopedValue<bool> DefaultMetadataOverride = new ScopedValue<bool>();

        private T GetValue<T>(Expression<Func<T>> property)
        {
            var prop = ReflectionUtils.GetPropertyName(property);
            return ((Func<T>)ConfigDefinitions[prop].Value)();
        }

        public void OverrideMetadata<TMetadata>(Expression<Func<IFrameworkConfig, TMetadata>> property, Func<TMetadata> value, IEnumerable<Type> invalidators = null)
        {
            var prop = ReflectionUtils.GetPropertyInfo(property);
            AssertInvalidators(prop.Name, invalidators);
            var configDefinition = new ConfigPropertyDefinition(value, invalidators ?? Enumerable.Empty<Type>());
            if(!ConfigDefinitions.ContainsKey(prop.Name))
            {
                ConfigDefinitions.Add(prop.Name, configDefinition);
            }
            else
            {
                ConfigDefinitions[prop.Name] = configDefinition;
            }

            if(!DefaultMetadataOverride.Value && prop.HasAttribute<UnsafeSetAttribute>())
            {
                Console.WriteLine($"WARNING : Overriding unsafe config property {prop.Name}. This might cause side effects.");
            }
        }

        private void AssertInvalidators(string propertyName, IEnumerable<Type> invalidators)
        {
            if (invalidators == null) return;
            if (invalidators.Any(o => o == null))
            {
                throw new Exception($"Error overriding metadata for the framework config property { propertyName} : \n" +
                                    $"The invalidator collection contains null types which is not allowed.");
            }

            foreach(var evt in invalidators)
            {
                if(!(evt.IsSubclassOfRawGeneric(typeof(SelectionBase<>)) || evt.IsSubclassOfRawGeneric(typeof(CompositePresentationEvent<>))))
                {
                    throw new Exception($"Error overriding metadata for the framework config property {propertyName} : \n" +
                                        $"The invalidation collection contains the type {evt.Name} which is not a valid eventType. \n" +
                                        $"Supported event types are types that extend {typeof(CompositePresentationEvent<>).Name} or {typeof(SelectionBase<>).Name}");
                }
            }
        }

        public IEnumerable<Type> GetPropertyInvalidators<T>(Expression<Func<IFrameworkConfig, T>> property)
        {
            return ConfigDefinitions[ReflectionUtils.GetPropertyName(property)].Invalidators;
        }

        private void SetDefaultMetadata()
        {
            using (DefaultMetadataOverride.BeginValueScope(true))
            {
                OverrideMetadata(c => c.LongOpDescription, () => Resources.LongOperation_DefaultDescription);
                OverrideMetadata(c => c.ShellTitle, () => AppInfo.ApplicationName);
                OverrideMetadata(c => c.ShellIcon, () => null);
                OverrideMetadata(c => c.ShellWidth, () => Double.NaN);
                OverrideMetadata(c => c.ShellHeight, () => Double.NaN);
                OverrideMetadata(c => c.ShellMinWidth, () => Double.NaN);
                OverrideMetadata(c => c.ShellMinHeight, () => Double.NaN);
                OverrideMetadata(c => c.ShellMaxWidth, () => Double.NaN);
                OverrideMetadata(c => c.ShellMaxHeight, () => Double.NaN);
                OverrideMetadata(c => c.ShellResizeMode, () => ResizeMode.CanResizeWithGrip);
                OverrideMetadata(c => c.ShellState, () => WindowState.Maximized);
                OverrideMetadata(c => c.ShellStartUpLocation, () => WindowStartupLocation.CenterScreen);
            }
        }

        private class ConfigPropertyDefinition
        {
            public Delegate Value { get; }
            public IEnumerable<Type> Invalidators { get; }

            public ConfigPropertyDefinition(Delegate value, IEnumerable<Type> invalidators)
            {
                Value = value;
                Invalidators = invalidators;
            }
        }
    }
    
    

}
