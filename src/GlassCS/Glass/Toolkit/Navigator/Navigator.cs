/*
Copyright 2013 Esri
Licensed under the Apache License, Version 2.0 (the "License");
You may not use this file except in compliance with the License.
You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Toolkit;
using Toolkit.Icons;

namespace Toolkit
{
    /// <summary>
    /// Navigator control supporting pan, zoom and rotation.
    /// </summary>
    [TemplatePart(Name = "ResetRotationGrid", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "NavProjection", Type = typeof(PlaneProjection))]
    [TemplatePart(Name = "SlantSlider", Type = typeof(Slider))]
    [TemplatePart(Name = "SlantSliderGrid", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "ZoomGrid_", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "ZoomGridBorder1", Type = typeof(Border))]
    [TemplatePart(Name = "ZoomGridBorder2", Type = typeof(Border))]
    [TemplatePart(Name = "SlantText_", Type = typeof(TextBlock))]
    [TemplatePart(Name = "GlobeGlass", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "ZoomGridStack", Type = typeof(StackPanel))]
    [TemplatePart(Name = "BottomZoomWingUpDownExpand", Type = typeof(UpDownExpand))]
    [TemplatePart(Name = "TopZoomWingUpDownExpand", Type = typeof(UpDownExpand))]
    [TemplatePart(Name = "TopZoomWingCollapseButton", Type = typeof(Button))]
    [TemplatePart(Name = "BottomZoomWingCollapseButton", Type = typeof(Button))]
    [TemplatePart(Name = "ExpandZoomBar", Type = typeof(Storyboard))]
    [TemplatePart(Name = "ShrinkZoomBar", Type = typeof(Storyboard))]
    [TemplatePart(Name = "ExpandProgressBarSpacer", Type = typeof(Storyboard))]
    [TemplatePart(Name = "ShrinkProgressBarSpacer", Type = typeof(Storyboard))]
    [TemplatePart(Name = "ProgressBarSpacer", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "ScaleBarBlock", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PaddingLeftForScaleBarTextMeters", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PaddingLeftTopNotch", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PaddingLeftForScaleBarTextMiles", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PaddingLeftBottomNotch", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "ScaleBarTextForMeters", Type = typeof(TextBlock))]
    [TemplatePart(Name = "ScaleBarTextForMiles", Type = typeof(TextBlock))]
    [TemplatePart(Name = "saleBar", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "root", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "NavOpacitySliderGrid", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "NavOpacitySlider", Type = typeof(Slider))]
    [TemplatePart(Name = "OpacityText", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "progressBarGrid", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "ScaleBarInnerBorder", Type = typeof(Border))]
    [TemplatePart(Name = "ScaleBarWingCollapseButton", Type = typeof(Button))]
    [TemplatePart(Name = "ScaleBarWingCollapseImage", Type = typeof(Image))]
    [TemplatePart(Name = "ScaleBarOuterBorder", Type = typeof(Border))]
    [TemplatePart(Name = "progressBar", Type = typeof(MapProgressBar))]
    [TemplatePart(Name = "ScaleBarWingLeftRightExpand", Type = typeof(LeftRightExpand))]
    public class Navigator : Navigation
    {
        #region private fields

        FrameworkElement ResetRotationGrid;
        FrameworkElement ScaleBarBlock;
        FrameworkElement PaddingLeftForScaleBarTextMeters;
        TextBlock ScaleBarTextForMeters;
        FrameworkElement PaddingLeftTopNotch;
        FrameworkElement PaddingLeftForScaleBarTextMiles;
        FrameworkElement PaddingLeftBottomNotch;
        FrameworkElement ScaleBar;
        FrameworkElement root;
        FrameworkElement NavOpacitySliderGrid;
        Slider NavOpacitySlider;
        FrameworkElement OpacityText;
        FrameworkElement progressBarGrid;
        Border ScaleBarInnerBorder;
        Button ScaleBarWingCollapseButton;
        Image ScaleBarWingCollapseImage;
        Border ScaleBarOuterBorder;
        MapProgressBar progressBar;

        TextBlock ScaleBarTextForMiles;

        SolidColorBrush levelGreaterThanBrush;
        SolidColorBrush levelLessThanBrush;
        SolidColorBrush levelEqualsBrush;

        PlaneProjection projection;
        PlaneProjection NavProjection;
        Slider SlantSlider;
        FrameworkElement SlantSliderGrid;
        Border NavCircle;
        ScaleTransform NavCircleScale;
        FrameworkElement ZoomGrid;
        Border ZoomGridOuterBorder;
        Border ZoomGridInnerBorder;
        TextBlock SlantText;
        FrameworkElement SlantGlass;
        StackPanel ZoomGridStack;
        UpDownExpand BottomZoomWingUpDownExpand;
        UpDownExpand TopZoomWingUpDownExpand;
        Button TopZoomCollapseButton;
        Button BottomZoomCollapseButton;
        Storyboard ExpandZoomBar;
        Storyboard ShrinkZoomBar;
        Storyboard ExpandProgressBarSpacer;
        Storyboard ShrinkProgressBarSpacer;
        FrameworkElement ProgressBarSpacer;
        LeftRightExpand ScaleBarWingLeftRightExpand;

        Navigator navControl;
        MapPoint mapCenter;
        Point startPoint;
        double lastY = 0;
        bool bottomAligned = false;
        bool usePlaneProjection = false;
        double mapAngle = 0;
        double mapWidth = 0;
        double mapHeight = 0;

        public PlaneProjection MapProjection { get; set; }


        #endregion

        public Navigator()
        {
            this.DefaultStyleKey = typeof(Navigator);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes (such as a rebuilding layout pass) call
        /// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            navControl = this;
            base.OnApplyTemplate();
            ResetRotationGrid = GetTemplateChild("ResetRotationGrid") as FrameworkElement;
            projection = GetTemplateChild("NavProjection") as PlaneProjection;
            SlantSlider = GetTemplateChild("SlantSlider") as Slider;
            SlantSliderGrid = GetTemplateChild("SlantSliderGrid") as FrameworkElement;
            NavCircle = GetTemplateChild("NavCircle") as Border;
            NavCircleScale = GetTemplateChild("NavCircleScale") as ScaleTransform;
            ZoomGrid = GetTemplateChild("ZoomGrid_") as FrameworkElement;
            ZoomGridOuterBorder = GetTemplateChild("ZoomGridBorder1") as Border;
            ZoomGridInnerBorder = GetTemplateChild("ZoomGridBorder2") as Border;
            SlantText = GetTemplateChild("SlantText_") as TextBlock;
            SlantGlass = GetTemplateChild("GlobeGlass") as FrameworkElement;
            //ZoomLevelStack = GetTemplateChild("ZoomLevelStack") as StackPanel;
            ZoomGridStack = GetTemplateChild("ZoomGridStack") as StackPanel;
            BottomZoomWingUpDownExpand = GetTemplateChild("BottomZoomWingUpDownExpand") as UpDownExpand;
            TopZoomWingUpDownExpand = GetTemplateChild("TopZoomWingUpDownExpand") as UpDownExpand;
            BottomZoomCollapseButton = GetTemplateChild("BottomZoomWingCollapseButton") as Button;
            TopZoomCollapseButton = GetTemplateChild("TopZoomWingCollapseButton") as Button;
            ExpandZoomBar = GetTemplateChild("ExpandZoomBar") as Storyboard;
            ShrinkZoomBar = GetTemplateChild("ShrinkZoomBar") as Storyboard;
            ExpandProgressBarSpacer = GetTemplateChild("ExpandProgressBarSpacer") as Storyboard;
            ShrinkProgressBarSpacer = GetTemplateChild("ShrinkProgressBarSpacer") as Storyboard;
            ProgressBarSpacer = GetTemplateChild("ProgressBarSpacer") as FrameworkElement;
            NavProjection = GetTemplateChild("NavProjection") as PlaneProjection;

            ScaleBarBlock = GetTemplateChild("ScaleBarBlock") as FrameworkElement;
            PaddingLeftForScaleBarTextMeters = GetTemplateChild("PaddingLeftForScaleBarTextMeters") as FrameworkElement;
            PaddingLeftTopNotch = GetTemplateChild("PaddingLeftTopNotch") as FrameworkElement;
            PaddingLeftForScaleBarTextMiles = GetTemplateChild("PaddingLeftForScaleBarTextMiles") as FrameworkElement;
            PaddingLeftBottomNotch = GetTemplateChild("PaddingLeftBottomNotch") as FrameworkElement;
            ScaleBarTextForMeters = GetTemplateChild("ScaleBarTextForMeters") as TextBlock;
            ScaleBarTextForMiles = GetTemplateChild("ScaleBarTextForMiles") as TextBlock;
            ScaleBar = GetTemplateChild("scaleBar") as FrameworkElement;
            root = GetTemplateChild("root") as FrameworkElement;
            NavOpacitySliderGrid = GetTemplateChild("NavOpacitySliderGrid") as FrameworkElement;
            NavOpacitySlider = GetTemplateChild("NavOpacitySlider") as Slider;
            OpacityText = GetTemplateChild("OpacityText") as FrameworkElement;
            progressBarGrid = GetTemplateChild("progressBarGrid") as FrameworkElement;
            ScaleBarInnerBorder = GetTemplateChild("ScaleBarInnerBorder") as Border;
            ScaleBarWingCollapseButton = GetTemplateChild("ScaleBarWingCollapseButton") as Button;
            ScaleBarWingCollapseImage = GetTemplateChild("ScaleBarWingCollapseImage") as Image;
            ScaleBarOuterBorder = GetTemplateChild("ScaleBarOuterBorder") as Border;
            progressBar = GetTemplateChild("progressBar") as MapProgressBar;
            ScaleBarWingLeftRightExpand = GetTemplateChild("ScaleBarWingLeftRightExpand") as LeftRightExpand;

            if (ExpandProgressBarSpacer != null)
                ExpandProgressBarSpacer.Completed += expandprogressbarSpacer_Completed;
            if (ExpandZoomBar != null)
                ExpandZoomBar.Completed += ExpandZoomBar_Completed;
            if (ShrinkZoomBar != null)
                ShrinkZoomBar.Completed += ShrinkZoomBar_Completed;
            if (ScaleBarWingCollapseButton != null)
                ScaleBarWingCollapseButton.Click += ScaleBarWingCollapseButton_Click;
            if (BottomZoomCollapseButton != null)
                BottomZoomCollapseButton.Click += ZoomWingCollapseButton_Click;
            if (TopZoomCollapseButton != null)
                TopZoomCollapseButton.Click += ZoomWingCollapseButton_Click;
            if (SlantGlass != null)
                SlantGlass.MouseLeftButtonDown += GlobeGlass_MouseLeftButtonDown;
            if (SlantSlider != null)
                SlantSlider.ValueChanged += SlantSlider_ValueChanged;
            if (NavOpacitySlider != null)
                NavOpacitySlider.ValueChanged += NavOpacitySlider_ValueChanged;
            ResourceDictionary rd = root.Resources;
            // try local resources first for zoom level brushes
            levelGreaterThanBrush = rd["levelGreaterThanBrush"] as SolidColorBrush;
            levelLessThanBrush = rd["levelLessThanBrush"] as SolidColorBrush;
            levelEqualsBrush = rd["levelEqualsBrush"] as SolidColorBrush;
            if (levelGreaterThanBrush == null)
            {
                // if local resources not used, try the application global resources
                rd = Application.Current.Resources;
                levelGreaterThanBrush = rd["levelGreaterThanBrush"] as SolidColorBrush;
                levelLessThanBrush = rd["levelLessThanBrush"] as SolidColorBrush;
                levelEqualsBrush = rd["levelEqualsBrush"] as SolidColorBrush;
            }

            VerticalAlignment va = this.VerticalAlignment;
            if (va == VerticalAlignment.Bottom)
            {
                bottomAligned = true;
                ScaleBarOuterBorder.CornerRadius = new CornerRadius(0, 21, 0, 0);
                ScaleBarOuterBorder.VerticalAlignment = VerticalAlignment.Bottom;
                ScaleBarInnerBorder.CornerRadius = new CornerRadius(0, 20, 0, 0);
                OpacityText.VerticalAlignment = VerticalAlignment.Bottom;
                NavOpacitySliderGrid.VerticalAlignment = VerticalAlignment.Bottom;

                ZoomGridOuterBorder.CornerRadius = new CornerRadius(0, 21, 0, 0);
                ZoomGridInnerBorder.CornerRadius = new CornerRadius(0, 20, 0, 0);
                ZoomGrid.VerticalAlignment = VerticalAlignment.Bottom;
                ZoomGridOuterBorder.VerticalAlignment = VerticalAlignment.Top;
                ZoomGridInnerBorder.Padding = new Thickness(5, 5, 5, 110);
                NavCircle.VerticalAlignment = VerticalAlignment.Bottom;
                NavCircle.RenderTransformOrigin = new Point(0, 1);
                SlantSliderGrid.Margin = new Thickness(85, 0, 0, 6);
                SlantSliderGrid.VerticalAlignment = VerticalAlignment.Bottom;
                SlantText.VerticalAlignment = VerticalAlignment.Bottom;
                ResetRotationGrid.VerticalAlignment = VerticalAlignment.Bottom;
                BottomZoomCollapseButton.Visibility = Visibility.Collapsed;
                TopZoomCollapseButton.Visibility = Visibility.Visible;
            }
            string strvalue = Application.Current.Resources["UsePlaneProjection"] as string;
            if (strvalue != null)
                usePlaneProjection = bool.Parse(strvalue);
            if (mapAngle != 0 && usePlaneProjection)
                SlantSlider.Value = mapAngle;
            if (this.Map != null)
            {
                Map.ExtentChanged += new EventHandler<ExtentEventArgs>(Map_ExtentChanged);
                Map.Progress += projectionCheck;
                Map.Progress += progressBarCheck;
            }

        }



        #region Properties



        /// <summary>
        /// Gets or sets angle of map view. Ignored if UseMapProjection is set to False.
        /// </summary>
        public double MapAngle
        {
            get { return (double)GetValue(MapAngleProperty); }
            set { SetValue(MapAngleProperty, value); }
        }

        public static readonly DependencyProperty MapAngleProperty =
            DependencyProperty.Register("MapAngle", typeof(double), typeof(Navigator), new PropertyMetadata(0d, OnMapAnglePropertyChanged));

        private static void OnMapAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Navigator n = d as Navigator;
            double angle = (double)e.NewValue;
            if (!double.IsNaN(angle))
            {
                n.mapAngle = angle;
                if (n.SlantSlider != null)
                    n.SlantSlider.Value = angle;

            }
        }

        /// <summary>
        /// Identifies the <see cref="FillColor1"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetWidthProperty = DependencyProperty.Register("TargetWidth", typeof(double), typeof(ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit), new PropertyMetadata(150.0));

        /// <summary>
        /// Gets or sets the target width of the scale bar.
        /// </summary>
        /// <remarks>The actual width of the scale bar changes when values are rounded.</remarks>
        public double TargetWidth
        {
            get { return (double)GetValue(TargetWidthProperty); }
            set { SetValue(TargetWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the map unit.
        /// </summary>
        public ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit MapUnit { get; set; }

        #endregion


        #region Setup Methods


        private void map_Progress(object sender, ProgressEventArgs e)
        {

        }

        // sets map of progressbar
        private void progressBarCheck(object sender, ProgressEventArgs e)
        {
            if (progressBar != null)
            {
                if (progressBar.Map == null)
                {
                    progressBar.Map = navControl.Map;
                    navControl.Map.Progress -= progressBarCheck;
                }
            }
        }

        // sets up use of plane projection
        private void projectionCheck(object sender, ProgressEventArgs e)
        {
            if (SlantSliderGrid != null)
            {
                if (usePlaneProjection)
                {
                    if (MapProjection != null)
                    {
                        SlantSliderGrid.Visibility = Visibility.Visible;
                        SlantGlass.Visibility = Visibility.Visible;
                        SlantText.Visibility = Visibility.Visible;
                        SlantSlider.Value = mapAngle;
                    }
                    else
                    {
                        SlantSliderGrid.Visibility = Visibility.Collapsed;
                        SlantGlass.Visibility = Visibility.Collapsed;
                        SlantText.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    SlantSliderGrid.Visibility = Visibility.Collapsed;
                    SlantGlass.Visibility = Visibility.Collapsed;
                    SlantText.Visibility = Visibility.Collapsed;
                    usePlaneProjection = false;
                }
                Map.Progress -= projectionCheck;
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            VerticalAlignment va = this.VerticalAlignment;
            if (va == VerticalAlignment.Bottom)
            {
                bottomAligned = true;
                ScaleBarOuterBorder.CornerRadius = new CornerRadius(0, 21, 0, 0);
                ScaleBarOuterBorder.VerticalAlignment = VerticalAlignment.Bottom;
                ScaleBarInnerBorder.CornerRadius = new CornerRadius(0, 20, 0, 0);
                OpacityText.VerticalAlignment = VerticalAlignment.Bottom;
                NavOpacitySliderGrid.VerticalAlignment = VerticalAlignment.Bottom;
            }
        }
        #endregion


        #region Events
        // Event handler for opacity slider
        private void NavOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (root != null)
                root.Opacity = e.NewValue;
        }

        private void Map_ExtentChanged(object sender, ExtentEventArgs args)
        {
            
            refreshScalebar();

        }

        #endregion


        #region Map Slant

        private void SlantSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Map != null)
            {
                double angle = e.NewValue;
                NavProjection.RotationX = angle;
                MapProjection.RotationX = angle;
                //if (OverviewMap2 != null) OverviewMap2.MapAngle = angle;
            }
        }

        private void SlantSlider_Loaded(object sender, RoutedEventArgs e)
        {
            if (usePlaneProjection)
            {
                SlantSlider.Value = mapAngle;
            }
        }

        private void SlantSlider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mapCenter = Map.Extent.GetCenter();
        }

        private void SlantSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            Slider slider = sender as Slider;
            RotateTransform rt = new RotateTransform();
            PlaneProjection pp = new PlaneProjection();
            if (mapWidth > 0 && mapHeight > 0 && slider.Value < 0)
            {
                double wmargin = mapWidth * -1;
                double hmargin = (mapHeight) * -1;
                Map.Margin = new Thickness(wmargin, hmargin, wmargin, hmargin);

                pp.CenterOfRotationX = mapWidth * 1.5;
                pp.CenterOfRotationY = mapHeight * 1.5;
                Map.Projection = pp;
            }
            else
            {
                Map.Margin = new Thickness(0);
                pp.CenterOfRotationX = mapWidth * 0.5;
                pp.CenterOfRotationY = mapHeight * 0.5;
                Map.Projection = pp;
            }

            Map.PanTo(mapCenter);
        }

        private void GlobeGlass_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
            lastY = startPoint.Y;
            SlantGlass.MouseMove += SlantGlass_MouseMove;
            SlantGlass.MouseLeftButtonUp += SlantGlass_MouseLeftButtonUp;
            SlantGlass.MouseLeave += SlantGlass_MouseLeave;
        }

        void SlantGlass_MouseLeave(object sender, MouseEventArgs e)
        {
            SlantGlass.MouseMove -= SlantGlass_MouseMove;
            SlantGlass.MouseLeftButtonUp -= SlantGlass_MouseLeftButtonUp;
            SlantGlass.MouseLeave -= SlantGlass_MouseLeave;
        }

        void SlantGlass_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SlantGlass.MouseMove -= SlantGlass_MouseMove;
            SlantGlass.MouseLeftButtonUp -= SlantGlass_MouseLeftButtonUp;

        }

        void SlantGlass_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(null);
            double diff = pt.Y - lastY;
            double slant = SlantSlider.Value;
            double newSlant = slant += diff;
            if (newSlant <= SlantSlider.Maximum && newSlant >= SlantSlider.Minimum) SlantSlider.Value = newSlant;
        }
        #endregion


        #region Wing methods
        private void ZoomWingCollapseButton_Click(object sender, RoutedEventArgs e)
        {
            StackPanel zes = ZoomGridStack;
            Visibility vis = zes.Visibility;
            BottomZoomWingUpDownExpand.Visibility = Visibility.Collapsed;
            TopZoomWingUpDownExpand.Visibility = Visibility.Collapsed;
            if (vis == Visibility.Collapsed)
            {
                ExpandZoomBar.Begin();
                TopZoomWingUpDownExpand.IsPointingDown = true;
                BottomZoomWingUpDownExpand.IsPointingDown = false;

            }
            else
            {
                NavOpacitySliderGrid.Visibility = Visibility.Collapsed;
                OpacityText.Visibility = Visibility.Collapsed;
                zes.Visibility = Visibility.Collapsed;
                ZoomGridInnerBorder.Padding = bottomAligned ? new Thickness(5, 0, 5, 70) : new Thickness(5, 70, 5, 0);
                TopZoomWingUpDownExpand.IsPointingDown = false;
                BottomZoomWingUpDownExpand.IsPointingDown = true;
                ShrinkZoomBar.Begin();
            }
        }

        private void ScaleBarWingCollapseButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility vis = ScaleBar.Visibility;

            if (vis == Visibility.Collapsed)
            {
                ExpandProgressBarSpacer.Begin();
                ScaleBarWingLeftRightExpand.IsPointingRight = false;
            }
            else
            {
                ScaleBar.Visibility = Visibility.Collapsed;
                progressBarGrid.Visibility = Visibility.Collapsed;
                SlantSliderGrid.Visibility = Visibility.Collapsed;
                SlantText.Visibility = Visibility.Collapsed;
                ScaleBarWingLeftRightExpand.IsPointingRight = true;

                ShrinkProgressBarSpacer.Begin();
                ScaleBarInnerBorder.Padding = new Thickness(70, 2, 0, 2);
            }
        }

        private void expandprogressbarSpacer_Completed(object sender, EventArgs e)
        {
            ScaleBar.Visibility = Visibility.Visible;
            progressBarGrid.Visibility = Visibility.Visible;
            if (usePlaneProjection)
            {
                SlantSliderGrid.Visibility = Visibility.Visible;
                SlantText.Visibility = Visibility.Visible;
            }
            ScaleBarInnerBorder.Padding = new Thickness(100, 2, 0, 2);
        }

        private void shrinkprogressbarSpacer_Completed(object sender, EventArgs e)
        {

        }

        private void ExpandZoomBar_Completed(object sender, EventArgs e)
        {
            ZoomGridStack.Visibility = Visibility.Visible;
            NavOpacitySliderGrid.Visibility = Visibility.Visible;
            OpacityText.Visibility = Visibility.Visible;
            ZoomGridInnerBorder.Padding = bottomAligned ? new Thickness(5, 0, 5, 110) : new Thickness(5, 110, 5, 0);
            if (bottomAligned)
                TopZoomWingUpDownExpand.Visibility = Visibility.Visible;
            else
                BottomZoomWingUpDownExpand.Visibility = Visibility.Visible;

        }

        private void ShrinkZoomBar_Completed(object sender, EventArgs e)
        {
            if (bottomAligned)
                TopZoomWingUpDownExpand.Visibility = Visibility.Visible;
            else
                BottomZoomWingUpDownExpand.Visibility = Visibility.Visible;

        }

        #endregion


        #region ScaleBar Helper Functions

        private void refreshScalebar()
        {
            Visibility viz = Visibility.Collapsed;
            bool canDisplay = true;
            if (ScaleBar != null)
            {
                viz = ScaleBar.Visibility;
                if (Map == null || double.IsNaN(Map.Resolution) ||
                    MapUnit == ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.DecimalDegrees && Math.Abs(Map.Extent.GetCenter().Y) >= 90)
                {
                    viz = Visibility.Collapsed;
                    canDisplay = false;
                }
                if (ScaleBar != null) ScaleBar.Visibility = viz;
            }
            if (!canDisplay) return;

            ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit outUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Undefined;
            double outResolution;

            #region KiloMeters/Meters
            double roundedKiloMeters = getBestEstimateOfValue(Map.Resolution, ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Kilometers, out outUnit, out outResolution);
            double widthMeters = roundedKiloMeters / outResolution;
            bool inMeters = outUnit == ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Meters;

            if (PaddingLeftForScaleBarTextMeters != null)
                PaddingLeftForScaleBarTextMeters.Width = widthMeters;
            if (PaddingLeftTopNotch != null)
                PaddingLeftTopNotch.Width = widthMeters;
            if (ScaleBarTextForMeters != null)
            {
                ScaleBarTextForMeters.Text = string.Format("{0}{1}", roundedKiloMeters, (inMeters ? "m" : "km"));
                ScaleBarTextForMeters.Width = widthMeters;
            }
            #endregion

            #region Miles
            double roundedMiles = getBestEstimateOfValue(Map.Resolution, ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Miles, out outUnit, out outResolution);
            double widthMiles = roundedMiles / outResolution;
            bool inFeet = outUnit == ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Feet;

            if (PaddingLeftForScaleBarTextMiles != null)
                PaddingLeftForScaleBarTextMiles.Width = widthMiles;
            if (PaddingLeftBottomNotch != null)
                PaddingLeftBottomNotch.Width = widthMiles;
            if (ScaleBarTextForMiles != null)
            {
                ScaleBarTextForMiles.Text = string.Format("{0}{1}", roundedMiles, inFeet ? "ft" : "mi");
                ScaleBarTextForMiles.Width = widthMiles;
            }
            #endregion

            double widthOfNotches = 4; // 2 for left notch, 2 for right notch
            double scaleBarBlockWidth = (widthMiles > widthMeters) ? widthMiles : widthMeters;
            scaleBarBlockWidth += widthOfNotches;

            if (!double.IsNaN(scaleBarBlockWidth) && ScaleBarBlock != null)
                ScaleBarBlock.Width = scaleBarBlockWidth;

        }

        private double getBestEstimateOfValue(double resolution, ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit displayUnit, out ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit unit, out double outResolution)
        {
            unit = displayUnit;
            double rounded = 0;
            double originalRes = resolution;
            while (rounded < 0.5)
            {
                resolution = originalRes;
                if (MapUnit == ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.DecimalDegrees)
                {
                    resolution = getResolutionForGeographic(Map.Extent, resolution);
                    resolution = resolution * (int)ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Meters / (int)unit;
                }
                else if (MapUnit != ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Undefined)
                {
                    resolution = resolution * (int)MapUnit / (int)unit;
                }

                double val = TargetWidth * resolution;
                val = roundToSignificant(val, resolution);
                double noFrac = Math.Round(val); // to get rid of the fraction
                if (val < 0.5)
                {
                    ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit newUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Undefined;
                    // Automatically switch unit to a lower one
                    if (unit == ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Kilometers)
                        newUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Meters;
                    else if (unit == ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Miles)
                        newUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Feet;
                    if (newUnit == ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Undefined) { break; } //no lower unit
                    unit = newUnit;
                }
                else if (noFrac > 1)
                {
                    rounded = noFrac;
                    var len = noFrac.ToString().Length;
                    if (len <= 2)
                    {
                        // single/double digits ... make it a multiple of 5 ..or 1,2,3,4
                        if (noFrac > 5)
                        {
                            rounded -= noFrac % 5;
                        }
                        while (rounded > 1 && (rounded / resolution) > TargetWidth)
                        {
                            // exceeded maxWidth .. decrement by 1 or by 5
                            double decr = noFrac > 5 ? 5 : 1;
                            rounded = rounded - decr;
                        }
                    }
                    else if (len > 2)
                    {
                        rounded = Math.Round(noFrac / Math.Pow(10, len - 1)) * Math.Pow(10, len - 1);
                        if ((rounded / resolution) > TargetWidth)
                        {
                            // exceeded maxWidth .. use the lower bound instead
                            rounded = Math.Floor(noFrac / Math.Pow(10, len - 1)) * Math.Pow(10, len - 1);
                        }
                    }
                }
                else
                { // anything between 0.5 and 1
                    rounded = Math.Floor(val);
                    if (rounded == 0)
                    {
                        //val >= 0.5 but < 1 so round up
                        rounded = (val == 0.5) ? 0.5 : 1;
                        if ((rounded / resolution) > TargetWidth)
                        {
                            // exceeded maxWidth .. re-try by switching to lower unit 
                            rounded = 0;
                            ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit newUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Undefined;
                            // Automatically switch unit to a lower one
                            if (unit == ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Kilometers)
                                newUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Meters;
                            else if (unit == ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Miles)
                                newUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Feet;
                            if (newUnit == ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Undefined) { break; } //no lower unit
                            unit = newUnit;
                        }
                    }
                }
            }
            outResolution = resolution;
            return rounded;
        }

        double roundToSignificant(double value, double resolution)
        {
            var round = Math.Floor(-Math.Log(resolution));
            if (round > 0)
            {
                round = Math.Pow(10, round);
                return Math.Round(value * round) / round;
            }
            else { return Math.Round(value); }
        }


        /// <summary>
        /// Calculates horizontal scale at center of extent
        /// for geographic / Plate Carrée projection.
        /// Horizontal scale is 0 at the poles.
        /// </summary>
        double toRadians = 0.017453292519943295769236907684886;
        double earthRadius = 6378137; //Earth radius in meters (defaults to WGS84 / GRS80)
        double degreeDist;
        private double getResolutionForGeographic(ESRI.ArcGIS.Client.Geometry.Envelope extent, double resolution)
        {
            degreeDist = earthRadius * toRadians;
            MapPoint center = extent.GetCenter();
            double y = center.Y;
            if (Math.Abs(y) > 90) { return 0; }
            return Math.Cos(y * toRadians) * resolution * degreeDist;
        }
        #endregion
   
    
    }
}
