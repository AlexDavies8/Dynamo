using NodeGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

        #endregion

        #region Mouse Events

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

		#region Methdos

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
