using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CG_Lab1
{
    public partial class Form1 : Form
    {
        Bitmap OriginalImage;
        Bitmap Image;
        int stride;
        bool grayscale;
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
                    grayscale = false;
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
            Image = new Bitmap(OriginalImage);
            pictureBox1.Image = Image;
            grayscale = false;
        }

        private void button5_Click(object sender, EventArgs e) // invert
        {
            if (Image == null)
            {
                return;
            }
            byte[] ImageArray = Program.ImageToByteArray(Image,out stride);
            for(int i = 0; i < Image.Height*stride;i+=3)
            {
                ImageArray[i] = (byte)(255 - ImageArray[i]);
                ImageArray[i+1] = (byte)(255 - ImageArray[i+1]);
                ImageArray[i+2] = (byte)(255 - ImageArray[i+2]);
            }
            Image = Program.ByteArrayToImage(ImageArray,Image.Width,Image.Height, stride);
            pictureBox1.Image = Image;
        }

        private void button6_Click(object sender, EventArgs e) // brightness correction
        {
            if (Image == null)
            {
                return;
            }
            int brightness = 10;
            byte[] ImageArray = Program.ImageToByteArray(Image, out stride);
            for (int i = 0; i < Image.Height * stride; i += 3)
            {
                ImageArray[i] = (byte)(Clamp(ImageArray[i] + brightness));
                ImageArray[i + 1] = (byte)(Clamp(ImageArray[i+1] + brightness));
                ImageArray[i + 2] = (byte)(Clamp(ImageArray[i+2] + brightness));
            }
            Image = Program.ByteArrayToImage(ImageArray, Image.Width, Image.Height, stride);
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
        private void button7_Click(object sender, EventArgs e) // contrast enhancement
        {
            if (Image == null)
            {
                return;
            }
            double alpha = 1.45;
            byte[] ImageArray = Program.ImageToByteArray(Image, out stride);
            for (int i = 0; i < Image.Height * stride; i += 3)
            {
                ImageArray[i] = (byte)(Clamp((ImageArray[i] - 128) * alpha + 128));
                ImageArray[i + 1] = (byte)(Clamp((ImageArray[i+1] - 128) * alpha + 128));
                ImageArray[i + 2] = (byte)(Clamp((ImageArray[i+2] - 128) * alpha + 128));
            }
            Image = Program.ByteArrayToImage(ImageArray, Image.Width, Image.Height, stride);
            pictureBox1.Image = Image;
        }

        private void button8_Click(object sender, EventArgs e) // gamma correction
        {
            if (Image == null)
            {
                return;
            }
            double gamma = 0.75;
            byte[] ImageArray = Program.ImageToByteArray(Image, out stride);
            for (int i = 0; i < Image.Height * stride; i += 3)
            {
                ImageArray[i] = (byte)(Math.Pow((double)ImageArray[i] / 255, gamma) * 255);
                ImageArray[i + 1] = (byte)(Math.Pow((double)ImageArray[i+1] / 255, gamma) * 255);
                ImageArray[i + 2] = (byte)(Math.Pow((double)ImageArray[i+2] / 255, gamma) * 255);
            }
            Image = Program.ByteArrayToImage(ImageArray, Image.Width, Image.Height, stride);
            pictureBox1.Image = Image;
        }
        /*private Bitmap ApplyConvolution(Bitmap original, int[,] kernel, int total)
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
        }*/
        private Bitmap ApplyConvolution(Bitmap original, int[,] kernel, int total)
        {
            if (original == null)
            {
                return original;
            }
            byte[] ImageArray = Program.ImageToByteArray(original, out stride);
            byte[] NewImageArray = new byte[ImageArray.Length];
            int size = kernel.GetLength(0);
            int k = (size - 1) / 2;
            int pixelIndex;
            for (int i = 0; i < original.Height; i++)
            {
                int sumR = 0, sumG = 0, sumB = 0;
                for (int j = 0; j < original.Width; j++)
                {
                    // now for the kernels
                    for (int x = -k; x <= k; x++)
                    {
                        for (int y = -k; y <= k; y++)
                        {
                            int xx = x + j, yy = y + i;
                            if (xx < 0)
                            {
                                xx = 0;
                            }
                            if (xx >= original.Width)
                            {
                                xx = original.Width - 1;
                            }
                            if (yy < 0)
                            {
                                yy = 0;
                            }
                            if (yy >= original.Height)
                            {
                                yy = original.Height - 1;
                            }
                            pixelIndex = (stride * yy) + (xx * 3);
                            //color = original.GetPixel(xx, yy)
                            sumR += ImageArray[pixelIndex] * kernel[x + k, y + k];
                            sumG += ImageArray[pixelIndex + 1] * kernel[x + k, y + k];
                            sumB += ImageArray[pixelIndex + 2] * kernel[x + k, y + k];
                        }
                    }
                    pixelIndex = (stride * i) + (j * 3);
                    //color = Color.FromArgb(Clamp(sumR / total), Clamp(sumG / total), Clamp(sumB / total));
                    //final.SetPixel(i, j, color);
                    NewImageArray[pixelIndex] = (byte)Clamp(sumR/total);
                    NewImageArray[pixelIndex + 1] = (byte)Clamp(sumG/total);
                    NewImageArray[pixelIndex + 2] = (byte)Clamp(sumB/total);
                    sumR = 0;
                    sumG = 0;
                    sumB = 0;
                }
            }
            //Color color1 = Color.FromArgb(100, 0, 0);
            //final.SetPixel(10, 10, color1);
            //pictureBox2.Image = final;
            Bitmap final = Program.ByteArrayToImage(NewImageArray, Image.Width, Image.Height, stride);
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

        private void ButtonGreyscale_Click(object sender, EventArgs e)
        {
            if (Image == null)
            {
                return;
            }
            byte[] ImageArray = Program.ImageToByteArray(Image, out stride);
            for (int i = 0; i < Image.Height * stride; i += 3)
            {
                byte average = (byte)((ImageArray[i] + ImageArray[i+1] + ImageArray[i+2])/ 3);
                ImageArray[i] = ImageArray[i+1] = ImageArray[i+2]  = average;
            }
            Image = Program.ByteArrayToImage(ImageArray, Image.Width, Image.Height, stride);
            pictureBox1.Image = Image;
            grayscale = true;
        }

        private void DitherButton_Click(object sender, EventArgs e)
        {
            if (Image == null)
            {
                return;
            }
            int levelCount = Decimal.ToInt32(DitherK.Value);
            if(levelCount<1)
            {
                ;
            }
            Random gen = new Random();
            int diff, chosen = 0, minDist;
            byte[] ImageArray = Program.ImageToByteArray(Image, out stride);
            for (int i = 0; i < Image.Height * stride; i += 1)
            {
                if((i%3!=0)&&grayscale)
                {
                    continue;
                }
                int[] levels = new int[levelCount];
                for(int j = 0; j < levelCount; j++)
                {
                    levels[j] = gen.Next()%256;
                }
                Array.Sort(levels);
                minDist = 1000;
                for(int j=0; j < levels.Length; j++)
                {
                    diff = Math.Abs(levels[j] - ImageArray[i]);
                    if (diff < minDist)
                    {
                        minDist = diff;
                        chosen = levels[j];
                    }
                }
                ImageArray[i] = (byte)chosen;
                if((i%3==0)&&grayscale)
                {
                    ImageArray[i + 1] = (byte)chosen;
                    ImageArray[i + 2] = (byte)chosen;
                }
            }
            Image = Program.ByteArrayToImage(ImageArray, Image.Width, Image.Height, stride);
            pictureBox1.Image = Image;
        }

        private void KMeansButton_Click(object sender, EventArgs e)
        {
            if (Image == null)
            {
                return;
            }
            int levelCount = Decimal.ToInt32(DitherK.Value);
            if (levelCount < 1)
            {
                ;
            }
            Random gen = new Random();
            double diff, minDist;
            int chosen = 0;
            byte[] ImageArray = Program.ImageToByteArray(Image, out stride);
            int[] redCenters = new int[levelCount];
            int[] greenCenters = new int[levelCount];
            int[] blueCenters = new int[levelCount];
            for(int i=0;i<levelCount;i++)
            {
                redCenters[i] = gen.Next() % 256;
                greenCenters[i] = gen.Next() % 256;
                blueCenters[i] = gen.Next() % 256;
            }
            int[] pixelGroup = new int[ImageArray.Length/3];
            for(int i=0; i<ImageArray.Length/3; i++)
            {
                minDist = 1000;
                for(int j=0;j<levelCount;j++)
                {
                    diff = Math.Sqrt(Math.Pow(redCenters[j] - ImageArray[i * 3], 2) + Math.Pow(greenCenters[j] - ImageArray[i * 3 + 1], 2) + Math.Pow(blueCenters[j] - ImageArray[i * 3 + 2], 2));
                    if(diff<minDist)
                    {
                        minDist = diff;
                        chosen = j;
                    }
                }
                pixelGroup[i] = chosen;
                /*ImageArray[i * 3] = (byte)redCenters[chosen];
                ImageArray[i * 3 + 1] = (byte)greenCenters[chosen];
                ImageArray[i * 3 + 2] = (byte)blueCenters[chosen];*/
            }
            int[] groupCounts = new int[levelCount];
            int[] groupSumsRed = new int[levelCount];
            int[] groupSumsGreen = new int[levelCount];
            int[] groupSumsBlue = new int[levelCount];
            for (int j = 0; j < pixelGroup.Length; j++)
            {
                groupCounts[pixelGroup[j]]++;
                groupSumsRed[pixelGroup[j]] += ImageArray[j * 3];
                groupSumsGreen[pixelGroup[j]] += ImageArray[j * 3 + 1];
                groupSumsBlue[pixelGroup[j]] += ImageArray[j * 3 + 2];
            }
            for(int i=0;i<levelCount;i++)
            {
                if (groupCounts[i] ==0)
                {
                    continue;
                }
                redCenters[i] = groupSumsRed[i] / groupCounts[i];
                greenCenters[i] = groupSumsGreen[i] / groupCounts[i];
                blueCenters[i] = groupSumsBlue[i] / groupCounts[i];
            }
            for (int i = 0; i < ImageArray.Length / 3; i++)
            {
                ImageArray[i * 3] = (byte)redCenters[pixelGroup[i]];
                ImageArray[i * 3 + 1] = (byte)greenCenters[pixelGroup[i]];
                ImageArray[i * 3 + 2] = (byte)blueCenters[pixelGroup[i]];
            }
            Image = Program.ByteArrayToImage(ImageArray, Image.Width, Image.Height, stride);
            pictureBox1.Image = Image;
        }

        private void HSVButton_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(new Bitmap(Image));
            form2.Show();
        }
    }
}
