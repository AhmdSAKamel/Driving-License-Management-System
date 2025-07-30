 using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


public class LicenseClassData
{

    public static DataTable GetAllLicenseClasses()
    {

        DataTable dt = new DataTable();

        string query = "SELECT * FROM LicenseClasses order by ClassName";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                    }

                }

            }

            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

        }

        return dt;

    }

    public static bool GetLicenseClassInfoByID(int LicenseClassID, ref string ClassName, ref string ClassDescription, ref byte MinimumAllowedAge,
                                               ref byte DefaultValidityLength, ref decimal ClassFees)
    {

        bool isFound = false;

        string query = "SELECT * FROM LicenseClasses WHERE LicenseClassID = @LicenseClassID";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // The record was found
                        isFound = true;

                        ClassName = (string)reader["ClassName"];
                        ClassDescription = (string)reader["ClassDescription"];
                        MinimumAllowedAge = (byte)reader["MinimumAllowedAge"];
                        DefaultValidityLength = (byte)reader["DefaultValidityLength"];
                        ClassFees = Convert.ToDecimal(reader["ClassFees"]);

                    }
                    else
                    {
                        isFound = false;
                    }
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                isFound = false;
            }
        
        }

        return isFound;
    }

    public static bool GetLicenseClassInfoByClassName(string ClassName, ref int LicenseClassID, ref string ClassDescription, ref byte MinimumAllowedAge,
                                                      ref byte DefaultValidityLength, ref decimal ClassFees)
    {
        bool isFound = false;            
        
        string query = "SELECT * FROM LicenseClasses WHERE ClassName = @ClassName";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@ClassName", ClassName);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (reader.Read())
                    {
                        // The record was found
                        isFound = true;
                        LicenseClassID = (int)reader["LicenseClassID"];
                        ClassDescription = (string)reader["ClassDescription"];
                        MinimumAllowedAge = (byte)reader["MinimumAllowedAge"];
                        DefaultValidityLength = (byte)reader["DefaultValidityLength"];
                        ClassFees = Convert.ToDecimal(reader["ClassFees"]);

                    }
                    else
                    {
                        // The record was not found
                        isFound = false;
                    }

                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                isFound = false;
            }

        }

        return isFound;
    }

    public static int AddNewLicenseClass(string ClassName, string ClassDescription, byte MinimumAllowedAge, byte DefaultValidityLength, decimal ClassFees)
    {
        int LicenseClassID = -1;

        string query = @"INSERT INTO LicenseClasses (ClassName, ClassDescription, MinimumAllowedAge, DefaultValidityLength, ClassFees)
                         VALUES (@ClassName, @ClassDescription, @MinimumAllowedAge, @DefaultValidityLength, @ClassFees);
                         SELECT SCOPE_IDENTITY();";

        try
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ClassName", ClassName);
                command.Parameters.AddWithValue("@ClassDescription", ClassDescription);
                command.Parameters.AddWithValue("@MinimumAllowedAge", MinimumAllowedAge);
                command.Parameters.AddWithValue("@DefaultValidityLength", DefaultValidityLength);
                command.Parameters.AddWithValue("@ClassFees", ClassFees);

                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    LicenseClassID = insertedID;
                }

            }

        }
        catch (Exception ex)
        {
            EventLogger.WriteExceptionToEventViewer(ex.Message);
        }

        return LicenseClassID;
    }

    public static bool UpdateLicenseClass(int LicenseClassID, string ClassName, string ClassDescription, byte MinimumAllowedAge, byte DefaultValidityLength, decimal ClassFees)
    {
        string query = @"UPDATE LicenseClasses  
                         SET ClassName = @ClassName, ClassDescription = @ClassDescription, MinimumAllowedAge = @MinimumAllowedAge,
                         DefaultValidityLength = @DefaultValidityLength, ClassFees = @ClassFees
                         WHERE LicenseClassID = @LicenseClassID";

        try
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                command.Parameters.AddWithValue("@ClassName", ClassName);
                command.Parameters.AddWithValue("@ClassDescription", ClassDescription);
                command.Parameters.AddWithValue("@MinimumAllowedAge", MinimumAllowedAge);
                command.Parameters.AddWithValue("@DefaultValidityLength", DefaultValidityLength);
                command.Parameters.AddWithValue("@ClassFees", ClassFees);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;  // Returns true if at least one row was updated
            }
        }
        catch (Exception ex)
        {
            EventLogger.WriteExceptionToEventViewer(ex.Message);
        }

        return false;
    }

}
