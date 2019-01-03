using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverAlert.Shared;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<MissingPerson> dic = JsonData.MissingPeopleList("el","lala");

            ReceivedInfo str = JsonData.IncomingJson("");
            int x = 0;
        }
    }
}
