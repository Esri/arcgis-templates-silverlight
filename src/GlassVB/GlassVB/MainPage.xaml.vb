Imports System.Windows
Imports System.Windows.Controls
Imports ESRI.ArcGIS.Client


Namespace GlassVB
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
      'nav.OverviewMap2 = OverView;

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
