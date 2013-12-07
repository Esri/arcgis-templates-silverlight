using System.Windows;
using System.Windows.Interactivity;
using ESRI.ArcGIS.SilverlightMapApp;

namespace ESRI.ArcGIS.SilverlightMapApp.Actions
{
    public class ToggleExpandCollapseAction : TargetedTriggerAction<UIElement>
    {
        protected override void Invoke(object parameter)
        {
            GlassPanel panel = this.Target as GlassPanel;
            if (panel.IsOpen)
                panel.Collapse();
            else
                panel.Expand();
        }
    }
}
