using System;
using System.CodeDom.Compiler;
using System.Data;
using System.Linq;
using DVLD_DataAccess;


namespace DVLDBuiness
{
    public class clsCountry
    {
        public byte NationalityCountryID { get; set; }

        public string CountryName { get; set; }

        private clsCountry(byte nationalityCountryID, string countryName)
        {
            NationalityCountryID = nationalityCountryID;
            CountryName = countryName;
        }

        public static DataTable GetAllCountries()
        {
            DataTable result = CountryData.RetrieveAllCountries();

            return result;
        }

        public static clsCountry Find(byte CountryID)
        {
            string Name = "";

            bool Found = CountryData.GetCountryInfoByID(CountryID, ref Name);

            if (Found)
            {
                return new clsCountry(CountryID, Name);
            }
            else
                return null;

        }

        public static clsCountry Find(string CountryName)
        {

            byte ID = 1;

            if (CountryData.GetCountryInfoByName(CountryName, ref ID))
                return new clsCountry(ID, CountryName);
            else
                return null;

        }

    }


}
