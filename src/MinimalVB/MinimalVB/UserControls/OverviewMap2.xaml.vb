Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports ESRI.ArcGIS.Client
Imports ESRI.ArcGIS.Client.Geometry

Namespace ESRI.ArcGIS.SilverlightMapApp
  ''' <summary>
  ''' OverviewMap2 Control
  ''' </summary>
  <TemplatePart(Name:="OVMapImage", Type:=GetType(Map)), TemplatePart(Name:="AOI", Type:=GetType(Grid)), TemplatePart(Name:="MapExtentBorder", Type:=GetType(Border)), TemplatePart(Name:="MapDisplayBorder", Type:=GetType(Border)), TemplatePart(Name:="MapDisplayEllipse", Type:=GetType(Ellipse)), TemplatePart(Name:="AOIprojection", Type:=GetType(PlaneProjection)), TemplatePart(Name:="MapProjection", Type:=GetType(PlaneProjection)), System.Windows.Markup.ContentProperty("Layer")>
  Public Class OverviewMap2
    Inherits Control
#Region "Private fields"

#Region "Template items"
    Private OVMapImage As Map
    Private AOI As Grid
    Private MapExtentBorder As Border
    Private MapDisplayBorder As Border
    Private AOIprojection As PlaneProjection
    Private MapProjection As PlaneProjection
    Private MapDisplayEllipse As Ellipse

#End Region

    Private mapExtent As Envelope
    Private fullExtent As Envelope
    Private lastMapExtent As New Envelope()
    Private lastOVExtent As Envelope
    Private startPoint As Point
    Private offsetLeft As Double = 0
    Private offsetTop As Double = 0
    Private dragOn As Boolean = False
    Private maxWidth As Double = 0
    Private maxHeight As Double = 0
    Private bUsePlaneProjection As Boolean = False
    Private marginLRFactor As Double = 0.125
    Private marginTBFactor As Double = 0.25
    Private bShowOversize As Boolean = False
    Private dblMapAngle As Double = 0

#End Region
    Public Sub New()
      Me.DefaultStyleKey = GetType(OverviewMap2)
    End Sub

    ''' <summary>
    ''' Static initialization for the <see cref="OverviewMap"/> control.
    ''' </summary>
    Shared Sub New()
      'DefaultStyleKeyProperty.OverrideMetadata(typeof(OverviewMap2),
      '    new FrameworkPropertyMetadata(typeof(OverviewMap2)));
    End Sub
    ''' <summary>
    ''' When overridden in a derived class, is invoked whenever application code 
    ''' or internal processes (such as a rebuilding layout pass) call
    ''' <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
    ''' </summary>
    Public Overrides Sub OnApplyTemplate()
      MyBase.OnApplyTemplate()
      Dim strvalue As String = TryCast(Application.Current.Resources("UsePlaneProjection"), String)
      If strvalue IsNot Nothing Then
        bUsePlaneProjection = Boolean.Parse(strvalue)
      End If
      If bUsePlaneProjection Then
        strvalue = TryCast(Application.Current.Resources("MapLeftRightMarginFactor"), String)
        If strvalue IsNot Nothing Then
          Dim val As Double = Double.Parse(strvalue)
          marginLRFactor = val / (1 + (2 * val))
        End If
        strvalue = TryCast(Application.Current.Resources("MapTopBottomMarginFactor"), String)
        If strvalue IsNot Nothing Then
          Dim val As Double = Double.Parse(strvalue)
          marginTBFactor = val / (1 + (2 * val))
        End If
      End If
      OVMapImage = TryCast(GetTemplateChild("OVMapImage"), Map)
      If OVMapImage Is Nothing Then
        Throw New ArgumentNullException("Template child 'OVMapImage' not found")
      End If
      OVMapImage.Width = Width
      OVMapImage.Height = Height
      AddHandler OVMapImage.ExtentChanged, Sub(s, e) UpdateAOI()
      MapExtentBorder = TryCast(GetTemplateChild("MapExtentBorder"), Border)
      MapDisplayBorder = TryCast(GetTemplateChild("MapDisplayBorder"), Border)
      AOIprojection = TryCast(GetTemplateChild("AOIprojection"), PlaneProjection)
      MapProjection = TryCast(GetTemplateChild("MapProjection"), PlaneProjection)
      MapDisplayEllipse = TryCast(GetTemplateChild("MapDisplayEllipse"), Ellipse)
      If Me.Layer IsNot Nothing Then
        Me.OVMapImage.Layers.Add(Me.Layer)
      End If

      AOI = TryCast(GetTemplateChild("AOI"), Grid)

      If AOI IsNot Nothing Then
        AddHandler AOI.MouseLeftButtonDown, AddressOf AOI_MouseLeftButtonDown
      End If

      UpdateAOI()
    End Sub

#Region "Properties"

    ''' <summary>
    ''' Identifies the <see cref="Map"/> dependency property.
    ''' </summary>
    Public Shared ReadOnly MapProperty As DependencyProperty = DependencyProperty.Register("Map", GetType(Map), GetType(OverviewMap2), New PropertyMetadata(AddressOf OnMapPropertyChanged))

    Private Shared Sub OnMapPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim ovmap As OverviewMap2 = TryCast(d, OverviewMap2)
      Dim oldMap As Map = TryCast(e.OldValue, Map)
      If oldMap IsNot Nothing Then 'clean up
        If ovmap.OVMapImage IsNot Nothing Then
          ovmap.OVMapImage.Layers.Clear()
        End If
        RemoveHandler oldMap.ExtentChanged, AddressOf ovmap.UpdateOVMap
      End If
      Dim newMap As Map = TryCast(e.NewValue, Map)
      If newMap IsNot Nothing Then
        AddHandler newMap.ExtentChanged, AddressOf ovmap.UpdateOVMap
        If ovmap.Layer IsNot Nothing AndAlso ovmap.OVMapImage IsNot Nothing Then
          ovmap.OVMapImage.Layers.Add(ovmap.Layer)
        End If
      End If
    End Sub

    ''' <summary>
    ''' Sets or gets the Map control associated with the OverviewMap.
    ''' </summary>
    Public Property Map() As Map
      Get
        Return CType(GetValue(MapProperty), Map)
      End Get
      Set(ByVal value As Map)
        SetValue(MapProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' Identifies the <see cref="MaximumExtent"/> dependency property.
    ''' </summary>
    Public Shared ReadOnly MaximumExtentProperty As DependencyProperty = DependencyProperty.Register("MaximumExtent", GetType(Envelope), GetType(OverviewMap2), Nothing)

    ''' <summary>
    ''' Gets or sets  the maximum map extent of the overview map. 
    ''' If undefined, the maximum extent is derived from the layer.
    ''' </summary>
    ''' <value>The maximum extent.</value>
    Public Property MaximumExtent() As Envelope
      Get
        Return CType(GetValue(MaximumExtentProperty), Envelope)
      End Get
      Set(ByVal value As Envelope)
        SetValue(MaximumExtentProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' Identifies the <see cref="Layer"/> dependency property.
    ''' </summary>
    Public Shared ReadOnly LayerProperty As DependencyProperty = DependencyProperty.Register("Layer", GetType(Layer), GetType(OverviewMap2), New PropertyMetadata(AddressOf OnLayerPropertyChanged))

    Private Shared Sub OnLayerPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim ovmap As OverviewMap2 = TryCast(d, OverviewMap2)
      If ovmap.OVMapImage IsNot Nothing Then
        ovmap.OVMapImage.Layers.Clear()
        If ovmap.Layer IsNot Nothing Then
          ovmap.OVMapImage.Layers.Add(ovmap.Layer)
        End If
      End If
      If ovmap.Layer IsNot Nothing Then
        Dim isInit As Boolean = ovmap.Layer.IsInitialized
        If isInit Then
          ovmap.Layer_LayersInitialized(ovmap.Layer, Nothing)
        Else
          AddHandler ovmap.Layer.Initialized, AddressOf ovmap.Layer_LayersInitialized
        End If
      End If
    End Sub

    ''' <summary>
    ''' Gets or sets the layer used in the overview map.
    ''' </summary>
    ''' <value>The layer.</value>
    Public Property Layer() As Layer
      Get
        Return CType(GetValue(LayerProperty), Layer)
      End Get
      Set(ByVal value As Layer)
        SetValue(LayerProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets the left/right margin factor used to define size of the MapDisplayBorder in the AOI.
    ''' Used for showing difference between actual oversized map extent and area visible in map view when map is not slanted (angle 0).
    ''' Only used when UsePlaneProjection is true.
    ''' Automatically set if MapLeftRightMarginFactor has been defined in application resources (ResourceDictionary).
    ''' Value should be MapLeftRightMarginFactor / (1 + (2 * MapLeftRightMarginFactor)).
    ''' Defaults to 0.25, which is proportional to the actual map's negative margin being half of the extent's width or height (0.5).
    ''' </summary>
    Public Property MarginLeftRightFactor() As Double
      Get
        Return marginLRFactor
      End Get
      Set(ByVal value As Double)
        marginLRFactor = value
      End Set
    End Property
    ''' <summary>
    ''' Gets or sets the top/bottom margin factor used to define size of the MapDisplayBorder in the AOI.
    ''' Used for showing difference between actual oversized map extent and area visible in map view when map is not slanted (angle 0).
    ''' Only used when UsePlaneProjection is true.
    ''' Automatically set if MapTopBottomMarginFactor has been defined in application resources (ResourceDictionary).
    ''' Value should be MapTopBottomMarginFactor / (1 + (2 * MapTopBottomMarginFactor)).
    ''' Defaults to 0.25, which is proportional to the actual map's negative margin being half of the extent's width or height (0.5).
    ''' </summary>
    Public Property MarginTopBottomFactor() As Double
      Get
        Return marginTBFactor
      End Get
      Set(ByVal value As Double)
        marginTBFactor = value
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets UsePlaneProjection. If true, then map is assumed to be oversized to view to accomondate filling the side edges when the map is slanted.
    ''' Automatically set if UsePlaneProjection has been defined in application resources (ResourceDictionary).
    ''' Defaults to false.
    ''' </summary>
    Public Property UsePlaneProjection() As Boolean
      Get
        Return bUsePlaneProjection
      End Get
      Set(ByVal value As Boolean)
        bUsePlaneProjection = value
      End Set
    End Property

    Public Property ShowOversize() As Boolean
      Get
        Return bShowOversize
      End Get
      Set(ByVal value As Boolean)
        bShowOversize = value
        MapExtentBorder.BorderBrush = If(bShowOversize, New SolidColorBrush(Color.FromArgb(102, 51, 51, 51)), New SolidColorBrush(Color.FromArgb(0, 255, 255, 255)))
      End Set
    End Property

    Public Property MapAngle() As Double
      Get
        Return dblMapAngle
      End Get
      Set(ByVal value As Double)
        dblMapAngle = value
        If AOIprojection IsNot Nothing Then
          AOIprojection.RotationX = dblMapAngle
        End If
        If MapProjection IsNot Nothing Then
          MapProjection.RotationX = dblMapAngle
        End If
      End Set
    End Property

    Public Property UseEllipseAOI() As Boolean
      Get
        Return CBool(GetValue(UseEllipseAOIProperty))
      End Get
      Set(ByVal value As Boolean)
        SetValue(UseEllipseAOIProperty, value)
      End Set
    End Property
    Public Shared ReadOnly UseEllipseAOIProperty As DependencyProperty = DependencyProperty.Register("UseEllipseAOI", GetType(Boolean), GetType(OverviewMap2), New PropertyMetadata(AddressOf OnUseEllipseAOIPropertyChanged))
    Private Shared Sub OnUseEllipseAOIPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim ovmap As OverviewMap2 = TryCast(d, OverviewMap2)
      If ovmap.UseEllipseAOI Then
        ovmap.MapDisplayEllipse.Visibility = Visibility.Visible
        ovmap.MapDisplayBorder.Visibility = Visibility.Collapsed
      Else
        ovmap.MapDisplayEllipse.Visibility = Visibility.Collapsed
        ovmap.MapDisplayBorder.Visibility = Visibility.Visible

      End If
    End Sub


#End Region

    ''' <summary>
    ''' Provides the behavior for the "Arrange" pass of Silverlight layout.
    ''' Classes can override this method to define their own arrange pass behavior.
    ''' </summary>
    ''' <param name="finalSize">The final area within the parent that this
    ''' object should use to arrange itself and its children.</param>
    ''' <returns>The actual size used.</returns>
    Protected Overrides Function ArrangeOverride(ByVal finalSize As Size) As Size
      Me.Clip = New RectangleGeometry() With {.Rect = New Rect(0, 0, ActualWidth, ActualHeight)}
      Return MyBase.ArrangeOverride(finalSize)
    End Function

#Region "Private Methods"

    ''' <summary>
    ''' Sets extents, limits, and events after layers have been initialized
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub Layer_LayersInitialized(ByVal sender As Object, ByVal args As EventArgs)
      fullExtent = OVMapImage.Layers.GetFullExtent()
      OVMapImage.MinimumResolution = Double.Epsilon
      OVMapImage.MaximumResolution = Double.MaxValue
      If MaximumExtent IsNot Nothing Then
        fullExtent = MaximumExtent.Clone()
        maxWidth = fullExtent.Width
        maxHeight = fullExtent.Height
        OVMapImage.ZoomTo(fullExtent)
      End If
      UpdateOVMap()
    End Sub

#Region "Methods for setting extent of OverviewMap"

    ''' <summary>
    ''' Determines if the OverviewMap extent should be changed. If so, set new 
    ''' extent and call ZoomTo or PanTo. If not, send to UpdateAOI
    ''' </summary>
    Private Sub UpdateOVMap()
      If Map Is Nothing OrElse OVMapImage Is Nothing OrElse OVMapImage.Extent Is Nothing OrElse Map.Extent Is Nothing Then
        If AOI IsNot Nothing Then
          AOI.Visibility = Visibility.Collapsed
        End If
        Return
      End If
      ' update ov extent if necessary
      Dim mapWidth As Double = Map.Extent.Width
      Dim mapHeight As Double = Map.Extent.Height
      Dim ovWidth As Double = OVMapImage.Extent.Width
      Dim ovHeight As Double = OVMapImage.Extent.Height
      Dim widthRatio As Double = mapWidth / ovWidth
      Dim heightRatio As Double = mapHeight / ovHeight
      Dim minRatio As Double = 0.15
      Dim maxRatio As Double = 0.8
      Dim extent As Envelope
      Dim sameWidthHeight As Boolean = (mapWidth = lastMapExtent.Width AndAlso mapHeight = lastMapExtent.Height)
      If sameWidthHeight Then
        Dim halfWidth As Double = ovWidth / 2
        Dim halfHeight As Double = ovHeight / 2
        Dim newCenter As MapPoint = Map.Extent.GetCenter()
        If MaximumExtent IsNot Nothing Then
          If newCenter.X - halfWidth < MaximumExtent.XMin Then
            newCenter.X = MaximumExtent.XMin + halfWidth
          End If
          If newCenter.X + halfWidth > MaximumExtent.XMax Then
            newCenter.X = MaximumExtent.XMax - halfWidth
          End If
          If newCenter.Y - halfHeight < MaximumExtent.YMin Then
            newCenter.Y = MaximumExtent.YMin + halfHeight
          End If
          If newCenter.Y + halfHeight > MaximumExtent.YMax Then
            newCenter.Y = MaximumExtent.YMax - halfHeight
          End If
        End If
        If ovWidth >= maxWidth Then
          UpdateAOI()
        Else
          If AOI IsNot Nothing Then
            AOI.Visibility = Visibility.Collapsed
          End If
          OVMapImage.PanTo(newCenter)
        End If
      ElseIf mapWidth >= maxWidth Then
        ZoomFullExtent()

      Else
        If widthRatio <= minRatio OrElse heightRatio <= minRatio OrElse widthRatio >= maxRatio OrElse heightRatio >= maxRatio Then
          'set new size around new mapextent
          If AOI IsNot Nothing Then
            AOI.Visibility = Visibility.Collapsed
          End If
          If maxWidth / 3 > mapWidth Then
            extent = New Envelope() With {.XMin = Map.Extent.XMin - mapWidth, .XMax = Map.Extent.XMax + mapWidth, .YMin = Map.Extent.YMin - mapHeight, .YMax = Map.Extent.YMax + mapHeight}
            If MaximumExtent IsNot Nothing Then
              If extent.XMin < MaximumExtent.XMin Then
                extent.XMin = MaximumExtent.XMin
              End If
              If extent.XMax > MaximumExtent.XMax Then
                extent.XMax = MaximumExtent.XMax
              End If
              If extent.YMin < MaximumExtent.YMin Then
                extent.YMin = MaximumExtent.YMin
              End If
              If extent.YMax > MaximumExtent.YMax Then
                extent.YMax = MaximumExtent.YMax
              End If
            End If
            OVMapImage.ZoomTo(extent)
          Else
            ZoomFullExtent()
          End If
        Else
          UpdateAOI()
        End If
      End If
    End Sub

    ''' <summary>
    ''' Overload of UpdateOVMap - ExtentEventHandler version
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub UpdateOVMap(ByVal sender As Object, ByVal e As ESRI.ArcGIS.Client.ExtentEventArgs)
      UpdateOVMap()
    End Sub

    Private Sub ZoomFullExtent()
      If lastOVExtent Is Nothing Then
        OVMapImage.ZoomTo(fullExtent)
      ElseIf lastOVExtent.Equals(fullExtent) Then
        UpdateAOI()
      Else
        OVMapImage.ZoomTo(fullExtent)
      End If
    End Sub

#End Region

#Region "Methods for setting size and position of AOI Box"

    ''' <summary>
    ''' Sets size and position of AOI Box
    ''' </summary>
    Private Sub UpdateAOI()
      If Map Is Nothing OrElse OVMapImage Is Nothing OrElse OVMapImage.Extent Is Nothing OrElse AOI Is Nothing Then
        Return
      End If
      Dim extent As Envelope = Map.Extent
      If extent Is Nothing Then
        AOI.Visibility = Visibility.Collapsed
        Return
      End If
      Dim pt1 As New MapPoint(extent.XMin, extent.YMax)
      Dim pt2 As New MapPoint(extent.XMax, extent.YMin)
      Dim topLeft As Point = OVMapImage.MapToScreen(pt1)
      Dim bottomRight As Point = OVMapImage.MapToScreen(pt2)
      If (Not Double.IsNaN(topLeft.X)) AndAlso (Not Double.IsNaN(topLeft.Y)) AndAlso (Not Double.IsNaN(bottomRight.X)) AndAlso (Not Double.IsNaN(bottomRight.Y)) Then
        AOI.Margin = New Thickness(topLeft.X, topLeft.Y, 0, 0)
        AOI.Width = bottomRight.X - topLeft.X
        AOI.Height = bottomRight.Y - topLeft.Y
        AOI.Visibility = Visibility.Visible
        ' the next if added for oversized map using plane projection
        If bUsePlaneProjection Then
          Dim mwidth As Double = AOI.Width * marginLRFactor
          Dim mheight As Double = AOI.Height * marginTBFactor
          MapDisplayBorder.Margin = New Thickness(mwidth, mheight - 1, mwidth, mheight + 1)
          'AOIprojection.RotationX = mapAngle;
          MapProjection.RotationX = dblMapAngle
        End If
      Else
        AOI.Visibility = Visibility.Collapsed
      End If
      lastMapExtent = extent
      lastOVExtent = OVMapImage.Extent.Clone()
    End Sub

#End Region

#Region "Method for setting extent of Map"

    ''' <summary>
    ''' Set new map extent of main map control. Called after AOI
    ''' Box has been repositioned by user
    ''' </summary>
    Private Sub UpdateMap()
      If AOI Is Nothing Then
        Return
      End If
      mapExtent = Map.Extent
      Dim aoiLeft As Double = AOI.Margin.Left
      Dim aoiTop As Double = AOI.Margin.Top
      Dim pt As MapPoint = OVMapImage.ScreenToMap(New Point(aoiLeft, aoiTop))
      Dim mapHalfWidth As Double = mapExtent.Width \ 2
      Dim mapHalfHeight As Double = mapExtent.Height \ 2
      Dim pnt As New MapPoint(pt.X + mapHalfWidth, pt.Y - mapHalfHeight)
      Map.PanTo(pnt)
    End Sub

#End Region

#Region "AOI Box Mouse handlers"
    Private Sub AOI_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
      dragOn = True
      startPoint = e.GetPosition(Me)
      offsetLeft = startPoint.X - AOI.Margin.Left
      offsetTop = startPoint.Y - AOI.Margin.Top
      AddHandler AOI.MouseMove, AddressOf AOI_MouseMove
      AddHandler AOI.MouseLeftButtonUp, AddressOf AOI_MouseLeftButtonUp
      AOI.CaptureMouse()
    End Sub

    Private Sub AOI_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
      If dragOn Then
        RemoveHandler AOI.MouseMove, AddressOf AOI_MouseMove
        RemoveHandler AOI.MouseLeftButtonUp, AddressOf AOI_MouseLeftButtonUp
        UpdateMap()
        dragOn = False
        AOI.ReleaseMouseCapture()
      End If
    End Sub

    Private Sub AOI_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
      If dragOn Then
        Dim pos As Point = e.GetPosition(Me)
        AOI.Margin = New Thickness(pos.X - offsetLeft, pos.Y - offsetTop, 0, 0)
      End If
    End Sub

#End Region

#End Region


  End Class
End Namespace
