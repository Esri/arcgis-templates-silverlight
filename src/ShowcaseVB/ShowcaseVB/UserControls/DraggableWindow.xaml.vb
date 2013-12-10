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
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media

Namespace ESRI.ArcGIS.SilverlightMapApp
  ''' <summary>
  ''' Resizable and draggable custom window control
  ''' </summary>
  <TemplatePart(Name:="btnClose", Type:=GetType(Button)), TemplatePart(Name:="TopBar", Type:=GetType(UIElement)), TemplatePart(Name:="ResizeWidth", Type:=GetType(UIElement)), TemplatePart(Name:="ResizeHeight", Type:=GetType(UIElement)), TemplateVisualState(GroupName:="CommonStates", Name:="Normal"), TemplateVisualState(GroupName:="CommonStates", Name:="MouseOver"), TemplateVisualState(GroupName:="CommonStates", Name:="Dragging"), TemplateVisualState(GroupName:="CommonStates", Name:="Focus"), ContentProperty("Content")>
  Public Class DraggableWindow 'top bar / area used for dragging the window - top bar / area used for dragging the window - top bar / area used for dragging the window - Close button
    Inherits ContentControl
    Private topbar As UIElement
    Private ResizeWidth As UIElement
    Private ResizeHeight As UIElement
    Private lastPoint As Point
    Private bIsMouseOver As Boolean = False
    Private isDragging As Boolean = False
    Private hasFocus As Boolean = False

    ''' <summary>
    ''' Initializes a new instance of the <see cref="DraggableWindow"/> class.
    ''' </summary>
    Public Sub New()
      DefaultStyleKey = GetType(DraggableWindow)
      AddHandler Me.MouseEnter, Sub(s, e)
                                  Me.bIsMouseOver = True
                                  ChangeVisualState(True)
                                End Sub
      AddHandler Me.MouseLeave, Sub(s, e)
                                  Me.bIsMouseOver = False
                                  ChangeVisualState(True)
                                End Sub
      AddHandler Me.GotFocus, Sub(s, e)
                                Me.hasFocus = True
                                ChangeVisualState(True)
                              End Sub
      AddHandler Me.LostFocus, Sub(s, e)
                                 Me.hasFocus = False
                                 ChangeVisualState(True)
                               End Sub
    End Sub

    ''' <summary>
    ''' When overridden in a derived class, is invoked whenever application 
    ''' code or internal processes (such as a rebuilding layout pass) call 
    ''' <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In 
    ''' simplest terms, this means the method is called just before a UI 
    ''' element displays in an application.
    ''' </summary>
    Public Overrides Sub OnApplyTemplate()
      MyBase.OnApplyTemplate()

      Dim btnClose As Button = TryCast(GetTemplateChild("btnClose"), Button)
      If btnClose IsNot Nothing Then
        AddHandler btnClose.Click, Sub(s, e) Me.IsOpen = False
      End If

      topbar = TryCast(GetTemplateChild("TopBar"), UIElement)
      If topbar IsNot Nothing Then
        AddHandler topbar.MouseLeftButtonDown, AddressOf topbar_MouseLeftButtonDown
      End If

      Me.RenderTransform = New TranslateTransform()

      ResizeWidth = TryCast(GetTemplateChild("ResizeWidth"), UIElement)
      ResizeHeight = TryCast(GetTemplateChild("ResizeHeight"), UIElement)
      If ResizeWidth IsNot Nothing Then
        AddHandler ResizeWidth.MouseLeftButtonDown, AddressOf Resize_MouseLeftButtonDown
        ResizeWidth.IsHitTestVisible = IsWidthResizeable
      End If
      If ResizeHeight IsNot Nothing Then
        AddHandler ResizeHeight.MouseLeftButtonDown, AddressOf Resize_MouseLeftButtonDown
        ResizeHeight.IsHitTestVisible = IsHeightResizeable
      End If

      ChangeVisualState(False)
    End Sub
    Private resizingWidth As Boolean

    ''' <summary>
    ''' Handles the MouseLeftButtonDown event of the Resize area.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> 
    ''' instance containing the event data.</param>
    Private Sub Resize_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
      resizingWidth = (sender Is ResizeWidth)
      If resizingWidth AndAlso Double.IsNaN(Me.Width) Then
        Return 'not supported/disabled
      ElseIf (Not resizingWidth) AndAlso Double.IsNaN(Me.Height) Then
        Return 'not supported/disabled
      End If
      lastPoint = e.GetPosition(TryCast(Me.Parent, UIElement))
      AddHandler Application.Current.RootVisual.MouseMove, AddressOf Resize_MouseMove
      AddHandler Application.Current.RootVisual.MouseLeftButtonUp, AddressOf Resize_MouseLeftButtonUp
      AddHandler Application.Current.RootVisual.MouseLeave, AddressOf Resize_MouseLeave
      e.Handled = True
    End Sub

    ''' <summary>
    ''' Handles the MouseMove event of the Resize area.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/>
    ''' instance containing the event data.</param>
    Private Sub Resize_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim p2 As Point = e.GetPosition(TryCast(Me.Parent, UIElement))
      If resizingWidth Then
        Dim d As Double = p2.X - lastPoint.X
        Me.Width += d
        If Me.HorizontalAlignment = HorizontalAlignment.Center Then
          Me.HorizontalOffset += d / 2
        ElseIf Me.HorizontalAlignment = HorizontalAlignment.Right Then
          Me.HorizontalOffset += d
        End If
      Else
        Dim d As Double = p2.Y - lastPoint.Y
        Me.Height += d
        If Me.VerticalAlignment = VerticalAlignment.Bottom Then
          Me.VerticalOffset += d
        ElseIf Me.VerticalAlignment = VerticalAlignment.Center Then
          Me.VerticalOffset += d / 2
        End If

      End If
      lastPoint = p2

    End Sub

    ''' <summary>
    ''' Handles the MouseLeave event of the Resize area.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> 
    ''' instance containing the event data.</param>
    Private Sub Resize_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
      StopResize()
    End Sub

    ''' <summary>
    ''' Handles the MouseLeftButtonUp event of the Resize area.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/>
    ''' instance containing the event data.</param>
    Private Sub Resize_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
      StopResize()
    End Sub

    ''' <summary>
    ''' Cleanup of event handlers. Called when resize has ended.
    ''' </summary>
    Private Sub StopResize()
      RemoveHandler Application.Current.RootVisual.MouseMove, AddressOf Resize_MouseMove
      RemoveHandler Application.Current.RootVisual.MouseLeftButtonUp, AddressOf Resize_MouseLeftButtonUp
      RemoveHandler Application.Current.RootVisual.MouseLeave, AddressOf Resize_MouseLeave
    End Sub

    ''' <summary>
    ''' Starts tracking window drag
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/>
    ''' instance containing the event data.</param>
    Private Sub topbar_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
      If Not IsDraggable Then
        Return
      End If
      lastPoint = e.GetPosition(TryCast(Me.Parent, UIElement))
      AddHandler Application.Current.RootVisual.MouseMove, AddressOf RootVisual_MouseMove
      AddHandler Application.Current.RootVisual.MouseLeftButtonUp, AddressOf RootVisual_MouseLeftButtonUp
      AddHandler Application.Current.RootVisual.MouseLeave, AddressOf RootVisual_MouseLeave
      e.Handled = True
    End Sub

    ''' <summary>
    ''' Handles the MouseLeave event of the RootVisual control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> 
    ''' instance containing the event data.</param>
    Private Sub RootVisual_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
      StopDrag()
    End Sub

    ''' <summary>
    ''' Handles the MouseLeftButtonUp event of the RootVisual control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> 
    ''' instance containing the event data.</param>
    Private Sub RootVisual_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
      StopDrag()
    End Sub

    ''' <summary>
    ''' Stops tracking window drag.
    ''' </summary>
    Private Sub StopDrag()
      isDragging = False
      RemoveHandler Application.Current.RootVisual.MouseMove, AddressOf RootVisual_MouseMove
      RemoveHandler Application.Current.RootVisual.MouseLeftButtonUp, AddressOf RootVisual_MouseLeftButtonUp
      RemoveHandler Application.Current.RootVisual.MouseLeave, AddressOf RootVisual_MouseLeave
      ChangeVisualState(True)
    End Sub

    ''' <summary>
    ''' Handles the MouseMove event of the RootVisual control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/>
    ''' instance containing the event data.</param>
    Private Sub RootVisual_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
      isDragging = True
      ChangeVisualState(True)
      Dim t As TranslateTransform = TryCast(Me.RenderTransform, TranslateTransform)
      Dim p2 As Point = e.GetPosition(TryCast(Me.Parent, UIElement))
      Dim dX As Double = p2.X - lastPoint.X
      Dim dY As Double = p2.Y - lastPoint.Y
      HorizontalOffset += dX
      VerticalOffset += dY
      lastPoint = p2
    End Sub

    ''' <summary>
    ''' Updates the visual state of the control.
    ''' </summary>
    ''' <param name="useTransitions">if set to <c>true</c> transitions will be used.</param>
    Private Sub ChangeVisualState(ByVal useTransitions As Boolean)
      If isDragging Then
        GoToState(useTransitions, "Dragging")
      ElseIf hasFocus Then
        GoToState(useTransitions, "Focus")
      ElseIf bIsMouseOver Then
        GoToState(useTransitions, "MouseOver")
      Else
        GoToState(useTransitions, "Normal")
      End If
    End Sub

    Private Function GoToState(ByVal useTransitions As Boolean, ByVal stateName As String) As Boolean
      Return VisualStateManager.GoToState(Me, stateName, useTransitions)
    End Function

#Region "Dependency Properties"

    ''' <summary>
    ''' Identifies the <see cref="Title"/> dependency property.
    ''' </summary>
    Public Shared ReadOnly TitleProperty As DependencyProperty = DependencyProperty.Register("Title", GetType(String), GetType(DraggableWindow), Nothing)
    ''' <summary>
    ''' Gets or sets the window title.
    ''' </summary>
    Public Property Title() As String
      Get
        Return CStr(GetValue(TitleProperty))
      End Get
      Set(ByVal value As String)
        SetValue(TitleProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' Identifies the <see cref="IsOpen"/> dependency property.
    ''' </summary>
    Public Shared ReadOnly IsOpenProperty As DependencyProperty = DependencyProperty.Register("IsOpen", GetType(Boolean), GetType(DraggableWindow), New PropertyMetadata(True, AddressOf OnIsOpenPropertyChanged))

    ''' <summary>
    ''' Gets or sets the open state of the window.
    ''' </summary>
    Public Property IsOpen() As Boolean
      Get
        Return CBool(GetValue(IsOpenProperty))
      End Get
      Set(ByVal value As Boolean)
        SetValue(IsOpenProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' IsOpenProperty property changed handler. 
    ''' </summary>
    ''' <param name="d">Window that changed its IsOpen.</param>
    ''' <param name="e">DependencyPropertyChangedEventArgs.</param> 
    Private Shared Sub OnIsOpenPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim dp As DraggableWindow = TryCast(d, DraggableWindow)
      Dim isOpen As Boolean = CBool(e.NewValue)
      dp.Visibility = If(isOpen, Visibility.Visible, Visibility.Collapsed)
      If isOpen Then
        dp.OnOpened()
      Else
        dp.OnClosed()
      End If
    End Sub

    ''' <summary>
    ''' Identifies the <see cref="HorizontalOffset"/> dependency property.
    ''' </summary>
    Public Shared ReadOnly HorizontalOffsetProperty As DependencyProperty = DependencyProperty.Register("HorizontalOffset", GetType(Double), GetType(DraggableWindow), New PropertyMetadata(0.0, AddressOf OnHorizontalOffsetPropertyChanged))

    ''' <summary>
    ''' Gets or sets HorisontalOffset.
    ''' </summary>
    Public Property HorizontalOffset() As Double
      Get
        Return CDbl(GetValue(HorizontalOffsetProperty))
      End Get
      Set(ByVal value As Double)
        SetValue(HorizontalOffsetProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' HorisontalOffsetProperty property changed handler. 
    ''' </summary>
    ''' <param name="d">Window that changed its HorisontalOffset.</param>
    ''' <param name="e">DependencyPropertyChangedEventArgs.</param> 
    Private Shared Sub OnHorizontalOffsetPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim dp As DraggableWindow = TryCast(d, DraggableWindow)
      If TypeOf dp.RenderTransform Is TranslateTransform Then
        TryCast(dp.RenderTransform, TranslateTransform).X = CDbl(e.NewValue)
      End If
    End Sub

    ''' <summary>
    ''' Identifies the <see cref="VerticalOffset"/> dependency property.
    ''' </summary>
    Public Shared ReadOnly VerticalOffsetProperty As DependencyProperty = DependencyProperty.Register("VerticalOffset", GetType(Double), GetType(DraggableWindow), New PropertyMetadata(0.0, AddressOf OnVerticalOffsetPropertyChanged))

    ''' <summary>
    ''' Gets or sets VerticalOffset.
    ''' </summary>
    Public Property VerticalOffset() As Double
      Get
        Return CDbl(GetValue(VerticalOffsetProperty))
      End Get
      Set(ByVal value As Double)
        SetValue(VerticalOffsetProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' VerticalOffsetProperty property changed handler. 
    ''' </summary>
    ''' <param name="d">Window that changed its VerticalOffset.</param>
    ''' <param name="e">DependencyPropertyChangedEventArgs.</param> 
    Private Shared Sub OnVerticalOffsetPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim dp As DraggableWindow = TryCast(d, DraggableWindow)
      If TypeOf dp.RenderTransform Is TranslateTransform Then
        TryCast(dp.RenderTransform, TranslateTransform).Y = CDbl(e.NewValue)
      End If
    End Sub

    ''' <summary>
    ''' Identifies the <see cref="IsDraggable"/> dependency property.
    ''' </summary>
    Public Shared ReadOnly IsDraggableProperty As DependencyProperty = DependencyProperty.Register("IsDraggable", GetType(Boolean), GetType(DraggableWindow), New PropertyMetadata(True))

    ''' <summary>
    ''' Gets or sets whether the window can be moved by dragging with the mouse.
    ''' </summary>
    Public Property IsDraggable() As Boolean
      Get
        Return CBool(GetValue(IsDraggableProperty))
      End Get
      Set(ByVal value As Boolean)
        SetValue(IsDraggableProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' Identifies the <see cref="IsWidthResizeable"/> dependency property.
    ''' </summary>
    Public Shared ReadOnly IsWidthResizeableProperty As DependencyProperty = DependencyProperty.Register("IsWidthResizeable", GetType(Boolean), GetType(DraggableWindow), New PropertyMetadata(True))
    ''' <summary>
    ''' Gets or sets whether the user can resize the width of the window.
    ''' </summary>
    Public Property IsWidthResizeable() As Boolean
      Get
        Return CBool(GetValue(IsWidthResizeableProperty))
      End Get
      Set(ByVal value As Boolean)
        SetValue(IsWidthResizeableProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' IsWidthResizeableProperty property changed handler. 
    ''' </summary>
    ''' <param name="d">Window that changed its IsWidthResizeable.</param>
    ''' <param name="e">DependencyPropertyChangedEventArgs.</param> 
    Private Shared Sub OnIsWidthResizeablePropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim dp As DraggableWindow = TryCast(d, DraggableWindow)
      If dp.ResizeWidth IsNot Nothing Then
        dp.ResizeWidth.IsHitTestVisible = CBool(e.NewValue)
      End If
    End Sub

    ''' <summary>
    ''' Identifies the <see cref="IsHeightResizeable"/> dependency property.
    ''' </summary>
    Public Shared ReadOnly IsHeightResizeableProperty As DependencyProperty = DependencyProperty.Register("IsHeightResizeable", GetType(Boolean), GetType(DraggableWindow), New PropertyMetadata(True, AddressOf OnIsHeightResizeablePropertyChanged))
    ''' <summary>
    ''' Gets or sets whether the user can resize the height of the window.
    ''' </summary>
    Public Property IsHeightResizeable() As Boolean
      Get
        Return CBool(GetValue(IsHeightResizeableProperty))
      End Get
      Set(ByVal value As Boolean)
        SetValue(IsHeightResizeableProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' IsHeightResizeableProperty property changed handler. 
    ''' </summary>
    ''' <param name="d">Window that changed its VerticalOffset.</param>
    ''' <param name="e">DependencyPropertyChangedEventArgs.</param> 
    Private Shared Sub OnIsHeightResizeablePropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      Dim dp As DraggableWindow = TryCast(d, DraggableWindow)
      If dp.ResizeHeight IsNot Nothing Then
        dp.ResizeHeight.IsHitTestVisible = CBool(e.NewValue)
      End If
    End Sub
#End Region

#Region "Events"

    ''' <summary>
    ''' Occurs when the window is closed.
    ''' </summary>
    Public Event Closed As System.EventHandler

    ''' <summary>
    ''' Occurs when the window is opened.
    ''' </summary>
    Public Event Opened As System.EventHandler

    ''' <summary>
    ''' Called when windows is closed.
    ''' </summary>
    Protected Sub OnClosed()
      RaiseEvent Closed(Me, New System.EventArgs())
    End Sub

    ''' <summary>
    ''' Called when windows is opened.
    ''' </summary>
    Protected Sub OnOpened()
      RaiseEvent Opened(Me, New System.EventArgs())
    End Sub
#End Region
  End Class
End Namespace
