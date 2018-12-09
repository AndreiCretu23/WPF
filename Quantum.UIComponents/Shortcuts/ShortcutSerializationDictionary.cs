using Quantum.Command;
using Quantum.Metadata;
using Quantum.UIComponents;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quantum.Shortcuts
{
    public class ShortcutDictionary
    {
        public List<ManagedCommandShortcutInformation> ManagedCommandShortcutInfo { get; set; }
        public List<StaticPanelShortcutInformation> StaticPanelShortcutInfo { get; set; }
        
        public static ShortcutDictionary Create()
        {
            return new ShortcutDictionary()
            {
                ManagedCommandShortcutInfo = new List<ManagedCommandShortcutInformation>(),
                StaticPanelShortcutInfo = new List<StaticPanelShortcutInformation>()
            };
        }
    }

    public class ManagedCommandShortcutInformation
    {
        public string CommandGuid { get; set; }
        public bool HasShortcut { get; set; }
        public bool IsDefault { get; set; }
        public ModifierKeys ModifierKeys { get; set; }
        public Key Key { get; set; }

        public static ManagedCommandShortcutInformation CreateFromManagedCommand(IManagedCommand command)
        {
            command.AssertParameterNotNull(nameof(command));

            var guid = command.Metadata.OfType<CommandGuid>().Single().Guid;
            var hasShortcut = command.Metadata.OfType<KeyShortcut>().Any();

            return new ManagedCommandShortcutInformation()
            {
                CommandGuid = guid,
                HasShortcut = hasShortcut,
                IsDefault = true,
                ModifierKeys = hasShortcut ? command.Metadata.OfType<KeyShortcut>().Single().ModifierKeys : ModifierKeys.None,
                Key = hasShortcut ? command.Metadata.OfType<KeyShortcut>().Single().Key : Key.None
            };
        }
        
        public void CheckAndResolveShortcutChangedContext(ManagedCommandShortcutInformation defaultInformation)
        {
            IsDefault = HasShortcut == defaultInformation.HasShortcut &&
                        ModifierKeys == defaultInformation.ModifierKeys &&
                        Key == defaultInformation.Key;
        }

        public bool Matches(IManagedCommand command)
        {
            command.AssertParameterNotNull(nameof(command));
            return CommandGuid == command.Metadata.OfType<CommandGuid>().Single().Guid;
        }
    }

    public class StaticPanelShortcutInformation
    {
        public string ViewGuid { get; set; }
        public string IViewGuid { get; set; }
        public string ViewModelGuid { get; set; }
        public string IViewModelGuid { get; set; }

        public bool HasShortcut { get; set; }
        public bool IsDefault { get; set; }
        public ModifierKeys ModifierKeys { get; set; }
        public Key Key { get; set; }


        public static StaticPanelShortcutInformation CreateFromDefinition(IStaticPanelDefinition definition)
        {
            definition.AssertParameterNotNull(nameof(definition));

            bool hasShortcut = definition.OfType<BringIntoViewOnKeyShortcut>().Any();

            return new StaticPanelShortcutInformation()
            {
                ViewGuid = definition.View.GetGuid(),
                IViewGuid = definition.IView.GetGuid(),
                ViewModelGuid = definition.ViewModel.GetGuid(),
                IViewModelGuid = definition.IViewModel.GetGuid(),

                HasShortcut = hasShortcut,
                IsDefault = true,
                ModifierKeys = hasShortcut ? definition.OfType<BringIntoViewOnKeyShortcut>().Single().ModifierKeys : ModifierKeys.None, 
                Key = hasShortcut ? definition.OfType<BringIntoViewOnKeyShortcut>().Single().Key : Key.None
            };
        }

        public void CheckAndResolveShortcutChangedContext(StaticPanelShortcutInformation defaultInformation)
        {
            IsDefault = HasShortcut == defaultInformation.HasShortcut &&
                        ModifierKeys == defaultInformation.ModifierKeys &&
                        Key == defaultInformation.Key;
        }

        public bool Matches(IStaticPanelDefinition definition)
        {
            definition.AssertParameterNotNull(nameof(definition));

            return ViewGuid == definition.View.GetGuid() &&
                   IViewGuid == definition.IView.GetGuid() &&
                   ViewModelGuid == definition.ViewModel.GetGuid() &&
                   IViewModelGuid == definition.IViewModel.GetGuid();
        }

        public bool Matches(StaticPanelShortcutInformation shortcutInfo)
        {
            return ViewGuid == shortcutInfo.ViewGuid &&
                   IViewGuid == shortcutInfo.IViewGuid &&
                   ViewModelGuid == shortcutInfo.ViewModelGuid &&
                   IViewModelGuid == shortcutInfo.IViewModelGuid;
        }
    }
}
