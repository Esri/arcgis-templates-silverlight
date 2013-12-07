Imports System.Windows.Interactivity

Namespace ESRI.ArcGIS.SilverlightMapApp.Actions
	''' <summary>
	''' Toggles the IsOpen property of the <see cref="DraggableWindow"/>.
	''' </summary>
	Public Class ToggleWindowVisibilityAction
		Inherits TargetedTriggerAction(Of DraggableWindow)
		''' <summary>
		''' Invokes the action.
		''' </summary>
		''' <param name="parameter">The parameter to the action. If the Action 
		''' does not require a parameter, the parameter may be set to a null 
		''' reference.</param>
		Protected Overrides Sub Invoke(ByVal parameter As Object)
			Me.Target.IsOpen = Not Me.Target.IsOpen
		End Sub
	End Class
End Namespace
