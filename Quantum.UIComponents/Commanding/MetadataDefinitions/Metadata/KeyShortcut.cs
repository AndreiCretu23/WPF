using System;
using System.Windows.Input;

namespace Quantum.Command
{
    public class KeyShortcut : IMainMenuMetadata
    {
        public bool SupportsMultiple { get { return true; } }
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
