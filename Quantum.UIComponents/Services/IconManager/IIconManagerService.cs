using System;

namespace Quantum.UIComponents
{
    /// <summary>
    /// This service manages the registration and extraction of the default icons for the application.
    /// </summary>
    public interface IIconManagerService
    {
        /// <summary>
        /// Gets or sets the absolute path of the default icon of the application. When trying to extract the icon associated with a type, if that type has no 
        /// icon registered, the default icon is going to be returned.
        /// </summary>
        string DefaultIcon { get; set; }

        /// <summary>
        /// Gets or sets the the URI path of the folder in which the icons are located.
        /// </summary>
        string IconRootFolder { get; set; }

        /// <summary>
        /// Maps the given type to the specified icon path. The icon path must be relative to the icon root folder.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iconPath"></param>
        void RegisterIconForType<T>(string iconPath);

        /// <summary>
        /// Maps the given type to the specified icon path. The icon path must be relative to the icon root folder.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="iconPath"></param>
        void RegisterIconForType(Type type, string iconPath);

        /// <summary>
        /// Returns the absolute URI path of the icon associated with the given type. If no icon is registered for the type, the default icon is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetIconForType<T>();

        /// <summary>
        /// Returns the absolute URI path of the icon associated with the given type. If no icon is registered for the type, the default icon is returned.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetIconForType(Type type);

        /// <summary>
        /// Returns the absolute URI path of an icon from the specified relative path.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        string GetIconPath(string relativePath);
    }
}
