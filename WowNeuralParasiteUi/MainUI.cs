using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassicWowNeuralParasite;

namespace WoWNeuralParasiteUI
{
    public partial class MainUI : Form
    {
        private WowAutomater m_Automater = new WowAutomater();
        private PlayerData m_PlayerData = new PlayerData();
        private string m_AutomaterStatusString = string.Empty;
        private Timer m_ApiDataUpdateTimer = new Timer();
        private bool m_InfoSHown = false;
        private bool m_Recording = false;
        private string m_FilePath = string.Empty;
        private Color m_RedColor = Color.FromArgb(50, 255, 0, 0);
        private Color m_GreenColor = Color.FromArgb(50, 0, 255, 0);

        public MainUI()
        {
            InitializeComponent();

            if (!File.Exists("targetpath.txt"))
            {
                File.WriteAllText("targetpath.txt", ";");
            }

            if (!File.Exists("revivepath.txt"))
            {
                File.WriteAllText("revivepath.txt", ";");
            }

            if (!File.Exists("shoppath.txt"))
            {
                File.WriteAllText("shoppath.txt", ";");
            }

            ReadPaths();

            m_Automater.RecordPathEvent += Automater_RecordPathEvent;
            m_Automater.StopEvent += Automater_StopEvent;

            PathTypeDropDown.SelectedIndex = 0;

            ModeDropDown.SelectedIndex = 0;
            m_ApiDataUpdateTimer.Interval = 100;
            m_ApiDataUpdateTimer.Tick += ApiDataUpdateTimer_Tick;
            m_ApiDataUpdateTimer.Enabled = true;

            m_Automater.AutomaterStatusEvent += AutomaterStatusEvent;
            WowApi.UpdateEvent += ApiEvent;

            Task.Run(() =>
            {
                try
                {
                    m_Automater.Run();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            });
        }

        private void Automater_StopEvent(object sender, EventArgs ea)
        {
            if (InvokeRequired)
            {
                InvokeWithoutDisposedException(new MethodInvoker(() => { Automater_StopEvent(sender, ea); }));
                return;
            }

            XTextBox.Enabled = true;
            YTextBox.Enabled = true;
            LoadFileButton.Enabled = true;
            SaveFileButton.Enabled = true;
            OKButton.Enabled = true;

            RecordButton.Text = "Record Path";
            RecordButton.BackColor = System.Drawing.Color.FromArgb(255, 128, 255, 128);

            m_Recording = false;
        }

        private void Automater_RecordPathEvent(object sender, RecordPathEventArgs wea)
        {
            if (InvokeRequired)
            {
                InvokeWithoutDisposedException(new MethodInvoker(() => { Automater_RecordPathEvent(sender, wea); }));
                return;
            }

            XTextBox.Text += wea.X.ToString("N3") + ", ";
            YTextBox.Text += wea.Y.ToString("N3") + ", ";
        }

        private void ReadPaths()
        {
            string allpaths = File.ReadAllText("targetpath.txt");
            string[] paths = allpaths.Split(';');

            m_Automater.SetPathCoordinates(Program.ExtractCommaDelimitedDoubles(paths[0]),
                                     Program.ExtractCommaDelimitedDoubles(paths[1]));


            allpaths = File.ReadAllText("revivepath.txt");
            paths = allpaths.Split(';');

            m_Automater.SetReviveCoordinates(Program.ExtractCommaDelimitedDoubles(paths[0]),
                                     Program.ExtractCommaDelimitedDoubles(paths[1]));


            allpaths = File.ReadAllText("shoppath.txt");
            paths = allpaths.Split(';');

            m_Automater.SetShopCoordinates(Program.ExtractCommaDelimitedDoubles(paths[0]),
                                     Program.ExtractCommaDelimitedDoubles(paths[1]));

        }

        private void ApiDataUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeWithoutDisposedException(new MethodInvoker(() => { ApiDataUpdateTimer_Tick(sender, e); }));
                return;
            }

            DataTextBox.Text = m_PlayerData.ToString();
            StatusLabel.Text = m_AutomaterStatusString;

            if (m_PlayerData.Found)
            {
                RecordButton.Enabled = true;
                InfoTab.BackColor = m_GreenColor;
            }
            else
            {
                RecordButton.Enabled = false;
                InfoTab.BackColor = m_RedColor;
            }
        }

        private void AutomaterStatusEvent(object sender, AutomaterStatusEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeWithoutDisposedException(new MethodInvoker(() => { AutomaterStatusEvent(sender, e); }));
                return;
            }

