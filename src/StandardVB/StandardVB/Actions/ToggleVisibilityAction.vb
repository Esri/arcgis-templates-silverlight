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
Imports System.Windows.Interactivity

Namespace ESRI.ArcGIS.SilverlightMapApp.Actions
	''' <summary>
	''' Toggles the visibility state of any UIElement.
	''' </summary>
	Public Class ToggleVisibilityAction
		Inherits TargetedTriggerAction(Of UIElement)
		''' <summary>
		''' Invokes the action.
		''' </summary>
		''' <param name="parameter">The parameter to the action. If the Action 
		''' does not require a parameter, the parameter may be set to a null 
		''' reference.</param>
		Protected Overrides Sub Invoke(ByVal parameter As Object)
			Me.Target.Visibility = If(Me.Target.Visibility = Visibility.Visible, Visibility.Collapsed, Visibility.Visible)
		End Sub
	End Class
End Namespace
