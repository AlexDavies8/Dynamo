using NodeGraph.Model;
using NodeGraph.ViewModel;
using PropertyTools.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace NodeGraph.View
{
    [TemplatePart( Name="PART_Header", Type = typeof(TextBlock))] // TODO: Implement Editable Text Block
    public class NodeView : ContentControl
    {
        #region Constants

        const double TargetSelectionThickness = 2.0;
        const double TargetCornerRadius = 4.0;

        const int DoubleClickTime = 500;

        #endregion

        #region Fields

        private EditableTextBlock _partHeader;
        private DispatcherTimer _clickTimer = new DispatcherTimer();
        private int _clickCount = 0;

        #endregion

        #region Properties

        public NodeViewModel ViewModel { get; private set; }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(NodeView), new PropertyMetadata(false));

        public bool HasConnection
        {
            get => (bool)GetValue(HasConnectionProperty);
            set => SetValue(HasConnectionProperty, value);
        }
        public static readonly DependencyProperty HasConnectionProperty = DependencyProperty.Register("HasConnection", typeof(bool), typeof(NodeView), new PropertyMetadata(false));

        public Thickness SelectionThickness
        {
            get => (Thickness)GetValue(SelectionThicknessProperty);
            set => SetValue(SelectionThicknessProperty, value);
        }
        public static readonly DependencyProperty SelectionThicknessProperty = DependencyProperty.Register("SelectionThickness", typeof(Thickness), typeof(NodeView), new PropertyMetadata(new Thickness(TargetSelectionThickness)));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(NodeView), new PropertyMetadata(new CornerRadius(TargetCornerRadius)));

        #endregion

        #region Constructors

        public NodeView()
        {
            DataContextChanged += NodeViewDataContextChanged;
            Loaded += NodeViewLoaded;
            Unloaded += NodeViewUnloaded;
        }

        #endregion

        #region Events

        private void NodeViewLoaded(object sender, RoutedEventArgs e)
        {
            SynchronizeProperties();
            OnCanvasRenderTransformChanged();

            _clickTimer.Interval = TimeSpan.FromMilliseconds(DoubleClickTime);
            _clickTimer.Tick += (sender, e) => ResetClickTimer();
        }

        private void NodeViewUnloaded(object sender, RoutedEventArgs e)
        {

        }

        private void NodeViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = DataContext as NodeViewModel;
            ViewModel.View = this;
            ViewModel.PropertyChanged += ViewModelPropertyChanged;

            SynchronizeProperties();
        }

        private void SynchronizeProperties()
        {
            if (ViewModel == null)
            {
                return;
            }

            IsSelected = ViewModel.IsSelected;
            HasConnection = (ViewModel.InputPortViewModels.Count > 0) || (ViewModel.OutputPortViewModels.Count > 0);
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SynchronizeProperties();
        }

        #endregion

        #region Template Events

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _partHeader = Template.FindName("PART_Header", this) as EditableTextBlock; // TODO: Implement Editable Text Block
            if (_partHeader != null)
            {
                _partHeader.MouseDown += HeaderMouseDown;
            }
        }

        private void ResetClickTimer()
        {
            _clickCount = 0;
            _clickTimer.Stop();
        }

        private void HeaderMouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(_partHeader);

            if (_clickCount == 0)
            {
                _clickCount++;
                _clickTimer.Start();
            }
            else if(_clickCount == 1)
            {
                _partHeader.IsEditing = true;
                Keyboard.Focus(_partHeader);
                ResetClickTimer();

                e.Handled = true;
            }
        }

        #endregion

        #region Mouse Events

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            Flowchart flowchart = ViewModel.Model.Owner;

            if (NodeGraphManager.IsConnecting)
            {
                // TODO: Implement History
                NodeGraphManager.EndConnection();
            }

            NodeGraphManager.EndSelection();
            NodeGraphManager.EndDragNode();

            e.Handled = true;
        }

        private Point _draggingStartPosition;
        private Matrix _zoomAndPanStartMatrix;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            Flowchart flowchart = ViewModel.Model.Owner;
            FlowchartView flowchartView = flowchart.ViewModel.View;
            Keyboard.Focus(flowchartView);

            NodeGraphManager.EndConnection();
            NodeGraphManager.EndDragNode();
            // TODO: End Selection

            NodeGraphManager.BeginDragNode(flowchart);

            if (NodeGraphManager.IsNodeDragged && !IsSelected)
            {
                NodeGraphManager.TrySelection(flowchart, ViewModel.Model);
            }

            Node node = ViewModel.Model;
            _draggingStartPosition = new Point(node.X, node.Y);

            // TODO: Implement History
            _zoomAndPanStartMatrix = flowchartView.ZoomAndPan.Matrix;

            e.Handled = true;
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            if (NodeGraphManager.IsNodeDragged)
            {
                Flowchart flowchart = ViewModel.Model.Owner;

                Node node = ViewModel.Model;
                Point delta = new Point(node.X - _draggingStartPosition.X, node.Y - _draggingStartPosition.Y);

                if ((int)delta.X != 0 && (int)delta.Y != 0)
                {
                    ObservableCollection<Guid> selectionList = NodeGraphManager.GetSelectedNodeGuids(node.Owner);
                    foreach (var guid in selectionList)
                    {
                        Node currentNode = NodeGraphManager.FindNode(guid);

                        // TODO: Implement History
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (NodeGraphManager.IsNodeDragged && !IsSelected)
            {
                Node node = ViewModel.Model;
                Flowchart flowchart = node.Owner;
                NodeGraphManager.TrySelection(flowchart, node);
            }
        }

        #endregion

        #region Render Transform

        public void OnCanvasRenderTransformChanged()
        {
            Matrix matrix = (VisualParent as Canvas).RenderTransform.Value;
            double scale = matrix.M11;

            SelectionThickness = new Thickness(TargetSelectionThickness / scale);

            //CornerRadius = new CornerRadius(TargetCornerRadius);
        }

        #endregion
    }
}
