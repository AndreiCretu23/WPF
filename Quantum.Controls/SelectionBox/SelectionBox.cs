using System;
using System.Windows;
using System.Windows.Media;

namespace Quantum.Controls
{
    public class SelectionBox : UIElement
    {

        #region DependencyProperties
        
        public static readonly DependencyProperty TargetTypeProperty = DependencyProperty.Register
        (
            name: "TargetType",
            propertyType: typeof(Type),
            ownerType: typeof(SelectionBox),
            typeMetadata: new PropertyMetadata(defaultValue: null)
        );

        public static readonly DependencyProperty TargetSelectionPropertyProperty = DependencyProperty.Register
        (
            name: "TargetSelectionProperty",
            propertyType: typeof(DependencyProperty),
            ownerType: typeof(SelectionBox),
            typeMetadata: new PropertyMetadata(defaultValue: null)
        );

        public static readonly DependencyProperty VisualTraverserProperty = DependencyProperty.Register
        (
            name: "VisualTraverser",
            propertyType: typeof(IVisualTraverser),
            ownerType: typeof(SelectionBox),
            typeMetadata: new PropertyMetadata(new VisualTraverser())
        );

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register
        (
            name: "Background",
            propertyType: typeof(Brush),
            ownerType: typeof(SelectionBox),
            typeMetadata: new PropertyMetadata(defaultValue: new BrushConverter().ConvertFromString("#7F4047F7"))
        );

        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register
        (
            name: "BorderBrush",
            propertyType: typeof(Brush),
            ownerType: typeof(SelectionBox),
            typeMetadata: new PropertyMetadata(defaultValue: Brushes.Gray)
        );

        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register
        (
            name: "BorderThickness",
            propertyType: typeof(double),
            ownerType: typeof(SelectionBox),
            typeMetadata: new PropertyMetadata(1d)
        );

        #endregion DependencyProperties

        
        #region EventProperties

        public static readonly RoutedEvent SelectionStateChangedEvent = EventManager.RegisterRoutedEvent
        (
            name: "SelectionStateChanged",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(SelectionStateChangedHandler),
            ownerType: typeof(SelectionBox)
        );

        #endregion EventProperties

        
        #region Properties

        public Type TargetType
        {
            get { return (Type)GetValue(TargetTypeProperty); }
            set { SetValue(TargetTypeProperty, value); }
        }
        
        public DependencyProperty TargetSelectionProperty
        {
            get { return (DependencyProperty)GetValue(TargetSelectionPropertyProperty); }
            set { SetValue(TargetSelectionPropertyProperty, value); }
        }

        public IVisualTraverser VisualTraverser
        {
            get { return (IVisualTraverser)GetValue(VisualTraverserProperty); }
            set { SetValue(VisualTraverserProperty, value); }
        }
        
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
        
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        #endregion Properties


        #region Events
        
        public event SelectionStateChangedHandler SelectionStateChanged
        {
            add { AddHandler(SelectionStateChangedEvent, value); }
            remove { RemoveHandler(SelectionStateChangedEvent, value); }
        }
        
        #endregion Events

        public SelectionBox()
        {
        }
    }

    public delegate void SelectionStateChangedHandler(object sender, SelectionStateChangedArgs e);

    public class SelectionStateChangedArgs : RoutedEventArgs
    {
        public FrameworkElement Owner { get; }
        public SelectionBox SelectionBox { get; }
        public bool IsSelecting { get; }

        public SelectionStateChangedArgs(RoutedEvent routedEvent, FrameworkElement owner, SelectionBox selectionBox, bool isSelecting)
            : base(routedEvent)
        {
            Owner = owner;
            SelectionBox = selectionBox;
            IsSelecting = isSelecting;
        }
    }
}
