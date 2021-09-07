using System;
using System.Collections.Generic;
using System.Windows;
using System.Net;
using LmlLibrary;
using Localization = CustomLearningUpdater.localizations.Resources;

namespace CustomLearningUpdater
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _baseUrl = "https://github.com/Schtinguerch/CustomLearning-keyboard-trainer/tree/master/WPFMeteroWindow/bin/Release";
        private readonly string _baseFolder = AppDomain.CurrentDomain.BaseDirectory;
        private const string _manifestFile = "manifest.lml";

        private string _downloadingObject = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetStatus(string text)
        {
            UpdatingStatusTextBox.Text = text;
            _downloadingObject = text;
        }
            

        private void StartUpdatingButton_Click(object sender, RoutedEventArgs e)
        {
            StartUpdatingButton.Visibility = Visibility.Hidden;
            StartUpdateProcess();
        }

        private void StartUpdateProcess()
        {
            SetStatus(Localization.uDownloadingUpdateManifest);

            var webClient = new WebClient();
            webClient.DownloadProgressChanged += DownloadProgressed;

            var manifestData = webClient.DownloadString($"{_baseUrl}/{_manifestFile}");

            var parcer = new Lml(manifestData, Lml.Open.FromString);

            SetStatus(Localization.uProcessingData);
            UpdatingToVersionTextBox.Text = parcer.GetString("Manifeset>CurrentVersion");

            var fileList = new Dictionary<string, string>();
            var fileCount = parcer.GetInt("Manifest>Files>Count");

            for (int i = 0; i < fileCount; i++)
            {
                var fileName = parcer.GetString("Manifest>Files>File");
                var fileUrl = parcer.GetString("Manifest>File>Url");

                fileList.Add(fileName, $"{_baseUrl}/{fileUrl}");
            }

            foreach (var file in fileList)
            {
                SetStatus($"{Localization.uDownloading} {System.IO.Path.GetFileName(file.Key)}");
                webClient.DownloadFile(file.Value, file.Key);
            }

            SetStatus(Localization.uUpdateIsCompleted);
            var processes = System.Diagnostics.Process.GetProcessesByName("CustomLearning");

            if (processes == null) return;
            if (processes.Length == 0) return;

            foreach (var process in processes)
                process.Kill();

            System.Diagnostics.Process.Start("CustomLearning.exe", "upd");
            Application.Current.Shutdown();
        }

        private void DownloadProgressed(object sender, DownloadProgressChangedEventArgs e)
        {
            UpdatingStatusTextBox.Text = $"{_downloadingObject}, {e.ProgressPercentage}% : {e.BytesReceived}B / {e.TotalBytesToReceive}B";
        }
    }
}
