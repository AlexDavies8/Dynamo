﻿using NodeGraph.Model;
using NodeGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NodeGraph.View
{
    public class FlowchartView : ContentControl
    {
        #region Fields

        #endregion

        #region Properties

        public FlowchartViewModel ViewModel { get; private set; }

		public ZoomAndPan ZoomAndPan { get; private set; } = new ZoomAndPan();

        public FrameworkElement NodeCanvas { get; private set; }

        public FrameworkElement ConnectorCanvas { get; private set; }

        public FrameworkElement PartConnectorViewsContainer { get; private set; }

        public FrameworkElement PartNodeViewsContainer { get; private set; }

        public FrameworkElement PartDragAndSelectionCanvas { get; private set; }

        #endregion

        #region Constructors

        static FlowchartView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlowchartView), new FrameworkPropertyMetadata(typeof(FlowchartView)));
        }

        public FlowchartView()
        {
            Focusable = true;

			DataContextChanged += FlowchartViewDataContextChanged;

			SizeChanged += FlowchartViewSizeChanged;

            AllowDrop = true;

			// TODO: Drag Events
        }

        #endregion

        #region Events

        private void FlowchartViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
			ZoomAndPan.ViewWidth = ActualWidth;
			ZoomAndPan.ViewHeight = ActualHeight;
		}

        private void FlowchartViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = DataContext as FlowchartViewModel;
            if (null == ViewModel)
                return;

            ViewModel.View = this;

            if (ConnectorCanvas == null)
            {
                ConnectorCanvas = ViewUtility.FindChild<Canvas>(PartNodeViewsContainer);
            }

            if (NodeCanvas == null)
            {
                NodeCanvas = ViewUtility.FindChild<Canvas>(PartNodeViewsContainer);
            }

			ZoomAndPan.UpdateTransform += ZoomAndPanUpdateTransform;
		}

		private void ZoomAndPanUpdateTransform()
		{
			NodeCanvas.RenderTransform = new MatrixTransform(ZoomAndPan.Matrix);

			foreach (var pair in NodeGraphManager.Nodes)
			{
				NodeView nodeView = pair.Value.ViewModel.View;
				nodeView.OnCanvasRenderTransformChanged();
			}
		}

		#endregion

		#region Mouse Events

		private bool _isDraggingCanvas;
		private Matrix _zoomAndPanStartMatrix;
		private Point _prevMousePosition;

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

			if (ViewModel == null)
				return;

			Keyboard.Focus(this);

			_zoomAndPanStartMatrix = ZoomAndPan.Matrix;

			Point mousePosition = e.GetPosition(this);
			_prevMousePosition = mousePosition;

			if (!NodeGraphManager.IsNodeDragged && !NodeGraphManager.IsConnecting && !NodeGraphManager.IsSelecting)
            {
				NodeGraphManager.BeginSelection(ViewModel.Model, ZoomAndPan.MatrixInv.Transform(mousePosition));
				ViewModel.SelectionStartX = mousePosition.X;
				ViewModel.SelectionStartY = mousePosition.Y;
				ViewModel.SelectionWidth = 0;
				ViewModel.SelectionHeight = 0;

				// TODO: Add Selection Modes
				NodeGraphManager.DeselectAllNodes(ViewModel.Model);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

			if (ViewModel == null)
				return;

			Flowchart flowchart = ViewModel.Model;

			NodeGraphManager.EndConnection();
			NodeGraphManager.EndDragNode();

			// TODO: Implement History
        }


        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

			if (ViewModel == null)
				return;

			Keyboard.Focus(this);
			_zoomAndPanStartMatrix = ZoomAndPan.Matrix;

			if (!NodeGraphManager.IsDragging)
            {
				_isDraggingCanvas = true;

				Mouse.Capture(this, CaptureMode.SubTree);
            }
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

			if (ViewModel == null)
				return;

			NodeGraphManager.EndConnection();
			NodeGraphManager.EndDragNode();
			NodeGraphManager.EndDragging();

			if (_isDraggingCanvas)
            {
				_isDraggingCanvas = false;
				Mouse.Capture(null);

				// TODO: Implement History
            }

			// TODO: Context Menu Logic
        }

		private void UpdateDragging(Point mousePosition, Point delta)
        {
			if (NodeGraphManager.IsConnecting)
            {
				NodeGraphManager.UpdateConnection(mousePosition);
            }
			else if (NodeGraphManager.IsNodeDragged)
            {
				double invScale = 1.0f / ZoomAndPan.Scale;
				NodeGraphManager.DragNode(new Point(delta.X * invScale, delta.Y * invScale));
            }
			else if (NodeGraphManager.IsSelecting)
            {
				NodeGraphManager.UpdateSelection(ViewModel.Model, ZoomAndPan.MatrixInv.Transform(mousePosition));

				Point startPosition = ZoomAndPan.Matrix.Transform(NodeGraphManager.SelectStartPosition);

				Point selectionMin = new Point(
					Math.Min(startPosition.X, mousePosition.X),
					Math.Min(startPosition.Y, mousePosition.Y)
				);
				Point selectionMax = new Point(
					Math.Max(startPosition.X, mousePosition.X),
					Math.Max(startPosition.Y, mousePosition.Y)
				);

				ViewModel.SelectionStartX = selectionMin.X;
				ViewModel.SelectionStartY = selectionMin.Y;
				ViewModel.SelectionWidth = selectionMax.X - selectionMin.X;
				ViewModel.SelectionHeight = selectionMax.Y - selectionMin.Y;
			}
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

			if (ViewModel == null)
				return;

			Point mousePosition = e.GetPosition(this);

			MouseArea area = CheckMouseArea();

			Point delta = new Point(
				mousePosition.X - _prevMousePosition.X,
				mousePosition.Y - _prevMousePosition.Y
			);

			if (NodeGraphManager.IsDragging)
            {
				UpdateDragging(mousePosition, delta);
            }
			else
            {
				if (_isDraggingCanvas)
                {
					ZoomAndPan.StartX -= delta.X;
					ZoomAndPan.StartY -= delta.Y;
                }
            }

			_prevMousePosition = mousePosition;

			e.Handled = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

			if (ViewModel == null)
				return;

			NodeGraphManager.EndConnection();
			NodeGraphManager.EndDragNode();
			NodeGraphManager.EndDragging();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

			if (ViewModel == null)
				return;

			NodeGraphManager.EndConnection();
			NodeGraphManager.EndDragNode();
			NodeGraphManager.EndDragging();

			if (_isDraggingCanvas)
            {
				_isDraggingCanvas = false;
				Mouse.Capture(null);

				// TODO: Implement History
            }
		}

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

			if (ViewModel == null)
				return;

			// TODO: Implement History

			double newScale = ZoomAndPan.Scale;
			newScale += (0.0 > e.Delta) ? -0.05 : 0.05;
			newScale = Math.Max(0.1, Math.Min(1.0, newScale));

			Point mousePosition = e.GetPosition(this);
			Point zoomCentre = ZoomAndPan.MatrixInv.Transform(mousePosition);

			ZoomAndPan.Scale = newScale;
			
			Point newZoomCentre = ZoomAndPan.MatrixInv.Transform(zoomCentre);
			Point zoomDelta = new Point(zoomCentre.X - newZoomCentre.X, zoomCentre.Y - newZoomCentre.Y);

			ZoomAndPan.StartX -= zoomDelta.X;
			ZoomAndPan.StartY -= zoomDelta.Y;
		}

        #endregion

        #region Keyboard Events

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

			if (ViewModel == null)
				return;

			if (IsFocused)
            {
				if (e.Key == Key.Delete)
                {
					NodeGraphManager.DestroySelectedNodes(ViewModel.Model);
                }
				else if (e.Key == Key.Escape)
                {
					NodeGraphManager.DeselectAllNodes(ViewModel.Model);
                }
				else if (e.Key == Key.A)
                {
					if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
						NodeGraphManager.SelectAllNodes(ViewModel.Model);
                    }
                }
				else if (e.Key == Key.F)
                {
					FitNodesToView();
                }					
				// TODO: Implement History
            }
        }

        #endregion

        #region Area

        [Flags]
		public enum MouseArea
		{
			None,
			Left,
			Right,
			Top,
			Bottom,
		}

		public MouseArea CheckMouseArea()
		{
			Point absPosition = Mouse.GetPosition(this);
			Point absTopLeft = new Point(0.0, 0.0);
			Point absBottomRight = new Point(ActualWidth, ActualHeight);

			MouseArea area = MouseArea.None;

			if (absPosition.X < (absTopLeft.X + 4.0))
				area |= MouseArea.Left;
			if (absPosition.X > (absBottomRight.X - 4.0))
				area |= MouseArea.Right;
			if (absPosition.Y < (absTopLeft.Y + 4.0))
				area |= MouseArea.Top;
			if (absPosition.Y > (absBottomRight.Y - 4.0))
				area |= MouseArea.Bottom;

			return area;
		}

        #endregion // Area

        #region Fitting

		public void FitNodesToView()
        {
			(double minX, double maxX, double minY, double maxY) = NodeGraphManager.CalculateContentBounds(ViewModel.Model);
			if (minX == maxX || minY == maxY)
            {
				return;
            }

			Flowchart flowchart = ViewModel.Model;

			_zoomAndPanStartMatrix = ZoomAndPan.Matrix;

			double width = ZoomAndPan.ViewWidth;
			double height = ZoomAndPan.ViewHeight;

			minX -= width * 0.05;
			minY -= height * 0.05;
			maxX += width * 0.05;
			maxY += height * 0.05;

			double contentWidth = maxX - minX;
			double contentHeight = maxY - minY;

			ZoomAndPan.StartX = (minX + maxX - width) * 0.5;
			ZoomAndPan.StartY = (minY + maxY - height) * 0.5;
			ZoomAndPan.Scale = 1;

			Point zoomCenter = new Point(width * 0.5, height * 0.5);
			Point scaledZoomCenter = ZoomAndPan.Matrix.Transform(zoomCenter);

			double newScale = Math.Min(width / contentWidth, height / contentHeight);
			ZoomAndPan.Scale = Math.Max(0.1, Math.Min(1.0, newScale));

			Point nextZoomCenter = ZoomAndPan.Matrix.Transform(zoomCenter);
			Point delta = new Point(zoomCenter.X - nextZoomCenter.X, zoomCenter.Y - nextZoomCenter.Y);

			ZoomAndPan.StartX -= delta.X;
			ZoomAndPan.StartY -= delta.Y;
		}

        #endregion

        #region



        #endregion
    }

    // ZoomAndPan copied from: https://github.com/lifeisforu/NodeGraph/blob/master/View/FlowChartView.cs
    public class ZoomAndPan
	{
		#region Properties

		private double _ViewWidth;
		public double ViewWidth
		{
			get { return _ViewWidth; }
			set
			{
				if (value != _ViewWidth)
				{
					_ViewWidth = value;
				}
			}
		}

		private double _ViewHeight;
		public double ViewHeight
		{
			get { return _ViewHeight; }
			set
			{
				if (value != _ViewHeight)
				{
					_ViewHeight = value;
				}
			}
		}

		private double _StartX = 0.0;
		public double StartX
		{
			get { return _StartX; }
			set
			{
				if (value != _StartX)
				{
					_StartX = value;
					_UpdateTransform();
				}
			}
		}

		private double _StartY = 0.0;
		public double StartY
		{
			get { return _StartY; }
			set
			{
				if (value != _StartY)
				{
					_StartY = value;
					_UpdateTransform();
				}
			}
		}

		private double _Scale = 1.0;
		public double Scale
		{
			get { return _Scale; }
			set
			{
				if (value != _Scale)
				{
					_Scale = value;
					_UpdateTransform();
				}
			}
		}

		private Matrix _Matrix = Matrix.Identity;
		public Matrix Matrix
		{
			get { return _Matrix; }
			set
			{
				if (value != _Matrix)
				{
					_Matrix = value;
					_MatrixInv = value;
					_MatrixInv.Invert();
				}
			}
		}

		private Matrix _MatrixInv = Matrix.Identity;
		public Matrix MatrixInv
		{
			get { return _MatrixInv; }
		}


		#endregion // Properties

		#region Methods

		private void _UpdateTransform()
		{
			Matrix newMatrix = Matrix.Identity;
			newMatrix.Scale(_Scale, _Scale);
			newMatrix.Translate(-_StartX, -_StartY);

			Matrix = newMatrix;

			UpdateTransform?.Invoke();
		}

		#endregion // Methods

		#region Events

		public delegate void UpdateTransformDelegate();
		public event UpdateTransformDelegate UpdateTransform;

		#endregion // Events
	}
}
