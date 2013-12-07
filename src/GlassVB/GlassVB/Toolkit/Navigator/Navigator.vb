Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports ESRI.ArcGIS.Client
Imports ESRI.ArcGIS.Client.Geometry
Imports ESRI.ArcGIS.Client.Toolkit
Imports Toolkit.Icons

Namespace Toolkit
  ''' <summary>
  ''' Navigator control supporting pan, zoom and rotation.
  ''' </summary>
  <TemplatePart(Name:="ResetRotationGrid", Type:=GetType(FrameworkElement)), TemplatePart(Name:="NavProjection", Type:=GetType(PlaneProjection)), TemplatePart(Name:="SlantSlider", Type:=GetType(Slider)), TemplatePart(Name:="SlantSliderGrid", Type:=GetType(FrameworkElement)), TemplatePart(Name:="ZoomGrid_", Type:=GetType(FrameworkElement)), TemplatePart(Name:="ZoomGridBorder1", Type:=GetType(Border)), TemplatePart(Name:="ZoomGridBorder2", Type:=GetType(Border)), TemplatePart(Name:="SlantText_", Type:=GetType(TextBlock)), TemplatePart(Name:="GlobeGlass", Type:=GetType(FrameworkElement)), TemplatePart(Name:="ZoomGridStack", Type:=GetType(StackPanel)), TemplatePart(Name:="BottomZoomWingUpDownExpand", Type:=GetType(UpDownExpand)), TemplatePart(Name:="TopZoomWingUpDownExpand", Type:=GetType(UpDownExpand)), TemplatePart(Name:="TopZoomWingCollapseButton", Type:=GetType(Button)), TemplatePart(Name:="BottomZoomWingCollapseButton", Type:=GetType(Button)), TemplatePart(Name:="ExpandZoomBar", Type:=GetType(Storyboard)), TemplatePart(Name:="ShrinkZoomBar", Type:=GetType(Storyboard)), TemplatePart(Name:="ExpandProgressBarSpacer", Type:=GetType(Storyboard)), TemplatePart(Name:="ShrinkProgressBarSpacer", Type:=GetType(Storyboard)), TemplatePart(Name:="ProgressBarSpacer", Type:=GetType(FrameworkElement)), TemplatePart(Name:="ScaleBarBlock", Type:=GetType(FrameworkElement)), TemplatePart(Name:="PaddingLeftForScaleBarTextMeters", Type:=GetType(FrameworkElement)), TemplatePart(Name:="PaddingLeftTopNotch", Type:=GetType(FrameworkElement)), TemplatePart(Name:="PaddingLeftForScaleBarTextMiles", Type:=GetType(FrameworkElement)), TemplatePart(Name:="PaddingLeftBottomNotch", Type:=GetType(FrameworkElement)), TemplatePart(Name:="ScaleBarTextForMeters", Type:=GetType(TextBlock)), TemplatePart(Name:="ScaleBarTextForMiles", Type:=GetType(TextBlock)), TemplatePart(Name:="saleBar", Type:=GetType(FrameworkElement)), TemplatePart(Name:="root", Type:=GetType(FrameworkElement)), TemplatePart(Name:="NavOpacitySliderGrid", Type:=GetType(FrameworkElement)), TemplatePart(Name:="NavOpacitySlider", Type:=GetType(Slider)), TemplatePart(Name:="OpacityText", Type:=GetType(FrameworkElement)), TemplatePart(Name:="progressBarGrid", Type:=GetType(FrameworkElement)), TemplatePart(Name:="ScaleBarInnerBorder", Type:=GetType(Border)), TemplatePart(Name:="ScaleBarWingCollapseButton", Type:=GetType(Button)), TemplatePart(Name:="ScaleBarWingCollapseImage", Type:=GetType(Image)), TemplatePart(Name:="ScaleBarOuterBorder", Type:=GetType(Border)), TemplatePart(Name:="progressBar", Type:=GetType(MapProgressBar)), TemplatePart(Name:="ScaleBarWingLeftRightExpand", Type:=GetType(LeftRightExpand))>
  Public Class Navigator
    Inherits Navigation
#Region "private fields"

    Private ResetRotationGrid As FrameworkElement
    Private ScaleBarBlock As FrameworkElement
    Private PaddingLeftForScaleBarTextMeters As FrameworkElement
    Private ScaleBarTextForMeters As TextBlock
    Private PaddingLeftTopNotch As FrameworkElement
    Private PaddingLeftForScaleBarTextMiles As FrameworkElement
    Private PaddingLeftBottomNotch As FrameworkElement
    Private ScaleBar As FrameworkElement
    Private root As FrameworkElement
    Private NavOpacitySliderGrid As FrameworkElement
    Private NavOpacitySlider As Slider
    Private OpacityText As FrameworkElement
    Private progressBarGrid As FrameworkElement
    Private ScaleBarInnerBorder As Border
    Private ScaleBarWingCollapseButton As Button
    Private ScaleBarWingCollapseImage As Image
    Private ScaleBarOuterBorder As Border
    Private progressBar As MapProgressBar

    Private ScaleBarTextForMiles As TextBlock

    Private levelGreaterThanBrush As SolidColorBrush
    Private levelLessThanBrush As SolidColorBrush
    Private levelEqualsBrush As SolidColorBrush

    Private projection As PlaneProjection
    Private NavProjection As PlaneProjection
    Private SlantSlider As Slider
    Private SlantSliderGrid As FrameworkElement
    Private NavCircle As Border
    Private NavCircleScale As ScaleTransform
    Private ZoomGrid As FrameworkElement
    Private ZoomGridOuterBorder As Border
    Private ZoomGridInnerBorder As Border
    Private SlantText As TextBlock
    Private SlantGlass As FrameworkElement
    Private ZoomGridStack As StackPanel
    Private BottomZoomWingUpDownExpand As UpDownExpand
    Private TopZoomWingUpDownExpand As UpDownExpand
    Private TopZoomCollapseButton As Button
    Private BottomZoomCollapseButton As Button
    Private ExpandZoomBar As Storyboard
    Private ShrinkZoomBar As Storyboard
    Private ExpandProgressBarSpacer As Storyboard
    Private ShrinkProgressBarSpacer As Storyboard
    Private ProgressBarSpacer As FrameworkElement
    Private ScaleBarWingLeftRightExpand As LeftRightExpand

    Private navControl As Navigator
    Private mapCenter As MapPoint
    Private startPoint As Point
    Private lastY As Double = 0
    Private bottomAligned As Boolean = False
    Private usePlaneProjection As Boolean = False
    'INSTANT VB NOTE: The variable mapAngle was renamed since Visual Basic does not allow class members with the same name:
    Private mapAngle_Renamed As Double = 0
    Private mapWidth As Double = 0
    Private mapHeight As Double = 0

    Public Property MapProjection() As PlaneProjection


#End Region

    Public Sub New()
      Me.DefaultStyleKey = GetType(Navigator)
    End Sub

    ''' <summary>
    ''' When overridden in a derived class, is invoked whenever application code or
    ''' internal processes (such as a rebuilding layout pass) call
    ''' <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
    ''' </summary>
    Public Overrides Sub OnApplyTemplate()
      navControl = Me
      MyBase.OnApplyTemplate()
      ResetRotationGrid = TryCast(GetTemplateChild("ResetRotationGrid"), FrameworkElement)
      projection = TryCast(GetTemplateChild("NavProjection"), PlaneProjection)
      SlantSlider = TryCast(GetTemplateChild("SlantSlider"), Slider)
      SlantSliderGrid = TryCast(GetTemplateChild("SlantSliderGrid"), FrameworkElement)
      NavCircle = TryCast(GetTemplateChild("NavCircle"), Border)
      NavCircleScale = TryCast(GetTemplateChild("NavCircleScale"), ScaleTransform)
      ZoomGrid = TryCast(GetTemplateChild("ZoomGrid_"), FrameworkElement)
      ZoomGridOuterBorder = TryCast(GetTemplateChild("ZoomGridBorder1"), Border)
      ZoomGridInnerBorder = TryCast(GetTemplateChild("ZoomGridBorder2"), Border)
      SlantText = TryCast(GetTemplateChild("SlantText_"), TextBlock)
      SlantGlass = TryCast(GetTemplateChild("GlobeGlass"), FrameworkElement)
      'ZoomLevelStack = GetTemplateChild("ZoomLevelStack") as StackPanel;
      ZoomGridStack = TryCast(GetTemplateChild("ZoomGridStack"), StackPanel)
      BottomZoomWingUpDownExpand = TryCast(GetTemplateChild("BottomZoomWingUpDownExpand"), UpDownExpand)
      TopZoomWingUpDownExpand = TryCast(GetTemplateChild("TopZoomWingUpDownExpand"), UpDownExpand)
      BottomZoomCollapseButton = TryCast(GetTemplateChild("BottomZoomWingCollapseButton"), Button)
      TopZoomCollapseButton = TryCast(GetTemplateChild("TopZoomWingCollapseButton"), Button)
      ExpandZoomBar = TryCast(GetTemplateChild("ExpandZoomBar"), Storyboard)
      ShrinkZoomBar = TryCast(GetTemplateChild("ShrinkZoomBar"), Storyboard)
      ExpandProgressBarSpacer = TryCast(GetTemplateChild("ExpandProgressBarSpacer"), Storyboard)
      ShrinkProgressBarSpacer = TryCast(GetTemplateChild("ShrinkProgressBarSpacer"), Storyboard)
      ProgressBarSpacer = TryCast(GetTemplateChild("ProgressBarSpacer"), FrameworkElement)
      NavProjection = TryCast(GetTemplateChild("NavProjection"), PlaneProjection)

      ScaleBarBlock = TryCast(GetTemplateChild("ScaleBarBlock"), FrameworkElement)
      PaddingLeftForScaleBarTextMeters = TryCast(GetTemplateChild("PaddingLeftForScaleBarTextMeters"), FrameworkElement)
      PaddingLeftTopNotch = TryCast(GetTemplateChild("PaddingLeftTopNotch"), FrameworkElement)
      PaddingLeftForScaleBarTextMiles = TryCast(GetTemplateChild("PaddingLeftForScaleBarTextMiles"), FrameworkElement)
      PaddingLeftBottomNotch = TryCast(GetTemplateChild("PaddingLeftBottomNotch"), FrameworkElement)
      ScaleBarTextForMeters = TryCast(GetTemplateChild("ScaleBarTextForMeters"), TextBlock)
      ScaleBarTextForMiles = TryCast(GetTemplateChild("ScaleBarTextForMiles"), TextBlock)
      ScaleBar = TryCast(GetTemplateChild("scaleBar"), FrameworkElement)
      root = TryCast(GetTemplateChild("root"), FrameworkElement)
      NavOpacitySliderGrid = TryCast(GetTemplateChild("NavOpacitySliderGrid"), FrameworkElement)
      NavOpacitySlider = TryCast(GetTemplateChild("NavOpacitySlider"), Slider)
      OpacityText = TryCast(GetTemplateChild("OpacityText"), FrameworkElement)
      progressBarGrid = TryCast(GetTemplateChild("progressBarGrid"), FrameworkElement)
      ScaleBarInnerBorder = TryCast(GetTemplateChild("ScaleBarInnerBorder"), Border)
      ScaleBarWingCollapseButton = TryCast(GetTemplateChild("ScaleBarWingCollapseButton"), Button)
      ScaleBarWingCollapseImage = TryCast(GetTemplateChild("ScaleBarWingCollapseImage"), Image)
      ScaleBarOuterBorder = TryCast(GetTemplateChild("ScaleBarOuterBorder"), Border)
      progressBar = TryCast(GetTemplateChild("progressBar"), MapProgressBar)
      ScaleBarWingLeftRightExpand = TryCast(GetTemplateChild("ScaleBarWingLeftRightExpand"), LeftRightExpand)

      If ExpandProgressBarSpacer IsNot Nothing Then
        AddHandler ExpandProgressBarSpacer.Completed, AddressOf expandprogressbarSpacer_Completed
      End If
      If ExpandZoomBar IsNot Nothing Then
        AddHandler ExpandZoomBar.Completed, AddressOf ExpandZoomBar_Completed
      End If
      If ShrinkZoomBar IsNot Nothing Then
        AddHandler ShrinkZoomBar.Completed, AddressOf ShrinkZoomBar_Completed
      End If
      If ScaleBarWingCollapseButton IsNot Nothing Then
        AddHandler ScaleBarWingCollapseButton.Click, AddressOf ScaleBarWingCollapseButton_Click
      End If
      If BottomZoomCollapseButton IsNot Nothing Then
        AddHandler BottomZoomCollapseButton.Click, AddressOf ZoomWingCollapseButton_Click
      End If
      If TopZoomCollapseButton IsNot Nothing Then
        AddHandler TopZoomCollapseButton.Click, AddressOf ZoomWingCollapseButton_Click
      End If
      If SlantGlass IsNot Nothing Then
        AddHandler SlantGlass.MouseLeftButtonDown, AddressOf GlobeGlass_MouseLeftButtonDown
      End If
      If SlantSlider IsNot Nothing Then
        AddHandler SlantSlider.ValueChanged, AddressOf SlantSlider_ValueChanged
      End If
      If NavOpacitySlider IsNot Nothing Then
        AddHandler NavOpacitySlider.ValueChanged, AddressOf NavOpacitySlider_ValueChanged
      End If
      Dim rd As ResourceDictionary = root.Resources
      ' try local resources first for zoom level brushes
      levelGreaterThanBrush = TryCast(rd("levelGreaterThanBrush"), SolidColorBrush)
      levelLessThanBrush = TryCast(rd("levelLessThanBrush"), SolidColorBrush)
      levelEqualsBrush = TryCast(rd("levelEqualsBrush"), SolidColorBrush)
      If levelGreaterThanBrush Is Nothing Then
        ' if local resources not used, try the application global resources
        rd = Application.Current.Resources
        levelGreaterThanBrush = TryCast(rd("levelGreaterThanBrush"), SolidColorBrush)
        levelLessThanBrush = TryCast(rd("levelLessThanBrush"), SolidColorBrush)
        levelEqualsBrush = TryCast(rd("levelEqualsBrush"), SolidColorBrush)
      End If

      Dim va As VerticalAlignment = Me.VerticalAlignment
      If va = VerticalAlignment.Bottom Then
        bottomAligned = True
        ScaleBarOuterBorder.CornerRadius = New CornerRadius(0, 21, 0, 0)
        ScaleBarOuterBorder.VerticalAlignment = VerticalAlignment.Bottom
        ScaleBarInnerBorder.CornerRadius = New CornerRadius(0, 20, 0, 0)
        OpacityText.VerticalAlignment = VerticalAlignment.Bottom
        NavOpacitySliderGrid.VerticalAlignment = VerticalAlignment.Bottom

        ZoomGridOuterBorder.CornerRadius = New CornerRadius(0, 21, 0, 0)
        ZoomGridInnerBorder.CornerRadius = New CornerRadius(0, 20, 0, 0)
        ZoomGrid.VerticalAlignment = VerticalAlignment.Bottom
        ZoomGridOuterBorder.VerticalAlignment = VerticalAlignment.Top
        ZoomGridInnerBorder.Padding = New Thickness(5, 5, 5, 110)
        NavCircle.VerticalAlignment = VerticalAlignment.Bottom
        NavCircle.RenderTransformOrigin = New Point(0, 1)
        SlantSliderGrid.Margin = New Thickness(85, 0, 0, 6)
        SlantSliderGrid.VerticalAlignment = VerticalAlignment.Bottom
        SlantText.VerticalAlignment = VerticalAlignment.Bottom
        ResetRotationGrid.VerticalAlignment = VerticalAlignment.Bottom
        BottomZoomCollapseButton.Visibility = Visibility.Collapsed
        TopZoomCollapseButton.Visibility = Visibility.Visible
      End If
      Dim strvalue As String = TryCast(Application.Current.Resources("UsePlaneProjection"), String)
      If strvalue IsNot Nothing Then
        usePlaneProjection = Boolean.Parse(strvalue)
      End If
      If mapAngle_Renamed <> 0 AndAlso usePlaneProjection Then
        SlantSlider.Value = mapAngle_Renamed
      End If
      If Me.Map IsNot Nothing Then
        AddHandler Map.ExtentChanged, AddressOf Map_ExtentChanged
        AddHandler Map.Progress, AddressOf projectionCheck
        AddHandler Map.Progress, AddressOf progressBarCheck
      End If

    End Sub



