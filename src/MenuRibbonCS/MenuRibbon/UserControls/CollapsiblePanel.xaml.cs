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
    [TemplateVisualState(Name = "Collapsed", GroupName = "ViewStates")]
    [TemplateVisualState(Name = "Expanded", GroupName = "ViewStates")]
    public class CollapsiblePanel : ContentControl
    {
        public CollapsiblePanel()
        {
            DefaultStyleKey = typeof(CollapsiblePanel);
        }

        #region Dependency Properties

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(CollapsiblePanel), new PropertyMetadata(true, OnIsExpandedPropertyChanged));

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CollapsiblePanel).ChangeVisualState(true);
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ChangeVisualState(false);
        }

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