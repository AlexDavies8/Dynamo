using Dynamo.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Reflection;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using Dynamo.Model;

namespace Dynamo.View
{
    public class ViewportPanelView : ContentControl
    {
        #region Properties

        public ViewportPanelViewModel ViewModel { get; private set; }
        #endregion

        #region Constructor

        public ViewportPanelView()
        {
            DataContextChanged += ViewportPanelDataContextChanged;
            Loaded += ViewportPanelViewLoaded;
            Unloaded += ViewportPanelViewUnloaded;
        }

        #endregion

        #region Events

        private void ViewportPanelViewLoaded(object sender, RoutedEventArgs e)
        {
            SynchronizeProperties();
        }

        private void ViewportPanelViewUnloaded(object sender, RoutedEventArgs e)
        {

        }

        private void ViewportPanelDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = DataContext as ViewportPanelViewModel;
            ViewModel.View = this;
            ViewModel.PropertyChanged += ViewModelPropertyChanged;

            SynchronizeProperties();
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SynchronizeProperties();
        }

        private void SynchronizeProperties()
        {
            if (ViewModel == null)
                return;

            var displayedNode = ViewModel.Model.DisplayedNode;
            if (displayedNode != null && displayedNode is ExecutableNode node)
            {
                if (node.PreviewImage != null)
                    ViewModel.Model.DisplayedImage = node.PreviewImage;
            }
        }

        #endregion
    }
}
