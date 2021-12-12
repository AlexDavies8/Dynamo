using Dynamo.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Dynamo.View
{
    public class PropertyPanelView : ContentControl
    {
        #region Properties

        public PropertyPanelViewModel ViewModel { get; private set; }

        #endregion

        #region Constructor

        public PropertyPanelView()
        {
            DataContextChanged += ViewportPanelDataContextChanged;
            Loaded += ViewportPanelViewLoaded;
            Unloaded += ViewportPanelViewUnloaded;
        }

        #endregion

        #region Templating

        static PropertyPanelView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyPanelView), new FrameworkPropertyMetadata(typeof(PropertyPanelView)));
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
            ViewModel = DataContext as PropertyPanelViewModel;
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
        }

        #endregion
    }
}
