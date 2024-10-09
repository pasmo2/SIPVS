using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;


public class AddressInformation {
    public string City { get; set; }
    public string CityDistrict { get; set; }
    public string StreetName { get; set; }
    public int StreetNumber { get; set; }
    public string ZipCode { get; set; }
}

public class ContactInformation {
    public string Phone { get; set; }
    public string Email { get; set; }
}

public class PersonalInformation {
    
    public string FullName { get; set; }
    public string Degree { get; set; }
}

public class Employer {
    public PersonalInformation personInfo { get; set; }
    public AddressInformation address { get; set; }
    public string LegalForm { get; set;}
    public string ICO { get; set;}
    public string DIC { get; set;}
    public ContactInformation contact { get; set; }
}

public class Candidate {
    public PersonalInformation personInfo { get; set; }
    public AddressInformation address { get; set; }
    public ContactInformation contact { get; set; }
}

public class FormPageModel : PageModel
{
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

public IActionResult OnPost()
{
    Attachments = Attachments = string.IsNullOrWhiteSpace(AttachmentsString)
                                ? new List<string>()  // If AttachmentsString is null or empty, return an empty list
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

    Console.WriteLine($"XML: {xmlData}");

    // Validate XML against XSD
    XmlReaderSettings settings = new XmlReaderSettings();
    settings.Schemas.Add("http://www.example.com/job-application", Path.Combine(_env.ContentRootPath, "Schemas/jobApplication.xsd"));
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

    try
    {
        // Load the XSLT file before performing the transformation
        XslCompiledTransform xslt = new XslCompiledTransform();
        xslt.Load(Path.Combine(_env.ContentRootPath, "Schemas/transform.xsl"));

        // Define the path where the transformed HTML will be saved temporarily
        string outputDir = Path.Combine(_env.WebRootPath, "output");
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        string htmlFilePath = Path.Combine(outputDir, $"jobApplication_{Guid.NewGuid()}.html");

        // Perform the transformation and store the result in XmlOutput
        using (StringReader xmlReader = new StringReader(xmlData))
        using (XmlReader reader = XmlReader.Create(xmlReader))
        using (StreamWriter writer = new StreamWriter(htmlFilePath))
        {
            xslt.Transform(reader, null, writer);
        }

        // Read the transformed HTML for display
        XmlOutput = System.IO.File.ReadAllText(htmlFilePath);

        // Return the file as a downloadable attachment
        byte[] fileBytes = System.IO.File.ReadAllBytes(htmlFilePath);
        return File(fileBytes, "text/html", "jobApplication.html");
    }
    catch (Exception ex)
    {
        _logger.LogError($"XSLT Transformation error: {ex.Message}");
        ValidationResult += " Transformation to HTML failed.";
        return Page();
    }
}


}
