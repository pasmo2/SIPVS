using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Collections.Generic;
using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.X509;
using System.Security.Cryptography.Xml;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto;
using WindowsFormsApp1;
using Org.BouncyCastle.Tsp;

public class AddressInformation
{
    public string City { get; set; }
    public string CityDistrict { get; set; }
    public string StreetName { get; set; }
    public int StreetNumber { get; set; }
    public string ZipCode { get; set; }
}

public class ContactInformation
{
    public string Phone { get; set; }
    public string Email { get; set; }
}

public class PersonalInformation
{
    public string FullName { get; set; }
    public string Degree { get; set; }
}

public class Employer
{
    public PersonalInformation personInfo { get; set; }
    public AddressInformation address { get; set; }
    public string LegalForm { get; set; }
    public string ICO { get; set; }
    public string DIC { get; set; }
    public ContactInformation contact { get; set; }
}

public class Candidate
{
    public PersonalInformation personInfo { get; set; }
    public AddressInformation address { get; set; }
    public ContactInformation contact { get; set; }
}

public class FormPageModel : PageModel
{
    public string outputPath { get; set; }
    private readonly ILogger<FormPageModel> _logger;
    private readonly IWebHostEnvironment _env;

    public FormPageModel(ILogger<FormPageModel> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    [BindProperty]
    public Employer employer { get; set; } = new Employer();

    [BindProperty]
    public Candidate candidate { get; set; } = new Candidate();

    [BindProperty]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [BindProperty]
    public string AttachmentsString { get; set; } = string.Empty;

    public List<string> Attachments { get; set; } = new List<string>();

    public string XmlOutput { get; set; } = string.Empty;
    public string ValidationResult { get; set; } = string.Empty;

    public async Task<IActionResult> OnPost(string action)
    {
        Attachments = string.IsNullOrWhiteSpace(AttachmentsString)
            ? new List<string>() // If AttachmentsString is null or empty, return an empty list
            : AttachmentsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim())
                .ToList();

        XNamespace ns = "http://www.example.com/job-application";

        XElement finalXml = new XElement(ns + "jobApplication",
            new XElement(ns + "employer",
                new XElement(ns + "person",
                    new XElement(ns + "fullName", employer.personInfo.FullName),
                    new XElement(ns + "degree", employer.personInfo.Degree),
                    new XAttribute(ns + "relation", "employer")
                ),
                new XElement(ns + "address",
                    new XElement(ns + "city", employer.address.City),
                    new XElement(ns + "cityDistrict", employer.address.CityDistrict),
                    new XElement(ns + "streetName", employer.address.StreetName),
                    new XElement(ns + "streetNumber", employer.address.StreetNumber),
                    new XElement(ns + "zipCode", employer.address.ZipCode),
                    new XAttribute(ns + "residentialType", "permanent address")
                ),
                new XElement(ns + "contact",
                    new XElement(ns + "phone", employer.contact.Phone),
                    new XElement(ns + "email", employer.contact.Email)
                ),
                new XAttribute(ns + "ico", employer.ICO ?? "not provided"),
                new XAttribute(ns + "dic", employer.DIC ?? "not provided"),
                new XAttribute(ns + "legalForm", employer.LegalForm ?? "not provided")
            ),
            new XElement(ns + "candidate",
                new XElement(ns + "person",
                    new XElement(ns + "fullName", candidate.personInfo.FullName),
                    new XElement(ns + "degree", candidate.personInfo.Degree),
                    new XAttribute(ns + "relation", "candidate")
                ),
                new XElement(ns + "address",
                    new XElement(ns + "city", candidate.address.City),
                    new XElement(ns + "cityDistrict", candidate.address.CityDistrict),
                    new XElement(ns + "streetName", candidate.address.StreetName),
                    new XElement(ns + "streetNumber", candidate.address.StreetNumber),
                    new XElement(ns + "zipCode", candidate.address.ZipCode),
                    new XAttribute(ns + "residentialType", "permanent address")
                ),
                new XElement(ns + "contact",
                    new XElement(ns + "phone", candidate.contact.Phone),
                    new XElement(ns + "email", candidate.contact.Email)
                )
            ),
            new XElement(ns + "startDate", StartDate.ToString("yyyy-MM-dd")),
            new XElement(ns + "attachments",
                Attachments.Any()
                    ? Attachments.Select(att => new XElement(ns + "attachment", att))
                    : new XElement(ns + "attachment", "not_provided")
            )
        );

        string xmlData = finalXml.ToString();

        if (action == "save")
        {
            string outputDir = Path.Combine(_env.WebRootPath, "output");
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            outputPath = Path.Combine(outputDir, "jobApplication.xml");
            using (StreamWriter writer = new StreamWriter(outputPath)) writer.Write(xmlData);
        }
        else if (action == "check")
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add("http://www.example.com/job-application", Path.Combine(_env.ContentRootPath, "wwwroot/schemas/jobApplication.xsd"));
            settings.ValidationType = ValidationType.Schema;

            using (StringReader stringReader = new StringReader(xmlData))
            using (XmlReader reader = XmlReader.Create(stringReader, settings))
            {
                try
                {
                    while (reader.Read()) { }
                    ValidationResult = "XML is valid!";
                    return Page();
                }
                catch (XmlSchemaValidationException ex)
                {
                    ValidationResult = $"Validation failed: {ex.Message}";
                    _logger.LogError($"XML validation exception: {ex.Message}");
                    return Page();
                }
            }
        }
        else if (action == "export")
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            string outputDir = Path.Combine(_env.WebRootPath, "output");
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            outputPath = Path.Combine(outputDir, "jobApplication.html");

