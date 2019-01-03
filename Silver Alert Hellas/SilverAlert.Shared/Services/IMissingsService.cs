using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverAlert.Shared.Services
{
    public interface IMissingsService
    {
        Task<string> GetLatestMissings(string timestamp = null);
    }
}
