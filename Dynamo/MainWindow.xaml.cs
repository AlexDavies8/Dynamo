using System;
using System.Diagnostics;
using System.Windows;
using WindowDocker.Model;
using Dynamo.Model;
using System.Windows.Media;
using NodeGraph;
using NodeGraph.Model;
using System.Windows.Controls;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Linq;

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

        public Dynamo.ViewModel.PropertyPanelViewModel PropertyPanelViewModel
        {
            get => (ViewModel.PropertyPanelViewModel)GetValue(PropertyPanelViewModelProperty);
            set => SetValue(PropertyPanelViewModelProperty, value);
        }
        public static readonly DependencyProperty PropertyPanelViewModelProperty = DependencyProperty.Register("PropertyPanelViewModel", typeof(Dynamo.ViewModel.PropertyPanelViewModel), typeof(MainWindow), new PropertyMetadata(null));

        public Dynamo.ViewModel.ViewportPanelViewModel ViewportPanelViewModel
        {
            get => (ViewModel.ViewportPanelViewModel)GetValue(ViewportPanelViewModelProperty);
            set => SetValue(ViewportPanelViewModelProperty, value);
        }
        public static readonly DependencyProperty ViewportPanelViewModelProperty = DependencyProperty.Register("ViewportPanelViewModel", typeof(Dynamo.ViewModel.ViewportPanelViewModel), typeof(MainWindow), new PropertyMetadata(null));

        public Dynamo.ViewModel.ViewportPanelViewModel ViewportPanelViewModel2
        {
            get => (ViewModel.ViewportPanelViewModel)GetValue(ViewportPanelViewModel2Property);
            set => SetValue(ViewportPanelViewModel2Property, value);
        }
        public static readonly DependencyProperty ViewportPanelViewModel2Property = DependencyProperty.Register("ViewportPanelViewModel2", typeof(Dynamo.ViewModel.ViewportPanelViewModel), typeof(MainWindow), new PropertyMetadata(null));

        public static bool AutoExecute
        {
            get => ExecutionManager.AutoExecute;
            set => ExecutionManager.AutoExecute = value;
        }

        public MainWindow()
        {
            // TODO: Move to App.xaml.cs
            GlobalState.Initialise();

            InitializeComponent();

            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Flowchart flowchart = NodeGraphManager.CreateFlowchart(Guid.NewGuid());
            FlowchartViewModel = flowchart.ViewModel;
            NodeGraphManager.BuildFlowchartContextMenu = BuildFlowchartContextMenu;

            PropertyPanel propertyPanel = new PropertyPanel();
            ViewModel.PropertyPanelViewModel propertyPanelViewModel = new ViewModel.PropertyPanelViewModel(propertyPanel);
            PropertyPanelViewModel = propertyPanelViewModel;

            ViewportPanel viewportPanel = new ViewportPanel();
            ViewModel.ViewportPanelViewModel viewportPanelViewModel = new ViewModel.ViewportPanelViewModel(viewportPanel);
            ViewportPanelViewModel = viewportPanelViewModel;

            ViewportPanel viewportPanel2 = new ViewportPanel();
            ViewModel.ViewportPanelViewModel viewportPanelViewModel2 = new ViewModel.ViewportPanelViewModel(viewportPanel2);
            ViewportPanelViewModel2 = viewportPanelViewModel2;

            ExecutionManager.OnPostExecute += () => viewportPanel.RaisePropertyChanged("DisplayedNode");
            ExecutionManager.OnPostExecute += () => viewportPanel2.RaisePropertyChanged("DisplayedNode");
        }

        private ContextMenu BuildFlowchartContextMenu(BuildContextMenuArgs args)
        {
            ContextMenu menu = NodeGraphManager.DefaultFlowchartContextMenu(args) ?? new ContextMenu();

            menu.Items.Add(GetAddNodeMenuItem(args));

            return menu;
        }

        private MenuItem GetAddNodeMenuItem(BuildContextMenuArgs args)
        {
            Flowchart flowchart = args.Model as Flowchart;

            MenuItem addNodeItem = new MenuItem() { Header = "Add Node" };
            Assembly assembly = Assembly.GetExecutingAssembly();
            PathTreeItem root = new PathTreeItem("Add Node", null);
            foreach (Type type in assembly.GetTypes())
            {
                var nodeAttribute = type.GetCustomAttribute<NodeAttribute>();
                if (nodeAttribute != null)
                {
                    PathTreeItem curr = root;
                    string[] pathArgs = nodeAttribute.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < pathArgs.Length - 1; i++)
                    {
                        PathTreeItem branch = new PathTreeItem(pathArgs[i], null);
                        if (!curr.Children.ContainsKey(pathArgs[i]))
                        {
                            curr.Children.Add(pathArgs[i], branch);
                            curr = branch;
                        }
                        else
                            curr = curr.Children[pathArgs[i]];
                    }
                    curr.Children.Add(
                        pathArgs[^1],
                        new PathTreeItem(pathArgs[^1],
                        () => {
                            Node node = NodeGraphManager.CreateNode(
                                pathArgs[^1],
                                Guid.NewGuid(),
                                flowchart,
                                type,
                                args.ModelSpaceMousePosition.X,
                                args.ModelSpaceMousePosition.Y
                                );
                            if (node is ExecutableNode executableNode)
                                ExecutionManager.MarkDirty(executableNode);
                            }, nodeAttribute.Order)
                        );
                }
            }

            RecurseOrder(root);
            RecurseTree(addNodeItem, root);

            return addNodeItem;

            int RecurseOrder(PathTreeItem root)
            {
                int order = root.Children.Count == 0 ? root.Order : root.Children.Min(x => RecurseOrder(x.Value));
                root.Order = order;
                return order;
            }

            void RecurseTree(MenuItem parentItem, PathTreeItem root)
            {
                foreach (var pair in root.Children.OrderBy(x => x.Value.Order))
                {
                    string header = pair.Key;
                    PathTreeItem child = pair.Value;
                    MenuItem item = new MenuItem() { Header = header };
                    if (child.Click != null) item.Click += (sender, e) => child.Click();
                    parentItem.Items.Add(item);
                    RecurseTree(item, child);
                }
            }
        }

        class PathTreeItem
        {
            public string Header;
            public Action Click;
            public Dictionary<string, PathTreeItem> Children;
            public int Order;

            public PathTreeItem(string header, Action click, int order = int.MaxValue)
            {
                Header = header;
                Click = click;
                Children = new Dictionary<string, PathTreeItem>();
                Order = order;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ExecutionManager.ResolveDirtyNodes();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                var path = saveFileDialog.FileName;
                if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                {
                    NodeGraphManager.Serialize(path);
                }
            }
        }

        private void OpenButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var path = openFileDialog.FileName;
                if (System.IO.File.Exists(path))
                {
                    bool temp = ExecutionManager.AutoExecute;
                    ExecutionManager.AutoExecute = false;

                    NodeGraphManager.Deserialize(path);
                    foreach (var pair in NodeGraphManager.Flowcharts)
                        FlowchartViewModel = pair.Value.ViewModel;
                    ExecutionManager.ResolveDirtyNodes();

                    ExecutionManager.AutoExecute = temp;
                }
            }
        }
    }
}
