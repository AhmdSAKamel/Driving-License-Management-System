using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Configuration;
using System.Net;
using System.Globalization;
using System.Xml.Linq;


namespace DVLDBuiness
{
    public static class clsGlobalSettings
    {
        public static clsUser CurrentUser = new clsUser();
       
        private static XmlDocument _licenseClassesDoc;

        public static List<string> LicenseClasses()
        {
            List<string> licenseClasses = new List<string>();

            // Read the XML as string
            string xmlString = ConfigurationManager.AppSettings["LicenseClasses"];

            // Load into XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            // Loop through each <Class> node
            foreach (XmlNode classNode in xmlDoc.SelectNodes("//Class"))
            {
                string description = classNode.Attributes["Description"]?.Value;

                licenseClasses.Add(description);

            }

            return licenseClasses;

        }
      
        public static string GetLicenseClassByKey(int key)
        {
            if (key < 1)
            {
                return "";
            }

            key--;

            string[] Classes = LicenseClasses().ToArray();

            return Classes[key];
        }

        //private static readonly Dictionary<int, string> _statusMap = new Dictionary<int, string>
        //{
        //    {1, "New"},
        //    {2, "Completed"},
        //    {3, "Canceled"}
        //};

        //public static string GetApplicationTypeValueBykey(int statusIndex)
        //{
        //    return _statusMap.TryGetValue(statusIndex, out var status) ? status : "Unknown";
        //}
        
        public static string GetAppStatusValueByKey(int key)
        {
            string appTypeMapXml = ConfigurationManager.AppSettings["ApplicationStatus"];

            XDocument doc = XDocument.Parse(appTypeMapXml);
            
            var item = doc.Descendants("Item").
                FirstOrDefault(i => (string)i.Attribute("Key") == key.ToString());


            return item?.Attribute("Value")?.Value;
        }

        public static string GetAppTypeValueByKey(int key)
        {
            // Get the XML string from app.config
            string appTypeMapXml = ConfigurationManager.AppSettings["AppTypeMap"];

            // Parse the XML string
            XDocument doc = XDocument.Parse(appTypeMapXml);

            // Query the XML for the item with the specified key
            var item = doc.Descendants("Item")
                          .FirstOrDefault(i => (string)i.Attribute("Key") == key.ToString());


            return item?.Attribute("Value")?.Value;
        }
        
        private static int CalculateAge(DateTime birthDate, DateTime referenceDate)
        {
            int age = referenceDate.Year - birthDate.Year;
            if (birthDate.Date > referenceDate.AddYears(-age)) age--;
            return age;
        }

        private static void LoadLicenseClasses()
        {
            // Step 1: Check if already loaded
            if (_licenseClassesDoc != null)
                return;

            // Step 2: Get the XML string from config
            string escapedXml = ConfigurationManager.AppSettings["LicenseClasses"];
            
            if (string.IsNullOrWhiteSpace(escapedXml))
                throw new Exception("LicenseClasses configuration not found in app settings.");

            // Step 3: Unescape the XML
            string xml = WebUtility.HtmlDecode(escapedXml);

            // Step 4: Load into XmlDocument
            _licenseClassesDoc = new XmlDocument();

            // Step 5: Secure XML loading
            _licenseClassesDoc.XmlResolver = null; // Prevent XXE attacks

            try
            {
                _licenseClassesDoc.LoadXml(xml);
            }
            catch (XmlException ex)
            {
                throw new Exception("Invalid XML format in LicenseClasses", ex);
            }

        }

        public static bool IsOldEnough(int classIndex, DateTime birthDate)
        {
            LoadLicenseClasses();

            XmlNodeList classes = _licenseClassesDoc.SelectNodes("/LicenseClasses/Class");
            if (classIndex < 0 || classIndex >= classes.Count)
                throw new ArgumentOutOfRangeException(nameof(classIndex), "License class index is out of range.");

            XmlNode selectedClass = classes[classIndex];
            string allowedAgeStr = selectedClass.Attributes["AllowedAge"]?.Value;
            if (!int.TryParse(allowedAgeStr, out int allowedAge))
                throw new Exception("Invalid AllowedAge attribute in config.");

            // Calculate age
            int age = CalculateAge(birthDate, DateTime.Today);

            return age >= allowedAge;
        
        }

        public static string ConvertDateToMonthNameFormat(string inputDateTime)
        {
            // Parse the input date string in "dd/MM/yyyy" format

            if (DateTime.TryParse(inputDateTime, out DateTime dateTime))
            {
                return dateTime.ToString("dd/MMMM/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                return ""; // or handle as needed.
            }

            /*CultureInfo.InvariantCulture is a culture-independent setting used in .NET to ensure consistent formatting and parsing of dates, numbers, and other data types regardless of the system's locale or regional settings.
            What it does:
            It provides a neutral, invariant culture that doesn't vary with user or system regional settings.
            When used in date/time formatting or parsing, it ensures the format remains consistent, avoiding issues caused by differences like date order, month names, decimal separators, etc.
            */

        }

    }

}
