using System;
using System.Windows.Controls;

using Localization = CustomLearningInstaller.Resources.Localizations.Resources;

namespace CustomLearningInstaller.Resources.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProcessingPage.xaml
    /// </summary>
    public partial class ProcessingPage : Page
    {
        public ProcessingPage()
        {
            InitializeComponent();

            AppInstaller.DownloadProgressChanged += AppInstaller_DownloadProgressChanged;
            AppInstaller.DownloadFileCompleted += AppInstaller_DownloadFileCompleted;

            StatusTextBlock.Text = Localization.Downloading;
            AppInstaller.DownloadArchive();
        }

        private void AppInstaller_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ProgressBar.Value = 50d;
            StatusTextBlock.Text = Localization.Extrading;

            AppInstaller.ExtractionCompleted += AppInstaller_ExtractionCompleted;
            AppInstaller.ExtractArchive();
        }

        private void AppInstaller_ExtractionCompleted()
        {
            AppInstaller.DoAdditionalTasks();
            PageNavigator.OpenNextPage();
        }

        private void AppInstaller_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            double percentage = (e.BytesReceived + 0d) / (e.TotalBytesToReceive + 0d);
            
            double downloadedMBs = e.BytesReceived / 1024d / 1024d;
            double totalMBs = e.TotalBytesToReceive / 1024d / 1024d;

            StatusTextBlock.Text = $"{Localization.Downloading}: {percentage:N}% {downloadedMBs:N} / {totalMBs:N} MB";
            ProgressBar.Value = percentage * 100d;
        }
    }
}
