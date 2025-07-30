using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace DVLDBuiness
{
    public class clsTestType
    {
        public enum enMode { AddNew = 0, Update = 1 };

        private enMode Mode;

        public enum enTestType { VisionTest = 1, WrittenTest = 2, StreetTest = 3 };
        public enTestType ID { set; get; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Fees { get; set; }


        public clsTestType()
        {
            this.ID = clsTestType.enTestType.VisionTest;
            Title = "";
            Description = "";
            
            Fees = -1;
            Mode = enMode.AddNew;
        }

        public clsTestType(clsTestType.enTestType ID, string TestTypeTitel, string Description, decimal TestTypeFees)
        {
            this.ID = ID;
            this.Title = TestTypeTitel;
            this.Description = Description;

            this.Fees = TestTypeFees;
            Mode = enMode.Update;
        }

        public static DataTable GetAllTestTypes()
        {
            DataTable dt = TestTypeData.GetAllTestTypes();

            return dt;
        }

        public static clsTestType FindTestTypeByTestTypeID(byte testtypeid)
        {
            string testtypename = "", testtypedescription = "";

            decimal testtypefees = -1;

            if (TestTypeData.GetTestTypeInfoByID(testtypeid, ref testtypename, ref testtypedescription, ref testtypefees))
            {
                return new clsTestType((enTestType)testtypeid, testtypename, testtypedescription, testtypefees);
            }
            
            return null;
        }

        private bool _AddNewTestType()
        {
            //call DataAccess Layer 

            this.ID = (clsTestType.enTestType)TestTypeData.AddNewTestType(this.Title, this.Description, this.Fees);

            return (this.Title != "");
        }

        private bool _UpdateTestType()
        {
            return (TestTypeData.UpdateTestType((int)ID, Title, Description, Fees));
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    {
                        //if (_AddNewUser())
                        //{
                        //    _Mode = enMode.Update;
                        //    return true;
                        //}
                        //else
                        //{
                        //    return false;
                        //}
                        break;
                    }
                case enMode.Update:
                    {
                        return _UpdateTestType();
                    }

            }

            return false;

        }

        public static clsTestType Find(clsTestType.enTestType TestTypeID)
        {
            string Title = "", Description = ""; decimal Fees = 0;

            if (TestTypeData.GetTestTypeInfoByID((int)TestTypeID, ref Title, ref Description, ref Fees))

                return new clsTestType(TestTypeID, Title, Description, Fees);
            else
                return null;

        }

    }

}
