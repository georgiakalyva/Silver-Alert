using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilverAlert.DataModel
{

    [Table("Language")]
    public class Language
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public string LanguageName { get; set; }

        [NotNull]
        public string LanguageID { get; set; }
    }

    [Table("EyeColor")]
    public class EyeColor
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public string Color { get; set; }

        [PrimaryKey]
        public string LanguageID { get; set; }
    }

    [Table("BodyType")]
    public class BodyType
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public string Type { get; set; }

        [PrimaryKey]
        public string LanguageID { get; set; }
    }

    [Table("Category")]
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public string CategoryName { get; set; }

        [PrimaryKey]
        public string LanguageID { get; set; }
    }

    [Table("Cities")]
    public class City
    {        
        
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        
        public string CityName { get; set; }

        public string CityCoordinates { get; set; }

        [PrimaryKey]
        public string LanguageID { get; set; }
    }

    [Table("MissingPerson")]
    public class MissingPerson
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [PrimaryKey]
        public string LanguageID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CategoryID { get; set; }
        public int NearestCity { get; set; }
        public string PlaceDescription{ get; set; }
        public string PlaceCoordinates { get; set; }
        public int Age{ get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int EyeColorID { get; set; }
        public int BodyTypeID { get; set; }
        public bool Sex { get; set; }
        public string ImageLink { get; set; }
        public string MissingSince{ get; set; }
        public bool Clothes { get; set; }
        public bool MoreInfo { get; set; }
        
    }
    
}
