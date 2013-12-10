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
Imports System.Windows.Media.Effects
Imports System.Windows.Shapes

Namespace TextControls
		Partial Public Class RaisedText
			Inherits UserControl
				Public Sub New()
						InitializeComponent()
						If (Not Double.IsNaN(Me.Width)) AndAlso Me.Width > 0 Then
								DisplayText.Width = Me.Width
								DisplayTextBlur.Width = Me.Width
						End If
						If (Not Double.IsNaN(Me.MaxWidth)) AndAlso Me.MaxWidth > 0 Then
								DisplayText.MaxWidth = Me.MaxWidth
								DisplayTextBlur.MaxWidth = Me.MaxWidth
						End If


				End Sub



				#Region "Dependency Properties"

				#Region "Text"
				''' <summary>
				''' Identifies the <see cref="Text"/> dependency property.
				''' </summary>
				Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(RaisedText), New PropertyMetadata("", AddressOf OnTextPropertyChanged))
				''' <summary>
				''' Gets or sets DisplayText.
				''' </summary>
				Public Property Text() As String
						Get
							Return CStr(GetValue(TextProperty))
						End Get
						Set(ByVal value As String)
							SetValue(TextProperty, value)
						End Set
				End Property

				 Private Shared Sub OnTextPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim text As String = CStr(e.NewValue)
						rtext.DisplayText.Text = text
						rtext.DisplayTextBlur.Text = text
				 End Sub
				#End Region

				#Region "TextBrush"
				Public Shared ReadOnly TextBrushProperty As DependencyProperty = DependencyProperty.Register("TextBrush", GetType(Brush), GetType(RaisedText), New PropertyMetadata(New SolidColorBrush(Colors.White), AddressOf OnTextBrushPropertyChanged))

				Public Property TextBrush() As Brush
						Get
							Return CType(GetValue(TextBrushProperty), Brush)
						End Get
						Set(ByVal value As Brush)
							SetValue(TextBrushProperty, value)
						End Set
				End Property

				Private Shared Sub OnTextBrushPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim brush As Brush = TryCast(e.NewValue, Brush)
						rtext.DisplayText.Foreground = brush
				End Sub
				#End Region

				#Region "TextSize"
				Public Shared ReadOnly TextSizeProperty As DependencyProperty = DependencyProperty.Register("TextSize", GetType(Double), GetType(RaisedText), New PropertyMetadata(10R, AddressOf OnTextSizePropertyChanged))

				Public Property TextSize() As Double
						Get
							Return CDbl(GetValue(TextSizeProperty))
						End Get
						Set(ByVal value As Double)
							SetValue(TextSizeProperty, value)
						End Set
				End Property

				Private Shared Sub OnTextSizePropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						If Not Double.IsNaN(CDbl(e.NewValue)) Then
								Dim size As Double = CDbl(e.NewValue)
								rtext.DisplayText.FontSize = size
								rtext.DisplayTextBlur.FontSize = size
						End If
				End Sub
				#End Region

				#Region "TextFamily"
				Public Shared ReadOnly TextFamilyProperty As DependencyProperty = DependencyProperty.Register("TextFamily", GetType(FontFamily), GetType(RaisedText), New PropertyMetadata(Nothing, AddressOf OnTextFamilyPropertyChanged))

				Public Property TextFamily() As FontFamily
						Get
							Return CType(GetValue(TextFamilyProperty), FontFamily)
						End Get
						Set(ByVal value As FontFamily)
							SetValue(TextFamilyProperty, value)
						End Set
				End Property

				Private Shared Sub OnTextFamilyPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim font As FontFamily = CType(e.NewValue, FontFamily)
						rtext.DisplayText.FontFamily = font
						rtext.DisplayTextBlur.FontFamily = font
				End Sub
				#End Region

				#Region "TextStyle"
				Public Shared ReadOnly TextStyleProperty As DependencyProperty = DependencyProperty.Register("TextStyle", GetType(FontStyle), GetType(RaisedText), New PropertyMetadata(FontStyles.Normal, AddressOf OnTextStylePropertyChanged))

				Public Property TextStyle() As FontStyle
						Get
							Return CType(GetValue(TextStyleProperty), FontStyle)
						End Get
						Set(ByVal value As FontStyle)
							SetValue(TextStyleProperty, value)
						End Set
				End Property

				Private Shared Sub OnTextStylePropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim style As FontStyle = CType(e.NewValue, FontStyle)
						rtext.DisplayText.FontStyle = style
						rtext.DisplayTextBlur.FontStyle = style
				End Sub
				#End Region

				#Region "TextWeight"
				Public Shared ReadOnly TextWeightProperty As DependencyProperty = DependencyProperty.Register("TextWeight", GetType(FontWeight), GetType(RaisedText), New PropertyMetadata(FontWeights.Normal, AddressOf OnTextWeightPropertyChanged))

				Public Property TextWeight() As FontWeight
						Get
							Return CType(GetValue(TextWeightProperty), FontWeight)
						End Get
						Set(ByVal value As FontWeight)
							SetValue(TextWeightProperty, value)
						End Set
				End Property

				Private Shared Sub OnTextWeightPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim weight As FontWeight = CType(e.NewValue, FontWeight)
						rtext.DisplayText.FontWeight = weight
						rtext.DisplayTextBlur.FontWeight = weight
				End Sub
				#End Region

				#Region "TextAlignment"
				Public Shared ReadOnly TextAlignmentProperty As DependencyProperty = DependencyProperty.Register("TextAlignment", GetType(TextAlignment), GetType(RaisedText), New PropertyMetadata(TextAlignment.Left, AddressOf OnTextAlignmentPropertyChanged))

				Public Property TextAlignment() As TextAlignment
						Get
							Return CType(GetValue(TextAlignmentProperty), TextAlignment)
						End Get
						Set(ByVal value As TextAlignment)
							SetValue(TextAlignmentProperty, value)
						End Set
				End Property

				Private Shared Sub OnTextAlignmentPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim align As TextAlignment = CType(e.NewValue, TextAlignment)
						rtext.DisplayText.TextAlignment = align
						rtext.DisplayTextBlur.TextAlignment = align
				End Sub
				#End Region

				#Region "TextWrapping"
				Public Shared ReadOnly TextWrappingProperty As DependencyProperty = DependencyProperty.Register("TextWrapping", GetType(TextWrapping), GetType(RaisedText), New PropertyMetadata(TextWrapping.NoWrap, AddressOf OnTextWrappingPropertyChanged))

				Public Property TextWrapping() As TextWrapping
						Get
							Return CType(GetValue(TextWrappingProperty), TextWrapping)
						End Get
						Set(ByVal value As TextWrapping)
							SetValue(TextWrappingProperty, value)
						End Set
				End Property

				Private Shared Sub OnTextWrappingPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim wrap As TextWrapping = CType(e.NewValue, TextWrapping)
						rtext.DisplayText.TextWrapping = wrap
						rtext.DisplayTextBlur.TextWrapping = wrap
				End Sub
				#End Region

				#Region "BlurBrush"
				Public Shared ReadOnly BlurBrushProperty As DependencyProperty = DependencyProperty.Register("BlurBrush", GetType(Brush), GetType(RaisedText), New PropertyMetadata(New SolidColorBrush(Colors.Black), AddressOf OnBlurBrushPropertyChanged))

				Public Property BlurBrush() As Brush
						Get
							Return CType(GetValue(BlurBrushProperty), Brush)
						End Get
						Set(ByVal value As Brush)
							SetValue(BlurBrushProperty, value)
						End Set
				End Property

				Private Shared Sub OnBlurBrushPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim brush As Brush = TryCast(e.NewValue, Brush)
						rtext.DisplayTextBlur.Foreground = brush
				End Sub
				#End Region

				#Region "BlurRadius"
				Public Shared ReadOnly BlurRadiusProperty As DependencyProperty = DependencyProperty.Register("BlurRadius", GetType(Double), GetType(RaisedText), New PropertyMetadata(5R, AddressOf OnBlurRadiusPropertyChanged))

				Public Property BlurRadius() As Double
						Get
							Return CDbl(GetValue(BlurRadiusProperty))
						End Get
						Set(ByVal value As Double)
							SetValue(BlurRadiusProperty, value)
						End Set
				End Property

				Private Shared Sub OnBlurRadiusPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim radius As Double = CDbl(e.NewValue)
						Dim effect As BlurEffect = TryCast(rtext.DisplayTextBlur.Effect, BlurEffect)
						effect.Radius = radius
				End Sub
				#End Region

				#Region "ShadowColor"
				Public Shared ReadOnly ShadowColorProperty As DependencyProperty = DependencyProperty.Register("ShadowColor", GetType(Color), GetType(RaisedText), New PropertyMetadata(Colors.Black, AddressOf OnShadowColorPropertyChanged))

				Public Property ShadowColor() As Color
						Get
							Return CType(GetValue(ShadowColorProperty), Color)
						End Get
						Set(ByVal value As Color)
							SetValue(ShadowColorProperty, value)
						End Set
				End Property

				Private Shared Sub OnShadowColorPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim color As Color = CType(e.NewValue, Color)
						Dim effect As DropShadowEffect = TryCast(rtext.DisplayText.Effect, DropShadowEffect)
						effect.Color = color

				End Sub
				#End Region

				#Region "ShadowBlurRadius"
				Public Shared ReadOnly ShadowBlurRadiusProperty As DependencyProperty = DependencyProperty.Register("ShadowBlurRadius", GetType(Double), GetType(RaisedText), New PropertyMetadata(5R, AddressOf OnShadowBlurRadiusPropertyChanged))

				Public Property ShadowBlurRadius() As Double
						Get
							Return CDbl(GetValue(ShadowBlurRadiusProperty))
						End Get
						Set(ByVal value As Double)
							SetValue(ShadowBlurRadiusProperty, value)
						End Set
				End Property

				Private Shared Sub OnShadowBlurRadiusPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim radius As Double = CDbl(e.NewValue)
						Dim effect As DropShadowEffect = TryCast(rtext.DisplayText.Effect, DropShadowEffect)
						effect.BlurRadius = radius
				End Sub
				#End Region

				#Region "ShadowDirection"
				Public Shared ReadOnly ShadowDirectionProperty As DependencyProperty = DependencyProperty.Register("ShadowDirection", GetType(Double), GetType(RaisedText), New PropertyMetadata(-45R, AddressOf OnShadowDirectionPropertyChanged))

				Public Property ShadowDirection() As Double
						Get
							Return CDbl(GetValue(ShadowDirectionProperty))
						End Get
						Set(ByVal value As Double)
							SetValue(ShadowDirectionProperty, value)
						End Set
				End Property

				Private Shared Sub OnShadowDirectionPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim direction As Double = CDbl(e.NewValue)
						Dim effect As DropShadowEffect = TryCast(rtext.DisplayText.Effect, DropShadowEffect)
						effect.Direction = direction
				End Sub
				#End Region

				#Region "ShadowDepth"
				Public Shared ReadOnly ShadowDepthProperty As DependencyProperty = DependencyProperty.Register("ShadowDepth", GetType(Double), GetType(RaisedText), New PropertyMetadata(2R, AddressOf OnShadowDepthPropertyChanged))

				Public Property ShadowDepth() As Double
						Get
							Return CDbl(GetValue(ShadowDepthProperty))
						End Get
						Set(ByVal value As Double)
							SetValue(ShadowDepthProperty, value)
						End Set
				End Property

				Private Shared Sub OnShadowDepthPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim depth As Double = CDbl(e.NewValue)
						Dim effect As DropShadowEffect = TryCast(rtext.DisplayText.Effect, DropShadowEffect)
						effect.ShadowDepth = depth
				End Sub
				#End Region

				#Region "ShadowOpacity"
				Public Shared ReadOnly ShadowOpacityProperty As DependencyProperty = DependencyProperty.Register("ShadowOpacity", GetType(Double), GetType(RaisedText), New PropertyMetadata(0.85R, AddressOf OnShadowOpacityPropertyChanged))

				Public Property ShadowOpacity() As Double
						Get
							Return CDbl(GetValue(ShadowOpacityProperty))
						End Get
						Set(ByVal value As Double)
							SetValue(ShadowOpacityProperty, value)
						End Set
				End Property

				Private Shared Sub OnShadowOpacityPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
						Dim rtext As RaisedText = TryCast(d, RaisedText)
						Dim opacity As Double = CDbl(e.NewValue)
						Dim effect As DropShadowEffect = TryCast(rtext.DisplayText.Effect, DropShadowEffect)
						effect.Opacity = opacity
				End Sub
				#End Region

				#End Region

		End Class
End Namespace
