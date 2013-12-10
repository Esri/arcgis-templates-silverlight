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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Toolkit
{
    [TemplatePart(Name = "LayoutRoot", Type = typeof(Grid))]
    [TemplatePart(Name = "PushPin", Type = typeof(Button))]
    [TemplatePart(Name = "TabPanelTop", Type = typeof(TabPanel))]
    [TemplatePart(Name = "TemplateTop", Type = typeof(FrameworkElement))]
    public class TabRibbon : TabControl
    {
        TabRibbon tabRibbon;
        Storyboard RibbonCollapse;
        Storyboard RibbonExpand;
        Grid root;
        Button PushPin;
        TabPanel tabPanel;
        FrameworkElement templateTop;

        bool isPinned = false;

        public TabRibbon()
        {
            this.DefaultStyleKey = typeof(TabRibbon);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes (such as a rebuilding layout pass) call
        /// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            tabRibbon = this;
            root = GetTemplateChild("LayoutRoot") as Grid;
            RibbonCollapse = root.Resources["RibbonCollapse"] as Storyboard;
            RibbonExpand = root.Resources["RibbonExpand"] as Storyboard;
            PushPin = GetTemplateChild("PushPin") as Button;
            tabPanel = GetTemplateChild("TabPanelTop") as TabPanel;
            templateTop = GetTemplateChild("TemplateTop") as FrameworkElement;
            if (templateTop != null)
                templateTop.MouseLeftButtonUp += templateTop_MouseLeftButtonUp;
            if (RibbonExpand != null)
                RibbonExpand.Completed += RibbonExpand_Completed;
            if (PushPin != null)
                PushPin.Click += PushPin_Click;
            
        }

        void templateTop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isPinned)
                SetPinned(true);
        }


         private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isPinned)
            {
                if (RibbonExpand == null)
                    RibbonExpand = root.Resources["RibbonExpand"] as Storyboard;
                RibbonExpand.Begin();
                
            }
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!isPinned)
            {
                if (RibbonExpand == null)
                {
                    RibbonCollapse = root.Resources["RibbonCollapse"] as Storyboard;
                }
                RibbonCollapse.Begin();
            }
        }

        private void PushPin_Click(object sender, RoutedEventArgs e)
        {
            isPinned = !isPinned;
            SetPinned(isPinned);
        }

        private void SetPinned(bool pinned)
        {
            isPinned = pinned;
            if (pinned)
            {
                VisualStateManager.GoToState(PushPin, "Expanded", true);
                if (RibbonExpand == null)
                {
                    RibbonExpand = root.Resources["RibbonExpand"] as Storyboard;
                }
                PushPin.SetValue(ToolTipService.ToolTipProperty, "Minimize Menu");
                RibbonExpand.Begin();
            }
            else
            {
                VisualStateManager.GoToState(PushPin, "Closed", true);
                if (RibbonExpand == null)
                {
                    RibbonCollapse = root.Resources["RibbonCollapse"] as Storyboard;
                }
                PushPin.SetValue(ToolTipService.ToolTipProperty, "Expand Menu");
                RibbonCollapse.Begin();
            }
        }

        private void RibbonExpand_Completed(object sender, EventArgs e)
        {
            //PushPin.Visibility = Visibility.Visible;
        }

    }
}
