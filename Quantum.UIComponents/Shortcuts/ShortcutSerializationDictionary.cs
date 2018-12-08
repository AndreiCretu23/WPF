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
    public class ShortcutSerializationDictionary
    {
        public List<ManagedCommandShortcutSerializationInfo> ManagedCommandShortcutSerializationInfo { get; set; }
        public List<BringPanelIntoViewShortcutSerializationInfo> BringPanelIntoViewShortcutSerializationInfo { get; set; }
        
        public static ShortcutSerializationDictionary Create()
        {
            return new ShortcutSerializationDictionary()
            {
                ManagedCommandShortcutSerializationInfo = new List<ManagedCommandShortcutSerializationInfo>(),
                BringPanelIntoViewShortcutSerializationInfo = new List<BringPanelIntoViewShortcutSerializationInfo>()
            };
        }
    }

    public class ManagedCommandShortcutSerializationInfo
    {
        public string CommandGuid { get; set; }
        public bool HasShortcut { get; set; }
        public ModifierKeys ModifierKeys { get; set; }
        public Key Key { get; set; }

        public static ManagedCommandShortcutSerializationInfo CreateFromManagedCommand(IManagedCommand command)
        {
            command.AssertParameterNotNull(nameof(command));

            var guid = command.Metadata.OfType<CommandGuid>().Single().Guid;
            var hasShortcut = command.Metadata.OfType<KeyShortcut>().Any();

            return new ManagedCommandShortcutSerializationInfo()
            {
                CommandGuid = guid,
                HasShortcut = hasShortcut,
                ModifierKeys = hasShortcut ? command.Metadata.OfType<KeyShortcut>().Single().ModifierKeys : ModifierKeys.None,
                Key = hasShortcut ? command.Metadata.OfType<KeyShortcut>().Single().Key : Key.None
            };
        }

        public bool Matches(IManagedCommand command)
        {
            command.AssertParameterNotNull(nameof(command));
            return CommandGuid == command.Metadata.OfType<CommandGuid>().Single().Guid;
        }
    }

    public class BringPanelIntoViewShortcutSerializationInfo
    {
        public string ViewGuid { get; set; }
        public string IViewGuid { get; set; }
        public string ViewModelGuid { get; set; }
        public string IViewModelGuid { get; set; }

        public bool HasShortcut { get; set; }
        public ModifierKeys ModifierKeys { get; set; }
        public Key Key { get; set; }


        public static BringPanelIntoViewShortcutSerializationInfo CreateFromDefinition(IStaticPanelDefinition definition)
        {
            definition.AssertParameterNotNull(nameof(definition));

            bool hasShortcut = definition.OfType<BringIntoViewOnKeyShortcut>().Any();

            return new BringPanelIntoViewShortcutSerializationInfo()
            {
                ViewGuid = definition.View.GetGuid(),
                IViewGuid = definition.IView.GetGuid(),
                ViewModelGuid = definition.ViewModel.GetGuid(),
                IViewModelGuid = definition.IViewModel.GetGuid(),

                HasShortcut = hasShortcut,
                ModifierKeys = hasShortcut ? definition.OfType<BringIntoViewOnKeyShortcut>().Single().ModifierKeys : ModifierKeys.None, 
                Key = hasShortcut ? definition.OfType<BringIntoViewOnKeyShortcut>().Single().Key : Key.None
            };
        }

        public bool Matches(IStaticPanelDefinition definition)
        {
            definition.AssertParameterNotNull(nameof(definition));

            return ViewGuid == definition.View.GetGuid() &&
                   IViewGuid == definition.IView.GetGuid() &&
                   ViewModelGuid == definition.ViewModel.GetGuid() &&
                   IViewModelGuid == definition.IViewModel.GetGuid();
        }
    }
}
