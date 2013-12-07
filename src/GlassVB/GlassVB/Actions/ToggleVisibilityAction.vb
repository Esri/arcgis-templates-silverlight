Imports System.Windows
Imports System.Windows.Interactivity

Namespace ESRI.ArcGIS.SilverlightMapApp.Actions
		Public Class ToggleVisibilityAction
			Inherits TargetedTriggerAction(Of UIElement)
				Protected Overrides Sub Invoke(ByVal parameter As Object)
						Me.Target.Visibility = If(Me.Target.Visibility = Visibility.Visible, Visibility.Collapsed, Visibility.Visible)
				End Sub
		End Class
End Namespace