using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;



public static class ApplicationData
{

    public static bool GetApplicationInfoByID(int ApplicationID, ref int ApplicantPersonID, ref DateTime ApplicationDate, ref byte ApplicationTypeID,
                                              ref byte ApplicationStatus, ref DateTime LastStatusDate, ref decimal PaidFees, ref short CreatedByUserID)
    {
        bool isFound = false;

        string query = @"SELECT Top 1 ApplicantPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID
                        FROM Applications WHERE(ApplicationID = @ApplicationID)";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        isFound = true;

                        ApplicantPersonID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                        ApplicationDate = reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1);
                       
                        int AppTypeID = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                        ApplicationTypeID = (byte)AppTypeID;

                        ApplicationStatus = reader.IsDBNull(3) ? (byte)0 : reader.GetByte(3);
                        LastStatusDate = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4);

                        PaidFees = reader.IsDBNull(5) ? 0m : reader.GetDecimal(5);

                        CreatedByUserID = Convert.ToInt16(reader.IsDBNull(6) ? 0 : reader.GetInt32(6));
                    

                    }

                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }
        }



        return isFound;
    }

    public static DataTable GetAllApplications()
    {
        DataTable dt = new DataTable();
        string query = "SELECT * FROM ApplicationsList_View ORDER BY ApplicationDate DESC";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }
        }

        return dt;
    }

    public static bool UpdateApplication(int ApplicationID, int ApplicantPersonID, DateTime ApplicationDate, int ApplicationTypeID,
                                         byte ApplicationStatus, DateTime LastStatusDate, decimal PaidFees, int CreatedByUserID)
    {
        string query = @" UPDATE Applications  
                          SET ApplicationDate = @ApplicationDate, ApplicationTypeID = @ApplicationTypeID, ApplicationStatus = @ApplicationStatus, 
                              LastStatusDate = @LastStatusDate, PaidFees = @PaidFees, CreatedByUserID = @CreatedByUserID
                              WHERE ApplicationID = @ApplicationID";



        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
            command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
            command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
            command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
            command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
            command.Parameters.AddWithValue("@PaidFees", PaidFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                return false;
            }

        }

    }

    public static bool DeleteApplication(int ApplicationID)
    {
        string query = "DELETE FROM Applications WHERE ApplicationID = @ApplicationID";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                return false;
            }
        }
    }

    public static bool IsApplicationExist(int ApplicationID)
    {
        string query = "SELECT 1 FROM Applications WHERE ApplicationID = @ApplicationID";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                connection.Open();
                return command.ExecuteScalar() != null;
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message); return false;
            }
        }
    }

    public static bool DoesPersonHaveActiveApplication(int PersonID, int ApplicationTypeID)
    {
        int activeAppID = GetActiveApplicationID(PersonID, ApplicationTypeID);

        // Ensures robust handling if GetActiveApplicationID fails internally
        return activeAppID > 0;
    }

    public static int GetActiveApplicationID(int PersonID, int ApplicationTypeID)
    {
        int ActiveApplicationID = -1;
        string query = "SELECT ApplicationID FROM Applications WHERE ApplicantPersonID = @ApplicantPersonID AND ApplicationTypeID = @ApplicationTypeID AND ApplicationStatus = 1";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);
            command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    ActiveApplicationID = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }
        }

        return ActiveApplicationID;
    }

    public static int GetActiveApplicationIDForLicenseClass(int PersonID, int ApplicationTypeID, int LicenseClassID)
    {

        int ActiveApplicationID = -1;
        string query = @"SELECT Applications.ApplicationID FROM Applications 
                        INNER JOIN LocalDrivingLicenseApplications ON Applications.ApplicationID = LocalDrivingLicenseApplications.ApplicationID
                        WHERE (ApplicantPersonID = @ApplicantPersonID AND ApplicationTypeID = @ApplicationTypeID 
                               AND LocalDrivingLicenseApplications.LicenseClassID = @LicenseClassID AND ApplicationStatus = 1)" ;

        
        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);
            command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
            command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    ActiveApplicationID = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }
        }

        return ActiveApplicationID;
    }

    public static bool UpdateStatus(int ApplicationID, short NewStatus)
    {
        string query = @"UPDATE Applications  
                     SET ApplicationStatus = @NewStatus,  LastStatusDate = @LastStatusDate
                     WHERE (ApplicationID = @ApplicationID);";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            command.Parameters.AddWithValue("@NewStatus", NewStatus);
            command.Parameters.AddWithValue("@LastStatusDate", DateTime.Now);

            try
            {
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                return false;
            }

        }

    }

    public static int AddNewApplication(int ApplicantPersonID, DateTime ApplicationDate, int ApplicationTypeID,
                                        byte ApplicationStatus, DateTime LastStatusDate, decimal PaidFees, int CreatedByUserID)
    {
        int ApplicationID = -1;
        string query = @"INSERT INTO Applications (
                        ApplicantPersonID, ApplicationDate, ApplicationTypeID,
                        ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID)
                     VALUES (@ApplicantPersonID, @ApplicationDate, @ApplicationTypeID,
                             @ApplicationStatus, @LastStatusDate, @PaidFees, @CreatedByUserID);
                     SELECT SCOPE_IDENTITY();";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
            command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
            command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
            command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
            command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
            command.Parameters.AddWithValue("@PaidFees", PaidFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    ApplicationID = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }
        }

        return ApplicationID;
    }


}


