using System;
using System.Data;
using System.Net;
using System.Security.Policy;
using DVLD_DataAccess;


namespace DVLDBuiness
{
    public class clsPerson
    {
        public enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;

        public int PersonID { set; get; }
        public string NationalNo { set; get; }
        public string FirstName { set; get; }
        public string SecondName { set; get; }
        public string ThirdName { set; get; }
        public string LastName { set; get; }
        public string FullName
        {
            get { return FirstName + " " + SecondName + " " + ThirdName + " " + LastName; }
        }


        public DateTime DateOfBirth { set; get; }
        public bool Gender { set; get; }
        public string Address { set; get; }
        public string Phone { set; get; }
        public string Email { set; get; }
        public int NationalityCountryID { set; get; }
        public clsCountry CountryInfo;
        //public string CountryName { get; set; }
        
        private string _ImagePath { set; get; }

        public string ImagePath
        {
            get { return _ImagePath; }
            set { _ImagePath = value; }
        }

        public clsPerson()
        {
            PersonID = -1;
            NationalNo = "";
            FirstName = string.Empty;
            SecondName = string.Empty;
            ThirdName = string.Empty;
            LastName = string.Empty;
            DateOfBirth = DateTime.Now;
            Gender = false;
            Address = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
            NationalityCountryID = -1;
            ImagePath = "";
            
            _Mode = enMode.AddNew;
        }

        private clsPerson(int PersonID, string Nationalno, string FirstName, string SecondName, string ThirdName,
            string LastName, DateTime DateOfBirth, bool Gender, string Address, string Phone, string Email, byte NationalityCountryID, string ImagePath)
        {
            this.PersonID = PersonID;
            this.NationalNo = Nationalno;
            this.FirstName = FirstName;
            this.SecondName = SecondName;
            this.ThirdName = ThirdName;
            this.LastName = LastName;
            this.DateOfBirth = DateOfBirth;
            this.Gender = Gender;
            this.Address = Address;
            this.Phone = Phone;
            this.Email = Email;
            this.NationalityCountryID = NationalityCountryID;
            this.ImagePath = ImagePath;

            this.CountryInfo = clsCountry.Find(NationalityCountryID);

            _Mode = enMode.Update;
        }

        public static DataTable GetAllPeople()
        {
            DataTable dt = clsPeopleDataAccess.RetrieveAllPeople();

            return dt;
        }

        public static clsPerson Find(int PersonID)
        {
            
            string Nationalno = "", FirstName = "", SecondName = "", ThirdName = "", LastName = "", Email = "", Phone = "", Address = "", ImagePath = "";
            DateTime DateOfBirth = DateTime.Now;
            byte CountryID = 0;
            bool Gender = true;

            bool Found = clsPeopleDataAccess.GetPersonInfoByID(PersonID, ref Nationalno, ref FirstName, ref SecondName, ref ThirdName, ref LastName, ref DateOfBirth,
                                                        ref Gender, ref Address, ref Phone, ref Email, ref CountryID, ref ImagePath);

            if (Found)
            {
                return new clsPerson(PersonID, Nationalno, FirstName, SecondName, ThirdName, LastName, DateOfBirth, Gender, Address, Phone, Email, CountryID, ImagePath);
            }
            else
                return null;

        }

        public static clsPerson Find(string NationalNo)
        {
            int PersonID = -1;
            string FirstName = "", SecondName = "", ThirdName = "", LastName = "", 
                 Email = "", Phone = "", Address = "", ImagePath = "";

            DateTime DateOfBirth = DateTime.Now;
            bool Gender = true;

            byte CountryID = 1;

            bool Found = clsPeopleDataAccess.GetPersonInfoByNationalNo(ref PersonID, NationalNo, ref FirstName, ref SecondName, ref ThirdName, ref LastName, ref DateOfBirth,
                                                        ref Gender, ref Address, ref Phone, ref Email, ref CountryID, ref ImagePath);
            if (Found)
            {
                return new clsPerson(PersonID, NationalNo.ToUpper(), FirstName, SecondName, ThirdName, LastName, DateOfBirth, Gender, Address, Phone, Email, CountryID, ImagePath);
            }
            else
                return null;

        }

        private bool _AddNewPerson()
        {
            this.PersonID = clsPeopleDataAccess.AddNewPerson(this.NationalNo,this.FirstName, this.SecondName, this.ThirdName, this.LastName, 
                this.DateOfBirth, this.Gender, this.Address, this.Phone, this.Email
                , this.NationalityCountryID, this.ImagePath);

            return (this.PersonID != -1);
        }

        private bool _UpdatePerson()
        {
            return (clsPeopleDataAccess.UpdatePersonData(this.PersonID, this.NationalNo, this.FirstName, this.SecondName, this.ThirdName, this.LastName,
                this.DateOfBirth, this.Gender, this.Address, this.Phone, this.Email
                , this.NationalityCountryID, this.ImagePath));
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    {
                        if (_AddNewPerson())
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
                        return _UpdatePerson();
                    }
            }

            return false;
        }

        public static bool DeletePerson(int PersonID)
        {
            if (clsPeopleDataAccess.DeletePerson(PersonID))
            {
                return true;
            }
            else
                return false;
        }

        public bool CanThisPersonBeAUser()
        {
            return clsPeopleDataAccess.CanPersonBeAUser(this.PersonID);
        }

        public static bool isPersonExist(int ID)
        {
            return clsPeopleDataAccess.IsPersonExist(ID);
        }

        public static bool isPersonExist(string NationlNo)
        {
            return clsPeopleDataAccess.IsPersonExist(NationlNo);
        }


    }

}
