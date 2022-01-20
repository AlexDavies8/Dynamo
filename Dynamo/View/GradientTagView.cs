using Dynamo.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Dynamo.View
{
    public class GradientTagView : ContentControl
    {
        public GradientTag Model;

        public GradientTagView() : base()
        {
            DataContextChanged += GradientTagDataContextChanged;
        }

        private void GradientTagDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            Model = DataContext as GradientTag;
        }
    }
}