            m_AutomaterStatusString = e.Status;
        }

        private void InvokeWithoutDisposedException(Delegate method)
        {
            try
            {
                Invoke(method);
            }
            catch (ObjectDisposedException ode)
            {
                // Do nothing
            }
            
        }

        private void ApiEvent(object sender, WowApiUpdateEventArguments e)
        {
            if (InvokeRequired)
            {
                InvokeWithoutDisposedException(new MethodInvoker(() => { ApiEvent(sender, e); }));
                return;
            }

            m_PlayerData = e.PlayerData;
        }

        private void ModeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(ModeDropDown.SelectedIndex)
            {
                case 0:
                    m_Automater.AutoAttackMode = false;
                    break;
                case 1:
                    m_Automater.AutoAttackMode = true;
                    break;
            }
        }

        private void ShowInfoButton_Click(object sender, EventArgs e)
        {
            if(m_InfoSHown)
            {
                Size = new System.Drawing.Size(236, 100);
                OptionTabs.Visible = false;
                ShowInfoButton.BackgroundImage = Properties.Resources.show;
            }
            else
            {
                Size = new System.Drawing.Size(236, 666);
                OptionTabs.Visible = true;
                ShowInfoButton.BackgroundImage = Properties.Resources.hide;
            }

            m_InfoSHown = !m_InfoSHown;
        }

        private void PathTypeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilePathFromDropDownIndex(PathTypeDropDown.SelectedIndex);
            ReadPathFromFile();
        }

        private void SetFilePathFromDropDownIndex(int index)
        {
            switch (index)
            {
                case 0:
                    m_FilePath = "targetpath.txt";
                    break;
                case 1:
                    m_FilePath = "revivepath.txt";
                    break;
                case 2:
                    m_FilePath = "shoppath.txt";
                    break;
            }
        }

        private void ReadPathFromFile()
        {
            if (File.Exists(m_FilePath))
            {
                string allpaths = File.ReadAllText(m_FilePath);
                string[] paths = allpaths.Split(';');

                XTextBox.Text = paths[0];
                YTextBox.Text = paths[1];
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            File.WriteAllText(m_FilePath, XTextBox.Text + ";" + YTextBox.Text);
            ReadPaths();
        }

        private void RecordButton_Click(object sender, EventArgs e)
        {
            if (m_Recording)
            {
                m_Automater.StopRecordingPath();
            }
            else
            {
                XTextBox.Text = "";
                YTextBox.Text = "";

                XTextBox.Enabled = false;
                YTextBox.Enabled = false;
                LoadFileButton.Enabled = false;
                SaveFileButton.Enabled = false;
                OKButton.Enabled = false;

                RecordButton.Text = "Stop Recording";
                RecordButton.BackColor = System.Drawing.Color.FromArgb(255, 255, 128, 128);

                Task.Run(() =>
                {
                    try
                    {
                        m_Automater.RecordPath();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                });

                m_Recording = true;
            }
        }

        private void SaveFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text |*.txt";

            sfd.Title = "Save path";
            sfd.FileName = m_FilePath;
            sfd.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (sfd.FileName != "")
            {
                File.WriteAllText(sfd.FileName, XTextBox.Text + ";" + YTextBox.Text);
            }
        }

        private void LoadFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text |*.txt";
            ofd.Title = "Open path";
            ofd.FileName = m_FilePath;
            ofd.ShowDialog();

            if (ofd.FileName != "")
            {
                string allpaths = File.ReadAllText(ofd.FileName);
                string[] paths = allpaths.Split(';');

                XTextBox.Text = paths[0];
                YTextBox.Text = paths[1];
            }
        }

        private void StealthlevelNumericInput_ValueChanged(object sender, EventArgs e)
        {
            m_Automater.StealthLevel = (int)Math.Round(StealthLevelNumericInput.Value);
        }

        private void TurnToleranceNumericInput_ValueChanged(object sender, EventArgs e)
        {
            m_Automater.TurnToleranceRad = (double)TurnToleranceNumericInput.Value;
        }

        private void OptionTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Recording)
            {
                m_Automater.StopRecordingPath();
            }

            // OptionTabs.SelectedIndex == 2 "Automater" tab
            if (OptionTabs.SelectedIndex == 2 && 
                m_PlayerData.Class > PlayerClass.None && 
                m_PlayerData.Class <= PlayerClass.LastPlayerClass)
            {
                ClassTabs.SelectedIndex = (int)m_PlayerData.Class - 1;
            }
        }

        private void StaleStealthNumericInput_ValueChanged(object sender, EventArgs e)
        {
            m_Automater.StaleStealthTimer.Interval = (double)(StaleStealthNumericInput.Value * 1000);
        }

        private void RegisterDelayNumericInput_ValueChanged(object sender, EventArgs e)
        {
            m_Automater.RegisterDelay = (double)RegisterDelayNumericInput.Value;
        }

        private void SplitDistanceNumericInput_ValueChanged(object sender, EventArgs e)
        {
            m_Automater.SplitDistance = (double)SplitDistanceNumericInput.Value;
        }

        private void SliceNDiceCPNumericInput_ValueChanged(object sender, EventArgs e)
        {
            m_Automater.SliceAndDiceComboPoints = (int)Math.Round(SliceNDiceCPNumericInput.Value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            m_Automater.RuptureComboPoints = (int)Math.Round(RuptureCPNumericInput.Value);
        }

        private void EvisceratePercentageNumericInput_ValueChanged(object sender, EventArgs e)
        {
            m_Automater.EvisceratePercentage = (int)Math.Round(EvisceratePercentageNumericInput.Value);
        }

        private void ClosestPointDistanceNumericInput_ValueChanged(object sender, EventArgs e)
        {
            m_Automater.ClosestPointDistance = (double)ClosestPointDistanceNumericInput.Value;
        }

        private void MainUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            WowApi.UpdateEvent -= ApiEvent;

            m_Automater.RecordPathEvent -= Automater_RecordPathEvent;
            m_Automater.StopEvent -= Automater_StopEvent;
            m_ApiDataUpdateTimer.Enabled = false;
            m_ApiDataUpdateTimer.Tick -= ApiDataUpdateTimer_Tick;
            m_Automater.AutomaterStatusEvent -= AutomaterStatusEvent;

            m_Automater.Stop();
        }
    }
}
