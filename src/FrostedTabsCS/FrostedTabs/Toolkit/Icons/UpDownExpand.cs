using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Toolkit.Icons
{
    [TemplatePart(Name = "arrow", Type = typeof(Polygon))]
    [TemplatePart(Name = "Flip", Type = typeof(ScaleTransform))]
    public class UpDownExpand : Control
    {
        Polygon arrow;
        ScaleTransform Flip;
        Brush fill;
        Brush stroke;
        bool isPointingDown = true;

        public UpDownExpand()
        {
            this.DefaultStyleKey = typeof(UpDownExpand);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes (such as a rebuilding layout pass) call
        /// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            Flip = GetTemplateChild("Flip") as ScaleTransform;
            arrow = GetTemplateChild("arrow") as Polygon;
            if (fill != null)
                arrow.Fill = fill;
            if (stroke != null)
                arrow.Stroke = stroke;
            //if (isPointingRight != null)
            Flip.ScaleY = isPointingDown ? 1 : -1;
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(UpDownExpand), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 224, 224, 224)), OnFillPropertyChanged));

        private static void OnFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpDownExpand expand = d as UpDownExpand;
            Brush brush = e.NewValue as Brush;
            expand.fill = brush;
            if (expand.arrow != null)
                expand.arrow.Fill = brush;
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(UpDownExpand), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 53, 53, 53)), OnStrokePropertyChanged));

        private static void OnStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpDownExpand expand = d as UpDownExpand;
            Brush brush = e.NewValue as Brush;
            expand.stroke = brush;
            if (expand.arrow != null)
                expand.arrow.Stroke = brush;
        }

        public bool IsPointingDown
        {
            get { return (bool)GetValue(IsPointingDownProperty); }
            set { SetValue(IsPointingDownProperty, value); }
        }

        public static readonly DependencyProperty IsPointingDownProperty =
            DependencyProperty.Register("IsPointingDown", typeof(bool), typeof(UpDownExpand), new PropertyMetadata(true, OnIsPointingDownPropertyChanged));

        private static void OnIsPointingDownPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpDownExpand expand = d as UpDownExpand;
            object obj = e.NewValue;
            if (obj != null)
            {
                bool pointingDown = (bool)obj;
                expand.isPointingDown = pointingDown;
                if (expand.Flip != null)
                    expand.Flip.ScaleY = pointingDown ? 1 : -1;
            }
        }

    }
}
