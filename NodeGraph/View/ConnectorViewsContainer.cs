using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NodeGraph.View
{
    public class ConnectorViewsContainer : ItemsControl
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            FrameworkElement fe = element as FrameworkElement;

            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("/Nodegraph;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };

            Style style = resourceDictionary["DefaultConnectorStyle"] as Style;
            if (style == null)
            {
                style = Application.Current.TryFindResource("DefaultConnectorStyle") as Style;
            }
            fe.Style = style;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ConnectorView();
        }
    }
}
