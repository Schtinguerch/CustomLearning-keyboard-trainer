using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFMeteroWindow.Controls
{
    /// <summary>
    /// Логика взаимодействия для ContentIsle.xaml
    /// </summary>
    public partial class ContentIsle : UserControl
    {
        public ContentIsle()
        {
            DataContext = this;
            InitializeComponent();
        }

        public void SetChild(UIElement child)
        {
            ContentGrid.Children?.Clear();
            ContentGrid.Children.Add(child);
        }

        bool _slip = false;
        private void SlipButton_Click(object sender, RoutedEventArgs e)
        {
            _slip = !_slip;
            if (ContentGrid.Children.Count > 0)
                RefreshStoryboards(ContentGrid.Children[0]);

            ButtonBeginStoryboard.Storyboard = _slip? 
                FindResource("SlipContentStoryboard") as Storyboard : 
                FindResource("ExpandContentStoryboard") as Storyboard;
        }

        private void RefreshStoryboards(UIElement child)
        {
            var expandStoryboard = FindResource("ExpandContentStoryboard") as Storyboard;
            var slipStoryboard = FindResource("SlipContentStoryboard") as Storyboard;

            var controlRef = child as Control;
            var margin = controlRef == null? new Thickness(0) : controlRef.Margin;
            var height = child.RenderSize.Height + margin.Top + margin.Bottom;

            (expandStoryboard.Children[0] as DoubleAnimation).To = height;
            (slipStoryboard.Children[0] as DoubleAnimation).From = height;
        }
    }
}
