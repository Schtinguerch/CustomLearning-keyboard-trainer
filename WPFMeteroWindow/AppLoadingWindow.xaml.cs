using System;
using System.Threading;
using System.Windows;

namespace WPFMeteroWindow
{
    /// <summary>
    /// Логика взаимодействия для AppLoadingWindow.xaml
    /// </summary>
    public partial class AppLoadingWindow : Window
    {
        public AppLoadingWindow()
        {
            Intermediary.Loader = this;
            InitializeComponent();
        }
    }
}
