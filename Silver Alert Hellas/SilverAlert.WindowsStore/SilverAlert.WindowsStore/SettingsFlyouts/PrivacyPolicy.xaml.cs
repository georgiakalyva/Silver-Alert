using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SilverAlert.WindowsStore.SettingsFlyouts
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PrivacyPolicy : SilverAlert.WindowsStore.Common.LayoutAwarePage
    {
        public PrivacyPolicy()
        {
            this.InitializeComponent();
            PolicyText.Text = @"• Privacy Policy

This Privacy Policy explains our policy regarding the collection and use of your information. As we update and expand our products, this policy may change, so please refer back to it periodically. By accessing our website or using our apps, you consent to our information practices. 

• What information do we collect?

When you opt to receive a push notification we collect anonymous information and your location so we can redirect notifications to your device.
We collect anonomous information about how you use our apps.
What do we use your information for? 

• Anonomous information that we collect from you during the use of our apps or website will be used in one of the following ways:

To personalize your experience (your information helps us to better respond to your individual needs)
To improve our website or apps (we continually strive to improve our app offerings based on the information and feedback we receive from you)
Your information, whether public or private, will not be sold, exchanged, transferred, or given to any other company for any reason whatsoever, without your consent, other than for the express purpose of delivering the purchased product or service requested.

• Do we disclose any information to outside parties? 

We do not sell, trade, or otherwise transfer to outside parties your personally identifiable information. This does not include trusted third parties who assist us in operating our website, conducting our business, or servicing you, so long as those parties agree to keep this information confidential. We may also release your information when we believe release is appropriate to comply with the law, enforce our site policies, or protect ours or others rights, property, or safety. However, non-personally identifiable visitor information may be provided to other parties for marketing, advertising, or other uses.

• Third party links

Occasionally, at our discretion, we may include or offer third party products or services on our website. These third party sites have separate and independent privacy policies. We therefore have no responsibility or liability for the content and activities of these linked sites. Nonetheless, we seek to protect the integrity of our site and welcome any feedback about these sites.

• Childrens Online Privacy Protection Act Compliance 

We are in compliance with the requirements of COPPA (Childrens Online Privacy Protection Act), we do not collect any information from anyone under 13 years of age. Our website, products and services are all directed to people who are at least 13 years old or older.

• Your Consent 

By using our site, you consent to our websites privacy policy.

• Changes to our Privacy Policy 

If we decide to change our privacy policy, we will post those changes on this page, and/or update the Privacy Policy modification date below. 

This policy was last modified on 15/4/2013

• Contacting Us 

If there are any questions regarding this privacy policy you may contact us using the information below. 

info@lifelinehellas.gr

";
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void CloseFlyout(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Popup)
                (this.Parent as Popup).IsOpen = false;
            SettingsPane.Show();
        }
    }
}
