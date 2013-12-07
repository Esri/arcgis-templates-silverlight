using System.Windows.Interactivity;

namespace ESRI.ArcGIS.SilverlightMapApp.Actions
{
	/// <summary>
	/// Toggles the IsOpen property of the <see cref="DraggableWindow"/>.
	/// </summary>
	public class ToggleWindowVisibilityAction : TargetedTriggerAction<DraggableWindow>
	{
		/// <summary>
		/// Invokes the action.
		/// </summary>
		/// <param name="parameter">The parameter to the action. If the Action 
		/// does not require a parameter, the parameter may be set to a null 
		/// reference.</param>
		protected override void Invoke(object parameter)
		{
			this.Target.IsOpen = !this.Target.IsOpen;
		}
	}
}
