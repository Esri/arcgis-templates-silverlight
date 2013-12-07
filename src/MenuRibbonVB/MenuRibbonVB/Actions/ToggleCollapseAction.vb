Imports System.Windows
Imports System.Windows.Interactivity

Namespace ESRI.ArcGIS.SilverlightMapApp.Actions
		Public Class ToggleCollapseAction
			Inherits TargetedTriggerAction(Of CollapsiblePanel)
				Protected Overrides Sub Invoke(ByVal parameter As Object)
						Me.Target.IsExpanded = Not Me.Target.IsExpanded
				End Sub
		End Class
End Namespace
