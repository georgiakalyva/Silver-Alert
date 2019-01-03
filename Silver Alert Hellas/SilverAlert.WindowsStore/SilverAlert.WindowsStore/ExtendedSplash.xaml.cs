using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using SilverAlert.WindowsStore.Common;
using SilverAlert.Shared;
using Windows.UI.Popups;
using SilverAlert.Shared.Services;
using SilverAlert.WindowsStore.AppStorage;
using Windows.Storage;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SilverAlert.WindowsStore
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplash : Page
    {
        internal Rect splashImageRect; // Rect to store splash screen image coordinates.
        private SplashScreen splash; // Variable to hold the splash screen object.
        internal bool dismissed = false; // Variable to track splash screen dismissal status.
        internal Frame rootFrame;
        private string ConnectionAndDownloadSuccess;
        public ExtendedSplash(SplashScreen splashscreen, bool loadState)
        {
            InitializeComponent();

            // Listen for window resize events to reposition the extended splash screen image accordingly.
            // This is important to ensure that the extended splash screen is formatted properly in response to snapping, unsnapping, rotation, etc...
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(ExtendedSplash_OnResize);

            splash = splashscreen;

            if (splash != null)
            {
                // Register an event handler to be executed when the splash screen has been dismissed.
                splash.Dismissed += new TypedEventHandler<SplashScreen, Object>(DismissedEventHandler);

                // Retrieve the window coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();

                // Optional: Add a progress ring to your splash screen to show users that content is loading
                PositionRing();
            }

            // Create a Frame to act as the navigation context
            rootFrame = new Frame();

            // Restore the saved session state if necessary
            RestoreStateAsync(loadState);
        }

        async void RestoreStateAsync(bool loadState)
        {
            if (loadState)
                await SuspensionManager.RestoreAsync();
        }

        // Position the extended splash screen image in the same location as the system splash screen image.
        void PositionImage()
        {
            extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
            extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
            extendedSplashImage.Height = splashImageRect.Height;
            extendedSplashImage.Width = splashImageRect.Width;

        }

        void PositionRing()
        {
            splashProgressRing.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (splashProgressRing.Width * 0.5));
            splashProgressRing.SetValue(Canvas.TopProperty, (splashImageRect.Y + splashImageRect.Height + splashImageRect.Height * 0.1));
        }

        void ExtendedSplash_OnResize(Object sender, WindowSizeChangedEventArgs e)
        {
            // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
            if (splash != null)
            {
                // Update the coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();
                PositionRing();
            }
        }

        // Include code to be executed when the system has transitioned from the splash screen to the extended splash screen (application's first view).
        async void DismissedEventHandler(SplashScreen sender, object e)
        {
            dismissed = true;

            //AppStorage.ModificationDate.RemoveValue();
            //FileManagement.DeleteFile("MissingPeople.json");
            
            if (WindowsStore.Connectivity.Connectivity.ConnectedToTheInternet())
            {
                string UniqueID = AppStorage.UniqueAppID.SetValue(Guid.NewGuid().ToString());
                
                string ModificationDate = AppStorage.ModificationDate.GetValue();

                string result = "";

                try
                {
                    IMissingsService ms = new MissingsService(AzureUrl.GetUrl());

                    if (ModificationDate != "")
                    {

                        result = await ms.GetLatestMissings(ModificationDate);
                    }
                    else
                    {
                        result = await ms.GetLatestMissings();
                    }

                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                    ReceivedInfo receivedInfo = JsonData.IncomingJson(result);

                    AppStorage.ModificationDate.SaveValue(receivedInfo.timestamp);


                    if (!receivedInfo.missings.Equals(null) && receivedInfo.missingsString != "")
                    {

                        FileManagement.SaveFile("MissingPeople.json", receivedInfo.missingsString);

                        List<string> ImagesList = receivedInfo.ImagesList;

                        if (ImagesList.Count() != 0)
                        {
                            foreach (var imageName in ImagesList)
                            {
                                string[] number = imageName.Split('.');
                                if (Convert.ToInt32(number[0]) >= 0 && Convert.ToInt32(number[0]) <= 37)
                                {
                                    FileManagement.SaveImageAsync(imageName);
                                }      
                                
                            }
                        }
                    }
                    if (receivedInfo.found.Count() != 0)
                    {
                        AppStorage.SkippedItems.Merge(receivedInfo.found);
                    }

                }
                catch (Exception)
                {
                   
                    
                }


            }
            else if (WindowsStore.Connectivity.Connectivity.ConnectedToTheInternet() == false)
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                localSettings.Values["Popup"] = "true";
            }

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                rootFrame.Navigate(typeof(MainPage));
                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            });
        }

        void DismissExtendedSplash()
        {
            if (rootFrame != null)
            { // Navigate to mainpage
                rootFrame.Navigate(typeof(MainPage));
                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
        }

        void DismissSplashButton_Click(object sender, RoutedEventArgs e)
        {
            DismissExtendedSplash();
        }
    }


}


