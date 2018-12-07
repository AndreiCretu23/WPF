using System.Windows.Input;

namespace Quantum.Metadata
{
    /// <summary>
    /// This metadata type can only be associated with a static panel definition.
    /// The panel associated with the static panel definition owner of this metadata type 
    /// will be brought into view when the specified key combination is pressed.
    /// </summary>
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class BringIntoViewOnKeyShortcut : KeyShortcutBase, IStaticPanelMetadata
    {
        public BringIntoViewOnKeyShortcut(ModifierKeys modifierKeys, Key key)
            : base(modifierKeys, key)
        {
        }
    }
}
