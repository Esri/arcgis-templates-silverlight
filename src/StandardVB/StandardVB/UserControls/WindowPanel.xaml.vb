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
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes

Namespace ESRI.ArcGIS.SilverlightMapApp
	''' <summary>
	''' Draggable Window Panel
	''' </summary>
	<TemplateVisualState(Name := "Opened", GroupName := "CommonStates"), TemplateVisualState(Name := "Closed", GroupName := "CommonStates")>
	Public Class WindowPanel
		Inherits ContentControl
		Private isTrackingMouse As Boolean = False
		Private mouseOffset As Point
		Private WidgetContent As UIElement
		Private renderTransform As TranslateTransform

		''' <summary>
		''' Initializes a new instance of the <see cref="WindowPanel"/> class.
		''' </summary>
		Public Sub New()
			DefaultStyleKey = GetType(WindowPanel)
		End Sub

		''' <summary>
		''' When overridden in a derived class, is invoked whenever application
		''' code or internal processes (such as a rebuilding layout pass) call
		''' <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. 
		''' In simplest terms, this means the method is called just before a UI 
		''' element displays in an application. For more information, see Remarks.
		''' </summary>
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

			ChangeVisualState(False)
		End Sub

		#Region "Properties"

		''' <summary>
		''' Gets or sets a value indicating whether this control is visible.
		''' </summary>
		''' <value>
		''' 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
		''' </value>
		Public Property IsOpen() As Boolean
			Get
				Return CBool(GetValue(IsOpenProperty))
			End Get
			Set(ByVal value As Boolean)
				SetValue(IsOpenProperty, value)
			End Set
		End Property

		''' <summary>
		''' Identifies the <see cref="ContentTitle"/> dependency property.
		''' </summary>
		Public Shared ReadOnly IsOpenProperty As DependencyProperty = DependencyProperty.Register("IsOpen", GetType(Boolean), GetType(WindowPanel), New PropertyMetadata(True, AddressOf OnIsOpenPropertyChanged))

		Private Shared Sub OnIsOpenPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
			Dim panel As WindowPanel = TryCast(d, WindowPanel)
			panel.ChangeVisualState(True)
		End Sub

		''' <summary>
		''' Gets or sets a value indicating whether this instance is expanded.
		''' </summary>
		''' <value>
		''' 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
		''' </value>
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

		''' <summary>
		''' Gets or sets the content title.
		''' </summary>
		Public Property ContentTitle() As Object
			Get
				Return CObj(GetValue(ContentTitleProperty))
			End Get
			Set(ByVal value As Object)
				SetValue(ContentTitleProperty, value)
			End Set
		End Property

		''' <summary>
		''' Identifies the <see cref="ContentTitle"/> dependency property.
		''' </summary>
		Public Shared ReadOnly ContentTitleProperty As DependencyProperty = DependencyProperty.Register("ContentTitle", GetType(Object), GetType(WindowPanel), Nothing)

		#End Region

		''' <summary>
		''' Hides the window.
		''' </summary>
		Private Sub ChangeVisualState(ByVal useTransitions As Boolean)
			If IsOpen OrElse System.ComponentModel.DesignerProperties.IsInDesignTool Then
				VisualStateManager.GoToState(Me, "Opened", useTransitions)
			Else
				VisualStateManager.GoToState(Me, "Closed", useTransitions)
			End If
		End Sub

		#Region "Public Methods"
		''' <summary>
		''' Toggles this show/hide state.
		''' </summary>
		Public Sub Toggle()
			IsOpen = Not IsOpen
		End Sub

		#End Region

		#Region "Event Handlers for dragging the window"

		''' <summary>
		''' Handles the MouseLeftButtonDown event of the imgClose control
		''' and collapses the control
		''' </summary>
		''' <param name="sender">The source of the event.</param>
		''' <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> 
		''' instance containing the event data.</param>
		Private Sub imgClose_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			IsOpen = False
		End Sub
		''' <summary>
		''' Handles the MouseLeftButtonDown event of the headerDragRectangle control.
		''' This is fired when the user starts to drag the window.
		''' </summary>
		''' <param name="sender">The source of the event.</param>
		''' <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/>
		''' instance containing the event data.</param>
		Private Sub headerDragRectangle_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			Dim rect As Rectangle = TryCast(sender, Rectangle)
			mouseOffset = e.GetPosition(Nothing)
			rect.CaptureMouse()
			isTrackingMouse = True
		End Sub

		''' <summary>
		''' Handles the MouseLeftButtonUp event of the headerDragRectangle control.
		''' This is fired when the user stopped dragging the window.
		''' </summary>
		''' <param name="sender">The source of the event.</param>
		''' <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> 
		''' instance containing the event data.</param>
		Private Sub headerDragRectangle_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			Dim rect As Rectangle = TryCast(sender, Rectangle)
			rect.ReleaseMouseCapture()
			isTrackingMouse = False
		End Sub

		''' <summary>
		''' Handles the MouseMove event of the headerDragRectangle control.
		''' This is fired when the user is dragging the window.
		''' </summary>
		''' <param name="sender">The source of the event.</param>
		''' <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/>
		''' instance containing the event data.</param>
		Private Sub headerDragRectangle_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
			If isTrackingMouse Then
				Dim rect As Rectangle = TryCast(sender, Rectangle)
				Dim point As Point = e.GetPosition(Nothing)
				Dim x0 As Double = Me.renderTransform.X
				Dim y0 As Double = Me.renderTransform.Y
				Dim sign As Double = 1
				If FlowDirection = System.Windows.FlowDirection.RightToLeft Then
					sign = -1
				End If
				Me.renderTransform.X = x0 + (point.X - mouseOffset.X) * sign
				Me.renderTransform.Y = y0 + point.Y - mouseOffset.Y
				mouseOffset = point
			End If
		End Sub

		#End Region

	End Class
End Namespace
