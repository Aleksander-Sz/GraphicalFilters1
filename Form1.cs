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
    public partial class Form1 : Form
    {
        Bitmap OriginalImage;
        Bitmap Image;
        public Form1()
        {
            InitializeComponent();
            OriginalImage = null;
            Image = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ;
        }

        private void button1_Click(object sender, EventArgs e) // load
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
            openFileDialog.Title = "Select an Image File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    OriginalImage = new Bitmap(openFileDialog.FileName);
                    Image = new Bitmap(OriginalImage);
                    pictureBox1.Image = Image;
                    pictureBox2.Image = OriginalImage;
                }
                catch(Exception exception)
                {
                    MessageBox.Show("Error loading the image: " + exception.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) // save
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            saveFileDialog.Title = "Save Image As";
            if(saveFileDialog.ShowDialog()==DialogResult.OK)
            {
                try
                {
                    Bitmap imageToBeSaved = (Bitmap)pictureBox1.Image;
                    imageToBeSaved.Save(saveFileDialog.FileName);
                }
                catch(Exception exception)
                {
                    MessageBox.Show("Error saving the image: " + exception.Message);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e) // reset
        {
            Image = OriginalImage;
            pictureBox1.Image = Image;
        }

        private void button5_Click(object sender, EventArgs e) // invert
        {
            if (Image == null)
            {
                return;
            }
            for (int i = 0; i < Image.Width; i++)
            {
                for(int j = 0; j < Image.Height; j++)
                {
                    Color OnePixel = Image.GetPixel(i, j);
                    Color InvertedPixel = Color.FromArgb(255 - OnePixel.R, 255 - OnePixel.G, 255 - OnePixel.B);
                    Image.SetPixel(i,j,InvertedPixel);
                }
            }
            pictureBox1.Image = Image;
        }

        private void button6_Click(object sender, EventArgs e) // brightness correction
        {
            if(Image==null)
            {
                return;
            }
            int brightness = 10;
            for (int i = 0; i < Image.Width; i++)
            {
                for (int j = 0; j < Image.Height; j++)
                {
                    Color OnePixel = Image.GetPixel(i, j);
                    Color InvertedPixel = Color.FromArgb(Clamp(brightness + OnePixel.R), Clamp(brightness + OnePixel.G), Clamp(brightness + OnePixel.B));
                    Image.SetPixel(i, j, InvertedPixel);
                }
            }
            pictureBox1.Image = Image;
        }
        protected int Clamp(double number)
        {
            if (number <= 0)
                return 0;
            if (number >= 255)
                return 255;
            return (int)number;
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (Image == null)
            {
                return;
            }
            double alpha = 1.45;
            for (int i = 0; i < Image.Width; i++)
            {
                for (int j = 0; j < Image.Height; j++)
                {
                    Color OnePixel = Image.GetPixel(i, j);
                    Color InvertedPixel = Color.FromArgb(Clamp((OnePixel.R-128)*alpha+128), Clamp((OnePixel.G - 128) * alpha + 128), Clamp((OnePixel.B - 128) * alpha + 128));
                    Image.SetPixel(i, j, InvertedPixel);
                }
            }
            pictureBox1.Image = Image;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (Image == null)
            {
                return;
            }
            double gamma = 0.75;
            for (int i = 0; i < Image.Width; i++)
            {
                for (int j = 0; j < Image.Height; j++)
                {
                    Color OnePixel = Image.GetPixel(i, j);
                    double C1 = (double)OnePixel.R / 255;
                    double C2 = (double)OnePixel.G / 255;
                    double C3 = (double)OnePixel.B / 255;
                    C1 = Math.Pow(C1, gamma) * 255;
                    C2 = Math.Pow(C2, gamma) * 255;
                    C3 = Math.Pow(C3, gamma) * 255;
                    Color InvertedPixel = Color.FromArgb((int)C1, (int)C2, (int)C3);
                    Image.SetPixel(i, j, InvertedPixel);
                }
            }
            pictureBox1.Image = Image;
        }
        private Bitmap ApplyConvolution(Bitmap original, int[,] kernel, int total)
        {
            int size = 3;  // Hardcoded, not suitable for kernels of different sizes
            int k = (size-1)/2;
            Color color;
            Bitmap final = new Bitmap(original);
            for (int i = 0; i<original.Width; i++)
            {
                int sumR = 0, sumG = 0, sumB = 0;
                for(int j = 0; j<original.Height; j++)
                {
                    //Color colortest = Color.FromArgb(100, 0, 0);
                    //final.SetPixel(0, 0, colortest);
                    for (int x = -k; x<=k; x++)
                    {
                        for(int y = -k; y<=k; y++)
                        {
                            int xx = x+i, yy = y+j;
                            if (xx < 0)
                            {
                                xx = 0;
                            }
                            if (xx >= original.Width)
                            {
                                xx = original.Width-1;
                            }
                            if (yy < 0)
                            {
                                yy = 0;
                            }
                            if (yy >= original.Height)
                            {
                                yy = original.Height-1;
                            }
                            color = original.GetPixel(xx, yy);
                            sumR += color.R * kernel[x + k, y + k];
                            sumG += color.G * kernel[x + k, y + k];
                            sumB += color.B * kernel[x + k, y + k];
                        }
                    }
                    color = Color.FromArgb(Clamp(sumR / total), Clamp(sumG / total), Clamp(sumB / total));
                    final.SetPixel(i, j, color);
                    sumR = 0;
                    sumG = 0;
                    sumB = 0;
                }
            }
            //Color color1 = Color.FromArgb(100, 0, 0);
            //final.SetPixel(10, 10, color1);
            //pictureBox2.Image = final;
            return final;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (Image == null)
            {
                return;
            }
            int[,] kernel = {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 },
            };
            int sum = 9;
            Image = ApplyConvolution(Image, kernel, sum);
            pictureBox1.Image = Image;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (Image == null)
            {
                return;
            }
            int[,] kernel = {
                { 0, 1, 0 },
                { 1, 4, 1 },
                { 0, 1, 0 },
            };
            int sum = 8;
            Image = ApplyConvolution(Image, kernel, sum);
            pictureBox1.Image = Image;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (Image == null)
            {
                return;
            }
            int[,] kernel = {
                { 0,-1, 0 },
                {-1, 5,-1 },
                { 0,-1, 0 },
            };
            int sum = 1;
            Image = ApplyConvolution(Image, kernel, sum);
            pictureBox1.Image = Image;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (Image == null)
            {
                return;
            }
            int[,] kernel = {
                { 0, 0, 0 },
                {-1, 1, 0 },
                { 0, 0, 0 },
            };
            int sum = 1;
            Image = ApplyConvolution(Image, kernel, sum);
            pictureBox1.Image = Image;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (Image == null)
            {
                return;
            }
            int[,] kernel = {
                {-1, 0, 1 },
                {-1, 1, 1 },
                {-1, 0, 1 },
            };
            int sum = 1;
            Image = ApplyConvolution(Image, kernel, sum);
            pictureBox1.Image = Image;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            int kernelSize = Decimal.ToInt32(numericUpDown1.Value);
            if (Image == null || kernelSize<1)
            {
                return;
            }
            List<int> listR = new List<int>();
            List<int> listG = new List<int>();
            List<int> listB = new List<int>();
            int xCount = (Image.Width - 1) / kernelSize + 1;
            int yCount = (Image.Height - 1) / kernelSize + 1;
            for(int i = 0; i<xCount; i++)
            {
                for(int j = 0; j<yCount; j++)
                {
                    for(int t = kernelSize * i; t<kernelSize * (i+1) && t<Image.Width; t++)
                    {
                        for(int u = kernelSize * j; u<kernelSize * (j+1) && u<Image.Height; u++)
                        {
                            Color color = Image.GetPixel(t, u);
                            listR.Add(color.R);
                            listG.Add(color.G);
                            listB.Add(color.B);
                            //Image.SetPixel(t,u,Color.FromArgb(Clamp(i * 10), Clamp(255 - 10 * j), 0));
                        }
                    }
                    listR.Sort();
                    listG.Sort();
                    listB.Sort();
                    int Rmedian, Gmedian, Bmedian;
                    if(listR.Count%2==0)
                    {
                        Rmedian = (listR[listR.Count / 2] + listR[listR.Count / 2 + 1]) / 2;
                        Gmedian = (listG[listR.Count / 2] + listR[listR.Count / 2 + 1]) / 2;
                        Bmedian = (listB[listR.Count / 2] + listR[listR.Count / 2 + 1]) / 2;
                    }
                    else
                    {
                        Rmedian = listR[listR.Count / 2];
                        Gmedian = listG[listR.Count / 2];
                        Bmedian = listB[listR.Count / 2];
                    }
                    Color finalPixel = Color.FromArgb(Rmedian, Gmedian, Bmedian);
                    for (int t = kernelSize * i; t < kernelSize * (i + 1) && t < Image.Width; t++)
                    {
                        for (int u = kernelSize * j; u < kernelSize * (j + 1) && u < Image.Height; u++)
                        {
                            Image.SetPixel(t,u,finalPixel);
                        }
                    }
                    listR.Clear();
                    listG.Clear();
                    listB.Clear();
                }
            }
            pictureBox1.Image = Image;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            int kernelSize = Decimal.ToInt32(numericUpDown1.Value);
            if (Image == null || kernelSize < 1)
            {
                return;
            }
            List<int> listR = new List<int>();
            List<int> listG = new List<int>();
            List<int> listB = new List<int>();
            int k = (kernelSize - 1) / 2;
            Color color;
            for (int i = 0; i < Image.Width; i++)
            {
                for (int j = 0; j < Image.Height; j++)
                {
                    //Color colortest = Color.FromArgb(100, 0, 0);
                    //final.SetPixel(0, 0, colortest);
                    for (int x = -k; x <= k; x++)
                    {
                        for (int y = -k; y <= k; y++)
                        {
                            int xx = x + i, yy = y + j;
                            if (xx < 0)
                            {
                                xx = 0;
                            }
                            if (xx >= Image.Width)
                            {
                                xx = Image.Width - 1;
                            }
                            if (yy < 0)
                            {
                                yy = 0;
                            }
                            if (yy >= Image.Height)
                            {
                                yy = Image.Height - 1;
                            }
                            color = Image.GetPixel(xx, yy);
                            listR.Add(color.R);
                            listG.Add(color.G);
                            listB.Add(color.B);
                        }
                    }
                    listR.Sort();
                    listG.Sort();
                    listB.Sort();
                    int Rmedian, Gmedian, Bmedian;
                    if (listR.Count % 2 == 0)
                    {
                        Rmedian = (listR[listR.Count / 2] + listR[listR.Count / 2 + 1]) / 2;
                        Gmedian = (listG[listR.Count / 2] + listR[listR.Count / 2 + 1]) / 2;
                        Bmedian = (listB[listR.Count / 2] + listR[listR.Count / 2 + 1]) / 2;
                    }
                    else
                    {
                        Rmedian = listR[listR.Count / 2];
                        Gmedian = listG[listR.Count / 2];
                        Bmedian = listB[listR.Count / 2];
                    }
                    color = Color.FromArgb(Rmedian,Gmedian, Bmedian);
                    Image.SetPixel(i, j, color);
                    listR.Clear();
                    listG.Clear();
                    listB.Clear();
                }
            }
            pictureBox1.Image = Image;
        }
    }
}
