using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverAlert.Web.Core.Notifications
{
    public interface IToastNotificationService
    {
        Task NotifyToast();
    }
}
