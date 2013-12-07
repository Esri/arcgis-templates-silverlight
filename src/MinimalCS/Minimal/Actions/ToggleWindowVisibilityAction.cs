using System.Windows.Interactivity;

namespace ESRI.ArcGIS.SilverlightMapApp.Actions
{
    public class ToggleWindowVisibilityAction : TargetedTriggerAction<DraggableWindow>
    {
        protected override void Invoke(object parameter)
        {
            this.Target.IsOpen = !this.Target.IsOpen;
        }
    }
}
