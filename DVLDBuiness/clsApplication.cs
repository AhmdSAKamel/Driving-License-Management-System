using System;
using DVLDBuiness;
using System.Collections.Generic;
using System.Threading.Tasks;



public class clsApplication
{

    public clsUser CreatedByUserInfo;
    public clsApplicationType ApplicationTypeInfo;
    public clsPerson PersonInfo;



    public enum enMode { AddNew = 0, Update = 1 };
    public enMode Mode = enMode.AddNew;

    public enum enApplicationType
    {
        NewDrivingLicense = 1, RenewDrivingLicense = 2, ReplaceLostDrivingLicense = 3,
        ReplaceDamagedDrivingLicense = 4, ReleaseDetainedDrivingLicsense = 5, NewInternationalLicense = 6, RetakeTest = 7
    }

    public enum enApplicationStatus { New = 1, Cancelled = 2, Completed = 3 };
    public enApplicationStatus ApplicationStatus { set; get; }


    public int ApplicationID { set; get; }

    public int ApplicantPersonID { set; get; }

    public string ApplicantFullName
    {
        get
        {
            return clsPerson.Find(ApplicantPersonID).FullName;
        }
    }

    public DateTime ApplicationDate { set; get; }

    public byte ApplicationTypeID { set; get; }

    public string StatusText
    {
        get
        {
            switch (ApplicationStatus)
            {
                case enApplicationStatus.New:
                    return "New";
                case enApplicationStatus.Cancelled:
                    return "Cancelled";
                case enApplicationStatus.Completed:
                    return "Completed";
                default:
                    return "Unknown";
            }

        }

    }

    public DateTime LastStatusDate { set; get; }

    public decimal PaidFees { set; get; }

    public short CreatedByUserID { set; get; }


    public clsApplication()
    {
        ApplicationID = -1;
        ApplicantPersonID = -1;
        ApplicationDate = DateTime.Now;
        ApplicationTypeID = 0;
        ApplicationStatus = enApplicationStatus.New;
        LastStatusDate = DateTime.Now;
        PaidFees = 0;
        CreatedByUserID = -1;


        Mode = enMode.AddNew;
    }


    private clsApplication(int ApplicationID, int ApplicantPersonID, DateTime ApplicationDate, byte ApplicationTypeID,
        enApplicationStatus ApplicationStatus, DateTime LastStatusDate, decimal PaidFees, short CreatedByUserID)
    {
        this.ApplicationID = ApplicationID;
        this.ApplicantPersonID = ApplicantPersonID;
        this.ApplicationDate = ApplicationDate;
        this.ApplicationTypeID = ApplicationTypeID;
        this.ApplicationStatus = ApplicationStatus;
        this.LastStatusDate = LastStatusDate;
        this.PaidFees = PaidFees;
        this.CreatedByUserID = CreatedByUserID;


        this.ApplicationTypeInfo = clsApplicationType.FindApplTypeByID(ApplicationTypeID);
        CreatedByUserInfo = clsUser.FindByUserID(CreatedByUserID);
        this.PersonInfo = clsPerson.Find(this.ApplicantPersonID);


        Mode = enMode.Update;
    }

    private bool _AddNewApplication()
    {

        ApplicationID = ApplicationData.AddNewApplication(ApplicantPersonID, ApplicationDate, ApplicationTypeID, (byte)ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID);

        return (this.ApplicationID != -1);

    }

    private bool _UpdateApplication()
    {

        return ApplicationData.UpdateApplication(ApplicationID, ApplicantPersonID, ApplicationDate, ApplicationTypeID, (byte)this.ApplicationStatus,
            this.LastStatusDate, PaidFees, CreatedByUserID);

    }

    public static clsApplication FindBaseApplication(int ApplicationID)
    {

        int ApplicantPersonID = -1;
        short CreatedByUserID = -1;
        DateTime ApplicationDate = DateTime.Now;
        byte ApplicationStatus = 0, ApplicationTypeID = 0; DateTime LastStatusDate = DateTime.Now;
        decimal PaidFees = 0;


        bool IsFound = ApplicationData.GetApplicationInfoByID(ApplicationID, ref ApplicantPersonID, ref ApplicationDate, ref ApplicationTypeID,
                                ref ApplicationStatus, ref LastStatusDate, ref PaidFees, ref CreatedByUserID);

        if (IsFound)
        {
            return new clsApplication(ApplicationID, ApplicantPersonID, ApplicationDate, ApplicationTypeID,
                             (enApplicationStatus)ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID);
        }
        else
            return null;

    }

    public bool CancelApplication()
    {
        return ApplicationData.UpdateStatus(ApplicationID, 2);
    }

    public bool SetComplete()
    {
        return ApplicationData.UpdateStatus(ApplicationID, 3);
    }

    public bool Save()
    {
        switch (Mode)
        {
            case enMode.AddNew:
                {
                    if (_AddNewApplication())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            case enMode.Update:
                {
                    return _UpdateApplication();
                }

        }

        return false;
    }

    public bool Delete()
    {
        return ApplicationData.DeleteApplication(ApplicationID);
    }

    public static bool IsApplicationExist(int ApplicationID)
    {
        return ApplicationData.IsApplicationExist(ApplicationID);
    }

    public static bool DoesPersonHaveActiveApplication(int PersonID, int ApplicationTypeID)
    {
        return ApplicationData.DoesPersonHaveActiveApplication(PersonID, ApplicationTypeID);
    }

    public bool DoesPersonHaveActiveApplication(int ApplicationTypeID)
    {
        return DoesPersonHaveActiveApplication(this.ApplicantPersonID, ApplicationTypeID);
    }

    public static int GetActiveApplicationID(int PersonID, clsApplication.enApplicationType ApplicationTypeID)
    {
        return ApplicationData.GetActiveApplicationID(PersonID, (int)ApplicationTypeID);
    }

    public static int GetActiveApplicationIDForLicenseClass(int PersonID, clsApplication.enApplicationType ApplicationTypeID, int LicenseClassID)
    {
        return ApplicationData.GetActiveApplicationIDForLicenseClass(PersonID, (int)ApplicationTypeID, LicenseClassID);
    }

    public int GetActiveApplicationID(clsApplication.enApplicationType ApplicationTypeID)
    {
        return GetActiveApplicationID(this.ApplicantPersonID, ApplicationTypeID);
    }

}