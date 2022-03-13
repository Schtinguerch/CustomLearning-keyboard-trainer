using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

using Localization = WPFMeteroWindow.Resources.localizations.Resources;


namespace WPFMeteroWindow.Controls
{
    public enum CMessageBoxType
    {
        Error,
        Info,
        Warning,
        YesNo,
        YesNoCancel,
    }

    /// <summary>
    /// Логика взаимодействия для CMessageBox.xaml
    /// </summary>
    public partial class CMessageBox : Window
    {
        public CMessageBox()
        {
            InitializeComponent();
        }

        private static CMessageBox cMessageBox;
        private static DialogResult result = System.Windows.Forms.DialogResult.No;

        
        public static DialogResult Show(string message, CMessageBoxType type = CMessageBoxType.Info)
        {
            cMessageBox = new CMessageBox();
            
            switch (type)
            {
                case CMessageBoxType.Error:
                    cMessageBox.HeaderTextBlock.Text = Localization.uError;
                    cMessageBox.FigurePath.Data = Geometry.Parse("M 0 16 A 1 1 0 0 0 32 16 A 1 1 0 0 0 0 16 M 18 26 L 14 26 L 14 22 L 18 22 L 18 26 M 13 6 L 19 6 L 18 19 L 14 19");

                    cMessageBox.YesButton.Visibility = Visibility.Collapsed;
                    cMessageBox.NoButton.Visibility = Visibility.Collapsed;
                    cMessageBox.CancelButton.Visibility = Visibility.Collapsed;
                    break;

                case CMessageBoxType.Warning:
                    cMessageBox.HeaderTextBlock.Text = Localization.uWarning;
                    cMessageBox.FigurePath.Data = Geometry.Parse("M 0 16 A 1 1 0 0 0 0 16 M 18 26 L 14 26 L 14 22 L 18 22 L 18 26 M 13 9 L 19 9 L 18 19 L 14 19 M 0 29 L 31 29 L 16 0 L 0 29");

                    cMessageBox.OkButton.Visibility = Visibility.Collapsed;
                    break;

                case CMessageBoxType.Info:
                    cMessageBox.HeaderTextBlock.Text = Localization.uInfo;
                    cMessageBox.FigurePath.Data = Geometry.Parse("M 0 16 A 1 1 0 0 0 32 16 A 1 1 0 0 0 0 16 M 18 26 L 14 26 L 14 13 L 18 13 L 18 17 M 14 7 L 18 7 L 18 11 L 14 11");

                    cMessageBox.YesButton.Visibility = Visibility.Collapsed;
                    cMessageBox.NoButton.Visibility = Visibility.Collapsed;
                    cMessageBox.CancelButton.Visibility = Visibility.Collapsed;
                    break;

                case CMessageBoxType.YesNoCancel:
                    cMessageBox.HeaderTextBlock.Text = Localization.uAttention;
                    cMessageBox.FigurePath.Data = Geometry.Parse("M 2 3 C 2 2 9 1 15 1 C 19 1 28 3 29 6 C 30 9 30 13 29 14 C 26 17 21 19 17 20 C 13 21 13 21 13 22 L 11 22 C 10 20 10 19 11 18 C 14 15 19 14 19 12 C 19 8 19 8 17 7 C 12 6 9 6 6 7 C 3 8 3 8 2 10 L 2 3 M 13 24 C 15 24 16 24 16 26 C 16 28 15 28 13 28 C 11 28 10 28 10 26 C 10 24 11 24 13 24");

                    cMessageBox.OkButton.Visibility = Visibility.Collapsed;
                    break;

                case CMessageBoxType.YesNo:
                    cMessageBox.HeaderTextBlock.Text = Localization.uAttention;
                    cMessageBox.FigurePath.Data = Geometry.Parse("M 2 3 C 2 2 9 1 15 1 C 19 1 28 3 29 6 C 30 9 30 13 29 14 C 26 17 21 19 17 20 C 13 21 13 21 13 22 L 11 22 C 10 20 10 19 11 18 C 14 15 19 14 19 12 C 19 8 19 8 17 7 C 12 6 9 6 6 7 C 3 8 3 8 2 10 L 2 3 M 13 24 C 15 24 16 24 16 26 C 16 28 15 28 13 28 C 11 28 10 28 10 26 C 10 24 11 24 13 24");

                    cMessageBox.OkButton.Visibility = Visibility.Collapsed;
                    cMessageBox.CancelButton.Visibility = Visibility.Collapsed;
                    break;
            }
            
            cMessageBox.MessageTextBlock.Text = message;
            cMessageBox.ShowDialog();
            return result;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            result = System.Windows.Forms.DialogResult.Ignore;
            cMessageBox?.Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            result = System.Windows.Forms.DialogResult.Yes;
            cMessageBox?.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            result = System.Windows.Forms.DialogResult.No;
            cMessageBox?.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            result = System.Windows.Forms.DialogResult.OK;
            cMessageBox?.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            result = System.Windows.Forms.DialogResult.Cancel;
            cMessageBox?.Close();
        }
    }
}
