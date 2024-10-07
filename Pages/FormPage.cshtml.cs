using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.Xml.Linq;

public class FormPageModel : PageModel
{
    private readonly ILogger<FormPageModel> _logger; // logger init

    public FormPageModel(ILogger<FormPageModel> logger) // logger injection
    {
        _logger = logger;
    }

    [BindProperty]
    public string PersonName { get; set; }

    [BindProperty]
    public int PersonAge { get; set; }

    public string XmlOutput { get; set; }
    public string ValidationResult { get; set; }

    public void OnPost()
    {
        // create an xml from the properties we extracted form the form
        XElement personXml = new XElement("person",
            new XElement("name", PersonName),
            new XElement("age", PersonAge)
        );

        // convert the XElement to a string
        string xmlData = personXml.ToString();

        XmlReaderSettings booksSettings = new XmlReaderSettings();
        booksSettings.Schemas.Add(null, "schemas/person.xsd");
        booksSettings.ValidationType = ValidationType.Schema;
        // booksSettings.ValidationEventHandler += ValidationCallback;

        StringReader stringReader = new StringReader(xmlData);
        XmlReader books = XmlReader.Create(stringReader, booksSettings);

        try
        {
            while (books.Read()) { }
            XmlOutput = xmlData;
            ValidationResult = $"XML from the form is valid!";
        }
        catch (XmlSchemaValidationException ex)
        {
            // if xml is invalid (according to our xsd)
            ValidationResult = $"Your form's validation failed!";
            _logger.LogError($"XML validation exception: {ex.Message}");
        }
    }

    // private void ValidationCallback(object sender, ValidationEventArgs e)
    // {
    //     ValidationResult = $"Validation error: {e.Message}";
    // }
}
