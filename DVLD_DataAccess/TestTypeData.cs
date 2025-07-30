using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


public static class TestTypeData
{

    public static bool GetTestTypeInfoByID(int TestTypeID, ref string TestTypeTitle, ref string TestDescription, ref decimal TestFees)
    {
        bool isFound = false;

        string query = "SELECT * FROM TestTypes WHERE TestTypeID = @TestTypeID";

        try
        {

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        TestTypeTitle = (string)reader["TestTypeTitle"];
                        TestDescription = (string)reader["TestTypeDescription"];
                        TestFees = Convert.ToDecimal(reader["TestTypeFees"]);

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

    public static DataTable GetAllTestTypes()
    {

        DataTable dt = new DataTable();
        string query = "SELECT * FROM TestTypes order by TestTypeID";

        try
        {


            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
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
        }
        catch (Exception ex)
        {
            EventLogger.WriteExceptionToEventViewer(ex.Message);

        }

        return dt;
    }

    public static int AddNewTestType(string Title, string Description, decimal Fees)
    {
        int TestTypeID = -1;
        string query = @"Insert Into TestTypes (TestTypeTitle,TestTypeTitle,TestTypeFees)
                            Values (@TestTypeTitle,@TestTypeDescription,@ApplicationFees)
                            where TestTypeID = @TestTypeID;
                            SELECT SCOPE_IDENTITY();";


        try
        {

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TestTypeTitle", Title);
                command.Parameters.AddWithValue("@TestTypeDescription", Description);
                command.Parameters.AddWithValue("@ApplicationFees", Fees);

                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    TestTypeID = insertedID;
                }
            }

        }
        catch (Exception ex)
        {
            EventLogger.WriteExceptionToEventViewer(ex.Message);

        }

        return TestTypeID;
    }

    public static bool UpdateTestType(int TestTypeID, string Title, string Description, decimal Fees)
    {

        int rowsAffected = 0;
        string query = @"Update  TestTypes  
                            set TestTypeTitle = @TestTypeTitle,
                                TestTypeDescription=@TestTypeDescription,
                                TestTypeFees = @TestTypeFees
                                where TestTypeID = @TestTypeID";


        try
        {

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                command.Parameters.AddWithValue("@TestTypeTitle", Title);
                command.Parameters.AddWithValue("@TestTypeDescription", Description);
                command.Parameters.AddWithValue("@TestTypeFees", Fees);

                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }

        }
        catch (Exception ex)
        {
            EventLogger.WriteExceptionToEventViewer(ex.Message);
            return false;
        }

        return (rowsAffected > 0);
    }

}


