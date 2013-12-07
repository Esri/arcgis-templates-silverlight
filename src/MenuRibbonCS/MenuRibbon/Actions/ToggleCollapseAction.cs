using System.Windows;
using System.Windows.Interactivity;

namespace ESRI.ArcGIS.SilverlightMapApp.Actions
{
    public class ToggleCollapseAction : TargetedTriggerAction<CollapsiblePanel>
    {
        protected override void Invoke(object parameter)
        {
            this.Target.IsExpanded = !this.Target.IsExpanded;
        }
    }
}
