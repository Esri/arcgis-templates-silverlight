using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ESRI.ArcGIS.SilverlightMapApp
{
        [TemplatePart(Name = "CloseButton", Type = typeof(Button))] //Close button
        [TemplatePart(Name = "ResizeCorner", Type = typeof(UIElement))] //area used for resizing the window
        [TemplatePart(Name = "RightSideTabText", Type = typeof(TextBlock))]
        [TemplatePart(Name = "ContentBorder", Type = typeof(Border))]
        [TemplatePart(Name = "LeftSideTabBorder", Type = typeof(UIElement))]
        [TemplatePart(Name = "RightSideTabBorder", Type = typeof(UIElement))]
        [TemplatePart(Name = "TopBar", Type = typeof(UIElement))]
        [TemplatePart(Name = "BottomBar", Type = typeof(UIElement))]
        [TemplatePart(Name = "LayoutRoot", Type = typeof(UIElement))]
        [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
        [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
        [TemplateVisualState(GroupName = "CommonStates", Name = "Dragging")]
        [TemplateVisualState(GroupName = "CommonStates", Name = "Focus")]
        //[ContentProperty("Content")]
   public class GlassPanel : ContentControl
    {
        GlassPanel glassPanel;
        UIElement ResizeCorner;
        Border contentBorder;
        Point lastPoint;
        TextBlock titleText;
        Button closeButton;
        bool isDragging = false;
        UIElement LeftSideTabBorder;
        UIElement RightSideTabBorder;
        UIElement TopBar;
        UIElement BottomBar;
        UIElement layoutRoot;
        
            public GlassPanel()
        {
            DefaultStyleKey = typeof(GlassPanel);

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            glassPanel = this; 
            layoutRoot = GetTemplateChild("LayoutRoot") as UIElement;
            contentBorder = GetTemplateChild("ContentBorder") as Border;
            if (contentBorder != null)
            {
                //contentBorder.Background = ContentBackground;
                //contentBorder.BorderBrush = ContentBackgroundBorder;
            }
            closeButton = GetTemplateChild("CloseButton") as Button;
            if (closeButton != null)
            {
                //closeButton.Click += (s, e) => { this.IsOpen = false; };
                closeButton.Visibility = IsCloseButtonVisible ? Visibility.Visible : Visibility.Collapsed;
                closeButton.Click += new RoutedEventHandler(closeButton_Click);
                //showCloseButton(IsCloseButtonVisible);
            }


            this.RenderTransform = new TranslateTransform();

            ResizeCorner = GetTemplateChild("ResizeCorner") as UIElement;
            if (ResizeCorner != null)
            {
                if (IsResizeable)
                {
                    ResizeCorner.MouseLeftButtonDown += Resize_MouseLeftButtonDown;
                    ResizeCorner.MouseEnter += ResizeCorner_MouseEnter;
                    ResizeCorner.MouseLeave += ResizeCorner_MouseLeave;
                }
                else
                    ResizeCorner.Visibility = Visibility.Collapsed;

            }
            titleText = GetTemplateChild("RightSideTabText") as TextBlock;
            LeftSideTabBorder = GetTemplateChild("LeftSideTabBorder") as UIElement;
            RightSideTabBorder = GetTemplateChild("RightSideTabBorder") as UIElement;
            TopBar = GetTemplateChild("TopBar") as UIElement;
            BottomBar = GetTemplateChild("BottomBar") as UIElement;
            if (IsDraggable)
            {
                if (TopBar != null)
                    TopBar.MouseLeftButtonDown += DragArea_MouseLeftButtonDown;
                if (BottomBar != null)
                    BottomBar.MouseLeftButtonDown += DragArea_MouseLeftButtonDown;
                if (LeftSideTabBorder != null)
                    LeftSideTabBorder.MouseLeftButtonDown += DragArea_MouseLeftButtonDown;
                if (RightSideTabBorder != null)
                    RightSideTabBorder.MouseLeftButtonDown += DragArea_MouseLeftButtonDown;
            }
            if (LeftSideTabBorder != null)
                LeftSideTabBorder.MouseLeftButtonUp += new MouseButtonEventHandler(LeftSideTabBorder_MouseLeftButtonUp);
        }


        void closeButton_Click(object sender, RoutedEventArgs e)
        {
            glassPanel.Collapse();
        }


        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
                        DependencyProperty.Register("Title", typeof(string), typeof(GlassPanel), null);
        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

           

        /// <summary>
        /// Identifies the <see cref="IsOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
                        DependencyProperty.Register("IsOpen", typeof(bool), typeof(GlassPanel),
                        new PropertyMetadata(true, OnIsOpenPropertyChanged));
        /// <summary>
        /// Gets or sets IsOpen.
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        /// <summary>
        /// IsOpenProperty property changed handler. 
        /// </summary>
        /// <param name="d">Window that changed its IsOpen.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlassPanel gp = d as GlassPanel;
            bool isOpen = (bool)e.NewValue;
            gp.Visibility = isOpen ? Visibility.Visible : Visibility.Collapsed;
            if (isOpen)
                gp.OnOpened();
            else
                gp.OnClosed();
        }

        /// <summary>
        /// Identifies the <see cref="HorizontalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
                        DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(GlassPanel),
                        new PropertyMetadata(0.0, OnHorizontalOffsetPropertyChanged));
        /// <summary>
        /// Gets or sets HorisontalOffset.
        /// </summary>
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }
        /// <summary>
        /// HorisontalOffsetProperty property changed handler. 
        /// </summary>
        /// <param name="d">Window that changed its HorisontalOffset.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnHorizontalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlassPanel gp = d as GlassPanel;
            if (gp.RenderTransform is TranslateTransform)
                (gp.RenderTransform as TranslateTransform).X = (double)e.NewValue;
        }

        /// <summary>
        /// Identifies the <see cref="VerticalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
                        DependencyProperty.Register("VerticalOffset", typeof(double), typeof(GlassPanel),
                        new PropertyMetadata(0.0, OnVerticalOffsetPropertyChanged));
        /// <summary>
        /// Gets or sets VerticalOffset.
        /// </summary>
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }
        /// <summary>
        /// VerticalOffsetProperty property changed handler. 
        /// </summary>
        /// <param name="d">Window that changed its VerticalOffset.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnVerticalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlassPanel gp = d as GlassPanel;
            if (gp.RenderTransform is TranslateTransform)
                (gp.RenderTransform as TranslateTransform).Y = (double)e.NewValue;
        }

        /// <summary>
        /// Identifies the <see cref="IsDraggable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDraggableProperty =
                        DependencyProperty.Register("IsDraggable", typeof(bool), typeof(GlassPanel),
                        new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets IsDraggable.
        /// </summary>
        public bool IsDraggable
        {
            get { return (bool)GetValue(IsDraggableProperty); }
            set { SetValue(IsDraggableProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsWidthResizeable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsResizeableProperty =
                        DependencyProperty.Register("IsWidthResizeable", typeof(bool), typeof(GlassPanel),
                        new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets IsResizeable.
        /// </summary>
        public bool IsResizeable
        {
            get { return (bool)GetValue(IsResizeableProperty); }
            set { SetValue(IsResizeableProperty, value); }
        }

        /// <summary>
        /// IsWidthResizeableProperty property changed handler. 
        /// </summary>
        /// <param name="d">Window that changed its IsWidthResizeable.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnResizeablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlassPanel gp = d as GlassPanel;
            if (gp.ResizeCorner != null)
            {
                gp.ResizeCorner.Visibility = (bool)e.NewValue == true ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsCloseButtonHidden"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsCloseButtonVisibleProperty =
                        DependencyProperty.Register("IsCloseButtonHidden", typeof(bool), typeof(GlassPanel),
                        new PropertyMetadata(true, OnIsCloseButtonVisiblePropertyChanged));

        /// <summary>
        /// Gets or sets IsCloseButtonVisible
        /// </summary>
        public bool IsCloseButtonVisible
        {
            get { return (bool)GetValue(IsCloseButtonVisibleProperty); }
            set { SetValue(IsCloseButtonVisibleProperty, value); }
        }

        private static void OnIsCloseButtonVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlassPanel gp = d as GlassPanel;
            if (gp.closeButton != null)
            {
                gp.closeButton.Visibility = (bool)e.NewValue ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        #endregion

        #region Events
        public event System.EventHandler Closed;
        public event System.EventHandler Opened;

        protected void OnClosed()
        {
            if (Closed != null)
            {
                Closed(this, new System.EventArgs());
            }
        }
        protected void OnOpened()
        {
            if (Opened != null)
            {
                Opened(this, new System.EventArgs());
            }
        }
        #endregion

        #region Action Methods

        private void AnimatedScale(object sender, double scale)
        {
            GlassPanel gp = sender as GlassPanel;
            if (gp.layoutRoot != null)
            {
                // animation storyboard
                Storyboard storyboard = new Storyboard();
                // the growing part
                DoubleAnimation growXAnimation = new DoubleAnimation()
                {
                    To = scale,
                    Duration = TimeSpan.FromSeconds(0.5)
                };
                Storyboard.SetTarget(growXAnimation, ((ScaleTransform)gp.layoutRoot.RenderTransform));
                Storyboard.SetTargetProperty(growXAnimation, new PropertyPath("ScaleX"));
                storyboard.Children.Add(growXAnimation);
                DoubleAnimation growYAnimation = new DoubleAnimation()
                {
                    To = scale,
                    Duration = TimeSpan.FromSeconds(0.5)
                };
                Storyboard.SetTarget(growYAnimation, ((ScaleTransform)gp.layoutRoot.RenderTransform));
                Storyboard.SetTargetProperty(growYAnimation, new PropertyPath("ScaleY"));
                storyboard.Children.Add(growYAnimation);
                if (scale == 0)
                    storyboard.Completed += new EventHandler(gp.ShrinkAnimation_Completed);
                else
                    storyboard.Completed += new EventHandler(gp.ExpandAnimation_Completed);
                storyboard.Begin();
            }
        }

        void ExpandAnimation_Completed(object sender, EventArgs e)
        {
            IsOpen = true;
        }

        void ShrinkAnimation_Completed(object sender, EventArgs e)
        {
            IsOpen = false;
        }

        public void Expand()
        {
            IsOpen = true;
            AnimatedScale(this, 1);
        }
        public void Collapse()
        {
            AnimatedScale(this, 0);
        }

        #endregion

        #region Drag

        void DragArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement elem = sender as UIElement;
            if (!IsDraggable) return;
            lastPoint = e.GetPosition(null);

            if (sender == TopBar) 
                TopBar.CaptureMouse();
            TopBar.MouseMove += RootVisual_MouseMove;
            TopBar.MouseLeftButtonUp += DragArea_MouseLeftButtonUp;
            TopBar.LostMouseCapture += DragArea_LostMouseCapture;

            if (sender == BottomBar)
                BottomBar.CaptureMouse();
            BottomBar.MouseMove += RootVisual_MouseMove;
            BottomBar.MouseLeftButtonUp += DragArea_MouseLeftButtonUp;
            BottomBar.LostMouseCapture += DragArea_LostMouseCapture;

            if (sender == LeftSideTabBorder)
                LeftSideTabBorder.CaptureMouse();
            LeftSideTabBorder.MouseMove += RootVisual_MouseMove;
            LeftSideTabBorder.MouseLeftButtonUp += DragArea_MouseLeftButtonUp;
            LeftSideTabBorder.LostMouseCapture += DragArea_LostMouseCapture;


            if (sender == RightSideTabBorder)
                TopBar.CaptureMouse();
            RightSideTabBorder.MouseMove += RootVisual_MouseMove;
            RightSideTabBorder.MouseLeftButtonUp += DragArea_MouseLeftButtonUp;
            RightSideTabBorder.LostMouseCapture += DragArea_LostMouseCapture;

            e.Handled = true;
        }

        void DragArea_LostMouseCapture(object sender, MouseEventArgs e)
        {
            StopDrag(sender);
        }

        private void DragArea_MouseLeave(object sender, MouseEventArgs e)
        {
            StopDrag(sender);
        }

        private void DragArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StopDrag(sender);
        }

        /// <summary>
        /// Stops tracking window drag.
        /// </summary>
        private void StopDrag(object sender)
        {
            isDragging = false;
            if (sender == TopBar)
                TopBar.ReleaseMouseCapture();
            TopBar.MouseMove -= RootVisual_MouseMove;
            TopBar.MouseLeftButtonUp -= DragArea_MouseLeftButtonUp;
            TopBar.LostMouseCapture -= DragArea_LostMouseCapture;

            if (sender == BottomBar)
                BottomBar.ReleaseMouseCapture();
            BottomBar.MouseMove -= RootVisual_MouseMove;
            BottomBar.MouseLeftButtonUp -= DragArea_MouseLeftButtonUp;
            BottomBar.LostMouseCapture -= DragArea_LostMouseCapture;

            if (sender == RightSideTabBorder)
                RightSideTabBorder.ReleaseMouseCapture();
            RightSideTabBorder.MouseMove -= RootVisual_MouseMove;
            RightSideTabBorder.MouseLeftButtonUp -= DragArea_MouseLeftButtonUp;
            RightSideTabBorder.LostMouseCapture -= DragArea_LostMouseCapture;

            if (sender == LeftSideTabBorder)
                LeftSideTabBorder.ReleaseMouseCapture();
            LeftSideTabBorder.MouseMove -= RootVisual_MouseMove;
            LeftSideTabBorder.MouseLeftButtonUp -= DragArea_MouseLeftButtonUp;
            LeftSideTabBorder.LostMouseCapture -= DragArea_LostMouseCapture;

            //ChangeVisualState(true);
        }

        private void RootVisual_MouseMove(object sender, MouseEventArgs e)
        {
            isDragging = true;
            //ChangeVisualState(true);
            TranslateTransform t = this.RenderTransform as TranslateTransform;
            Point p2 = e.GetPosition(null);
            double dX = p2.X - lastPoint.X;
            double dY = p2.Y - lastPoint.Y;
            HorizontalOffset += dX;
            VerticalOffset += dY;
            lastPoint = p2;
        }

        void LeftSideTabBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region Resize

        private void Resize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (double.IsNaN(this.ActualWidth) || double.IsNaN(this.ActualHeight))
                return;
            lastPoint = e.GetPosition(null);
            Application.Current.RootVisual.MouseMove += Resize_MouseMove;
            Application.Current.RootVisual.MouseLeftButtonUp += Resize_MouseLeftButtonUp;
            Application.Current.RootVisual.MouseLeave += Resize_MouseLeave;
            e.Handled = true;
        }
        private void Resize_MouseMove(object sender, MouseEventArgs e)
        {
            Point p2 = e.GetPosition(null);
                double d = p2.X - lastPoint.X;
                if (this.Width + d >= this.MinWidth)
                    this.Width += d;
                //if (this.HorizontalAlignment == HorizontalAlignment.Center)
                //    this.HorizontalOffset += d / 2;
                //else if (this.HorizontalAlignment == HorizontalAlignment.Right)
                //    this.HorizontalOffset += d;
                d = p2.Y - lastPoint.Y;
                if (this.Height + d >= this.MinHeight) 
                    this.Height += d;
                //if (this.VerticalAlignment == VerticalAlignment.Bottom)
                //    this.VerticalOffset += d;
                //else if (this.VerticalAlignment == VerticalAlignment.Center)
                //    this.VerticalOffset += d / 2;
            lastPoint = p2;

        }
        private void Resize_MouseLeave(object sender, MouseEventArgs e)
        {
            StopResize();
        }
        private void Resize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StopResize();
        }

        private void StopResize()
        {
            Application.Current.RootVisual.MouseMove -= Resize_MouseMove;
            Application.Current.RootVisual.MouseLeftButtonUp -= Resize_MouseLeftButtonUp;
            Application.Current.RootVisual.MouseLeave -= Resize_MouseLeave;
        }

        void ResizeCorner_MouseLeave(object sender, MouseEventArgs e)
        {
            FrameworkElement rc = sender as FrameworkElement;
            rc.Opacity = 0.5;
        }

        void ResizeCorner_MouseEnter(object sender, MouseEventArgs e)
        {
            FrameworkElement rc = sender as FrameworkElement;
            rc.Opacity = 1;
        }

        #endregion

    }


}
