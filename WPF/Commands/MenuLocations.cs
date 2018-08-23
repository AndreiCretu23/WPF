using Quantum.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Commands
{
    public static class MenuLocations
    {
        public static readonly AbstractMenuPath File = new AbstractMenuPath(AbstractMenuPath.Root, new Description("File"), 0, 1);
        public static readonly AbstractMenuPath Edit = new AbstractMenuPath(AbstractMenuPath.Root, new Description("Edit"), 0, 2);
        public static readonly AbstractMenuPath View = new AbstractMenuPath(AbstractMenuPath.Root, new Description("View"), 0, 3);

        public static readonly AbstractMenuPath Category3To4 = new AbstractMenuPath(File, new Description("3To4"), 1, 1);
        public static readonly AbstractMenuPath Yolo1 = new AbstractMenuPath(Category3To4, new Description("Yolo1"), 1, 4);
        public static readonly AbstractMenuPath Yolo2 = new AbstractMenuPath(Category3To4, new Description("Yolo"), 2, 1);
        public static readonly AbstractMenuPath SubCat3To4 = new AbstractMenuPath(Category3To4, new Description("SubCategory"), 1, 3);
        public static readonly AbstractMenuPath Recent = new AbstractMenuPath(File, new Description("Recent"), 2, 1);

        public static readonly AbstractMenuPath Category8To10 = new AbstractMenuPath(Edit, new Description("8To10"), 1, 2);

    }

}
