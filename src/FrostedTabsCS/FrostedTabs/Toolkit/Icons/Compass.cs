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
    [TemplatePart(Name = "CompassScale", Type = typeof(ScaleTransform))]
    [TemplatePart(Name = "compassFace", Type = typeof(Ellipse))]
    public class Compass : Control
    {
        Compass compass;
        ScaleTransform CompassScale;
        double scale = 1;
        bool expandOnMouseOver = false;
        Ellipse CompassFace;
        Brush faceFill;

        public Compass()
        {
            this.DefaultStyleKey = typeof(Compass);
        }

        /// <summary>
		/// When overridden in a derived class, is invoked whenever application code or
		/// internal processes (such as a rebuilding layout pass) call
		/// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
        public override void OnApplyTemplate()
        {
            compass = this;
            CompassScale = GetTemplateChild("CompassScale") as ScaleTransform;
            CompassFace = GetTemplateChild("compassFace") as Ellipse;
            if (scale != 1)
            {
                compass.CompassScale.ScaleX = scale;
                compass.CompassScale.ScaleY = scale;
            }
            if (expandOnMouseOver)
            {
                compass.MouseEnter += new MouseEventHandler(compass_MouseEnter);
                compass.MouseLeave += new MouseEventHandler(compass_MouseLeave);
            }
            if (faceFill != null)
                CompassFace.Fill = faceFill;
        }

        void compass_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation xda = new DoubleAnimation()
            {
                To=scale,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            DoubleAnimation yda = new DoubleAnimation()
            {
                To = scale,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(xda, compass.CompassScale);
            Storyboard.SetTargetProperty(xda, new PropertyPath(ScaleTransform.ScaleXProperty));
            Storyboard.SetTarget(yda, compass.CompassScale);
            Storyboard.SetTargetProperty(yda, new PropertyPath(ScaleTransform.ScaleYProperty));
            sb.Children.Add(xda);
            sb.Children.Add(yda);
            sb.Begin();
        }

        void compass_MouseEnter(object sender, MouseEventArgs e)
        {
            double expandScale = scale * 1.25;
            DoubleAnimation xda = new DoubleAnimation()
            {
                To = expandScale,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            DoubleAnimation yda = new DoubleAnimation()
            {
                To = expandScale,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(xda, compass.CompassScale);
            Storyboard.SetTargetProperty(xda, new PropertyPath(ScaleTransform.ScaleXProperty));
            Storyboard.SetTarget(yda, compass.CompassScale);
            Storyboard.SetTargetProperty(yda, new PropertyPath(ScaleTransform.ScaleYProperty));
            sb.Children.Add(xda);
            sb.Children.Add(yda);
            sb.Begin();
        }

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(Compass), new PropertyMetadata(1d, OnScalePropertyChanged));

        private static void OnScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            Compass compass = d as Compass;
            double value = Convert.ToDouble(e.NewValue);
            compass.scale = value;
            if (compass.CompassScale != null)
            {
                compass.CompassScale.ScaleX = value;
                compass.CompassScale.ScaleY = value;
            }
        }

        public bool ExpandOnMouseOver
        {
            get { return (bool)GetValue(ExpandOnMouseOverProperty); }
            set { SetValue(ExpandOnMouseOverProperty, value); }
        }

        public static readonly DependencyProperty ExpandOnMouseOverProperty =
            DependencyProperty.Register("ExpandOnMouseOver", typeof(bool), typeof(Compass), new PropertyMetadata(false, OnExpandOnMouseOverChanged));

        private static void OnExpandOnMouseOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Compass compass = d as Compass;
            object obj = e.NewValue;
            if (obj != null)
            {
                compass.MouseEnter -= compass.compass_MouseEnter;
                compass.MouseLeave -= compass.compass_MouseLeave;
                bool expand = (bool)obj;
                compass.expandOnMouseOver = expand;
                if (expand)
                {
                    compass.MouseEnter += compass.compass_MouseEnter;
                    compass.MouseLeave += compass.compass_MouseLeave;
                }
            }
        }

        public Brush FaceFill
        {
            get { return (Brush)GetValue(FaceFillProperty); }
            set { SetValue(FaceFillProperty, value); }
        }

        public static readonly DependencyProperty FaceFillProperty =
            DependencyProperty.Register("FaceFill", typeof(Brush), typeof(Compass), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 74, 119, 234)), OnFaceFillPropertyChanged));

        private static void OnFaceFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            Compass compass = d as Compass;
            Brush brush = e.NewValue as Brush;
            compass.faceFill = brush;
            if (compass.CompassFace != null)
                compass.CompassFace.Fill = brush;
        }

    }
}
