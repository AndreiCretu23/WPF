using Quantum.Command;
using Quantum.Events;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.UIComponents;
using Quantum.Utils;
using System;
using System.Collections.Generic;
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

        private Scope LoadingScope { get; }
        
        private ShortcutDictionary DefaultShortcuts { get; set; }

        private GenericComparer<KeyShortcutBase> ShortcutComparer
        {
            get
            {
                return new GenericComparer<KeyShortcutBase>
                (
                    (s1, s2) => s1.ModifierKeys == s2.ModifierKeys && s1.Key == s2.Key,
                    s => s.ModifierKeys.GetHashCode() + s.Key.GetHashCode()
                );
            }
        }

        public ShortcutManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            LoadingScope = new Scope();
            LoadingScope.OnAllScopesEnd += (sender, e) => EventAggregator.GetEvent<ShortcutChangedEvent>().Publish(new GlobalRebuildShortcutChangedArgs());
        }

        
        #region HasShortcut

        public bool HasShortcut(IGlobalCommand command)
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

        public KeyShortcut GetShortcut(IGlobalCommand command)
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
        
        public void SetShortcut(IGlobalCommand command, ModifierKeys modifierKeys, Key key)
        {
            command.AssertParameterNotNull(nameof(command));
            if (!CommandManager.IsRegistered(command)) {
                throw new Exception("Error : Cannot set the shortcut for the specified command : The command is not registered in the command manager.");
            }
            
            if(!LoadingScope.IsInScope) {
                var matchingElements = GetElementsMatchingShortcut(modifierKeys, key).Where(o => !(o == command));
                if(matchingElements.Any()) {
                    throw new Exception($"Error setting the shortcut {modifierKeys.ToString()}+{key.ToString()} on command {GetElementName(command)} " +
                                        $"because element(s) {string.Join(",", matchingElements.Select(o => GetElementName(o)))} already have a shortcut with the same " +
                                        $"key combination.");
                }
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

            if (!LoadingScope.IsInScope) {
                var matchingElements = GetElementsMatchingShortcut(modifierKeys, key).Where(o => !(o == definition));
                if (matchingElements.Any()) {
                    throw new Exception($"Error setting the shortcut {modifierKeys.ToString()}+{key.ToString()} on command {GetElementName(definition)} " +
                                        $"because element(s) {string.Join(",", matchingElements.Select(o => GetElementName(o)))} already have a shortcut with the same " +
                                        $"key combination.");
                }
            }

            if (HasShortcut(definition)) {
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

        public void ClearShortcut(IGlobalCommand command)
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

        private void NotifyCommandShortcutsChanged(IGlobalCommand command)
        {
            if(!LoadingScope.IsInScope) {
                EventAggregator.GetEvent<ShortcutChangedEvent>().Publish(new GlobalCommandShortcutChangedArgs(command));
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
            AssertShortcuts();
            DefaultShortcuts = GetShortcutDictionaryForCurrentState();

            if (!File.Exists(ShortcutSerializationFile)) return;

            ShortcutDictionary shortcutDictionary = null;
            var serializer = new XmlSerializer(typeof(ShortcutDictionary));
            using (var reader = new StreamReader(ShortcutSerializationFile)) {
                shortcutDictionary = (ShortcutDictionary)serializer.Deserialize(reader);
            }

            using (LoadingScope.BeginScope()) {
                foreach(var globalCommand in CommandManager.GlobalCommands) {
                    var shortcutInfo = shortcutDictionary.GlobalCommandShortcutInfo.SingleOrDefault(o => o.Matches(globalCommand));
                    if (shortcutInfo == null) continue;
                    
                    if(!shortcutInfo.IsDefault) {
                        if(shortcutInfo.HasShortcut) {
                            SetShortcut(globalCommand, shortcutInfo.ModifierKeys, shortcutInfo.Key);
                        }
                        else {
                            ClearShortcut(globalCommand);
                        }
                    }
                }

                foreach(var staticPanelDefinition in PanelManager.StaticPanelDefinitions) {
                    var shortcutInfo = shortcutDictionary.StaticPanelShortcutInfo.SingleOrDefault(o => o.Matches(staticPanelDefinition));
                    if (shortcutInfo == null) continue;

                    bool hasShortcutByDefault = staticPanelDefinition.OfType<BringIntoViewOnKeyShortcut>().Any();

                    if(!shortcutInfo.IsDefault) {
                        if(shortcutInfo.HasShortcut) {
                            SetShortcut(staticPanelDefinition, shortcutInfo.ModifierKeys, shortcutInfo.Key);
                        }
                        else {
                            ClearShortcut(staticPanelDefinition);
                        }
                    }
                }
            }

            AssertShortcuts();
        }
        
        #endregion LoadShortcuts


        #region SaveShortcuts

        [Handles(typeof(ShutdownEvent))]
        public void SaveShortcuts()
        {
            var shortcutDictionary = GetShortcutDictionaryForCurrentState();
            foreach(var shortcutInfo in shortcutDictionary.GlobalCommandShortcutInfo) {
                var defaultShortcut = DefaultShortcuts.GlobalCommandShortcutInfo.Single(o => o.CommandGuid == shortcutInfo.CommandGuid);
                shortcutInfo.CheckAndResolveShortcutChangedContext(defaultShortcut);
            }

            foreach(var shortcutInfo in shortcutDictionary.StaticPanelShortcutInfo) {
                var defaultShortcut = DefaultShortcuts.StaticPanelShortcutInfo.Single(o => o.Matches(shortcutInfo));
                shortcutInfo.CheckAndResolveShortcutChangedContext(defaultShortcut);
            }

            if(File.Exists(ShortcutSerializationFile)) {
                File.Delete(ShortcutSerializationFile);
            }

            var serializer = new XmlSerializer(typeof(ShortcutDictionary));
            using(var writer = new StreamWriter(ShortcutSerializationFile)) {
                serializer.Serialize(writer, shortcutDictionary);
            }
        }

        #endregion SaveShortcuts


        #region ShortcutDictionary

        private ShortcutDictionary GetShortcutDictionaryForCurrentState()
        {
            var dictionary = ShortcutDictionary.Create();

            foreach(var globalCommand in CommandManager.GlobalCommands) {
                dictionary.GlobalCommandShortcutInfo.Add(GlobalCommandShortcutInformation.CreateFromGlobalCommand(globalCommand));
            }

            foreach(var staticPanelDefinition in PanelManager.StaticPanelDefinitions) {
                dictionary.StaticPanelShortcutInfo.Add(StaticPanelShortcutInformation.CreateFromDefinition(staticPanelDefinition));
            }

            return dictionary;
        }
        
        #endregion ShortcutDictionary


        #region AssertShortcuts

        private void AssertShortcuts()
        {
            var allShortcuts = CommandManager.GlobalCommands.Where(o => o.Metadata.OfType<KeyShortcut>().Any()).SelectMany(o => o.Metadata.OfType<KeyShortcutBase>()).Concat
                                    (PanelManager.StaticPanelDefinitions.Where(o => o.OfType<BringIntoViewOnKeyShortcut>().Any()).SelectMany(o => o.OfType<KeyShortcutBase>()));

            var duplicates = allShortcuts.Duplicates(ShortcutComparer);
            
            if(duplicates.Any()) {

                string getName(object o)
                {
                    if (o is IStaticPanelDefinition def) {
                        return $"StaticPanelDefinition<{def.IView}, {def.View}, {def.IViewModel}, {def.ViewModel}>";
                    }
                    else if (o is IGlobalCommand globalCommand) {
                        return CommandManager.GetCommandName(globalCommand);
                    }
                    return String.Empty;
                }

                var duplicateShortcut = duplicates.First();
                var matchingElements = GetElementsMatchingShortcut(duplicateShortcut.ModifierKeys, duplicateShortcut.Key);
                var elementString = string.Join(",", matchingElements.Select(o => getName(o)));

                throw new Exception($"Error : Commands/PanelDefinitions {elementString} have the same shortcut : {duplicateShortcut.ModifierKeys.ToString()} + {duplicateShortcut.Key.ToString()}");
            }
        }

        #endregion AssertShortcuts


        #region Misc

        private string GetElementName(object o)
        {
            if (o is IStaticPanelDefinition def) {
                return $"StaticPanelDefinition<{def.IView}, {def.View}, {def.IViewModel}, {def.ViewModel}>";
            }
            else if (o is IGlobalCommand globalCommand) {
                return CommandManager.GetCommandName(globalCommand);
            }
            return String.Empty;
        }

        public IEnumerable<object> GetElementsMatchingShortcut(ModifierKeys modifierKeys, Key key)
        {
            var keyShortcut = new KeyShortcut(modifierKeys, key);
            return CommandManager.GlobalCommands.Where(o => o.Metadata.OfType<KeyShortcutBase>().Any() && ShortcutComparer.Equals(o.Metadata.OfType<KeyShortcutBase>().Single(), keyShortcut)).Cast<object>().Concat
                  (PanelManager.StaticPanelDefinitions.Where(def => def.OfType<KeyShortcutBase>().Any() && ShortcutComparer.Equals(def.OfType<KeyShortcutBase>().Single(), keyShortcut)).Cast<object>());
        }

        #endregion Misc
    }
}
