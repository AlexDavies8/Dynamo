using System;
using System.Diagnostics;
using System.Windows;
using WindowDocker.Model;
using Dynamo.Model;
using System.Windows.Media;

namespace Dynamo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public NodeGraph.ViewModel.FlowchartViewModel FlowchartViewModel
        {
            get => (NodeGraph.ViewModel.FlowchartViewModel)GetValue(FlowchartViewModelProperty);
            set => SetValue(FlowchartViewModelProperty, value);
        }
        public static readonly DependencyProperty FlowchartViewModelProperty = DependencyProperty.Register("FlowchartViewModel", typeof(NodeGraph.ViewModel.FlowchartViewModel), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();

            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NodeGraph.Model.Flowchart flowchart = NodeGraph.NodeGraphManager.CreateFlowchart(Guid.NewGuid());
            FlowchartViewModel = flowchart.ViewModel;

            ValueNode<int> intNodeA = (ValueNode<int>)NodeGraph.NodeGraphManager.CreateNode("Int Value", Guid.NewGuid(), flowchart, typeof(ValueNode<int>), 150, -20);
            ValueNode<int> intNodeB = (ValueNode<int>)NodeGraph.NodeGraphManager.CreateNode("Int Value", Guid.NewGuid(), flowchart, typeof(ValueNode<int>), 150, -120);

            MathAddNode addNode = (MathAddNode)NodeGraph.NodeGraphManager.CreateNode("Add Integers", Guid.NewGuid(), flowchart, typeof(MathAddNode), 100, 200);

            DebugValueNode<int> debugNode = (DebugValueNode<int>)NodeGraph.NodeGraphManager.CreateNode("Debug Log", Guid.NewGuid(), flowchart, typeof(DebugValueNode<int>), 500, 300);

            
            ImageNode imageNode = (ImageNode)NodeGraph.NodeGraphManager.CreateNode("Open Image", Guid.NewGuid(), flowchart, typeof(ImageNode), 600, 100);
            ResizeImageNode imageResizeNode = (ResizeImageNode)NodeGraph.NodeGraphManager.CreateNode("Resize Image", Guid.NewGuid(), flowchart, typeof(ResizeImageNode), 600, 300);
            SaveImageNode saveImageNode = (SaveImageNode)NodeGraph.NodeGraphManager.CreateNode("Save Image", Guid.NewGuid(), flowchart, typeof(SaveImageNode), 800, 150);
              
    }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ExecutionManager.ResolveDirtyNodes();
        }
    }
}
