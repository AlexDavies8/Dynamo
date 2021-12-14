using NodeGraph.Controls;
using NodeGraph.Converters;
using NodeGraph.Model;
using NodeGraph.ViewModel;
using PropertyTools.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace NodeGraph.View
{
    public class PortView : ContentControl
    {
		#region Properties

		public bool LinkToViewModel { get; set; } = true;
        public PortViewModel ViewModel { get; private set; }
        public FrameworkElement PartPort { get; private set; }
        public TextBlock PartTooltip { get; private set; }

		public bool IsInput
		{
			get { return (bool)GetValue(IsInputProperty); }
			set { SetValue(IsInputProperty, value); }
		}
		public static readonly DependencyProperty IsInputProperty =
			DependencyProperty.Register("IsInput", typeof(bool), typeof(PortView), new PropertyMetadata(false));

		public bool IsFilledPort
		{
			get { return (bool)GetValue(IsFilledPortProperty); }
			set { SetValue(IsFilledPortProperty, value); }
		}
		public static readonly DependencyProperty IsFilledPortProperty =
			DependencyProperty.Register("IsFilledPort", typeof(bool), typeof(PortView), new PropertyMetadata(false));

		public bool IsPortEnabled
		{
			get { return (bool)GetValue(IsPortEnabledProperty); }
			set { SetValue(IsPortEnabledProperty, value); }
		}
		public static readonly DependencyProperty IsPortEnabledProperty =
			DependencyProperty.Register("IsPortEnabled", typeof(bool), typeof(PortView), new PropertyMetadata(true));

		public bool Interactable
		{
			get { return (bool)GetValue(InteractableProperty); }
			set { SetValue(InteractableProperty, value); }
		}
		public static readonly DependencyProperty InteractableProperty =
			DependencyProperty.Register("Interactable", typeof(bool), typeof(PortView), new PropertyMetadata(true));

		public bool ToolTipVisibility
		{
			get { return (bool)GetValue(ToolTipVisibilityProperty); }
			set { SetValue(ToolTipVisibilityProperty, value); }
		}
		public static readonly DependencyProperty ToolTipVisibilityProperty =
			DependencyProperty.Register("ToolTipVisibility", typeof(bool), typeof(PortView), new PropertyMetadata(false));

		public string ToolTipText
		{
			get { return (string)GetValue(ToolTipTextProperty); }
			set { SetValue(ToolTipTextProperty, value); }
		}
		public static readonly DependencyProperty ToolTipTextProperty =
			DependencyProperty.Register("ToolTipText", typeof(string), typeof(PortView), new PropertyMetadata(""));

		public bool IsConnectorMouseOver
		{
			get { return (bool)GetValue(IsConnectorMouseOverProperty); }
			set { SetValue(IsConnectorMouseOverProperty, value); }
		}
		public static readonly DependencyProperty IsConnectorMouseOverProperty =
			DependencyProperty.Register("IsConnectorMouseOver", typeof(bool), typeof(PortView), new PropertyMetadata(false));

		public Visibility PropertyEditorVisibility
		{
			get { return (Visibility)GetValue(PropertyEditorVisibilityProperty); }
			set { SetValue(PropertyEditorVisibilityProperty, value); }
		}
		public static readonly DependencyProperty PropertyEditorVisibilityProperty =
			DependencyProperty.Register("PropertyEditorVisibility", typeof(Visibility), typeof(PortView), new PropertyMetadata(Visibility.Hidden));

		public FrameworkElement PropertyEditor
		{
			get { return (FrameworkElement)GetValue(PropertyEditorProperty); }
			set { SetValue(PropertyEditorProperty, value); }
		}
		public static readonly DependencyProperty PropertyEditorProperty =
			DependencyProperty.Register("PropertyEditor", typeof(FrameworkElement), typeof(PortView), new PropertyMetadata(null));

		#endregion

		#region Template

		static PortView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PortView), new FrameworkPropertyMetadata(typeof(NodeView)));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			PartPort = Template.FindName("PART_Port", this) as FrameworkElement;
		}

        #endregion

        #region Constructor

		public PortView(bool isInput)
		{
			IsInput = isInput;
			DataContextChanged += PortViewDataContextChanged;
			Loaded += PortViewLoaded;
		}

        #endregion

        #region Events

		private void PortViewLoaded(object sender, RoutedEventArgs e)
		{
			if (ViewModel.Model.HasEditor) CreatePropertyEditor();
			SynchronizeProperties();


			if (PropertyEditor != null && IsInput)
			{
				PropertyEditorVisibility = Visibility.Collapsed;
			}
			else
			{
				PropertyEditorVisibility = Visibility.Visible;
			}
		}

		private void PortViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			ViewModel = DataContext as PortViewModel;
			if (LinkToViewModel) ViewModel.View = this;
			ViewModel.PropertyChanged += ViewModelPropertyChanged;

			SynchronizeProperties();
		}

		private void SynchronizeProperties()
		{
			if (ViewModel == null)
				return;

			Port port = ViewModel.Model;
			IsInput = port.IsInput;
			IsFilledPort = (0 < port.Connectors.Count);
			IsPortEnabled = true;
			IsEnabled = true;

			if (IsInput)
			{
				//PropertyEditorVisibility = IsFilledPort ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			SynchronizeProperties();
		}

        #endregion

        #region Mouse Events

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			Node node = ViewModel.Model.Owner;
			Flowchart flowchart = node.Owner;
			Keyboard.Focus(flowchart.ViewModel.View);

			if (Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				NodeGraphManager.DisconnectAll(ViewModel.Model);
			}
			else if(!NodeGraphManager.IsConnecting)
			{
				IsFilledPort = true;
				NodeGraphManager.BeginConnection(ViewModel.Model);
			}

			e.Handled = true;
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (NodeGraphManager.IsConnecting)
				{
					bool connectable = NodeGraphManager.CanConnect(ViewModel.Model, out string error);
					if (connectable)
					{
						NodeGraphManager.SetOtherConnectorPort(ViewModel.Model);
						ToolTipVisibility = false;
					}
					else
					{
						if (string.IsNullOrEmpty(error))
							ToolTipVisibility = false;
						else
						{
							ToolTipText = error;
							ToolTipVisibility = true;
						}
					}
				}
			}
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);

			if (NodeGraphManager.IsConnecting)
			{
				NodeGraphManager.SetOtherConnectorPort(null);
			}

			ToolTipVisibility = false;
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);

			if (NodeGraphManager.IsConnecting)
			{
				NodeGraphManager.SetOtherConnectorPort(null);
			}

			ToolTipVisibility = false;
		}

		#endregion

		#region Hit Test

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			if (VisualTreeHelper.GetDescendantBounds(this).Contains(hitTestParameters.HitPoint))
				return new PointHitTestResult(this, hitTestParameters.HitPoint);

			return null;
		}

        #endregion

        #region Property Editors

		private void CreatePropertyEditor()
		{
			Port port = ViewModel.Model as Port;
			if (port.HasEditor)
			{
				Type type = port.ValueType;

				if (type == typeof(bool))
				{
					PropertyEditor = CreateBoolEditor();
				}
				else if (type == typeof(string))
				{
					PropertyEditor = CreateStringEditor();
				}
				else if (type == typeof(int))
				{
					PropertyEditor = CreateIntEditor();
				}
				else if (type == typeof(double))
				{
					PropertyEditor = CreateDoubleEditor();
				}
				else if (type == typeof(Color))
				{
					PropertyEditor = CreateColourEditor();
				}
			}
		}

		private FrameworkElement CreateBoolEditor()
		{
			Port port = ViewModel.Model as Port;

			CheckBox checkBox = new CheckBox();
			checkBox.IsChecked = (bool)port.Value;
			checkBox.SetBinding(CheckBox.IsCheckedProperty, CreateBinding(port, "Value", null));
			return checkBox;
		}

		private FrameworkElement CreateStringEditor()
		{
			Port port = ViewModel.Model as Port;

			TextBox textBox = new TextBox();
			textBox.MinWidth = 50;
			textBox.Text = (string)port.Value;
			textBox.SetBinding(TextBox.TextProperty, CreateBinding(port, "Value", null));
			return textBox;
		}

		private FrameworkElement CreateIntEditor()
		{
			Port port = ViewModel.Model as Port;

			NumericTextBox textBox = new NumericTextBox();
			textBox.MinWidth = 50;
			textBox.IsInteger = true;
			textBox.Text = (port.Value ?? 0).ToString();
			textBox.SetBinding(TextBox.TextProperty, CreateBinding(port, "Value", new IntToStringConverter()));
			return textBox;
		}

		private FrameworkElement CreateDoubleEditor()
		{
			Port port = ViewModel.Model as Port;

			NumericTextBox textBox = new NumericTextBox();
			textBox.MinWidth = 50;
			textBox.IsInteger = false;
			textBox.Text = (port.Value ?? 0).ToString();
			textBox.SetBinding(TextBox.TextProperty, CreateBinding(port, "Value", new DoubleToStringConverter()));
			return textBox;
		}

		private FrameworkElement CreateColourEditor()
		{
			Port port = ViewModel.Model as Port;

			ColorPicker picker = new ColorPicker();
			picker.SelectedColor = (Color)(port.Value != null ? port.Value : Color.FromRgb(255, 255, 255));
			picker.SetBinding(TextBox.TextProperty, CreateBinding(port, "Value", null));
			return picker;
		}

		// Copied from https://github.com/lifeisforu/NodeGraph/blob/1308fb28f5fdb1b9424bb2d86a01d3a288b5d963/View/NodePropertyPortView.cs#L311
		public Binding CreateBinding(Port port, string propertyName, IValueConverter converter)
		{
			var binding = new Binding(propertyName)
			{
				Source = port,
				Mode = BindingMode.TwoWay,
				Converter = converter,
				UpdateSourceTrigger = UpdateSourceTrigger.Default,
				ValidatesOnDataErrors = true,
				ValidatesOnExceptions = true
			};

			return binding;
		}

		#endregion
	}
}
