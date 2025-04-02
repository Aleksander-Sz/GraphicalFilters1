using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CG_Lab1
{
    internal static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        static public byte[] ImageToByteArray(Bitmap Image, out int stride)
        {
            Rectangle rect = new Rectangle(0, 0, Image.Width, Image.Height);
            BitmapData bmpData = Image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            stride = bmpData.Stride;
            int bytes = Math.Abs(bmpData.Stride) * Image.Height;
            byte[] rgbValues = new byte[bytes];

            Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);
            Image.UnlockBits(bmpData);

            return rgbValues;
        }
        static public Bitmap ByteArrayToImage(byte[] rgbValues, int width, int height, int stride)
        {
            Bitmap Image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = Image.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            Marshal.Copy(rgbValues, 0, bmpData.Scan0, rgbValues.Length);
            Image.UnlockBits(bmpData);

            return Image;
        }
    }

}
