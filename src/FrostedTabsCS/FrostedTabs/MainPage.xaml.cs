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
using System.ComponentModel;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Toolkit;
using ESRI.ArcGIS.Client.Tasks;
using Toolkit.Icons;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace FrostedTabs
{
    public partial class MainPage : UserControl
    {
        private double _panFactor = 0.5;
        double[] layerResolutions;

        bool zoomSetUp = false;
        bool settingZoomLevel = false;
        int currentLevel = 0;
        Point startPoint;
        double rotationAngle = 0;
        double startRotationAngle = 0;

        SolidColorBrush levelGreaterThanBrush;
        SolidColorBrush levelLessThanBrush;
        SolidColorBrush levelEqualsBrush;
        LinearGradientBrush ribbonElementBackground;
        LinearGradientBrush ribbonElementBackgroundHighlight;

        private Draw _drawSurface;
        QueryTask _queryTask;


        public MainPage()
        {
            InitializeComponent();
            ResourceDictionary rd = Application.Current.Resources;
            levelGreaterThanBrush = rd["levelGreaterThanBrush"] as SolidColorBrush;
            levelLessThanBrush = rd["levelLessThanBrush"] as SolidColorBrush;
            levelEqualsBrush = rd["levelEqualsBrush"] as SolidColorBrush;
            ribbonElementBackground = rd["RibbonElementBackground"] as LinearGradientBrush;
            ribbonElementBackgroundHighlight = rd["RibbonElementBackgroundHighlight"] as LinearGradientBrush;

            _drawSurface = new Draw(Map)
            {
                LineSymbol = DefaultLineSymbol,
                FillSymbol = DefaultFillSymbol
            };
            _drawSurface.DrawComplete += MyDrawSurface_DrawComplete;

            _queryTask = new QueryTask("http://sampleserver6.arcgisonline.com/arcgis/rest/services/USA/MapServer/2");
            _queryTask.ExecuteCompleted += QueryTask_ExecuteCompleted;
            _queryTask.Failed += QueryTask_Failed;
        }

        #region Startup Event Handlers

        private void ArcGISTiledMapServiceLayer_Initialized(object sender, EventArgs e)
        {
            if (!zoomSetUp)
                SetUpZoom();
        }

        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            Map.ExtentChanged += Map_ExtentChanged;
        }

        #endregion


        #region Event Handlers

        private void Map_ExtentChanged(object sender, ExtentEventArgs args)
        {
            if (!zoomSetUp)
                SetUpZoom();
            if (ZoomLevelStack != null)
            {
                currentLevel = Convert.ToInt32(Math.Round(getValueFromMap(Map.Extent)));
                StackPanel zs = ZoomLevelStack;
                int level = currentLevel;
                for (int i = 0; i < zs.Children.Count; i++)
                {
                    Border border = zs.Children[i] as Border;
                    SolidColorBrush brush = setLevelBrush(i);
                    border.Background = brush;
                }
            }

        }

        private void PanButton_Click(object sender, RoutedEventArgs e)
        {
            if (Map == null || sender == null) return;

            Envelope env = Map.Extent;
            if (env == null) return;
            double x = 0, y = 0;
            MapPoint oldCenter = env.GetCenter();
            MapPoint newCenter = null;
            var height = env.Height * _panFactor;
            var width = env.Width * _panFactor;
            // if units are degrees (the default), limit or alter panning to the lat/lon limits
            if (sender == PanUp) // North
            {
                y = oldCenter.Y + height;
                newCenter = new MapPoint(oldCenter.X, y);
            }
            else if (sender == PanRight) // East
            {
                x = oldCenter.X + width;
                newCenter = new MapPoint(x, oldCenter.Y);
            }
            else if (sender == PanLeft) // West
            {
                x = oldCenter.X - width;
                newCenter = new MapPoint(x, oldCenter.Y);
            }
            else if (sender == PanDown) // South
            {
                y = oldCenter.Y - height;
                newCenter = new MapPoint(oldCenter.X, y);
            }

            if (newCenter != null)
                Map.PanTo(newCenter);

        }

        private void ZoomFullExtentButton_Click(object sender, RoutedEventArgs e)
        {
            if (Map != null)
                Map.ZoomTo(Map.Layers.GetFullExtent());
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MapRotationGrid.Width = MapDisplayGrid.ActualWidth;
            MapRotationGrid.Height = MapDisplayGrid.ActualHeight;
        }

        private void TabItem_MouseEnter(object sender, MouseEventArgs e)
        {
            //TabItem item = sender as TabItem;
            //Int32 index = Convert.ToInt32(item.Tag);
            //tabRibbon.SelectedIndex = index;
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Background = ribbonElementBackgroundHighlight;

        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Background = ribbonElementBackground;
        }

        private void ZoomStackGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            //ZoomStackHighlight.Opacity = 1;
        }

        private void ZoomStackGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            //ZoomStackHighlight.Opacity = 0;
        }

        #endregion

        #region Zoom Level

        private void SetUpZoom()
        {
            if (ZoomLevelStack != null)
            {
                System.Collections.Generic.List<double> resolutions = new System.Collections.Generic.List<double>();
                foreach (Layer layer in Map.Layers)
                {
                    if (layer is TiledMapServiceLayer)
                    {
                        TiledMapServiceLayer tlayer = layer as TiledMapServiceLayer;
                        if (tlayer.TileInfo == null || tlayer.TileInfo.Lods == null) continue;
                        var res = from t in tlayer.TileInfo.Lods
                                  select t.Resolution;
                        resolutions.AddRange(res);
                    }
                }
                if (resolutions.Count < 1)
                    return;
                resolutions.Sort();
                layerResolutions = resolutions.Distinct().Reverse().ToArray();
                StackPanel sp = ZoomLevelStack;
                int min = 0;
                int max = layerResolutions.Length - 1;
                int numLevels = max + 1 - min;
                double levelHeight = (100 / numLevels);
                for (int i = min; i <= max; i++)
                {
                    SolidColorBrush brush = setLevelBrush(i);
                    Border levelBorder = new Border()
                    {
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(1),
                        Height = levelHeight,
                        Width = 18,
                        BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                        Background = brush,
                        Tag = i
                    };
                    levelBorder.MouseLeftButtonDown += levelBorder_MouseLeftButtonDown;
                    levelBorder.MouseMove += levelBorder_MouseMove;
                    levelBorder.MouseLeftButtonUp += levelBorder_MouseLeftButtonUp;
                    sp.Children.Add(levelBorder);
                }
                sp.MouseLeave += new MouseEventHandler(sp_MouseLeave);
                zoomSetUp = true;
            }
        }

        void sp_MouseLeave(object sender, MouseEventArgs e)
        {
            if (settingZoomLevel)
                FinishSettingLevel();
        }

        void levelBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FinishSettingLevel();
        }

        private void FinishSettingLevel()
        {
            settingZoomLevel = false;
            Map.ZoomToResolution(layerResolutions[currentLevel]);

        }

        void levelBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (settingZoomLevel)
            {
                Border levelBorder = sender as Border;
                int level = Convert.ToInt32(levelBorder.Tag);
                StackPanel zs = ZoomLevelStack;
                for (int i = 0; i < zs.Children.Count; i++)
                {
                    Border border = zs.Children[i] as Border;
                    SolidColorBrush brush = setLevelBrush(i);
                    border.Background = brush;

                }
                currentLevel = level;
            }
        }

        void levelBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border levelBorder = sender as Border;
            currentLevel = Convert.ToInt32(levelBorder.Tag);
            settingZoomLevel = true;
        }


        private double getValueFromMap(ESRI.ArcGIS.Client.Geometry.Envelope extent)
        {
            if (layerResolutions == null || layerResolutions.Length == 0 ||
                Map == null || extent == null) return -1;
            double mapRes = extent.Width / Map.ActualWidth;
            for (int i = 0; i < layerResolutions.Length - 1; i++)
            {
                double thisRes = layerResolutions[i];
                double nextRes = layerResolutions[i + 1];
                if (mapRes >= thisRes)
                {
                    return i;
                }
                if (mapRes < thisRes && mapRes > nextRes)
                {
                    return i + (thisRes - mapRes) / (thisRes - nextRes);
                }
            }
            return Convert.ToDouble(layerResolutions.Length - 1);
        }

        private SolidColorBrush setLevelBrush(int level)
        {
            SolidColorBrush brush;
            if (level < currentLevel)
                brush = levelGreaterThanBrush;
            else if (currentLevel == level)
                brush = levelEqualsBrush;
            else
                brush = levelLessThanBrush;
            return brush;
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {

            Map.Zoom(0.5);
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            Map.Zoom(2.0);
        }

        #endregion

        #region Map Rotation

        private void MapRotationGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            FrameworkElement mapRotator = sender as FrameworkElement;
            startPoint = e.GetPosition(MapRotationCanvas);
            startRotationAngle = Math.Atan2(startPoint.Y, startPoint.X) / Math.PI * 180;
            mapRotator.MouseMove += mapRotator_MouseMove;
            mapRotator.MouseLeftButtonUp += mapRotator_MouseLeftButtonUp;
            mapRotator.MouseLeave += mapRotator_MouseLeave;

        }

        void mapRotator_MouseLeave(object sender, MouseEventArgs e)
        {
            FrameworkElement mapRotator = sender as FrameworkElement;
            finishRotation(mapRotator);
        }

        void mapRotator_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement mapRotator = sender as FrameworkElement;
            finishRotation(mapRotator);
        }

        void mapRotator_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(MapRotationCanvas);
            double newAngle = Math.Atan2(pt.Y, pt.X) / Math.PI * 180;
            double diff = newAngle - startRotationAngle;
            double angle = rotationAngle + diff;
            Map.Rotation = angle;
            RotationAngleText.Text = String.Format("{0}°", Math.Round(angle, 1));
            RotateTransform rotate = new RotateTransform();
            rotate.Angle = angle;
            RotationGlobeGrid.RenderTransform = rotate;

        }

        private void finishRotation(FrameworkElement mapRotator)
        {
            mapRotator.MouseMove -= mapRotator_MouseMove;
            mapRotator.MouseLeftButtonUp -= mapRotator_MouseLeftButtonUp;
            mapRotator.MouseLeave -= mapRotator_MouseLeave;
            rotationAngle = Map.Rotation;
        }


        private void ResetRotation_Click(object sender, RoutedEventArgs e)
        {
            ResetMapRotation.Begin();
        }

        private void ResetMapRotation_Completed(object sender, EventArgs e)
        {
            ResetMapRotation.Stop();
            Map.Rotation = 0;
            MapRotationTransform.Angle = 0;
            RotationAngleText.Text = "0°";
            rotationAngle = 0;
            RotationGlobeGrid.RenderTransform = MapRotationTransform;
        }

        #endregion

        #region Spatial Query

        private void SelectionTool_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string tagstring = (string)button.Tag;
            switch (tagstring)
            {
                case "0": // Point
                    _drawSurface.DrawMode = DrawMode.Point;
                    break;
                case "1": // Polyline
                    _drawSurface.DrawMode = DrawMode.Polyline;
                    break;
                case "2": // Polygon
                    _drawSurface.DrawMode = DrawMode.Polygon;
                    break;
                case "3": // Rectangle
                    _drawSurface.DrawMode = DrawMode.Rectangle;
                    break;
                default: // Clear
                    _drawSurface.DrawMode = DrawMode.None;
                    GraphicsLayer selectionGraphicslayer = Map.Layers["MySelectionGraphicsLayer"] as GraphicsLayer;
                    selectionGraphicslayer.ClearGraphics();
                    QueryDetailsDataGrid.ItemsSource = null;
                    ResultsDisplay.IsExpanded = false;
                    break;
            }
            _drawSurface.IsEnabled = (_drawSurface.DrawMode != DrawMode.None);
        }


        private void MyDrawSurface_DrawComplete(object sender, ESRI.ArcGIS.Client.DrawEventArgs args)
        {
            GraphicsLayer selectionGraphicslayer = Map.Layers["MySelectionGraphicsLayer"] as GraphicsLayer;
            selectionGraphicslayer.ClearGraphics();

            // Bind data grid to query results
            Binding resultFeaturesBinding = new Binding("LastResult.Features");
            resultFeaturesBinding.Source = _queryTask;
            QueryDetailsDataGrid.SetBinding(DataGrid.ItemsSourceProperty, resultFeaturesBinding);

            Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            query.OutFields.AddRange(new string[] { "state_name", "pop2000", "sub_region" });
            query.OutSpatialReference = Map.SpatialReference;
            query.Geometry = args.Geometry;
            query.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
            query.ReturnGeometry = true;

            _queryTask.ExecuteAsync(query);
        }
        private void QueryTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            FeatureSet featureSet = args.FeatureSet;

            if (featureSet == null || featureSet.Features.Count < 1)
            {
                MessageBox.Show("No features retured from query");
                return;
            }

            GraphicsLayer graphicsLayer = Map.Layers["MySelectionGraphicsLayer"] as GraphicsLayer;

            if (featureSet != null && featureSet.Features.Count > 0)
            {
                foreach (Graphic feature in featureSet.Features)
                {
                    feature.Symbol = ResultsFillSymbol;
                    graphicsLayer.Graphics.Insert(0, feature);
                }
            }

            //ResultsDisplay.Visibility = Visibility.Visible;
            ResultsDisplay.IsExpanded = true;

            _drawSurface.IsEnabled = false;
        }

        private void QueryTask_Failed(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Query failed: " + args.Error);
        }

				private void QueryDetailsDataGrid_SelectionChanged(object sender,SelectionChangedEventArgs e)
				{
					foreach (Graphic g in e.AddedItems)
						g.Select();

					foreach (Graphic g in e.RemovedItems)
						g.UnSelect();
				}

				private void QueryDetailsDataGrid_LoadingRow(object sender,DataGridRowEventArgs e)
				{
					e.Row.MouseEnter += Row_MouseEnter;
					e.Row.MouseLeave += Row_MouseLeave;
				}

				void Row_MouseEnter(object sender,MouseEventArgs e)
				{
					(((System.Windows.FrameworkElement)(sender)).DataContext as Graphic).Select();
				}

				void Row_MouseLeave(object sender,MouseEventArgs e)
				{
					DataGridRow row = sender as DataGridRow;
					Graphic g = ((System.Windows.FrameworkElement)(sender)).DataContext as Graphic;

					if (!QueryDetailsDataGrid.SelectedItems.Contains(g))
						g.UnSelect();
				}

				private void GraphicsLayer_MouseEnter(object sender,GraphicMouseEventArgs args)
				{
					QueryDetailsDataGrid.Focus();
					QueryDetailsDataGrid.SelectedItem = args.Graphic;
					QueryDetailsDataGrid.CurrentColumn = QueryDetailsDataGrid.Columns[0];
					QueryDetailsDataGrid.ScrollIntoView(QueryDetailsDataGrid.SelectedItem,QueryDetailsDataGrid.Columns[0]);
				}

				private void GraphicsLayer_MouseLeave(object sender,GraphicMouseEventArgs args)
				{
					QueryDetailsDataGrid.Focus();
					QueryDetailsDataGrid.SelectedItem = null;
				}

        #endregion

        #region HeaderGrid Button events

        private void PanHeaderGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PanArrowsGrid.ActualHeight > 0)
                HidePanArrows.Begin();
            else
                ShowPanArrows.Begin();
        }

        private void MapScaleHeaderGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MapScaleGrid.ActualHeight > 0)
                HideMapScale.Begin();
            else
                ShowMapScale.Begin();

        }

        private void MapZoomHeaderGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MapZoomGrid.ActualHeight > 0)
                HideMapZoom.Begin();
            else
                ShowMapZoom.Begin();
        }

        private void WorldGlobe_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Map != null)
                Map.ZoomTo(Map.Layers.GetFullExtent());
        }

        private void OverviewHeaderGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OverviewGrid.ActualHeight > 0)
                HideOverview.Begin();
            else
                ShowOverview.Begin();
        }

        private void HeaderTitleHeaderGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (HeaderTitleGrid.ActualHeight > 0)
                HideHeaderTitle.Begin();
            else
                ShowHeaderTitle.Begin();
        }

        private void ViewOptionsHeaderGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewOptionsGrid.ActualHeight > 0)
                HideViewOptions.Begin();
            else
                ShowViewOptions.Begin();

        }

        private void SpatialQueryHeaderGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SpatialQueryGrid.ActualHeight > 0)
                HideSpatialQuery.Begin();
            else
                ShowSpatialQuery.Begin();
        }

        #endregion


    }
}
