using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class TestData
{

    public static byte GetPassedTestCount(int LocalDrivingLicenseApplicationID)
    {
        
        byte PassedTestCount = 0;

        string query = @"SELECT PassedTestCount = count(TestTypeID)
                         FROM Tests INNER JOIN
                         TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
						 where (LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID and TestResult = 1)" ;


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && byte.TryParse(result.ToString(), out byte ptCount))
                {
                    PassedTestCount = ptCount;
                }
            }

            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

        }

        return PassedTestCount;

    }

    public static bool GetTestInfoByID(int TestID, ref int TestAppointmentID, ref bool TestResult, ref string Notes, ref short CreatedByUserID)
    {
        bool isFound = false;

        string query = "SELECT * FROM Tests WHERE TestID = @TestID";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@TestID", TestID);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (reader.Read())
                    {
                        TestResult = (bool)reader["TestResult"];
                        int CreatedByUser = (int)reader["CreatedByUserID"];
                        CreatedByUserID = (short)CreatedByUser;
                        TestAppointmentID = (int)reader["TestAppointmentID"];


                        if (reader["Notes"] == DBNull.Value)
                            Notes = "";
                        else
                            Notes = (string)reader["Notes"];


                        isFound = true;
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

    public static bool GetLastTestByPersonAndTestTypeAndLicenseClass(int PersonID, int LicenseClassID, int TestTypeID, ref int TestID, ref int TestAppointmentID, 
                                                                     ref bool TestResult, ref string Notes, ref short CreatedByUserID)
    {
        bool isFound = false;

        string query = @"SELECT  top 1 Tests.TestID, Tests.TestAppointmentID, Tests.TestResult, Tests.Notes, Tests.CreatedByUserID, Applications.ApplicantPersonID
                         FROM            LocalDrivingLicenseApplications INNER JOIN Tests INNER JOIN
                                         TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID 
                                      ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID 
                                         INNER JOIN Applications ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID

                         WHERE (Applications.ApplicantPersonID = @PersonID) AND (LocalDrivingLicenseApplications.LicenseClassID = @LicenseClassID) AND (TestAppointments.TestTypeID = @TestTypeID)
                         ORDER BY Tests.TestAppointmentID DESC";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@PersonID", PersonID);
            command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (reader.Read())
                    {
                        TestID = (int)reader["TestID"];
                        TestResult = (bool)reader["TestResult"];
                        TestAppointmentID = (int)reader["TestAppointmentID"];

                        int CreatedByUser = (int)reader["CreatedByUserID"];
                        CreatedByUserID = (short)CreatedByUser;


                        if (reader["Notes"] == DBNull.Value)
                            Notes = "";
                        else
                            Notes = (string)reader["Notes"];


                        isFound = true;
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
    public static DataTable GetAllTests()
    {

        DataTable dt = new DataTable();

        string query = "SELECT * FROM Tests order by TestID";

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
                // Console.WriteLine("Error: " + ex.Message);
            }

        }

        return dt;
    }

    public static int AddNewTest(int TestAppointmentID, bool TestResult, string Notes, short CreatedByUserID)
    {
        int TestID = -1;

        string query = @"Insert Into Tests (TestAppointmentID ,TestResult ,Notes ,CreatedByUserID)
                         Values (@TestAppointmentID,@TestResult, @Notes, @CreatedByUserID);
                                UPDATE TestAppointments SET IsLocked=1 where TestAppointmentID = @TestAppointmentID;
                                SELECT SCOPE_IDENTITY();";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
            command.Parameters.AddWithValue("@TestResult", TestResult);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            
            if (Notes != "" && Notes != null)
                command.Parameters.AddWithValue("@Notes", Notes);
            else
                command.Parameters.AddWithValue("@Notes", System.DBNull.Value);

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

    public static bool UpdateTest(int TestID, int TestAppointmentID, bool TestResult, string Notes, short CreatedByUserID)
    {

        int rowsAffected = 0;

        string query = @"Update  Tests  
                         Set TestAppointmentID = @TestAppointmentID, TestResult = @TestResult, 
                             Notes = @Notes, CreatedByUserID=@CreatedByUserID
                             Where (TestID = @TestID)";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@TestID", TestID);
            command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
            command.Parameters.AddWithValue("@TestResult", TestResult);
            command.Parameters.AddWithValue("@Notes", Notes);
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

}
