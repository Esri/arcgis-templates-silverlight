'Copyright 2013 Esri
'Licensed under the Apache License, Version 2.0 (the "License");
'You may not use this file except in compliance with the License.
'You may obtain a copy of the License at
'http://www.apache.org/licenses/LICENSE-2.0
'Unless required by applicable law or agreed to in writing, software
'distributed under the License is distributed on an "AS IS" BASIS,
'WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'See the License for the specific language governing permissions and
'limitations under the License.
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports System.ComponentModel
Imports ESRI.ArcGIS.Client
Imports ESRI.ArcGIS.Client.Geometry
Imports ESRI.ArcGIS.Client.Toolkit
Imports ESRI.ArcGIS.Client.Tasks
Imports Toolkit.Icons
Imports System.Windows.Controls.Primitives
Imports System.Windows.Data

Namespace MenuRibbonVB
  Partial Public Class MainPage
    Inherits UserControl
    Private _panFactor As Double = 0.5
    Private layerResolutions() As Double

    Private zoomSetUp As Boolean = False
    Private settingZoomLevel As Boolean = False
    Private currentLevel As Integer = 0
    Private startPoint As Point
    Private rotationAngle As Double = 0
    Private startRotationAngle As Double = 0

    Private levelGreaterThanBrush As SolidColorBrush
    Private levelLessThanBrush As SolidColorBrush
    Private levelEqualsBrush As SolidColorBrush
    Private ribbonElementBackground As LinearGradientBrush
    Private ribbonElementBackgroundHighlight As LinearGradientBrush

    Private _drawSurface As Draw
    Private _queryTask As QueryTask


    Public Sub New()
      InitializeComponent()
      Dim rd As ResourceDictionary = Application.Current.Resources
      levelGreaterThanBrush = TryCast(rd("levelGreaterThanBrush"), SolidColorBrush)
      levelLessThanBrush = TryCast(rd("levelLessThanBrush"), SolidColorBrush)
      levelEqualsBrush = TryCast(rd("levelEqualsBrush"), SolidColorBrush)
      ribbonElementBackground = TryCast(rd("RibbonElementBackground"), LinearGradientBrush)
      ribbonElementBackgroundHighlight = TryCast(rd("RibbonElementBackgroundHighlight"), LinearGradientBrush)

      _drawSurface = New Draw(Map) With {.LineSymbol = DefaultLineSymbol, .FillSymbol = DefaultFillSymbol}
      AddHandler _drawSurface.DrawComplete, AddressOf MyDrawSurface_DrawComplete

      _queryTask = New QueryTask("http://sampleserver6.arcgisonline.com/arcgis/rest/services/USA/MapServer/2")
      AddHandler _queryTask.ExecuteCompleted, AddressOf QueryTask_ExecuteCompleted
      AddHandler _queryTask.Failed, AddressOf QueryTask_Failed
    End Sub

#Region "Startup Event Handlers"

    Private Sub ArcGISTiledMapServiceLayer_Initialized(ByVal sender As Object, ByVal e As EventArgs)
      If Not zoomSetUp Then
        SetUpZoom()
      End If
    End Sub

    Private Sub Map_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
      AddHandler Map.ExtentChanged, AddressOf Map_ExtentChanged
    End Sub

#End Region


