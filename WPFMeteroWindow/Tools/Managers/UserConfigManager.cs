using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using WPFMeteroWindow.Properties;
using System.Windows;
using System.IO;
using LmlLibrary;
using Newtonsoft.Json;

namespace WPFMeteroWindow
{
    public static class UserConfigManager
    {
        public static void AddToRecent(string filename)
        {
            var recentSonfigs = AppManager.JsonReadData<List<string>>(Settings.Default.RecentConfigs);
            var hasTheSame = false;

            foreach (var config in recentSonfigs)
                if (filename == config)
                {
                    hasTheSame = true;
                    break;
                }

            if (!hasTheSame)
            {
                recentSonfigs.Add(filename);
                File.WriteAllText(
                    Settings.Default.RecentConfigs,
                    JsonConvert.SerializeObject(recentSonfigs, Formatting.Indented));
            }
        }

        public static void ImportConfigViaExplorer()
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
                ImportConfigFromFile(openFileDialog.FileName);
        }

        public static void ImportConfigFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                LogManager.Log($"Read from \"{filename}\" -> failed: file does not exist");
                return;
            }

            LoadConfig(File.ReadAllText(filename));
            LogManager.Log($"Read from \"{filename}\" -> success");

            AddToRecent(filename);
        }

        public static void ImportConfigFromClipboard() =>
            LoadConfig(Clipboard.GetText());

        public static void LoadConfig(string data)
        {
            var reader = new Lml(data.Replace("\n", ""), Lml.Open.FromString);

            SetFont.SummaryLetters(reader.GetString("UserConfig>Fonts>AppGUIFontFamily"));
            SetFont.Summary_Color(reader.GetString("UserConfig>Fonts>AppGUIFontColor"));

            SetFont.MainLetters(reader.GetString("UserConfig>Fonts>LessonFontFamily"));
            SetFont.MainLetters_Color(reader.GetString("UserConfig>Fonts>LessonFontColor"));
            SetFont.MainLetters_Size(reader.GetString("UserConfig>Fonts>LessonFontSize"));
            SetFont.MainRaidedLetters_Color(reader.GetString("UserConfig>Fonts>RaidedLessonFontColor"));

            SetFont.Keyboard(reader.GetString("UserConfig>Fonts>KeyboardFontFamily"));
            SetFont.Keyboard_Color(reader.GetString("UserConfig>Fonts>KeyboardFontColor"));

            Settings.Default.MenuOpacity = reader.GetString("UserConfig>Opacity>MenuOpacity");
            Settings.Default.HandsOpacity = reader.GetString("UserConfig>Opacity>HandsOpacity");
            Settings.Default.KeyboardOpacity = reader.GetString("UserConfig>Opacity>KeyboardOpacity");

            SetColor.KeyboardBackground(reader.GetString("UserConfig>Keyboard>KeyColor"));
            SetColor.KeyboardBorder(reader.GetString("UserConfig>Keyboard>BorderColor"));
            SetColor.KeyboardHighlight(reader.GetString("UserConfig>Keyboard>HighlightColor"));
            SetColor.KeyboardErrorHighlight(reader.GetString("UserConfig>Keyboard>MistakeColor"));

            SetColor.SecondColor(reader.GetString("UserConfig>AppColors>SecondaryColor"));
            SetColor.CommandLineFirstColor(reader.GetString("UserConfig>AppColors>TextBoxColor"));
            SetColor.CommandLineSecondColor(reader.GetString("UserConfig>AppColors>SecondaryMenuColor"));
            SetColor.ColorScheme(reader.GetString("UserConfig>AppColors>Theme"));
            SetColor.WindowColor(reader.GetString("UserConfig>AppColors>ControlsHighlightColor"));
            Opener.NewTextInputWay(reader.GetString("UserConfig>AppColors>InputTextBox"));

            SetColor.Hands(reader.GetString("UserConfig>Hands>HandsColor"));
            SetColor.HandsThickness(reader.GetString("UserConfig>Hands>HandsThickness"));

            Settings.Default.HandsOpacity = reader.GetString("UserConfig>Opacity>HandsOpacity");
            Settings.Default.ShowHands = reader.GetBool("UserConfig>Hands>ShowHands");

            if (!reader.GetBool("UserConfig>AppColors>HasImageBackground"))
                SetColor.WindowStandardColor();
            else
                SetColor.WindowBackgroundImage(reader.GetString("UserConfig>Wallpaper>PathToImage"));

            SetColor.FirstColor(reader.GetString("UserConfig>AppColors>MainColor"));

            Settings.Default.EnableParallax = reader.GetBool("UserConfig>Animations>EnableParallaxEffect");
            Settings.Default.EnableSplashAnimation = reader.GetBool("UserConfig>Animations>EnabledSplash");

            Settings.Default.ChosenSplashShapeName = reader.GetString("UserConfig>Animations>ChosenKeyboardShape");
            Settings.Default.ChosenClickSplashName = reader.GetString("UserConfig>Animations>ChosenMouseShape");
            Settings.Default.ShakeBackgroundInClicking = reader.GetBool("UserConfig>Animations>EnableClickBump");
            Settings.Default.ShakeBackgroundInTyping = reader.GetBool("UserConfig>Animations>EnableTypeBump");

            Intermediary.App.MouseSplashShape.Shape = Intermediary.MouseShapesDictionary[Settings.Default.ChosenClickSplashName];

            Settings.Default.BackgroundBlurRadius = reader.GetString("UserConfig>Wallpaper>BlurRadius");
            Settings.Default.Save();
        }

        public static void ExportConfigViaExplorer()
        {
            var saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == true)
                ExportConfigToFile(saveFileDialog.FileName);
        }

        public static void ExportConfigToFile(string filename)
        {
            var data = CollectedDataProperties();

            if (!File.Exists(filename))
            {
                LogManager.Log($"Write to \"{filename}\" -> failed: file does not exist");
                return;
            }

            File.WriteAllText(filename, data);
            LogManager.Log($"Write to \"{filename}\" -> success");
        }

        public static void CopyConfigToClipboard() =>
            Clipboard.SetText(CollectedDataProperties());

        private static string CollectedDataProperties()
        {
            var theme = Settings.Default.ThemeResourceDictionary.ToLower().Contains("light") ? "light" : "dark";
            var colorScheme = Settings.Default.ColorSchemeResourceDictionary.Split('/').Last().Replace(".xaml", "");

            var data = "#config\n\n";
            data += $"<<UserConfig:\n";
            data += $"    <<Fonts:\n";
            data += $"        <AppGUIFontFamily {Settings.Default.SummaryFont}>>\n";
            data += $"        <AppGUIFontColor {Settings.Default.SummaryFontColor}>>\n\n";

            data += $"        <LessonFontFamily {Settings.Default.LessonLettersFont}>>\n";
            data += $"        <LessonFontSize {Settings.Default.LessonLettersFontSize}>>\n";
            data += $"        <LessonFontColor {Settings.Default.MainFontColor}>>\n";
            data += $"        <RaidedLessonFontColor {Settings.Default.RaidedFontColor}>>\n\n";

            data += $"        <KeyboardFontFamily {Settings.Default.KeyboardFont}>>\n";
            data += $"        <KeyboardFontColor {Settings.Default.KeyboardFontColor}>>\n";
            data += $"    :Fonts>>\n\n";

            data += $"    <<Opacity:\n";
            data += $"        <MenuOpacity {Settings.Default.MenuOpacity}>>\n";
            data += $"        <KeyboardOpacity {Settings.Default.KeyboardOpacity}>>\n";
            data += $"        <HandsOpacity {Settings.Default.HandsOpacity}>>\n";
            data += $"    :Opacity>>\n\n";

            data += $"    <<Hands:\n";
            data += $"        <ShowHands {Settings.Default.ShowHands}>>\n";
            data += $"        <HandsColor {Settings.Default.HandsColor}>>\n";
            data += $"        <HandsThickness {Settings.Default.HandsThickness}>>\n";
            data += $"    :Hands>>\n\n";

            data += $"    <<Keyboard:\n";
            data += $"        <KeyColor {Settings.Default.KeyboardBackgroundColor}>>\n";
            data += $"        <BorderColor {Settings.Default.KeyboardBorderColor}>>\n";
            data += $"        <HighlightColor {Settings.Default.KeyboardHighlightColor}>>\n";
            data += $"        <MistakeColor {Settings.Default.KeyboardErrorHighlightColor}>>\n";
            data += $"    :Keyboard>>\n\n";

            data += $"    <<AppColors:\n";
            data += $"        <MainColor {Settings.Default.MainBackground}>>\n";
            data += $"        <SecondaryColor {Settings.Default.SecondBackground}>>\n";
            data += $"        <TextBoxColor {Settings.Default.ThirdBackground}>>\n";
            data += $"        <SecondaryMenuColor {Settings.Default.ThirdSecBackground}>>\n";
            data += $"        <Theme {theme}>>\n";
            data += $"        <ControlsHighlightColor {colorScheme}>>\n";
            data += $"        <HasImageBackground {Settings.Default.IsBackgroundImage}>>\n";
            data += $"        <InputTextBox {Settings.Default.TextInputType}>>\n";
            data += $"    :AppColors>>\n\n";

            data += $"    <<Wallpaper:\n";
            data += $"        <PathToImage {Settings.Default.BackgroundImagePath}>>\n";
            data += $"        <BlurRadius {Settings.Default.BackgroundBlurRadius}>>\n";
            data += $"    :Wallpaper>>\n\n";

            data += $"    <<Animations:\n";
            data += $"        <EnableParallaxEffect {Settings.Default.EnableParallax}>>\n";
            data += $"        <EnabledSplash {Settings.Default.EnableSplashAnimation}>>\n";
            data += $"        <ChosenKeyboardShape {Settings.Default.ChosenSplashShapeName}>>\n";
            data += $"        <ChosenMouseShape {Settings.Default.ChosenClickSplashName}>>\n";
            data += $"        <EnableClickBump {Settings.Default.ShakeBackgroundInClicking}>>\n";
            data += $"        <EnableTypeBump {Settings.Default.ShakeBackgroundInTyping}>>\n";
            data += $"    :Animations>>\n";
            data += $":UserConfig>>";

            return data;
        }
    }
}
