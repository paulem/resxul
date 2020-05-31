﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Resxul.Framework.ToastNotifications;

namespace Resxul
{
    // The GUID CLSID must be unique to your app. Create a new GUID if copying this code.
    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(INotificationActivationCallback))]
    [Guid("4BF727D6-C681-4285-8FF7-7E05A331E6BB"), ComVisible(true)]
    public class ResxulNotificationActivator : NotificationActivator
    {
        public override void OnActivated(string invokedArgs, NotificationUserInput userInput, string appUserModelId)
        {
            Application.Current.Dispatcher.Invoke(delegate { });


            //public override void OnActivated(string arguments, NotificationUserInput userInput, string appUserModelId)
            //{
            //    Application.Current.Dispatcher.Invoke(delegate
            //    {
            //        if (arguments.Length == 0)
            //        {
            //            OpenWindowIfNeeded();
            //            return;
            //        }

            //        // Parse the query string (using NuGet package QueryString.NET)
            //        QueryString args = QueryString.Parse(arguments);

            //        // See what action is being requested 
            //        switch (args["action"])
            //        {
            //            // Open the image
            //            case "viewImage":

            //                // The URL retrieved from the toast args
            //                string imageUrl = args["imageUrl"];

            //                // Make sure we have a window open and in foreground
            //                OpenWindowIfNeeded();

            //                // And then show the image
            //                (App.Current.Windows[0] as MainWindow).ShowImage(imageUrl);

            //                break;

            //            // Open the conversation
            //            case "viewConversation":

            //                // The conversation ID retrieved from the toast args
            //                int conversationId = int.Parse(args["conversationId"]);

            //                // Make sure we have a window open and in foreground
            //                OpenWindowIfNeeded();

            //                // And then show the conversation
            //                (App.Current.Windows[0] as MainWindow).ShowConversation();

            //                break;

            //            // Background: Quick reply to the conversation
            //            case "reply":

            //                // Get the response the user typed
            //                string msg = userInput["tbReply"];

            //                // And send this message
            //                ShowToast("Sending message: " + msg);

            //                // If there's no windows open, exit the app
            //                if (App.Current.Windows.Count == 0)
            //                {
            //                    Application.Current.Shutdown();
            //                }

            //                break;

            //            // Background: Send a like
            //            case "like":

            //                ShowToast("Sending like");

            //                // If there's no windows open, exit the app
            //                if (App.Current.Windows.Count == 0)
            //                {
            //                    Application.Current.Shutdown();
            //                }

            //                break;

            //            default:

            //                OpenWindowIfNeeded();

            //                break;
            //        }
            //    });
            //}

            //private void OpenWindowIfNeeded()
            //{
            //    // Make sure we have a window open (in case user clicked toast while app closed)
            //    if (App.Current.Windows.Count == 0)
            //    {
            //        new MainWindow().Show();
            //    }

            //    // Activate the window, bringing it to focus
            //    App.Current.Windows[0].Activate();

            //    // And make sure to maximize the window too, in case it was currently minimized
            //    App.Current.Windows[0].WindowState = WindowState.Normal;
            //}

            //private void ShowToast(string msg)
            //{
            //    // Construct the visuals of the toast
            //    ToastContent toastContent = new ToastContent()
            //    {
            //        // Arguments when the user taps body of toast
            //        Launch = "action=ok",

            //        Visual = new ToastVisual()
            //        {
            //            BindingGeneric = new ToastBindingGeneric()
            //            {
            //                Children =
            //                {
            //                    new AdaptiveText()
            //                    {
            //                        Text = msg
            //                    }
            //                }
            //            }
            //        }
            //    };

            //    var doc = new XmlDocument();
            //    doc.LoadXml(toastContent.GetContent());

            //    // And create the toast notification
            //    var toast = new ToastNotification(doc);

            //    // And then show it
            //    DesktopNotificationManagerCompat.CreateToastNotifier().Show(toast);
            //}
        }
    }
}
