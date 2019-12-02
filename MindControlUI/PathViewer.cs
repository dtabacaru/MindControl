using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MindControlUI
{
    public partial class PathViewer : Form
    {
        class MapString
        {
            public string Text;
            public float X;
            public float Y;

            public MapString(string text, float x, float y)
            {
                Text = text;
                X = x;
                Y = y;
            }
        }

        List<float> m_PathXCoords = new List<float>();
        List<float> m_PathYCoords = new List<float>();
        List<float> m_PathStartXCoords = new List<float>();
        List<float> m_PathStartYCoords = new List<float>();
        List<float> m_PathEndXCoords = new List<float>();
        List<float> m_PathEndYCoords = new List<float>();
        List<MapString> m_PathNames = new List<MapString>();

        public PathViewer()
        {
            InitializeComponent();
        }

        private void LoadMapImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PNG |*.png";
            ofd.Title = "Open map image";
            ofd.ShowDialog();

            if (ofd.FileName != "")
            {
                try
                {
                    MapPictureBox.Image = new Bitmap(ResizeImage(new Bitmap(ofd.FileName), 1000, 700));
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text |*.txt";
            ofd.Title = "Open path";
            ofd.ShowDialog();

            if (ofd.FileName != "")
            {
                try
                {
                    string allpaths = File.ReadAllText(ofd.FileName);
                    string[] paths = allpaths.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    string[] xString = paths[0].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] yString = paths[1].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    List<float> xCoords = new List<float>();
                    List<float> yCoords = new List<float>();

                    foreach (string xcoordinate in xString)
                    {
                        xCoords.Add(Convert.ToSingle(xcoordinate));
                    }

                    foreach (string ycoordinate in yString)
                    {
                        yCoords.Add(Convert.ToSingle(ycoordinate));
                    }

                    m_PathXCoords.AddRange(xCoords);
                    m_PathYCoords.AddRange(yCoords);

                    m_PathStartXCoords.Add(xCoords[0]);
                    m_PathStartYCoords.Add(yCoords[0]);

                    m_PathEndXCoords.Add(xCoords[xCoords.Count - 1]);
                    m_PathEndYCoords.Add(yCoords[yCoords.Count - 1]);

                    FileInfo fi = new FileInfo(ofd.FileName);
                    m_PathNames.Add(new MapString(fi.Name, xCoords[0], yCoords[0]));

                    MapPictureBox.Invalidate();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public Image ResizeImage(Image image, int width, int height)
        {
            Rectangle destRect = new Rectangle(0, 0, width, height);
            Image destImage = new Bitmap(width, height);

            //destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        Pen m_StartPen = new Pen(Brushes.Cyan, 4);
        Pen m_EndPen = new Pen(Brushes.Magenta, 4);
        Pen m_PathPen = new Pen(Brushes.White, 2);

        private void MapPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (m_PathXCoords.Count == 0)
                return;

            for(int i = 0; i < m_PathXCoords.Count; i++)
            {
                e.Graphics.DrawEllipse(m_PathPen, m_PathXCoords[i] * 10, m_PathYCoords[i] * 7, 1, 1);
            }

            for (int i = 0; i < m_PathStartXCoords.Count; i++)
            {
                e.Graphics.DrawEllipse(m_StartPen, m_PathStartXCoords[i] * 10, m_PathStartYCoords[i] * 7, 1, 1);
            }

            for (int i = 0; i < m_PathEndXCoords.Count; i++)
            {
                e.Graphics.DrawEllipse(m_EndPen, m_PathEndXCoords[i] * 10, m_PathEndYCoords[i] * 7, 1, 1);
            }

            for (int i = 0; i < m_PathNames.Count; i++)
            {
                e.Graphics.DrawString(m_PathNames[i].Text,SystemFonts.DefaultFont, Brushes.Cyan, m_PathNames[i].X*10, m_PathNames[i].Y*7);
            }

            
        }

        private void clearPathsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_PathXCoords.Clear();
            m_PathYCoords.Clear();
            m_PathEndXCoords.Clear();
            m_PathEndYCoords.Clear();
            m_PathStartXCoords.Clear();
            m_PathStartYCoords.Clear();
            m_PathNames.Clear();

            MapPictureBox.Invalidate();
        }
    }
}
