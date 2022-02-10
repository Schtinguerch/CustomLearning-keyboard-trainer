using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using WPFMeteroWindow.Properties;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using TextBox = System.Windows.Controls.TextBox;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using System.Windows.Media.Animation;
using WPFMeteroWindow.Controls;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для ApplicationUserSettingsPage.xaml
    /// </summary>
    public partial class ApplicationUserSettingsPage : Page
    {
        private UIElement _chosenTextBox;
        private bool _loadImage;

        private List<object> _foundControls;
        private int _chosenItemIndex = 0;

        private Storyboard _showHighlightStoryboard;

        private string _defaultColorScheme;

        private string _defaultBackgroundColor;
        private Theme _defaultTheme;

        private readonly List<string> _windowColors = new List<string>()
        {
            "amber",
            "blue",
            "brown",
            "cobalt",
            "crimson",
            "cyan",
            "emerald",
            "green",
            "indigo",
            "lime",
            "magenta",
            "mauve",
            "olive",
            "orange",
            "pink",
            "purple",
            "red",
            "sienna",
            "steel",
            "taupe",
            "teal",
            "violet",
            "yellow"
        };

        private void SetYesOrNo(string target, string text)
        {
            var state = true;

            if (text.ToLower() == Localization.uNo.ToLower())
                state = false;

            switch (target)
            {
                case "WPM":
                    Settings.Default.RequireWPM = state;
                    break;

                case "Stats":
                    Opener.Statistics(state);
                    break;

                case "Hands":
                    Settings.Default.ShowHands = state;
                    break;

                case "Parallax":
                    Settings.Default.EnableParallax = state;
                    break;

                case "BumpTyping":
                    Settings.Default.ShakeBackgroundInTyping = state;
                    break;

                case "BumpClick":
                    Settings.Default.ShakeBackgroundInClicking = state;
                    break;

                case "HideImage":
                    Settings.Default.HideImageWhenLessonStart = state;
                    break;

                case "BlurImage":
                    Settings.Default.BlurUpImageWhenLessonStart = state;
                    break;

                case "BumpKey":
                    Settings.Default.EnableSplashAnimation = state;
                    break;
            }
        }

        private void SetFontFamily(string target)
        {
            var fontDialog = new FontDialog();
            fontDialog.ShowColor = false;
            fontDialog.ShowEffects = false;

            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                var converter = new FontConverter();
                var fontFamily = converter.ConvertToString(fontDialog.Font).Split(new char[] {';'})[0];

                switch (target)
                {
                    case "keyboard":
                        SetFont.Keyboard(fontFamily);
                        break;

                    case "main":
                        SetFont.MainLetters(fontFamily);
                        break;

                    case "UI":
                        SetFont.SummaryLetters(fontFamily);
                        break;
                }
            }
        }
        
        private void ShowColorPicker(UIElement targetControl)
        {
            _chosenTextBox = targetControl;

            var relativePoint = targetControl.TransformToAncestor(wrapPanel).Transform(new Point(0, 0));
            SetupColorPicker.Margin = 
                new Thickness(-206, relativePoint.Y + targetControl.RenderSize.Height, 0, 0);

            SetupColorPicker.IsOpen = true;
        }
        
        public ApplicationUserSettingsPage()
        {
            InitializeComponent();
            WindowColors.Focus();

            _loadImage = false;
            _showHighlightStoryboard = FindResource("HighlightSearchResultStoryboard") as Storyboard;

            WindowColors.Items.Clear();
            foreach (var color in _windowColors)
                WindowColors.Items.Add(color);

            if (Settings.Default.ThemeResourceDictionary.Contains("BaseLight"))
                _defaultTheme = Theme.Light;
            else
                _defaultTheme = Theme.Dark;

            _defaultColorScheme = Settings.Default.ColorSchemeResourceDictionary.Split(new[] {'/'}).Last()
                .Split(new[] {'.'})[0];

            _defaultBackgroundColor = BackGrid.Background.ToString();

            Intermediary.RichPresentManager.Update("Settings", "Configuring the trainer's setup", "");

            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape)
                    PageManager.HidePages();
            };

            if (Settings.Default.CurrentLayout == 1)
                FirstKeyboardLayoutRadioButton.IsChecked = true;
            else
                SecondKeyboardLayoutRadioButton.IsChecked = true;

            ShowHandsTextBox.Text = Settings.Default.ShowHands ? Localization.uYes : Localization.uNo;
            ShowStatisticsTextBox.Text = Settings.Default.ShowStatistics ? Localization.uYes : Localization.uNo;
            EnableParallaxComboBox.Text = Settings.Default.EnableParallax ? Localization.uYes : Localization.uNo;
            EnableBumpAnimationComboBox.Text = Settings.Default.ShakeBackgroundInTyping ? Localization.uYes : Localization.uNo;
            EnableBumpClickAnimationComboBox.Text = Settings.Default.ShakeBackgroundInClicking ? Localization.uYes : Localization.uNo;
            HideImageLessonStartComboBox.Text = Settings.Default.HideImageWhenLessonStart ? Localization.uYes : Localization.uNo;
            BlurImageLessonStartComboBox.Text = Settings.Default.BlurUpImageWhenLessonStart ? Localization.uYes : Localization.uNo;
            EnableKeyboardBumpComboBox.Text = Settings.Default.EnableSplashAnimation ? Localization.uYes : Localization.uNo;

            RequireWPMtextBox.Text = Settings.Default.RequireWPM ? Localization.uYes : Localization.uNo;
            WindowColorType.Text = Settings.Default.IsBackgroundImage ? Localization.uSettImage : Localization.uSettBrush;

            WindowTheme.Text = Settings.Default.ThemeResourceDictionary.ToLower().Contains("light") ? Localization.uLight : Localization.uDark;
            _loadImage = true;

            foreach (var key in Intermediary.KeyboardShapesDictionary.Keys)
                TypingAnimationComboBox.Items.Add(key);

            foreach (var key in Intermediary.MouseShapesDictionary.Keys)
                ClickAnimationComboBox.Items.Add(key);

            TypingAnimationComboBox.Text = Settings.Default.ChosenSplashShapeName;
            ClickAnimationComboBox.Text = Settings.Default.ChosenClickSplashName;

            KeyboardLightingDonut.RepeatForever = true;
            MouseLightingDonut.RepeatForever = true;

            var button = Intermediary.KeyboardData.buttons[29].Copy();
            button.Width = button.Height = 50d;
            button.Margin = new Thickness(0, 10, 0, 0);

            KeyboardDonutGrid.Children.Insert(0, button);

            foreach (var replacement in LessonManager.Exceptions)
                AddReplacement(replacement.Key, replacement.Value);
        }

        private void SetupColorPicker_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            ((TextBox) _chosenTextBox).Text = SetupColorPicker.SelectedColorText;
        }

        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.CurrentLayout == 1)
                KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
            else
                KeyboardManager.LoadKeyboardData(Settings.Default.SecondKeyboardLayoutFile);

            Settings.Default.Save();
            LessonManager.Exceptions = GetReplacementDictionary();
            Intermediary.App.MouseSplashShape.Shape = Intermediary.MouseShapesDictionary[Settings.Default.ChosenClickSplashName];

            PageManager.HidePages();
        }

        private void DiscardButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reload();

            if (Settings.Default.CurrentLayout == 1)
                KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
            else 
                KeyboardManager.LoadKeyboardData(Settings.Default.SecondKeyboardLayoutFile);

            SetColor.ColorScheme(_defaultTheme);
            SetColor.WindowColor(_defaultColorScheme);

            SetFont.MainLetters(Settings.Default.LessonLettersFont);

            if (!Settings.Default.IsBackgroundImage)
                SetColor.FirstColor(_defaultBackgroundColor);

            Intermediary.App.ImageBlurEffect.Radius = Settings.Default.BackgroundBlurRadius.Parse();
            Intermediary.App.MouseSplashShape.Shape = Intermediary.MouseShapesDictionary[Settings.Default.ChosenClickSplashName];
            PageManager.HidePages();
        }

        private void LessonlettersFontSizeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var fontsize = Convert.ToDouble(LessonlettersFontSizeTextBox.Text);
                SetFont.MainLetters_Size(fontsize);
            }
            
            catch
            {
                
            }
        }

        private void ImageBlurSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = ImageBlurSlider.Value;
            ImageBlurTextBox.Text = value.ToString("N");

            Settings.Default.BackgroundBlurRadius = value.ToString();
            Intermediary.App.ImageBlurEffect.Radius = value;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
                PageManager.MakeTrasparency();
        }

        private void Page_MouseUp(object sender, MouseButtonEventArgs e) =>
            PageManager.CancelTransparency();

        private void FirstKeyboardLayoutRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (FirstKeyboardLayoutRadioButton.IsChecked == true)
                Settings.Default.CurrentLayout = 1;
            else 
                Settings.Default.CurrentLayout = 2;
        }

        private void SearchPropertyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchPropertyTextBox.Text.Length < 2) return;

            var allControls = new List<object>();
            foreach (var control in ScrollListStackPanel.Children)
                foreach (var subcontrol in (control as Grid).Children)
                    allControls.Add(subcontrol);

            _foundControls = (
                from ctrl in allControls
                where ContainsText(ctrl, SearchPropertyTextBox.Text)
                select ctrl
            ).ToList();

            if (_foundControls.Count == 0) return;

            wrapPanel.ScrollToVerticalOffset((_foundControls[0] as UIElement).TranslatePoint(new Point(0, 0), TopPositionGrid).Y - 20);
            HighlightControl(_foundControls[0]);
        }

        private void HighlightControl(object control)
        {
            wrapPanel.ScrollToVerticalOffset((control as UIElement).TranslatePoint(new Point(0, 0), TopPositionGrid).Y - 20);
            SearchResultHighLightRectangle.Height = (control as UIElement).RenderSize.Height + 4;

            SearchResultHighLightRectangle.Margin = new Thickness(0, 42, 0, 0);

            if (control.GetType().ToString() == "System.Windows.Controls.TextBlock")
                if ((control as TextBlock).Text == Localization.uSettBaseColors)
                    SearchResultHighLightRectangle.Margin = new Thickness(0, 20, 0, 0);

            _showHighlightStoryboard.Begin();
        }

        private void HighlightNext()
        {
            if (_foundControls == null) return;
            if (_chosenItemIndex >= _foundControls.Count - 1) return;

            _chosenItemIndex++;
            HighlightControl(_foundControls[_chosenItemIndex]);
        }

        private void HighlightPrevious()
        {
            if (_foundControls == null) return;
            if (_chosenItemIndex <= 0) return;

            _chosenItemIndex--;
            HighlightControl(_foundControls[_chosenItemIndex]);
        }

        private void AddReplacement(string key = "", string replacement = "") =>
            ReplacementStackPanel.Children.Add(new ReplaceItem(ReplacementStackPanel, key, replacement));

        private bool ContainsText(object control, string text)
        {
            var containsText = false;
            var controlType = control.GetType().ToString();

            switch (controlType)
            {
                case "System.Windows.Controls.TextBox":
                    return (control as TextBox).Text.ToLower().Contains(text.ToLower());

                case "System.Windows.Controls.TextBlock":
                    return (control as TextBlock).Text.ToLower().Contains(text.ToLower());

                case "System.Windows.Controls.ComboBox":
                    return (control as System.Windows.Controls.ComboBox).Text.ToLower().Contains(text.ToLower());

                case "System.Windows.Controls.StackPanel":
                    var subControls = (control as StackPanel).Children;
                    var containsTheText = false;

                    foreach (var subControl in subControls)
                        containsTheText = containsTheText || ContainsText(subControl, text.ToLower());

                    return containsTheText;
            }

            return containsText;
        }

        private void Page_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Right || e.Key == Key.Down)
            {
                e.Handled = true;
                HighlightNext();
            }

            else if (e.Key == Key.Left || e.Key == Key.Up)
            {
                e.Handled = true;
                HighlightPrevious();
            }
        }

        private void AddReplacementButton_Click(object sender, RoutedEventArgs e) => AddReplacement();

        private Dictionary<string, string> GetReplacementDictionary()
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var child in ReplacementStackPanel.Children)
            {
                var replacementControl = child as ReplaceItem;

                if (string.IsNullOrEmpty(replacementControl.InputKey))
                {
                    Intermediary.App.ShowMessage($"{Localization.uError}: {Localization.uKeyIsEmpty}");
                    continue;
                }

                if (dictionary.TryGetValue(replacementControl.InputKey, out _))
                {
                    Intermediary.App.ShowMessage($"{Localization.uError}: {Localization.uKeyAlreadyIsInDictionary}");
                    continue;
                }

                dictionary.Add(replacementControl.InputKey, replacementControl.Replacement);
            }

            return dictionary;
        }
    }
}
