using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SilverAlert.WindowsStore.AppStorage
{
    public class ProfileImage : IValueConverter
    {

        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {

            if (value == null)
            {
                return "";
            }

            return "ms-appdata:///local/" + (String)value;

        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            return value;
        }

    }
}
