﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class clsLicenseClass
{

    public enum enMode { AddNew = 0, Update = 1 };
    public enMode Mode = enMode.AddNew;

    public int LicenseClassID { set; get; }
    public string ClassName { set; get; }
    public string ClassDescription { set; get; }
    public byte MinimumAllowedAge { set; get; }
    public byte DefaultValidityLength { set; get; }
    public decimal ClassFees { set; get; }

    public clsLicenseClass()
    {
        this.LicenseClassID = -1;
        this.ClassName = "";
        this.ClassDescription = "";
        this.MinimumAllowedAge = 18;
        this.DefaultValidityLength = 10;
        this.ClassFees = 0;

        Mode = enMode.AddNew;

    }

    public clsLicenseClass(int LicenseClassID, string ClassName, string ClassDescription,
                           byte MinimumAllowedAge, byte DefaultValidityLength, decimal ClassFees)

    {
        this.LicenseClassID = LicenseClassID;
        this.ClassName = ClassName;
        this.ClassDescription = ClassDescription;
        this.MinimumAllowedAge = MinimumAllowedAge;
        this.DefaultValidityLength = DefaultValidityLength;
        this.ClassFees = ClassFees;
        Mode = enMode.Update;
    }

    private bool _AddNewLicenseClass()
    {

        this.LicenseClassID = LicenseClassData.AddNewLicenseClass(this.ClassName, this.ClassDescription, this.MinimumAllowedAge, 
                                                                  this.DefaultValidityLength, this.ClassFees);

        return (this.LicenseClassID != -1);
    }

    private bool _UpdateLicenseClass()
    {
        return LicenseClassData.UpdateLicenseClass(this.LicenseClassID, this.ClassName, this.ClassDescription,
                                                   this.MinimumAllowedAge, this.DefaultValidityLength, this.ClassFees);
    }

    public static clsLicenseClass Find(int LicenseClassID)
    {

        string ClassName = ""; string ClassDescription = "";
        byte MinimumAllowedAge = 18; byte DefaultValidityLength = 10; decimal ClassFees = 0;

        if (LicenseClassData.GetLicenseClassInfoByID(LicenseClassID, ref ClassName, ref ClassDescription,
                                         ref MinimumAllowedAge, ref DefaultValidityLength, ref ClassFees))
        {
            return new clsLicenseClass(LicenseClassID, ClassName, ClassDescription,
                MinimumAllowedAge, DefaultValidityLength, ClassFees);
        }

        else
            return null;

    }

    public static clsLicenseClass Find(string ClassName)
    {
        int LicenseClassID = -1; string ClassDescription = "";
        byte MinimumAllowedAge = 18; byte DefaultValidityLength = 10; decimal ClassFees = 0;

        if (LicenseClassData.GetLicenseClassInfoByClassName(ClassName, ref LicenseClassID, ref ClassDescription,
                                                ref MinimumAllowedAge, ref DefaultValidityLength, ref ClassFees))
        {
            return new clsLicenseClass(LicenseClassID, ClassName, ClassDescription,
                MinimumAllowedAge, DefaultValidityLength, ClassFees);
        }
        else
            return null;

    }

    public static DataTable GetAllLicenseClasses()
    {
        return LicenseClassData.GetAllLicenseClasses();
    }

    public bool Save()
    {
        switch (Mode)
        {
            case enMode.AddNew:
                if (_AddNewLicenseClass())
                {

                    Mode = enMode.Update;
                    return true;
                }
                else
                {
                    return false;
                }

            case enMode.Update:

                return _UpdateLicenseClass();

        }

        return false;
    }

}

