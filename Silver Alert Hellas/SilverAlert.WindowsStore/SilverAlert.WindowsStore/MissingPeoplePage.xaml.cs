using SilverAlert.Shared;
using SilverAlert.WindowsStore.Common;
using SilverAlert.WindowsStore.DataModel;
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
using SilverAlert.WindowsStore.AppStorage;
using System.Threading.Tasks;
using Windows.UI;
using SilverAlert.WindowsStore.Connectivity;
using SilverAlert.Shared.Services;
using System.Collections.ObjectModel;
using Windows.UI.Popups;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace SilverAlert.WindowsStore
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class MissingPeoplePage : SilverAlert.WindowsStore.Common.LayoutAwarePage
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        ObservableCollection<MissingPerson> oc = new ObservableCollection<MissingPerson>();
        Category navigationParameter;

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public MissingPeoplePage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

        void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            throw new NotImplementedException();
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
            navigationParameter = (Category)e.NavigationParameter;
            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
            // Allow saved page state to override the initial item to display

            
            string FileData = await FileManagement.ReadFile("MissingPeople.json");

            string ResultJson = "[" + FileData + "]";

            List<MissingPerson> Mis = JsonData.MissingPeopleList("el", ResultJson, navigationParameter).OrderByDescending(x => x.DateMissing).ToList<MissingPerson>();

            if (Mis.Count()!=0)
            {
                string SkippedItemsString = AppStorage.SkippedItems.Get();

                String[] Skipped = SkippedItemsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (Skipped.Count() != 0)
                {

                    foreach (var item in Skipped)
                    {
                        Int32 Intitem = Convert.ToInt32(item);
                        var stuffToRemove = Mis.SingleOrDefault(s => s.ID == Intitem);
                        if (stuffToRemove != null)
                        {
                            Mis.Remove(stuffToRemove);
                        }
                    }
                }

                oc = new ObservableCollection<MissingPerson>(Mis);

                itemsViewSource.Source = oc;
            }

            else
            {
                itemGridView.Visibility = Visibility.Collapsed;
                itemsViewSource.Source = oc;
                
                txt.Width = 400;
                txt.Height = 400;
                txt.Foreground = new SolidColorBrush(Colors.White);
                txt.TextWrapping = TextWrapping.WrapWholeWords;
                txt.FontSize = 26;
                txt.TextAlignment = TextAlignment.Center;

                if (Connectivity.Connectivity.ConnectedToTheInternet())
                {
                    txt.Text = "Δεν υπάρχουν αποθηκευμένες εξαφανίσεις. Πατήστε ανανέωση για να ενημερώσετε τα δεδομένα της εφαρμογής";
                }
                else
                {
                    txt.Text = "Δεν υπάρχουν αποθηκευμένες εξαφανίσεις. Συνδεθείτε στο διαδίκτυο και πατήστε ανανέωση για να ενημερώσετε τα δεδομένα της εφαρμογής";
                }

                Grid.SetRow(txt,1);
                Grid.SetColumn(txt, 1);
                txt.Visibility = Visibility.Visible;

            }

           

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

        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter

            var itemId = ((MissingPerson)e.ClickedItem);

            this.Frame.Navigate(typeof(MissingPersonsPage), itemId);


        }


        private async void RefreshBar_Click(object sender, RoutedEventArgs e)
        {
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
                        //result = await ms.GetLatestMissings("2043");
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

                    string ResultJson = "[" + receivedInfo.missingsString + "]";

                    List<MissingPerson> Mis = new List<MissingPerson>();

                    Mis = JsonData.MissingPeopleList("el", ResultJson, navigationParameter).OrderByDescending(x => x.DateMissing).ToList<MissingPerson>();


                    if (Mis.Count() != 0)
                    {
                        txt.Visibility = Visibility.Collapsed;
                        itemGridView.Visibility = Visibility.Visible;
                        string SkippedItemsString = AppStorage.SkippedItems.Get();

                        String[] Skipped = SkippedItemsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        if (Skipped.Count() != 0)
                        {

                            foreach (var item in Skipped)
                            {
                                Int32 Intitem = Convert.ToInt32(item);
                                var stuffToRemove = Mis.SingleOrDefault(s => s.ID == Intitem);
                                if (stuffToRemove != null)
                                {
                                    Mis.Remove(stuffToRemove);
                                }
                            }
                        }
                        int i = 0;
                        foreach (MissingPerson item in Mis)
                        {
                            oc.Insert(i, item);
                            i++;
                        }

                    }
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
