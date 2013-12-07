Imports System.Windows.Interactivity

Namespace ESRI.ArcGIS.SilverlightMapApp.Actions
		Public Class ToggleWindowVisibilityAction
			Inherits TargetedTriggerAction(Of DraggableWindow)
				Protected Overrides Sub Invoke(ByVal parameter As Object)
						Me.Target.IsOpen = Not Me.Target.IsOpen
				End Sub
		End Class
End Namespace
