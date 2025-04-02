using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CG_Lab1
{
    public partial class Form2 : Form
    {
        public Form2(Bitmap Image)
        {
            InitializeComponent();
            pictureBox1.Image = Image;
            int stride;
            byte[] ImageArray = Program.ImageToByteArray(Image, out stride);
            double[] Hue = new double[ImageArray.Length / 3];
            byte[] Saturation = new byte[ImageArray.Length / 3];
            byte[] Value = new byte[ImageArray.Length / 3];
            double red, green, blue, delta;
            int temp;
            for(int i = 0; i < Hue.Length; i++)
            {
                red = ImageArray[i];
                green = ImageArray[i + 1];
                blue = ImageArray[i + 2];
                // value
                Value[i] = (byte)(Math.Max(Math.Max(ImageArray[i*3], ImageArray[i * 3+1]), ImageArray[i * 3 + 2]) * 100 / 256);
                temp = Math.Min(Math.Min(ImageArray[i * 3], ImageArray[i * 3 + 1]), ImageArray[i * 3 + 2]) * 100 / 256;
                delta = Value[i] - temp;
                // hue
                if (temp == Value[i])
                {
                    Hue[i] = 0;
                }
                else if (Value[i] == ImageArray[i * 3]) // R max
                {
                    Hue[i] = ((green - blue) * 60 / delta);
                }
                else if (Value[i] == ImageArray[i * 3 + 1]) // G max
                {
                    Hue[i] = 120 + ((blue - red) * 60 / delta);
                }
                else if (Value[i] == ImageArray[i * 3 + 2]) // B max
                {
                    Hue[i] = 240 + ((red - green) * 60 / delta);
                }
                else
                {
                    throw new Exception();
                }
                if (Hue[i] < 0)
                    Hue[i] += 360;
                // saturation
                if (Value[i] == 0)
                    Saturation[i] = 0;
                else
                    Saturation[i] = (byte)((Value[i] - temp) * 100 / Value[i]);
            }
            // now printing the grayscales
            byte[] HueArray = new byte[Hue.Length * 3];
            byte[] SaturationArray = new byte[Hue.Length * 3];
            byte[] ValueArray = new byte[Hue.Length * 3];
            for(int i=0;i<ValueArray.Length;i++)
            {
                HueArray[i] = (byte)((Hue[i / 3]) * 256 / 360);
                SaturationArray[i] = (byte)(Saturation[i / 3]*256 / 100);
                ValueArray[i] = (byte)(Value[i / 3] * 256 / 100);
        }
            Bitmap ImageH = Program.ByteArrayToImage(HueArray, Image.Width, Image.Height, stride);
            Bitmap ImageS = Program.ByteArrayToImage(SaturationArray, Image.Width, Image.Height, stride);
            Bitmap ImageV = Program.ByteArrayToImage(ValueArray, Image.Width, Image.Height, stride);
            pictureBox2.Image = ImageH;
            pictureBox3.Image = ImageS;
            pictureBox4.Image = ImageV;

        }
    }
}
