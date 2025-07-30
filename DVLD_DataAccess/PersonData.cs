using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;


namespace DVLD_DataAccess
{
    public class clsPeopleDataAccess    // Don't forget to add the code to set imagepath to null if empty.
    {
        public static DataTable RetrieveAllPeople()
        {
            DataTable dt = new DataTable();

            //SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            //string query = "SELECT * FROM People";

            string query = @"SELECT PersonID, NationalNo, FirstName, SecondName, ThirdName, LastName,   
                        DateOfBirth As 'Date Of Birth',  
                        Countries.CountryName As Nationality,
                        Phone,   
                        Email
                        From People Inner Join 
						Countries On People.NationalityCountryID = Countries.CountryID;";

            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                {

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

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                dt = null;
            }

            return dt;

        }

        public static bool GetPersonInfoByID(int PersonID, ref string Nationalno, ref string FirstName,
              ref string SecondName, ref string ThirdName, ref string LastName,
              ref DateTime DateOfBirth, ref bool Gender, ref string Address,
              ref string Phone, ref string Email, ref byte CountryID, ref string ImagePath)
        {
            bool isFound = false;

            string query = @"SELECT TOP 1 * FROM People WHERE (PersonID = @PersonID);";

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;

                            // Use IsDBNull checks to safeguard against nulls  
                            Nationalno = reader.IsDBNull(reader.GetOrdinal("NationalNo")) ? "" : reader["NationalNo"].ToString();
                            FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? "" : reader["FirstName"].ToString();
                            SecondName = reader.IsDBNull(reader.GetOrdinal("SecondName")) ? "" : reader["SecondName"].ToString();
                            ThirdName = reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? "" : reader["ThirdName"].ToString();
                            LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? "" : reader["LastName"].ToString();

                            int dobOrdinal = reader.GetOrdinal("DateOfBirth");
                            DateOfBirth = reader.IsDBNull(dobOrdinal) ? DateTime.MinValue : reader.GetDateTime(dobOrdinal);

                            int genderOrdinal = reader.GetOrdinal("Gender");
                            Gender = reader.IsDBNull(genderOrdinal) ? false : reader.GetBoolean(genderOrdinal);

                            Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? "" : reader["Address"].ToString();
                            Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? "" : reader["Phone"].ToString();
                            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader["Email"].ToString();

                            int countryOrdinal = reader.GetOrdinal("NationalityCountryID");
                            int tempValue = reader.IsDBNull(countryOrdinal) ? 0 : reader.GetInt32(countryOrdinal);
                            CountryID = (byte)tempValue;

                            int imgOrdinal = reader.GetOrdinal("ImagePath");
                            ImagePath = reader.IsDBNull(imgOrdinal) ? "" : reader["ImagePath"].ToString();

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

        public static bool GetPersonInfoByNationalNo(ref int PersonID, string Nationalno, ref string FirstName,
                ref string SecondName, ref string ThirdName, ref string LastName,
                ref DateTime DateOfBirth, ref bool Gender, ref string Address,
                ref string Phone, ref string Email, ref byte CountryID, ref string ImagePath)
        {
            bool isFound = false;

            string query = @"SELECT TOP 1 * FROM People WHERE (Nationalno = @Nationalno);";


            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", Nationalno);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;


                            PersonID = (int)reader["PersonID"];                           
                            FirstName = reader["FirstName"] as string;
                            SecondName = reader["SecondName"] as string;
                            ThirdName = reader["ThirdName"] as string;
                            LastName = reader["LastName"] as string;
                            DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth"));
                            Gender = reader.GetBoolean(reader.GetOrdinal("Gender"));
                            Address = reader["Address"] as string;
                            Phone = reader["Phone"] as string;
                            Email = reader["Email"] as string;

                            int countrynationalID = (int)reader["NationalityCountryID"];
                            CountryID = (byte)countrynationalID;

                            ImagePath = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : "";

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

        public static int AddNewPerson(string Nationalno, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth, bool Gender, string Address, string Phone, string Email, int NationalityCountryID, string ImagePath)
        {
            int PersonID = -1;



            //string querym = @"INSERT INTO People (Nationalno ,FirstName, SecondName ,ThirdName ,LastName, Email, Phone ,Gender ,Address, DateOfBirth, NationalityCountryID ,ImagePath)
            //                 VALUES (@Nationalno, @FirstName, @SecondName , @ThirdName ,@LastName, @Email, @Phone, @Address,@DateOfBirth, @NationalityCountryID ,@ImagePath, @Gender);
            //                SELECT SCOPE_IDENTITY();";

            string query = @"INSERT INTO People (Nationalno, FirstName, SecondName, ThirdName, LastName, Email, Phone, Gender, Address, DateOfBirth, NationalityCountryID, ImagePath)  
                 VALUES (@Nationalno, @FirstName, @SecondName, @ThirdName, @LastName, @Email, @Phone, @Gender, @Address, @DateOfBirth, @NationalityCountryID, @ImagePath);  
                 SELECT SCOPE_IDENTITY();";         // You should focus on the Parameter Positioning.


            
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nationalno", Nationalno);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@SecondName", SecondName);
                    command.Parameters.AddWithValue("@ThirdName", ThirdName);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                    command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
                    command.Parameters.AddWithValue("@Gender", Gender);

                    if (!string.IsNullOrEmpty(ImagePath))
                        command.Parameters.AddWithValue("@ImagePath", ImagePath);
                    else
                        command.Parameters.AddWithValue("@ImagePath", System.DBNull.Value);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        PersonID = insertedID;
                    }

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

            return PersonID;

        }


