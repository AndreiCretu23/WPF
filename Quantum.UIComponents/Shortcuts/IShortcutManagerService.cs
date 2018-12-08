﻿using Quantum.Command;
using Quantum.Metadata;
using Quantum.UIComponents;
using System.Windows.Input;

namespace Quantum.Shortcuts
{
    /// <summary>
    /// This service is responsible for managing, changing and serializing all shortcuts in the application.
    /// </summary>
    public interface IShortcutManagerService
    {
        /// <summary>
        /// Returns the shortcut associated with the specified command. If the command doesn't have an associated shortcut, an exception will be thrown.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        KeyShortcut GetShortcut(IManagedCommand command);

        /// <summary>
        /// Returns the BringIntoView shortcut associated with the specified static panel definition. If the definition doesn't have an associated 
        /// BringIntoView shortcut, an exception will be thrown.
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        BringIntoViewOnKeyShortcut GetShortcut(IStaticPanelDefinition definition);

        /// <summary>
        /// Returns a value indicating if the specified command has an associated shortcut.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        bool HasShortcut(IManagedCommand command);

        /// <summary>
        /// Returns a value indicating if the specified static panel definition has an associated BringIntoView shortcut.
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        bool HasShortcut(IStaticPanelDefinition definition);

        /// <summary>
        /// Removes any shortcuts associated with the specified command.
        /// </summary>
        /// <param name="command"></param>
        void ClearShortcut(IManagedCommand command);

        /// <summary>
        /// Removes any BringIntoView shortcuts associated with the specified static panel definition.
        /// </summary>
        /// <param name="definition"></param>
        void ClearShortcut(IStaticPanelDefinition definition);

        /// <summary>
        /// Sets or changes the shortcut of the specified managed command. The command must be registered in the CommandManager.
        /// </summary>
        /// <param name="command"></param>
        void SetShortcut(IManagedCommand command, ModifierKeys modifierKeys, Key key);

        /// <summary>
        /// Sets or changes the "BringIntoView" shortcut associated with the given static panel definition. The static panel definition must be registered in the PanelManager.
        /// </summary>
        /// <param name="definition"></param>
        void SetShortcut(IStaticPanelDefinition definition, ModifierKeys modifierKeys, Key key);
    }
}
