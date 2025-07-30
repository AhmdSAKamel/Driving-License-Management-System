using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using DVLD_DataAccess;




namespace DVLDBuiness
{

    public class clsUser
    {

        public clsPerson Person { get; set; }   // Composition.

        public enum enMode { AddNew = 0, Update = 1 };

        private enMode _Mode;


        public short UserID { get; set; }

        public int PersonID { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool isActive { get; set; }


        public clsUser()
        {
            UserID = -1;
            UserName = null;
            Password = null;
            isActive = false;
            PersonID = -1;
            _Mode = enMode.AddNew;

            Person = new clsPerson();
        }

        private clsUser (short UserID, int PersonID, string UserName, string Password, bool isActive)
        {
            this.isActive = isActive;
            this.UserName = UserName;
            this.Password = Password;
            this.UserID = UserID;
            this.PersonID = PersonID;

            this.Person = clsPerson.Find(PersonID);
            this._Mode = enMode.Update;
        }

        public static clsUser FindByUsernameAndPassword(string UserName, string Password)
        {
            short UserID = -1;
            int PersonID = -1;
            
            bool isActive = false;

            bool Found = UserData.FindUserInfo(UserName, Password, ref UserID, ref PersonID, ref isActive);


            if (Found)
                return new clsUser(UserID, PersonID, UserName, Password, isActive);
            else
                return null;

        }

        public static clsUser FindByPersonID(int PersonID)
        {
            short UserID = -1;
            string UserName = null, Password = null;
            bool isActive = false;


            if (UserData.FindUserByPersonID(PersonID, ref UserID, ref UserName, ref Password, ref isActive))
            {
                return new clsUser(UserID, PersonID, UserName, Password, isActive);
            }
            else
                return null;

        }

        public static clsUser FindByUserID(short UserID)
        {
            int PersonID = -1;
            string UserName = null, Password = null;
            bool isActive = false;


            if (UserData.FindUserByUserID(UserID, ref PersonID, ref UserName, ref Password, ref isActive))
            {
                return new clsUser(UserID, PersonID, UserName, Password, isActive);
            }
            else
                return null;
        }

        public static DataTable GetAllUsers()
        {
            DataTable dt = UserData.RetrieveAllUsers();

            return dt;

        }

        public static bool isUserExistForPersonID(int PersonID)
        {
            return UserData.IsUserExistForPersonID(PersonID);
        }

        public static bool isUserExist(string userName)
        {
            return UserData.isUserExist(userName);
        }

        private bool _AddNewUser()
        {
            this.UserID = UserData.AddNewUser(this.PersonID, UserName, Password, isActive);

            return (this.UserID != 1);
        }

        private bool _UpdateUser()
        {
            return (UserData.UpdateUserInfo(UserID, UserName, Password, isActive));
        }

        public static bool DeleteUser(int UserID)
        {
            return UserData.DeleteUser(UserID);
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    {
                        if (_AddNewUser())
                        {
                            _Mode = enMode.Update;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case enMode.Update:
                    {
                        return _UpdateUser();
                    }

            }

            return false;

        }

    }

}
