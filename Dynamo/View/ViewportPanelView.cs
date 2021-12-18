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
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace Dynamo.View
{
    public class ViewportPanelView : ContentControl
    {
        #region Properties

        public ViewportPanelViewModel ViewModel { get; private set; }

        private ImageSource DisplayedImage
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
                Debug.WriteLine("Updating ImageSource");

                var image = port.Value as Image<Rgba32>;

                var bmp = new WriteableBitmap(image.Width, image.Height, image.Metadata.HorizontalResolution, image.Metadata.VerticalResolution, PixelFormats.Bgra32, null);

                bmp.Lock();
                try
                {
                    var backBuffer = bmp.BackBuffer;

                    for (var y = 0; y < image.Height; y++)
                    {
                        var buffer = image.GetPixelRowSpan(y);
                        for (var x = 0; x < image.Width; x++)
                        {
                            var backBufferPos = backBuffer + (y * image.Width + x) * 4;
                            var rgba = buffer[x];
                            var color = rgba.A << 24 | rgba.R << 16 | rgba.G << 8 | rgba.B;

                            System.Runtime.InteropServices.Marshal.WriteInt32(backBufferPos, color);
                        }
                    }

                    bmp.AddDirtyRect(new Int32Rect(0, 0, image.Width, image.Height));
                }
                finally
                {
                    bmp.Unlock();
                    DisplayedImage = bmp;
                }
            }
        }

        #endregion
    }
}
