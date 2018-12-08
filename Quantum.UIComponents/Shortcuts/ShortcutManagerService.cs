using Quantum.Command;
using Quantum.Events;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.UIComponents;
using Quantum.Utils;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Quantum.Shortcuts
{
    internal class ShortcutManagerService : ServiceBase, IShortcutManagerService
    {
        [Service]
        public ICommandManagerService CommandManager { get; set; }

        [Service]
        public IPanelManagerService PanelManager { get; set; }

        private ThreadSyncScope LoadingScope { get; }

        public ShortcutManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            LoadingScope = new ThreadSyncScope();
            LoadingScope.OnAllScopesEnd += (sender, e) => EventAggregator.GetEvent<ShortcutChangedEvent>().Publish(new GlobalRebuildShortcutChangedArgs());
        }

        
        #region HasShortcut

        public bool HasShortcut(IManagedCommand command)
        {
            command.AssertParameterNotNull(nameof(command));
            if (!CommandManager.IsRegistered(command)) {
                throw new Exception("Error : Requesting shortcut information of an unknown command : The command is not registered in the command manager.");
            }

            return command.Metadata.OfType<KeyShortcut>().Any();
        }

        public bool HasShortcut(IStaticPanelDefinition definition)
        {
            definition.AssertParameterNotNull(nameof(definition));
            if(!PanelManager.IsRegistered(definition)) {
                throw new Exception("Error : Requesting BringIntoView shortcut information of an unknown static panel definition : The definition is not registered in the panel manager.");
            }

            return definition.OfType<BringIntoViewOnKeyShortcut>().Any();
        }

        #endregion HasShortcut


        #region GetShortcut

        public KeyShortcut GetShortcut(IManagedCommand command)
        {
            command.AssertParameterNotNull(nameof(command));

            if (!CommandManager.IsRegistered(command)) {
                throw new Exception("Error : Requesting shortcut information of an unknown command : The command is not registered in the command manager.");
            }

            try 
            {
                return command.Metadata.OfType<KeyShortcut>().Single();
            }

            catch(InvalidOperationException) 
            {
                throw new Exception("Error : The specified command doesn't have an associated shortcut.");
            }

        }

        public BringIntoViewOnKeyShortcut GetShortcut(IStaticPanelDefinition definition)
        {
            definition.AssertParameterNotNull(nameof(definition));

            if (!PanelManager.IsRegistered(definition)) {
                throw new Exception("Error : Requesting BringIntoView shortcut information of an unknown static panel definition : The definition is not registered in the panel manager.");
            }

            try 
            {
                return definition.OfType<BringIntoViewOnKeyShortcut>().Single();
            }
            catch(InvalidOperationException) 
            {
                throw new Exception($"Error : The specified static panel definition({definition.ViewModel.Name}) doesn't have an associated BringIntoView shortcut.");
            }
        }

        #endregion GetShortcut


        #region SetShortcut

        public void SetShortcut(IManagedCommand command, ModifierKeys modifierKeys, Key key)
        {
            command.AssertParameterNotNull(nameof(command));
            if (!CommandManager.IsRegistered(command)) {
                throw new Exception("Error : Cannot set the shortcut for the specified command : The command is not registered in the command manager.");
            }
            
            if(HasShortcut(command)) {
                var shortcut = GetShortcut(command);
                if(shortcut.ModifierKeys == modifierKeys && shortcut.Key == key) {
                    return;
                }
                shortcut.ModifierKeys = modifierKeys;
                shortcut.Key = key;
            }
            else {
                command.Metadata.Add(new KeyShortcut(modifierKeys, key));
            }

            NotifyCommandShortcutsChanged(command);
        }

        public void SetShortcut(IStaticPanelDefinition definition, ModifierKeys modifierKeys, Key key)
        {
            definition.AssertParameterNotNull(nameof(definition));
            if (!PanelManager.IsRegistered(definition)) {
                throw new Exception("Error : Cannot set the BringIntoView shortcut for the specified static panel definition : The definition is not registered in the panel manager.");
            }

            if(HasShortcut(definition)) {
                var shortcut = GetShortcut(definition);
                if(shortcut.ModifierKeys == modifierKeys && shortcut.Key == key) {
                    return;
                }
                shortcut.ModifierKeys = modifierKeys;
                shortcut.Key = key;
            }
            else {
                var metadataCollection = definition as MetadataCollection<IStaticPanelMetadata>;
                metadataCollection.Add(new BringIntoViewOnKeyShortcut(modifierKeys, key));
            }

            NotifyPanelShortcutsChanged(definition);
        }

        #endregion SetShortcut


        #region ClearShortcut

        public void ClearShortcut(IManagedCommand command)
        {
            command.AssertParameterNotNull(nameof(command));
            if(!CommandManager.IsRegistered(command)) {
                throw new Exception($"Error : Requesting shortcut cleanup on an unknown command : The command is not registered in the CommandManager.");
            }

            if(HasShortcut(command)) {
                var shortcut = GetShortcut(command);
                command.Metadata.Remove(shortcut);
                NotifyCommandShortcutsChanged(command);
            }
        }

        public void ClearShortcut(IStaticPanelDefinition definition)
        {
            definition.AssertParameterNotNull(nameof(definition));
            if(!PanelManager.IsRegistered(definition)) {
                throw new Exception($"Error : Requesting shortcut cleanup on an unknown static panel definition : The definition is not registered in the PanelManager.");
            }

            if(HasShortcut(definition)) {
                var shortcut = GetShortcut(definition);
                var metadataCollection = definition as MetadataCollection<IStaticPanelMetadata>;
                metadataCollection.Remove(shortcut);
                NotifyPanelShortcutsChanged(definition);
            }
        }

        #endregion ClearShortcut


        #region Notify

        private void NotifyCommandShortcutsChanged(IManagedCommand command)
        {
            if(!LoadingScope.IsInScope) {
                EventAggregator.GetEvent<ShortcutChangedEvent>().Publish(new ManagedCommandShortcutChangedArgs(command));
            }
        }

        private void NotifyPanelShortcutsChanged(IStaticPanelDefinition definition)
        {
            if(!LoadingScope.IsInScope) {
                EventAggregator.GetEvent<ShortcutChangedEvent>().Publish(new BringPanelIntoViewShortcutChangedArgs(definition));
            }
        }

        #endregion Notify


        #region LoadShortcuts

        public static string ShortcutSerializationFile { get { return Path.Combine(AppInfo.ApplicationConfigRepository, "Shortcuts.xml"); } }

        [Handles(typeof(UILoadedEvent))]
        public void LoadShortcuts()
        {
            if (!File.Exists(ShortcutSerializationFile)) return;

            ShortcutSerializationDictionary shortcutDictionary = null;
            var serializer = new XmlSerializer(typeof(ShortcutSerializationDictionary));
            using (var reader = new StreamReader(ShortcutSerializationFile)) {
                shortcutDictionary = (ShortcutSerializationDictionary)serializer.Deserialize(reader);
            }

            using (LoadingScope.BeginThreadScope()) {
                foreach(var managedCommand in CommandManager.ManagedCommands) {
                    var shortcutInfo = shortcutDictionary.ManagedCommandShortcutSerializationInfo.SingleOrDefault(o => o.Matches(managedCommand));
                    if (shortcutInfo == null) continue;

                    bool hasShortcutByDefault = managedCommand.Metadata.OfType<KeyShortcut>().Any();

                    if (hasShortcutByDefault) {
                        var defaultShortcut = managedCommand.Metadata.OfType<KeyShortcut>().Single();
                        if(defaultShortcut.ModifierKeys != shortcutInfo.ModifierKeys || defaultShortcut.Key != shortcutInfo.Key) {
                            SetShortcut(managedCommand, shortcutInfo.ModifierKeys, shortcutInfo.Key);
                        }
                    }
                    else {
                        if(shortcutInfo.HasShortcut) {
                            SetShortcut(managedCommand, shortcutInfo.ModifierKeys, shortcutInfo.Key);
                        }
                    }
                }
                foreach(var staticPanelDefinition in PanelManager.StaticPanelDefinitions) {
                    var shortcutInfo = shortcutDictionary.BringPanelIntoViewShortcutSerializationInfo.SingleOrDefault(o => o.Matches(staticPanelDefinition));
                    if (shortcutInfo == null) continue;

                    bool hasShortcutByDefault = staticPanelDefinition.OfType<BringIntoViewOnKeyShortcut>().Any();

                    if (hasShortcutByDefault) {
                        var defaultShortcut = staticPanelDefinition.OfType<BringIntoViewOnKeyShortcut>().Single();
                        if (defaultShortcut.ModifierKeys != shortcutInfo.ModifierKeys || defaultShortcut.Key != shortcutInfo.Key) {
                            SetShortcut(staticPanelDefinition, shortcutInfo.ModifierKeys, shortcutInfo.Key);
                        }
                    }
                    else {
                        if (shortcutInfo.HasShortcut) {
                            SetShortcut(staticPanelDefinition, shortcutInfo.ModifierKeys, shortcutInfo.Key);
                        }
                    }
                }
            }

        }
        
        #endregion LoadShortcuts


        #region SaveShortcuts

        [Handles(typeof(ShutdownEvent))]
        public void SaveShortcuts()
        {
            var shortcutDictionary = ShortcutSerializationDictionary.Create();
            foreach(var managedCommand in CommandManager.ManagedCommands) {
                shortcutDictionary.ManagedCommandShortcutSerializationInfo.Add(ManagedCommandShortcutSerializationInfo.CreateFromManagedCommand(managedCommand));
            }
            foreach(var staticPanelDefinition in PanelManager.StaticPanelDefinitions) {
                shortcutDictionary.BringPanelIntoViewShortcutSerializationInfo.Add(BringPanelIntoViewShortcutSerializationInfo.CreateFromDefinition(staticPanelDefinition));
            }

            if(File.Exists(ShortcutSerializationFile)) {
                File.Delete(ShortcutSerializationFile);
            }

            var serializer = new XmlSerializer(typeof(ShortcutSerializationDictionary));
            using(var writer = new StreamWriter(ShortcutSerializationFile)) {
                serializer.Serialize(writer, shortcutDictionary);
            }
        }

        #endregion SaveShortcuts

    }
}
