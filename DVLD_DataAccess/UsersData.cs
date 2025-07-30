using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Linq.Expressions;
using System.Diagnostics;


namespace DVLD_DataAccess
{

    public class UserData
    {
        public static bool FindUserInfo(string UserName, string Password, ref short UserID, ref int PersonID, ref bool IsActive)
        {
            bool isFound = false;
            string query = "Select UserID, PersonID, isActive From Users Where(UserName = @UserName And Password = @Password)";


            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                       
                        if (reader.Read())
                        {
                            isFound = true;

                            UserID = Convert.ToInt16(reader["UserID"]);
                            PersonID = (int)reader["PersonID"];
                            IsActive = (bool)reader["IsActive"];
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

        public static DataTable RetrieveAllUsers()
        {
            DataTable dt = new DataTable();


            string query = @"SELECT        Users.UserID, Users.PersonID, (People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName) As FullName,
                                           Users.UserName, Users.IsActive
                             FROM          People INNER JOIN
                                           Users ON People.PersonID = Users.PersonID";


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
                dt = null;
            }


            return dt;
        }

        public static short AddNewUser(int PersonID, string UserName, string Password, bool isActive)
        {
            short UserID = -1;

            string query = "Insert Into Users(PersonID, UserName, Password, isActive)" +
                    "Values(@PersonID ,@UserName, @Password, @isActive );" +
                    "SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {                    

                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);
                    command.Parameters.AddWithValue("@isActive", isActive);
                    
                    connection.Open();


                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        UserID = (short)insertedID;
                    }

                }

            }
            catch
            {
                UserID = -1;
            }

            return UserID;
        }

        public static bool FindUserByPersonID(int PersonID, ref short UserID, ref string UserName, ref string Password, ref bool isActive) 
        {
            bool isFound = false;

            string query = @"SELECT TOP 1 * FROM Users WHERE PersonID = @PersonID";

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

                            int user = reader.IsDBNull(reader.GetOrdinal("UserID")) ? -1 : (int)reader["UserID"];
                            UserID = (short)user;
                            UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? "" : reader["UserName"].ToString();
                            Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? "" : reader["Password"].ToString();
                            isActive = reader.IsDBNull(reader.GetOrdinal("isActive")) ? false : (bool)reader["isActive"];


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

        public static bool FindUserByUserID(int UserID, ref int PersonID, ref string UserName, ref string Password, ref bool isActive)
        {
            bool isFound = false;

            string query = @"SELECT TOP 1 * FROM Users WHERE UserID = @UserID";


            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            PersonID = reader.IsDBNull(reader.GetOrdinal("PersonID")) ? -1 : (int)reader["PersonID"];
                            UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? "" : reader["UserName"].ToString();
                            Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? "" : reader["Password"].ToString();
                            isActive = reader.IsDBNull(reader.GetOrdinal("isActive")) ? false : (bool)reader["isActive"];
                            
                            isFound = true;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

            return isFound;

        }

        public static bool UpdateUserInfo(int UserID, string UserName, string Password, bool isActive)
        {
            int AffectedRows = 0;

            string query = @"Update Users
                             Set 
                                 UserName = @UserName,
                                 Password = @Password,
                                 isActive = @isActive
                                 Where (UserID = @UserID);";


            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", UserName);
                        command.Parameters.AddWithValue("@Password", Password);
                        command.Parameters.AddWithValue("@isActive", isActive);

                        command.Parameters.AddWithValue("@UserID", UserID);

                        connection.Open();

                        AffectedRows = command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

            return (AffectedRows > 0);

        }

        public static bool DeleteUser(int UserID)
        {

            int affectedrows = 0;

            string query = @"Delete Users Where UserID = @UserID";

            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                {
                    connection.Open(); // Open the connection  
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", UserID);
                        affectedrows = command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

            return (affectedrows > 0);

        }

        public static bool IsUserExistForPersonID(int PersonID)
        {
            bool isFound = false;

            string query = "SELECT Found=1 FROM Users WHERE PersonID = @PersonID";

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
            }

            return isFound;
        }

        public static bool isUserExist(string UserName)
        {

            bool isFound = false;

            string query = "SELECT Found = 1 FROM Users WHERE UserName = @UserName";

            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        isFound = reader.HasRows;

                        reader.Close();
                    }

                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteExceptionToEventViewer(ex.Message);
            }

            return isFound;

        }

    }

}
