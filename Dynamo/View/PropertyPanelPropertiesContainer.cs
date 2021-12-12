using NodeGraph.View;
using NodeGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Dynamo.View
{
    public class PropertyPanelPropertiesContainer : ItemsControl
    {
        #region Properties
        public bool ShowPort
        {
            get { return (bool)GetValue(ShowPortProperty); }
            set { SetValue(ShowPortProperty, value); }
        }
        public static readonly DependencyProperty ShowPortProperty =
            DependencyProperty.Register("ShowPort", typeof(bool), typeof(PropertyPanelPropertiesContainer), new PropertyMetadata(true));

        public bool IsInput
        {
            get { return (bool)GetValue(IsInputProperty); }
            set { SetValue(IsInputProperty, value); }
        }
        public static readonly DependencyProperty IsInputProperty =
            DependencyProperty.Register("IsInput", typeof(bool), typeof(PropertyPanelPropertiesContainer), new PropertyMetadata(false));

        #endregion

        #region Overrides

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            FrameworkElement fe = element as FrameworkElement;

            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("/Dynamo;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };

            Style style = resourceDictionary["PropertyStyle"] as Style;
            if (style == null)
                style = Application.Current.TryFindResource("PropertyStyle") as Style;
            fe.Style = style;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            PortView portView = new PortView(IsInput);
            portView.LinkToViewModel = false;
            portView.Interactable = ShowPort;
            return portView;
        }

        #endregion
    }
}
