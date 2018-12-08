using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Quantum.Metadata
{
    public abstract class KeyShortcutBase
    {
        public ModifierKeys ModifierKeys { get; internal set; }
        public Key Key { get; internal set; }

        public KeyShortcutBase(ModifierKeys modifierKeys, Key key)
        {
            ModifierKeys = modifierKeys;
            Key = key;
        }

        public string GetInputGestureText()
        {
            string inputGestureText = String.Empty;

            Func<Enum, string> getModifierString = modifier =>
            {
                var modifierString = modifier.ToString();
                if(modifierString == "Control") {
                    return "Ctrl";
                }
                return modifierString;
            };

            var modifiers = Enum.GetValues(typeof(ModifierKeys)).Cast<Enum>().Skip(1).ToList();
            var sortedModifiers = new List<Enum>()
            {
                modifiers[1],
                modifiers[2],
                modifiers[0],
                modifiers[3]
            };

            foreach(var modifier in sortedModifiers) {
                if(ModifierKeys.HasFlag(modifier)) {
                    inputGestureText += getModifierString(modifier);
                    inputGestureText += "+";
                }
            }
            
            inputGestureText += Key.ToString();

            return inputGestureText;
        }
    }

    /// <summary>
    /// This metadata type is used to attach a shortcut to a command. The UIElement on which the 
    /// actual gesture will be added depends on the owner command's type and configuration. 
    /// For more details, see ManagedCommand and ComponentCommand. <para/>
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// </summary>
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class KeyShortcut : KeyShortcutBase, ICommandMetadata
    {
        public KeyShortcut(ModifierKeys modifierKeys, Key key)
            : base(modifierKeys, key)
        {
        }

    }
}
