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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using WPFMeteroWindow.Controls;

namespace WPFMeteroWindow
{
    /// <summary>
    /// Логика взаимодействия для TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CanvasGrid.Children?.Clear();
            CanvasGrid.Children.Add(new StatsVisualizer(ValuesFromText(), ArgsTextBox.Text.Split(new char[] { ';' },  StringSplitOptions.RemoveEmptyEntries).ToList()));
        }

        private List<List<double>> ValuesFromText()
        {
            var plots = PointsTextBox.Text.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var values = new List<List<double>>();

            foreach (var plot in plots)
                values.Add(ParseNumbers(plot));

            return values;
        }

        private List<double> ParseNumbers(string text)
        {
            var numbersString = text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var numbers = new List<double>();

            foreach (var number in numbersString)
                numbers.Add(double.Parse(number));

            return numbers;
        }
    }
}
