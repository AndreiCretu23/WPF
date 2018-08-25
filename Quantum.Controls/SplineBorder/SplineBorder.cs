using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Quantum.Controls
{
    public class SplineBorder : Control
    {

        public SplineBorder()
        {
        }


        #region Thickness
        
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(SplineBorder),
                new FrameworkPropertyMetadata((double)1.0, FrameworkPropertyMetadataOptions.AffectsRender));
        
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        #endregion

        #region Fill
        
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(SplineBorder),
                new FrameworkPropertyMetadata((Brush)null, FrameworkPropertyMetadataOptions.AffectsRender));
        
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        #endregion

        #region Stroke
        
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(SplineBorder),
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
        
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        #endregion

        #region BottomBorderMargin
        
        public static readonly DependencyProperty BottomBorderMarginProperty =
            DependencyProperty.Register("BottomBorderMargin", typeof(double), typeof(SplineBorder),
                new FrameworkPropertyMetadata((double)0.0, FrameworkPropertyMetadataOptions.AffectsRender));
        
        public double BottomBorderMargin
        {
            get { return (double)GetValue(BottomBorderMarginProperty); }
            set { SetValue(BottomBorderMarginProperty, value); }
        }

        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {

            var pgFill = new PathGeometry();
            var pfFill = new PathFigure() { IsFilled = true, IsClosed = true };
            pfFill.StartPoint = new Point(0.0, 0.0);

            var q1Fill = new QuadraticBezierSegment() { Point1 = new Point(RenderSize.Width / 3, RenderSize.Height / 10.0), Point2 = new Point(RenderSize.Width, RenderSize.Height), IsStroked = false };
            pfFill.Segments.Add(q1Fill);

            pfFill.Segments.Add(new LineSegment() { Point = new Point(0, RenderSize.Height), IsStroked = false });

            pgFill.Figures.Add(pfFill);

            drawingContext.DrawGeometry(Fill, null, pgFill);

            var pgBorder = new PathGeometry();
            var pfBorder = new PathFigure() { IsFilled = false, IsClosed = false };
            pfBorder.StartPoint = new Point(0.0, Thickness / 2);

            var q1Border = new QuadraticBezierSegment() { Point1 = new Point(RenderSize.Width / 3, RenderSize.Height / 10.0), Point2 = new Point(RenderSize.Width, RenderSize.Height) };
            pfBorder.Segments.Add(q1Border);

            pfFill.Segments.Add(new LineSegment() { Point = new Point(0, RenderSize.Height), IsStroked = false });

            pgBorder.Figures.Add(pfBorder);

            drawingContext.DrawGeometry(null, new Pen(Stroke, Thickness), pgBorder);

            base.OnRender(drawingContext);
        }
    }
}
