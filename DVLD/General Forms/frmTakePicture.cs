using System;
using AForge.Video;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using System.IO;




namespace Driving_License_Management.General_Forms
{
    public partial class frmTakePicture : Form
    {
        public Action<string> imageBack;


        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoSource;

        public frmTakePicture()
        {
            InitializeComponent();
        }
       
        private void frmTakePicture_Load(object sender, EventArgs e)
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("No video sources found");
                    return;
                }

                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
                videoSource.Start();
            }
            catch (Exception ex)
            {
                LogUIExceptions.WriteExceptionToEventViewer(ex);
            }

        }

        private void frmTakePicture_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanUpVideoSource();
            // The form will then close naturally as part of the window manager's process.
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CleanUpVideoSource()
        {

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                //videoSource.WaitForStop();

                videoSource = null;
            }

        }

        private void UpdatePictureBox(Bitmap newFrame)
        {
            // Dispose of the previous image to avoid memory leaks
            if (pbPersonImage.Image != null)
            {
                pbPersonImage.Image.Dispose();
            }

            pbPersonImage.Image = newFrame;
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap newFrame = (Bitmap)eventArgs.Frame.Clone();

            //if (pbPersonImage.InvokeRequired)
            //{
            //    pbPersonImage.Invoke(new Action(() =>
            //    {
            //        UpdatePictureBox(newFrame);
            //    }));
            //}
            //else
            //{
                UpdatePictureBox(newFrame);
            //}

        }

        private void btnTakePicture_Click(object sender, EventArgs e)
        {

            if (pbPersonImage.Image != null)
            {
                try
                {

                    string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    string saveFolder = Path.Combine(picturesPath, "C:\\Users\\Sc\\OneDrive\\Pictures\\Saved Pictures");

                    // Create the folder if it doesn't already exist.
                    if (!Directory.Exists(saveFolder))
                    {
                        Directory.CreateDirectory(saveFolder);
                    }

                    string fileName = "captured_image_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg";
                    string filePath = Path.Combine(saveFolder, fileName);

                    pbPersonImage.Image.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);


                    imageBack?.Invoke(filePath);
                    this.Close();
                }
                catch (Exception ex)
                {
                    LogUIExceptions.WriteExceptionToEventViewer(ex);
                }

            }

        }

    }

}

