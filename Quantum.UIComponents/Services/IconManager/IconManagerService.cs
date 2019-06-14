using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;

namespace Quantum.UIComponents
{
    internal class IconManagerService : ServiceBase, IIconManagerService
    {
        public string DefaultIcon { get; set; } = "pack://application:,,,/Quantum.ResourceLibrary;component/Icons/Common/appbar.arcade.button.png";
        public string IconRootFolder { get; set; } = "pack://application:,,,/Quantum.ResourceLibrary;component/Icons";
        private Dictionary<Type, string> IconLibrary { get; } = new Dictionary<Type, string>();
        
        public IconManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
        
        public void RegisterIconForType<T>(string iconPath)
        {
            RegisterIconForType(typeof(T), iconPath);
        }

        public void RegisterIconForType(Type type, string iconPath)
        {
            type.AssertParameterNotNull(nameof(type));
            iconPath.AssertParameterNotNull(nameof(iconPath));
         
            if(IconLibrary.ContainsKey(type)) {
                throw new Exception($"Error : IconManager : An icon has already been registered for type {type.Name}");
            }
            
            IconLibrary.Add(type, iconPath);
        }

        public string GetIconForType<T>()
        {
            return GetIconForType(typeof(T));
        }

        public string GetIconForType(Type type)
        {
            type.AssertParameterNotNull(nameof(type));
            
            if(IconLibrary.ContainsKey(type)) {
                return GetIconPath(IconLibrary[type]);
            }
            
            return DefaultIcon;
        }

        public string GetIconPath(string relativePath)
        {
            relativePath.AssertParameterNotNull(nameof(relativePath));
            return IconRootFolder + relativePath;
        }
    }
}
