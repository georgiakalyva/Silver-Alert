using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SilverAlert.Web.Models;

namespace SilverAlert.Web.Controllers
{
    public class NotificationServiceController : ApiController
    {
        #region Private fields

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

        #endregion
        public async Task<HttpResponseMessage> SendNotifications()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<HttpResponseMessage> RegisterClient([FromBody] NotificationClient notificationClient)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("RegisterClient", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("@NotificationChannel", SqlDbType.VarChar).Value = notificationClient.NotificationChannel.ToString();
                    cmd.Parameters.Add("@ValidUntil", SqlDbType.DateTime).Value = notificationClient.ValidUntil;
                    cmd.Parameters.Add(param);
                    connection.Open();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected != 1)
                    {
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    }

                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(@"{""clientId"": """ + param.Value + @"""}")
                    };
                }
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Update([FromBody] NotificationClient notificationClient)
        {
            if(!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            throw new NotImplementedException();
        }
    }
}
