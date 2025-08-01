﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class clsLicense
{

    public enum enMode { AddNew = 0, Update = 1 };
    public enMode Mode = enMode.AddNew;

    public enum enIssueReason { FirstTime = 1, Renew = 2, DamagedReplacement = 3, LostReplacement = 4 };

    public int LicenseID { set; get; }
    public int ApplicationID { set; get; }
    public int DriverID { set; get; }
    public int LicenseClass { set; get; }
    public DateTime IssueDate { set; get; }
    public DateTime ExpirationDate { set; get; }
    public string Notes { set; get; }
    public decimal PaidFees { set; get; }
    public bool IsActive { set; get; }
    public int CreatedByUserID { set; get; }

    public enIssueReason IssueReason { set; get; }

    public clsDetainedLicense DetainedInfo { set; get; }
    public clsLicenseClass LicenseClassInfo;
    public clsDriver DriverInfo;

    public string IssueReasonText
    {
        get
        {
            return GetIssueReasonText(this.IssueReason);
        }
    }

    public bool IsDetained
    {
        get { return clsDetainedLicense.IsLicenseDetained(this.LicenseID); }
    }

    
    public clsLicense()
    {
        this.LicenseID = -1;
        this.ApplicationID = -1;
        this.DriverID = -1;
        this.LicenseClass = -1;
        this.IssueDate = DateTime.Now;
        this.ExpirationDate = DateTime.Now;
        this.Notes = "";
        this.PaidFees = 0;
        this.IsActive = true;
        this.IssueReason = enIssueReason.FirstTime;
        this.CreatedByUserID = -1;


        Mode = enMode.AddNew;
    }

    public clsLicense(int LicenseID, int ApplicationID, int DriverID, int LicenseClass, DateTime IssueDate, DateTime ExpirationDate, 
                      string Notes, decimal PaidFees, bool IsActive, enIssueReason IssueReason, int CreatedByUserID)
    {

        this.LicenseID = LicenseID;
        this.ApplicationID = ApplicationID;
        this.DriverID = DriverID;
        this.LicenseClass = LicenseClass;
        this.IssueDate = IssueDate;
        this.ExpirationDate = ExpirationDate;
        this.Notes = Notes;
        this.PaidFees = PaidFees;
        this.IsActive = IsActive;
        this.IssueReason = IssueReason;
        this.CreatedByUserID = CreatedByUserID;

        this.DriverInfo = clsDriver.FindByDriverID(this.DriverID);
        this.LicenseClassInfo = clsLicenseClass.Find(this.LicenseClass);
        this.DetainedInfo = clsDetainedLicense.FindByLicenseID(this.LicenseID);

        Mode = enMode.Update;
    }

    private bool _AddNewLicense()
    {

        this.LicenseID = LicenseData.AddNewLicense(this.ApplicationID, this.DriverID, this.LicenseClass, this.IssueDate, this.ExpirationDate, 
                                                   this.Notes, this.PaidFees, this.IsActive, (byte)this.IssueReason, this.CreatedByUserID);

        return (this.LicenseID != -1);
    }

    private bool _UpdateLicense()
    {
        return LicenseData.UpdateLicense(this.ApplicationID, this.LicenseID, this.DriverID, this.LicenseClass, this.IssueDate, 
                                         this.ExpirationDate, this.Notes, this.PaidFees, this.IsActive, (byte)this.IssueReason, this.CreatedByUserID);
    }

    public static clsLicense Find(int LicenseID)
    {
        int ApplicationID = -1, DriverID = -1, LicenseClass = -1, CreatedByUserID = 1;
        DateTime IssueDate = DateTime.Now; DateTime ExpirationDate = DateTime.Now;
        string Notes = "";
        decimal PaidFees = 0; bool IsActive = true; 
        byte IssueReason = 1;

        if (LicenseData.GetLicenseInfoByID(LicenseID, ref ApplicationID, ref DriverID, ref LicenseClass, ref IssueDate, ref ExpirationDate,
                                            ref Notes, ref PaidFees, ref IsActive, ref IssueReason, ref CreatedByUserID))
        {
            return new clsLicense(LicenseID, ApplicationID, DriverID, LicenseClass, IssueDate, ExpirationDate,
                                   Notes, PaidFees, IsActive, (enIssueReason)IssueReason, CreatedByUserID);
        }

        else
            return null;
    }

    public static DataTable GetAllLicenses()
    {
        return LicenseData.GetAllLicenses();
    }

    public bool Save()
    {

        switch (Mode)
        {
            case enMode.AddNew:
                if (_AddNewLicense())
                {

                    Mode = enMode.Update;
                    return true;
                }
                else
                {
                    return false;
                }

            case enMode.Update:

                return _UpdateLicense();

        }

        return false;
    }

    public static bool IsLicenseExistByPersonID(int PersonID, int LicenseClassID)
    {
        return (GetActiveLicenseIDByPersonID(PersonID, LicenseClassID) != -1);
    }

    public static int GetActiveLicenseIDByPersonID(int PersonID, int LicenseClassID)
    {
        return LicenseData.GetActiveLicenseIDByPersonID(PersonID, LicenseClassID);
    }

    public static DataTable GetDriverLicenses(int DriverID)
    {
        return LicenseData.GetDriverLicenses(DriverID);
    }

    public bool IsLicenseExpired()
    {
        return (this.ExpirationDate < DateTime.Now);
    }

    public bool DeactivateCurrentLicense()
    {
        return (LicenseData.DeactivateLicense(this.LicenseID));
    }

    public static string GetIssueReasonText(enIssueReason IssueReason)
    {

        switch (IssueReason)
        {
            case enIssueReason.FirstTime:
                return "First Time";
            case enIssueReason.Renew:
                return "Renew";
            case enIssueReason.DamagedReplacement:
                return "Replacement for Damaged";
            case enIssueReason.LostReplacement:
                return "Replacement for Lost";
            default:
                return "First Time";
        }
    }

    public int Detain(decimal FineFees, short CreatedByUserID)
    {
        clsDetainedLicense detainedLicense = new clsDetainedLicense();

        detainedLicense.LicenseID = this.LicenseID;
        detainedLicense.DetainDate = DateTime.Now;
        detainedLicense.FineFees = FineFees;
        detainedLicense.CreatedByUserID = CreatedByUserID;

        if (!detainedLicense.Save())
        {
            return -1;
        }

        return detainedLicense.DetainID;
    }

    public bool ReleaseDetainedLicense(short ReleasedByUserID, ref int ApplicationID)
    {
        clsApplication Application = new clsApplication();

        Application.ApplicantPersonID = this.DriverInfo.PersonID;
        Application.ApplicationDate = DateTime.Now;
        Application.ApplicationTypeID = (int)clsApplication.enApplicationType.ReleaseDetainedDrivingLicsense;
        Application.ApplicationStatus = clsApplication.enApplicationStatus.Completed;
        Application.LastStatusDate = DateTime.Now;
        Application.PaidFees = clsApplicationType.Find((int)clsApplication.enApplicationType.ReleaseDetainedDrivingLicsense).Fees;
        Application.CreatedByUserID = ReleasedByUserID;


        if (!Application.Save())
        {
            ApplicationID = -1;
            return false;
        }

        ApplicationID = Application.ApplicationID;

        return this.DetainedInfo.ReleaseDetainedLicense(ReleasedByUserID, Application.ApplicationID);
    }

    public clsLicense RenewLicense(string Notes, short CreatedByUserID)
    {
        clsApplication Application = new clsApplication();

        Application.ApplicantPersonID = this.DriverInfo.PersonID;
        Application.ApplicationDate = DateTime.Now;
        Application.ApplicationTypeID = (int)clsApplication.enApplicationType.RenewDrivingLicense;
        Application.ApplicationStatus = clsApplication.enApplicationStatus.Completed;
        Application.LastStatusDate = DateTime.Now;
        Application.PaidFees = clsApplicationType.Find((int)clsApplication.enApplicationType.RenewDrivingLicense).Fees;
        Application.CreatedByUserID = CreatedByUserID;


        if (!Application.Save())
        {
            return null;
        }

        clsLicense NewLicense = new clsLicense();


        NewLicense.ApplicationID = Application.ApplicationID;
        NewLicense.DriverID = this.DriverID;
        NewLicense.LicenseClass = this.LicenseClass;
        NewLicense.IssueDate = DateTime.Now;

        int DefaultValidityLength = this.LicenseClassInfo.DefaultValidityLength;

        NewLicense.ExpirationDate = DateTime.Now.AddYears(DefaultValidityLength);
        NewLicense.Notes = Notes;
        NewLicense.PaidFees = this.LicenseClassInfo.ClassFees;
        NewLicense.IsActive = true;
        NewLicense.IssueReason = clsLicense.enIssueReason.Renew;
        NewLicense.CreatedByUserID = CreatedByUserID;


        if (!NewLicense.Save())
        {
            return null;
        }

        //We should deactivate the old License.
        DeactivateCurrentLicense();

        return NewLicense;
    }

    public clsLicense Replace(enIssueReason IssueReason, short CreatedByUserID)
    {

        //First Create Applicaiton 
        clsApplication Application = new clsApplication();


        Application.ApplicantPersonID = this.DriverInfo.PersonID;
        Application.ApplicationDate = DateTime.Now;


        Application.ApplicationTypeID = (IssueReason == enIssueReason.DamagedReplacement)
            ?
            (byte)clsApplication.enApplicationType.ReplaceDamagedDrivingLicense :
            (byte)clsApplication.enApplicationType.ReplaceLostDrivingLicense;

        Application.ApplicationStatus = clsApplication.enApplicationStatus.Completed;
        Application.LastStatusDate = DateTime.Now;
        Application.PaidFees = clsApplicationType.Find(Application.ApplicationTypeID).Fees;
        Application.CreatedByUserID = CreatedByUserID;

        if (!Application.Save())
        {
            return null;
        }

        clsLicense NewLicense = new clsLicense();

        NewLicense.ApplicationID = Application.ApplicationID;
        NewLicense.DriverID = this.DriverID;
        NewLicense.LicenseClass = this.LicenseClass;
        NewLicense.IssueDate = DateTime.Now;
        NewLicense.ExpirationDate = this.ExpirationDate;
        NewLicense.Notes = this.Notes;
        NewLicense.PaidFees = 0;// no fees for the license because it's a replacement.
        NewLicense.IsActive = true;
        NewLicense.IssueReason = IssueReason;
        NewLicense.CreatedByUserID = CreatedByUserID;



        if (!NewLicense.Save())
        {
            return null;
        }

        DeactivateCurrentLicense();

        return NewLicense;
    }

}
