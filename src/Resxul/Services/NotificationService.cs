using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Resxul.Framework.ToastNotifications;

namespace Resxul.Services
{
    internal class NotificationService
    {
        public NotificationService()
        {

        }

        public void ShowNotification(string description, double amount)
        {
            string xml = $@"<toast>
                      <visual>
                        <binding template='ToastGeneric'>
                          <text>Expense added</text>
                          <text>Description: {description} - Amount: {amount} </text>
                        </binding>
                      </visual>
                    </toast>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            ToastNotification toast = new ToastNotification(doc);

            //ToastNotificationManager.CreateToastNotifier().Show(toast);
            DesktopNotificationManagerCompat.CreateToastNotifier().Show(toast);
        }
    }
}