#Region "Event Handlers"

    Private Sub Map_ExtentChanged(ByVal sender As Object, ByVal args As ExtentEventArgs)
      If Not zoomSetUp Then
        SetUpZoom()
      End If
      If ZoomLevelStack IsNot Nothing Then
        currentLevel = Convert.ToInt32(Math.Round(getValueFromMap(Map.Extent)))
        Dim zs As StackPanel = ZoomLevelStack
        Dim level As Integer = currentLevel
        For i As Integer = 0 To zs.Children.Count - 1
          Dim border As Border = TryCast(zs.Children(i), Border)
          Dim brush As SolidColorBrush = setLevelBrush(i)
          border.Background = brush
        Next i
      End If

    End Sub

    Private Sub PanButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      If Map Is Nothing OrElse sender Is Nothing Then
        Return
      End If

      Dim env As Envelope = Map.Extent
      If env Is Nothing Then
        Return
      End If
      Dim x As Double = 0, y As Double = 0
      Dim oldCenter As MapPoint = env.GetCenter()
      Dim newCenter As MapPoint = Nothing
      Dim height = env.Height * _panFactor
      Dim width = env.Width * _panFactor
      ' if units are degrees (the default), limit or alter panning to the lat/lon limits
      If sender Is PanUp Then ' North
        y = oldCenter.Y + height
        newCenter = New MapPoint(oldCenter.X, y)
      ElseIf sender Is PanRight Then ' East
        x = oldCenter.X + width
        newCenter = New MapPoint(x, oldCenter.Y)
      ElseIf sender Is PanLeft Then ' West
        x = oldCenter.X - width
        newCenter = New MapPoint(x, oldCenter.Y)
      ElseIf sender Is PanDown Then ' South
        y = oldCenter.Y - height
        newCenter = New MapPoint(oldCenter.X, y)
      End If

      If newCenter IsNot Nothing Then
        Map.PanTo(newCenter)
      End If

    End Sub

    Private Sub ZoomFullExtentButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      If Map IsNot Nothing Then
        Map.ZoomTo(Map.Layers.GetFullExtent())
      End If
    End Sub

    Private Sub LayoutRoot_SizeChanged(ByVal sender As Object, ByVal e As SizeChangedEventArgs)
      MapRotationGrid.Width = MapDisplayGrid.ActualWidth
      MapRotationGrid.Height = MapDisplayGrid.ActualHeight
    End Sub

    Private Sub TabItem_MouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs)
      'TabItem item = sender as TabItem;
      'Int32 index = Convert.ToInt32(item.Tag);
      'tabRibbon.SelectedIndex = index;
    End Sub

    Private Sub Border_MouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim border As Border = TryCast(sender, Border)
      border.Background = ribbonElementBackgroundHighlight

    End Sub

    Private Sub Border_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim border As Border = TryCast(sender, Border)
      border.Background = ribbonElementBackground
    End Sub

    Private Sub ZoomStackGrid_MouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs)
      ZoomStackHighlight.Opacity = 1
    End Sub

    Private Sub ZoomStackGrid_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
      ZoomStackHighlight.Opacity = 0
    End Sub

#End Region

#Region "Zoom Level"

    Private Sub SetUpZoom()
      If ZoomLevelStack IsNot Nothing Then
        Dim resolutions As New List(Of Double)()
        For Each layer As Layer In Map.Layers
          If TypeOf layer Is TiledMapServiceLayer Then
            Dim tlayer As TiledMapServiceLayer = TryCast(layer, TiledMapServiceLayer)
            If tlayer.TileInfo Is Nothing OrElse tlayer.TileInfo.Lods Is Nothing Then
              Continue For
            End If
            Dim res = From t In tlayer.TileInfo.Lods
                      Select t.Resolution
            resolutions.AddRange(res)
          End If
        Next layer
        If resolutions.Count < 1 Then
          Return
        End If
        resolutions.Sort()
        layerResolutions = resolutions.Distinct().Reverse().ToArray()
        Dim sp As StackPanel = ZoomLevelStack
        Dim min As Integer = 0
        Dim max As Integer = layerResolutions.Length - 1
        Dim numLevels As Integer = max + 1 - min
        Dim levelWidth As Double = (100 \ numLevels)
        For i As Integer = min To max
          Dim brush As SolidColorBrush = setLevelBrush(i)
          Dim levelBorder As New Border() With {.BorderThickness = New Thickness(0.5), .CornerRadius = New CornerRadius(1), .Width = levelWidth, .Height = 14, .BorderBrush = New SolidColorBrush(Color.FromArgb(255, 0, 0, 0)), .Background = brush, .Tag = i}
          AddHandler levelBorder.MouseLeftButtonDown, AddressOf levelBorder_MouseLeftButtonDown
          AddHandler levelBorder.MouseMove, AddressOf levelBorder_MouseMove
          AddHandler levelBorder.MouseLeftButtonUp, AddressOf levelBorder_MouseLeftButtonUp
          sp.Children.Add(levelBorder)
        Next i
        AddHandler sp.MouseLeave, AddressOf sp_MouseLeave
        zoomSetUp = True
      End If
    End Sub

    Private Sub sp_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
      If settingZoomLevel Then
        FinishSettingLevel()
      End If
    End Sub

    Private Sub levelBorder_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
      FinishSettingLevel()
    End Sub

    Private Sub FinishSettingLevel()
      settingZoomLevel = False
      Map.ZoomToResolution(layerResolutions(currentLevel))

    End Sub

    Private Sub levelBorder_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
      If settingZoomLevel Then
        Dim levelBorder As Border = TryCast(sender, Border)
        Dim level As Integer = Convert.ToInt32(levelBorder.Tag)
        Dim zs As StackPanel = ZoomLevelStack
        For i As Integer = 0 To zs.Children.Count - 1
          Dim border As Border = TryCast(zs.Children(i), Border)
          Dim brush As SolidColorBrush = setLevelBrush(i)
          border.Background = brush

        Next i
        currentLevel = level
      End If
    End Sub

    Private Sub levelBorder_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
      Dim levelBorder As Border = TryCast(sender, Border)
      currentLevel = Convert.ToInt32(levelBorder.Tag)
      settingZoomLevel = True
    End Sub


    Private Function getValueFromMap(ByVal extent As ESRI.ArcGIS.Client.Geometry.Envelope) As Double
      If layerResolutions Is Nothing OrElse layerResolutions.Length = 0 OrElse Map Is Nothing OrElse extent Is Nothing Then
        Return -1
      End If
      Dim mapRes As Double = extent.Width / Map.ActualWidth
      For i As Integer = 0 To layerResolutions.Length - 2
        Dim thisRes As Double = layerResolutions(i)
        Dim nextRes As Double = layerResolutions(i + 1)
        If mapRes >= thisRes Then
          Return i
        End If
        If mapRes < thisRes AndAlso mapRes > nextRes Then
          Return i + (thisRes - mapRes) / (thisRes - nextRes)
        End If
      Next i
      Return Convert.ToDouble(layerResolutions.Length - 1)
    End Function

    Private Function setLevelBrush(ByVal level As Integer) As SolidColorBrush
      Dim brush As SolidColorBrush
      If level < currentLevel Then
        brush = levelGreaterThanBrush
      ElseIf currentLevel = level Then
        brush = levelEqualsBrush
      Else
        brush = levelLessThanBrush
      End If
      Return brush
    End Function

    Private Sub ZoomOutButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)

      Map.Zoom(0.5)
    End Sub

    Private Sub ZoomInButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      Map.Zoom(2.0)
    End Sub