#Region "Properties"



    ''' <summary>
    ''' Gets or sets angle of map view. Ignored if UseMapProjection is set to False.
    ''' </summary>
    Public Property MapAngle() As Double
      Get
        Return CDbl(GetValue(MapAngleProperty))
      End Get
      Set(ByVal value As Double)
        SetValue(MapAngleProperty, value)
      End Set
    End Property

    Public Shared ReadOnly MapAngleProperty As DependencyProperty = DependencyProperty.Register("MapAngle", GetType(Double), GetType(Navigator), New PropertyMetadata(0.0R, AddressOf OnMapAnglePropertyChanged))

    Private Shared Sub OnMapAnglePropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim n As Navigator = TryCast(d, Navigator)
      Dim angle As Double = CDbl(e.NewValue)
      If Not Double.IsNaN(angle) Then
        n.mapAngle_Renamed = angle
        If n.SlantSlider IsNot Nothing Then
          n.SlantSlider.Value = angle
        End If

      End If
    End Sub

    ''' <summary>
    ''' Identifies the <see cref="FillColor1"/> dependency property.
    ''' </summary>
    Public Shared ReadOnly TargetWidthProperty As DependencyProperty = DependencyProperty.Register("TargetWidth", GetType(Double), GetType(ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit), New PropertyMetadata(150.0))

    ''' <summary>
    ''' Gets or sets the target width of the scale bar.
    ''' </summary>
    ''' <remarks>The actual width of the scale bar changes when values are rounded.</remarks>
    Public Property TargetWidth() As Double
      Get
        Return CDbl(GetValue(TargetWidthProperty))
      End Get
      Set(ByVal value As Double)
        SetValue(TargetWidthProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets the map unit.
    ''' </summary>
    Public Property MapUnit() As ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit

#End Region


#Region "Setup Methods"


    Private Sub map_Progress(ByVal sender As Object, ByVal e As ProgressEventArgs)

    End Sub

    ' sets map of progressbar
    Private Sub progressBarCheck(ByVal sender As Object, ByVal e As ProgressEventArgs)
      If progressBar IsNot Nothing Then
        If progressBar.Map Is Nothing Then
          progressBar.Map = navControl.Map
          RemoveHandler navControl.Map.Progress, AddressOf progressBarCheck
        End If
      End If
    End Sub

    ' sets up use of plane projection
    Private Sub projectionCheck(ByVal sender As Object, ByVal e As ProgressEventArgs)
      If SlantSliderGrid IsNot Nothing Then
        If usePlaneProjection Then
          If MapProjection IsNot Nothing Then
            SlantSliderGrid.Visibility = Visibility.Visible
            SlantGlass.Visibility = Visibility.Visible
            SlantText.Visibility = Visibility.Visible
            SlantSlider.Value = mapAngle_Renamed
          Else
            SlantSliderGrid.Visibility = Visibility.Collapsed
            SlantGlass.Visibility = Visibility.Collapsed
            SlantText.Visibility = Visibility.Collapsed
          End If
        Else
          SlantSliderGrid.Visibility = Visibility.Collapsed
          SlantGlass.Visibility = Visibility.Collapsed
          SlantText.Visibility = Visibility.Collapsed
          usePlaneProjection = False
        End If
        RemoveHandler Map.Progress, AddressOf projectionCheck
      End If

    End Sub

    Private Sub UserControl_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
      Dim va As VerticalAlignment = Me.VerticalAlignment
      If va = VerticalAlignment.Bottom Then
        bottomAligned = True
        ScaleBarOuterBorder.CornerRadius = New CornerRadius(0, 21, 0, 0)
        ScaleBarOuterBorder.VerticalAlignment = VerticalAlignment.Bottom
        ScaleBarInnerBorder.CornerRadius = New CornerRadius(0, 20, 0, 0)
        OpacityText.VerticalAlignment = VerticalAlignment.Bottom
        NavOpacitySliderGrid.VerticalAlignment = VerticalAlignment.Bottom
      End If
    End Sub
#End Region


#Region "Events"
    ' Event handler for opacity slider
    Private Sub NavOpacitySlider_ValueChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Double))
      If root IsNot Nothing Then
        root.Opacity = e.NewValue
      End If
    End Sub

    Private Sub Map_ExtentChanged(ByVal sender As Object, ByVal args As ExtentEventArgs)

      refreshScalebar()

    End Sub

#End Region


#Region "Map Slant"

    Private Sub SlantSlider_ValueChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Double))
      If Map IsNot Nothing Then
        Dim angle As Double = e.NewValue
        NavProjection.RotationX = angle
        MapProjection.RotationX = angle
        'if (OverviewMap2 != null) OverviewMap2.MapAngle = angle;
      End If
    End Sub

    Private Sub SlantSlider_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
      If usePlaneProjection Then
        SlantSlider.Value = mapAngle_Renamed
      End If
    End Sub

    Private Sub SlantSlider_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
      mapCenter = Map.Extent.GetCenter()
    End Sub

    Private Sub SlantSlider_LostMouseCapture(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim slider As Slider = TryCast(sender, Slider)
      Dim rt As New RotateTransform()
      Dim pp As New PlaneProjection()
      If mapWidth > 0 AndAlso mapHeight > 0 AndAlso slider.Value < 0 Then
        Dim wmargin As Double = mapWidth * -1
        Dim hmargin As Double = (mapHeight) * -1
        Map.Margin = New Thickness(wmargin, hmargin, wmargin, hmargin)

        pp.CenterOfRotationX = mapWidth * 1.5
        pp.CenterOfRotationY = mapHeight * 1.5
        Map.Projection = pp
      Else
        Map.Margin = New Thickness(0)
        pp.CenterOfRotationX = mapWidth * 0.5
        pp.CenterOfRotationY = mapHeight * 0.5
        Map.Projection = pp
      End If

      Map.PanTo(mapCenter)
    End Sub

    Private Sub GlobeGlass_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
      startPoint = e.GetPosition(Nothing)
      lastY = startPoint.Y
      AddHandler SlantGlass.MouseMove, AddressOf SlantGlass_MouseMove
      AddHandler SlantGlass.MouseLeftButtonUp, AddressOf SlantGlass_MouseLeftButtonUp
      AddHandler SlantGlass.MouseLeave, AddressOf SlantGlass_MouseLeave
    End Sub

    Private Sub SlantGlass_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
      RemoveHandler SlantGlass.MouseMove, AddressOf SlantGlass_MouseMove
      RemoveHandler SlantGlass.MouseLeftButtonUp, AddressOf SlantGlass_MouseLeftButtonUp
      RemoveHandler SlantGlass.MouseLeave, AddressOf SlantGlass_MouseLeave
    End Sub

    Private Sub SlantGlass_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
      RemoveHandler SlantGlass.MouseMove, AddressOf SlantGlass_MouseMove
      RemoveHandler SlantGlass.MouseLeftButtonUp, AddressOf SlantGlass_MouseLeftButtonUp

    End Sub

    Private Sub SlantGlass_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim pt As Point = e.GetPosition(Nothing)
      Dim diff As Double = pt.Y - lastY
      Dim slant As Double = SlantSlider.Value
      slant += diff
      Dim newSlant As Double = slant
      If newSlant <= SlantSlider.Maximum AndAlso newSlant >= SlantSlider.Minimum Then
        SlantSlider.Value = newSlant
      End If
    End Sub
#End Region


#Region "Wing methods"
    Private Sub ZoomWingCollapseButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      Dim zes As StackPanel = ZoomGridStack
      Dim vis As Visibility = zes.Visibility
      BottomZoomWingUpDownExpand.Visibility = Visibility.Collapsed
      TopZoomWingUpDownExpand.Visibility = Visibility.Collapsed
      If vis = Visibility.Collapsed Then
        ExpandZoomBar.Begin()
        TopZoomWingUpDownExpand.IsPointingDown = True
        BottomZoomWingUpDownExpand.IsPointingDown = False

      Else
        NavOpacitySliderGrid.Visibility = Visibility.Collapsed
        OpacityText.Visibility = Visibility.Collapsed
        zes.Visibility = Visibility.Collapsed
        ZoomGridInnerBorder.Padding = If(bottomAligned, New Thickness(5, 0, 5, 70), New Thickness(5, 70, 5, 0))
        TopZoomWingUpDownExpand.IsPointingDown = False
        BottomZoomWingUpDownExpand.IsPointingDown = True
        ShrinkZoomBar.Begin()
      End If
    End Sub

    Private Sub ScaleBarWingCollapseButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      Dim vis As Visibility = ScaleBar.Visibility

      If vis = Visibility.Collapsed Then
        ExpandProgressBarSpacer.Begin()
        ScaleBarWingLeftRightExpand.IsPointingRight = False
      Else
        ScaleBar.Visibility = Visibility.Collapsed
        progressBarGrid.Visibility = Visibility.Collapsed
        SlantSliderGrid.Visibility = Visibility.Collapsed
        SlantText.Visibility = Visibility.Collapsed
        ScaleBarWingLeftRightExpand.IsPointingRight = True

        ShrinkProgressBarSpacer.Begin()
        ScaleBarInnerBorder.Padding = New Thickness(70, 2, 0, 2)
      End If
    End Sub

    Private Sub expandprogressbarSpacer_Completed(ByVal sender As Object, ByVal e As EventArgs)
      ScaleBar.Visibility = Visibility.Visible
      progressBarGrid.Visibility = Visibility.Visible
      If usePlaneProjection Then
        SlantSliderGrid.Visibility = Visibility.Visible
        SlantText.Visibility = Visibility.Visible
      End If
      ScaleBarInnerBorder.Padding = New Thickness(100, 2, 0, 2)
    End Sub

    Private Sub shrinkprogressbarSpacer_Completed(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Private Sub ExpandZoomBar_Completed(ByVal sender As Object, ByVal e As EventArgs)
      ZoomGridStack.Visibility = Visibility.Visible
      NavOpacitySliderGrid.Visibility = Visibility.Visible
      OpacityText.Visibility = Visibility.Visible
      ZoomGridInnerBorder.Padding = If(bottomAligned, New Thickness(5, 0, 5, 110), New Thickness(5, 110, 5, 0))
      If bottomAligned Then
        TopZoomWingUpDownExpand.Visibility = Visibility.Visible
      Else
        BottomZoomWingUpDownExpand.Visibility = Visibility.Visible
      End If

    End Sub

    Private Sub ShrinkZoomBar_Completed(ByVal sender As Object, ByVal e As EventArgs)
      If bottomAligned Then
        TopZoomWingUpDownExpand.Visibility = Visibility.Visible
      Else
        BottomZoomWingUpDownExpand.Visibility = Visibility.Visible
      End If

    End Sub

#End Region


#Region "ScaleBar Helper Functions"

    Private Sub refreshScalebar()
      Dim viz As Visibility = Visibility.Collapsed
      Dim canDisplay As Boolean = True
      If ScaleBar IsNot Nothing Then
        viz = ScaleBar.Visibility
        If Map Is Nothing OrElse Double.IsNaN(Map.Resolution) OrElse MapUnit = ScaleLine.ScaleLineUnit.DecimalDegrees AndAlso Math.Abs(Map.Extent.GetCenter().Y) >= 90 Then
          viz = Visibility.Collapsed
          canDisplay = False
        End If
        If ScaleBar IsNot Nothing Then
          ScaleBar.Visibility = viz
        End If
      End If
      If Not canDisplay Then
        Return
      End If

      Dim outUnit As ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Undefined
      Dim outResolution As Double

      '						#Region "KiloMeters/Meters"
      Dim roundedKiloMeters As Double = getBestEstimateOfValue(Map.Resolution, ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Kilometers, outUnit, outResolution)
      Dim widthMeters As Double = roundedKiloMeters / outResolution
      Dim inMeters As Boolean = outUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Meters

      If PaddingLeftForScaleBarTextMeters IsNot Nothing Then
        PaddingLeftForScaleBarTextMeters.Width = widthMeters
      End If
      If PaddingLeftTopNotch IsNot Nothing Then
        PaddingLeftTopNotch.Width = widthMeters
      End If
      If ScaleBarTextForMeters IsNot Nothing Then
        ScaleBarTextForMeters.Text = String.Format("{0}{1}", roundedKiloMeters, (If(inMeters, "m", "km")))
        ScaleBarTextForMeters.Width = widthMeters
      End If
      '						#End Region

      '						#Region "Miles"
      Dim roundedMiles As Double = getBestEstimateOfValue(Map.Resolution, ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Miles, outUnit, outResolution)
      Dim widthMiles As Double = roundedMiles / outResolution
      Dim inFeet As Boolean = outUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Feet

      If PaddingLeftForScaleBarTextMiles IsNot Nothing Then
        PaddingLeftForScaleBarTextMiles.Width = widthMiles
      End If
      If PaddingLeftBottomNotch IsNot Nothing Then
        PaddingLeftBottomNotch.Width = widthMiles
      End If
      If ScaleBarTextForMiles IsNot Nothing Then
        ScaleBarTextForMiles.Text = String.Format("{0}{1}", roundedMiles, If(inFeet, "ft", "mi"))
        ScaleBarTextForMiles.Width = widthMiles
      End If
      '						#End Region

      Dim widthOfNotches As Double = 4 ' 2 for left notch, 2 for right notch
      Dim scaleBarBlockWidth As Double = If(widthMiles > widthMeters, widthMiles, widthMeters)
      scaleBarBlockWidth += widthOfNotches

      If (Not Double.IsNaN(scaleBarBlockWidth)) AndAlso ScaleBarBlock IsNot Nothing Then
        ScaleBarBlock.Width = scaleBarBlockWidth
      End If

    End Sub

    Private Function getBestEstimateOfValue(ByVal resolution As Double, ByVal displayUnit As ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit, <System.Runtime.InteropServices.Out()> ByRef unit As ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit, <System.Runtime.InteropServices.Out()> ByRef outResolution As Double) As Double
      unit = displayUnit
      Dim rounded As Double = 0
      Dim originalRes As Double = resolution
      Do While rounded < 0.5
        resolution = originalRes
        If MapUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.DecimalDegrees Then
          resolution = getResolutionForGeographic(Map.Extent, resolution)
          resolution = resolution * CInt(Fix(ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Meters)) / CInt(Fix(unit))
        ElseIf MapUnit <> ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Undefined Then
          resolution = resolution * CInt(Fix(MapUnit)) / CInt(Fix(unit))
        End If

        Dim val As Double = TargetWidth * resolution
        val = roundToSignificant(val, resolution)
        Dim noFrac As Double = Math.Round(val) ' to get rid of the fraction
        If val < 0.5 Then
          Dim newUnit As ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Undefined
          ' Automatically switch unit to a lower one
          If unit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Kilometers Then
            newUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Meters
          ElseIf unit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Miles Then
            newUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Feet
          End If
          If newUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Undefined Then 'no lower unit
            Exit Do
          End If
          unit = newUnit
        ElseIf noFrac > 1 Then
          rounded = noFrac
          Dim len = noFrac.ToString().Length
          If len <= 2 Then
            ' single/double digits ... make it a multiple of 5 ..or 1,2,3,4
            If noFrac > 5 Then
              rounded -= noFrac Mod 5
            End If
            Do While rounded > 1 AndAlso (rounded / resolution) > TargetWidth
              ' exceeded maxWidth .. decrement by 1 or by 5
              Dim decr As Double = If(noFrac > 5, 5, 1)
              rounded = rounded - decr
            Loop
          ElseIf len > 2 Then
            rounded = Math.Round(noFrac / Math.Pow(10, len - 1)) * Math.Pow(10, len - 1)
            If (rounded / resolution) > TargetWidth Then
              ' exceeded maxWidth .. use the lower bound instead
              rounded = Math.Floor(noFrac / Math.Pow(10, len - 1)) * Math.Pow(10, len - 1)
            End If
          End If
        Else
          rounded = Math.Floor(val)
          If rounded = 0 Then
            'val >= 0.5 but < 1 so round up
            rounded = If(val = 0.5, 0.5, 1)
            If (rounded / resolution) > TargetWidth Then
              ' exceeded maxWidth .. re-try by switching to lower unit 
              rounded = 0
              Dim newUnit As ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Undefined
              ' Automatically switch unit to a lower one
              If unit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Kilometers Then
                newUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Meters
              ElseIf unit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Miles Then
                newUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Feet
              End If
              If newUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.Undefined Then 'no lower unit
                Exit Do
              End If
              unit = newUnit
            End If
          End If
        End If
      Loop
      outResolution = resolution
      Return rounded
    End Function

    Private Function roundToSignificant(ByVal value As Double, ByVal resolution As Double) As Double
      Dim round = Math.Floor(-Math.Log(resolution))
      If round > 0 Then
        round = Math.Pow(10, round)
        Return Math.Round(value * round) / round
      Else
        Return Math.Round(value)
      End If
    End Function


    ''' <summary>
    ''' Calculates horizontal scale at center of extent
    ''' for geographic / Plate Carrée projection.
    ''' Horizontal scale is 0 at the poles.
    ''' </summary>
    Private toRadians As Double = 0.017453292519943295
    Private earthRadius As Double = 6378137 'Earth radius in meters (defaults to WGS84 / GRS80)
    Private degreeDist As Double
    Private Function getResolutionForGeographic(ByVal extent As ESRI.ArcGIS.Client.Geometry.Envelope, ByVal resolution As Double) As Double
      degreeDist = earthRadius * toRadians
      Dim center As MapPoint = extent.GetCenter()
      Dim y As Double = center.Y
      If Math.Abs(y) > 90 Then
        Return 0
      End If
      Return Math.Cos(y * toRadians) * resolution * degreeDist
    End Function
#End Region


  End Class
End Namespace
