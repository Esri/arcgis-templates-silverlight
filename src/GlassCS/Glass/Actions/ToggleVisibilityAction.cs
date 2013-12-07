using System.Windows;
using System.Windows.Interactivity;

namespace ESRI.ArcGIS.SilverlightMapApp.Actions
{
    public class ToggleVisibilityAction : TargetedTriggerAction<UIElement>
    {
        protected override void Invoke(object parameter)
        {
            this.Target.Visibility = this.Target.Visibility == Visibility.Visible ?
                Visibility.Collapsed : Visibility.Visible;
        }
    }
}