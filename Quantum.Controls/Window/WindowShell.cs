using Quantum.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Quantum.Controls
{
    [TemplatePart(Name = "PART_Header", Type = typeof(Border))]
    [TemplatePart(Name = "PART_Minimize", Type = typeof(Button))]
    [TemplatePart(Name = "PART_State", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Close", Type = typeof(Button))]
    [TemplatePart(Name = "PART_LeftSizeGrip", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_TopSizeGrip", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_RightSizeGrip", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_BottomSizeGrip", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_TopLeftSizeGrip", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_TopRightSizeGrip", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_BottomLeftSizeGrip", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_BottomRightSizeGrip", Type = typeof(Rectangle))]
    public class WindowShell : Window
    {
        static WindowShell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowShell), new FrameworkPropertyMetadata(typeof(WindowShell)));
        }

        public WindowShell()
        {
            
        }

        #region InitializeTemplate

        public override void OnApplyTemplate()
        {
            var headerBorder = Template.FindName("PART_Header", this) as Border;
            var minimizeButton = Template.FindName("PART_Minimize", this) as Button;
            var stateButton = Template.FindName("PART_State", this) as Button;
            var closeButton = Template.FindName("PART_Close", this) as Button;

            InitializeHeaderBorder(headerBorder);
            InitializeMinimizeButton(minimizeButton);
            InitializeStateButton(stateButton);
            InitializeCloseButton(closeButton);
            InitializeGrips();

            base.OnApplyTemplate();
        }


        #endregion InitializeTemplate



        #region Chrome

        
        private void InitializeHeaderBorder(Border headerBorder)
        {
            headerBorder.MouseLeftButtonDown += (sender, e) => HandleDrag(e);
            MouseLeftButtonUp += (sender, e) => IsInDrag = false;
        }


        private bool IsInDrag = false;

        private void HandleDrag(MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                SwitchStates();
            }
            else if (WindowState == WindowState.Maximized)
            {
                HandleMaximizedDrag();
            }
            else if (WindowState == WindowState.Normal)
            {
                HandleNormalDrag();
            }
        }

        private void HandleNormalDrag()
        {
            IsInDrag = true;
            DragMove();
        }

        private void HandleMaximizedDrag()
        {
            if(!IsInDrag)
            {
                var percentageHorizontal = PointToScreen(Mouse.GetPosition(this)).X / ActualWidth;
                var targetHorizontal = RestoreBounds.Width * percentageHorizontal;

                var percentageVertival = PointToScreen(Mouse.GetPosition(this)).Y / ActualHeight;
                var targetVertical = RestoreBounds.Height * percentageVertival;

                WindowState = WindowState.Normal;

                var cursorPosition = UINativeUtils.GetCursorPos();
                
                Left = cursorPosition.X - targetHorizontal;
                Top = cursorPosition.Y - targetVertical;

                IsInDrag = true;
                DragMove();
            }
        }
        
        private void SwitchStates()
        {
            if(WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else if(WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
        }



        #endregion Chrome



        #region Buttons

        protected virtual void InitializeMinimizeButton(Button minimizeButton)
        {
            minimizeButton.Click += (sender, e) => WindowState = WindowState.Minimized;
        }

        protected virtual void InitializeStateButton(Button stateButton)
        {
            stateButton.Click += (sender, e) =>
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
                else if (WindowState == WindowState.Normal)
                {
                    WindowState = WindowState.Maximized;
                }
            };
        }

        protected virtual void InitializeCloseButton(Button closeButton)
        {
            closeButton.Click += (sender, e) => Close();
        }


        #endregion ActionButtons
        


        #region Resize

        private void InitializeGrips()
        {
            var partGrips = new Collection<Rectangle>();

            partGrips.Add(Template.FindName("PART_LeftSizeGrip", this) as Rectangle);
            partGrips.Add(Template.FindName("PART_TopSizeGrip", this) as Rectangle);
            partGrips.Add(Template.FindName("PART_RightSizeGrip", this) as Rectangle);
            partGrips.Add(Template.FindName("PART_BottomSizeGrip", this) as Rectangle);

            partGrips.Add(Template.FindName("PART_TopLeftSizeGrip", this) as Rectangle);
            partGrips.Add(Template.FindName("PART_TopRightSizeGrip", this) as Rectangle);
            partGrips.Add(Template.FindName("PART_BottomLeftSizeGrip", this) as Rectangle);
            partGrips.Add(Template.FindName("PART_BottomRightSizeGrip", this) as Rectangle);

            foreach(var grip in partGrips)
            {
                grip.MouseLeftButtonDown += Resize_Init;
                grip.MouseLeftButtonUp += Resize_End;
                grip.MouseMove += Resizing_Form;
            }
        }

        bool ResizeInProcess = false;

        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            if(ResizeMode == ResizeMode.CanResizeWithGrip)
            {
                var senderRect = sender as Rectangle;
                if (senderRect != null)
                {
                    ResizeInProcess = true;
                    senderRect.CaptureMouse();
                }
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            if (ResizeMode == ResizeMode.CanResizeWithGrip)
            {
                var senderRect = sender as Rectangle;
                if (senderRect != null)
                {
                    ResizeInProcess = false; ;
                    senderRect.ReleaseMouseCapture();
                }
            }            
        }

        private void Resizing_Form(object sender, MouseEventArgs e)
        {
            if (ResizeMode == ResizeMode.CanResizeWithGrip)
            {
                if (ResizeInProcess)
                {
                    var senderRect = sender as Rectangle;
                    if (senderRect != null)
                    {
                        var width = e.GetPosition(this).X;
                        var height = e.GetPosition(this).Y;
                        senderRect.CaptureMouse();
                        if (senderRect.Name.ToLower().Contains("right"))
                        {
                            width += 5;
                            if (width > 0)
                            {
                                Width = width;
                            }
                        }
                        if (senderRect.Name.ToLower().Contains("left"))
                        {
                            width -= 5;
                            Left += width;
                            width = Width - width;
                            if (width > 0)
                            {
                                Width = width;
                            }
                        }
                        if (senderRect.Name.ToLower().Contains("bottom"))
                        {
                            height += 5;
                            if (height > 0)
                            {
                                Height = height;
                            }
                        }
                        if (senderRect.Name.ToLower().Contains("top"))
                        {
                            height -= 5;
                            Top += height;
                            height = Height - height;
                            if (height > 0)
                            {
                                Height = height;
                            }
                        }
                    }
                }
            }
        }

        #endregion Resize


        
    }
}
