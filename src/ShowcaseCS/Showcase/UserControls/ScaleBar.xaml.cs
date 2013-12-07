using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;

namespace ESRI.ArcGIS.SilverlightMapApp
{
    /// <summary>
    /// Custom ScaleBar control showing both Metric and US units.
    /// </summary>
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
        #region Private fields
        #region TemplatePart References
        private FrameworkElement ScaleBarBlock;
        private FrameworkElement PaddingLeftForScaleBarTextMeters;
        private TextBlock ScaleBarTextForMeters;
        private FrameworkElement PaddingLeftTopNotch;
        private FrameworkElement PaddingLeftForScaleBarTextMiles;
        private FrameworkElement PaddingLeftBottomNotch;
        private FrameworkElement LayoutRoot;
        private TextBlock ScaleBarTextForMiles;
        #endregion
        private static ESRI.ArcGIS.Client.Projection.WebMercator merc = new ESRI.ArcGIS.Client.Projection.WebMercator();
        private static SpatialReference webMercSref = new SpatialReference(102100);
        private const double toRadians = 0.017453292519943295769236907684886; //Conversion factor from degrees to radians
        private const double earthRadius = 6378137; //Earth radius in meters (defaults to WGS84 / GRS80)
        private const double degreeDist = 111319.49079327357264771338267052; //earthRadius * toRadians;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleBar"/> class.
        /// </summary>
        public ScaleBar()
        {
            DefaultStyleKey = typeof(ScaleBar);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application
        /// code or internal processes (such as a rebuilding layout pass) call 
        /// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In
        /// simplest terms, this means the method is called just before a UI
        /// element displays in an application.
        /// </summary>
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
            RefreshScalebar();
            base.OnApplyTemplate();
        }

        #region Helper Functions

        /// <summary>
        /// Handles the ExtentChanged and ExtentChanging event of the map control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ESRI.ArcGIS.Client.ExtentEventArgs"/> 
        /// instance containing the event data.</param>
        private void map_ExtentChanged(object sender, ESRI.ArcGIS.Client.ExtentEventArgs args)
        {
            RefreshScalebar();
        }

        /// <summary>
        /// Refreshes the scalebar when the map extent changes.
        /// </summary>
        private void RefreshScalebar()
        {
            Visibility viz = Visibility.Visible;
            if (Map == null || double.IsNaN(Map.Resolution) ||
                MapUnit == ScaleBarUnit.DecimalDegrees && Math.Abs(Map.Extent.GetCenter().Y) >= 90)
            {
                viz = Visibility.Collapsed;
            }
            if (LayoutRoot != null) LayoutRoot.Visibility = viz;
            if (viz == Visibility.Collapsed) return;
            ScaleBarUnit outUnit = ScaleBarUnit.Undefined;
            double outResolution;

            #region KiloMeters/Meters
            double roundedKiloMeters = GetBestEstimateOfValue(Map.Resolution, ScaleBarUnit.Kilometers, out outUnit, out outResolution);
            double widthMeters = roundedKiloMeters / outResolution;
            bool inMeters = outUnit == ScaleBarUnit.Meters;

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
            double roundedMiles = GetBestEstimateOfValue(Map.Resolution, ScaleBarUnit.Miles, out outUnit, out outResolution);
            double widthMiles = roundedMiles / outResolution;
            bool inFeet = outUnit == ScaleBarUnit.Feet;

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

        private double GetBestEstimateOfValue(double resolution, ScaleBarUnit displayUnit, out ScaleBarUnit unit, out double outResolution)
        {
            unit = displayUnit;
            double rounded = 0;
            double originalRes = resolution;
            while (rounded < 0.5)
            {
                resolution = originalRes;
                if (MapUnit == ScaleBarUnit.DecimalDegrees)
                {
                    resolution = GetResolutionForGeographic(Map.Extent.GetCenter(), resolution);
                    resolution = resolution * (int)ScaleBarUnit.Meters / (int)unit;
                }
                else if (webMercSref.Equals(Map.SpatialReference))
                {
                    //WebMercator
                    MapPoint center = Map.Extent.GetCenter();
                    center.X = Math.Min(Math.Max(center.Y, -20037508.3427892), 20037508.3427892);
                    MapPoint center2 = merc.ToGeographic(new MapPoint(Math.Min(center.X + Map.Resolution, 20037508.3427892), center.Y)) as MapPoint;
                    center = merc.ToGeographic(center) as MapPoint;
                    resolution = GetResolutionForGeographic(center, center2.X - center.X);
                    resolution = resolution * (int)ScaleBarUnit.Meters / (int)unit;
                }
                else if (MapUnit != ScaleBarUnit.Undefined)
                {
                    resolution = resolution * (int)MapUnit / (int)unit;
                }

                double val = TargetWidth * resolution;
                val = RoundToSignificant(val, resolution);
                double noFrac = Math.Round(val); // to get rid of the fraction
                if (val < 0.5)
                {
                    ScaleBarUnit newUnit = ScaleBarUnit.Undefined;
                    // Automatically switch unit to a lower one
                    if (unit == ScaleBarUnit.Kilometers)
                        newUnit = ScaleBarUnit.Meters;
                    else if (unit == ScaleBarUnit.Miles)
                        newUnit = ScaleBarUnit.Feet;
                    if (newUnit == ScaleBarUnit.Undefined) { break; } //no lower unit
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
                            ScaleBarUnit newUnit = ScaleBarUnit.Undefined;
                            // Automatically switch unit to a lower one
                            if (unit == ScaleBarUnit.Kilometers)
                                newUnit = ScaleBarUnit.Meters;
                            else if (unit == ScaleBarUnit.Miles)
                                newUnit = ScaleBarUnit.Feet;
                            if (newUnit == ScaleBarUnit.Undefined) { break; } //no lower unit
                            unit = newUnit;
                        }
                    }
                }
            }
            outResolution = resolution;
            return rounded;
        }

        /// <summary>
        /// Rounds to a value to the significant number of digits.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="resolution">The resolution.</param>
        /// <returns></returns>
        private static double RoundToSignificant(double value, double resolution)
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
        private static double GetResolutionForGeographic(MapPoint center, double resolution)
        {
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
                bar.RefreshScalebar();
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
        /// Identifies the <see cref="TargetWidth"/> dependency property.
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
        public ESRI.ArcGIS.Client.ScaleBarUnit MapUnit { get; set; }

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