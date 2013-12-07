Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes

Namespace ESRI.ArcGIS.SilverlightMapApp
				'[ContentProperty("Content")]
				<TemplatePart(Name := "CloseButton", Type := GetType(Button)), TemplatePart(Name := "ResizeCorner", Type := GetType(UIElement)), TemplatePart(Name := "RightSideTabText", Type := GetType(TextBlock)), TemplatePart(Name := "ContentBorder", Type := GetType(Border)), TemplatePart(Name := "LeftSideTabBorder", Type := GetType(UIElement)), TemplatePart(Name := "RightSideTabBorder", Type := GetType(UIElement)), TemplatePart(Name := "TopBar", Type := GetType(UIElement)), TemplatePart(Name := "BottomBar", Type := GetType(UIElement)), TemplatePart(Name := "LayoutRoot", Type := GetType(UIElement)), TemplateVisualState(GroupName := "CommonStates", Name := "Normal"), TemplateVisualState(GroupName := "CommonStates", Name := "MouseOver"), TemplateVisualState(GroupName := "CommonStates", Name := "Dragging"), TemplateVisualState(GroupName := "CommonStates", Name := "Focus")>
				Public Class GlassPanel 'area used for resizing the window - Close button
					Inherits ContentControl
'INSTANT VB NOTE: The variable glassPanel was renamed since Visual Basic does not allow class members with the same name:
				Private glassPanel_Renamed As GlassPanel
				Private ResizeCorner As UIElement
				Private contentBorder As Border
				Private lastPoint As Point
				Private titleText As TextBlock
				Private closeButton As Button
				Private isDragging As Boolean = False
				Private LeftSideTabBorder As UIElement
				Private RightSideTabBorder As UIElement
				Private TopBar As UIElement
				Private BottomBar As UIElement
				Private layoutRoot As UIElement

						Public Sub New()
						DefaultStyleKey = GetType(GlassPanel)

						End Sub

				Public Overrides Sub OnApplyTemplate()
						MyBase.OnApplyTemplate()
						glassPanel_Renamed = Me
						layoutRoot = TryCast(GetTemplateChild("LayoutRoot"), UIElement)
						contentBorder = TryCast(GetTemplateChild("ContentBorder"), Border)
						If contentBorder IsNot Nothing Then
								'contentBorder.Background = ContentBackground;
								'contentBorder.BorderBrush = ContentBackgroundBorder;
						End If
						closeButton = TryCast(GetTemplateChild("CloseButton"), Button)
						If closeButton IsNot Nothing Then
								'closeButton.Click += (s, e) => { this.IsOpen = false; };
								closeButton.Visibility = If(IsCloseButtonVisible, Visibility.Visible, Visibility.Collapsed)
								AddHandler closeButton.Click, AddressOf closeButton_Click
								'showCloseButton(IsCloseButtonVisible);
						End If


						Me.RenderTransform = New TranslateTransform()

						ResizeCorner = TryCast(GetTemplateChild("ResizeCorner"), UIElement)
						If ResizeCorner IsNot Nothing Then
								If IsResizeable Then
										AddHandler ResizeCorner.MouseLeftButtonDown, AddressOf Resize_MouseLeftButtonDown
										AddHandler ResizeCorner.MouseEnter, AddressOf ResizeCorner_MouseEnter
										AddHandler ResizeCorner.MouseLeave, AddressOf ResizeCorner_MouseLeave
								Else
										ResizeCorner.Visibility = Visibility.Collapsed
								End If

						End If
						titleText = TryCast(GetTemplateChild("RightSideTabText"), TextBlock)
						LeftSideTabBorder = TryCast(GetTemplateChild("LeftSideTabBorder"), UIElement)
						RightSideTabBorder = TryCast(GetTemplateChild("RightSideTabBorder"), UIElement)
						TopBar = TryCast(GetTemplateChild("TopBar"), UIElement)
						BottomBar = TryCast(GetTemplateChild("BottomBar"), UIElement)
						If IsDraggable Then
								If TopBar IsNot Nothing Then
										AddHandler TopBar.MouseLeftButtonDown, AddressOf DragArea_MouseLeftButtonDown
								End If
								If BottomBar IsNot Nothing Then
										AddHandler BottomBar.MouseLeftButtonDown, AddressOf DragArea_MouseLeftButtonDown
								End If
								If LeftSideTabBorder IsNot Nothing Then
										AddHandler LeftSideTabBorder.MouseLeftButtonDown, AddressOf DragArea_MouseLeftButtonDown
								End If
								If RightSideTabBorder IsNot Nothing Then
										AddHandler RightSideTabBorder.MouseLeftButtonDown, AddressOf DragArea_MouseLeftButtonDown
								End If
						End If
						If LeftSideTabBorder IsNot Nothing Then
								AddHandler LeftSideTabBorder.MouseLeftButtonUp, AddressOf LeftSideTabBorder_MouseLeftButtonUp
						End If
				End Sub


				Private Sub closeButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
						glassPanel_Renamed.Collapse()
				End Sub


				#Region "Dependency Properties"

				''' <summary>
				''' Identifies the <see cref="Title"/> dependency property.
				''' </summary>
				Public Shared ReadOnly TitleProperty As DependencyProperty = DependencyProperty.Register("Title", GetType(String), GetType(GlassPanel), Nothing)
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
				Public Shared ReadOnly IsOpenProperty As DependencyProperty = DependencyProperty.Register("IsOpen", GetType(Boolean), GetType(GlassPanel), New PropertyMetadata(True, AddressOf OnIsOpenPropertyChanged))
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
						Dim gp As GlassPanel = TryCast(d, GlassPanel)
						Dim isOpen As Boolean = CBool(e.NewValue)
						gp.Visibility = If(isOpen, Visibility.Visible, Visibility.Collapsed)
						If isOpen Then
								gp.OnOpened()
						Else
								gp.OnClosed()
						End If
				End Sub

				''' <summary>
				''' Identifies the <see cref="HorizontalOffset"/> dependency property.
				''' </summary>
				Public Shared ReadOnly HorizontalOffsetProperty As DependencyProperty = DependencyProperty.Register("HorizontalOffset", GetType(Double), GetType(GlassPanel), New PropertyMetadata(0.0, AddressOf OnHorizontalOffsetPropertyChanged))
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
						Dim gp As GlassPanel = TryCast(d, GlassPanel)
						If TypeOf gp.RenderTransform Is TranslateTransform Then
								TryCast(gp.RenderTransform, TranslateTransform).X = CDbl(e.NewValue)
						End If
				End Sub

				''' <summary>
				''' Identifies the <see cref="VerticalOffset"/> dependency property.
				''' </summary>
				Public Shared ReadOnly VerticalOffsetProperty As DependencyProperty = DependencyProperty.Register("VerticalOffset", GetType(Double), GetType(GlassPanel), New PropertyMetadata(0.0, AddressOf OnVerticalOffsetPropertyChanged))
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
						Dim gp As GlassPanel = TryCast(d, GlassPanel)
						If TypeOf gp.RenderTransform Is TranslateTransform Then
								TryCast(gp.RenderTransform, TranslateTransform).Y = CDbl(e.NewValue)
						End If
				End Sub

				''' <summary>
				''' Identifies the <see cref="IsDraggable"/> dependency property.
				''' </summary>
				Public Shared ReadOnly IsDraggableProperty As DependencyProperty = DependencyProperty.Register("IsDraggable", GetType(Boolean), GetType(GlassPanel), New PropertyMetadata(True))
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
				Public Shared ReadOnly IsResizeableProperty As DependencyProperty = DependencyProperty.Register("IsWidthResizeable", GetType(Boolean), GetType(GlassPanel), New PropertyMetadata(True))
				''' <summary>
				''' Gets or sets IsResizeable.
				''' </summary>
				Public Property IsResizeable() As Boolean
						Get
							Return CBool(GetValue(IsResizeableProperty))
						End Get
						Set(ByVal value As Boolean)
							SetValue(IsResizeableProperty, value)
						End Set
				End Property

				''' <summary>
				''' IsWidthResizeableProperty property changed handler. 
				''' </summary>
				''' <param name="d">Window that changed its IsWidthResizeable.</param>
				''' <param name="e">DependencyPropertyChangedEventArgs.</param> 
				Private Shared Sub OnResizeablePropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim gp As GlassPanel = TryCast(d, GlassPanel)
						If gp.ResizeCorner IsNot Nothing Then
								gp.ResizeCorner.Visibility = If(CBool(e.NewValue) = True, Visibility.Visible, Visibility.Collapsed)
						End If
				End Sub

				''' <summary>
				''' Identifies the <see cref="IsCloseButtonHidden"/> dependency property
				''' </summary>
				Public Shared ReadOnly IsCloseButtonVisibleProperty As DependencyProperty = DependencyProperty.Register("IsCloseButtonHidden", GetType(Boolean), GetType(GlassPanel), New PropertyMetadata(True, AddressOf OnIsCloseButtonVisiblePropertyChanged))

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
						Dim gp As GlassPanel = TryCast(d, GlassPanel)
						If gp.closeButton IsNot Nothing Then
								gp.closeButton.Visibility = If(CBool(e.NewValue), Visibility.Collapsed, Visibility.Visible)
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

				#Region "Action Methods"

				Private Sub AnimatedScale(ByVal sender As Object, ByVal scale As Double)
						Dim gp As GlassPanel = TryCast(sender, GlassPanel)
						If gp.layoutRoot IsNot Nothing Then
								' animation storyboard
								Dim storyboard As New Storyboard()
								' the growing part
								Dim growXAnimation As New DoubleAnimation() With {.To = scale, .Duration = TimeSpan.FromSeconds(0.5)}
								Storyboard.SetTarget(growXAnimation, (CType(gp.layoutRoot.RenderTransform, ScaleTransform)))
								Storyboard.SetTargetProperty(growXAnimation, New PropertyPath("ScaleX"))
								storyboard.Children.Add(growXAnimation)
								Dim growYAnimation As New DoubleAnimation() With {.To = scale, .Duration = TimeSpan.FromSeconds(0.5)}
								Storyboard.SetTarget(growYAnimation, (CType(gp.layoutRoot.RenderTransform, ScaleTransform)))
								Storyboard.SetTargetProperty(growYAnimation, New PropertyPath("ScaleY"))
								storyboard.Children.Add(growYAnimation)
								If scale = 0 Then
										AddHandler storyboard.Completed, AddressOf gp.ShrinkAnimation_Completed
								Else
										AddHandler storyboard.Completed, AddressOf gp.ExpandAnimation_Completed
								End If
								storyboard.Begin()
						End If
				End Sub

				Private Sub ExpandAnimation_Completed(ByVal sender As Object, ByVal e As EventArgs)
						IsOpen = True
				End Sub

				Private Sub ShrinkAnimation_Completed(ByVal sender As Object, ByVal e As EventArgs)
						IsOpen = False
				End Sub

				Public Sub Expand()
						IsOpen = True
						AnimatedScale(Me, 1)
				End Sub
				Public Sub Collapse()
						AnimatedScale(Me, 0)
				End Sub

				#End Region

				#Region "Drag"

				Private Sub DragArea_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
						Dim elem As UIElement = TryCast(sender, UIElement)
						If Not IsDraggable Then
							Return
						End If
						lastPoint = e.GetPosition(Nothing)

						If sender Is TopBar Then
								TopBar.CaptureMouse()
						End If
						AddHandler TopBar.MouseMove, AddressOf RootVisual_MouseMove
						AddHandler TopBar.MouseLeftButtonUp, AddressOf DragArea_MouseLeftButtonUp
						AddHandler TopBar.LostMouseCapture, AddressOf DragArea_LostMouseCapture

						If sender Is BottomBar Then
								BottomBar.CaptureMouse()
						End If
						AddHandler BottomBar.MouseMove, AddressOf RootVisual_MouseMove
						AddHandler BottomBar.MouseLeftButtonUp, AddressOf DragArea_MouseLeftButtonUp
						AddHandler BottomBar.LostMouseCapture, AddressOf DragArea_LostMouseCapture

						If sender Is LeftSideTabBorder Then
								LeftSideTabBorder.CaptureMouse()
						End If
						AddHandler LeftSideTabBorder.MouseMove, AddressOf RootVisual_MouseMove
						AddHandler LeftSideTabBorder.MouseLeftButtonUp, AddressOf DragArea_MouseLeftButtonUp
						AddHandler LeftSideTabBorder.LostMouseCapture, AddressOf DragArea_LostMouseCapture


						If sender Is RightSideTabBorder Then
								TopBar.CaptureMouse()
						End If
						AddHandler RightSideTabBorder.MouseMove, AddressOf RootVisual_MouseMove
						AddHandler RightSideTabBorder.MouseLeftButtonUp, AddressOf DragArea_MouseLeftButtonUp
						AddHandler RightSideTabBorder.LostMouseCapture, AddressOf DragArea_LostMouseCapture

						e.Handled = True
				End Sub

				Private Sub DragArea_LostMouseCapture(ByVal sender As Object, ByVal e As MouseEventArgs)
						StopDrag(sender)
				End Sub

				Private Sub DragArea_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
						StopDrag(sender)
				End Sub

				Private Sub DragArea_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
						StopDrag(sender)
				End Sub

				''' <summary>
				''' Stops tracking window drag.
				''' </summary>
				Private Sub StopDrag(ByVal sender As Object)
						isDragging = False
						If sender Is TopBar Then
								TopBar.ReleaseMouseCapture()
						End If
						RemoveHandler TopBar.MouseMove, AddressOf RootVisual_MouseMove
						RemoveHandler TopBar.MouseLeftButtonUp, AddressOf DragArea_MouseLeftButtonUp
						RemoveHandler TopBar.LostMouseCapture, AddressOf DragArea_LostMouseCapture

						If sender Is BottomBar Then
								BottomBar.ReleaseMouseCapture()
						End If
						RemoveHandler BottomBar.MouseMove, AddressOf RootVisual_MouseMove
						RemoveHandler BottomBar.MouseLeftButtonUp, AddressOf DragArea_MouseLeftButtonUp
						RemoveHandler BottomBar.LostMouseCapture, AddressOf DragArea_LostMouseCapture

						If sender Is RightSideTabBorder Then
								RightSideTabBorder.ReleaseMouseCapture()
						End If
						RemoveHandler RightSideTabBorder.MouseMove, AddressOf RootVisual_MouseMove
						RemoveHandler RightSideTabBorder.MouseLeftButtonUp, AddressOf DragArea_MouseLeftButtonUp
						RemoveHandler RightSideTabBorder.LostMouseCapture, AddressOf DragArea_LostMouseCapture

						If sender Is LeftSideTabBorder Then
								LeftSideTabBorder.ReleaseMouseCapture()
						End If
						RemoveHandler LeftSideTabBorder.MouseMove, AddressOf RootVisual_MouseMove
						RemoveHandler LeftSideTabBorder.MouseLeftButtonUp, AddressOf DragArea_MouseLeftButtonUp
						RemoveHandler LeftSideTabBorder.LostMouseCapture, AddressOf DragArea_LostMouseCapture

						'ChangeVisualState(true);
				End Sub

				Private Sub RootVisual_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
						isDragging = True
						'ChangeVisualState(true);
						Dim t As TranslateTransform = TryCast(Me.RenderTransform, TranslateTransform)
						Dim p2 As Point = e.GetPosition(Nothing)
						Dim dX As Double = p2.X - lastPoint.X
						Dim dY As Double = p2.Y - lastPoint.Y
						HorizontalOffset += dX
						VerticalOffset += dY
						lastPoint = p2
				End Sub

				Private Sub LeftSideTabBorder_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
						'throw new NotImplementedException();
				End Sub

				#End Region

				#Region "Resize"

				Private Sub Resize_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
						If Double.IsNaN(Me.ActualWidth) OrElse Double.IsNaN(Me.ActualHeight) Then
								Return
						End If
						lastPoint = e.GetPosition(Nothing)
						AddHandler Application.Current.RootVisual.MouseMove, AddressOf Resize_MouseMove
						AddHandler Application.Current.RootVisual.MouseLeftButtonUp, AddressOf Resize_MouseLeftButtonUp
						AddHandler Application.Current.RootVisual.MouseLeave, AddressOf Resize_MouseLeave
						e.Handled = True
				End Sub
				Private Sub Resize_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
						Dim p2 As Point = e.GetPosition(Nothing)
								Dim d As Double = p2.X - lastPoint.X
								If Me.Width + d >= Me.MinWidth Then
										Me.Width += d
								End If
								'if (this.HorizontalAlignment == HorizontalAlignment.Center)
								'    this.HorizontalOffset += d / 2;
								'else if (this.HorizontalAlignment == HorizontalAlignment.Right)
								'    this.HorizontalOffset += d;
								d = p2.Y - lastPoint.Y
								If Me.Height + d >= Me.MinHeight Then
										Me.Height += d
								End If
								'if (this.VerticalAlignment == VerticalAlignment.Bottom)
								'    this.VerticalOffset += d;
								'else if (this.VerticalAlignment == VerticalAlignment.Center)
								'    this.VerticalOffset += d / 2;
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

				Private Sub ResizeCorner_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
						Dim rc As FrameworkElement = TryCast(sender, FrameworkElement)
						rc.Opacity = 0.5
				End Sub

				Private Sub ResizeCorner_MouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs)
						Dim rc As FrameworkElement = TryCast(sender, FrameworkElement)
						rc.Opacity = 1
				End Sub

				#End Region

				End Class


End Namespace
