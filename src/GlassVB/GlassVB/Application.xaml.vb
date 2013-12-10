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

Namespace GlassVB
  Partial Public Class App
    Inherits Application

    Public Sub New()
      AddHandler Me.Startup, AddressOf Application_Startup
      AddHandler Me.Exit, AddressOf Application_Exit
      AddHandler Me.UnhandledException, AddressOf Application_UnhandledException

      InitializeComponent()
    End Sub

    Private Sub Application_Startup(ByVal sender As Object, ByVal e As StartupEventArgs)
      Me.RootVisual = New MainPage()
    End Sub

    Private Sub Application_Exit(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Private Sub Application_UnhandledException(ByVal sender As Object, ByVal e As ApplicationUnhandledExceptionEventArgs)
      ' If the app is running outside of the debugger then report the exception using
      ' the browser's exception mechanism. On IE this will display it a yellow alert 
      ' icon in the status bar and Firefox will display a script error.
      If Not Debugger.IsAttached Then

        ' NOTE: This will allow the application to continue running after an exception has been thrown
        ' but not handled. 
        ' For production applications this error handling should be replaced with something that will 
        ' report the error to the website and stop the application.
        e.Handled = True
        Deployment.Current.Dispatcher.BeginInvoke(Sub() ReportErrorToDOM(e))
      End If
    End Sub

    Private Sub ReportErrorToDOM(ByVal e As ApplicationUnhandledExceptionEventArgs)
      Try
        Dim errorMsg As String = e.ExceptionObject.Message + e.ExceptionObject.StackTrace
        errorMsg = errorMsg.Replace(""""c, "'"c).Replace(vbCrLf, vbLf)

        System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(""Unhandled Error in Silverlight Application " & errorMsg & """);")
      Catch e1 As Exception
      End Try
    End Sub
  End Class
End Namespace
