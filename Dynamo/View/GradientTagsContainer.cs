using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Dynamo.View
{
    public class GradientTagsContainer : ItemsControl
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            FrameworkElement fe = element as FrameworkElement;

            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("/Dynamo;component/Themes/Default.xaml", UriKind.RelativeOrAbsolute)
            };

            Style style = resourceDictionary["GradientTagStyle"] as Style;
            if (style == null)
                style = Application.Current.TryFindResource("GradientTagStyle") as Style;
            fe.Style = style;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            GradientTagView tagView = new GradientTagView();
            return tagView;
        }
    }
}
