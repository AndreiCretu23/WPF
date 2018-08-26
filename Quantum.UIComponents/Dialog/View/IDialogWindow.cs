using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using System.Windows.Shell;
using System.Windows.Threading;

namespace Quantum.UIComponents
{
    public interface IDialogWindow : IAddChild, IFrameworkInputElement, IInputElement, ISupportInitialize, IQueryAmbient, IAnimatable
    {
        #region .NETFramework

        Dispatcher Dispatcher { get; }

        DependencyObjectType DependencyObjectType { get; }
        bool IsSealed { get; }

        string Uid { get; set; }
        Visibility Visibility { get; set; }
        bool ClipToBounds { get; set; }
        Geometry Clip { get; set; }
        bool SnapsToDevicePixels { get; set; }
        bool IsFocused { get; }
        bool IsHitTestVisible { get; set; }
        bool IsVisible { get; }
        bool AreAnyTouchesCapturedWithin { get; }
        int PersistId { get; }
        bool IsManipulationEnabled { get; set; }
        bool AreAnyTouchesOver { get; }
        bool AreAnyTouchesDirectlyOver { get; }
        bool AreAnyTouchesCaptured { get; }
        IEnumerable<TouchDevice> TouchesCaptured { get; }
        IEnumerable<TouchDevice> TouchesCapturedWithin { get; }
        IEnumerable<TouchDevice> TouchesOver { get; }
        CacheMode CacheMode { get; set; }
        BitmapEffectInput BitmapEffectInput { get; set; }
        BitmapEffect BitmapEffect { get; set; }
        Size RenderSize { get; set; }
        bool IsArrangeValid { get; }
        bool IsMeasureValid { get; }
        Size DesiredSize { get; }
        bool AllowDrop { get; set; }
        CommandBindingCollection CommandBindings { get; }
        InputBindingCollection InputBindings { get; }
        Effect Effect { get; set; }
        bool IsMouseCaptureWithin { get; }
        bool IsStylusCaptureWithin { get; }
        bool IsInputMethodEnabled { get; }
        double Opacity { get; set; }
        Brush OpacityMask { get; set; }
        IEnumerable<TouchDevice> TouchesDirectlyOver { get; }
        Point RenderTransformOrigin { get; set; }
        Transform RenderTransform { get; set; }

        Transform LayoutTransform { get; set; }
        double Width { get; set; }
        double MinWidth { get; set; }
        double MaxHeight { get; set; }
        double Height { get; set; }
        double MinHeight { get; set; }
        double ActualHeight { get; }
        double MaxWidth { get; set; }
        double ActualWidth { get; }
        TriggerCollection Triggers { get; }
        object Tag { get; set; }
        XmlLanguage Language { get; set; }
        BindingGroup BindingGroup { get; set; }
        object DataContext { get; set; }
        ResourceDictionary Resources { get; set; }
        DependencyObject TemplatedParent { get; }
        bool UseLayoutRounding { get; set; }
        FlowDirection FlowDirection { get; set; }
        InputScope InputScope { get; set; }
        Thickness Margin { get; set; }
        Style Style { get; set; }
        VerticalAlignment VerticalAlignment { get; set; }
        bool OverridesDefaultStyle { get; set; }
        HorizontalAlignment HorizontalAlignment { get; set; }
        ContextMenu ContextMenu { get; set; }
        object ToolTip { get; set; }
        DependencyObject Parent { get; }
        bool IsInitialized { get; }
        bool ForceCursor { get; set; }
        Cursor Cursor { get; set; }
        Style FocusVisualStyle { get; set; }
        bool IsLoaded { get; }

        FontStyle FontStyle { get; set; }
        FontStretch FontStretch { get; set; }
        double FontSize { get; set; }
        FontFamily FontFamily { get; set; }
        Brush Foreground { get; set; }
        Brush Background { get; set; }
        Thickness BorderThickness { get; set; }
        bool IsTabStop { get; set; }
        VerticalAlignment VerticalContentAlignment { get; set; }
        int TabIndex { get; set; }
        Thickness Padding { get; set; }
        ControlTemplate Template { get; set; }
        FontWeight FontWeight { get; set; }
        Brush BorderBrush { get; set; }
        HorizontalAlignment HorizontalContentAlignment { get; set; }

        
        DataTemplate ContentTemplate { get; set; }
        bool HasContent { get; }
        object Content { get; set; }
        string ContentStringFormat { get; set; }
        DataTemplateSelector ContentTemplateSelector { get; set; }

