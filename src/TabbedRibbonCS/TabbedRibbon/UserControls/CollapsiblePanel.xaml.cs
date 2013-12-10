/*
Copyright 2013 Esri
Licensed under the Apache License, Version 2.0 (the "License");
You may not use this file except in compliance with the License.
You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
