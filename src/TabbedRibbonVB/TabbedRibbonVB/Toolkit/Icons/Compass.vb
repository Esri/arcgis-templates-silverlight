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
		<TemplatePart(Name := "CompassScale", Type := GetType(ScaleTransform)), TemplatePart(Name := "compassFace", Type := GetType(Ellipse))>
		Public Class Compass
			Inherits Control
    Private oCompass As Compass
    Private CompassScale As ScaleTransform
    Private dblScale As Double = 1
    Private bexpandOnMouseOver As Boolean = False
    Private CompassFace As Ellipse
    Private face_Fill As Brush

    Public Sub New()
      Me.DefaultStyleKey = GetType(Compass)
    End Sub

    ''' <summary>
    ''' When overridden in a derived class, is invoked whenever application code or
    ''' internal processes (such as a rebuilding layout pass) call
    ''' <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
    ''' </summary>
    Public Overrides Sub OnApplyTemplate()
      oCompass = Me
      CompassScale = TryCast(GetTemplateChild("CompassScale"), ScaleTransform)
      CompassFace = TryCast(GetTemplateChild("compassFace"), Ellipse)
      If dblScale <> 1 Then
        oCompass.CompassScale.ScaleX = dblScale
        oCompass.CompassScale.ScaleY = dblScale
      End If
      If bexpandOnMouseOver Then
        AddHandler oCompass.MouseEnter, AddressOf compass_MouseEnter
        AddHandler oCompass.MouseLeave, AddressOf compass_MouseLeave
      End If
      If face_Fill IsNot Nothing Then
        CompassFace.Fill = face_Fill
      End If
    End Sub

    Private Sub compass_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim xda As New DoubleAnimation() With {.To = dblScale, .Duration = New Duration(TimeSpan.FromSeconds(0.5))}
      Dim yda As New DoubleAnimation() With {.To = dblScale, .Duration = New Duration(TimeSpan.FromSeconds(0.5))}
      Dim sb As New Storyboard()
      Storyboard.SetTarget(xda, oCompass.CompassScale)
      Storyboard.SetTargetProperty(xda, New PropertyPath(ScaleTransform.ScaleXProperty))
      Storyboard.SetTarget(yda, oCompass.CompassScale)
      Storyboard.SetTargetProperty(yda, New PropertyPath(ScaleTransform.ScaleYProperty))
      sb.Children.Add(xda)
      sb.Children.Add(yda)
      sb.Begin()
    End Sub

    Private Sub compass_MouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim expandScale As Double = dblScale * 1.25
      Dim xda As New DoubleAnimation() With {.To = expandScale, .Duration = New Duration(TimeSpan.FromSeconds(0.5))}
      Dim yda As New DoubleAnimation() With {.To = expandScale, .Duration = New Duration(TimeSpan.FromSeconds(0.5))}
      Dim sb As New Storyboard()
      Storyboard.SetTarget(xda, oCompass.CompassScale)
      Storyboard.SetTargetProperty(xda, New PropertyPath(ScaleTransform.ScaleXProperty))
      Storyboard.SetTarget(yda, oCompass.CompassScale)
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

    Public Shared ReadOnly ScaleProperty As DependencyProperty = DependencyProperty.Register("Scale", GetType(Double), GetType(Compass), New PropertyMetadata(1.0R, AddressOf OnScalePropertyChanged))

    Private Shared Sub OnScalePropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)

      Dim compass As Compass = TryCast(d, Compass)
      Dim value As Double = Convert.ToDouble(e.NewValue)
      compass.dblScale = value
      If compass.CompassScale IsNot Nothing Then
        compass.CompassScale.ScaleX = value
        compass.CompassScale.ScaleY = value
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

    Public Shared ReadOnly ExpandOnMouseOverProperty As DependencyProperty = DependencyProperty.Register("ExpandOnMouseOver", GetType(Boolean), GetType(Compass), New PropertyMetadata(False, AddressOf OnExpandOnMouseOverChanged))

    Private Shared Sub OnExpandOnMouseOverChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim compass As Compass = TryCast(d, Compass)
      Dim obj As Object = e.NewValue
      If obj IsNot Nothing Then
        RemoveHandler compass.MouseEnter, AddressOf compass.compass_MouseEnter
        RemoveHandler compass.MouseLeave, AddressOf compass.compass_MouseLeave
        Dim expand As Boolean = CBool(obj)
        compass.bexpandOnMouseOver = expand
        If expand Then
          AddHandler compass.MouseEnter, AddressOf compass.compass_MouseEnter
          AddHandler compass.MouseLeave, AddressOf compass.compass_MouseLeave
        End If
      End If
    End Sub

    Public Property FaceFill() As Brush
      Get
        Return CType(GetValue(FaceFillProperty), Brush)
      End Get
      Set(ByVal value As Brush)
        SetValue(FaceFillProperty, value)
      End Set
    End Property

    Public Shared ReadOnly FaceFillProperty As DependencyProperty = DependencyProperty.Register("FaceFill", GetType(Brush), GetType(Compass), New PropertyMetadata(New SolidColorBrush(Color.FromArgb(255, 74, 119, 234)), AddressOf OnFaceFillPropertyChanged))

    Private Shared Sub OnFaceFillPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)

      Dim compass As Compass = TryCast(d, Compass)
      Dim brush As Brush = TryCast(e.NewValue, Brush)
      compass.face_Fill = brush
      If compass.CompassFace IsNot Nothing Then
        compass.CompassFace.Fill = brush
      End If
    End Sub

		End Class
End Namespace
