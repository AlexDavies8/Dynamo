using Dynamo.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Dynamo.View
{
    public class GradientTagView : ContentControl
    {
        public bool InFront
        {
            get => (bool)GetValue(InFrontProperty);
            set => SetValue(InFrontProperty, value);
        }
        public static readonly DependencyProperty InFrontProperty = DependencyProperty.Register("InFront", typeof(bool), typeof(GradientTagView), new PropertyMetadata(false));

        public readonly GradientView Owner;
        public GradientTag Model;

        public FrameworkElement Container;

        private bool _dragged;
        private float _offset;

        public GradientTagView(GradientView owner) : base()
        {
            Owner = owner;

            DataContextChanged += GradientTagDataContextChanged;
            Loaded += (sender, e) => SynchroniseProperties();
        }

        private void GradientTagDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            Model = DataContext as GradientTag;

            if (Model == null) return;

            Model.PropertyChanged += (sender, e) => SynchroniseProperties();
            SynchroniseProperties();
        }

        private void SynchroniseProperties()
        {
            if (Model == null) return;

            Canvas.SetTop(this, (Container.ActualHeight - ActualHeight) * 0.5f);
            Canvas.SetLeft(this, TimeToPosition(Model.Time));

            InFront = Owner.SelectedTag == Model;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _dragged = true;
            _offset = Model.Time - MouseToTime();

            Owner.SelectedTag = Model;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_dragged)
            {
                DragTag();
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            _dragged = false;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            _dragged = false;
        }

        private void DragTag()
        {
            Model.Time = MouseToTime(_offset);
            Owner.Model.TryUpdateTags();
        }

        float MouseToTime(float offset = 0)
        {
            float position = (float)Mouse.GetPosition(Container).X + (float)ActualWidth * 0.5f;
            position /= (float)(Container.ActualWidth);
            return Math.Min(Math.Max(position + offset, 0f), 1f);
        }

        float TimeToPosition(float time)
        {
            return time * (float)(Container.ActualWidth) - (float)ActualWidth * 0.5f;
        }
    }
}