        public static bool UpdatePersonData(int PersonID, string Nationalno, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth, bool Gender, string Address, string Phone, string Email, int NationalityCountryID, string ImagePath)
        {

            int AffectedRows = 0;

            string query = @"Update people  
                            set 
                                Nationalno = @NationalNo, 
                                FirstName = @FirstName, 
                                SecondName = @SecondName, 
                                ThirdName = @ThirdName, 
                                LastName = @LastName, 
                                Email = @Email, 
                                Phone = @Phone, 
                                Gender = @Gender, 
                                Address = @Address, 
                                DateOfBirth = @DateOfBirth,
                                NationalityCountryID = @NationalityCountryID, 
                                ImagePath = @ImagePath
                                Where PersonID = @PersonID";

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@Nationalno", Nationalno);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@SecondName", SecondName);
                    command.Parameters.AddWithValue("@ThirdName", ThirdName);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                    command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
                    command.Parameters.AddWithValue("@Gender", Gender);


                    if (!string.IsNullOrEmpty(ImagePath))
                    {
                        command.Parameters.AddWithValue("@ImagePath", ImagePath);
                    }
                    else
                    {
                        // Handle null if ImagePath is optional
                        command.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                    }


                    connection.Open();
                    AffectedRows = command.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                return false;
            }

            return (AffectedRows > 0);

        }

        public static bool DeletePerson(int PersonID)
        {
            int affectedrows = 0;

            string query = @"Delete People Where PersonID = @PersonID";

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@PersonID", PersonID);

                        connection.Open();
                        affectedrows = command.ExecuteNonQuery();
                    
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
                return false;
            }

            return (affectedrows > 0);

        }
         
        public static bool CanPersonBeAUser(int PersonID)
        {
            bool CanBeAUser = false;

            string query = "SELECT Found=1 FROM Users WHERE PersonID = @PersonID";

            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                CanBeAUser = false;
                            }
                            else
                            {
                                CanBeAUser = true;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);

            }


            return CanBeAUser;
        }

        public static bool IsPersonExist(int PersonID)
        {
            bool isFound = false;

            string query = "SELECT Found=1 FROM People WHERE PersonID = @PersonID";

            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@PersonID", PersonID);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        isFound = reader.HasRows;
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

        public static bool IsPersonExist(string NationalNo)
        {
            bool isFound = false;

            string query = "SELECT Found=1 FROM People WHERE NationalNo = @NationalNo";

            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@NationalNo", NationalNo);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        isFound = reader.HasRows;
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

    }

}