        bool IsActive { get; }
        bool ShowInTaskbar { get; set; }
        WindowStartupLocation WindowStartupLocation { get; set; }
        Rect RestoreBounds { get; }
        TaskbarItemInfo TaskbarItemInfo { get; set; }
        double Top { get; set; }
        SizeToContent SizeToContent { get; set; }
        bool AllowsTransparency { get; set; }
        Window Owner { get; set; }
        double Left { get; set; }
        WindowCollection OwnedWindows { get; }
        ImageSource Icon { get; set; }
        WindowStyle WindowStyle { get; set; }
        WindowState WindowState { get; set; }
        ResizeMode ResizeMode { get; set; }
        bool Topmost { get; set; }
        bool ShowActivated { get; set; }
        string Title { get; set; }
        bool? DialogResult { get; set; }
        
        event EventHandler<TouchEventArgs> TouchMove;
        event EventHandler<TouchEventArgs> PreviewTouchMove;
        event EventHandler<TouchEventArgs> TouchDown;
        event EventHandler<TouchEventArgs> PreviewTouchDown;
        event DragEventHandler Drop;
        event DragEventHandler PreviewDrop;
        event DragEventHandler DragLeave;
        event DragEventHandler PreviewDragLeave;
        event DragEventHandler DragOver;
        event DragEventHandler PreviewDragOver;
        event DragEventHandler DragEnter;
        event DragEventHandler PreviewDragEnter;
        event GiveFeedbackEventHandler GiveFeedback;
        event GiveFeedbackEventHandler PreviewGiveFeedback;
        event QueryContinueDragEventHandler QueryContinueDrag;
        event QueryContinueDragEventHandler PreviewQueryContinueDrag;
        event EventHandler<TouchEventArgs> PreviewTouchUp;
        event EventHandler<TouchEventArgs> TouchUp;
        event EventHandler<TouchEventArgs> LostTouchCapture;
        event EventHandler<ManipulationInertiaStartingEventArgs> ManipulationInertiaStarting;
        event EventHandler<ManipulationDeltaEventArgs> ManipulationDelta;
        event EventHandler<ManipulationStartedEventArgs> ManipulationStarted;
        event EventHandler<ManipulationStartingEventArgs> ManipulationStarting;
        event DependencyPropertyChangedEventHandler FocusableChanged;
        event DependencyPropertyChangedEventHandler IsVisibleChanged;
        event DependencyPropertyChangedEventHandler IsHitTestVisibleChanged;
        event DependencyPropertyChangedEventHandler IsEnabledChanged;
        event RoutedEventHandler LostFocus;
        event EventHandler<TouchEventArgs> GotTouchCapture;
        event RoutedEventHandler GotFocus;
        event DependencyPropertyChangedEventHandler IsKeyboardFocusedChanged;
        event DependencyPropertyChangedEventHandler IsStylusCaptureWithinChanged;
        event DependencyPropertyChangedEventHandler IsStylusDirectlyOverChanged;
        event DependencyPropertyChangedEventHandler IsMouseCaptureWithinChanged;
        event DependencyPropertyChangedEventHandler IsMouseCapturedChanged;
        event DependencyPropertyChangedEventHandler IsKeyboardFocusWithinChanged;
        event DependencyPropertyChangedEventHandler IsMouseDirectlyOverChanged;
        event EventHandler<TouchEventArgs> TouchLeave;
        event EventHandler<TouchEventArgs> TouchEnter;
        event EventHandler LayoutUpdated;
        event MouseButtonEventHandler PreviewMouseDown;
        event MouseButtonEventHandler MouseDown;
        event MouseButtonEventHandler PreviewMouseUp;
        event MouseButtonEventHandler MouseUp;
        event QueryCursorEventHandler QueryCursor;
        event DependencyPropertyChangedEventHandler IsStylusCapturedChanged;
        event EventHandler<ManipulationCompletedEventArgs> ManipulationCompleted;
        event EventHandler<ManipulationBoundaryFeedbackEventArgs> ManipulationBoundaryFeedback;
        event ToolTipEventHandler ToolTipClosing;
        event ToolTipEventHandler ToolTipOpening;
        event RoutedEventHandler Unloaded;
        event DependencyPropertyChangedEventHandler DataContextChanged;
        event SizeChangedEventHandler SizeChanged;
        event RequestBringIntoViewEventHandler RequestBringIntoView;
        event EventHandler<DataTransferEventArgs> SourceUpdated;
        event EventHandler<DataTransferEventArgs> TargetUpdated;
        event RoutedEventHandler Loaded;
        event EventHandler Initialized;
        event ContextMenuEventHandler ContextMenuClosing;
        event ContextMenuEventHandler ContextMenuOpening;
        event MouseButtonEventHandler MouseDoubleClick;
        event MouseButtonEventHandler PreviewMouseDoubleClick;
        event EventHandler Activated;
        event EventHandler Deactivated;
        event EventHandler StateChanged;
        event EventHandler LocationChanged;
        event CancelEventHandler Closing;
        event EventHandler SourceInitialized;
        event EventHandler ContentRendered;
        event EventHandler Closed;

