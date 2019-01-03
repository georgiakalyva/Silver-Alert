using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using SilverAlert.Shared;

namespace SilverAlert.WindowsStore.DataModel
{
    
        /// <summary>
        /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
        /// defines properties common to both.
        /// </summary>
        [Windows.Foundation.Metadata.WebHostHidden]
        public abstract class SampleDataCommon : SilverAlert.WindowsStore.Common.BindableBase
        {
            private static Uri _baseUri = new Uri("ms-appx:///");

            public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
            {
                this._uniqueId = uniqueId;
                this._title = title;
                this._subtitle = subtitle;
                this._description = description;
                this._imagePath = imagePath;
            }

            private string _uniqueId = string.Empty;
            public string UniqueId
            {
                get { return this._uniqueId; }
                set { this.SetProperty(ref this._uniqueId, value); }
            }

            private string _title = string.Empty;
            public string Title
            {
                get { return this._title; }
                set { this.SetProperty(ref this._title, value); }
            }

            private string _subtitle = string.Empty;
            public string Subtitle
            {
                get { return this._subtitle; }
                set { this.SetProperty(ref this._subtitle, value); }
            }

            private string _description = string.Empty;
            public string Description
            {
                get { return this._description; }
                set { this.SetProperty(ref this._description, value); }
            }

            private ImageSource _image = null;
            private String _imagePath = null;
            public ImageSource Image
            {
                get
                {
                    if (this._image == null && this._imagePath != null)
                    {
                        this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                    }
                    return this._image;
                }

                set
                {
                    this._imagePath = null;
                    this.SetProperty(ref this._image, value);
                }
            }

            public void SetImage(String path)
            {
                this._image = null;
                this._imagePath = path;
                this.OnPropertyChanged("Image");
            }

            public override string ToString()
            {
                return this.Title;
            }
        }

        /// <summary>
        /// Generic item data model.
        /// </summary>
        public class SampleDataItem : SampleDataCommon
        {
            public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int ColSpan, int RowSpan, SampleDataGroup group)
                : base(uniqueId, title, subtitle, imagePath, description)
            {
                this._content = content;
                this._ColSpan = ColSpan;
                this._RowSpan = RowSpan;
                this._group = group;
            }

            private string _content = string.Empty;
            public string Content
            {
                get { return this._content; }
                set { this.SetProperty(ref this._content, value); }
            }

            private int _ColSpan = 1;
            public int ColSpan
            {
                get { return this._ColSpan; }
                set { this.SetProperty(ref this._ColSpan, value); }
            }

            private int _RowSpan = 1;
            public int RowSpan
            {
                get { return this._RowSpan; }
                set { this.SetProperty(ref this._RowSpan, value); }
            }

