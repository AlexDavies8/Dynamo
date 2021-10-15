using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NodeGraph.View
{
    class NodeViewsContainer : ItemsControl
    {
        #region Overrides

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

			FrameworkElement fe = element as FrameworkElement;

			ResourceDictionary resourceDictionary = new ResourceDictionary
			{
				Source = new Uri("/NodeGraph;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
			};

			Style style = resourceDictionary["DefaultNodeStyle"] as Style;
			if (style == null)
			{
				style = Application.Current.TryFindResource("DefaultNodeStyle") as Style;
			}
			fe.Style = style;
		}

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new NodeView();
        }

        #endregion
    }
}
