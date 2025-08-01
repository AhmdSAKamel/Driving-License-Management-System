﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class LicenseData
{

    public static bool GetLicenseInfoByID(int LicenseID, ref int ApplicationID, ref int DriverID, ref int LicenseClass, ref DateTime IssueDate, ref DateTime ExpirationDate, 
                                          ref string Notes, ref decimal PaidFees, ref bool IsActive, ref byte IssueReason, ref int CreatedByUserID)
    {
        bool isFound = false;
        string query = "SELECT * FROM Licenses WHERE LicenseID = @LicenseID";


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

                        isFound = true;
                        ApplicationID = (int)reader["ApplicationID"];
                        DriverID = (int)reader["DriverID"];
                        LicenseClass = (int)reader["LicenseClass"];
                        IssueDate = (DateTime)reader["IssueDate"];
                        ExpirationDate = (DateTime)reader["ExpirationDate"];

                        if (reader["Notes"] == DBNull.Value)
                            Notes = "";
                        else
                            Notes = (string)reader["Notes"];

                        PaidFees = Convert.ToDecimal(reader["PaidFees"]);
                        IsActive = (bool)reader["IsActive"];
                        IssueReason = (byte)reader["IssueReason"];
                        CreatedByUserID = (int)reader["DriverID"];

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

    public static DataTable GetAllLicenses()
    {
        DataTable dt = new DataTable();

        string query = "SELECT * FROM Licenses";


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

    public static DataTable GetDriverLicenses(int DriverID)
    {

        DataTable dt = new DataTable();
        string query = @"SELECT Licenses.LicenseID, ApplicationID, LicenseClasses.ClassName, Licenses.IssueDate, 
		                        Licenses.ExpirationDate, Licenses.IsActive
                         FROM   Licenses INNER JOIN LicenseClasses ON Licenses.LicenseClass = LicenseClasses.LicenseClassID
                         Where  (DriverID = @DriverID)
                         Order By IsActive Desc, ExpirationDate Desc";


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

    public static int AddNewLicense(int ApplicationID, int DriverID, int LicenseClass, DateTime IssueDate, DateTime ExpirationDate, 
                                  string Notes, decimal PaidFees, bool IsActive, byte IssueReason, int CreatedByUserID)
    {
        int LicenseID = -1;


        string query = @"INSERT INTO Licenses (ApplicationID, DriverID, LicenseClass, IssueDate, ExpirationDate, 
                         Notes, PaidFees, IsActive, IssueReason, CreatedByUserID)
                         VALUES (@ApplicationID, @DriverID, @LicenseClass, @IssueDate, @ExpirationDate, @Notes, @PaidFees, @IsActive, @IssueReason, @CreatedByUserID);
                         SELECT SCOPE_IDENTITY();";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            command.Parameters.AddWithValue("@DriverID", DriverID);
            command.Parameters.AddWithValue("@LicenseClass", LicenseClass);
            command.Parameters.AddWithValue("@IssueDate", IssueDate);
            command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
            command.Parameters.AddWithValue("@PaidFees", PaidFees);
            command.Parameters.AddWithValue("@IsActive", IsActive);
            command.Parameters.AddWithValue("@IssueReason", IssueReason);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            if (Notes == "")
                command.Parameters.AddWithValue("@Notes", DBNull.Value);
            else
                command.Parameters.AddWithValue("@Notes", Notes);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    LicenseID = insertedID;
                }
            
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

        }

        return LicenseID;
    }

    public static bool UpdateLicense(int LicenseID, int ApplicationID, int DriverID, int LicenseClass, DateTime IssueDate, DateTime ExpirationDate,
                                        string Notes, decimal PaidFees, bool IsActive, byte IssueReason, int CreatedByUserID)
    {

        int rowsAffected = 0;

        string query = @"UPDATE Licenses 
                            SET 
                              ApplicationID = @ApplicationID, DriverID = @DriverID, LicenseClass = @LicenseClass, IssueDate = @IssueDate,
                              ExpirationDate = @ExpirationDate, Notes = @Notes, PaidFees = @PaidFees, IsActive = @IsActive, 
                              IssueReason = @IssueReason, CreatedByUserID = @CreatedByUserID
                              WHERE LicenseID=@LicenseID";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@LicenseID", LicenseID);
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            command.Parameters.AddWithValue("@DriverID", DriverID);
            command.Parameters.AddWithValue("@LicenseClass", LicenseClass);
            command.Parameters.AddWithValue("@IssueDate", IssueDate);
            command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
            command.Parameters.AddWithValue("@PaidFees", PaidFees);
            command.Parameters.AddWithValue("@IsActive", IsActive);
            command.Parameters.AddWithValue("@IssueReason", IssueReason);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            if (Notes == "")
                command.Parameters.AddWithValue("@Notes", DBNull.Value);
            else
                command.Parameters.AddWithValue("@Notes", Notes);


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

    public static int GetActiveLicenseIDByPersonID(int PersonID, int LicenseClassID)
    {
        int LicenseID = -1;

        string query = @"SELECT   Licenses.LicenseID
                           FROM   Licenses INNER JOIN Drivers ON Licenses.DriverID = Drivers.DriverID
                          WHERE   (Licenses.LicenseClass = @LicenseClass AND Drivers.PersonID = @PersonID AND IsActive = 1);";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@PersonID", PersonID);
            command.Parameters.AddWithValue("@LicenseClass", LicenseClassID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    LicenseID = insertedID;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }
            
        }


        return LicenseID;
    }

    public static bool DeactivateLicense(int LicenseID)
    {

        int rowsAffected = 0;

        string query = @"UPDATE Licenses
                         SET IsActive = 0
                         WHERE (LicenseID = @LicenseID)";


        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LicenseID", LicenseID);

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
