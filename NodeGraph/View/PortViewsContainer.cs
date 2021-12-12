﻿using NodeGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NodeGraph.View
{
    public class PortViewsContainer : ItemsControl
    {
        #region Properties
        public bool ShowPort
        {
            get { return (bool)GetValue(ShowPortProperty); }
            set { SetValue(ShowPortProperty, value); }
        }
        public static readonly DependencyProperty ShowPortProperty =
            DependencyProperty.Register("ShowPort", typeof(bool), typeof(PortViewsContainer), new PropertyMetadata(true));

        public bool IsInput
        {
            get { return (bool)GetValue(IsInputProperty); }
            set { SetValue(IsInputProperty, value); }
        }
        public static readonly DependencyProperty IsInputProperty =
            DependencyProperty.Register("IsInput", typeof(bool), typeof(PortViewsContainer), new PropertyMetadata(false));

        #endregion

        #region Overrides

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            FrameworkElement fe = element as FrameworkElement;

            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("/NodeGraph;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };

            Style style = resourceDictionary["DefaultPortStyle"] as Style;
            if (style == null)
                style = Application.Current.TryFindResource("DefaultPortStyle") as Style;
            fe.Style = style;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            PortView portView = new PortView(IsInput);
            portView.Interactable = ShowPort;
            return portView;
        }

        #endregion
    }
}
