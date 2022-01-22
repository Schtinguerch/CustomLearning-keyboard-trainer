using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Reflection;

using Newtonsoft.Json;

namespace CustomLearningUpdater
{
    public class UpdateData
    {
        public List<string> Files { get; set; }
        public string PreviousVersion { get; set; }
    }

    public partial class MainWindow : Window
    {
        private const string _baseUrl = "https://schtinguerch.github.io/UpdateManifests";
        private const string _assetsFolder = "UpdateFiles";
        private readonly string _baseFolder = AppDomain.CurrentDomain.BaseDirectory;
        private const string _manifestFile = "lastVersion";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartUpdatingButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatingToVersionTextBox.Text = localizations.Resources.uDownloading;

            var appVersion = AssemblyName.GetAssemblyName(@"CustomLearning.exe").Version.ToString();
            var files = UpdateFileList(appVersion, _manifestFile, _baseUrl);

            DownloadFiles(files, _baseFolder, $"{_baseUrl}/{_assetsFolder}");
            UpdatingToVersionTextBox.Text = localizations.Resources.uUpdateIsCompleted;
        }

        private List<string> UpdateFileList(string currentVelsion, string manifestFile, string serverUrl)
        {
            var webClient = new WebClient();
            var files = new List<string>();

            do
            {
                try
                {
                    var path = $"{serverUrl}/{manifestFile}.json";
                    var jsonData = webClient.DownloadString(path);
                    var data = JsonConvert.DeserializeObject<UpdateData>(jsonData);

                    manifestFile = data.PreviousVersion;
                    foreach (var file in data.Files)
                    {
                        files.Add(file);
                    }
                }
                catch
                {
                    break;
                }
            }
            while (!string.IsNullOrWhiteSpace(manifestFile) && manifestFile != currentVelsion);

            var filesWithoutDuplicates = files.Distinct().ToList();
            return filesWithoutDuplicates;
        }

        private void DownloadFiles(List<string> files, string localComputerFolder, string serverFolder)
        {
            var webClient = new WebClient();
            foreach (var file in files)
            {
                var webAddress = $"{serverFolder}/{file}";
                var fileName = $"{localComputerFolder}{file}";

                webClient.DownloadFile(webAddress, fileName);
            }
        }
    }
}
