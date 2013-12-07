Imports System.Windows
Imports System.Windows.Interactivity

Namespace ESRI.ArcGIS.SilverlightMapApp.Actions
	''' <summary>
	''' Toggles collapse state of the <see cref="CollapsiblePanel"/>
	''' </summary>
	Public Class ToggleCollapseAction
		Inherits TargetedTriggerAction(Of CollapsiblePanel)
		''' <summary>
		''' Invokes the action.
		''' </summary>
		''' <param name="parameter">The parameter to the action. If the Action 
		''' does not require a parameter, the parameter may be set to a null 
		''' reference.</param>
		Protected Overrides Sub Invoke(ByVal parameter As Object)
			Me.Target.IsExpanded = Not Me.Target.IsExpanded
		End Sub
	End Class
End Namespace
