using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


public class clsApplicationType
{


    public enum enMode
    {
        AddNew = 1, Update = 2
    }
    private enMode _Mode;

    public byte ApplicationTypeID { get; set; }

    public string Title { get; set; }

    public decimal Fees { get; set; }

    public clsApplicationType()
    {
        ApplicationTypeID = 0;
        Title = "";
        Fees = 0;

        _Mode = enMode.AddNew;
    }

    private clsApplicationType(byte applicationID, string title, decimal fees)
    {
        ApplicationTypeID = applicationID;
        Title = title;
        Fees = fees;

        _Mode = enMode.Update;
    }

    public static DataTable GetAllApplicationsTypes()
    {
        return ApplicationTypesData.GetAllApplicationTypes();
    }

    public static clsApplicationType FindApplTypeByID(byte AppID)
    {
        string Title = "";
        decimal fees = -1;

        /*ref:
                The variable must be initialized before passing to the method.
                The method can read and modify its value.

         out:
                The variable does not need to be initialized before passing.
                The method must assign a value before returning.*/


        if (ApplicationTypesData.GetApplicationTypeInfoByID(AppID, ref Title, ref fees))
        {
            return new clsApplicationType(AppID, Title, fees);
        }
        else
        {
            return null;
        }

    }

    private bool UpdateAppInfo() 
    {
        return ApplicationTypesData.UpdateApplicationType(ApplicationTypeID, Title, Fees);
    }

    public bool Save()
    {

        switch (_Mode)
        {
            case enMode.AddNew:
                {
                    break;
                }
            case enMode.Update:
                {
                    return UpdateAppInfo();
                }
        }

        return false;

    }

    public static clsApplicationType Find(byte ID)
    {
       
        string Title = ""; decimal Fees = 0;

        if (ApplicationTypesData.GetApplicationTypeInfoByID(ID, ref Title, ref Fees))
            { return new clsApplicationType(ID, Title, Fees);}
        else
            return null;

    }

}



