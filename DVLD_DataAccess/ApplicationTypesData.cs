using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;



public class ApplicationTypesData
{

    public static bool GetApplicationTypeInfoByID(byte ApplicationTypeID,
       ref string ApplicationTypeTitle, ref decimal ApplicationFees)
    {
        bool isFound = false;
        string query = @"SELECT * FROM ApplicationTypes WHERE ApplicationTypeID = @ApplicationTypeID";

        try
        {

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        isFound = true;

                        ApplicationTypeTitle = (string)reader["ApplicationTypeTitle"];
                        ApplicationFees = (decimal)(reader["ApplicationFees"]);
                    }
                    else
                    {
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

    public static DataTable GetAllApplicationTypes()
    {

        DataTable dt = new DataTable();

        string query = "SELECT * FROM ApplicationTypes order by ApplicationTypeTitle";

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

    public static int AddNewApplicationType(string Title, decimal Fees)
    {
        int ApplicationTypeID = -1;
        string query = @"Insert Into ApplicationTypes (ApplicationTypeTitle,ApplicationFees)
                            Values (@Title,@Fees)
                            
                            SELECT SCOPE_IDENTITY();";

        try
        {

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ApplicationTypeTitle", Title);
                command.Parameters.AddWithValue("@ApplicationFees", Fees);

                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    ApplicationTypeID = insertedID;
                }
            }

        }
        catch (Exception ex)
        {
            EventLogger.WriteExceptionToEventViewer(ex.Message);
        }

        return ApplicationTypeID;

    }

    public static bool UpdateApplicationType(int ApplicationTypeID, string Title, decimal Fees)
    {

        int rowsAffected = 0;

        string query = @"Update  ApplicationTypes  
                            set ApplicationTypeTitle = @Title,
                                ApplicationFees = @Fees
                                where ApplicationTypeID = @ApplicationTypeID";

        try
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                command.Parameters.AddWithValue("@Title", Title);
                command.Parameters.AddWithValue("@Fees", Fees);

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
