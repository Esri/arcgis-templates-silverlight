Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes

Namespace Toolkit.Icons
  <TemplatePart(Name:="arrow", Type:=GetType(Polygon)), TemplatePart(Name:="Flip", Type:=GetType(ScaleTransform))>
  Public Class UpDownExpand
    Inherits Control
    Private arrow As Polygon
    Private Flip As ScaleTransform
    Private brush_fill As Brush
    Private brush_stroke As Brush
    Private bisPointingDown As Boolean = True

    Public Sub New()
      Me.DefaultStyleKey = GetType(UpDownExpand)
    End Sub

    ''' <summary>
    ''' When overridden in a derived class, is invoked whenever application code or
    ''' internal processes (such as a rebuilding layout pass) call
    ''' <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
    ''' </summary>
    Public Overrides Sub OnApplyTemplate()
      Flip = TryCast(GetTemplateChild("Flip"), ScaleTransform)
      arrow = TryCast(GetTemplateChild("arrow"), Polygon)
      If brush_fill IsNot Nothing Then
        arrow.Fill = brush_fill
      End If
      If brush_stroke IsNot Nothing Then
        arrow.Stroke = brush_stroke
      End If
      'if (isPointingRight != null)
      Flip.ScaleY = If(bisPointingDown, 1, -1)
    End Sub

    Public Property Fill() As Brush
      Get
        Return CType(GetValue(FillProperty), Brush)
      End Get
      Set(ByVal value As Brush)
        SetValue(FillProperty, value)
      End Set
    End Property

    Public Shared ReadOnly FillProperty As DependencyProperty = DependencyProperty.Register("Fill", GetType(Brush), GetType(UpDownExpand), New PropertyMetadata(New SolidColorBrush(Color.FromArgb(255, 224, 224, 224)), AddressOf OnFillPropertyChanged))

    Private Shared Sub OnFillPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim expand As UpDownExpand = TryCast(d, UpDownExpand)
      Dim brush As Brush = TryCast(e.NewValue, Brush)
      expand.brush_fill = brush
      If expand.arrow IsNot Nothing Then
        expand.arrow.Fill = brush
      End If
    End Sub

    Public Property Stroke() As Brush
      Get
        Return CType(GetValue(StrokeProperty), Brush)
      End Get
      Set(ByVal value As Brush)
        SetValue(StrokeProperty, value)
      End Set
    End Property

    Public Shared ReadOnly StrokeProperty As DependencyProperty = DependencyProperty.Register("Stroke", GetType(Brush), GetType(UpDownExpand), New PropertyMetadata(New SolidColorBrush(Color.FromArgb(255, 53, 53, 53)), AddressOf OnStrokePropertyChanged))

    Private Shared Sub OnStrokePropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim expand As UpDownExpand = TryCast(d, UpDownExpand)
      Dim brush As Brush = TryCast(e.NewValue, Brush)
      expand.brush_stroke = brush
      If expand.arrow IsNot Nothing Then
        expand.arrow.Stroke = brush
      End If
    End Sub

    Public Property IsPointingDown() As Boolean
      Get
        Return CBool(GetValue(IsPointingDownProperty))
      End Get
      Set(ByVal value As Boolean)
        SetValue(IsPointingDownProperty, value)
      End Set
    End Property

    Public Shared ReadOnly IsPointingDownProperty As DependencyProperty = DependencyProperty.Register("IsPointingDown", GetType(Boolean), GetType(UpDownExpand), New PropertyMetadata(True, AddressOf OnIsPointingDownPropertyChanged))

    Private Shared Sub OnIsPointingDownPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim expand As UpDownExpand = TryCast(d, UpDownExpand)
      Dim obj As Object = e.NewValue
      If obj IsNot Nothing Then
        Dim pointingDown As Boolean = CBool(obj)
        expand.bisPointingDown = pointingDown
        If expand.Flip IsNot Nothing Then
          expand.Flip.ScaleY = If(pointingDown, 1, -1)
        End If
      End If
    End Sub

  End Class
End Namespace
