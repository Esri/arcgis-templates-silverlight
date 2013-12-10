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
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports ESRI.ArcGIS.Client
Imports ESRI.ArcGIS.SilverlightMapApp

Namespace MinimalVB
  Partial Public Class MainPage
    Inherits UserControl
    Private usePlaneProjection As Boolean = False
    Private marginLRFactor As Double = 0.25
    Private marginTBFactor As Double = 0.5

    Public Sub New()
      InitializeComponent()
      Dim strvalue As String = TryCast(Application.Current.Resources("UsePlaneProjection"), String)
      If strvalue IsNot Nothing Then
        usePlaneProjection = Boolean.Parse(strvalue)
      End If
      If usePlaneProjection Then
        strvalue = TryCast(Application.Current.Resources("MapLeftRightMarginFactor"), String)
        If strvalue IsNot Nothing Then
          marginLRFactor = Double.Parse(strvalue)
        End If
        strvalue = TryCast(Application.Current.Resources("MapTopBottomMarginFactor"), String)
        If strvalue IsNot Nothing Then
          marginTBFactor = Double.Parse(strvalue)
        End If
      End If
    End Sub

    Private Sub nav_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
      nav.MapProjection = mapPlaneProjection

    End Sub

    Private Sub UserControl_SizeChanged(ByVal sender As Object, ByVal e As SizeChangedEventArgs)
      If usePlaneProjection Then
        Dim mwidth As Double = MapBorder.ActualWidth * marginLRFactor * -1
        Dim mheight As Double = MapBorder.ActualHeight * marginTBFactor * -1
        Map.Margin = New Thickness(mwidth, mheight, mwidth, mheight)
      End If
    End Sub

  End Class
End Namespace
