using System;
using System.Diagnostics;
using System.Windows;
using Dynamo.Model;
using System.Windows.Media;
using NodeGraph;
using NodeGraph.Model;
using System.Windows.Controls;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Linq;
using System.Windows.Media.Effects;

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

        private string _projectPath;

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

            BuidFileContextMenu();
            BuildEditContextMenu();

            ExecutionManager.OnPostExecute += () => viewportPanel.RaisePropertyChanged("DisplayedNode");
            ExecutionManager.OnPostExecute += () => viewportPanel2.RaisePropertyChanged("DisplayedNode");

            var startupWindow = new StartupDialog()
            {
                Owner = this
            };

            Opacity = 0.7;
            Effect = new BlurEffect();

            startupWindow.Closed += (sender, e) =>
            {
                Opacity = 1;
                Effect = null;
            };
            startupWindow.NewProjectCallback += () => startupWindow.Close();
            startupWindow.OpenProjectCallback += () =>
            {
                OpenButtonClick(null, null);
                // Check if project actually opened
                startupWindow.Close();
            };

            startupWindow.Show();
        }

        private void BuidFileContextMenu()
        {
            MenuItem root = new MenuItem() { Header = "File" };

            MenuItem newItem = new MenuItem() { Header = "New Project" };
            newItem.Click += (sender, e) => NewFile();
            root.Items.Add(newItem);

            root.Items.Add(new Separator());

            MenuItem openItem = new MenuItem() { Header = "Open Project"};
            openItem.Click += (sender, e) => OpenFile();
            root.Items.Add(openItem);

            root.Items.Add(new Separator());

            MenuItem saveItem = new MenuItem() { Header = "Save Project" };
            saveItem.Click += (sender, e) => SaveFile(false);
            root.Items.Add(saveItem);

            MenuItem saveAsItem = new MenuItem() { Header = "Save Project As" };
            saveAsItem.Click += (sender, e) => SaveFile(true);
            root.Items.Add(saveAsItem);

            root.Items.Add(new Separator());

            MenuItem quitItem = new MenuItem() { Header = "Quit" };
            quitItem.Click += (sender, e) => Close();
            root.Items.Add(quitItem);

            FileMenu.Items.Add(root);
        }

        private void BuildEditContextMenu()
        {
            MenuItem root = new MenuItem() { Header = "Edit" };

            MenuItem openItem = new MenuItem() { Header = "Select All" };
            openItem.Click += (sender, e) => NodeGraphManager.SelectAllNodes(FlowchartViewModel.Model);
            root.Items.Add(openItem);

            MenuItem saveItem = new MenuItem() { Header = "Deselect" };
            saveItem.Click += (sender, e) => NodeGraphManager.DeselectAllNodes(FlowchartViewModel.Model);
            root.Items.Add(saveItem);

            EditMenu.Items.Add(root);
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

        private void SaveFile(bool saveAs)
        {
            if (!saveAs && _projectPath != null)
            {
                if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(_projectPath)))
                {
                    NodeGraphManager.Serialize(_projectPath);
                    Title = $" Dynamo ({System.IO.Path.GetFileNameWithoutExtension(_projectPath)})";
                    return;
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "Dynamo Project (*.dynamo)|*.dynamo|All files (*.*)|*.*";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                var path = saveFileDialog.FileName;
                if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                {
                    NodeGraphManager.Serialize(path);
                    Title = $" Dynamo ({System.IO.Path.GetFileNameWithoutExtension(path)})";
                    _projectPath = path;
                }
            }
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Dynamo Project (*.dynamo)|*.dynamo|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;
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

                    Title = $" Dynamo ({System.IO.Path.GetFileNameWithoutExtension(path)})";

                    _projectPath = path;
                }
            }
        }

        private void NewFile()
        {
            bool temp = ExecutionManager.AutoExecute;
            ExecutionManager.AutoExecute = false;

            NodeGraphManager.DestroyFlowchart(FlowchartViewModel.Model.Guid);

            ExecutionManager.ResolveDirtyNodes();

            ExecutionManager.AutoExecute = temp;

            Title = $" Dynamo (Unnamed Project)";
        }
    }
}
