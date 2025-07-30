using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;





public class InternationalLicenseData
{

    public static bool GetInternationalLicenseInfoByID(int InternationalLicenseID, ref int ApplicationID, ref int DriverID, ref int IssuedUsingLocalLicenseID,
                                                       ref DateTime IssueDate, ref DateTime ExpirationDate, ref bool IsActive, ref short CreatedByUserID)
    {
        bool isFound = false;

        string query = "SELECT * FROM InternationalLicenses WHERE (InternationalLicenseID = @InternationalLicenseID)";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (reader.Read())
                    {
                        isFound = true;
                        ApplicationID = (int)reader["ApplicationID"];
                        DriverID = (int)reader["DriverID"];
                        IssuedUsingLocalLicenseID = (int)reader["IssuedUsingLocalLicenseID"];
                        IssueDate = (DateTime)reader["IssueDate"];
                        ExpirationDate = (DateTime)reader["ExpirationDate"];

                        IsActive = (bool)reader["IsActive"];
                        CreatedByUserID = Convert.ToInt16(reader["DriverID"]);
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
    
    public static DataTable GetAllInternationalLicenses()
    {

        DataTable dt = new DataTable();
        string query = @"SELECT InternationalLicenseID AS [Int.License ID], ApplicationID As [Application ID], DriverID AS [Driver ID],
                         IssuedUsingLocalLicenseID AS [L.License ID], IssueDate AS [Issue Date], ExpirationDate AS [Expiration Date],
                         IsActive AS [Is Active]
                         FROM InternationalLicenses
                         ORDER BY IsActive, ExpirationDate DESC";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
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

    public static DataTable GetDriverInternationalLicenses(int DriverID)
    {

        DataTable dt = new DataTable();

        string query = @"SELECT  InternationalLicenseID, ApplicationID, IssuedUsingLocalLicenseID, IssueDate, ExpirationDate, IsActive
		                 From InternationalLicenses where DriverID=@DriverID
                         Order by ExpirationDate desc";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@DriverID", DriverID);

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

    public static int AddNewInternationalLicense(int ApplicationID, int DriverID, int IssuedUsingLocalLicenseID, DateTime IssueDate, DateTime ExpirationDate, bool IsActive, int CreatedByUserID)
    {
        int InternationalLicenseID = -1;

        // For disabling any old internationalLicense that a driver has!
        string query = @"Update InternationalLicenses 
                         Set IsActive = 0 where (DriverID = @DriverID); 

                             INSERT INTO InternationalLicenses(ApplicationID, DriverID, IssuedUsingLocalLicenseID, IssueDate, ExpirationDate, IsActive, CreatedByUserID)
                             VALUES (@ApplicationID, @DriverID, @IssuedUsingLocalLicenseID, @IssueDate, @ExpirationDate, @IsActive, @CreatedByUserID);
                         SELECT SCOPE_IDENTITY();";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@IsActive", IsActive);

            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            command.Parameters.AddWithValue("@DriverID", DriverID);
            command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
            command.Parameters.AddWithValue("@IssueDate", IssueDate);
            command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);

            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);


            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    InternationalLicenseID = insertedID;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

        }

        return InternationalLicenseID;
    }

    public static bool UpdateInternationalLicense(int InternationalLicenseID, int ApplicationID, int DriverID, int IssuedUsingLocalLicenseID,
                                                DateTime IssueDate, DateTime ExpirationDate, bool IsActive, int CreatedByUserID)
    {
        int rowsAffected = 0;

        string query = @"UPDATE InternationalLicenses
                           SET 
                              ApplicationID=@ApplicationID,
                              DriverID = @DriverID,
                              IssuedUsingLocalLicenseID = @IssuedUsingLocalLicenseID,
                              IssueDate = @IssueDate,
                              ExpirationDate = @ExpirationDate,
                              IsActive = @IsActive,
                              CreatedByUserID = @CreatedByUserID
                         WHERE InternationalLicenseID=@InternationalLicenseID";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            command.Parameters.AddWithValue("@DriverID", DriverID);
            command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
            command.Parameters.AddWithValue("@IssueDate", IssueDate);
            command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);

            command.Parameters.AddWithValue("@IsActive", IsActive);
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

    public static int GetActiveInternationalLicenseIDByDriverID(int DriverID)
    {
        int InternationalLicenseID = -1;

        string query = @"SELECT Top 1 InternationalLicenseID
                         FROM InternationalLicenses 
                         Where (DriverID = @DriverID And GetDate() Between IssueDate and ExpirationDate)
                         Order by ExpirationDate Desc;";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@DriverID", DriverID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    InternationalLicenseID = insertedID;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

        }

        return InternationalLicenseID;
    }

}
