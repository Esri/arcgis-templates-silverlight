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

namespace Toolkit
{
    [TemplatePart(Name = "LayoutRoot", Type = typeof(Grid))]
    [TemplatePart(Name = "PushPin", Type = typeof(Button))]
    public class TabRibbon : TabControl
    {
        TabControl tabControl;
        TabRibbon tabRibbon;
        ItemCollection items;
        Storyboard RibbonCollapse;
        Storyboard RibbonExpand;
        Grid root;
        Button PushPin;

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
            if (root != null)
            {
                root.MouseEnter += LayoutRoot_MouseEnter;
                root.MouseLeave += LayoutRoot_MouseLeave;
            }
            if (RibbonExpand != null)
                RibbonExpand.Completed += RibbonExpand_Completed;
            if (PushPin != null)
                PushPin.Click += PushPin_Click;
            
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
                PushPin.Visibility = Visibility.Collapsed;
                if (RibbonExpand == null)
                {
                    RibbonCollapse = root.Resources["RibbonCollapse"] as Storyboard;
                    RibbonExpand.Completed += RibbonExpand_Completed;
                }
                RibbonCollapse.Begin();
            }
        }

        private void PushPin_Click(object sender, RoutedEventArgs e)
        {
            isPinned = !isPinned;
            if (isPinned)
            {
                VisualStateManager.GoToState(PushPin, "Pinned", true);
            }
            else
            {
                VisualStateManager.GoToState(PushPin, "UnPinned", true);
            }
        }

        private void RibbonExpand_Completed(object sender, EventArgs e)
        {
            PushPin.Visibility = Visibility.Visible;
        }

    }
}
