using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using Microsoft.PowerPlatform.Dataverse.Client;
﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System.Linq;

namespace CustomWorkflowConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var connectionString = "AuthType=ClientSecret;Url=https://edmsp-dev.crm3.dynamics.com/;ClientId=ddc6f782-786c-442f-89c1-f9ec7a806525;ClientSecret=YOURSECRET";
            var service = new CrmServiceClient(connectionString);
            var portalUrl = GetPowerAppsPortalUrl(service);
            gcnotifypayload payload = new gcnotifypayload
            {
                email_address = "petersonfred613@gmail.com",
                template_id = "ec640977-e0a5-4339-b6ba-41fe5baf5c96",
                personalisation = new Personalisation
                {
                    link = $"https://{portalUrl}",
                    fullname = "Fred Pearson",
                    invitecode = "ABC123"
                }

            };
            string ApiKey = "ApiKey-v1 YOURKEY"; 
            string BaseUrl = "https://api.notification.canada.ca";

            using (HttpClient httpClient = new HttpClient())
            {

                httpClient.BaseAddress = new Uri(BaseUrl);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpClient.DefaultRequestHeaders.Add("Authorization", ApiKey);

                string endpoint = "/v2/notifications/email"; // API endpoint for sending email              

                try
                {
                    var myjson = SerializerWrapper.Serialize<gcnotifypayload>(payload);
                    
                    HttpResponseMessage response = await httpClient.PostAsync(endpoint, new StringContent(myjson,Encoding.UTF8,"application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Email sent successfully.");
                    }
                    else
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        string responseRqeust = await response.RequestMessage.Content.ReadAsStringAsync();
                        Console.WriteLine($"API request failed with status code: {responseBody}  {responseRqeust}");
                       
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }            

                Console.WriteLine(payload);
            }
        }
        public static string GetPowerAppsPortalUrl(IOrganizationService service)
        {
            try
            {
                // Query the ADX_website record to retrieve the portal URL (Replace with the correct query to retrieve URL)
                var query = new QueryExpression("adx_website")
                {
                    ColumnSet = new ColumnSet("adx_primarydomainname"),
                    TopCount = 1
                };

                var websiteEntity = service.RetrieveMultiple(query).Entities.FirstOrDefault();
                if (websiteEntity != null && websiteEntity.Contains("adx_primarydomainname"))
                {
                    return websiteEntity.GetAttributeValue<string>("adx_primarydomainname");
                }
                else
                {
                        throw new InvalidPluginExecutionException("Error in the Get Apps Portals URL Query");
                    
                }
            }
            catch (Exception ex)
            {
                    throw new InvalidPluginExecutionException("Error in custom workflow step - GC notify: " + ex.Message.ToString());
            
            }
        }
    }
    

    [DataContract]
    public class gcnotifypayload
    {
        [DataMember(Name = "email_address")]
        public string email_address { get; set; }

        [DataMember(Name = "template_id")]
        public string template_id { get; set; }

        [DataMember(Name = "personalisation")]
        public Personalisation personalisation { get; set; }

    }

    [DataContract]
    public class Personalisation
    {
        [DataMember(Name = "link")]
        public string link { get; set; }

        [DataMember(Name = "fullname")]
        public string fullname { get; set; }

        [DataMember(Name = "invitecode")]
        public Object invitecode { get; set; }

    }

    public class SerializerWrapper
    {
        public static string Serialize<T>(T srcObject)
        {
            using (MemoryStream SerializeMemoryStream = new MemoryStream())
            {
                //initialize DataContractJsonSerializer object and pass AssessmentRequestStandAloneDTO class type to it
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

                //write newly created object(assessmentRequest) into memory stream
                serializer.WriteObject(SerializeMemoryStream, srcObject);
                string jsonString = Encoding.Default.GetString(SerializeMemoryStream.ToArray());
                return jsonString;
            }
        }

        public static T Deserialize<T>(string jsonObject)
        {
            using (MemoryStream DeSerializeMemoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

                StreamWriter writer = new StreamWriter(DeSerializeMemoryStream);
                writer.Write(jsonObject);
                writer.Flush();
                DeSerializeMemoryStream.Position = 0;

                T deserializedObject = (T)serializer.ReadObject(DeSerializeMemoryStream);
                return deserializedObject;
            }
        }

    }
}
            