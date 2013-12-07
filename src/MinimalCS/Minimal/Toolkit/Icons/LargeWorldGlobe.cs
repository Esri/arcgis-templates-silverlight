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
    [TemplatePart(Name = "OceanEllipse", Type = typeof(Ellipse))]
    [TemplatePart(Name = "LandPath", Type = typeof(Path))]
    [TemplatePart(Name = "GlobeScale", Type = typeof(ScaleTransform))]
    public class LargeWorldGlobe : Control
    {
        Ellipse OceanEllipse;
        Path LandPath;
        Brush oceanFill;
        Brush landFill;
        ScaleTransform GlobeScale;
        double scale = 1;
        LargeWorldGlobe world;
        bool expandOnMouseOver = false;

        public LargeWorldGlobe()
        {
            this.DefaultStyleKey = typeof(LargeWorldGlobe);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes (such as a rebuilding layout pass) call
        /// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            world = this;
            OceanEllipse = GetTemplateChild("OceanEllipse") as Ellipse;
            LandPath = GetTemplateChild("LandPath") as Path;
            GlobeScale = GetTemplateChild("GlobeScale") as ScaleTransform;
            if (oceanFill != null)
                OceanEllipse.Fill = oceanFill;
            if (landFill != null)
                LandPath.Fill = landFill;
            if (scale != 1)
            {
                GlobeScale.ScaleX = scale;
                world.GlobeScale.ScaleY = scale;
            }
            if (expandOnMouseOver)
            {
                world.MouseEnter += globe_MouseEnter;
                world.MouseLeave += globe_MouseLeave;
            }
        }

        public Brush OceanFill
        {
            get { return (Brush)GetValue(OceanFillProperty); }
            set { SetValue(OceanFillProperty, value); }
        }

        public static readonly DependencyProperty OceanFillProperty =
            DependencyProperty.Register("OceanFill", typeof(Brush), typeof(LargeWorldGlobe), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 193, 97, 57)), OnOceanFillPropertyChanged));

        private static void OnOceanFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            LargeWorldGlobe globe = d as LargeWorldGlobe;
            Brush brush = e.NewValue as Brush;
            globe.oceanFill = brush;
            if (globe.OceanEllipse != null)
                globe.OceanEllipse.Fill = brush;
        }

        public Brush LandFill
        {
            get { return (Brush)GetValue(LandFillProperty); }
            set { SetValue(LandFillProperty, value); }
        }

        public static readonly DependencyProperty LandFillProperty =
            DependencyProperty.Register("LandFill", typeof(Brush), typeof(LargeWorldGlobe), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 193, 97, 57)), OnLandFillPropertyChanged));

        private static void OnLandFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LargeWorldGlobe globe = d as LargeWorldGlobe;
            Brush brush = e.NewValue as Brush;
            globe.landFill = brush;
            if (globe.LandPath != null)
                globe.LandPath.Fill = brush;
        }

        public bool ExpandOnMouseOver
        {
            get { return (bool)GetValue(ExpandOnMouseOverProperty); }
            set { SetValue(ExpandOnMouseOverProperty, value); }
        }

        public static readonly DependencyProperty ExpandOnMouseOverProperty =
            DependencyProperty.Register("ExpandOnMouseOver", typeof(bool), typeof(LargeWorldGlobe), new PropertyMetadata(false, OnExpandOnMouseOverChanged));

        private static void OnExpandOnMouseOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LargeWorldGlobe globe = d as LargeWorldGlobe;
            object obj = e.NewValue;
            if (obj != null)
            {
                globe.MouseEnter -= globe.globe_MouseEnter;
                globe.MouseLeave -= globe.globe_MouseLeave;
                bool expand = (bool)obj;
                globe.expandOnMouseOver = expand;
                if (expand)
                {
                    globe.MouseEnter += globe.globe_MouseEnter;
                    globe.MouseLeave += globe.globe_MouseLeave;
                }
            }
        }

        void globe_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation xda = new DoubleAnimation()
            {
                To = scale,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            DoubleAnimation yda = new DoubleAnimation()
            {
                To = scale,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(xda, world.GlobeScale);
            Storyboard.SetTargetProperty(xda, new PropertyPath(ScaleTransform.ScaleXProperty));
            Storyboard.SetTarget(yda, world.GlobeScale);
            Storyboard.SetTargetProperty(yda, new PropertyPath(ScaleTransform.ScaleYProperty));
            sb.Children.Add(xda);
            sb.Children.Add(yda);
            sb.Begin();
        }

        void globe_MouseEnter(object sender, MouseEventArgs e)
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
            Storyboard.SetTarget(xda, world.GlobeScale);
            Storyboard.SetTargetProperty(xda, new PropertyPath(ScaleTransform.ScaleXProperty));
            Storyboard.SetTarget(yda, world.GlobeScale);
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
            DependencyProperty.Register("Scale", typeof(double), typeof(LargeWorldGlobe), new PropertyMetadata(1d, OnScalePropertyChanged));

        private static void OnScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            LargeWorldGlobe globe = d as LargeWorldGlobe;
            double value = Convert.ToDouble(e.NewValue);
            globe.scale = value;
            if (globe.GlobeScale != null)
            {
                globe.GlobeScale.ScaleX = value;
                globe.GlobeScale.ScaleY = value;
            }
        }
    
    }
}
