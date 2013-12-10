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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;

namespace ESRI.ArcGIS.SilverlightMapApp
{
    [TemplatePart(Name = "ScaleBarBlock", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PaddingLeftForScaleBarTextMeters", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PaddingLeftTopNotch", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PaddingLeftForScaleBarTextMiles", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PaddingLeftBottomNotch", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "ScaleBarTextForMeters", Type = typeof(TextBlock))]
    [TemplatePart(Name = "ScaleBarTextForMiles", Type = typeof(TextBlock))]
    [TemplatePart(Name = "LayoutRoot", Type = typeof(FrameworkElement))]
    public class ScaleBar : Control
    {
        FrameworkElement ScaleBarBlock;
        FrameworkElement PaddingLeftForScaleBarTextMeters;
        TextBlock ScaleBarTextForMeters;
        FrameworkElement PaddingLeftTopNotch;
        FrameworkElement PaddingLeftForScaleBarTextMiles;
        FrameworkElement PaddingLeftBottomNotch;
        FrameworkElement LayoutRoot;

        TextBlock ScaleBarTextForMiles;
        public ScaleBar()
        {
            DefaultStyleKey = typeof(ScaleBar);
        }
        public override void OnApplyTemplate()
        {
            ScaleBarBlock = this.GetTemplateChild("ScaleBarBlock") as FrameworkElement;
            PaddingLeftForScaleBarTextMeters = this.GetTemplateChild("PaddingLeftForScaleBarTextMeters") as FrameworkElement;
            PaddingLeftTopNotch = this.GetTemplateChild("PaddingLeftTopNotch") as FrameworkElement;
            PaddingLeftForScaleBarTextMiles = this.GetTemplateChild("PaddingLeftForScaleBarTextMiles") as FrameworkElement;
            PaddingLeftBottomNotch = this.GetTemplateChild("PaddingLeftBottomNotch") as FrameworkElement;
            ScaleBarTextForMeters = this.GetTemplateChild("ScaleBarTextForMeters") as TextBlock;
            ScaleBarTextForMiles = this.GetTemplateChild("ScaleBarTextForMiles") as TextBlock;
            LayoutRoot = this.GetTemplateChild("LayoutRoot") as FrameworkElement;
            refreshScalebar();
            base.OnApplyTemplate();
        }
        #region Helper Functions
        private void map_ExtentChanged(object sender, ESRI.ArcGIS.Client.ExtentEventArgs args)
        {
            refreshScalebar();
        }

        private void refreshScalebar()
        {
            Visibility viz = Visibility.Visible;
            if (Map == null || double.IsNaN(Map.Resolution) ||
                MapUnit == ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.DecimalDegrees && Math.Abs(Map.Extent.GetCenter().Y) >= 90)
            {
                viz = Visibility.Collapsed;
            }
            if (LayoutRoot != null) LayoutRoot.Visibility = viz;
            if (viz == Visibility.Collapsed) return;

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

        #region Properties

        /// <summary>
        /// Identifies the Map dependency property.
        /// </summary>
        public static readonly DependencyProperty MapProperty =
            DependencyProperty.Register(
              "Map",
              typeof(Map),
              typeof(ScaleBar),
              new PropertyMetadata(OnMapPropertyChanged));


        private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Map oldMap = e.OldValue as Map;
            Map newMap = e.NewValue as Map;
            ScaleBar bar = d as ScaleBar;
            if (bar != null)
            {
                if (oldMap != null)
                {
                    oldMap.ExtentChanged -= bar.map_ExtentChanged;
                    oldMap.ExtentChanging -= bar.map_ExtentChanged;
                }
                if (newMap != null)
                {
                    newMap.ExtentChanged += bar.map_ExtentChanged;
                    newMap.ExtentChanging += bar.map_ExtentChanged;
                }
                bar.refreshScalebar();
            }
        }

        /// <summary>
        /// Gets or sets the map that the scale bar is buddied to.
        /// </summary>
        public ESRI.ArcGIS.Client.Map Map
        {
            get { return GetValue(MapProperty) as Map; }
            set { SetValue(MapProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FillColor1"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetWidthProperty = DependencyProperty.Register("TargetWidth", typeof(double), typeof(ScaleBar), new PropertyMetadata(150.0));

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


        /// <summary>
        /// Identifies the <see cref="Fill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
                        DependencyProperty.Register("Fill", typeof(Brush), typeof(ScaleBar),
                        new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        /// <summary>
        /// Gets or sets the color of the scalebar.
        /// </summary>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        #endregion
    }
}
