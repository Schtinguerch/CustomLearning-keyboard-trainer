using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ScriptMaker;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Commands
{
    public class PlaySoundTrack : Command
    {
        public override string Name { get; set; } = "play";
        
        public override void Run(List<string> arguments, object processingObject = null)
        {
            SetAdditional(arguments);
            var mediaElement = processingObject as MediaElement;

            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.Source = new Uri(UnitedStringArgument, UriKind.Absolute);

            mediaElement.Volume = Settings.Default.MusicVolume;
        }
    }
}