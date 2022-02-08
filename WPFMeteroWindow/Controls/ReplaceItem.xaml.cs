using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFMeteroWindow.Controls
{
    /// <summary>
    /// Логика взаимодействия для ReplaceItem.xaml
    /// </summary>
    public partial class ReplaceItem : UserControl
    {
        public Panel ParentControl { get; set; }

        public string InputKey
        {
            get => KeyTextBox.Text;
            set => KeyTextBox.Text = value;
        }

        public string Replacement
        {
            get => ReplacementTextBox.Text;
            set => ReplacementTextBox.Text = value;
        }

        public ReplaceItem()
        {
            InitializeComponent();
        }

        public ReplaceItem(Panel parentControl, string key, string replacement)
        {
            InitializeComponent();

            ParentControl = parentControl;
            InputKey = key;
            Replacement = replacement;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ParentControl == null) return;
            ParentControl.Children.Remove(this);
        }
    }
}
