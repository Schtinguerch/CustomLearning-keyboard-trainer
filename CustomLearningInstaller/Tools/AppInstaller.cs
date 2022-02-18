using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using IWshRuntimeLibrary;

namespace CustomLearningInstaller
{
    public delegate void ExtractionCompletedHandler();

    public static class AppInstaller
    {
        public static event DownloadProgressChangedEventHandler DownloadProgressChanged;
        public static event AsyncCompletedEventHandler DownloadFileCompleted;
        public static event ExtractionCompletedHandler ExtractionCompleted;

        private static string _archivePath = "https://github.com/Schtinguerch/schtinguerch.github.io/raw/master/MissingPackages/AppFullPackage.zip";

        public static string DestinationFolder { get; set; }
        public static bool CreateDesktopShortcut { get; set; }
        public static bool AddShortcutToStart { get; set; }

        public static string ArchiveFilename => $"{DestinationFolder}\\AppFullPackpage.zip";
        public static string ApplicationFilename => $"{DestinationFolder}\\CustomLearning.exe";
        public static string SetupFilename => $"{DestinationFolder}\\CustomLearningSetup.exe";

        public static void DownloadArchive()
        {
            var client = new WebClient();

            client.DownloadProgressChanged += (s, e) => DownloadProgressChanged?.Invoke(s, e);
            client.DownloadFileCompleted += (s, e) => DownloadFileCompleted?.Invoke(s, e);

            Directory.CreateDirectory(DestinationFolder);
            var files = Directory.GetFiles(DestinationFolder, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
                System.IO.File.Delete(file);

            client.DownloadFileAsync(new Uri(_archivePath), ArchiveFilename);
        }

        public static void ExtractArchive()
        {
            ZipFile.ExtractToDirectory(ArchiveFilename, DestinationFolder);
            ExtractionCompleted?.Invoke();
        }

        public static void DoAdditionalTasks()
        {
            if (AddShortcutToStart)
            {
                var commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
                var appStartMenuPath = Path.Combine(commonStartMenuPath, "Programs", "CustomLearning");

                if (!Directory.Exists(appStartMenuPath))
                    Directory.CreateDirectory(appStartMenuPath);

                CreateShortcut(appStartMenuPath);
            }

            if (CreateDesktopShortcut)
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                CreateShortcut(desktopPath);
            }

            RegisterInControlPanel();
        }

        public static void RegisterInControlPanel()
        {
            CopyInstallerToAppFolder();
            AddUninstallerToRegistry();
        }

        private static void CreateShortcut(string appStartPath)
        {
            var shortcutLocation = Path.Combine(appStartPath, "CustomLearning.lnk");
            var shell = new WshShell();

            var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
            shortcut.Description = "Customizable touch typing trainer";
            shortcut.TargetPath = ApplicationFilename;
            shortcut.WorkingDirectory = DestinationFolder;

            shortcut.Save();
        }

        private static void CopyInstallerToAppFolder()
        {
            var installerPath = Environment.GetCommandLineArgs()[0];

            if (installerPath == SetupFilename) return;
            System.IO.File.Copy(installerPath, SetupFilename);
        }

        private static void AddUninstallerToRegistry()
        {
            using (var parent = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
            {
                if (parent == null)
                {
                    throw new Exception("Uninstall registry key not found.");
                }
                try
                {
                    RegistryKey key = null;

                    try
                    {
                        var guidText = "CustomLearning";
                        key = parent.OpenSubKey(guidText, true) ??
                              parent.CreateSubKey(guidText);

                        if (key == null)
                        {
                            throw new Exception(String.Format("Unable to create uninstaller '{0}'", guidText));
                        }

                        key.SetValue("DisplayName", "CustomLearning");
                        key.SetValue("Publisher", "Schtinguêrch");
                        key.SetValue("URLInfoAbout", "https://github.com/Schtinguerch/CustomLearning-keyboard-trainer");
                        key.SetValue("Contact", "schtinguerch@gmail.com");
                        key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
                        key.SetValue("UninstallString", SetupFilename + " /uninstall");
                    }

                    finally
                    {
                        if (key != null)
                        {
                            key.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        "An error occurred writing uninstall information to the registry.  The service is fully installed but can only be uninstalled manually through the command line.",
                        ex);
                }
            }
        }
    }
}
