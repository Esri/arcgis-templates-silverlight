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
		<TemplatePart(Name := "btnClose", Type := GetType(Button)), TemplatePart(Name := "TopBar", Type := GetType(UIElement)), TemplatePart(Name := "ResizeWidth", Type := GetType(UIElement)), TemplatePart(Name := "ResizeHeight", Type := GetType(UIElement)), TemplatePart(Name := "ResizeCorner", Type := GetType(UIElement)), TemplatePart(Name := "TitleText", Type := GetType(TextBlock)), TemplatePart(Name := "ContentBorder", Type := GetType(Border)), TemplateVisualState(GroupName := "CommonStates", Name := "Normal"), TemplateVisualState(GroupName := "CommonStates", Name := "MouseOver"), TemplateVisualState(GroupName := "CommonStates", Name := "Dragging"), TemplateVisualState(GroupName := "CommonStates", Name := "Focus"), ContentProperty("Content")>
		Public Class DraggableWindow 'area used for resizing the window -  area used for resizing the window - area used for resizing the window - top bar / area used for dragging the window - Close button
			Inherits ContentControl
				Private topbar As UIElement
				Private ResizeWidth As UIElement
				Private ResizeHeight As UIElement
				Private ResizeCorner As UIElement
				Private contentBorder As Border
				Private lastPoint As Point
				Private titleText As TextBlock
				Private btnClose As Button
				Private isMouseOver As Boolean = False
				Private isDragging As Boolean = False
				Private hasFocus As Boolean = False
				Private hideHeader As Boolean = False



				Public Sub New()
						DefaultStyleKey = GetType(DraggableWindow)
						'this.MouseEnter += (s, e) => { this.isMouseOver = true; ChangeVisualState(true); };
						'this.MouseLeave += (s, e) => { this.isMouseOver = false; ChangeVisualState(true); };
						'this.GotFocus += (s, e) => { this.hasFocus = true; ChangeVisualState(true); };
						'this.LostFocus += (s, e) => { this.hasFocus = false; ChangeVisualState(true); };
				End Sub

				''' <summary>
				''' When overridden in a derived class, is invoked whenever application code or
				''' internal processes (such as a rebuilding layout pass) call
				''' <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
				''' </summary>
				Public Overrides Sub OnApplyTemplate()
						MyBase.OnApplyTemplate()
						contentBorder = TryCast(GetTemplateChild("ContentBorder"), Border)
						If contentBorder IsNot Nothing Then
								contentBorder.Background = ContentBackground
						End If
						btnClose = TryCast(GetTemplateChild("btnClose"), Button)
						If btnClose IsNot Nothing Then
								AddHandler btnClose.Click, Sub(s, e) Me.IsOpen = False
								showCloseButton(IsCloseButtonVisible)
						End If

						topbar = TryCast(GetTemplateChild("TopBar"), UIElement)
						If topbar IsNot Nothing Then
								AddHandler topbar.MouseLeftButtonDown, AddressOf topbar_MouseLeftButtonDown
						End If

						Me.RenderTransform = New TranslateTransform()

						ResizeWidth = TryCast(GetTemplateChild("ResizeWidth"), UIElement)
						ResizeHeight = TryCast(GetTemplateChild("ResizeHeight"), UIElement)
						ResizeCorner = TryCast(GetTemplateChild("ResizeCorner"), UIElement)
						If ResizeWidth IsNot Nothing Then
								AddHandler ResizeWidth.MouseLeftButtonDown, AddressOf Resize_MouseLeftButtonDown
								ResizeWidth.IsHitTestVisible = IsWidthResizeable
						End If
						If ResizeHeight IsNot Nothing Then
								AddHandler ResizeHeight.MouseLeftButtonDown, AddressOf Resize_MouseLeftButtonDown
								ResizeHeight.IsHitTestVisible = IsHeightResizeable
						End If
						If ResizeCorner IsNot Nothing Then
								AddHandler ResizeCorner.MouseLeftButtonDown, AddressOf Resize_MouseLeftButtonDown
								ResizeCorner.IsHitTestVisible = IsWidthResizeable AndAlso IsHeightResizeable
						End If
						titleText = TryCast(GetTemplateChild("TitleText"), TextBlock)
						If Not IsHeaderVisible Then
								If topbar IsNot Nothing Then
									topbar.Visibility = Visibility.Collapsed
								End If
								If btnClose IsNot Nothing Then
									btnClose.Visibility = Visibility.Collapsed
								End If
								If titleText IsNot Nothing Then
									titleText.Visibility = Visibility.Collapsed
								End If
						End If

						ChangeVisualState(False)
				End Sub
				Private resizingWidth As Boolean
				Private resizingBoth As Boolean

				Public Sub showCloseButton(ByVal show As Boolean)
						If btnClose IsNot Nothing Then
								btnClose.Visibility = If(show, Visibility.Visible, Visibility.Collapsed)
						End If
				End Sub

				Private Sub Resize_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
						resizingBoth = (sender Is ResizeCorner)
						resizingWidth = (sender Is ResizeWidth)
						If resizingBoth AndAlso Double.IsNaN(Me.Width) AndAlso Double.IsNaN(Me.Height) Then
								Return
						ElseIf resizingWidth AndAlso Double.IsNaN(Me.Width) Then
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
				Private Sub Resize_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
						Dim p2 As Point = e.GetPosition(TryCast(Me.Parent, UIElement))
						If resizingBoth Then
								Dim d As Double = p2.X - lastPoint.X
								Me.Width += d
								If Me.HorizontalAlignment = HorizontalAlignment.Center Then
										Me.HorizontalOffset += d / 2
								ElseIf Me.HorizontalAlignment = HorizontalAlignment.Right Then
										Me.HorizontalOffset += d
								End If
								d = p2.Y - lastPoint.Y
								Me.Height += d
								If Me.VerticalAlignment = VerticalAlignment.Bottom Then
										Me.VerticalOffset += d
								ElseIf Me.VerticalAlignment = VerticalAlignment.Center Then
										Me.VerticalOffset += d / 2
								End If
						ElseIf resizingWidth Then
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
				Private Sub Resize_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
						StopResize()
				End Sub
				Private Sub Resize_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
						StopResize()
				End Sub

				Private Sub StopResize()
						RemoveHandler Application.Current.RootVisual.MouseMove, AddressOf Resize_MouseMove
						RemoveHandler Application.Current.RootVisual.MouseLeftButtonUp, AddressOf Resize_MouseLeftButtonUp
						RemoveHandler Application.Current.RootVisual.MouseLeave, AddressOf Resize_MouseLeave
				End Sub

				''' <summary>
				''' Starts tragging window drag
				''' </summary>
				''' <param name="sender">The source of the event.</param>
				''' <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
				Private Sub topbar_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
						If Not IsDraggable Then
							Return
						End If
						lastPoint = e.GetPosition(TryCast(Me.Parent, UIElement))
						topbar.CaptureMouse()

						AddHandler topbar.MouseMove, AddressOf RootVisual_MouseMove
						AddHandler topbar.MouseLeftButtonUp, AddressOf RootVisual_MouseLeftButtonUp
						AddHandler topbar.LostMouseCapture, AddressOf topbar_LostMouseCapture
						'topbar.MouseLeave += RootVisual_MouseLeave;
						e.Handled = True
				End Sub

				Private Sub topbar_LostMouseCapture(ByVal sender As Object, ByVal e As MouseEventArgs)
						StopDrag()
				End Sub

				Private Sub RootVisual_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
						StopDrag()
				End Sub

				Private Sub RootVisual_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
						StopDrag()
				End Sub

				''' <summary>
				''' Stops tracking window drag.
				''' </summary>
				Private Sub StopDrag()
						isDragging = False
						topbar.ReleaseMouseCapture()
						RemoveHandler topbar.MouseMove, AddressOf RootVisual_MouseMove
						RemoveHandler topbar.MouseLeftButtonUp, AddressOf RootVisual_MouseLeftButtonUp
						RemoveHandler topbar.LostMouseCapture, AddressOf topbar_LostMouseCapture
						' Application.Current.RootVisual.MouseLeave -= RootVisual_MouseLeave;
						ChangeVisualState(True)
				End Sub

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

				Private Sub ChangeVisualState(ByVal useTransitions As Boolean)
						If isDragging Then
								GoToState(useTransitions, "Dragging")
						ElseIf hasFocus Then
								GoToState(useTransitions, "Focus")
						ElseIf isMouseOver Then
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
				''' Gets or sets Title.
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
				''' Gets or sets IsOpen.
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
				''' Gets or sets IsDraggable.
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
				''' Gets or sets IsWidthResizeable.
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
				''' Gets or sets IsHeightResizeable.
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

				''' <summary>
				''' Identifies the <see cref="IsHeaderHidden"/> dependency property
				''' </summary>
				Public Shared ReadOnly IsHeaderVisibleProperty As DependencyProperty = DependencyProperty.Register("IsHeaderHidden", GetType(Boolean), GetType(DraggableWindow), New PropertyMetadata(True, AddressOf OnIsHeaderVisiblePropertyChanged))

				''' <summary>
				''' Gets or sets IsHeaderVisible
				''' </summary>
				Public Property IsHeaderVisible() As Boolean
						Get
							Return CBool(GetValue(IsHeaderVisibleProperty))
						End Get
						Set(ByVal value As Boolean)
							SetValue(IsHeaderVisibleProperty, value)
						End Set
				End Property

				Private Shared Sub OnIsHeaderVisiblePropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim dp As DraggableWindow = TryCast(d, DraggableWindow)
						If dp.topbar IsNot Nothing Then
								dp.topbar.Visibility = If(CBool(e.NewValue), Visibility.Collapsed, Visibility.Visible)
						End If
				End Sub

				''' <summary>
				''' Identifies the <see cref="IsCloseButtonHidden"/> dependency property
				''' </summary>
				Public Shared ReadOnly IsCloseButtonVisibleProperty As DependencyProperty = DependencyProperty.Register("IsCloseButtonHidden", GetType(Boolean), GetType(DraggableWindow), New PropertyMetadata(True, AddressOf OnIsCloseButtonVisiblePropertyChanged))

				''' <summary>
				''' Gets or sets IsCloseButtonVisible
				''' </summary>
				Public Property IsCloseButtonVisible() As Boolean
						Get
							Return CBool(GetValue(IsCloseButtonVisibleProperty))
						End Get
						Set(ByVal value As Boolean)
							SetValue(IsCloseButtonVisibleProperty, value)
						End Set
				End Property

				Private Shared Sub OnIsCloseButtonVisiblePropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim dp As DraggableWindow = TryCast(d, DraggableWindow)
						If dp.btnClose IsNot Nothing Then
								dp.btnClose.Visibility = If(CBool(e.NewValue), Visibility.Collapsed, Visibility.Visible)
						End If
				End Sub

				''' <summary>
				''' Identifies the <see cref="Title"/> dependency property.
				''' </summary>
				Public Shared ReadOnly ContentBackgroundBorderProperty As DependencyProperty = DependencyProperty.Register("ContentBackground", GetType(Brush), GetType(DraggableWindow), New PropertyMetadata(New SolidColorBrush(Colors.Transparent), AddressOf OnContentBackgroundPropertyChanged))
				''' <summary>
				''' Gets or sets Title.
				''' </summary>
				Public Property ContentBackgroundBorder() As Brush
						Get
							Return CType(GetValue(ContentBackgroundBorderProperty), Brush)
						End Get
						Set(ByVal value As Brush)
							SetValue(ContentBackgroundBorderProperty, value)
						End Set
				End Property

				Private Shared Sub OnContentBackgroundBorderPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim dp As DraggableWindow = TryCast(d, DraggableWindow)
						If dp.contentBorder IsNot Nothing Then
								dp.contentBorder.BorderBrush = TryCast(e.NewValue, Brush)
						End If
				End Sub

				''' <summary>
				''' Identifies the <see cref="Title"/> dependency property.
				''' </summary>
				Public Shared ReadOnly ContentBackgroundProperty As DependencyProperty = DependencyProperty.Register("ContentBackground", GetType(Brush), GetType(DraggableWindow), New PropertyMetadata(New SolidColorBrush(Colors.Transparent), AddressOf OnContentBackgroundPropertyChanged))
				''' <summary>
				''' Gets or sets Title.
				''' </summary>
				Public Property ContentBackground() As Brush
						Get
							Return CType(GetValue(ContentBackgroundProperty), Brush)
						End Get
						Set(ByVal value As Brush)
							SetValue(ContentBackgroundProperty, value)
						End Set
				End Property

				Private Shared Sub OnContentBackgroundPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim dp As DraggableWindow = TryCast(d, DraggableWindow)
						If dp.contentBorder IsNot Nothing Then
								dp.contentBorder.Background = TryCast(e.NewValue, Brush)
						End If
				End Sub

				#End Region

				#Region "Events"
				Public Event Closed As EventHandler
				Public Event Opened As EventHandler

				Protected Sub OnClosed()
							RaiseEvent Closed(Me, New EventArgs())
				End Sub
				Protected Sub OnOpened()
							RaiseEvent Opened(Me, New EventArgs())
				End Sub
				#End Region
		End Class
End Namespace
