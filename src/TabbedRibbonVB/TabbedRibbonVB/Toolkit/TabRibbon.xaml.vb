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

Namespace Toolkit
		<TemplatePart(Name := "LayoutRoot", Type := GetType(Grid)), TemplatePart(Name := "PushPin", Type := GetType(Button))>
		Public Class TabRibbon
			Inherits TabControl
				Private tabControl As TabControl
'INSTANT VB NOTE: The variable tabRibbon was renamed since Visual Basic does not allow class members with the same name:
				Private tabRibbon_Renamed As TabRibbon
				Private items As ItemCollection
				Private RibbonCollapse As Storyboard
				Private RibbonExpand As Storyboard
				Private root As Grid
				Private PushPin As Button

				Private isPinned As Boolean = False

				Public Sub New()
						Me.DefaultStyleKey = GetType(TabRibbon)
				End Sub

				''' <summary>
				''' When overridden in a derived class, is invoked whenever application code or
				''' internal processes (such as a rebuilding layout pass) call
				''' <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
				''' </summary>
				Public Overrides Sub OnApplyTemplate()
						MyBase.OnApplyTemplate()
						tabRibbon_Renamed = Me
						root = TryCast(GetTemplateChild("LayoutRoot"), Grid)
						RibbonCollapse = TryCast(root.Resources("RibbonCollapse"), Storyboard)
						RibbonExpand = TryCast(root.Resources("RibbonExpand"), Storyboard)
						PushPin = TryCast(GetTemplateChild("PushPin"), Button)
						If root IsNot Nothing Then
								AddHandler root.MouseEnter, AddressOf LayoutRoot_MouseEnter
								AddHandler root.MouseLeave, AddressOf LayoutRoot_MouseLeave
						End If
						If RibbonExpand IsNot Nothing Then
								AddHandler RibbonExpand.Completed, AddressOf RibbonExpand_Completed
						End If
						If PushPin IsNot Nothing Then
								AddHandler PushPin.Click, AddressOf PushPin_Click
						End If

				End Sub


				 Private Sub LayoutRoot_MouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs)
						If Not isPinned Then
								If RibbonExpand Is Nothing Then
										RibbonExpand = TryCast(root.Resources("RibbonExpand"), Storyboard)
								End If
								RibbonExpand.Begin()

						End If
				 End Sub

				Private Sub LayoutRoot_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
						If Not isPinned Then
								PushPin.Visibility = Visibility.Collapsed
								If RibbonExpand Is Nothing Then
										RibbonCollapse = TryCast(root.Resources("RibbonCollapse"), Storyboard)
										AddHandler RibbonExpand.Completed, AddressOf RibbonExpand_Completed
								End If
								RibbonCollapse.Begin()
						End If
				End Sub

				Private Sub PushPin_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
						isPinned = Not isPinned
						If isPinned Then
								VisualStateManager.GoToState(PushPin, "Pinned", True)
						Else
								VisualStateManager.GoToState(PushPin, "UnPinned", True)
						End If
				End Sub

				Private Sub RibbonExpand_Completed(ByVal sender As Object, ByVal e As EventArgs)
						PushPin.Visibility = Visibility.Visible
				End Sub

		End Class
End Namespace
