using DiscordRPC;
using DiscordRPC.Logging;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow
{
    public class DiscordManager
    {
        private DiscordRpcClient Client { get; set; }

        public void Initialize()
        {
            Client = new DiscordRpcClient(Settings.Default.DiscordID);
            Client.Logger = new ConsoleLogger()
            {
                Level = LogLevel.Warning
            };

            Client.OnReady += (s, e) =>
            {
                LogManager.Log($"Received ready from user: {e.User.Username}");
            };
            Client.OnPresenceUpdate += (s, e) =>
            {
                LogManager.Log($"Received update: {e.Presence}");
            };

            Client.Initialize();
        }

        public void Update(string lessonName, string status, string imageKey)
        {
            Client.SetPresence(new RichPresence()
            {
                Details = lessonName,
                State = status,
                Assets = new Assets()
                {
                    LargeImageKey = "logolight",
                    LargeImageText = "CustomLearning",
                    SmallImageKey = imageKey
                }
            });
        }

        public void Update()
        {
            Update(Settings.Default.LessonName, "Typing", "");
        }
    }
}
