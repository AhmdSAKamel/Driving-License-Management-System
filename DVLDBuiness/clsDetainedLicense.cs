using DVLDBuiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class clsDetainedLicense
{
    public enum enMode { AddNew = 0, Update = 1 };
    public enMode Mode = enMode.AddNew;


    public int DetainID { set; get; }
    public int LicenseID { set; get; }
    public DateTime DetainDate { set; get; }
    public decimal FineFees { set; get; }
    public short CreatedByUserID { set; get; }
    public short ReleasedByUserID { set; get; }
    public bool IsReleased { set; get; }
    public DateTime ReleaseDate { set; get; }
    public int ReleaseApplicationID { set; get; }
    

    public clsUser CreatedByUserInfo { set; get; }
    public clsUser ReleasedByUserInfo { set; get; }


    public clsDetainedLicense()
    {
        this.DetainID = -1;
        this.LicenseID = -1;
        this.DetainDate = DateTime.Now;
        this.FineFees = 0;
        this.CreatedByUserID = -1;
        this.IsReleased = false;
        this.ReleaseDate = DateTime.MaxValue;
        this.ReleasedByUserID = 0;
        this.ReleaseApplicationID = -1;


        Mode = enMode.AddNew;
    }

    public clsDetainedLicense(int DetainID, int LicenseID, DateTime DetainDate, decimal FineFees, short CreatedByUserID, 
                             bool IsReleased, DateTime ReleaseDate, short ReleasedByUserID, int ReleaseApplicationID)
    {
        this.DetainID = DetainID;
        this.LicenseID = LicenseID;
        this.DetainDate = DetainDate;
        this.FineFees = FineFees;
        this.CreatedByUserID = CreatedByUserID;
        this.IsReleased = IsReleased;
        this.ReleaseDate = ReleaseDate;
        this.ReleasedByUserID = ReleasedByUserID;
        this.ReleaseApplicationID = ReleaseApplicationID;


        this.CreatedByUserInfo = clsUser.FindByUserID(this.CreatedByUserID);
        this.ReleasedByUserInfo = clsUser.FindByPersonID(this.ReleasedByUserID);

        Mode = enMode.Update;
    }

    private bool _AddNewDetainedLicense()
    {
        this.DetainID = DetainedLicenseData.AddNewDetainedLicense(this.LicenseID, this.DetainDate, this.FineFees, this.CreatedByUserID);

        return (this.DetainID != -1);
    }

    private bool _UpdateDetainedLicense()
    {
        return DetainedLicenseData.UpdateDetainedLicense(this.DetainID, this.LicenseID, this.DetainDate, this.FineFees, this.CreatedByUserID);
    }

    public static clsDetainedLicense Find(int DetainID)
    {
        int LicenseID = -1, ReleaseApplicationID = -1; 
        
        DateTime DetainDate = DateTime.Now, ReleaseDate = DateTime.MaxValue;
        decimal FineFees = 0; 
        short CreatedByUserID = -1, ReleasedByUserID = -1;
        bool IsReleased = false;

        if (DetainedLicenseData.GetDetainedLicenseInfoByID(DetainID, ref LicenseID, ref DetainDate, ref FineFees, ref CreatedByUserID,
                                                       ref IsReleased, ref ReleaseDate, ref ReleasedByUserID, ref ReleaseApplicationID))
        {
            return new clsDetainedLicense(DetainID, LicenseID, DetainDate, FineFees, CreatedByUserID, IsReleased, 
                                          ReleaseDate, ReleasedByUserID, ReleaseApplicationID);
        }
        else
            return null;
    }

    public static DataTable GetAllDetainedLicenses()
    {
        return DetainedLicenseData.GetAllDetainedLicenses();
    }

    public static clsDetainedLicense FindByLicenseID(int LicenseID)
    {      
        int DetainID = -1, ReleaseApplicationID = -1;
        DateTime DetainDate = DateTime.Now, ReleaseDate = DateTime.MaxValue;
        decimal FineFees = 0;
        short CreatedByUserID = -1, ReleasedByUserID = -1;
        bool IsReleased = false;

        if (DetainedLicenseData.GetDetainedLicenseInfoByLicenseID(LicenseID, ref DetainID, ref DetainDate, ref FineFees, ref CreatedByUserID,
                                                              ref IsReleased, ref ReleaseDate, ref ReleasedByUserID, ref ReleaseApplicationID))
        {
            return new clsDetainedLicense(DetainID, LicenseID, DetainDate, FineFees, CreatedByUserID,
                                          IsReleased, ReleaseDate, ReleasedByUserID, ReleaseApplicationID);
        }
        else
            return null;

    }

    public bool Save()
    {

        switch (Mode)
        {
            case enMode.AddNew:
                if (_AddNewDetainedLicense())
                {
                    Mode = enMode.Update;
                    return true;
                }
                else
                {
                    return false;
                }

            case enMode.Update:
                return _UpdateDetainedLicense();

        }

        return false;
    }

    public static bool IsLicenseDetained(int LicenseID)
    {
        return DetainedLicenseData.IsLicenseDetained(LicenseID);
    }

    public bool ReleaseDetainedLicense(short ReleasedByUserID, int ReleaseApplicationID)
    {
        return DetainedLicenseData.ReleaseDetainedLicense(this.DetainID, ReleasedByUserID, ReleaseApplicationID);
    }


}

