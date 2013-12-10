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
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.SilverlightMapApp;

namespace Minimal
{
    public partial class MainPage : UserControl
    {
        bool usePlaneProjection = false;
        double marginLRFactor = 0.25;
        double marginTBFactor = 0.5;

        public MainPage()
        {
            InitializeComponent();
            string strvalue = Application.Current.Resources["UsePlaneProjection"] as string;
            if (strvalue != null)
                usePlaneProjection = bool.Parse(strvalue);
            if (usePlaneProjection)
            {
                strvalue = Application.Current.Resources["MapLeftRightMarginFactor"] as string;
                if (strvalue != null)
                    marginLRFactor = double.Parse(strvalue);
                strvalue = Application.Current.Resources["MapTopBottomMarginFactor"] as string;
                if (strvalue != null)
                    marginTBFactor = double.Parse(strvalue);
            }
        }

        private void nav_Loaded(object sender, RoutedEventArgs e)
        {
            nav.MapProjection = mapPlaneProjection;

        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (usePlaneProjection)
            {
                double mwidth = MapBorder.ActualWidth * marginLRFactor * -1;
                double mheight = MapBorder.ActualHeight * marginTBFactor * -1;
                Map.Margin = new Thickness(mwidth, mheight, mwidth, mheight);
            }
        }

    }
}
