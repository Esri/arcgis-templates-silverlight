using System.Windows;
using System.Windows.Interactivity;

namespace ESRI.ArcGIS.SilverlightMapApp.Actions
{
	/// <summary>
	/// Toggles collapse state of the <see cref="CollapsiblePanel"/>
	/// </summary>
	public class ToggleCollapseAction : TargetedTriggerAction<CollapsiblePanel>
	{
		/// <summary>
		/// Invokes the action.
		/// </summary>
		/// <param name="parameter">The parameter to the action. If the Action 
		/// does not require a parameter, the parameter may be set to a null 
		/// reference.</param>
		protected override void Invoke(object parameter)
		{
			this.Target.IsExpanded = !this.Target.IsExpanded;
		}
	}
}
