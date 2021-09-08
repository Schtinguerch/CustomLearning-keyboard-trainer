using System;
using System.Collections.Generic;
using System.Windows;
using System.Net;
using LmlLibrary;
using Localization = CustomLearningUpdater.localizations.Resources;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace CustomLearningUpdater
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _baseUrl = "https://raw.githubusercontent.com/Schtinguerch/CustomLearning-keyboard-trainer/master/WPFMeteroWindow/bin/Release";
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
            GetCommonData();
        }

        private void GetCommonData()
        {
            var webClient = new WebClient();
            Lml parcer = new Lml("empty", Lml.Open.FromString);
            string manifestData;
            int fileCount;
            Dictionary<string, string> fileList = new Dictionary<string, string>();
            System.Diagnostics.Process[] processes;
            string previousVersion, newVersion = "", appVersion = AssemblyName.GetAssemblyName(@"CustomLearning.exe").Version.ToString();
            Clipboard.SetText(appVersion);

            ThreadPool.QueueUserWorkItem((o) =>
            {
                Dispatcher.Invoke((Action)(() => 
                {
                    SetStatus(Localization.uDownloadingUpdateManifest);

                    webClient.DownloadProgressChanged += DownloadProgressed;

                    manifestData = webClient.DownloadString($"{_baseUrl}/{_manifestFile}");
                    parcer = new Lml(manifestData, Lml.Open.FromString);

                    SetStatus(Localization.uProcessingData);
                    newVersion = parcer.GetString("Manifest>CurrentVersion");
                    UpdatingToVersionTextBox.Text = $"{Localization.uUpdateTo} {newVersion}";
                }));
                Thread.Sleep(100);

                Dispatcher.Invoke((Action)(() =>
                {
                    if (newVersion == appVersion) return;

                    fileList = new Dictionary<string, string>();

                    do
                    {
                        previousVersion = parcer.GetString("Manifest>PreviousVersion");
                        fileCount = parcer.GetInt("Manifest>Files>Count");

                        for (int i = 0; i < fileCount; i++)
                        {
                            var fileName = parcer.GetString($"Manifest>Files>File{i}");
                            string buf;

                            if (!fileList.TryGetValue(fileName, out buf))
                                fileList.Add($"{_baseFolder}{fileName}", $"{_baseUrl}/{fileName}");
                        }

                        if (previousVersion == "NO") break;

                        manifestData = webClient.DownloadString($"{_baseUrl}/{previousVersion}.lml");
                        parcer = new Lml(manifestData, Lml.Open.FromString);
                    }
                    while (previousVersion != appVersion);

                }));
                Thread.Sleep(100);

                foreach (var file in fileList)
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        SetStatus($"{Localization.uDownloading} {System.IO.Path.GetFileName(file.Key)}");
                        webClient.DownloadFile(file.Value, file.Key);
                    }));
                    Thread.Sleep(100);
                }

                Dispatcher.Invoke((Action)(() =>
                {
                    SetStatus(Localization.uUpdateIsCompleted);
                    processes = System.Diagnostics.Process.GetProcessesByName("CustomLearning");

                    if (processes == null) return;
                    if (processes.Length == 0) return;

                    foreach (var process in processes)
                        process.Kill();

                    System.Diagnostics.Process.Start("CustomLearning.exe", "upd");
                    
                }));

                Thread.Sleep(2000);
                
            });
        }
        
        private void DownloadProgressed(object sender, DownloadProgressChangedEventArgs e)
        {
            UpdatingStatusTextBox.Text = $"{_downloadingObject}, {e.ProgressPercentage}% : {e.BytesReceived}B / {e.TotalBytesToReceive}B";
        }
    }
}
