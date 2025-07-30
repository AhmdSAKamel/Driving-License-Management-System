using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class DetainedLicenseData
{

    public static bool GetDetainedLicenseInfoByID(int DetainID, ref int LicenseID, ref DateTime DetainDate, ref decimal FineFees, ref short CreatedByUserID,
                                                    ref bool IsReleased, ref DateTime ReleaseDate, ref short ReleasedByUserID, ref int ReleaseApplicationID)
    {
        bool isFound = false;


        string query = "SELECT * FROM DetainedLicenses WHERE DetainID = @DetainID";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@DetainID", DetainID);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (reader.Read())
                    {

                        // The record was found
                        isFound = true;

                        LicenseID = (int)reader["LicenseID"];
                        DetainDate = (DateTime)reader["DetainDate"];
                        FineFees = Convert.ToDecimal(reader["FineFees"]);
                        CreatedByUserID = Convert.ToInt16(reader["CreatedByUserID"]);
                        IsReleased = (bool)reader["IsReleased"];


                        if (reader["ReleaseDate"] == DBNull.Value)
                            ReleaseDate = DateTime.MaxValue;
                        else
                            ReleaseDate = (DateTime)reader["ReleaseDate"];


                        if (reader["ReleasedByUserID"] == DBNull.Value)
                            ReleasedByUserID = -1;
                        else
                            ReleasedByUserID = Convert.ToInt16(reader["ReleasedByUserID"]);

                        if (reader["ReleaseApplicationID"] == DBNull.Value)
                            ReleaseApplicationID = -1;
                        else
                            ReleaseApplicationID = (int)reader["ReleaseApplicationID"];
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

    public static bool GetDetainedLicenseInfoByLicenseID(int LicenseID, ref int DetainID, ref DateTime DetainDate, ref decimal FineFees, ref short CreatedByUserID,
                                                         ref bool IsReleased, ref DateTime ReleaseDate, ref short ReleasedByUserID, ref int ReleaseApplicationID)
    {
        bool isFound = false;

        string query = "SELECT top 1 * FROM DetainedLicenses WHERE (LicenseID = @LicenseID)" +
                       "Order by DetainID desc";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LicenseID", LicenseID);

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (reader.Read())
                    {

                        // The record was found
                        isFound = true;

                        DetainID = (int)reader["DetainID"];
                        DetainDate = (DateTime)reader["DetainDate"];
                        FineFees = Convert.ToDecimal(reader["FineFees"]);
                        CreatedByUserID = Convert.ToInt16(reader["CreatedByUserID"]);
                        IsReleased = (bool)reader["IsReleased"];


                        if (reader["ReleaseDate"] == DBNull.Value)
                            ReleaseDate = DateTime.MaxValue;
                        else
                            ReleaseDate = (DateTime)reader["ReleaseDate"];

                        if (reader["ReleasedByUserID"] == DBNull.Value)
                            ReleasedByUserID = -1;
                        else
                            ReleasedByUserID = Convert.ToInt16(reader["ReleasedByUserID"]);

                        if (reader["ReleaseApplicationID"] == DBNull.Value)
                            ReleaseApplicationID = -1;
                        else
                            ReleaseApplicationID = (int)reader["ReleaseApplicationID"];

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

    public static DataTable GetAllDetainedLicenses()
    {

        DataTable dt = new DataTable();

        string query = "select * from detainedLicenses_View Order by [D.ID] desc;";

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

    public static int AddNewDetainedLicense(int LicenseID, DateTime DetainDate, decimal FineFees, short CreatedByUserID)
    {
        int DetainID = -1;

        string query = @"INSERT INTO dbo.DetainedLicenses (LicenseID, DetainDate, FineFees, CreatedByUserID, IsReleased)
                         VALUES (@LicenseID, @DetainDate,  @FineFees,  @CreatedByUserID, 0);
                         SELECT SCOPE_IDENTITY();";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LicenseID", LicenseID);
            command.Parameters.AddWithValue("@DetainDate", DetainDate);
            command.Parameters.AddWithValue("@FineFees", FineFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    DetainID = insertedID;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

        }

        return DetainID;
    }

    public static bool UpdateDetainedLicense(int DetainID, int LicenseID, DateTime DetainDate, decimal FineFees, short CreatedByUserID)
    {

        int rowsAffected = 0;

        string query = @"UPDATE dbo.DetainedLicenses
                              SET (LicenseID = @LicenseID, DetainDate = @DetainDate, FineFees = @FineFees, 
                                   CreatedByUserID = @CreatedByUserID),   
                                   WHERE (DetainID = @DetainID);";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@DetainedLicenseID", DetainID);
            command.Parameters.AddWithValue("@LicenseID", LicenseID);
            command.Parameters.AddWithValue("@DetainDate", DetainDate);
            command.Parameters.AddWithValue("@FineFees", FineFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);


            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                return false;
            }

        }

        return (rowsAffected > 0);
    }

    public static bool ReleaseDetainedLicense(int detainID, short releasedByUserID, int releaseApplicationID)
    {

        const string query = @"UPDATE dbo.DetainedLicenses
                               SET IsReleased = 1, ReleaseDate = @ReleaseDate, 
                               ReleaseApplicationID = @ReleaseApplicationID,
                               releasedByUserID = @releasedByUserID

                               WHERE (DetainID = @DetainID);";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@DetainID", detainID);
            command.Parameters.AddWithValue("@ReleasedByUserID", releasedByUserID);
            command.Parameters.AddWithValue("@ReleaseApplicationID", releaseApplicationID);
            command.Parameters.AddWithValue("@ReleaseDate", DateTime.Now);

            try
            {
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                return false;
            }

        }

    }

    public static bool IsLicenseDetained(int licenseID)
    {
        const string query = @"SELECT 1 FROM dbo.DetainedLicenses WHERE (LicenseID = @LicenseID AND IsReleased = 0);";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LicenseID", licenseID);

            try
            {
                connection.Open();
                return command.ExecuteScalar() != null;
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                return false;
            }

        }

    }
   
}