#End Region

#Region "Map Rotation"

    Private Sub MapRotationGrid_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)

      Dim mapRotator As FrameworkElement = TryCast(sender, FrameworkElement)
      startPoint = e.GetPosition(MapRotationCanvas)
      startRotationAngle = Math.Atan2(startPoint.Y, startPoint.X) / Math.PI * 180
      AddHandler mapRotator.MouseMove, AddressOf mapRotator_MouseMove
      AddHandler mapRotator.MouseLeftButtonUp, AddressOf mapRotator_MouseLeftButtonUp
      AddHandler mapRotator.MouseLeave, AddressOf mapRotator_MouseLeave

    End Sub

    Private Sub mapRotator_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim mapRotator As FrameworkElement = TryCast(sender, FrameworkElement)
      finishRotation(mapRotator)
    End Sub

    Private Sub mapRotator_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
      Dim mapRotator As FrameworkElement = TryCast(sender, FrameworkElement)
      finishRotation(mapRotator)
    End Sub

    Private Sub mapRotator_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim pt As Point = e.GetPosition(MapRotationCanvas)
      Dim newAngle As Double = Math.Atan2(pt.Y, pt.X) / Math.PI * 180
      Dim diff As Double = newAngle - startRotationAngle
      Dim angle As Double = rotationAngle + diff
      Map.Rotation = angle
      RotationAngleText.Text = String.Format("{0}°", Math.Round(angle, 1))
      Dim rotate As New RotateTransform()
      rotate.Angle = angle
      RotationGlobeGrid.RenderTransform = rotate

    End Sub

    Private Sub finishRotation(ByVal mapRotator As FrameworkElement)
      RemoveHandler mapRotator.MouseMove, AddressOf mapRotator_MouseMove
      RemoveHandler mapRotator.MouseLeftButtonUp, AddressOf mapRotator_MouseLeftButtonUp
      RemoveHandler mapRotator.MouseLeave, AddressOf mapRotator_MouseLeave
      rotationAngle = Map.Rotation
    End Sub


    Private Sub ResetRotation_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      ResetMapRotation.Begin()
    End Sub

    Private Sub ResetMapRotation_Completed(ByVal sender As Object, ByVal e As EventArgs)
      ResetMapRotation.Stop()
      Map.Rotation = 0
      MapRotationTransform.Angle = 0
      RotationAngleText.Text = "0°"
      rotationAngle = 0
      RotationGlobeGrid.RenderTransform = MapRotationTransform
    End Sub

#End Region

