using NodeGraph.Model;
using NodeGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NodeGraph.View
{
    public class ConnectorView : ContentControl
    {
        #region Properties

        public ConnectorViewModel ViewModel { get; private set; }

        public string CurveData
        {
            get => (string)GetValue(CurveDataProperty);
            set => SetValue(CurveDataProperty, value);
        }
        public static readonly DependencyProperty CurveDataProperty = DependencyProperty.Register("CurveData", typeof(string), typeof(ConnectorView), new PropertyMetadata(""));

        #endregion

        #region Constructor

        public ConnectorView()
        {
            LayoutUpdated += ConnectorViewLayoutUpdated;
            DataContextChanged += ConnectorViewDataContextChanged;
        }

        #endregion

        #region Events

        private void ConnectorViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = DataContext as ConnectorViewModel;
            ViewModel.View = this;
        }

        private void ConnectorViewLayoutUpdated(object sender, EventArgs e)
        {
            Flowchart flowchart = ViewModel.Model.Owner;
            FlowchartView flowchartView = flowchart.ViewModel.View;
            BuildCurveData(Mouse.GetPosition(flowchartView));
        }

        #endregion

        #region Curve

        public void BuildCurveData(Point mousePosition)
        {
            Connector connector = ViewModel.Model;
            Flowchart flowchart = connector.Owner;
            FlowchartView flowchartView = flowchart.ViewModel.View;

            Port startPort = connector.StartPort;
            Port endPort = connector.EndPort;

            Point start = (startPort != null) ? ViewUtility.GetRelativeCenterLocation(startPort.ViewModel.View.PartPort, flowchartView) : mousePosition;
            Point end = (endPort != null) ? ViewUtility.GetRelativeCenterLocation(endPort.ViewModel.View.PartPort, flowchartView) : mousePosition;
            Point centre = new Point((start.X + end.X) * 0.5, (start.Y + end.Y) * 0.5);

            if (start.X > end.X)
                (start, end) = (end, start);

            double ratio = Math.Min(1, (centre.X - start.X) / 100);
            Point c0 = start;
            Point c1 = end;
            c0.X += 100 * ratio;
            c1.X -= 100 * ratio;

            CurveData = string.Format("M{0},{1} C{0},{1} {2},{3} {4},{5} " +
                "M{4},{5} C{4},{5} {6},{7} {8},{9}",
                (int)start.X, (int)start.Y, // 0, 1
                (int)c0.X, (int)c0.Y, // 2, 3
                (int)centre.X, (int)centre.Y, // 4, 5
                (int)c1.X, (int)c1.Y, // 6, 7
                (int)end.X, (int)end.Y); // 8.9
        }

        #endregion

        #region Mouse Events

        // Highlight connected ports on hover
        /*
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            Connector connector = ViewModel.Model;

            if (connector.StartPort != null)
            {
                connector.StartPort.ViewModel.View.IsConnectorMouseOver = true;
            }

            if (connector.EndPort != null)
            {
                connector.EndPort.ViewModel.View.IsConnectorMouseOver = true;
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            Connector connector = ViewModel.Model;

            if (connector.StartPort != null)
            {
                connector.StartPort.ViewModel.View.IsConnectorMouseOver = false;
            }

            if (connector.EndPort != null)
            {
                connector.EndPort.ViewModel.View.IsConnectorMouseOver = false;
            }
        }
        */

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            
            // TODO: Add Router Nodes
        }

        #endregion
    }
}
