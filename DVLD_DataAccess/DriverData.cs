using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


public class DriverData
{

    public static bool GetDriverInfoByDriverID(int DriverID, ref int PersonID, ref short CreatedByUserID, ref DateTime CreatedDate)
    {
        bool isFound = false;
        string query = "SELECT * FROM Drivers WHERE (DriverID = @DriverID)";



        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@DriverID", DriverID);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (reader.Read())
                    {
                        isFound = true;

                        PersonID = (int)reader["PersonID"];
                        CreatedDate = (DateTime)reader["CreatedDate"];

                        int CreatedByUser = (int)reader["CreatedByUserID"];
                        CreatedByUserID = (short)CreatedByUser;
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

    public static bool GetDriverInfoByPersonID(int PersonID, ref int DriverID, ref short CreatedByUserID, ref DateTime CreatedDate)
    {
        bool isFound = false;
        string query = "SELECT * FROM Drivers WHERE PersonID = @PersonID";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        isFound = true;

                        DriverID = (int)reader["DriverID"];
                        CreatedDate = (DateTime)reader["CreatedDate"];

                        int CreatedByUser = (int)reader["CreatedByUserID"];
                        CreatedByUserID = (short)CreatedByUser;
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

    public static DataTable GetAllDrivers()
    {

        DataTable dt = new DataTable();
        string query = "SELECT * FROM Drivers_View order by Date";


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

    public static int AddNewDriver(int PersonID, short CreatedByUserID)
    {
        int DriverID = -1;

        string query = @"Insert Into Drivers (PersonID,CreatedByUserID,CreatedDate)
                         Values (@PersonID, @CreatedByUserID, @CreatedDate);
                                  SELECT SCOPE_IDENTITY();";



        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@PersonID", PersonID);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
            command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    DriverID = insertedID;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

        }

        return DriverID;
    }

    public static bool UpdateDriver(int DriverID, int PersonID, short CreatedByUserID)
    {
        //we dont update the createddate for the driver.

        int rowsAffected = 0;
        string query = @"Update  Drivers  
                         Set PersonID = @PersonID,
                             CreatedByUserID = @CreatedByUserID
                             Where (DriverID = @DriverID)";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@DriverID", DriverID);
            command.Parameters.AddWithValue("@PersonID", PersonID);
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

