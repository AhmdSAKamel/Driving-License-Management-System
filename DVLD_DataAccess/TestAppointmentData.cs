using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;



public class TestAppointmentData
{

    public static bool GetTestAppointmentInfoByID(int TestAppointmentID, ref int TestTypeID, ref int LocalDrivingLicenseApplicationID, ref DateTime AppointmentDate, ref float PaidFees, 
                                                  ref int CreatedByUserID, ref bool IsLocked, ref int RetakeTestApplicationID)
    {
        bool isFound = false;

        string query = "SELECT * FROM TestAppointments WHERE TestAppointmentID = @TestAppointmentID";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        isFound = true;
                        TestTypeID = (int)reader["TestTypeID"];
                        LocalDrivingLicenseApplicationID = (int)reader["LocalDrivingLicenseApplicationID"];
                        AppointmentDate = (DateTime)reader["AppointmentDate"];
                        CreatedByUserID = (int)reader["CreatedByUserID"];
                        PaidFees = Convert.ToSingle(reader["PaidFees"]);
                        IsLocked = (bool)reader["IsLocked"];

                        if (reader["RetakeTestApplicationID"] == DBNull.Value)
                        { RetakeTestApplicationID = -1; }
                        else
                        { RetakeTestApplicationID = (int)reader["RetakeTestApplicationID"]; }
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

    public static bool GetLastTestAppointment(int LocalDrivingLicenseApplicationID, int TestTypeID, ref int TestAppointmentID, ref DateTime AppointmentDate,
                                              ref float PaidFees, ref int CreatedByUserID, ref bool IsLocked, ref int RetakeTestApplicationID)
    {
        bool isFound = false;

        string query = @"SELECT Top 1 * FROM TestAppointments
                         WHERE        (TestTypeID = @TestTypeID) AND (LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID) 
                         Order by      TestAppointmentID Desc";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        isFound = true;

                        TestAppointmentID = (int)reader["TestAppointmentID"];
                        AppointmentDate = (DateTime)reader["AppointmentDate"];
                        PaidFees = Convert.ToSingle(reader["PaidFees"]);
                        CreatedByUserID = (int)reader["CreatedByUserID"];
                        IsLocked = (bool)reader["IsLocked"];

                        if (reader["RetakeTestApplicationID"] == DBNull.Value)
                            RetakeTestApplicationID = -1;
                        else
                            RetakeTestApplicationID = (int)reader["RetakeTestApplicationID"];
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

    public static DataTable GetAllTestAppointments()
    {

        DataTable dt = new DataTable();
        string query = @"select * from TestAppointments_View
                          order by AppointmentDate Desc";

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

    public static DataTable GetApplicationTestAppointmentsPerTestType(int LocalDrivingLicenseApplicationID, int TestTypeID)
    {

        DataTable dt = new DataTable();
        string query = @"SELECT TestAppointmentID, AppointmentDate,PaidFees, IsLocked
                         FROM TestAppointments
                         WHERE (TestTypeID = @TestTypeID) AND (LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID)
                         Order by TestAppointmentID desc;";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

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

    public static int AddNewTestAppointment(int TestTypeID, int LocalDrivingLicenseApplicationID, DateTime AppointmentDate, float PaidFees, int CreatedByUserID, int RetakeTestApplicationID)
    {
        int TestAppointmentID = -1;

        string query = @"Insert Into TestAppointments (TestTypeID,LocalDrivingLicenseApplicationID,AppointmentDate,PaidFees,CreatedByUserID,IsLocked,RetakeTestApplicationID)
                    Values (@TestTypeID,@LocalDrivingLicenseApplicationID,@AppointmentDate,@PaidFees,@CreatedByUserID,0,@RetakeTestApplicationID);
                    SELECT SCOPE_IDENTITY();";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
            command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);
            command.Parameters.AddWithValue("@PaidFees", PaidFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            if (RetakeTestApplicationID == -1)

                command.Parameters.AddWithValue("@RetakeTestApplicationID", DBNull.Value);
            else
                command.Parameters.AddWithValue("@RetakeTestApplicationID", RetakeTestApplicationID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    TestAppointmentID = insertedID;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

        }


        return TestAppointmentID;
    }

    public static bool UpdateTestAppointment(int TestAppointmentID, int TestTypeID, int LocalDrivingLicenseApplicationID, DateTime AppointmentDate, 
                                            float PaidFees, int CreatedByUserID, bool IsLocked, int RetakeTestApplicationID)
    {

        int rowsAffected = 0;
        string query = @"Update  TestAppointments  
                    set TestTypeID = @TestTypeID,
                        LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID,
                        AppointmentDate = @AppointmentDate,
                        PaidFees = @PaidFees,
                        CreatedByUserID = @CreatedByUserID,
                        IsLocked=@IsLocked,
                        RetakeTestApplicationID=@RetakeTestApplicationID
                        where TestAppointmentID = @TestAppointmentID";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
            command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);
            command.Parameters.AddWithValue("@PaidFees", PaidFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
            command.Parameters.AddWithValue("@IsLocked", IsLocked);

            if (RetakeTestApplicationID == -1)
                command.Parameters.AddWithValue("@RetakeTestApplicationID", DBNull.Value);
            else
                command.Parameters.AddWithValue("@RetakeTestApplicationID", RetakeTestApplicationID);


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

    public static int GetTestID(int TestAppointmentID)
    {
        int TestID = -1;
        string query = @"select TestID from Tests where TestAppointmentID = @TestAppointmentID;";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    TestID = insertedID;
                }
            }

            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

        }

        return TestID;
    }

}
