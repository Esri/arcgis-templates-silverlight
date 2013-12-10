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

Namespace Toolkit.Icons
  <TemplatePart(Name:="OceanEllipse", Type:=GetType(Ellipse)), TemplatePart(Name:="LandPath", Type:=GetType(Path)), TemplatePart(Name:="GlobeScale", Type:=GetType(ScaleTransform))>
  Public Class LargeWorldGlobe
    Inherits Control
    Private OceanEllipse As Ellipse
    Private LandPath As Path
    Private ocean_Fill As Brush
    Private land_Fill As Brush
    Private GlobeScale As ScaleTransform
    Private dblScale As Double = 1
    Private world As LargeWorldGlobe
    Private bexpandOnMouseOver As Boolean = False

    Public Sub New()
      Me.DefaultStyleKey = GetType(LargeWorldGlobe)
    End Sub

    ''' <summary>
    ''' When overridden in a derived class, is invoked whenever application code or
    ''' internal processes (such as a rebuilding layout pass) call
    ''' <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
    ''' </summary>
    Public Overrides Sub OnApplyTemplate()
      world = Me
      OceanEllipse = TryCast(GetTemplateChild("OceanEllipse"), Ellipse)
      LandPath = TryCast(GetTemplateChild("LandPath"), Path)
      GlobeScale = TryCast(GetTemplateChild("GlobeScale"), ScaleTransform)
      If ocean_Fill IsNot Nothing Then
        OceanEllipse.Fill = ocean_Fill
      End If
      If land_Fill IsNot Nothing Then
        LandPath.Fill = land_Fill
      End If
      If dblScale <> 1 Then
        GlobeScale.ScaleX = dblScale
        world.GlobeScale.ScaleY = dblScale
      End If
      If bexpandOnMouseOver Then
        AddHandler world.MouseEnter, AddressOf globe_MouseEnter
        AddHandler world.MouseLeave, AddressOf globe_MouseLeave
      End If
    End Sub

    Public Property OceanFill() As Brush
      Get
        Return CType(GetValue(OceanFillProperty), Brush)
      End Get
      Set(ByVal value As Brush)
        SetValue(OceanFillProperty, value)
      End Set
    End Property

    Public Shared ReadOnly OceanFillProperty As DependencyProperty = DependencyProperty.Register("OceanFill", GetType(Brush), GetType(LargeWorldGlobe), New PropertyMetadata(New SolidColorBrush(Color.FromArgb(255, 193, 97, 57)), AddressOf OnOceanFillPropertyChanged))

    Private Shared Sub OnOceanFillPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)

      Dim globe As LargeWorldGlobe = TryCast(d, LargeWorldGlobe)
      Dim brush As Brush = TryCast(e.NewValue, Brush)
      globe.ocean_Fill = brush
      If globe.OceanEllipse IsNot Nothing Then
        globe.OceanEllipse.Fill = brush
      End If
    End Sub

    Public Property LandFill() As Brush
      Get
        Return CType(GetValue(LandFillProperty), Brush)
      End Get
      Set(ByVal value As Brush)
        SetValue(LandFillProperty, value)
      End Set
    End Property

    Public Shared ReadOnly LandFillProperty As DependencyProperty = DependencyProperty.Register("LandFill", GetType(Brush), GetType(LargeWorldGlobe), New PropertyMetadata(New SolidColorBrush(Color.FromArgb(255, 193, 97, 57)), AddressOf OnLandFillPropertyChanged))

    Private Shared Sub OnLandFillPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim globe As LargeWorldGlobe = TryCast(d, LargeWorldGlobe)
      Dim brush As Brush = TryCast(e.NewValue, Brush)
      globe.land_Fill = brush
      If globe.LandPath IsNot Nothing Then
        globe.LandPath.Fill = brush
      End If
    End Sub

    Public Property ExpandOnMouseOver() As Boolean
      Get
        Return CBool(GetValue(ExpandOnMouseOverProperty))
      End Get
      Set(ByVal value As Boolean)
        SetValue(ExpandOnMouseOverProperty, value)
      End Set
    End Property

    Public Shared ReadOnly ExpandOnMouseOverProperty As DependencyProperty = DependencyProperty.Register("ExpandOnMouseOver", GetType(Boolean), GetType(LargeWorldGlobe), New PropertyMetadata(False, AddressOf OnExpandOnMouseOverChanged))

    Private Shared Sub OnExpandOnMouseOverChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim globe As LargeWorldGlobe = TryCast(d, LargeWorldGlobe)
      Dim obj As Object = e.NewValue
      If obj IsNot Nothing Then
        RemoveHandler globe.MouseEnter, AddressOf globe.globe_MouseEnter
        RemoveHandler globe.MouseLeave, AddressOf globe.globe_MouseLeave
        Dim expand As Boolean = CBool(obj)
        globe.bexpandOnMouseOver = expand
        If expand Then
          AddHandler globe.MouseEnter, AddressOf globe.globe_MouseEnter
          AddHandler globe.MouseLeave, AddressOf globe.globe_MouseLeave
        End If
      End If
    End Sub

    Private Sub globe_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim xda As New DoubleAnimation() With {.To = dblScale, .Duration = New Duration(TimeSpan.FromSeconds(0.5))}
      Dim yda As New DoubleAnimation() With {.To = dblScale, .Duration = New Duration(TimeSpan.FromSeconds(0.5))}
      Dim sb As New Storyboard()
      Storyboard.SetTarget(xda, world.GlobeScale)
      Storyboard.SetTargetProperty(xda, New PropertyPath(ScaleTransform.ScaleXProperty))
      Storyboard.SetTarget(yda, world.GlobeScale)
      Storyboard.SetTargetProperty(yda, New PropertyPath(ScaleTransform.ScaleYProperty))
      sb.Children.Add(xda)
      sb.Children.Add(yda)
      sb.Begin()
    End Sub

    Private Sub globe_MouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim expandScale As Double = dblScale * 1.25
      Dim xda As New DoubleAnimation() With {.To = expandScale, .Duration = New Duration(TimeSpan.FromSeconds(0.5))}
      Dim yda As New DoubleAnimation() With {.To = expandScale, .Duration = New Duration(TimeSpan.FromSeconds(0.5))}
      Dim sb As New Storyboard()
      Storyboard.SetTarget(xda, world.GlobeScale)
      Storyboard.SetTargetProperty(xda, New PropertyPath(ScaleTransform.ScaleXProperty))
      Storyboard.SetTarget(yda, world.GlobeScale)
      Storyboard.SetTargetProperty(yda, New PropertyPath(ScaleTransform.ScaleYProperty))
      sb.Children.Add(xda)
      sb.Children.Add(yda)
      sb.Begin()
    End Sub

    Public Property Scale() As Double
      Get
        Return CDbl(GetValue(ScaleProperty))
      End Get
      Set(ByVal value As Double)
        SetValue(ScaleProperty, value)
      End Set
    End Property

    Public Shared ReadOnly ScaleProperty As DependencyProperty = DependencyProperty.Register("Scale", GetType(Double), GetType(LargeWorldGlobe), New PropertyMetadata(1.0R, AddressOf OnScalePropertyChanged))

    Private Shared Sub OnScalePropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)

      Dim globe As LargeWorldGlobe = TryCast(d, LargeWorldGlobe)
      Dim value As Double = Convert.ToDouble(e.NewValue)
      globe.dblScale = value
      If globe.GlobeScale IsNot Nothing Then
        globe.GlobeScale.ScaleX = value
        globe.GlobeScale.ScaleY = value
      End If
    End Sub

  End Class
End Namespace
