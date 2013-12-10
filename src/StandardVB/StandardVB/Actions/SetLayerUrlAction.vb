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
Imports ESRI.ArcGIS.Client

Namespace ESRI.ArcGIS.SilverlightMapApp.Actions
	''' <summary>
	''' When triggered, sets the URL property on any ArcGIS Server Layer.
	''' </summary>
	Public Class SetLayerUrlAction
		Inherits TargetedTriggerAction(Of Map)
		''' <summary>
		''' Invokes the action.
		''' </summary>
		''' <param name="parameter">The parameter to the action. If the Action
		''' does not require a parameter, the parameter may be set to a null 
		''' reference.</param>
		Protected Overrides Sub Invoke(ByVal parameter As Object)
			If Not String.IsNullOrEmpty(LayerID) Then
				Dim l As Layer = Target.Layers(LayerID)
				If TypeOf l Is ArcGISTiledMapServiceLayer Then
					TryCast(l, ArcGISTiledMapServiceLayer).Url = Url
				ElseIf TypeOf l Is ArcGISDynamicMapServiceLayer Then
					TryCast(l, ArcGISDynamicMapServiceLayer).Url = Url
				ElseIf TypeOf l Is ArcGISImageServiceLayer Then
					TryCast(l, ArcGISImageServiceLayer).Url = Url
				End If
			End If
		End Sub

		''' <summary>
		''' Gets or sets the ID of layer to zoom to.
		''' </summary>
		''' <value>The layer ID.</value>
		Public Property LayerID() As String
			Get
				Return CStr(GetValue(LayerIDProperty))
			End Get
			Set(ByVal value As String)
				SetValue(LayerIDProperty, value)
			End Set
		End Property

		''' <summary>
		''' Identifies the <see cref="LayerID"/> dependency property.
		''' </summary>
		Public Shared ReadOnly LayerIDProperty As DependencyProperty = DependencyProperty.Register("LayerID", GetType(String), GetType(SetLayerUrlAction), Nothing)

		''' <summary>
		''' Gets or sets the ID of layer to zoom to.
		''' </summary>
		''' <value>The layer ID.</value>
		Public Property Url() As String
			Get
				Return CStr(GetValue(UrlProperty))
			End Get
			Set(ByVal value As String)
				SetValue(UrlProperty, value)
			End Set
		End Property

		''' <summary>
		''' Identifies the <see cref="Url"/> dependency property.
		''' </summary>
		Public Shared ReadOnly UrlProperty As DependencyProperty = DependencyProperty.Register("Url", GetType(String), GetType(SetLayerUrlAction), Nothing)

	End Class
End Namespace
