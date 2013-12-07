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
using System.Windows.Controls.Primitives;

namespace ESRI.ArcGIS.SilverlightMapApp
{
	/// <summary>
	/// Panel that can be collapsed with an animation defined in the visual state.
	/// </summary>
	[TemplateVisualState(Name = "Collapsed", GroupName = "ViewStates")]
	[TemplateVisualState(Name = "Expanded", GroupName = "ViewStates")]
	public class CollapsiblePanel : ContentControl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CollapsiblePanel"/> class.
		/// </summary>
		public CollapsiblePanel()
		{
			DefaultStyleKey = typeof(CollapsiblePanel);
		}

		#region Dependency Properties

		/// <summary>
		/// Gets or sets a value indicating whether this control is expanded.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
		/// </value>
		public bool IsExpanded
		{
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="IsExpanded"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyProperty.Register("IsExpanded", typeof(bool), typeof(CollapsiblePanel), new PropertyMetadata(true, OnIsExpandedPropertyChanged));

		private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as CollapsiblePanel).ChangeVisualState(true);
		}

		#endregion

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application 
		/// code or internal processes (such as a rebuilding layout pass) call 
		/// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In 
		/// simplest terms, this means the method is called just before a UI 
		/// element displays in an application.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			ChangeVisualState(false);
		}

		/// <summary>
		/// Updates the visual state of the control.
		/// </summary>
		/// <param name="useTransitions">if set to <c>true</c> transitions will be used.</param>
		private void ChangeVisualState(bool useTransitions)
		{
			if (IsExpanded)
			{
				VisualStateManager.GoToState(this, "Expanded", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState(this, "Collapsed", useTransitions);
			}
		}
	}
}