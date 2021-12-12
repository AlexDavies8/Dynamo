using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WindowDocker.ViewModel;

namespace WindowDocker.View
{
    public class SpanView : ContentControl
    {
        #region Properties

        public SpanViewModel ViewModel { get; private set; }

        public ObservableCollection<SpanView> SpanViews
        {
            get => (ObservableCollection<SpanView>)GetValue(SpanViewsProperty);
            set => SetValue(SpanViewsProperty, value);
        }
        public static readonly DependencyProperty SpanViewsProperty = DependencyProperty.Register("SpanViews", typeof(ObservableCollection<SpanView>), typeof(SpanView), new PropertyMetadata(new ObservableCollection<SpanView>()));

        #endregion

        #region Constructor

        public SpanView() : base()
        {
            DataContextChanged += SpanViewDataContextChanged;
        }

        #endregion

        #region Templating

        static SpanView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpanView), new FrameworkPropertyMetadata(typeof(SpanView)));
        }

        #endregion

        #region Events

        private void SpanViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = (SpanViewModel)DataContext;
            ViewModel.View = this;
            ViewModel.PropertyChanged += ViewModelPropertyChanged;

            SynchronizeProperties();
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SynchronizeProperties();
        }

        private void SynchronizeProperties()
        {
            if (ViewModel == null)
            {
                return;
            }

            SpanViews.Clear();
            foreach (var span in ViewModel.Model.Children)
            {
                SpanViews.Add(span.ViewModel.View);
            }
        }

        #endregion
    }
}
