Imports System.Windows
Imports System.Windows.Interactivity

Namespace ESRI.ArcGIS.SilverlightMapApp.Actions
		Public Class ToggleFullScreenAction
			Inherits TriggerAction(Of UIElement)
				Protected Overrides Sub Invoke(ByVal parameter As Object)
						Application.Current.Host.Content.IsFullScreen = Not Application.Current.Host.Content.IsFullScreen
				End Sub
		End Class
End Namespace
