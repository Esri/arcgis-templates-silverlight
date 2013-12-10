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

namespace ESRI.ArcGIS.SilverlightMapApp
{
    public class WindowPanel : ContentControl
    {
        internal bool IsRolledUp = true;
        private bool isTrackingMouse = false;
        private Point mouseOffset;
        UIElement WidgetContent;
        TranslateTransform renderTransform;

        public WindowPanel()
        {
            DefaultStyleKey = typeof(WindowPanel);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            WidgetContent = GetTemplateChild("WidgetContent") as UIElement;
            this.RenderTransform = renderTransform = new TranslateTransform();

            UIElement headerDragRectangle = GetTemplateChild("headerDragRectangle") as UIElement;
            UIElement imgClose = GetTemplateChild("imgClose") as UIElement;
            if (headerDragRectangle != null)
            {
                headerDragRectangle.MouseLeftButtonDown += headerDragRectangle_MouseLeftButtonDown;
                headerDragRectangle.MouseLeftButtonUp += headerDragRectangle_MouseLeftButtonUp;
                headerDragRectangle.MouseMove += headerDragRectangle_MouseMove;
            }
            if (imgClose != null)
                imgClose.MouseLeftButtonDown += new MouseButtonEventHandler(imgClose_MouseLeftButtonDown);
        }
        void imgClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        { this.Visibility = Visibility.Collapsed; }

        public bool IsVisible { get { return this.Visibility == Visibility.Visible; } set { if (value == true) Show(); else Hide(); } }
        public bool IsExpanded { get { return WidgetContent.Visibility == Visibility.Visible; } set { if (value == true) WidgetContent.Visibility = Visibility.Visible; else WidgetContent.Visibility = Visibility.Collapsed; } }

        public void Show()
        { VisualStateManager.GoToState(this, "Opened", true); }
        public void Hide()
        { VisualStateManager.GoToState(this, "Closed", true); }
        public void Toggle() { if (IsVisible) Hide(); else Show(); }


        void headerDragRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            mouseOffset = e.GetPosition(null);
            rect.CaptureMouse();
            isTrackingMouse = true;
        }
        void headerDragRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            rect.ReleaseMouseCapture();
            isTrackingMouse = false;
        }
        void headerDragRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (isTrackingMouse)
            {
                Rectangle rect = sender as Rectangle;
                Point point = e.GetPosition(null);
                double x0 = this.renderTransform.X;
                double y0 = this.renderTransform.Y;
                this.renderTransform.X = x0 + point.X - mouseOffset.X;
                this.renderTransform.Y = y0 + point.Y - mouseOffset.Y;
                mouseOffset = point;
            }
        }

        public object ContentTitle
        {
            get { return (object)GetValue(ContentTitleProperty); }
            set { SetValue(ContentTitleProperty, value); }
        }
        public static readonly DependencyProperty ContentTitleProperty =
            DependencyProperty.Register("ContentTitle", typeof(object), typeof(WindowPanel), null);
    }
}
