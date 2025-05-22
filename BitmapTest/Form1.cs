using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace BitmapTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap bmp;
        Graphics flagGraphics;
        DateTime time;
        BackgroundWorker bgw;

        int width = 0;
        int height = 0;

        private static readonly object locker = new object();

        private void Form1_Shown(object sender, EventArgs e)
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            flagGraphics = Graphics.FromImage(bmp);

            width = bmp.Width;
            height = bmp.Height;

            time = DateTime.Now;

            bgw = new BackgroundWorker();
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgw.WorkerReportsProgress = true;
            bgw.RunWorkerAsync();
        }

        void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            setFPS();
        }

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            int c = 0;
            while(true)
            {
                updateScreen();
                bgw.ReportProgress(c);
                c++;
            }
        }

        private Color[] colors = new Color[] {Color.Wheat, Color.WhiteSmoke, Color.Black, Color.Brown,
                                            Color.Yellow, Color.Gray, Color.LightBlue, Color.LightGray};
        int index = 0;

        private void draw() {
            flagGraphics.Clear(SystemColors.Control);
            int squareSize = 5;

            //vertical
            for (int i = 0; i < width / squareSize + 1; i++)
            {
                flagGraphics.DrawLine(new Pen(colors[index], 1), i * squareSize, 0, i * squareSize, height);
            }

            //horizontal
            for (int i = 0; i < height / squareSize + 1; i++)
            {
                flagGraphics.DrawLine(new Pen(colors[index], 1), 0, i * squareSize, width, i * squareSize);
            }

            lock (locker) {
                pictureBox1.Image = bmp;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                
            }
        }

        int counter = 0;

        private void updateScreen() {
            index++;
            index = index > colors.Length - 1 ? 0 : index;
            draw();
            counter++;
        }

        private void setFPS() {
            TimeSpan timeSpan = DateTime.Now - time;
            if (timeSpan.TotalSeconds >= 1) {
                double fps = (1.0 / timeSpan.TotalSeconds) * counter;
                Text = "Bitmap Test - FPS: " + fps;
                time = DateTime.Now;
                counter = 0;
            }
        }
    }
}
