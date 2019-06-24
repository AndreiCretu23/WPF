using System.Windows;

namespace Quantum.Controls
{
    public interface ICustomContentOwner
    {
        FrameworkElement ContentElement { get; }
    }
}
