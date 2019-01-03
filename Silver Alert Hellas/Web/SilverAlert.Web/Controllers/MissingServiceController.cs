using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SilverAlert.Web.Models;

namespace SilverAlert.Web.Controllers
{
    public class MissingServiceController : ApiController
    {
#if DEBUG
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
#else
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["AzureConnectionString"].ConnectionString;
#endif
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> SearchMissings(long? timestamp = null)
        {
            string currentTimestamp = string.Empty;
            IList<string> missings = new List<string>();
            IList<int> found = new List<int>();

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string action = timestamp.HasValue ? "SearchMissings" : "GetMissings";

                using (SqlCommand command = new SqlCommand(action, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (timestamp.HasValue)
                    {
                        command.Parameters.Add("@ClientTimestamp", SqlDbType.Timestamp).Value = BitConverter.GetBytes(timestamp.Value).Reverse().ToArray();
                    }

                    SqlParameter param = new SqlParameter("@CurrentTimestamp", SqlDbType.Timestamp) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(param);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            while (await dataReader.ReadAsync())
                            {
                                byte category = dataReader.GetByte(1);

                                switch (category)
                                {
                                    case 0:
                                        missings.Add(dataReader.GetString(2));
                                        break;
                                    case 1:
                                        found.Add(dataReader.GetInt32(0));
                                        break;
                                    case 2:
                                        missings.Add(dataReader.GetString(2));
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        long retVal = BitConverter.ToInt64(((byte[])param.Value).Reverse().ToArray(), 0);
                        currentTimestamp = retVal.ToString();
                    }
                    catch (Exception ex)
                    {
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    }

                    JsonResult jsonResult = new JsonResult
                    {
                        Data = new
                            {
                                timestamp = currentTimestamp,
                                missings = missings.ToArray(),
                                found = found.ToArray()
                            }
                    };

                    HttpResponseMessage responseMessage = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(new JavaScriptSerializer().Serialize(jsonResult.Data))
                    };

                    return responseMessage;
                }
            }
        }
    }
}
