using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WPFMeteroWindow.Properties;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow
{
    public static class QualityManager
    {
        public static Dictionary<string, string> QualityNames = new Dictionary<string, string>()
        {
            { Localization.uLow, "Low" },
            { Localization.uMedium, "Medium" },
            { Localization.uHigh, "High" },
        };

        private static List<Action> _qualitySetupActions = new List<Action>()
        {
            SetLowQuality,
            SetMediumQuality,
            SetHighQuality,
        };

        public static string Quality
        {
            get => Settings.Default.GraphicsQuality;
            set
            {
                int index = -1;
                foreach (var name in QualityNames)
                {
                    index += 1;
                    if (name.Key == value || name.Value == value)
                    {
                        Settings.Default.GraphicsQuality = name.Value;
                        _qualitySetupActions[index].Invoke();
                        return;
                    }
                }

                throw new ArgumentException("Unknown quality name");
            }
        }

        private static void SetLowQuality()
        {
            RenderOptions.SetBitmapScalingMode(Intermediary.App, BitmapScalingMode.LowQuality);
            RenderOptions.SetEdgeMode(Intermediary.App, EdgeMode.Aliased);
            ReloadMouseClickShape();
        }

        private static void SetMediumQuality()
        {
            RenderOptions.SetBitmapScalingMode(Intermediary.App, BitmapScalingMode.Unspecified);
            RenderOptions.SetEdgeMode(Intermediary.App, EdgeMode.Unspecified);
            ReloadMouseClickShape();
        }

        private static void SetHighQuality()
        {
            RenderOptions.SetBitmapScalingMode(Intermediary.App, BitmapScalingMode.HighQuality);
            RenderOptions.SetEdgeMode(Intermediary.App, EdgeMode.Unspecified);
            ReloadMouseClickShape();
        }

        private static void ReloadMouseClickShape() =>
            Intermediary.App.MouseSplashShape.Shape = Intermediary.MouseShapesDictionary[Settings.Default.ChosenClickSplashName];
    }
}
