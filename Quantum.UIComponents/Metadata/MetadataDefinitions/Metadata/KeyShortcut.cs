using System;
using System.Windows.Input;

namespace Quantum.Metadata
{
    /// <summary>
    /// This metadata type is used to attach a shortcut to a command. The UIElement on which the 
    /// actual gesture will be added depends on the owner command's type and configuration. 
    /// For more details, see ManagedCommand and ComponentCommand. <para/>
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// </summary>
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class KeyShortcut : ICommandMetadata
    {
        public ModifierKeys ModifierKeys { get; private set; }
        public Key Key { get; private set; }

        public KeyShortcut(ModifierKeys modifierKeys, Key key)
        {
            ModifierKeys = modifierKeys;
            Key = key;
        }

        public string GetInputGestureText()
        {
            string inputGestureText = String.Empty;

            inputGestureText += ModifierKeys.ToString();
            inputGestureText = inputGestureText.Replace("Control", "Ctrl");
            inputGestureText = inputGestureText.Replace(", ", "+");

            inputGestureText += "+";
            inputGestureText += Key.ToString();

            return inputGestureText;
        }
    }
}
