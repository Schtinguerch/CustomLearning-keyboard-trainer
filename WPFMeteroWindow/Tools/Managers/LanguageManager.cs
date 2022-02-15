using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow
{
    public static class LanguageManager
    {
        public static readonly List<string> Languages = new List<string>()
        {
            Localization.uLanguageOfSystem,
            "English",
            "Русский",
        };

        private static readonly List<Action> _setLanguageActions = new List<Action>()
        {
            () => Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentCulture,
            () => Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US"),
            () => Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU"),
        };

        public static void SetLanguage(int index)
        {
            _setLanguageActions[index].Invoke();
            Settings.Default.ChosenLanguageIndex = index;
        }
    }
}
