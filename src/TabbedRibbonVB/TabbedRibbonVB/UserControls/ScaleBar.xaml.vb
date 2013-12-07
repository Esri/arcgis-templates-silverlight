Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports ESRI.ArcGIS.Client
Imports ESRI.ArcGIS.Client.Geometry
Imports ESRI.ArcGIS.Client.Toolkit

Namespace ESRI.ArcGIS.SilverlightMapApp
  <TemplatePart(Name:="ScaleBarBlock", Type:=GetType(FrameworkElement)), TemplatePart(Name:="PaddingLeftForScaleBarTextMeters", Type:=GetType(FrameworkElement)), TemplatePart(Name:="PaddingLeftTopNotch", Type:=GetType(FrameworkElement)), TemplatePart(Name:="PaddingLeftForScaleBarTextMiles", Type:=GetType(FrameworkElement)), TemplatePart(Name:="PaddingLeftBottomNotch", Type:=GetType(FrameworkElement)), TemplatePart(Name:="ScaleBarTextForMeters", Type:=GetType(TextBlock)), TemplatePart(Name:="ScaleBarTextForMiles", Type:=GetType(TextBlock)), TemplatePart(Name:="LayoutRoot", Type:=GetType(FrameworkElement))>
  Public Class ScaleBar
    Inherits Control
    Private ScaleBarBlock As FrameworkElement
    Private PaddingLeftForScaleBarTextMeters As FrameworkElement
    Private ScaleBarTextForMeters As TextBlock
    Private PaddingLeftTopNotch As FrameworkElement
    Private PaddingLeftForScaleBarTextMiles As FrameworkElement
    Private PaddingLeftBottomNotch As FrameworkElement
    Private LayoutRoot As FrameworkElement

    Private ScaleBarTextForMiles As TextBlock
    Public Sub New()
      DefaultStyleKey = GetType(ScaleBar)
    End Sub
    Public Overrides Sub OnApplyTemplate()
      ScaleBarBlock = TryCast(Me.GetTemplateChild("ScaleBarBlock"), FrameworkElement)
      PaddingLeftForScaleBarTextMeters = TryCast(Me.GetTemplateChild("PaddingLeftForScaleBarTextMeters"), FrameworkElement)
      PaddingLeftTopNotch = TryCast(Me.GetTemplateChild("PaddingLeftTopNotch"), FrameworkElement)
      PaddingLeftForScaleBarTextMiles = TryCast(Me.GetTemplateChild("PaddingLeftForScaleBarTextMiles"), FrameworkElement)
      PaddingLeftBottomNotch = TryCast(Me.GetTemplateChild("PaddingLeftBottomNotch"), FrameworkElement)
      ScaleBarTextForMeters = TryCast(Me.GetTemplateChild("ScaleBarTextForMeters"), TextBlock)
      ScaleBarTextForMiles = TryCast(Me.GetTemplateChild("ScaleBarTextForMiles"), TextBlock)
      LayoutRoot = TryCast(Me.GetTemplateChild("LayoutRoot"), FrameworkElement)
      refreshScalebar()
      MyBase.OnApplyTemplate()
    End Sub
