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
		Public Class WindowPanel
			Inherits ContentControl
				Friend IsRolledUp As Boolean = True
				Private isTrackingMouse As Boolean = False
				Private mouseOffset As Point
				Private WidgetContent As UIElement
				Private renderTransform As TranslateTransform

				Public Sub New()
						DefaultStyleKey = GetType(WindowPanel)
				End Sub
				Public Overrides Sub OnApplyTemplate()
						MyBase.OnApplyTemplate()
						WidgetContent = TryCast(GetTemplateChild("WidgetContent"), UIElement)
						renderTransform = New TranslateTransform()
						Me.RenderTransform = renderTransform

						Dim headerDragRectangle As UIElement = TryCast(GetTemplateChild("headerDragRectangle"), UIElement)
						Dim imgClose As UIElement = TryCast(GetTemplateChild("imgClose"), UIElement)
						If headerDragRectangle IsNot Nothing Then
								AddHandler headerDragRectangle.MouseLeftButtonDown, AddressOf headerDragRectangle_MouseLeftButtonDown
								AddHandler headerDragRectangle.MouseLeftButtonUp, AddressOf headerDragRectangle_MouseLeftButtonUp
								AddHandler headerDragRectangle.MouseMove, AddressOf headerDragRectangle_MouseMove
						End If
						If imgClose IsNot Nothing Then
								AddHandler imgClose.MouseLeftButtonDown, AddressOf imgClose_MouseLeftButtonDown
						End If
				End Sub
				Private Sub imgClose_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
					Me.Visibility = Visibility.Collapsed
				End Sub

				Public Property IsVisible() As Boolean
					Get
						Return Me.Visibility = Visibility.Visible
					End Get
					Set(ByVal value As Boolean)
						If value = True Then
							Show()
							Else
								Hide()
							End If
					End Set
				End Property
				Public Property IsExpanded() As Boolean
					Get
						Return WidgetContent.Visibility = Visibility.Visible
					End Get
					Set(ByVal value As Boolean)
						If value = True Then
							WidgetContent.Visibility = Visibility.Visible
							Else
								WidgetContent.Visibility = Visibility.Collapsed
							End If
					End Set
				End Property

				Public Sub Show()
					VisualStateManager.GoToState(Me, "Opened", True)
				End Sub
				Public Sub Hide()
					VisualStateManager.GoToState(Me, "Closed", True)
				End Sub
				Public Sub Toggle()
					If IsVisible Then
						Hide()
						Else
							Show()
						End If
				End Sub


				Private Sub headerDragRectangle_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
						Dim rect As Rectangle = TryCast(sender, Rectangle)
						mouseOffset = e.GetPosition(Nothing)
						rect.CaptureMouse()
						isTrackingMouse = True
				End Sub
				Private Sub headerDragRectangle_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
						Dim rect As Rectangle = TryCast(sender, Rectangle)
						rect.ReleaseMouseCapture()
						isTrackingMouse = False
				End Sub
				Private Sub headerDragRectangle_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
						If isTrackingMouse Then
								Dim rect As Rectangle = TryCast(sender, Rectangle)
								Dim point As Point = e.GetPosition(Nothing)
								Dim x0 As Double = Me.renderTransform.X
								Dim y0 As Double = Me.renderTransform.Y
								Me.renderTransform.X = x0 + point.X - mouseOffset.X
								Me.renderTransform.Y = y0 + point.Y - mouseOffset.Y
								mouseOffset = point
						End If
				End Sub

				Public Property ContentTitle() As Object
						Get
							Return CObj(GetValue(ContentTitleProperty))
						End Get
						Set(ByVal value As Object)
							SetValue(ContentTitleProperty, value)
						End Set
				End Property
				Public Shared ReadOnly ContentTitleProperty As DependencyProperty = DependencyProperty.Register("ContentTitle", GetType(Object), GetType(WindowPanel), Nothing)
		End Class
End Namespace
