using System;
using System.Windows.Media.Imaging;

namespace Quantum.Utils
{
    public static class IconUtils
    {
        public static BitmapImage GetResourceIcon(string UriPath)
        {
            var image = new BitmapImage();

            image.BeginInit();
            image.UriSource = new Uri(UriPath, UriKind.RelativeOrAbsolute);
            image.EndInit();

            return image;
        }
    }
}
