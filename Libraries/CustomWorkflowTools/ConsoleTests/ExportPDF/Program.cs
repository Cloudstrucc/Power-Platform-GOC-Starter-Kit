using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Query;

namespace ExportPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "AuthType=ClientSecret;Url=https://edmsp-dev.crm3.dynamics.com/;ClientId=ddc6f782-786c-442f-89c1-f9ec7a806525;ClientSecret=YOURSECRET";
            IOrganizationService service = new CrmServiceClient(connectionString);

            ConvertAndSavePDF(service);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void ConvertAndSavePDF(IOrganizationService service)
        {
            string inputFilePath = @"C:\Users\Fred\Desktop\Cloudstrucc Iyvan Resume.docx";


            string outputFilePath = ConvertToPdfWithPandoc(inputFilePath);

            if (!string.IsNullOrEmpty(outputFilePath))
            {
                // Save the PDF to the contact record as an annotation attachment
                Guid contactId = RetrieveContactIdByFullName(service, "FredZip PearsonZip");
                if (contactId != Guid.Empty)
                {
                    SavePdfAsAttachment(service, contactId, outputFilePath);
                    Console.WriteLine("PDF saved as attachment.");
                }
                else
                {
                    Console.WriteLine("Contact 'FredZip PearsonZip' not found.");
                }
            }
            else
            {
                Console.WriteLine("PDF conversion failed.");
            }
        }

        static string ConvertToPdfWithPandoc(string inputFilePath)
        {
            string outputFilePath = Path.ChangeExtension(inputFilePath, "pdf");

            // Extract and save the embedded Pandoc executable
            string pandocExecutablePath = ExtractEmbeddedPandocExecutable();

            if (!string.IsNullOrEmpty(pandocExecutablePath))
            {
                
                 // Build the Pandoc command
        // string arguments = $"--pdf-engine=wkhtmltopdf \"{inputFilePath}\" -o \"{outputFilePath}\"";
         string arguments = $"--pdf-engine=wkhtmltopdf \"{inputFilePath}\" -o \"{outputFilePath}\" --toc";
        

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

                return File.Exists(outputFilePath) ? outputFilePath : null;
            }
            else
            {
                Console.WriteLine("Failed to extract Pandoc executable.");
                return null;
            }
        }



        static string ExtractEmbeddedPandocExecutable()
        {
            // Name of the embedded Pandoc executable resource
            string resourceName = "ExportPDF.pandoc.exe"; // Replace with the actual resource name

            // Extract the resource to a temporary location
            string tempPath = Path.GetTempFileName() + ".exe";
            using (Stream resourceStream = typeof(Program).Assembly.GetManifestResourceStream(resourceName))
            using (FileStream fileStream = new FileStream(tempPath, FileMode.Create))
            {
                resourceStream.CopyTo(fileStream);
            }
            Console.WriteLine($"Extracting Pandoc executable to: {tempPath}");
            return File.Exists(tempPath) ? tempPath : null;
        }


        static Guid RetrieveContactIdByFullName(IOrganizationService service, string fullName)
        {
            // Implement your logic to retrieve contact by full name
            // Example query expression:
            QueryExpression query = new QueryExpression("contact");
            query.Criteria.AddCondition("fullname", ConditionOperator.Equal, fullName);
            query.ColumnSet = new ColumnSet("contactid");

            EntityCollection contacts = service.RetrieveMultiple(query);
            if (contacts.Entities.Count > 0)
            {
                return contacts.Entities[0].Id;
            }
            return Guid.Empty;
        }

        static void SavePdfAsAttachment(IOrganizationService service, Guid contactId, string pdfFilePath)
        {
            // Implement your logic to save PDF as attachment to contact
            // Example code:
            byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);

            Entity attachment = new Entity("annotation");
            attachment["objectid"] = new EntityReference("contact", contactId);
            attachment["objecttypecode"] = "contact";
            attachment["notetext"] = "Converted PDF";
            attachment["filename"] = Path.GetFileName(pdfFilePath);
            attachment["documentbody"] = Convert.ToBase64String(pdfBytes);

            service.Create(attachment);
        }
    }
}
