using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




public class LocalDrivingLicenseApplicationData
{

    public static bool GetLocalDrivingLicenseApplicationInfoByID(int LocalDrivingLicenseApplicationID, ref int ApplicationID, ref int LicenseClassID)
    {
        bool isFound = false;

        string query = @"SELECT TOP 1 ApplicationID, LicenseClassID  FROM LocalDrivingLicenseApplications 
                         WHERE (LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID); ";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID",LocalDrivingLicenseApplicationID);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Handle potential DBNull values if columns are nullable
                        ApplicationID = reader["ApplicationID"] == DBNull.Value ? 0 : (int)reader["ApplicationID"];

                        LicenseClassID = reader["LicenseClassID"] == DBNull.Value? 0 : (int)reader["LicenseClassID"];

                        isFound = true;
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

    public static bool GetLocalDrivingLicenseApplicationInfoByApplicationID(int ApplicationID, ref int LocalDrivingLicenseApplicationID,ref int LicenseClassID)
    {
        bool isFound = false;
        string connectionString = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;

        if (string.IsNullOrEmpty(connectionString))
        {
            // Handle missing connection string
            return false;
        }

        string query = @"SELECT LocalDrivingLicenseApplicationID, LicenseClassID  FROM LocalDrivingLicenseApplications 
                         WHERE ApplicationID = @ApplicationID";

        using (SqlConnection connection = new SqlConnection(connectionString))
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
                        // Handle potential DBNull values
                        LocalDrivingLicenseApplicationID = reader["LocalDrivingLicenseApplicationID"] != DBNull.Value
                            ? (int)reader["LocalDrivingLicenseApplicationID"] : 0;

                        LicenseClassID = reader["LicenseClassID"] != DBNull.Value
                            ? (int)reader["LicenseClassID"] : 0;

                        isFound = true;
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
     
    public static DataTable GetAllLocalDrivingLicenseApplications()
    {
        DataTable dt = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;


        string query = @"SELECT * FROM LocalDrivingLicenseApplications_View2
                         Where Status != 'Cancelled'
                         ORDER BY [Application Date] DESC ";

        using (SqlConnection connection = new SqlConnection(connectionString))
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

    public static int AddNewLocalDrivingLicenseApplication(int ApplicationID, int LicenseClassID)
    {

        int LocalDrivingLicenseApplicationID = -1;
        string connectionString = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;

        if (string.IsNullOrEmpty(connectionString))
            return LocalDrivingLicenseApplicationID;


        string query = @"INSERT INTO LocalDrivingLicenseApplications ( 
                    ApplicationID, LicenseClassID)
                    VALUES (@ApplicationID, @LicenseClassID);
                    SELECT SCOPE_IDENTITY();";

        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    LocalDrivingLicenseApplicationID = insertedID;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }
           
        }

        return LocalDrivingLicenseApplicationID;
    }

    public static bool UpdateLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID, int ApplicationID, int LicenseClassID)
    {
        int rowsAffected = 0;
        string connectionString = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;

        if (string.IsNullOrEmpty(connectionString))
            return false;


        string query = @"UPDATE LocalDrivingLicenseApplications  
                    SET ApplicationID = @ApplicationID,
                        LicenseClassID = @LicenseClassID
                    WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";


        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

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

        return rowsAffected > 0;
    }

    public static bool DeleteLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID)
    {

        int rowsAffected = 0;
        string connectionString = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;

        string query = @"DELETE FROM LocalDrivingLicenseApplications 
                         WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";


        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }
        }

        return rowsAffected > 0;
    }

    public static bool DoesPassTestType(int LocalDrivingLicenseApplicationID, int TestTypeID)
    {
        bool result = false;
        string connectionString = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;

        string query = @"SELECT TOP 1 TestResult FROM LocalDrivingLicenseApplications 
                    INNER JOIN TestAppointments ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID 
                    INNER JOIN Tests ON TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                    
                    WHERE LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID AND TestAppointments.TestTypeID = @TestTypeID
                    ORDER BY TestAppointments.TestAppointmentID DESC";


        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                connection.Open();
                object queryResult = command.ExecuteScalar();

                if (queryResult != null && bool.TryParse(queryResult.ToString(), out bool testResult))
                {
                    result = testResult;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                result = false;
            }
           
        }

        return result;
    }

    public static bool DoesAttendTestType(int LocalDrivingLicenseApplicationID, int TestTypeID)
    {

        bool IsFound = false;


        string query = @" SELECT  Top 1 Found = 1
                            FROM  LocalDrivingLicenseApplications INNER JOIN
                                  TestAppointments ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID INNER JOIN
                                  Tests ON TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                            WHERE (LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID) AND (TestAppointments.TestTypeID = @TestTypeID)
                            ORDER BY TestAppointments.TestAppointmentID desc";



        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null)
                {
                    IsFound = true;
                }

            }

            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

        }

        return IsFound;

    }

    public static byte TotalTrialsPerTest(int LocalDrivingLicenseApplicationID, int TestTypeID)
    {

        byte TotalTrialsPerTest = 0;

        string query = @" SELECT TotalTrialsPerTest = count(TestID)
                            FROM LocalDrivingLicenseApplications INNER JOIN
                                 TestAppointments ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID INNER JOIN
                                 Tests ON TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                            WHERE
                            (LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID) AND (TestAppointments.TestTypeID = @TestTypeID)";

        
        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && byte.TryParse(result.ToString(), out byte Trials))
                {
                    TotalTrialsPerTest = Trials;
                }
            }

            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

        }

        return TotalTrialsPerTest;
    }

    public static bool IsThereAnActiveScheduledTest(int LocalDrivingLicenseApplicationID, int TestTypeID)
    {

        bool Result = false;

        string query = @" SELECT top 1 Found = 1
                            FROM LocalDrivingLicenseApplications INNER JOIN
                                 TestAppointments ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID
                            WHERE (LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID) AND (TestAppointments.TestTypeID = @TestTypeID) and (isLocked = 0)
                            ORDER BY TestAppointments.TestAppointmentID desc";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null)
                {
                    Result = true;
                }

            }

            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

        }

        return Result;

    }

}

