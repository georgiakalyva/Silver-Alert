using PushNotificationsHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;
using Windows.UI.Core;
using Windows.UI.Notifications;

namespace SilverAlert.WindowsStore.Notifications
{
    public static class Notifications
    {
        public static Windows.Networking.PushNotifications.PushNotificationChannel Channel { get; set; }
        public static PushNotificationsHelper.Notifier Notifier { get; set; }

        private const string PUSH_NOTIFICATIONS_TASK_NAME = "UpdateChannels";
        private const string PUSH_NOTIFICATIONS_TASK_ENTRY_POINT = "PushNotificationsHelper.MaintenanceTask";
        private const int MAINTENANCE_INTERVAL = 10 * 24 * 60; // Check for channels that need to be updated every 10 days

        private static CoreDispatcher dispatcher = null;
        public static async void OpenChannel(string ServerText)
        {
            try
            {
                ChannelAndWebResponse channelAndWebResponse = await Notifier.OpenChannelAndUploadAsync(ServerText);
                //rootPage.NotifyUser("Channel uploaded! Response:" + channelAndWebResponse.WebResponse, NotifyType.StatusMessage);
                //rootPage.Channel = channelAndWebResponse.Channel;
                
            }
            catch (FormatException ex)
            {
            }
        }
        public static async void CloseChannel(string ServerText)
        {
            try
            {
                ChannelAndWebResponse channelAndWebResponse = await Notifier.OpenChannelAndUploadAsync(ServerText);
                //rootPage.NotifyUser("Channel uploaded! Response:" + channelAndWebResponse.WebResponse, NotifyType.StatusMessage);
                //rootPage.Channel = channelAndWebResponse.Channel;
            }
            catch (FormatException ex)
            {
            }
        }
        public static async void RenewChannels()
        {
            try
            {
                // The Notifier object allows us to use the same code in the maintenance task and this foreground application
                await Notifier.RenewAllAsync(true);

            }
            catch (Exception ex)
            {

            }
        }

        public static void RegisterTaskButton_Click()
        {
            if (GetRegisteredTask() == null)
            {
                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                MaintenanceTrigger trigger = new MaintenanceTrigger(MAINTENANCE_INTERVAL, false);
                taskBuilder.SetTrigger(trigger);
                taskBuilder.TaskEntryPoint = PUSH_NOTIFICATIONS_TASK_ENTRY_POINT;
                taskBuilder.Name = PUSH_NOTIFICATIONS_TASK_NAME;

                SystemCondition internetCondition = new SystemCondition(SystemConditionType.InternetAvailable);
                taskBuilder.AddCondition(internetCondition);

                try
                {
                    taskBuilder.Register();
                    //rootPage.NotifyUser("Task registered", NotifyType.StatusMessage);
                }
                catch (Exception ex)
                {
                    //rootPage.NotifyUser("Error registering task: " + ex.Message, NotifyType.ErrorMessage);
                }
            }
            else
            {
                //rootPage.NotifyUser("Task already registered", NotifyType.ErrorMessage);
            }
        }

        public static void UnregisterTask()
        {
            IBackgroundTaskRegistration task = GetRegisteredTask();
            if (task != null)
            {
                task.Unregister(true);
                //rootPage.NotifyUser("Task unregistered", NotifyType.StatusMessage);
            }
            else
            {
                //rootPage.NotifyUser("Task not registered", NotifyType.ErrorMessage);
            }
        }

        public static IBackgroundTaskRegistration GetRegisteredTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks.Values)
            {
                if (task.Name == PUSH_NOTIFICATIONS_TASK_NAME)
                {
                    return task;
                }
            }
            return null;
        }

        private static void AddCallback(PushNotificationChannel currentChannel)
        {
            //PushNotificationChannel currentChannel = rootPage.Channel;
            if (currentChannel != null)
            {
                dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
                currentChannel.PushNotificationReceived += OnPushNotificationReceived;
                //rootPage.NotifyUser("Callback added.", NotifyType.StatusMessage);
            }
            else
            {
                //rootPage.NotifyUser("Channel not open. Open the channel in scenario 1.", NotifyType.ErrorMessage);
            }
        }

        public static void RemoveCallback(PushNotificationChannel currentChannel)
        {

            if (currentChannel != null)
            {
                currentChannel.PushNotificationReceived -= OnPushNotificationReceived;
                //rootPage.NotifyUser("Callback removed.", NotifyType.StatusMessage);
            }
            else
            {
                //rootPage.NotifyUser("Channel not open. Open the channel in scenario 1.", NotifyType.StatusMessage);
            }
        }

        static void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs e)
        {
            string typeString = String.Empty;
            string notificationContent = String.Empty;
            switch (e.NotificationType)
            {
                case PushNotificationType.Badge:
                    typeString = "Badge";
                    notificationContent = e.BadgeNotification.Content.GetXml();
                    break;
                case PushNotificationType.Tile:
                    notificationContent = e.TileNotification.Content.GetXml();
                    typeString = "Tile";
                    break;
                case PushNotificationType.Toast:
                    notificationContent = e.ToastNotification.Content.GetXml();
                    typeString = "Toast";
                    // Setting the cancel property prevents the notification from being delivered. It's especially important to do this for toasts:
                    // if your application is already on the screen, there's no need to display a toast from push notifications.
                    e.Cancel = true;
                    break;
                case PushNotificationType.Raw:
                    notificationContent = e.RawNotification.Content;
                    typeString = "Raw";
                    break;
            }



            string text = "Received a " + typeString + " notification, containing: " + notificationContent;
            var ignored = dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //rootPage.NotifyUser(text, NotifyType.StatusMessage);
            });
        }

        static void StartTilePolling(List<Uri> Uris)
        {
            List<Uri> urisToPoll = new List<Uri>(5);
            foreach (Uri uri in Uris)
            {
                string polledUrl = uri.ToString();

                // The default string for this text box is "http://".
                // Make sure the user has entered some data.
                if (Uri.IsWellFormedUriString(polledUrl, UriKind.Absolute))
                {
                    urisToPoll.Add(new Uri(polledUrl));
                }
                else
                {
                    //rootPage.NotifyUser("Please enter a valid uri to poll.", NotifyType.ErrorMessage);
                }
            }

            PeriodicUpdateRecurrence recurrence = PeriodicUpdateRecurrence.Hour;

            if (urisToPoll.Count == 1)
            {
                TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdate(urisToPoll[0], recurrence);
                //rootPage.NotifyUser("Started polling " + urisToPoll[0].AbsolutePath + ". Look at the application’s tile on the Start menu to see the latest update.", NotifyType.StatusMessage);
            }
            else if (urisToPoll.Count > 1)
            {
                TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdateBatch(urisToPoll, recurrence);
                //rootPage.NotifyUser("Started polling the specified URLs. Look at the application’s tile on the Start menu to see the latest update.", NotifyType.StatusMessage);
            }
            else
            {
                //rootPage.NotifyUser("Specify a URL that returns tile XML to begin tile polling.", NotifyType.ErrorMessage);
            }
        }

        static void StopTilePolling_Click()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().StopPeriodicUpdate();
            //rootPage.NotifyUser("Stopped polling.", NotifyType.StatusMessage);
        }

    }
}
