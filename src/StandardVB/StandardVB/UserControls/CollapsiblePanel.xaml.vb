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
Imports System.Windows.Controls.Primitives

Namespace ESRI.ArcGIS.SilverlightMapApp
	''' <summary>
	''' Panel that can be collapsed with an animation defined in the visual state.
	''' </summary>
	<TemplateVisualState(Name := "Collapsed", GroupName := "ViewStates"), TemplateVisualState(Name := "Expanded", GroupName := "ViewStates")>
	Public Class CollapsiblePanel
		Inherits ContentControl
		''' <summary>
		''' Initializes a new instance of the <see cref="CollapsiblePanel"/> class.
		''' </summary>
		Public Sub New()
			DefaultStyleKey = GetType(CollapsiblePanel)
		End Sub

		#Region "Dependency Properties"

		''' <summary>
		''' Gets or sets a value indicating whether this control is expanded.
		''' </summary>
		''' <value>
		''' 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
		''' </value>
		Public Property IsExpanded() As Boolean
			Get
				Return CBool(GetValue(IsExpandedProperty))
			End Get
			Set(ByVal value As Boolean)
				SetValue(IsExpandedProperty, value)
			End Set
		End Property

		''' <summary>
		''' Identifies the <see cref="IsExpanded"/> dependency property.
		''' </summary>
		Public Shared ReadOnly IsExpandedProperty As DependencyProperty = DependencyProperty.Register("IsExpanded", GetType(Boolean), GetType(CollapsiblePanel), New PropertyMetadata(True, AddressOf OnIsExpandedPropertyChanged))

		Private Shared Sub OnIsExpandedPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
			TryCast(d, CollapsiblePanel).ChangeVisualState(True)
		End Sub

		#End Region

		''' <summary>
		''' When overridden in a derived class, is invoked whenever application 
		''' code or internal processes (such as a rebuilding layout pass) call 
		''' <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In 
		''' simplest terms, this means the method is called just before a UI 
		''' element displays in an application.
		''' </summary>
		Public Overrides Sub OnApplyTemplate()
			MyBase.OnApplyTemplate()
			ChangeVisualState(False)
		End Sub

		''' <summary>
		''' Updates the visual state of the control.
		''' </summary>
		''' <param name="useTransitions">if set to <c>true</c> transitions will be used.</param>
		Private Sub ChangeVisualState(ByVal useTransitions As Boolean)
			If IsExpanded Then
				VisualStateManager.GoToState(Me, "Expanded", useTransitions)
			Else
				VisualStateManager.GoToState(Me, "Collapsed", useTransitions)
			End If
		End Sub
	End Class
End Namespace
