using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverAlert.Shared
{
    public class City
    {
        public int ID { get; set; }
        public String Description { get; set; }
    }
    public class Cities
    {
        public List<City> CityList { get; set; }
    }
}
