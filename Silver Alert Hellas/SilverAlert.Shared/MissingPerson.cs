using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilverAlert.Shared
{
    public class MissingPerson
    {
        string name;
        string surname;
        public int? ID { get; set; }
        public String Name
        {
            get
            {
                if (MissingCategory == Category.Uknown)
                {
                    return "ΑΓΝΩΣΤΩΝ";
                }
                else
                {
                    return name;
                }
            }
            set
            {
                name = value;
            }
        }
        public String Surname
        {
            get
            {
                if (MissingCategory == Category.Uknown)
                {
                    return "ΣΤΟΙΧΕΙΩΝ";
                }
                else
                {
                    return surname;
                }
            }
            set
            {
                surname = value;
            }
        }
        public String FullName
        {
            get
            {
                if (MissingCategory == Category.Uknown)
                {
                    return "ΑΓΝΩΣΤΩΝ ΣΤΟΙΧΕΙΩΝ";
                }
                else
                { 
                    return Name + " " + Surname; 
                }
            }
        }
        public int? Age { get; set; }
        public Boolean Sex { get; set; }
        public DateTime DateMissing { get; set; }
        public String TownDescription { get; set; }
        public int? TownMissing { get; set; }
        public String StateMissing { get; set; }
        public String EyeColor { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public String BodyType { get; set; }
        public String Description { get; set; }
        public String Clothes { get; set; }
        public Category MissingCategory { get; set; }
        public Status MissingStatus { get; set; }
        public String Photo { get; set; }
        public String ImageSrc { get
            {
                string[] number = Photo.Split('.');
                if (Convert.ToInt32(number[0]) >= 0 && Convert.ToInt32(number[0]) <= 37)
                {
                    return "ms-appx:///Assets/people/" + Photo;
                } 
            return "ms-appdata:///local/" + Photo;
            }
        }

        public String Date { get { return DateMissing.Date.ToString("dd/MM/yyyy"); } }

        public String WeightOrBodyType
        {
            get
            {
                if (Weight!=null)
                {
                    return Weight.ToString() + " κιλά";
                }
                else
                {
                    return BodyType;
                }
            }
        }
        public string TownMissingName
        {
            get
            {
                List<City> list = JsonData.CitiesList();

                City item = list.Where(a => a.ID == TownMissing).FirstOrDefault();

                if (item!=null)
                {
                    return item.Description;
                }
                else
                {
                    return "";
                }
                
            }
        }

        public String Town { get { return TownDescription + " " + TownMissingName; } }

    }

    public enum Category { Missing, Uknown };
    public enum Status { Missing, Found };

    public class JsonObjects
    {
        object[] miniObjects { get; set; }
    }
}
