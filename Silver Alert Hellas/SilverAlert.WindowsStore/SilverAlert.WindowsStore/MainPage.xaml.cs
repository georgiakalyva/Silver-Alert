using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using SilverAlert.WindowsStore.Common;
using SilverAlert.WindowsStore.DataModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.UI.Core;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using SilverAlert.Shared;
using SilverAlert.Shared.Services;
using SilverAlert.WindowsStore.AppStorage;
using SilverAlert.WindowsStore.Connectivity;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace SilverAlert.WindowsStore
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainPage : SilverAlert.WindowsStore.Common.LayoutAwarePage
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        
        Object ID = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Popup"];
        
        public static bool Showed= false;


        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get
            {
                return this.navigationHelper;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            //this.Loaded += page_Loaded;
            //this.Unloaded += page_Unloaded;
        }

                /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
            
                
                // Read data from a simple setting
                if (Connectivity.Connectivity.ConnectedToTheInternet()==false)
                {
                    if (ID != null)
                    {
                        // Create a simple setting
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("Popup");
                        MessageDialog dialog = new MessageDialog("Δεν υπάρχει ενεργή σύνδεση στο διαδίκτυο. Μπορείτε να δείτε τα δεδομένα που είναι αποθηκευμένα σην εφαρμογή.", "Σφάλμα σύνδεσης");
                        await dialog.ShowAsync();
                    }
                }
              
            var sampleDataGroups = SampleDataSource.GetGroups("AllGroups");
            this.DefaultViewModel["Groups"] = sampleDataGroups;
            EnableLiveTile.CreateLiveTile.ShowliveTile();
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
           
        }

        #endregion

        #region UserClicks

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter

            //this.Frame.Navigate(typeof(GroupDetailPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter

            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            if ((String)itemId == "Big-MissingPeoplePage")
            {
                this.Frame.Navigate(typeof(MissingPeoplePage), Category.Missing);
            }
            else if ((String)itemId == "Small-MissingPeoplePage")
            {
                this.Frame.Navigate(typeof(MissingPeoplePage), Category.Uknown);
            }
            else
            {
                this.Frame.Navigate(typeof(WhoWeAre), itemId.ToString());
            }
            
        }

        #endregion



        private async void RefreshBar_Click(object sender, RoutedEventArgs e)
        {
            if (WindowsStore.Connectivity.Connectivity.ConnectedToTheInternet()==true)
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
                    EnableLiveTile.CreateLiveTile.ShowliveTile();

                }
                catch (Exception)
                {

                }
            }
            else if (WindowsStore.Connectivity.Connectivity.ConnectedToTheInternet()==false)
            {
                
                     MessageDialog dialog = new MessageDialog("Δεν υπάρχει ενεργή σύνδεση στο διαδίκτυο.", "Σφάλμα σύνδεσης");
                    await dialog.ShowAsync();
                
               
            }
        }
    }
}