        bool CheckAccess();
        void VerifyAccess();
        void ClearValue(DependencyProperty dp);
        void ClearValue(DependencyPropertyKey key);
        void CoerceValue(DependencyProperty dp);
        LocalValueEnumerator GetLocalValueEnumerator();
        object GetValue(DependencyProperty dp);
        void InvalidateProperty(DependencyProperty dp);
        object ReadLocalValue(DependencyProperty dp);
        void SetCurrentValue(DependencyProperty dp, object value);
        void SetValue(DependencyProperty dp, object value);
        void SetValue(DependencyPropertyKey key, object value);
        DependencyObject FindCommonVisualAncestor(DependencyObject otherVisual);
        bool IsAncestorOf(DependencyObject descendant);
        bool IsDescendantOf(DependencyObject ancestor);
        Point PointFromScreen(Point point);
        Point PointToScreen(Point point);
        GeneralTransform2DTo3D TransformToAncestor(Visual3D ancestor);
        GeneralTransform TransformToAncestor(Visual ancestor);
        GeneralTransform TransformToDescendant(Visual descendant);
        GeneralTransform TransformToVisual(Visual visual);
        void AddHandler(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo);
        void AddToEventRoute(EventRoute route, RoutedEventArgs e);
        void Arrange(Rect finalRect);
        bool CaptureTouch(TouchDevice touchDevice);
        IInputElement InputHitTest(Point point);
        void InvalidateArrange();
        void InvalidateMeasure();
        void InvalidateVisual();
        void Measure(Size availableSize);
        bool MoveFocus(TraversalRequest request);
        DependencyObject PredictFocus(FocusNavigationDirection direction);
        void ReleaseAllTouchCaptures();
        bool ReleaseTouchCapture(TouchDevice touchDevice);
        bool ShouldSerializeCommandBindings();
        bool ShouldSerializeInputBindings();
        Point TranslatePoint(Point point, UIElement relativeTo);
        void UpdateLayout();
        bool ApplyTemplate();
        void BeginStoryboard(Storyboard storyboard, HandoffBehavior handoffBehavior, bool isControllable);
        void BeginStoryboard(Storyboard storyboard);
        void BeginStoryboard(Storyboard storyboard, HandoffBehavior handoffBehavior);
        void BringIntoView();
        void BringIntoView(Rect targetRectangle);
        object FindName(string name);
        object FindResource(object resourceKey);
        BindingExpression GetBindingExpression(DependencyProperty dp);
        void OnApplyTemplate();
        void RegisterName(string name, object scopedElement);
        BindingExpressionBase SetBinding(DependencyProperty dp, BindingBase binding);
        BindingExpression SetBinding(DependencyProperty dp, string path);
        void SetResourceReference(DependencyProperty dp, object name);
        bool ShouldSerializeResources();
        bool ShouldSerializeStyle();
        bool ShouldSerializeTriggers();
        object TryFindResource(object resourceKey);
        void UnregisterName(string name);
        void UpdateDefaultStyle();
        bool Activate();
        void Close();
        void DragMove();
        void Hide();
        void Show();
        bool? ShowDialog();

        #endregion .NETFramework
    }
}
