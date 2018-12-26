using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Quantum.Controls
{
    public class SelectionBoxAdorner : Adorner
    {
        #region DependencyProperties

        public static readonly DependencyProperty IsRemovedProperty = DependencyProperty.Register
        (
            name: "IsRemoved",
            propertyType: typeof(bool),
            ownerType: typeof(SelectionBoxAdorner),
            typeMetadata: new PropertyMetadata(false)
        );

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register
        (
            name: "Color",
            propertyType: typeof(Brush),
            ownerType: typeof(SelectionBoxAdorner),
            typeMetadata: new PropertyMetadata(null)
        );

        public static readonly DependencyProperty BorderProperty = DependencyProperty.Register
        (
            name: "Border",
            propertyType: typeof(Pen),
            ownerType: typeof(SelectionBoxAdorner),
            typeMetadata: new PropertyMetadata(null)
        );

        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register
        (
            name: "StartPoint", 
            propertyType: typeof(Point), 
            ownerType: typeof(SelectionBoxAdorner), 
            typeMetadata: new PropertyMetadata(new Point(0, 0))
        );

        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register
        (
            
            name: "EndPoint", 
            propertyType: typeof(Point), 
            ownerType: typeof(SelectionBoxAdorner), 
            typeMetadata: new PropertyMetadata(new Point(0, 0))
        );
        
        #endregion DependencyProperties

        #region Properties

        public bool IsRemoved
        {
            get { return (bool)GetValue(IsRemovedProperty); }
            set { SetValue(IsRemovedProperty, value); }
        }

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public Pen Border
        {
            get { return (Pen)GetValue(BorderProperty); }
            set { SetValue(BorderProperty, value); }
        }
        
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }
        
        public Point EndPoint
        {
            get { return (Point)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        #endregion Properties

        public SelectionBoxAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }
        

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Color, Border, new Rect(StartPoint, EndPoint));
            base.OnRender(drawingContext);
        }

    }
}
