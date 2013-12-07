Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media

Namespace ESRI.ArcGIS.SilverlightMapApp
		''' <summary>
		''' Resizable and draggable custom window control
		''' </summary>
		Public Class DropDownMenu
			Inherits ContentControl
				Public Sub New()
						DefaultStyleKey = GetType(DropDownMenu)
						AddHandler Me.MouseEnter, AddressOf DropDownMenu_MouseEnter
						AddHandler Me.MouseLeave, AddressOf DropDownMenu_MouseLeave
				End Sub

				Private Sub DropDownMenu_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
						GoToState(True, "Hidden")
				End Sub

				Private Sub DropDownMenu_MouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs)
						GoToState(True, "Visible")
				End Sub


				Private Function GoToState(ByVal useTransitions As Boolean, ByVal stateName As String) As Boolean
						Return VisualStateManager.GoToState(Me, stateName, useTransitions)
				End Function

				Protected Overrides Sub OnContentChanged(ByVal oldContent As Object, ByVal newContent As Object)
						MyBase.OnContentChanged(oldContent, newContent)
						If oldContent IsNot Nothing AndAlso TypeOf oldContent Is UIElement Then
								RemoveHandler TryCast(oldContent, UIElement).MouseEnter, AddressOf DropDownMenu_MouseEnter
								RemoveHandler TryCast(oldContent, UIElement).MouseLeave, AddressOf DropDownMenu_MouseLeave
						End If
						If newContent IsNot Nothing AndAlso TypeOf newContent Is UIElement Then
								AddHandler TryCast(newContent, UIElement).MouseEnter, AddressOf DropDownMenu_MouseEnter
								AddHandler TryCast(newContent, UIElement).MouseLeave, AddressOf DropDownMenu_MouseLeave
						End If
				End Sub

				''' <summary>
				''' When overridden in a derived class, is invoked whenever application code or
				''' internal processes (such as a rebuilding layout pass) call
				''' <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
				''' </summary>
				Public Overrides Sub OnApplyTemplate()
						MyBase.OnApplyTemplate()
						Dim isDesignMode As Boolean = System.ComponentModel.DesignerProperties.GetIsInDesignMode(Me)
						GoToState(False,If(isDesignMode, "Visible", "Hidden")) 'Show submenu when in designmode
				End Sub

				''' <summary>
				''' Identifies the <see cref="MenuContent"/> dependency property.
				''' </summary>
				Public Shared ReadOnly MenuHeaderProperty As DependencyProperty = DependencyProperty.Register("MenuHeader", GetType(Object), GetType(DropDownMenu), Nothing)

				''' <summary>
				''' Gets or sets MenuContent.
				''' </summary>
				Public Property MenuHeader() As Object
						Get
							Return CObj(GetValue(MenuHeaderProperty))
						End Get
						Set(ByVal value As Object)
							SetValue(MenuHeaderProperty, value)
						End Set
				End Property

				''' <summary>
				''' Gets or sets the template that is used to display the content of the
				''' control's header.
				''' </summary>
				''' <value>
				''' The template that is used to display the content of the control's
				''' header. The default is null.
				''' </value>
				Public Property MenuHeaderTemplate() As DataTemplate
						Get
							Return CType(GetValue(MenuHeaderTemplateProperty), DataTemplate)
						End Get
						Set(ByVal value As DataTemplate)
							SetValue(MenuHeaderTemplateProperty, value)
						End Set
				End Property

				''' <summary>
				''' Identifies the <see cref="HeaderTemplate" /> dependency property.
				''' </summary>
				Public Shared ReadOnly MenuHeaderTemplateProperty As DependencyProperty = DependencyProperty.Register("MenuHeaderTemplate", GetType(DataTemplate), GetType(DropDownMenu), Nothing)

		End Class
End Namespace