#Region "Spatial Query"

    Private Sub SelectionTool_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      Dim button As Button = TryCast(sender, Button)
      Dim tagstring As String = CStr(button.Tag)
      Select Case tagstring
        Case "0" ' Point
          _drawSurface.DrawMode = DrawMode.Point
        Case "1" ' Polyline
          _drawSurface.DrawMode = DrawMode.Polyline
        Case "2" ' Polygon
          _drawSurface.DrawMode = DrawMode.Polygon
        Case "3" ' Rectangle
          _drawSurface.DrawMode = DrawMode.Rectangle
        Case Else ' Clear
          _drawSurface.DrawMode = DrawMode.None
          Dim selectionGraphicslayer As GraphicsLayer = TryCast(Map.Layers("MySelectionGraphicsLayer"), GraphicsLayer)
          selectionGraphicslayer.ClearGraphics()
          QueryDetailsDataGrid.ItemsSource = Nothing
          ResultsDisplay.IsExpanded = False
      End Select
      _drawSurface.IsEnabled = (_drawSurface.DrawMode <> DrawMode.None)
    End Sub


    Private Sub MyDrawSurface_DrawComplete(ByVal sender As Object, ByVal args As ESRI.ArcGIS.Client.DrawEventArgs)
      Dim selectionGraphicslayer As GraphicsLayer = TryCast(Map.Layers("MySelectionGraphicsLayer"), GraphicsLayer)
      selectionGraphicslayer.ClearGraphics()

      ' Bind data grid to query results
      Dim resultFeaturesBinding As New Binding("LastResult.Features")
      resultFeaturesBinding.Source = _queryTask
      QueryDetailsDataGrid.SetBinding(DataGrid.ItemsSourceProperty, resultFeaturesBinding)

      Dim query As Query = New ESRI.ArcGIS.Client.Tasks.Query()
      query.OutFields.AddRange(New String() {"state_name", "pop2000", "sub_region"})
      query.OutSpatialReference = Map.SpatialReference
      query.Geometry = args.Geometry
      query.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects
      query.ReturnGeometry = True

      _queryTask.ExecuteAsync(query)
    End Sub
    Private Sub QueryTask_ExecuteCompleted(ByVal sender As Object, ByVal args As ESRI.ArcGIS.Client.Tasks.QueryEventArgs)
      Dim featureSet As FeatureSet = args.FeatureSet

      If featureSet Is Nothing OrElse featureSet.Features.Count < 1 Then
        MessageBox.Show("No features retured from query")
        Return
      End If

      Dim graphicsLayer As GraphicsLayer = TryCast(Map.Layers("MySelectionGraphicsLayer"), GraphicsLayer)

      If featureSet IsNot Nothing AndAlso featureSet.Features.Count > 0 Then
        For Each feature As Graphic In featureSet.Features
          feature.Symbol = ResultsFillSymbol
          graphicsLayer.Graphics.Insert(0, feature)
        Next feature
      End If

      'ResultsDisplay.Visibility = Visibility.Visible;
      ResultsDisplay.IsExpanded = True

      _drawSurface.IsEnabled = False
    End Sub

    Private Sub QueryTask_Failed(ByVal sender As Object, ByVal args As TaskFailedEventArgs)
      MessageBox.Show("Query failed: " & args.Error.ToString())
    End Sub

    Private Sub QueryDetailsDataGrid_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
      For Each g As Graphic In e.AddedItems
        g.Select()
      Next g

      For Each g As Graphic In e.RemovedItems
        g.UnSelect()
      Next g
    End Sub

    Private Sub QueryDetailsDataGrid_LoadingRow(ByVal sender As Object, ByVal e As DataGridRowEventArgs)
      AddHandler e.Row.MouseEnter, AddressOf Row_MouseEnter
      AddHandler e.Row.MouseLeave, AddressOf Row_MouseLeave
    End Sub

    Private Sub Row_MouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs)
      TryCast((CType(sender, System.Windows.FrameworkElement)).DataContext, Graphic).Select()
    End Sub

    Private Sub Row_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim row As DataGridRow = TryCast(sender, DataGridRow)
      Dim g As Graphic = TryCast((CType(sender, System.Windows.FrameworkElement)).DataContext, Graphic)

      If Not QueryDetailsDataGrid.SelectedItems.Contains(g) Then
        g.UnSelect()
      End If
    End Sub

    Private Sub GraphicsLayer_MouseEnter(ByVal sender As Object, ByVal args As GraphicMouseEventArgs)
      QueryDetailsDataGrid.Focus()
      QueryDetailsDataGrid.SelectedItem = args.Graphic
      QueryDetailsDataGrid.CurrentColumn = QueryDetailsDataGrid.Columns(0)
      QueryDetailsDataGrid.ScrollIntoView(QueryDetailsDataGrid.SelectedItem, QueryDetailsDataGrid.Columns(0))
    End Sub

    Private Sub GraphicsLayer_MouseLeave(ByVal sender As Object, ByVal args As GraphicMouseEventArgs)
      QueryDetailsDataGrid.Focus()
      QueryDetailsDataGrid.SelectedItem = Nothing
    End Sub
#End Region
  End Class
End Namespace
