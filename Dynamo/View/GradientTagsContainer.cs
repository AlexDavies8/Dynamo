using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Dynamo.View
{
    public class GradientTagsContainer : ItemsControl
    {
        public GradientView Owner
        {
            get => (GradientView)GetValue(OwnerProperty);
            set => SetValue(OwnerProperty, value);
        }
        public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof(GradientView), typeof(GradientTagsContainer), new PropertyMetadata(null));

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
            GradientTagView tagView = new GradientTagView(Owner);
            tagView.Container = this;
            return tagView;
        }
    }
}
