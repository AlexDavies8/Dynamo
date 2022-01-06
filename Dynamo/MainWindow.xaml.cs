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

            ExecutionManager.OnPostExecute += () => viewportPanel.RaisePropertyChanged("DisplayedNode");
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
                        () => NodeGraphManager.CreateNode(
                            pathArgs[^1],
                            Guid.NewGuid(),
                            flowchart,
                            type,
                            args.ModelSpaceMousePosition.X,
                            args.ModelSpaceMousePosition.Y
                            )
                        ));
                }
            }

            RecurseTree(addNodeItem, root);

            return addNodeItem;

            void RecurseTree(MenuItem parentItem, PathTreeItem root)
            {
                foreach (var pair in root.Children)
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

        struct PathTreeItem
        {
            public string Header;
            public Action Click;
            public Dictionary<string, PathTreeItem> Children;

            public PathTreeItem(string header, Action click)
            {
                Header = header;
                Click = click;
                Children = new Dictionary<string, PathTreeItem>();
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
                    NodeGraphManager.Deserialize(path);
                    foreach (var pair in NodeGraphManager.Flowcharts)
                        FlowchartViewModel = pair.Value.ViewModel;
                    ExecutionManager.ResolveDirtyNodes();
                }
            }
        }
    }
}
