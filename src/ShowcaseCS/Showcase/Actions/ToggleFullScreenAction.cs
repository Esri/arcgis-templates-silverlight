using System.Windows;
using System.Windows.Interactivity;

namespace ESRI.ArcGIS.SilverlightMapApp.Actions
{
	/// <summary>
	/// Toggles the full screen state of the Silverlight Application
	/// </summary>
	public class ToggleFullScreenAction : TriggerAction<UIElement>
	{
		/// <summary>
		/// Invokes the action.
		/// </summary>
		/// <param name="parameter">The parameter to the action. If the Action 
		/// does not require a parameter, the parameter may be set to a null 
		/// reference.</param>
		protected override void Invoke(object parameter)
		{
			Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
		}
	}
}