#Region "Helper Functions"
    Private Sub map_ExtentChanged(ByVal sender As Object, ByVal args As ESRI.ArcGIS.Client.ExtentEventArgs)
      refreshScalebar()
    End Sub

    Private Sub refreshScalebar()
      Dim viz As Visibility = Visibility.Visible
      If Map Is Nothing OrElse Double.IsNaN(Map.Resolution) OrElse MapUnit = ScaleLine.ScaleLineUnit.DecimalDegrees AndAlso Math.Abs(Map.Extent.GetCenter().Y) >= 90 Then
        viz = Visibility.Collapsed
      End If
      If LayoutRoot IsNot Nothing Then
        LayoutRoot.Visibility = viz
      End If
      If viz = Visibility.Collapsed Then
        Return
      End If

      Dim outUnit As ScaleLine.ScaleLineUnit = ScaleLine.ScaleLineUnit.Undefined
      Dim outResolution As Double

      '						#Region "KiloMeters/Meters"
      Dim roundedKiloMeters As Double = getBestEstimateOfValue(Map.Resolution, ScaleLine.ScaleLineUnit.Kilometers, outUnit, outResolution)
      Dim widthMeters As Double = roundedKiloMeters / outResolution
      Dim inMeters As Boolean = outUnit = ScaleLine.ScaleLineUnit.Meters

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
      Dim roundedMiles As Double = getBestEstimateOfValue(Map.Resolution, ScaleLine.ScaleLineUnit.Miles, outUnit, outResolution)
      Dim widthMiles As Double = roundedMiles / outResolution
      Dim inFeet As Boolean = outUnit = ScaleLine.ScaleLineUnit.Feet

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

    Private Function getBestEstimateOfValue(ByVal resolution As Double, ByVal displayUnit As ScaleLine.ScaleLineUnit, <System.Runtime.InteropServices.Out()> ByRef unit As ScaleLine.ScaleLineUnit, <System.Runtime.InteropServices.Out()> ByRef outResolution As Double) As Double
      unit = displayUnit
      Dim rounded As Double = 0
      Dim originalRes As Double = resolution
      Do While rounded < 0.5
        resolution = originalRes
        If MapUnit = ScaleLine.ScaleLineUnit.DecimalDegrees Then
          resolution = getResolutionForGeographic(Map.Extent, resolution)
          resolution = resolution * CInt(Fix(ScaleLine.ScaleLineUnit.Meters)) / CInt(Fix(unit))
        ElseIf MapUnit <> ScaleLine.ScaleLineUnit.Undefined Then
          resolution = resolution * CInt(Fix(MapUnit)) / CInt(Fix(unit))
        End If

        Dim val As Double = TargetWidth * resolution
        val = roundToSignificant(val, resolution)
        Dim noFrac As Double = Math.Round(val) ' to get rid of the fraction
        If val < 0.5 Then
          Dim newUnit As ScaleLine.ScaleLineUnit = ScaleLine.ScaleLineUnit.Undefined
          ' Automatically switch unit to a lower one
          If unit = ScaleLine.ScaleLineUnit.Kilometers Then
            newUnit = ScaleLine.ScaleLineUnit.Meters
          ElseIf unit = ScaleLine.ScaleLineUnit.Miles Then
            newUnit = ScaleLine.ScaleLineUnit.Feet
          End If
          If newUnit = ScaleLine.ScaleLineUnit.Undefined Then 'no lower unit
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
              Dim newUnit As ScaleLine.ScaleLineUnit = ScaleLine.ScaleLineUnit.Undefined
              ' Automatically switch unit to a lower one
              If unit = ScaleLine.ScaleLineUnit.Kilometers Then
                newUnit = ScaleLine.ScaleLineUnit.Meters
              ElseIf unit = ScaleLine.ScaleLineUnit.Miles Then
                newUnit = ScaleLine.ScaleLineUnit.Feet
              End If
              If newUnit = ScaleLine.ScaleLineUnit.Undefined Then 'no lower unit
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

#Region "Properties"

    ''' <summary>
    ''' Identifies the Map dependency property.
    ''' </summary>
    Public Shared ReadOnly MapProperty As DependencyProperty = DependencyProperty.Register("Map", GetType(Map), GetType(ScaleBar), New PropertyMetadata(AddressOf OnMapPropertyChanged))


    Private Shared Sub OnMapPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim oldMap As Map = TryCast(e.OldValue, Map)
      Dim newMap As Map = TryCast(e.NewValue, Map)
      Dim bar As ScaleBar = TryCast(d, ScaleBar)
      If bar IsNot Nothing Then
        If oldMap IsNot Nothing Then
          RemoveHandler oldMap.ExtentChanged, AddressOf bar.map_ExtentChanged
          RemoveHandler oldMap.ExtentChanging, AddressOf bar.map_ExtentChanged
        End If
        If newMap IsNot Nothing Then
          AddHandler newMap.ExtentChanged, AddressOf bar.map_ExtentChanged
          AddHandler newMap.ExtentChanging, AddressOf bar.map_ExtentChanged
        End If
        bar.refreshScalebar()
      End If
    End Sub

    ''' <summary>
    ''' Gets or sets the map that the scale bar is buddied to.
    ''' </summary>
    Public Property Map() As ESRI.ArcGIS.Client.Map
      Get
        Return TryCast(GetValue(MapProperty), Map)
      End Get
      Set(ByVal value As ESRI.ArcGIS.Client.Map)
        SetValue(MapProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' Identifies the <see cref="FillColor1"/> dependency property.
    ''' </summary>
    Public Shared ReadOnly TargetWidthProperty As DependencyProperty = DependencyProperty.Register("TargetWidth", GetType(Double), GetType(ScaleBar), New PropertyMetadata(150.0))

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
    Public Property MapUnit() As ScaleLine.ScaleLineUnit


    ''' <summary>
    ''' Identifies the <see cref="Fill"/> dependency property.
    ''' </summary>
    Public Shared ReadOnly FillProperty As DependencyProperty = DependencyProperty.Register("Fill", GetType(Brush), GetType(ScaleBar), New PropertyMetadata(New SolidColorBrush(Colors.Black)))
    ''' <summary>
    ''' Gets or sets the color of the scalebar.
    ''' </summary>
    Public Property Fill() As Brush
      Get
        Return CType(GetValue(FillProperty), Brush)
      End Get
      Set(ByVal value As Brush)
        SetValue(FillProperty, value)
      End Set
    End Property
#End Region
  End Class
End Namespace