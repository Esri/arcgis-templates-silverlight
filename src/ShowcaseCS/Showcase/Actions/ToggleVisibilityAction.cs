using System.Windows;
using System.Windows.Interactivity;

namespace ESRI.ArcGIS.SilverlightMapApp.Actions
{
	/// <summary>
	/// Toggles the visibility state of any UIElement.
	/// </summary>
	public class ToggleVisibilityAction : TargetedTriggerAction<UIElement>
	{
		/// <summary>
		/// Invokes the action.
		/// </summary>
		/// <param name="parameter">The parameter to the action. If the Action 
		/// does not require a parameter, the parameter may be set to a null 
		/// reference.</param>
		protected override void Invoke(object parameter)
		{
			this.Target.Visibility = this.Target.Visibility == Visibility.Visible ?
				Visibility.Collapsed : Visibility.Visible;
		}
	}
}