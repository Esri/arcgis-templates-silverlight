Imports System.Windows
Imports System.Windows.Interactivity
Imports ESRI.ArcGIS.SilverlightMapApp

Namespace ESRI.ArcGIS.SilverlightMapApp.Actions
		Public Class ToggleExpandCollapseAction
			Inherits TargetedTriggerAction(Of UIElement)
				Protected Overrides Sub Invoke(ByVal parameter As Object)
						Dim panel As GlassPanel = TryCast(Me.Target, GlassPanel)
						If panel.IsOpen Then
								panel.Collapse()
						Else
								panel.Expand()
						End If
				End Sub
		End Class
End Namespace
