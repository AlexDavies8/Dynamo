using Dynamo.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ImageSharp.WpfImageSource;
using System.Windows.Media;
using System.Reflection;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace Dynamo.View
{
    public class ViewportPanelView : ContentControl
    {
        #region Properties

        public ViewportPanelViewModel ViewModel { get; private set; }

        private ImageSource _displayedImage
        {
            get => (ImageSource)GetValue(DisplayedImageProperty);
            set => SetValue(DisplayedImageProperty, value);
        }
        public static readonly DependencyProperty DisplayedImageProperty = DependencyProperty.Register("DisplayedImage", typeof(ImageSource), typeof(ViewportPanelView), new PropertyMetadata(null));

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

            var port = ViewModel.Model.DisplayedPort;
            if (port != null)
            {
                _displayedImage = new ImageSharpImageSource<Rgba32>(port.Value as Image<Rgba32>);
            }
        }

        #endregion
    }
}
