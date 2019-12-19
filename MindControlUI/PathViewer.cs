using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WowAutomater;
using WowApi;
using System;
using System.IO;

namespace MindControlUI
{
    public partial class PathViewer : Form
    {
        List<Waypoint> SetPathWaypoints = new List<Waypoint>();
        string SetPathWaypointName = string.Empty;

        List<Waypoint> CustomPathWaypoints = new List<Waypoint>();
        List<Waypoint> CustomPathStartWaypoints = new List<Waypoint>();
        List<Waypoint> CustomPathEndWaypoints = new List<Waypoint>();
        List<string> CustomPathNames = new List<string>();

        private System.Windows.Forms.Timer m_ApiDataUpdateTimer = new System.Windows.Forms.Timer();

        public PathViewer(List<Waypoint> waypoints, string name)
        {
            InitializeComponent();

            SetPathWaypointName = name;
            SetPathWaypoints = waypoints;
            MapPictureBox.Image = new Bitmap(Helper.ResizeImage(new Bitmap("MapImages\\" + waypoints[0].MapId.ToString() + ".jpg"), 1000, 700));

            m_ApiDataUpdateTimer.Interval = 50;
            m_ApiDataUpdateTimer.Tick += M_ApiDataUpdateTimer_Tick;
            m_ApiDataUpdateTimer.Enabled = true;
        }

        private void M_ApiDataUpdateTimer_Tick(object sender, System.EventArgs e)
        {
            MapPictureBox.Invalidate();
        }

        Pen m_PathPen = new Pen(Brushes.Black, 2);
        Pen m_StartPen = new Pen(Brushes.LightGreen, 4);
        Pen m_EndPen = new Pen(Brushes.Crimson, 4);
        Pen m_PlayerPen = new Pen(Brushes.Cyan, 4);

        private void MapPictureBox_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < CustomPathWaypoints.Count; i++)
            {
                e.Graphics.DrawEllipse(m_PathPen, (float)(CustomPathWaypoints[i].X * 10), (float)(CustomPathWaypoints[i].Y * 7), 1f, 1f);
            }

            for (int i = 0; i < CustomPathStartWaypoints.Count; i++)
            {
                e.Graphics.DrawEllipse(m_StartPen, (float)(CustomPathStartWaypoints[i].X * 10), (float)(CustomPathStartWaypoints[i].Y * 7), 1f, 1f);
            }

            for (int i = 0; i < CustomPathEndWaypoints.Count; i++)
            {
                e.Graphics.DrawEllipse(m_EndPen, (float)(CustomPathEndWaypoints[i].X * 10), (float)(CustomPathEndWaypoints[i].Y * 7), 1f, 1f);
            }

            for (int i = 0; i < CustomPathStartWaypoints.Count; i++)
            {
                e.Graphics.DrawString(CustomPathNames[i], DefaultFont, Brushes.White, (float)(CustomPathStartWaypoints[i].X * 10), (float)(CustomPathStartWaypoints[i].Y * 7));
            }

            if (SetPathWaypoints.Count < 2)
                return;

            for (int i = 0; i < SetPathWaypoints.Count; i++)
            {
                e.Graphics.DrawEllipse(m_PathPen, (float)(SetPathWaypoints[i].X * 10), (float)(SetPathWaypoints[i].Y * 7), 1f, 1f);
            }

            e.Graphics.DrawEllipse(m_StartPen, (float)(SetPathWaypoints[0].X * 10), (float)(SetPathWaypoints[0].Y * 7), 1f, 1f);
            e.Graphics.DrawString(SetPathWaypointName, DefaultFont, Brushes.White, (float)(SetPathWaypoints[0].X * 10), (float)(SetPathWaypoints[0].Y * 7));
            e.Graphics.DrawEllipse(m_EndPen, (float)(SetPathWaypoints[SetPathWaypoints.Count-1].X * 10), (float)(SetPathWaypoints[SetPathWaypoints.Count - 1].Y * 7), 1f, 1f);

            if (SetPathWaypoints[0].MapId == Api.PlayerData.MapId)
                e.Graphics.DrawEllipse(m_PlayerPen, (float)(Api.PlayerData.PlayerXPosition * 10), (float)(Api.PlayerData.PlayerYPosition * 7), 1f, 1f);
        }

        private void LoadPathToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text |*.txt";
            ofd.Title = "Open path";
            ofd.ShowDialog();

            if (ofd.FileName != "")
            {
                try
                {
                    string path = File.ReadAllText(ofd.FileName);

                    string[] waypointsComponents = path.Split(';');

                    List<uint> mapIds = Program.ExtractCommaDelimitedUInts(waypointsComponents[0].Trim());

                    if(mapIds[0] != SetPathWaypoints[0].MapId)
                    {
                        MessageBox.Show("Path not on current map.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    List<double> xs = Program.ExtractCommaDelimitedDoubles(waypointsComponents[1].Trim());
                    List<double> ys = Program.ExtractCommaDelimitedDoubles(waypointsComponents[2].Trim());
                    List<Waypoint> waypointList = new List<Waypoint>();

                    for (int i = 0; i < mapIds.Count; i++)
                    {
                        waypointList.Add(new Waypoint(mapIds[i], xs[i], ys[i]));
                    }

                    CustomPathWaypoints.AddRange(waypointList);
                    CustomPathStartWaypoints.Add(waypointList[0]);
                    CustomPathEndWaypoints.Add(waypointList[waypointList.Count - 1]);

                    FileInfo fi = new FileInfo(ofd.FileName);
                    CustomPathNames.Add(fi.Name);

                    MapPictureBox.Invalidate();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearPathsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            CustomPathWaypoints.Clear();
            CustomPathStartWaypoints.Clear();
            CustomPathEndWaypoints.Clear();
            CustomPathNames.Clear();

            MapPictureBox.Invalidate();
        }
    }
}
