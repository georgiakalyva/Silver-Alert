using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SilverAlert.Shared.Services
{
    public class MissingsService : IMissingsService
    {
        #region Constants

        private const string TimestampParameter = "?timestamp={0}";

        #endregion
        #region Private fields

        private readonly Uri serviceUri;

        #endregion

        #region Constructors

        public MissingsService(string serviceUri)
        {
            if (string.IsNullOrEmpty(serviceUri))
            {
                throw new ArgumentNullException("serviceUri");
            }

            this.serviceUri = new Uri(serviceUri);
        }

        public MissingsService(Uri serviceUri)
        {
            if (serviceUri == null)
            {
                throw new ArgumentNullException("serviceUri");
            }

            this.serviceUri = serviceUri;
        }

        #endregion

        #region IMissingsService Members

        /// <summary>
        /// Gets the latest missings
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>The missings collection in JSON format.</returns>
        public async Task<string> GetLatestMissings(string timestamp = null)
        {
            Uri requestUri = timestamp == null ? this.serviceUri : new Uri(this.serviceUri.AbsoluteUri + string.Format(TimestampParameter, timestamp));

            // Initialize a web request for the service uri
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);

            // Awaits the response
            WebResponse response = await request.GetResponseAsync();

            // Read and returns the response to the caller.
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                string json = await streamReader.ReadToEndAsync();
                return json;
            }
        }

        #endregion
    }
}
