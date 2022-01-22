using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPFMeteroWindow.Controls
{
    /// <summary>
    /// Логика взаимодействия для TextList.xaml
    /// </summary>
    public partial class TextList : UserControl
    {
        public double ItemsHeight => 
            (ItemStackPanel.Children.Count == 0? 
                0 : (ItemStackPanel.Children[0] as TextBlock).FontSize + 4)
                * (ItemStackPanel.Children.Count + 1);

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set
            {
                SetValue(OrientationProperty, value);
                ItemStackPanel.Orientation = value;

                if (value == Orientation.Horizontal)
                {
                    Viewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    Viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                }

                else
                {
                    Viewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    Viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                }
            }
        }

        public static readonly DependencyProperty OrientationProperty 
            = DependencyProperty.Register(
                "Orientation", 
                typeof(Orientation), 
                typeof(TextList), 
                new UIPropertyMetadata(Orientation.Vertical));

        private Style _textBlockStyle = new Style(typeof(TextBlock));

        private List<string> _items = new List<string>();
        public List<string> Items
        {
            get => _items;
            set
            {
                var list = value;
                list.Sort();

                _items = list.Distinct().ToList();
                ItemStackPanel.Children?.Clear();
                ItemStackPanel.Children.Capacity = _items.Count;

                foreach (var item in _items)
                    ItemStackPanel.Children.Add(
                        new TextBlock()
                        {
                            Text = item,
                            Padding = new Thickness(10, 2, 10, 2),
                            Style = _textBlockStyle,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            FontSize = 18d,
                        });
            }
        }

        public TextList()
        {
            var dictionary = (ResourceDictionary)Application.LoadComponent(new Uri("\\Resources\\dictionaries\\Styles.xaml", UriKind.Relative));
           _textBlockStyle = (Style)dictionary["SummaryTextStyle"];

            InitializeComponent();
        }
    }
}
