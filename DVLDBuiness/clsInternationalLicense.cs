using System;
using System.Data;






public class clsInternationalLicense : clsApplication
{

    public enum enMode { AddNew = 0, Update = 1 };
    public enMode Mode = enMode.AddNew;


    public int InternationalLicenseID { set; get; }
    public int DriverID { set; get; }
    public int IssuedUsingLocalLicenseID { set; get; }
    public DateTime IssueDate { set; get; }
    public DateTime ExpirationDate { set; get; }
    public bool IsActive { set; get; }


    public clsDriver DriverInfo;


    public clsInternationalLicense()
    {
        //here we set the applicaiton type to New International License.
        this.ApplicationTypeID = (int)clsApplication.enApplicationType.NewInternationalLicense;

        this.InternationalLicenseID = -1;
        this.DriverID = -1;
        this.IssuedUsingLocalLicenseID = -1;
        this.IssueDate = DateTime.Now;
        this.ExpirationDate = DateTime.Now;
        this.IsActive = true;


        Mode = enMode.AddNew;
    }

    public clsInternationalLicense(int ApplicationID, int ApplicantPersonID, DateTime ApplicationDate, enApplicationStatus ApplicationStatus, DateTime LastStatusDate, decimal PaidFees, 
                                 short CreatedByUserID, int InternationalLicenseID, int DriverID, int IssuedUsingLocalLicenseID, DateTime IssueDate, DateTime ExpirationDate, bool IsActive)
    {

        base.ApplicationID = ApplicationID;
        base.ApplicantPersonID = ApplicantPersonID;
        base.ApplicationDate = ApplicationDate;
        base.ApplicationTypeID = (int)clsApplication.enApplicationType.NewInternationalLicense;
        base.ApplicationStatus = ApplicationStatus;
        base.LastStatusDate = LastStatusDate;
        base.PaidFees = PaidFees;
        base.CreatedByUserID = CreatedByUserID;

        this.InternationalLicenseID = InternationalLicenseID;
        this.ApplicationID = ApplicationID;
        this.DriverID = DriverID;
        this.IssuedUsingLocalLicenseID = IssuedUsingLocalLicenseID;
        this.IssueDate = IssueDate;
        this.ExpirationDate = ExpirationDate;
        this.IsActive = IsActive;
        this.CreatedByUserID = CreatedByUserID;


        this.DriverInfo = clsDriver.FindByDriverID(this.DriverID);

        Mode = enMode.Update;
    }

    private bool _AddNewInternationalLicense()
    {
        //call DataAccess Layer 

        this.InternationalLicenseID = InternationalLicenseData.AddNewInternationalLicense(this.ApplicationID, this.DriverID, this.IssuedUsingLocalLicenseID, 
                                                                                          this.IssueDate, this.ExpirationDate, this.IsActive, this.CreatedByUserID);

        return (this.InternationalLicenseID != -1);
    }

    private bool _UpdateInternationalLicense()
    {
        return InternationalLicenseData.UpdateInternationalLicense(this.InternationalLicenseID, this.ApplicationID, this.DriverID, this.IssuedUsingLocalLicenseID,
                                                                   this.IssueDate, this.ExpirationDate, this.IsActive, this.CreatedByUserID);
    }

    public static clsInternationalLicense Find(int InternationalLicenseID)
    {
        int ApplicationID = -1, DriverID = -1, IssuedUsingLocalLicenseID = -1;
        DateTime IssueDate = DateTime.Now, ExpirationDate = DateTime.Now;
        bool IsActive = true; 
        short CreatedByUserID = 1;

        if (InternationalLicenseData.GetInternationalLicenseInfoByID(InternationalLicenseID, ref ApplicationID, ref DriverID, ref IssuedUsingLocalLicenseID,
                                                                    ref IssueDate, ref ExpirationDate, ref IsActive, ref CreatedByUserID))
        {
            clsApplication Application = clsApplication.FindBaseApplication(ApplicationID);


            return new clsInternationalLicense(Application.ApplicationID, Application.ApplicantPersonID, Application.ApplicationDate, (enApplicationStatus)Application.ApplicationStatus, Application.LastStatusDate, 
                                               Application.PaidFees, Application.CreatedByUserID, InternationalLicenseID, DriverID, IssuedUsingLocalLicenseID, IssueDate, ExpirationDate, IsActive);
        }

        else
            return null;

    }

    public static DataTable GetAllInternationalLicenses()
    {
        return InternationalLicenseData.GetAllInternationalLicenses();
    }

    public bool Save()
    {

        //Because of inheritance first we call the save method in the base class,
        //it will take care of adding all information to the application table.
       
        base.Mode = (clsApplication.enMode)Mode;
        if (!base.Save())
            return false;

        switch (Mode)
        {
            case enMode.AddNew:
                if (_AddNewInternationalLicense())
                {
                    Mode = enMode.Update;
                    return true;
                }
                else
                {
                    return false;
                }

            case enMode.Update:
                return _UpdateInternationalLicense();

        }

        return false;
    }

    public static int GetActiveInternationalLicenseIDByDriverID(int DriverID)
    {
        return InternationalLicenseData.GetActiveInternationalLicenseIDByDriverID(DriverID);
    }

    public static DataTable GetDriverInternationalLicenses(int DriverID)
    {
        return InternationalLicenseData.GetDriverInternationalLicenses(DriverID);
    }


}





