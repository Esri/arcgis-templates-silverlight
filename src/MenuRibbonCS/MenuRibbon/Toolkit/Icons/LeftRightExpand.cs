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
    public class LeftRightExpand : Control
    {
        Polygon arrow;
        ScaleTransform Flip;
        Brush fill;
        Brush stroke;
        bool isPointingRight = true;

        public LeftRightExpand()
        {
            this.DefaultStyleKey = typeof(LeftRightExpand);
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
                Flip.ScaleX = isPointingRight ? 1 : -1;
        }
 
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(LeftRightExpand), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 224, 224, 224)), OnFillPropertyChanged));

        private static void OnFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LeftRightExpand expand = d as LeftRightExpand;
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
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(LeftRightExpand), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 53, 53, 53)), OnStrokePropertyChanged));

        private static void OnStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LeftRightExpand expand = d as LeftRightExpand;
            Brush brush = e.NewValue as Brush;
            expand.stroke = brush;
            if (expand.arrow != null)
                expand.arrow.Stroke = brush;
        }

        public bool IsPointingRight
        {
            get { return (bool)GetValue(IsPointingRightProperty); }
            set { SetValue(IsPointingRightProperty, value); }
        }

        public static readonly DependencyProperty IsPointingRightProperty =
            DependencyProperty.Register("IsPointingRight", typeof(bool), typeof(LeftRightExpand), new PropertyMetadata(true, OnIsPointingRightPropertyChanged));

        private static void OnIsPointingRightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LeftRightExpand expand = d as LeftRightExpand;
            object obj = e.NewValue;
            if (obj != null)
            {
                bool pointingRight = (bool)obj;
                expand.isPointingRight = pointingRight;
                if (expand.Flip != null)
                    expand.Flip.ScaleX = pointingRight ? 1 : -1;
            }
        }

    }
}
