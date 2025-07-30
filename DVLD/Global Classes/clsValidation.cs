using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;


public class clsValidation
{

    public static bool ValidateEmail(string emailAddress)
    {
        // This is the regular expression to make sure that email address is valid.

        var pattern = @"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

        var regex = new Regex(pattern);

        return regex.IsMatch(emailAddress); // It compares if the email we send matches the pattern!
    }

    public static bool ValidateInteger(string Number)
    {
        var pattern = @"^[0-9]*$";

        var regex = new Regex(pattern);

        return regex.IsMatch(Number);
    }

    public static bool ValidateFloat(string Number)
    {
        var pattern = @"^[0-9]*(?:\.[0-9]*)?$";

        var regex = new Regex(pattern);

        return regex.IsMatch(Number);
    }

    public static bool IsNumber(string Number)
    {
        return (ValidateInteger(Number) || ValidateFloat(Number));
    }

    public static string ComputeHash(string Input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Input));

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }


}

