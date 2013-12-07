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