            private SampleDataGroup _group;
            public SampleDataGroup Group
            {
                get { return this._group; }
                set { this.SetProperty(ref this._group, value); }
            }
        }

        /// <summary>
        /// Generic group data model.
        /// </summary>
        public class SampleDataGroup : SampleDataCommon
        {
            public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
                : base(uniqueId, title, subtitle, imagePath, description)
            {
                Items.CollectionChanged += ItemsCollectionChanged;
            }

            private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                // Provides a subset of the full items collection to bind to from a GroupedItemsPage
                // for two reasons: GridView will not virtualize large items collections, and it
                // improves the user experience when browsing through groups with large numbers of
                // items.
                //
                // A maximum of 12 items are displayed because it results in filled grid columns
                // whether there are 1, 2, 3, 4, or 6 rows displayed

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewStartingIndex < 12)
                        {
                            TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                            if (TopItems.Count > 12)
                            {
                                TopItems.RemoveAt(12);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                        {
                            TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                        }
                        else if (e.OldStartingIndex < 12)
                        {
                            TopItems.RemoveAt(e.OldStartingIndex);
                            TopItems.Add(Items[11]);
                        }
                        else if (e.NewStartingIndex < 12)
                        {
                            TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                            TopItems.RemoveAt(12);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldStartingIndex < 12)
                        {
                            TopItems.RemoveAt(e.OldStartingIndex);
                            if (Items.Count >= 12)
                            {
                                TopItems.Add(Items[11]);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        if (e.OldStartingIndex < 12)
                        {
                            TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        TopItems.Clear();
                        while (TopItems.Count < Items.Count && TopItems.Count < 12)
                        {
                            TopItems.Add(Items[TopItems.Count]);
                        }
                        break;
                }
            }

            private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
            public ObservableCollection<SampleDataItem> Items
            {
                get { return this._items; }
            }

            private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
            public ObservableCollection<SampleDataItem> TopItems
            {
                get { return this._topItem; }
            }
        }

        /// <summary>
        /// Creates a collection of groups and items with hard-coded content.
        /// 
        /// SampleDataSource initializes with placeholder data rather than live production
        /// data so that sample data is provided at both design-time and run-time.
        /// </summary>
        public sealed class SampleDataSource
        {
            private static SampleDataSource _sampleDataSource = new SampleDataSource();

            private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
            public ObservableCollection<SampleDataGroup> AllGroups
            {
                get { return this._allGroups; }
            }

            public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
            {
                if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

                return _sampleDataSource.AllGroups;
            }

            public static SampleDataGroup GetGroup(string uniqueId)
            {
                // Simple linear search is acceptable for small data sets
                var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
                if (matches.Count() == 1) return matches.First();
                return null;
            }

            public static SampleDataItem GetItem(string uniqueId)
            {
                // Simple linear search is acceptable for small data sets
                var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
                if (matches.Count() == 1) return matches.First();
                return null;
            }

            public SampleDataSource()
            {
                String ITEM_CONTENT = "";
                #region Strings

                String redbutton = @"Η Γραμμή Ζωής μέσω της υπηρεσίας «Τηλεειδοποίηση», δίνει τη δυνατότητα στους ηλικιωμένους να επικοινωνούν άμεσα με το Συντονιστικό Κέντρο Διαχείρισης Κλήσεων, αμφίδρομα και με ανοιχτή ακρόαση, πατώντας απλά ένα φορητό κόκκινο κουμπί, 24ώρες το 24ώρο και όλες τις ημέρες του χρόνου, χωρίς να χρειάζεται να μετακινηθούν από τη θέση τους ή να επιλέξουν οποιοδήποτε αριθμό.

Πως λειτουργεί το «Kόκκινο Kουμπί»

Ο ηλικιωμένος πατά μία φορά το κόκκινο κουμπί, το οποίο φοράει στο λαιμό ή στον καρπό του.

Η βασική συσκευή, που έχει συνδεθεί με το τηλέφωνο του ηλικιωμένου ενεργοποιείται αυτόματα, δίνοντας σήμα στο κέ ντρο διαχείρισης κλήσεων της ΜΚΟ ΓΡΑΜΜΗ ΖΩΗΣ.

Κατάλληλα εκπαιδευμένο προσωπικό, που έχει άμεση πρόσβαση στο αρχείο κάθε ηλικιωμένου και αποτελείται από κοινωνικούς λειτουργούς, ψυχολόγους, νοσηλευτές, γενικούς ιατρούς και εθελοντές γείτονες, ανταποκρίνεται άμεσα σε οποιαδήποτε ανάγκη ή επιθυμία του.

Ακόμα και αν ο ηλικιωμένος δεν είναι σε θέση να απαντήσει η ΓΡΑΜΜΗ ΖΩΗΣ στέλνει αμέσως βοήθεια, κάποιο γείτονα, μέλος της οικογένειάς του, ασθενοφόρο, την Άμεση Δράση, την Πυροσβεστική.

«Kόκκινο Kουμπί» με αριθμούς

1.276 ηλικιωμένοι έχουν αποκτήσει Δωρεάν το «Kόκκινο Kουμπί» και εξυπηρετούνται σήμερα από την υπηρεσία «Τηλεειδοποίηση».

2.068 ηλικιωμένοι έχουν εξυπηρετηθεί από την υπηρεσία «Τηλεειδοποίηση» από 15 Μαΐου 2009 έως σήμερα.

4.500 και πλέον ηλικιωμένοι, οι οποίοι χρειάζονται το κόκκινο κουμπί, είναι εγγεγραμμένοι σε λίστα αναμονής.

800 και πλέον εθελοντές από όλη την Ελλάδα έχουν ενταχθεί στο πρόγραμμα εθελοντισμού με τον τίτλο «Εθελοντής Γείτονας».

623 από αυτούς τους εθελοντές γείτονες έχουν ταυτοποιηθεί με αντίστοιχο ηλικιωμένο του προγράμματος «Τηλεειδοποίηση» και προσφέρουν απλόχερα τον πολύτιμο χρόνο τους όποτε τους έχει ζητηθεί.";

                String BloodBank = @"Ανταποκρινόμενη στις διαρκώς αυξανόμενες ανάγκες της ευαίσθητης κοινωνικής ομάδας των ηλικιωμένων, η Γραμμή Ζωής επεκτείνει την προσφορά της και στον τομέα της εθελοντικής αιμοδοσίας.

Η Γραμμή Ζωής και η Μονάδα Αιμοδοσίας του Αντικαρκινικού – Ογκολογικού Νοσοκομείου Αθηνών «Ο Άγιος Σάββας», με αίσθημα ευθύνης και σεβασμού απέναντι στους ηλικιωμένους συνανθρώπους μας, ένωσαν τις δυνάμεις τους με σκοπό τη δημιουργία της Πανελλήνιας Τράπεζας Αίματος για την Τρίτη Ηλικία.

Η ιδρυτική εκδήλωση αιμοδοσίας της 6ης Μαρτίου 2011, στέφθηκε με επιτυχία, η προσπάθεια όμως είναι συνεχής και με τη συμβολή όλων αποτελεί έναν ισχυρό και δυναμικό θεσμό εθελοντισμού στην ελληνική κοινωνία.

Η ΜΚΟ Γραμμή Ζωής θεωρεί επιτακτική ανάγκη διάδοσης της ιδέας της εθελοντικής αιμοδοσίας, ως μία πράξη ανθρωπιάς και στοιχειώδους κοινωνικής αλληλεγγύης, ιδίως προς την ευαίσθητη κοινωνική ομάδα των ηλικιωμένων (65 +), η οποία αριθμεί 2.762.000 ανθρώπους στην Ελλάδα.

Από τον Μάρτιο του 2011 έως σήμερα πανελλαδικά έχουμε διαθέσει 367 φιάλες αίμα σε 123 ηλικιωμένους.";

                String PoioiEimaste = @"Η «Γραμμή Ζωής», είναι Μη Κερδοσκοπικός Εθελοντικός Οργανισμός.

                Ιδρύθηκε τον Μάιο του 2006 από τον Γεράσιμο Κουρούκλη, πλαισιωμένο από μία ομάδα ανθρώπων με επιστημονική γνώση και εμπειρία στο χώρο της κοινωνικής μέριμνας, της σωματικής και ψυχικής υγείας, ευαισθητοποιημένους από τα υπάρχοντα προβλήματα στην οργανωμένη και ουσιαστική στήριξη των ηλικιωμένων.

                Σταθεροί στο στόχο μας σχεδιάζουμε, υλοποιούμε και υποστηρίζουμε προγράμματα δράσης, στην κατεύθυνση της εξασφάλισης ποιοτικής και αξιοπρεπούς διαβίωσης της πιο ευαίσθητης κοινωνικής ομάδας, αυτής των ατόμων μεγαλύτερης ηλικίας (65+).

                Φροντίδα, προστασία, αίσθημα ασφάλειας είναι  αυτά που προσπαθούμε να προσφέρουμε στους ηλικιωμένους.

                Εθελοντισμός, ενεργοποίηση της κοινωνίας, αξιοποίηση της τεχνολογίας και της επικοινωνίας, είναι τα μέσα που χρησιμοποιούμε για να το πετύχουμε.

                Η διαρκής υποστήριξη  Συλλόγων, Ιδρυμάτων, Επιχειρήσεων αλλά και των απλών πολιτών, μέχρι σήμερα αποτελεί για εμάς το εφαλτήριο, για να φτάσει το μήνυμα ΖΩΗΣ που εκπέμπουμε, σε όσο το δυνατόν περισσότερα αυτιά και να ευαισθητοποιήσει τους πολίτες και την πολιτεία ακόμη περισσότερο απέναντι στις ανάγκες και τα προβλήματα των ατόμων μεγαλύτερης ηλικίας (της τρίτης και τέταρτης).";
                String TiKanoume = @"-Προγράμματα Κοινωνικής προσφοράς

Τηλεειδοποίηση – το γνωστό πλέον σε όλους ως «Kόκκινο Kουμπί»

Silver Alert – Το Εθνικό συντονιστικό πρόγραμμα έγκαιρης και έγκυρης ειδοποίησης των πολιτών σε περιστατικά εξαφάνισης ηλικιωμένων

SOS 1065 – Εθνική Τηλεφωνική Γραμμή για ηλικιωμένους

116123 – Ευρωπαϊκή Τηλεφωνική Γραμμή Ψυχολογικής Υποστήριξης Ενηλίκων

Πανελλήνια Τράπεζα Αίματος για την τρίτη ηλικία

-Προγράμματα Εθελοντισμού

«Προσφέρουμε τροφή σε εκείνους που…μας ανέθρεψαν» – Πρόγραμμα διανομής τροφίμων

«Κάθε μέρα δίπλα τους» -Πρόγραμμα διανομής τροφίμων

«Yπολόγισε σε μένα» – Σχεδιάσθηκε για να στηρίξει ηλικιωμένους προσφέροντας δωρεάν υπηρεσίες ηλεκτρονικής υποβολής φορολογικών δηλώσεων

Εθελοντής Γείτονας (εθελοντική διάθεση χρόνου και συνδέεται με την Τηλεειδοποίηση)

Εθελοντής Αναζήτησης (εθελοντική διάθεση χρόνου και συνδέεται με το Silver Alert)

Εθελοντής Αιμοδότης (εθελοντική διάθεση χρόνου)";

                String GineE8elontis = @"Στη σημερινή εποχή που έχουν αυξηθεί κατακόρυφα οι άνθρωποι που έχουν σοβαρά αδιέξοδα λόγω έλλειψης χρημάτων, τώρα είναι η κατάλληλη στιγμή για να ανθίσει ο Εθελοντισμός. Άνθρωποι που δεν τους επιτρέπει το σώμα τους πλέον λόγω ηλικίας να καλύπτουν επαρκώς βασικές πτυχές της καθημερινής τους διαβίωσης, περιμένουν από εμάς μια χείρα βοηθείας.

Δυστυχώς τα σύγχρονα οικονομικά και εργασιακά δεδομένα έχουν αφήσει λίγο περισσότερο διαθέσιμο χρόνο σε πολλούς από εμάς και είναι τώρα η ευκαιρία να αφιερωθεί αυτός ο χρόνος σε έναν καλό σκοπό. Ο εθελοντισμός προσφέρει μια διόλου ευκαταφρόνητη ικανοποίηση σε αυτούς που τον παρέχουν και ειδικά εάν προσανατολιστεί προς τους μεγαλύτερη σε ηλικία συνανθρώπους μας, τα συναισθήματα που προκαλεί είναι μοναδικά. Άλλωστε όλοι μας έχουν έναν ηλικιωμένο που στο όνομα του θέλουμε να βρούμε έναν τρόπο να «επιστρέψουμε» το καλό που μας έχει κάνει. 

Ελάτε σε επικοινωνία μαζί μας και εμείς θα σας προτείνουμε τις καταλληλότερες δράσεις για να επιλέξετε εσείς, σύμφωνα με το χρόνο που διαθέτετε και τις δεξιότητές σας.";
                String donate = @"Η ύπαρξη και η δράση της Γραμμής Ζωής εξαρτάται αποκλειστικά από τις δωρεές και τις χορηγίες. Κάνοντας χρηστή χρήση αυτών, προσπαθούμε να φτάσει η βοήθεια και η στήριξη που παρέχουμε σε όσο το δυνατόν περισσότερους ηλικιωμένους συνανθρώπους μας και σε αυτούς που έχουν περισσότερο ανάγκη.

Ακόμα και η πιο μικρή χρηματική δωρεά είναι σημαντική για να εξασφαλίσουμε τη συνεχή μας δράση με στόχο την ανακούφιση των ηλικιωμένων.
 
Σας γνωστοποιούμε ότι,

Η Γραμμή Ζωής δεν χρηματοδοτείται σε καμία περίπτωση από τον κρατικό κορβανά. Σε καμία επίσης περίπτωση δεν πραγματοποιούμε εράνους ή μοιράζουμε κουπόνια πόρτα πόρτα. Όλες οι χρηματικές δωρεές γίνονται με κατάθεση σε συγκεκριμένους τραπεζικούς λογαριασμούς, στα γραφεία μας ή σε κλειστούς χώρους κατά τη διάρκεια εκδηλώσεών μας. Είναι σημαντικό να γνωρίζετε ότι για κάθε δωρεά ή για οποιαδήποτε συναλλαγή ανεξαρτήτως ποσού, εκδίδεται υποχρεωτικά νόμιμη διάτρητη απόδειξη με το λογότυπο και την σφραγίδα του Οργανισμού μας.

Μπορείτε κι εσείς να κάνετε μια δωρεά στη Γραμμή Ζωής. Για τις διαθέσιμες επιλογές επισκεφτείτε την ιστοσελίδα: http://lifelinehellas.gr/";
                String EnProgr = @"
Στη ΜΚΟ Γραμμή Ζωής πιστεύουμε πως οι εταιρίες και τα ιδρύματα αποτελούν οντότητες οι οποίες είναι άρρηκτα συνδεδεμένες με το κοινωνικό σύνολο μέσα στο οποίο δραστηριοποιούνται, επηρεάζοντας και επηρεαζόμενες από τα δεδομένα της εποχής και του χώρου δράσης τους.

Μία εταιρία μπορεί να διακριθεί όχι μόνο για τη στήριξη της οικονομίας μιας χώρας, αλλά και για την ενεργή και έμπρακτη συμμετοχή της στη διάδοση ηθικών αξιών και της ανθρωπιστικής δράσης μέσω της Εταιρικής Κοινωνικής Ευθύνης.

Επίσης, πολλά Κοινωφελή Ιδρύματα με αξιόλογη δράση στην Ελλάδα και το εξωτερικό στηρίζουν οικονομικά, υλικά και ηθικά το έργο του Οργανισμού μας με συνεργασίες που στοχεύουν στην προστασία, την ασφάλεια και την αξιοπρεπή διαβίωση των ηλικιωμένων συνανθρώπων μας.

Ο Οργανισμός μας ταυτίζεται με την κινητοποίηση της Ευρωπαϊκής Ένωσης για τους ηλικιωμένους, εξυπηρετώντας τους σκοπούς μιας απόλυτα ολοκληρωμένης εκστρατείας κοινωνικής ευθύνης με επικεντρωμένους και σαφείς στόχους, τη ζωή και την υγεία των ηλικιωμένων.

Στη ΜΚΟ Γραμμή Ζωής συνεργαζόμαστε με φορείς και εταιρίες που μοιράζονται ένα κοινό όραμα για μια πιο ουσιαστική και αποτελεσματική ανθρωπιστική δράση.

«Υιοθεσίες Ηλικιωμένων»

Συμμετέχοντας στο πρόγραμμα “Υιοθεσία Ηλικιωμένων” κάθε εταιρία, ίδρυμα ή και ιδιώτης έχει τη δυνατότητα να “υιοθετήσει” έναν ή και περισσότερους ηλικιωμένους, προσφέροντας μία και μοναδική φορά το ποσό των 360€ για τον καθένα. Το ποσό αυτό αντιστοιχεί στο κόστος της βασικής συσκευής (Κόκκινο Κουμπί) που συνδέεται με το τηλέφωνο του ηλικιωμένου. Η στήριξη σταματά σε αυτό το σημείο, χωρίς καμία μελλοντική οικονομική ή άλλου είδους ευθύνη από το χορηγό. Διαθέσιμες προτάσεις είναι επίσης τα πακέτα για υιοθεσία

10 ηλικιωμένων στο εφάπαξ κόστος των 3.600 €

20 ηλικιωμένων στο εφάπαξ κόστος των 7.200 €

30 ηλικιωμένων στο εφάπαξ κόστος των 10.800 €

50 ηλικιωμένων στο εφάπαξ κόστος των 18.000 €

Τα ανταποδοτικά οφέλη για το χορηγό καλύπτουν μια ευρεία γκάμα επιλογών και προβολής (ΜΜΕ, φορείς, Δελτία Τύπου, ιστοσελίδες, τηλεοπτικές διαφημίσεις κ.α.)  και είναι πάντα προσαρμοσμένα στις ανάγκες του.

 
Χορηγός στο πρόγραμμα του SILVER ALERT

Το γνωστό σε όλους μας Silver Alert είναι ένα πρόγραμμα το οποίο αποτελεί στην Ελλάδα το μοναδικό τρόπο για την έγκαιρη και έγκυρη δημόσια κοινοποίηση και μετάδοση πληροφοριών, που αφορούν στην εξαφάνιση ηλικιωμένων. Η Ελλάδα, μετά από πρωτοβουλία και στοχευμένες  ενέργειες της ΓΡΑΜΜΗΣ ΖΩΗΣ, είναι η μοναδική χώρα στην Ευρώπη  στην οποία  λειτουργεί το SILVER ALERT.

Όμως, το πρόγραμμα του Silver Alert δεν επιχορηγείται από κανένα κρατικό κονδύλι ή φορέα, ενώ προσφέρει τις υπηρεσίες του δωρεάν στους ωφελουμένους. Στηρίζεται οικονομικά μόνο από δωρεές των συμπολιτών μας και χορηγίες εταιριών. Απασχολεί περί τα 15 άτομα, ενώ το ετήσιο κόστος του ανέρχεται στα 145.000€ και αναζητούμε χορηγούς από Εταιρίες και Ιδρύματα που έχουν υψηλό το αίσθημα της Κοινωνικής Ευθύνης. Η δυνατότητα και η διαβάθμιση των χορηγών είναι ενδεικτικά η εξής:

Αποκλειστικός χορηγός: € 145.000

Μέγας χορηγός: € 100.000

Χρυσός χορηγός: € 70.000

Αργυρός χορηγός: € 50.000

Χορηγός: € 10.000

Υποστηρικτής: € 5.000

Φίλος: € 1.000

 
Συνεργασίες που «αποδίδουν»

Μαζί μπορούμε να αναπτύξουμε συνεργασίες για προκαθορισμένο από κοινού διάστημα, όπου μέρος των εσόδων από μια εκδήλωση ή δραστηριότητα της εταιρίας θα μπορεί να ενισχύσει τη Δράση της Γραμμής Ζωής.

 
Κουτί συλλογής χρημάτων

Με την τοποθέτηση κουτιού συλλογής χρημάτων υπέρ Οργανισμού μας, στο χώρο της εταιρίας σας ή των καταστημάτων σας, μπορείτε να συμβάλλετε στην οικονομική ενίσχυση των Δράσεων του Οργανισμού μας.

 
Υποστήριξη δράσεων και εκδηλώσεων

Οι εκδηλώσεις που πραγματοποιούμε κατά τη διάρκεια του έτους, χρειάζονται και εσάς για να έχουν μεγαλύτερη επιτυχία και απήχηση στο ευρύ κοινό. Μπορείτε να μας υποστηρίξετε με διάφορους τρόπους όπως, με την κάλυψη του κόστους επικοινωνιακού υλικού ή με παραχώρηση χώρου για εκδηλώσεις, καθώς και ως χορηγοί επικοινωνίας.

 
Απλή οικονομική ενίσχυση

Ακόμα και η πιο μικρή χρηματική δωρεά είναι πολύτιμη για τη εκπλήρωση του έργου μας. Προσδοκούμε οι εταιρίες να υποστηρίξουν το έργο μας που αφορά σε προσφορά και συμπαράσταση στους ηλικιωμένους συνανθρώπους μας.

 
Προβολή της Εταιρικής Κοινωνικής Ευθύνης

Η Γραμμή Ζωής, για να ενισχύσει τις Συμπράξεις με τον ιδιωτικό τομέα προσφέρει ανταποδοτικά οφέλη που μπορούν να αναδείξουν την κοινωνική ευαισθησία μιας εταιρίας. Οι δράσεις μας υλοποιούνται μέσα στον Ελλαδικό χώρο και το μήνυμα που φτάνει στην ελληνική κοινωνία, με την κατάλληλη διαχείριση της πληροφορίας, μπορεί να είναι πολύ ισχυρό προς όφελος και της ίδιας της εταιρίας.";
                String links = @"Παρατίθενται σχετικοί σύνδεσμοι και links φορέων που συνδέονται με τη δράση της Γραμμής Ζωής και τη φροντίδα των ηλικιωμένων συνανθρώπων μας.
Σε περίπτωση που θέλετε να προστεθεί και το δικό σας link, μην διστάσετε να επικοινωνήσετε μαζί μας στο promotions@lifelinehellas.gr
 
 
ΥΠΟΥΡΓΕΙΟ ΕΣΩΤΕΡΙΚΩΝ ΔΗΜΟΣΙΑΣ ΔΙΟΙΚΗΣΗΣ ΚΑΙ ΑΠΟΚΕΝΤΡΩΣΗΣ
http://www.ypes.gr
 
ΥΠΟΥΡΓΕΙΟ ΔΗΜΟΣΙΑΣ ΤΑΞΗΣ ΚΑΙ ΠΡΟΣΤΑΣΙΑΣ ΤΟΥ ΠΟΛΙΤΗ
http://www.ydt.gr
 
 
ΥΠΟΥΡΓΕΙΟ ΔΙΚΑΙΟΣΥΝΗΣ, ΔΙΑΦΑΝΕΙΑΣ ΚΑΙ ΑΝΘΡΩΠΙΝΩΝ ΔΙΚΑΙΩΜΑΤΩΝ
http://www.ministryofjustice.gr
 
 
ΕΙΣΑΓΓΕΛΙΑ ΑΡΕΙΟΥ ΠΑΓΟΥ
http://www.eisap.gr
 
 
ΕΦΕΤΕΙΟ ΑΘΗΝΩΝ
http://www.defeteio-ath.gr/
 
 
ΕΙΣΑΓΓΕΛΙΑ ΠΡΩΤΟΔΙΚΩΝ ΑΘΗΝΩΝ
https://www.eispa.gr/opencms/opencms/epa_site
 
 
ΥΠΗΡΕΣΙΑ ΠΟΛΙΤΙΚΗΣ ΑΕΡΟΠΟΡΙΑΣ – ΑΕΡΟΛΙΜΕΝΕΣ
http://www.ypa.gr/content/index2.asp
 
 
ΥΠΟΥΡΓΕΙΟ ΥΓΕΙΑΣ & ΚΟΙΝΩΝΙΚΗΣ ΑΛΛΗΛΕΓΓΥΗΣ
http://www.mohaw.gr
 
 
ΕΛΛΗΝΙΚΗ ΑΣΤΥΝΟΜΙΑ
http://www.hellenicpolice.gr/
 
 
ΠΥΡΟΣΒΕΣΤΙΚΟ ΣΩΜΑ ΕΛΛΑΔΑΣ
http://www.fireservice.gr
 
 
Ε.Κ.Α.Β. ΕΘΝΙΚΟ ΚΕΝΤΡΟ ΑΜΕΣΗΣ ΒΟΗΘΕΙΑΣ
http://www.ekab.gr
 
 
ΓΕΝΙΚΗ ΓΡΑΜΜΑΤΕΙΑ ΠΟΛΙΤΙΚΗΣ ΠΡΟΣΤΑΣΙΑΣ
http://www.civilprotection.gr
 
 
ΒΟΥΛΗ ΤΩΝ ΕΛΛΗΝΩΝ
http://www.parliament.gr
 
 
ΑΡΧΗ ΠΡΟΣΤΑΣΙΑΣ ΔΕΔΟΜΕΝΩΝ
http://www.dpa.gr
 
 
ΕΛΛΗΝΙΚΗ ΨΥΧΙΑΤΡΟΔΙΚΑΣΤΙΚΗ ΕΤΑΙΡΕΙΑ
http://www.psychiatrodikastiki.gr/
 
 
ΕΥΡΩΠΑΪΚΟ ΔΙΚΤΥΟ ΚΑΤΑ ΤΗΣ ΒΙΑΣ
http://antiviolence-net.eu/
 
 
ΣΥΝΗΓΟΡΟΣ ΤΟΥ ΠΟΛΙΤΗ
http://www.synigoros.gr
 
 
Κ.Ε.Π. ΚΕΝΤΡΑ ΕΞΥΠΗΡΕΤΗΣΗΣ ΠΟΛΙΤΩΝ
http://www.kep.gov.gr/
 
 
ΥΠΟΥΡΓΕΙΟ ΥΠΟΔΟΜΩΝ, ΜΕΤΑΦΟΡΩΝ ΚΑΙ ΔΙΚΤΥΩΝ
http://www.yme.gr/
 
 
ΥΠΟΥΡΓΕΙΟ ΝΑΥΤΙΛΙΑΣ ΚΑΙ ΑΙΓΑΙΟΥ – ΛΙΜΕΝΑΡΧΕΙΑ
http://www.yen.gr/
 
 
ΕΛΛΗΝΙΚΗ ΕΤΑΙΡΙΑ ΝΟΣΟΥ ALZHEIMER ΚΑΙ ΣΥΓΓΕΝΩΝ ΔΙΑΤΑΡΑΧΩΝ
http://www.alzheimer-hellas.gr/
 
 
ΚΟΜΒΟΣ ΓΙΑ ΤΗ ΝΟΣΟ ΤΟΥ PARKINSON
http://www.parkinsonportal.gr/
 
 
50KAI ΕΛΛΑΣ
http://www.50plus.gr/
 
 
ΕΛΛΗΝΙΚΗ ΕΤΑΙΡΙΑ ΑΝΟΙΑΣ
http://www.anoiahellas.gr/
 
 
ΠΑΝΕΛΛΗΝΙΑ ΟΜΟΣΠΟΝΔΙΑ ΣΥΛΛΟΓΩΝ ΕΘΕΛΟΝΤΩΝ ΑΙΜΟΔΟΤΩΝ
http://www.posea.gr/
 
 
«ΦΩΣ ΣΤΟ ΤΟΥΝΕΛ» ΑΓΓΕΛΙΚΗ ΝΙΚΟΛΟΥΛΗ
http://www.anikolouli.gr
 
 
ΕΘΝΙΚΟ ΔΙΑΔΗΜΟΤΙΚΟ ΔΙΚΤΥΟ ΥΓΙΩΝ ΠΟΛΕΩΝ – ΠΡΟΑΓΩΓΗΣ ΥΓΕΙΑΣ
http://www.ddy.gr/
 
 
SENIOR CARE – ΦΡΟΝΤΙΔΑ ΓΙΑ ΤΗΝ ΤΡΙΤΗ ΗΛΙΚΙΑ
http://www.seniorcare.gr/
 
 
ΠΕΜΦΗ – ΠΑΝΕΛΛΗΝΙΑ ΕΝΩΣΗ ΜΟΝΑΔΩΝ ΦΡΟΝΤΙΔΑΣ ΗΛΙΚΙΩΜΕΝΩΝ
http://www.pemfi.gr/
 
 
ΜΦΗ ΑΚΤΙΟΣ
http://www.aktios.gr/
 
 
ΜΦΗ ΑΓ.ΓΕΩΡΓΙΟΣ Α.Ε. ΜΟΣΧΑΤΟ
http://www.agge.gr
 
 
ΜΦΗ ΝΕΑ ΘΑΛΠΗ
http://www.neathalpi.gr/";
                String Communic = @"Κοδράτου 4 Μεταξουργείο, Αθήνα

ΤΚ. 10437  

Email: info@lifelinehellas.gr,  

Τηλ: 211-3499700, 210-5246202      

Fax: 210-5246208

 
Έχετε επίσης τη δυνατότητα να επικοινωνήσετε μαζί μας σε πραγματικό χρόνο:

Skype (username: eliaspg).
Facebook (www.facebook.com/Lifelinehellas)
Twitter (@SilverAlertGR)";
                string grammhzwhs = @"Η Εθνική Τηλεφωνική Γραμμή SOS 1065 έχει δοθεί στη διάθεσή της ΜΚΟ Γραμμή Ζωής από την Εθνική Επιτροπή Τηλεπικοινωνιών και Ταχυδρομείων και συμβάλει θετικά σε περιστατικά αναγγελίας εξαφάνισης ηλικιωμένων αλλά και σε περιστατικά καταγγελίας κακομεταχείρισης, κακοποίησης ή παραμέλησης ηλικιωμένων.

Διέπεται από τους παρακάτω περιορισμούς, για την προστασία των ηλικιωμένων :

Όλες οι τηλεφωνικές συνομιλίες είναι απόρρητες και ΔΕΝ καταγράφονται.

Η παρέμβαση σε περιστατικά που αφορούν ηλικιωμένους, γίνεται πάντα αφού ληφθεί υπόψη το ευρύτερο κοινωνικό περιβάλλον τους και οριοθετείται από τη σωστή αποτίμηση των αναγκών τους.

Όσοι ασχολούνται με τη γραμμή υποστήριξης είναι υπεύθυνοι για τη διασφάλιση του υψηλότερου επιπέδου παροχής υπηρεσιών, μέσα στα όρια των διαθέσιμων μέσων και στα επίπεδα που έχουν τεθεί από τους Νόμους.

Τα περιστατικά αντιμετωπίζονται με σεβασμό.

Όλες οι συμβουλευτικές υπηρεσίες της γραμμής υποστήριξης, παρακολουθούνται με περιοδικούς ελέγχους και εκτιμήσεις.

Η Εθνική Τηλεφωνική Γραμμή SOS 1065 απευθύνεται σε:

• Ηλικιωμένους θύματα παραμέλησης ή/και κακομεταχείρισης.

• Ηλικιωμένους θύματα οικονομικής εκμετάλλευσης.

• Ηλικιωμένους θύματα ψυχικής, σωματικής ή/και σεξουαλικής βίας.

• Ηλικιωμένους που αντιμετωπίζουν προβλήματα οικογενειακά ή/και κοινωνικά.

• Ηλικιωμένους που αντιμετωπίζουν προβλήματα από τα παιδιά τους.

• Ηλικιωμένους με σοβαρά προβλήματα υγείας.

Ο σκοπός της Εθνικής Τηλεφωνικής Γραμμή SOS 1065 είναι:

• Η παροχή φροντίδας, προστασίας και ψυχολογικής στήριξης  στους ηλικιωμένους ηλικίας άνω των 65 ετών και η προώθηση των δικαιωμάτων τους αφού οι ίδιοι δεν έχουν τη δυνατότητα ή τα μέσα να αυτοπροστατευθούν.

• Συμβουλευτική σε Ηλικιωμένους, στα παιδιά τους ή/και το συγγενικό τους περιβάλλον.

• Συμβουλευτική σε Ηλικιωμένους για νομικά θέματα

• Μέσω της γραμμής γίνονται δεκτές ανώνυμες και επώνυμες καταγγελίες που αφορούν περιστατικά κακοποίησης / κακομεταχείρισης Ηλικιωμένων, ενώ παράλληλα λειτουργεί και ως εργαλείο συμβουλευτικής ηλικιωμένων, των παιδιών τους και των φροντιστών τους.

• Οι καταγγελίες αποστέλλονται  στις αρμόδιες εισαγγελικές αρχές προς διερεύνηση προκειμένου να αναζητηθούν οι καλύτερες δυνατές λύσεις για τους ηλικιωμένους θύματα, ενώ η συμβουλευτική αφορά συνήθως σε θέματα σχέσεων με τα παιδιά τους, το οικογενειακό τους περιβάλλον και εν γένει αυτούς που τους φροντίζουν.

• Η γραμμή λειτουργεί όλο το 24ωρο, 365 ημέρες το έτος. Στελεχώνεται από κοινωνικούς λειτουργούς, ψυχολόγους, καθώς και ειδικά εκπαιδευμένους εθελοντές.";
                String grammh2 = @"Η Ευρωπαϊκή Τηλεφωνική Γραμμή 116123 έχει δοθεί στη διάθεσή της ΜΚΟ Γραμμή Ζωής από την Εθνική Επιτροπή Τηλεπικοινωνιών και Ταχυδρομείων και συμβάλει θετικά σε περιστατικά ψυχολογικής στήριξης ενηλίκων ιδιαιτέρως δε ηλικιωμένων.

Η Τηλεφωνική Γραμμή 116123 διέπεται από τους παρακάτω περιορισμούς, για την προστασία των Ενηλίκων και των Ηλικιωμένων που χρειάζονται ψυχολογική στήριξη:

Όλες οι τηλεφωνικές συνομιλίες είναι απόρρητες και ΔΕΝ καταγράφονται.

Η παρέμβαση σε περιστατικά που αφορούν ηλικιωμένους, γίνεται πάντα αφού ληφθεί υπόψη το ευρύτερο κοινωνικό περιβάλλον τους και οριοθετείται από τη σωστή αποτίμηση των αναγκών τους.

Όσοι ασχολούνται με τη γραμμή υποστήριξης είναι υπεύθυνοι για τη διασφάλιση του υψηλότερου επιπέδου παροχής υπηρεσιών, μέσα στα όρια των διαθέσιμων μέσων και στα επίπεδα που έχουν τεθεί από τους Νόμους.

Τα περιστατικά αντιμετωπίζονται με σεβασμό.

Όλες οι συμβουλευτικές υπηρεσίες της γραμμής υποστήριξης, παρακολουθούνται με περιοδικούς ελέγχους και εκτιμήσεις.

Έτος 2012

1.160 κλήσεις για περιστατικά ψυχολογικής υποστήριξης

Από 1ο έως 9ο 2013

2.146 κλήσεις για περιστατικά ψυχολογικής υποστήριξης";
                string Food = @"Η οικονομική κρίση έπληξε κυρίως τους ηλικιωμένους. Στη ΜΚΟ Γραμμή ζωής αποφασίσαμε να μη μείνουμε απαθείς.

Δημιουργήσαμε ένα επισιτιστικό μηνιαίο πρόγραμμα για διανομή τροφίμων μακράς διαρκείας με το σύνθημα «Προσφέρουμε τροφή σε εκείνους που…μας ανέθρεψαν».

Επικοινωνήσαμε με αποθήκες τροφίμων αλλά και με παραγωγούς εταιρείες από όλη την Ελλάδα και καταφέραμε από την 1η Δεκεμβρίου 2011 μέχρι σήμερα να παραδίδουμε κάθε μήνα 800 τσάντες γεμάτες με τρόφιμα μακράς διαρκείας σε αντίστοιχους ηλικιωμένους που ζουν κυρίως στα Δυτικά προάστια, αλλά και σε άλλες περιοχές της Αττικής.

Προγράμματα διανομής τροφίμων «Κάθε μέρα δίπλα τους»

300 ηλικιωμένοι που κατοικούν στις εσχατιές των Δυτικών προαστίων της Αττικής, κάθε μήνα παραλαμβάνουν στο σπίτι τους από την Γραμμή Ζωής ένα κιβώτιο βάρους περίπου 20 κιλών που περιλαμβάνει τρόφιμα μακράς διαρκείας όπως:

ελαιόλαδο, φυτίνη, γάλα, ζάχαρη, καφέ, μακαρόνια, όσπρια, ρύζι, αλεύρι, φρυγανιές, μέλι, πελτέ, κλπ.";
                #endregion


                var group1 = new SampleDataGroup("Group-1",
                        "Εξαφανίσεις",
                        "Εξαφανίσεις",
                        "Assets/DarkGray.png",
                        "");
                group1.Items.Add(new SampleDataItem("Big-MissingPeoplePage",
                        "Εξαφανισθέντες",
                        "Εξαφανισθέντες",
                        "Assets/StaticContent/Lost.png",
                        "Assets/StaticContent/Lost.png",
                        ITEM_CONTENT, 30, 30,
                        group1));
                group1.Items.Add(new SampleDataItem("Small-MissingPeoplePage",
                        "Αγνώστων στοιχείων",
                        "Αγνώστων στοιχείων",
                        "Assets/StaticContent/Old-Man.png",
                        "Assets/StaticContent/Old-Man.png"
                        ,ITEM_CONTENT, 15, 15,
                        group1));
                
                
                
                this.AllGroups.Add(group1);

                var group2 = new SampleDataGroup("Group-2",
                        "Γραμμή Ζωής",
                        "Γραμμή Ζωής",
                        "Assets/LightGray.png",
                        "");
                
                group2.Items.Add(new SampleDataItem("Big-WhoWeAre",
                        "Ποιοι είμαστε",
                        "Η «Γραμμή Ζωής», είναι Μη Κερδοσκοπικός Εθελοντικός Οργανισμός.",
                        "Assets/TileLifeline2.png",
                        "Assets/StaticContent/lifeline.jpg", 
                        PoioiEimaste, 30, 30,
                        group2));
                group2.Items.Add(new SampleDataItem("Small-WhatDoWeDo",
                        "Τι κάνουμε",
                        "",
                        "Assets/StaticContent/Double-Check.png",
                        "Assets/StaticContent/2401.jpg",
                        TiKanoume, 15, 15,
                        group2));
                group2.Items.Add(new SampleDataItem("Small-Group-2-Item-3",
                        "Γίνε εθελοντής",
                        "",
                        "Assets/StaticContent/Load-Man.png",
                        "Assets/StaticContent/ethelontis.png",
                        GineE8elontis, 15, 15,
                        group2));
                group2.Items.Add(new SampleDataItem("Small-Group-2-Item-4",
                       "Ενίσχυση",
                       "Οικονομική Ενίσχυση",
                       "Assets/StaticContent/Donate .png",
                       "Assets/StaticContent/papous1.jpg",
                       donate, 15, 15,
                       group2));
                group2.Items.Add(new SampleDataItem("Small-Group-2-Item-5",
                        "Στηρίξτε μας",
                        "Ενίσχυση Προγραμμάτων",
                        "Assets/StaticContent/Business-Man-Add-01.png",
                        "Assets/StaticContent/giagia (1).jpg",
                        EnProgr, 15, 15,
                        group2));
                
                group2.Items.Add(new SampleDataItem("Small-Group-2-Item-7",
                        "Σχετικά links",
                        "",
                        "Assets/StaticContent/Web.png",
                        "",
                        links, 15, 15,
                        group2));
              
                group2.Items.Add(new SampleDataItem("Small-Group-2-Item-9",
                        "Γραμμή SOS 10-65",
                        "Εθνική Τηλεφωνική Γραμμή SOS 1065",
                        "Assets/StaticContent/247.png",
                        "Assets/Logo2.png",
                        grammhzwhs, 15, 15,
                        group2));
                group2.Items.Add(new SampleDataItem("Small-Group-2-Item-10",
                        "Κόκκινο κουμπί",
                        "Τηλεειδοποίηση «Kόκκινο Kουμπί»",
                        "Assets/StaticContent/Button.png",
                        "Assets/StaticContent/red.JPG",
                        redbutton, 15, 15,
                        group2));
                group2.Items.Add(new SampleDataItem("Small-Group-2-Item-11",
                        "Τράπεζα Αίματος",
                        "Πανελλήνια Τράπεζα Αίματος για την Τρίτη Ηλικία",
                        "Assets/StaticContent/Blood.png",
                        "Assets/StaticContent/bloodbankpic.png",
                        BloodBank, 15, 15,
                        group2));
                

                    group2.Items.Add(new SampleDataItem("Small-Group-2-Item-12",
                        "Γραμμή 116123",
                        "Ευρωπαϊκή Τηλεφωνική Γραμμή Ψυχολογικής Υποστήριξης Ενηλίκων",
                        "Assets/StaticContent/Telephone.png",
                        "Assets/StaticContent/grammi.png",
                        grammh2, 15, 15,
                        group2));

                    group2.Items.Add(new SampleDataItem("Small-Group-2-Item-13",
                            "Διανομή Τροφίμων",
                            "Προγράμματα διανομής τροφίμων «Προσφέρουμε τροφή σε εκείνους που…μας ανέθρεψαν»",
                            "Assets/StaticContent/Box.png",
                            "Assets/StaticContent/food.png",
                            Food, 15, 15,
                            group2));

                group2.Items.Add(new SampleDataItem("Small-Group-2-Item-8",
                      "Επικοινωνία",
                      "Χάρτης",
                      "Assets/StaticContent/Send To.png",
                      "Assets/StaticContent/location.png",
                      Communic, 15, 15,
                      group2));
                
                this.AllGroups.Add(group2);

               
            }

            public List<MissingPerson> MissingPeopleSource()
            {
                List<MissingPerson> Mis = new List<MissingPerson>();

                MissingPerson person = new MissingPerson();

                person.Name = "PersonName";

                return Mis;
            }
        }
    }

