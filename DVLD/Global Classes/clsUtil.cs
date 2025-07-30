using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLDBuiness;


public static class clsUtil
{
    public static StringBuilder GUIDAddress = new StringBuilder();


    static string folderName = ConfigurationManager.AppSettings["ImageFolderPath"];
    static string folderPath = Path.Combine(Application.StartupPath, folderName);

    public static string GenerateGUID()
    {

        // Generate a new GUID
        Guid newGuid = Guid.NewGuid();

        GUIDAddress = new StringBuilder(newGuid.ToString());

        // convert the GUID to a string
        return newGuid.ToString();
    }

    private static bool CreateFolderIfDoesNotExist(string FolderPath)
    {

        // Check if the folder exists
        if (!Directory.Exists(FolderPath))
        {
            try
            {
                // If it doesn't exist, create the folder
                Directory.CreateDirectory(FolderPath);
                return true;
            }
            catch //(Exception ex)
            {
                //MessageBox.Show("Error creating folder: " + ex.Message);
                return false;
            }

        }

        return true;

    }

    private static string ReplaceFileNameWithGUID(string sourceFile)
    {
        // Full file name. Change your file name   
        string fileName = sourceFile;

        FileInfo fi = new FileInfo(fileName);

        string extn = fi.Extension;

        return GenerateGUID() + extn;

    }

    private static bool CopyImageToProjectImagesFolder(ref string sourceFile)
    {
        // Retrieve the file extension of the source image
        string fileExtension = Path.GetExtension(sourceFile);

        // Generate a new file name based on a GUID and append the original extension.
        string newFileName = Guid.NewGuid().ToString("N") + fileExtension;

        // Combine the existing folder path and the new file name to form the destination path.
        string destinationFile = Path.Combine(folderPath.ToString(), newFileName);

        try
        {
            // Copy the file to the destination folder with overwriting enabled in case there is an existing file.
            File.Copy(sourceFile, destinationFile, true);
        }
        catch //(IOException ex)
        {
            // Optionally log the error message: ex.Message
            return false;
        }

        // Update the sourceFile reference to the new destination file path.
        sourceFile = destinationFile;
        GUIDAddress = new StringBuilder(newFileName);
        return true;
    }

    public static bool SetPersonImage(ref PictureBox pbPersonImage, ref clsPerson person)
    {

        StringBuilder searchPattern = new StringBuilder(person.ImagePath + ".*");
        string[] files;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath.ToString());
        }

        try
        {
            files = Directory.GetFiles(folderPath, searchPattern.ToString());
        }
        catch //(Exception ex)
        {
            // Handle error properly
            //MessageBox.Show($"An error occurred while accessing the folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            files = Array.Empty<string>(); // Return an empty array to prevent failure
        }

        if (files.Length > 0)
        {
            // Dispose previous image if any
            if (pbPersonImage.Image != null)
            {
                pbPersonImage.Image.Dispose();
                pbPersonImage.Image = null;
            }

            // Directly set the image location without modifying the original path
            pbPersonImage.ImageLocation = files[0];

            return true;
        }
        else
        {
            if (MessageBox.Show("The image file associated with this record could not be found. Would you like to remove the image reference from the person's information?",
                                "Image Not Found", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                person.ImagePath = "";
             
                
                if (person.Save())
                {
                    MessageBox.Show("Data Updated Successfully");
                }

            }

            return false;

        }

    }

    public static bool HandlePersonImage(ref PictureBox pbPersonImage, string OldImagePath)
    {

        StringBuilder searchPattern = new StringBuilder(OldImagePath + ".*");
        string[] Testfiles;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath.ToString());
        }


        try
        {
            Testfiles = Directory.GetFiles(folderPath, searchPattern.ToString());
        }
        catch
        {
            return false;
        }

        if (Testfiles.Length == 0)
        {
            Testfiles = new string[] { "" };  // Assign an empty array with one element
        }


        if (pbPersonImage.ImageLocation != Testfiles[0])
        {
            // Code to execute if the image location does NOT match

            if (!string.IsNullOrEmpty(OldImagePath))
            {

                string[] files = Directory.GetFiles(folderPath, OldImagePath + ".*");

                if (files.Length > 0)
                {
                    try
                    {
                        File.Delete(files[0]);
                    }
                    catch //(IOException ex)
                    {
                        // handle
                    }
                }

            }

            if (pbPersonImage.ImageLocation != null)
            {
                string SourceImageFile = pbPersonImage.ImageLocation.ToString();

                if (clsUtil.CopyImageToProjectImagesFolder(ref SourceImageFile))
                {
                    pbPersonImage.ImageLocation = SourceImageFile;
                }
                else
                {
                    return false;
                }

            }

            return true;

        }


        return false;
    }

    public static bool SetPersonImage(ref PictureBox pb, clsPerson person)
    {
        return SetPersonImage(ref pb, ref person);
    }


}

