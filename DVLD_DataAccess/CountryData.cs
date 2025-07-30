using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;



namespace DVLD_DataAccess
{
    public class CountryData
    {
        public static DataTable RetrieveAllCountries()
        {
            DataTable dt = new DataTable();


            string query = "SELECT * FROM Countries";


            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)     // (if reader has data)
                        {
                            dt.Load(reader);
                        }
                    }

                }

            }
            catch
            {
                dt = null;
            }


            return dt;

        }

        public static bool GetCountryInfoByID(int NationalityCountryID, ref string CountryName)
        {
            bool isFound = false;

            string query = @"SELECT TOP 1 * FROM Countries WHERE (CountryID = @NationalityCountryID);";

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString))
                using (SqlCommand cnd = new SqlCommand(query, con))
                {
                    cnd.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
                    con.Open();

                    using (SqlDataReader reader = cnd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CountryName = reader.IsDBNull(reader.GetOrdinal("CountryName")) ? "" : reader["CountryName"].ToString();
                            isFound = true;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                isFound = false;
            }


            return isFound;
        }

        public static bool GetCountryInfoByName(string CountryName, ref byte CountryID)
        {
            bool isFound = false;

            string query = "SELECT * FROM Countries WHERE CountryName = @CountryName";

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString))
                using (SqlCommand cnd = new SqlCommand(query, con))
                {

                    cnd.Parameters.AddWithValue("@CountryName", CountryName);
                    con.Open();

                    using (SqlDataReader reader = cnd.ExecuteReader())
                    {
                        
                        if (reader.Read())
                        {
                            CountryID = (byte)reader["CountryID"];   
                            isFound = true;
                        }
                        else
                        {
                            // The record was not found
                            isFound = false;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                isFound = false;
            }

            return isFound;

        }
    }

}
