using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ESRI.ArcGIS.SilverlightMapApp
{
	/// <summary>
	/// Draggable Window Panel
	/// </summary>
	[TemplateVisualState(Name = "Opened", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Closed", GroupName = "CommonStates")]
	public class WindowPanel : ContentControl
	{
		private bool isTrackingMouse = false;
		private Point mouseOffset;
		private UIElement WidgetContent;
		private TranslateTransform renderTransform;

		/// <summary>
		/// Initializes a new instance of the <see cref="WindowPanel"/> class.
		/// </summary>
		public WindowPanel()
		{
			DefaultStyleKey = typeof(WindowPanel);
		}

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application
		/// code or internal processes (such as a rebuilding layout pass) call
		/// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. 
		/// In simplest terms, this means the method is called just before a UI 
		/// element displays in an application. For more information, see Remarks.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			WidgetContent = GetTemplateChild("WidgetContent") as UIElement;
			this.RenderTransform = renderTransform = new TranslateTransform();

			UIElement headerDragRectangle = GetTemplateChild("headerDragRectangle") as UIElement;
			UIElement imgClose = GetTemplateChild("imgClose") as UIElement;
			if (headerDragRectangle != null)
			{
				headerDragRectangle.MouseLeftButtonDown += headerDragRectangle_MouseLeftButtonDown;
				headerDragRectangle.MouseLeftButtonUp += headerDragRectangle_MouseLeftButtonUp;
				headerDragRectangle.MouseMove += headerDragRectangle_MouseMove;
			}
			if (imgClose != null)
				imgClose.MouseLeftButtonDown += imgClose_MouseLeftButtonDown;

			ChangeVisualState(false);
		}

		#region Properties
		
		/// <summary>
		/// Gets or sets a value indicating whether this control is visible.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
		/// </value>
		public bool IsOpen
		{
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ContentTitle"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IsOpenProperty =
			DependencyProperty.Register("IsOpen", typeof(bool), typeof(WindowPanel), new PropertyMetadata(true, OnIsOpenPropertyChanged));

		private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			WindowPanel panel = d as WindowPanel;
			panel.ChangeVisualState(true);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is expanded.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
		/// </value>
		public bool IsExpanded
		{
			get
			{
				return WidgetContent.Visibility == Visibility.Visible;
			}
			set
			{
				if (value == true)
					WidgetContent.Visibility = Visibility.Visible;
				else
					WidgetContent.Visibility = Visibility.Collapsed;
			}
		}

		/// <summary>
		/// Gets or sets the content title.
		/// </summary>
		public object ContentTitle
		{
			get { return (object)GetValue(ContentTitleProperty); }
			set { SetValue(ContentTitleProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ContentTitle"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ContentTitleProperty =
			DependencyProperty.Register("ContentTitle", typeof(object), typeof(WindowPanel), null);

		#endregion

		/// <summary>
		/// Hides the window.
		/// </summary>
		private void ChangeVisualState(bool useTransitions)
		{
			if(IsOpen || System.ComponentModel.DesignerProperties.IsInDesignTool)
				VisualStateManager.GoToState(this, "Opened", useTransitions);
			else
				VisualStateManager.GoToState(this, "Closed", useTransitions);
		}

		#region Public Methods
		/// <summary>
		/// Toggles this show/hide state.
		/// </summary>
		public void Toggle()
		{
			IsOpen = !IsOpen;
		}

		#endregion

		#region Event Handlers for dragging the window

		/// <summary>
		/// Handles the MouseLeftButtonDown event of the imgClose control
		/// and collapses the control
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> 
		/// instance containing the event data.</param>
		private void imgClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			IsOpen = false;
		}
		/// <summary>
		/// Handles the MouseLeftButtonDown event of the headerDragRectangle control.
		/// This is fired when the user starts to drag the window.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/>
		/// instance containing the event data.</param>
		private void headerDragRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Rectangle rect = sender as Rectangle;
			mouseOffset = e.GetPosition(null);
			rect.CaptureMouse();
			isTrackingMouse = true;
		}

		/// <summary>
		/// Handles the MouseLeftButtonUp event of the headerDragRectangle control.
		/// This is fired when the user stopped dragging the window.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> 
		/// instance containing the event data.</param>
		private void headerDragRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Rectangle rect = sender as Rectangle;
			rect.ReleaseMouseCapture();
			isTrackingMouse = false;
		}

		/// <summary>
		/// Handles the MouseMove event of the headerDragRectangle control.
		/// This is fired when the user is dragging the window.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/>
		/// instance containing the event data.</param>
		private void headerDragRectangle_MouseMove(object sender, MouseEventArgs e)
		{
			if (isTrackingMouse)
			{
				Rectangle rect = sender as Rectangle;
				Point point = e.GetPosition(null);
				double x0 = this.renderTransform.X;
				double y0 = this.renderTransform.Y;
				double sign = 1;
				if (FlowDirection == System.Windows.FlowDirection.RightToLeft)
					sign = -1;
				this.renderTransform.X = x0 + (point.X - mouseOffset.X) * sign;
				this.renderTransform.Y = y0 + point.Y - mouseOffset.Y;
				mouseOffset = point;
			}
		}

		#endregion

	}
}
