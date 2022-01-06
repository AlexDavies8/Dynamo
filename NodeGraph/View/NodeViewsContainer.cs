using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using NodeGraph.Model;
using NodeGraph.ViewModel;

namespace NodeGraph.View
{
    public class NodeViewsContainer : ItemsControl
    {
        #region Overrides

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var attributes = ((NodeViewModel)item).Model.GetType().GetCustomAttributes(typeof(OverrideStyleAttribute), true);

            FrameworkElement fe = element as FrameworkElement;

			ResourceDictionary resourceDictionary = new ResourceDictionary
			{
				Source = new Uri("/NodeGraph;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
			};

            string styleName = attributes.Length > 0 ? ((OverrideStyleAttribute)attributes[0]).StyleName : "DefaultNodeStyle";
			Style style = resourceDictionary[styleName] as Style;
			if (style == null)
			{
				style = Application.Current.TryFindResource(styleName) as Style;
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
