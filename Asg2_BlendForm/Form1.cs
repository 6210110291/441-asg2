using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Asg2_BlendForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string newLine = Environment.NewLine;

        Stopwatch stopWatch1 = new Stopwatch();
        Bitmap image1, image1_clone, image2, image2_clone;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float weigth = trackBar1.Value;
            weigth = weigth / 100;
            //textBox1.AppendText(newLine + "W = " + weigth);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //------------------------------lotus.jpg------------------------------
                stopWatch1.Reset();
                stopWatch1.Start();
                
                image1 = new Bitmap(@"lotus.jpg", true);

                stopWatch1.Stop();
                textBox1.AppendText("Time for reading the image from file = " + stopWatch1.ElapsedMilliseconds.ToString() + " mS\r\n");

                RectangleF cloneRect = new RectangleF(0, 0, image1.Width, image1.Height);
                System.Drawing.Imaging.PixelFormat format = image1.PixelFormat;
                image1_clone = image1.Clone(cloneRect, format);

                textBox1.AppendText("Successfully load the lotus image\r\n");
                // Set the PictureBox to display the image.
                pictureBox1.Image = image1;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                //------------------------------steve-jobs.jpg------------------------------
                stopWatch1.Reset();
                stopWatch1.Start();

                image2 = new Bitmap(@"steve-jobs.jpg", true);

                stopWatch1.Stop();
                textBox1.AppendText("Time for reading the image from file = " + stopWatch1.ElapsedMilliseconds.ToString() + " mS\r\n");

                RectangleF cloneRect2 = new RectangleF(0, 0, image2.Width, image2.Height);
                System.Drawing.Imaging.PixelFormat format2 = image2.PixelFormat;
                image2_clone = image2.Clone(cloneRect2, format2);

                textBox1.AppendText("Successfully load the Steve-Job image\r\n");
                // Set the PictureBox to display the image.
                pictureBox2.Image = image2;
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

            }
            catch (ArgumentException)
            {
                MessageBox.Show("There was an error." +
                    "Check the path to the image file.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stopWatch1.Reset();
            stopWatch1.Start();
            object lockObject = new object();

            float weigth = trackBar1.Value;
            weigth = weigth / 100;
            //textBox1.AppendText(newLine + "W = " + weigth);

            // Create a new bitmap to hold the blended image
            Bitmap blendedImage = new Bitmap(image1.Width, image1.Height);

            // Lock the bits of the images
            BitmapData data1 = image1.LockBits(new Rectangle(0, 0, image1.Width, image1.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData data2 = image2.LockBits(new Rectangle(0, 0, image2.Width, image2.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData blendedData = blendedImage.LockBits(new Rectangle(0, 0, blendedImage.Width, blendedImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            // Get the address of the first pixel of the images
            IntPtr ptr1 = data1.Scan0;
            IntPtr ptr2 = data2.Scan0;
            IntPtr blendedPtr = blendedData.Scan0;

            // Declare an array to hold the bytes of the image
            int bytes = data1.Stride * data1.Height;
            byte[] rgbValues1 = new byte[bytes];
            byte[] rgbValues2 = new byte[bytes];
            byte[] blendedValues = new byte[bytes];

            // Copy the RGB values of the images into the array
            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgbValues1, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbValues2, 0, bytes);

            // Use Parallel.For() to blend the images
            Parallel.For(0, bytes / 3, i =>
            {
                //lock (lockObject)
                //{
                    int index = i * 3;
                    blendedValues[index] = (byte)((rgbValues1[index] * (1 - weigth)) + (rgbValues2[index] * weigth));
                    blendedValues[index + 1] = (byte)((rgbValues1[index + 1] * (1 - weigth)) + (rgbValues2[index + 1] * weigth));
                    blendedValues[index + 2] = (byte)((rgbValues1[index + 2] * (1 - weigth)) + (rgbValues2[index + 2] * weigth));
                //}
            });

            // Copy the blended RGB values back into the blended image
            System.Runtime.InteropServices.Marshal.Copy(blendedValues, 0, blendedPtr, bytes);

            // Unlock the bits of the images
            image1.UnlockBits(data1);
            image2.UnlockBits(data2);
            blendedImage.UnlockBits(blendedData);

            stopWatch1.Stop();
            pictureBox3.Image = blendedImage;
            //pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            textBox1.AppendText(newLine + "Time for Blending these two images = " + stopWatch1.ElapsedMilliseconds.ToString() + " mS\r\n");
        }
    }
}
