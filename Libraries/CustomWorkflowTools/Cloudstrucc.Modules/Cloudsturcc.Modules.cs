using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Diagnostics;



namespace GAC.Modules.Steps
{
    public class SendGCNotifyEmailNotificationWorkflowStep : CodeActivity
    {

        [Input("GC Notify Email Template ID")]
        public InArgument<string> EmailTemplateNameOrId { get; set; }

        [Input("Contact Email")]
        public InArgument<string> ContactEmail { get; set; }

        [Input("Full Name")]
        public InArgument<string> FullName { get; set; }

        [Input("InvitationCode")]
        public InArgument<string> InvitationCode { get; set; }

        [Input("GC notify Api Key")]
        public InArgument<string> ApiKey { get; set; }

        [Input("Reference # (e.g. an ID recognizable to the notified user such as a Case ID) - Optional")]
        public InArgument<string> ReferenceNumber { get; set; }

        [Input("Reply to email (set an email that the user can communicate to re. the notifcation - Optional")]
        public InArgument<string> ReplyTo { get; set; }

        protected override async void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            tracingService.Trace("Invoking Post API request");
            try
            {

                var portalUrl = GetPowerAppsPortalUrl(service);


                string reference = this.ReferenceNumber.Get<string>(executionContext);
                string emailReplyToId = this.ReplyTo.Get<string>(executionContext);
                string BaseUrl = "https://api.notification.canada.ca";
                string apiKey = this.ApiKey.Get<string>(executionContext);

                gcnotifypayload payload = new gcnotifypayload
                {
                    email_address = this.ContactEmail.Get<string>(executionContext),
                    template_id = this.EmailTemplateNameOrId.Get<string>(executionContext),
                    personalisation = new Personalisation
                    {
                        link = $"https://{portalUrl}",
                        fullname = this.FullName.Get<string>(executionContext),
                        invitecode = this.InvitationCode.Get<string>(executionContext)
                    }

                };

                using (HttpClient httpClient = new HttpClient())
                {

                    httpClient.BaseAddress = new Uri(BaseUrl);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);

                    string endpoint = "/v2/notifications/email"; // API endpoint for sending email              

                    try
                    {
                        var myjson = SerializerWrapper.Serialize<gcnotifypayload>(payload);
                        HttpResponseMessage response = await httpClient.PostAsync(endpoint, new StringContent(myjson, Encoding.UTF8, "application/json"));
                        string responseBody = await response.Content.ReadAsStringAsync();
                        string responseRequest = await response.RequestMessage.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            tracingService.Trace($"Email sent successfully: {responseBody}  {responseRequest}");
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException($"API request failed with status code: {responseBody}  {responseRequest}");

                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException($"An error occurred: {ex.Message}");
                    }

                    tracingService.Trace("Successfully sent GC notify notification");
                }

            }
            catch (Exception ex)
            {

                throw new InvalidPluginExecutionException("Error in custom workflow step - GC notify: " + ex.Message.ToString());

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

    public class GenerateRandCode : CodeActivity
    {

        [Output("Generated Code")]
        public OutArgument<string> GeneratedCode { get; set; }

        protected override void Execute(CodeActivityContext context)
        {

            var generatedCode = GenerateRandomString();

            // Save the generated code to the record or use it as needed
            GeneratedCode.Set(context, generatedCode);
        }

        private string GenerateRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var code = new StringBuilder(6);

            for (int i = 0; i < 6; i++)
            {
                code.Append(chars[random.Next(chars.Length)]);
            }

            return code.ToString();
        }
    }

    public class ConvertWordToPdf : CodeActivity
    {
        [Input("Word Template")]
        [ReferenceTarget("contact")] // Replace with the entity logical name for the Word template
        public InArgument<EntityReference> WordTemplate { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            EntityReference wordTemplateReference = WordTemplate.Get(context);
            Entity wordTemplate = service.Retrieve(wordTemplateReference.LogicalName, wordTemplateReference.Id, new ColumnSet("documentbody", "filename"));

            if (wordTemplate.Contains("documentbody") && wordTemplate.Contains("filename"))
            {
                byte[] docxBytes = Convert.FromBase64String(wordTemplate.GetAttributeValue<string>("documentbody"));
                string fileName = wordTemplate.GetAttributeValue<string>("filename");

                string pdfFilePath = ConvertToPdfWithPandoc(docxBytes, fileName);

                if (!string.IsNullOrEmpty(pdfFilePath))
                {
                    Entity noteEntity = new Entity("annotation");
                    noteEntity["objectid"] = new EntityReference(wordTemplateReference.LogicalName, wordTemplateReference.Id);
                    noteEntity["objecttypecode"] = wordTemplateReference.LogicalName;
                    noteEntity["subject"] = "Converted PDF";
                    noteEntity["filename"] = Path.ChangeExtension(fileName, "pdf");
                    noteEntity["mimetype"] = "application/pdf";
                    noteEntity["documentbody"] = Convert.ToBase64String(File.ReadAllBytes(pdfFilePath));

                    service.Create(noteEntity);
                }
            }
        }

        private string ConvertToPdfWithPandoc(byte[] docxBytes, string fileName)
        {
            string docxFilePath = Path.Combine(Path.GetTempPath(), fileName);
            string pdfFilePath = Path.ChangeExtension(docxFilePath, "pdf");

            // Save docxBytes to a temporary file
            File.WriteAllBytes(docxFilePath, docxBytes);
           
             // Extract and save the embedded Pandoc executable
            string pandocExecutablePath = ExtractEmbeddedPandocExecutable();
            string arguments = $"--pdf-engine=wkhtmltopdf \"{docxFilePath}\" -o \"{pdfFilePath}\" --toc";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = pandocExecutablePath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }

            return File.Exists(pdfFilePath) ? pdfFilePath : null;
        }
        static string ExtractEmbeddedPandocExecutable()
        {
            // Name of the embedded Pandoc executable resource
            string resourceName = "GAC.Modules.Steps.pandoc.exe"; // Replace with the actual resource name

            // Extract the resource to a temporary location
            string tempPath = Path.GetTempFileName() + ".exe";
            using (Stream resourceStream = typeof(ConvertWordToPdf).Assembly.GetManifestResourceStream(resourceName))
            using (FileStream fileStream = new FileStream(tempPath, FileMode.Create))
            {
                resourceStream.CopyTo(fileStream);
            }
            Console.WriteLine($"Extracting Pandoc executable to: {tempPath}");
            return File.Exists(tempPath) ? tempPath : null;
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