            try
            {
                xslt.Load(Path.Combine(_env.ContentRootPath, "wwwroot/schemas/transform.xsl"));
                using (StringReader stringReader = new StringReader(xmlData))
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                using (StreamWriter writer = new StreamWriter(outputPath)) xslt.Transform(xmlReader, null, writer);

                Console.WriteLine($"HTML transformation successful! Output saved to: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during transformation: {ex.Message}");
            }
        }
    else if (action == "timestamp")
        {
        string asicFilePath = Path.Combine("", "signed_jobapplication.asice");

        if (!System.IO.File.Exists(asicFilePath))
        {
            ModelState.AddModelError("file", "The ASiC file does not exist at the expected location.");
            return Page();
        }

        byte[] asicContent = await System.IO.File.ReadAllBytesAsync(asicFilePath);

        try
        {
            //DONE
            byte[] signatureContent = GetSignatureFromAsic(asicContent);

            //DONE
            byte[] timestampToken = await GetTimestampTokenAsync(signatureContent);

            //add timestamp to ASIC container
            byte[] asicWithTimestamp = AddTimestampToAsice(asicContent, timestampToken);

            //return updated ASIC file
            string outputDir = Path.Combine(_env.WebRootPath, "output");
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            string outputPath = Path.Combine(outputDir, "jobApplication_with_timestamp.asice");
            return File(asicWithTimestamp, "application/zip", "jobApplication_with_timestamp.asice");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("timestamp", $"An error occurred while adding the timestamp: {ex.Message}");
            Console.Write(ex.Message);
            return Page();
        }
    }

    return Page();
}

    //DONE
    private static byte[] GetSignatureFromAsic(byte[] asicFile)
{
    using (MemoryStream ms = new MemoryStream(asicFile))
    using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Read))
    {

        var signaturesXmlEntry = zip.GetEntry("META-INF/signatures.xml");

        if (signaturesXmlEntry != null)
        {
            using (var entryStream = signaturesXmlEntry.Open())
            using (var reader = new XmlTextReader(entryStream))
            {
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            XmlNode signatureValueNode = doc.SelectSingleNode("//ds:SignatureValue", nsManager);
            
            if (signatureValueNode != null)
            {
                Console.WriteLine("SignatureValue found: " + signatureValueNode.InnerText);
                byte[] signatureData = Convert.FromBase64String(signatureValueNode.InnerText);
                return signatureData;
            }
            else
            {
                Console.WriteLine("SignatureValue node not found. Printing entire XML:");
                //Console.WriteLine(doc.OuterXml);
                return null;
            }
            }
        }
    }

    throw new Exception("Signature not found in the ASiC container.");
}

    //DONE
    private static async Task<byte[]> GetTimestampTokenAsync(byte[] dataToTimestamp)
    {
        Org.BouncyCastle.Tsp.TimeStampRequestGenerator tsRequestGenerator = new Org.BouncyCastle.Tsp.TimeStampRequestGenerator(); // certificate generator
		tsRequestGenerator.SetCertReq(true);
		Org.BouncyCastle.Tsp.TimeStampRequest tsRequest = tsRequestGenerator.Generate(Org.BouncyCastle.Tsp.TspAlgorithms.Sha256, dataToTimestamp); // vygenerujeme request

		Timestamp ts = new Timestamp();
		byte[] responseBytes = ts.GetTimestamp(tsRequest.GetEncoded(), "https://test.ditec.sk/TSAServer/tsa.aspx");

		Org.BouncyCastle.Tsp.TimeStampResponse tsResponse = new Org.BouncyCastle.Tsp.TimeStampResponse(responseBytes);
        return tsResponse.GetEncoded();
    }

    //add timestamp token to ASIC container
     private static byte[] AddTimestampToAsice(byte[] asiceFileData, byte[] timestampResponse)
    {
        //memory stream to hold the new .asice content
        using (var outputStream = new MemoryStream())
        {
            //zip archivebased on the original byte array
            using (var asiceStream = new MemoryStream(asiceFileData))
            using (var archive = new ZipArchive(asiceStream, ZipArchiveMode.Read))
            {
                //new Zip to output
                using (var newArchive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    //iterate original .asice file
                    foreach (var entry in archive.Entries)
                    {
                        //copy the original entries into new archive okrem signatures.xml
                        if (entry.FullName != "META-INF/signatures.xml")
                        {
                            //copy original entries to the new archive
                            var newEntry = newArchive.CreateEntry(entry.FullName);
                            using (var entryStream = entry.Open())
                            using (var newEntryStream = newEntry.Open())
                            {
                                entryStream.CopyTo(newEntryStream);
                            }
                        }
                    }

                    //extract the signatures.xml from original
                    var signatureEntry = archive.GetEntry("META-INF/signatures.xml");
                    if (signatureEntry != null)
                    {
                        //load original signatures.xml
                        XmlDocument signaturesXml = new XmlDocument();
                        using (var signatureStream = signatureEntry.Open())
                        {
                            signaturesXml.Load(signatureStream);
                        }

                        //SignatureTimestamp to UnsignedSignatureProperties
                        AddSignatureTimestamp(signaturesXml, timestampResponse);

                        //save modified signatures.xml 
                        var newSignatureEntry = newArchive.CreateEntry("META-INF/signatures.xml");
                        using (var entryStream = newSignatureEntry.Open())
                        using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
                        {
                            signaturesXml.Save(writer);
                        }
                    }
                    else
                    {
                        throw new Exception("signatures.xml not found in the .asice file.");
                    }
                }
            }

            return outputStream.ToArray();
        }
    }

    private static void AddSignatureTimestamp(XmlDocument signaturesXml, byte[] timestampResponse)
{
    string timestampBase64 = Convert.ToBase64String(timestampResponse);

    XmlElement signatureTimeStamp = signaturesXml.CreateElement("xades", "SignatureTimeStamp", "http://www.w3.org/2000/09/xmldsig#");
    signatureTimeStamp.SetAttribute("Id", Guid.NewGuid().ToString()); // Using GUID for unique Id
    
    XmlElement encapsulatedTimeStamp = signaturesXml.CreateElement("xades", "EncapsulatedTimeStamp", "http://www.w3.org/2000/09/xmldsig#");
    encapsulatedTimeStamp.InnerText = timestampBase64;
    signatureTimeStamp.AppendChild(encapsulatedTimeStamp);

    // Locate the QualifyingProperties element
    XmlElement qualifyingProperties = signaturesXml.SelectSingleNode("//xades:QualifyingProperties", GetNamespaceManager(signaturesXml)) as XmlElement;
    if (qualifyingProperties == null)
    {
        throw new InvalidOperationException("QualifyingProperties element not found in the XML.\n");
    }

    XmlElement unsignedProperties = signaturesXml.SelectSingleNode("//xades:UnsignedProperties", GetNamespaceManager(signaturesXml)) as XmlElement;
    if (unsignedProperties == null)
    {
        unsignedProperties = signaturesXml.CreateElement("xades", "UnsignedProperties", "http://uri.etsi.org/01903/v1.3.2#");
        qualifyingProperties.AppendChild(unsignedProperties);
    }

    XmlElement unsignedSignatureProperties = unsignedProperties.SelectSingleNode("xades:UnsignedSignatureProperties", GetNamespaceManager(signaturesXml)) as XmlElement;
    if (unsignedSignatureProperties == null)
    {
        unsignedSignatureProperties = signaturesXml.CreateElement("xades", "UnsignedSignatureProperties", "http://uri.etsi.org/01903/v1.3.2#");
        unsignedProperties.AppendChild(unsignedSignatureProperties);
    }

    unsignedSignatureProperties.AppendChild(signatureTimeStamp);
}


private static XmlNamespaceManager GetNamespaceManager(XmlDocument xmlDoc)
{
    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
    nsmgr.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");
    nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
    return nsmgr;
}
}