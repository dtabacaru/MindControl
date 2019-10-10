using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace ClassicWowNeuralParasite
{
    public partial class MainUI : Form
    {
        private string WowAutomaterStatusString = string.Empty;
        private Timer m_ApiDataUpdateTimer = new Timer();
        private bool m_InfoSHown = false;
        private bool m_Recording = false;
        private string m_FilePath = string.Empty;
        private Color m_RedColor = Color.FromArgb(50, 255, 0, 0);
        private Color m_GreenColor = Color.FromArgb(50, 0, 255, 0);
        private InputSimulator m_InputSimulator = new InputSimulator();

        public MainUI()
        {
            InitializeComponent();

            if (File.Exists("Config.ftw"))
            {
                ReadConfigString(File.ReadAllText("Config.ftw"));
            }      

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

            if (!File.Exists("walkpath.txt"))
            {
                File.WriteAllText("walkpath.txt", ";");
            }

            ReadPaths();

            RecordWowPath.RecordPathEvent += Automater_RecordPathEvent;
            RecordWowPath.StopEvent += Automater_StopEvent;

            PathTypeDropDown.SelectedIndex = 0;

            ModeDropDown.SelectedIndex = 0;
            m_ApiDataUpdateTimer.Interval = 100;
            m_ApiDataUpdateTimer.Tick += ApiDataUpdateTimer_Tick;
            m_ApiDataUpdateTimer.Enabled = true;

            WowAutomater.AutomaterStatusEvent += AutomaterStatusEvent;

            Task.Run(() =>
            {
                try
                {
                    WowAutomater.Run();
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

        private void ReadConfigString(string configString)
        {
            string[] configStrings = configString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string config in configStrings)
            {
                // Ignore comments
                if (config.Substring(0, 2) == "--")
                    continue;

                ParseConfig(config);
            }
        }

        private void ParseConfig(string config)
        {
            string[] configParts = config.Split(':');

            if (configParts.Length != 2)
                throw new Exception("Unknown line in config: " + config);

            string configName = configParts[0].Trim();
            string configValue = configParts[1].Trim();

            switch (configName)
            {
                case "Split distance":
                    RecordWowPath.SplitDistance = Convert.ToDouble(configValue);
                    SplitDistanceNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Turn tolerance (rad)":
                    WaypointFollower.TurnToleranceRad = Convert.ToDouble(configValue);
                    TurnToleranceNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Position tolerance":
                    WaypointFollower.PositionTolerance = Convert.ToDouble(configValue);
                    PositionToleranceNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Register delay (sec)":
                    WowAutomater.RegisterDelay = Convert.ToDouble(configValue);
                    RegisterDelayNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Recompute waypoint distance":
                    WaypointFollower.ClosestPointDistance = Convert.ToDouble(configValue);
                    ClosestPointDistanceNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Skin after loot":
                    WowAutomater.SkinLoot = Convert.ToBoolean(configValue);
                    SkinLootCheckbox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "X revive button location":
                    WowAutomater.XReviveButtonLocation = Convert.ToDouble(configValue) * 100;
                    XReviveButtonLocationNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Y revive button location":
                    WowAutomater.YReviveButtonLocation = Convert.ToDouble(configValue) * 100;
                    YReviveButtonLocationNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Regenerate health %":
                    WowAutomater.RegenerateVitalsHealthPercentage = Convert.ToDouble(configValue);
                    RegenerateVitalsNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Auto loot":
                    WowAutomater.AutoLoot = Convert.ToBoolean(configValue);
                    AutoLootLabelCheckbox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "First seal":
                    
                    switch (configValue)
                    {
                        case "None":
                            WowAutomater.Paladin.FirstSeal = FirstSealType.None;
                            FirstSealCrusaderButton.Checked = true;
                            break;
                        case "Crusader":
                            WowAutomater.Paladin.FirstSeal = FirstSealType.Crusader;
                            FirstSealCrusaderButton.Checked = true;
                            break;
                        case "Justice":
                            WowAutomater.Paladin.FirstSeal = FirstSealType.Justice;
                            FirstSealJusticeButton.Checked = true;
                            break;
                        default:
                            throw new Exception("Unknown line in config: " + config);
                    }

                    break;
                case "Stealth":
                    WowAutomater.Rogue.StealthFlag = Convert.ToBoolean(configValue);
                    StealthCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Stealth level":
                    WowAutomater.Rogue.StealthLevel = Convert.ToInt32(configValue);
                    StealthLevelNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Remove stealth after (sec)":
                    WowAutomater.Rogue.StaleStealthTimer.Interval = Convert.ToDouble(configValue) * 1000;
                    StaleStealthNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Always throw":
                    WowAutomater.Rogue.ThrowFlag = Convert.ToBoolean(configValue);
                    AlwaysThrowCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Slice and dice combo points":
                    WowAutomater.Rogue.SliceAndDice.MinimumComboPointsCost = Convert.ToUInt16(configValue);
                    WowAutomater.Rogue.SliceAndDice.MaximumComboPointsCost = Convert.ToUInt16(configValue);
                    SliceNDiceCPNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Rupture combo points":
                    WowAutomater.Rogue.Rupture.MinimumComboPointsCost = Convert.ToUInt16(configValue);
                    RuptureCPNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Eviscerate percentage":
                    WowAutomater.Rogue.Eviscerate.TargetHealthPercentage = Convert.ToDouble(configValue);
                    EvisceratePercentageNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Evasion percentage":
                    WowAutomater.Rogue.Evasion.PlayerHealthPercentage = Convert.ToDouble(configValue);
                    EvasionPercentaceNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Rupture first":
                    WowAutomater.Rogue.RuptureFirst = Convert.ToBoolean(configValue);
                    RuptureFirstCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Passive humanoid":
                    WowAutomater.Druid.Passive = Convert.ToBoolean(configValue);
                    PassiveHumanoidCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                default:
                    throw new Exception("Unknown line in config: " + config);
            }
        }

        private void ReadPaths()
        {
            string allpaths = File.ReadAllText("targetpath.txt");
            string[] paths = allpaths.Split(';');

            WowAutomater.SetPathCoordinates(Program.ExtractCommaDelimitedDoubles(paths[0]),
                                     Program.ExtractCommaDelimitedDoubles(paths[1]));


            allpaths = File.ReadAllText("revivepath.txt");
            paths = allpaths.Split(';');

            WowAutomater.SetReviveCoordinates(Program.ExtractCommaDelimitedDoubles(paths[0]),
                                     Program.ExtractCommaDelimitedDoubles(paths[1]));


            allpaths = File.ReadAllText("shoppath.txt");
            paths = allpaths.Split(';');

            WowAutomater.SetShopCoordinates(Program.ExtractCommaDelimitedDoubles(paths[0]),
                                     Program.ExtractCommaDelimitedDoubles(paths[1]));

            allpaths = File.ReadAllText("walkpath.txt");
            paths = allpaths.Split(';');

            WowAutomater.SetWalkCoordinates(Program.ExtractCommaDelimitedDoubles(paths[0]),
                                     Program.ExtractCommaDelimitedDoubles(paths[1]));

        }

        private void ApiDataUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeWithoutDisposedException(new MethodInvoker(() => { ApiDataUpdateTimer_Tick(sender, e); }));
                return;
            }

            DataTextBox.Text = WowApi.PlayerData.ToString();
            StatusLabel.Text = WowAutomaterStatusString;

            if (WowApi.PlayerData.Found)
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

        private void AutomaterStatusEvent(object sender, AutomaterActionEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeWithoutDisposedException(new MethodInvoker(() => { AutomaterStatusEvent(sender, e); }));
                return;
            }

            WowAutomaterStatusString = e.CurrentAction.ToString();
        }

        private void InvokeWithoutDisposedException(Delegate method)
        {
            try
            {
                Invoke(method);
            }
            catch (ObjectDisposedException)
            {
                // Do nothing
            }
            
        }

        private void ModeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(ModeDropDown.SelectedIndex)
            {
                case 0:
                    WowAutomater.CurrentActionMode = ActionMode.FindTarget;
                    break;
                case 1:
                    WowAutomater.CurrentActionMode = ActionMode.AutoAttack;
                    break;
                case 2:
                    WowAutomater.CurrentActionMode = ActionMode.AutoWalk;
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
                Size = new System.Drawing.Size(236, 760);
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
                case 3:
                    m_FilePath = "walkpath.txt";
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
                RecordWowPath.StopRecordingPath();
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
                        RecordWowPath.RecordPath();
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

        private void StealthLevelNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WowAutomater.Rogue.StealthLevel = (int)Math.Round(StealthLevelNumericInput.Value);
        }

        private void TurnToleranceNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WaypointFollower.TurnToleranceRad = (double)TurnToleranceNumericInput.Value;
        }

        private void OptionTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Recording)
            {
                RecordWowPath.StopRecordingPath();
            }

            // OptionTabs.SelectedIndex == 3 "Classes" tab
            if (OptionTabs.SelectedIndex == 3 && 
                WowApi.PlayerData.Class > PlayerClassType.None && 
                WowApi.PlayerData.Class <= PlayerClassType.LastPlayerClass)
            {
                ClassTabs.SelectedIndex = (int)WowApi.PlayerData.Class - 1;
            }
        }

        private void StaleStealthNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WowAutomater.Rogue.StaleStealthTimer.Interval = (double)(StaleStealthNumericInput.Value * 1000);
        }

        private void RegisterDelayNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WowAutomater.RegisterDelay = (double)RegisterDelayNumericInput.Value;
        }

        private void SplitDistanceNumericInput_ValueChanged(object sender, EventArgs e)
        {
            RecordWowPath.SplitDistance = (double)SplitDistanceNumericInput.Value;
        }

        private void SliceNDiceCPNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WowAutomater.Rogue.SliceAndDice.MaximumComboPointsCost = (ushort)SliceNDiceCPNumericInput.Value ;
            WowAutomater.Rogue.SliceAndDice.MinimumComboPointsCost = (ushort)SliceNDiceCPNumericInput.Value;
        }

        private void RuptureCPNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WowAutomater.Rogue.Rupture.MinimumComboPointsCost = (ushort)RuptureCPNumericInput.Value;
        }

        private void EvisceratePercentageNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WowAutomater.Rogue.Eviscerate.TargetHealthPercentage = (ushort)EvisceratePercentageNumericInput.Value;
        }

        private void ClosestPointDistanceNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WaypointFollower.ClosestPointDistance = (double)ClosestPointDistanceNumericInput.Value;
        }

        private void MainUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            RecordWowPath.RecordPathEvent -= Automater_RecordPathEvent;
            RecordWowPath.StopEvent -= Automater_StopEvent;
            m_ApiDataUpdateTimer.Enabled = false;
            m_ApiDataUpdateTimer.Tick -= ApiDataUpdateTimer_Tick;
            WowAutomater.AutomaterStatusEvent -= AutomaterStatusEvent;

            WowAutomater.Stop();
        }

        private void SkinLootCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            WowAutomater.SkinLoot = SkinLootCheckbox.Checked;
        }

        private void ReversePathButton_Click(object sender, EventArgs e)
        {
            List<double> xCoordinates = Program.ExtractCommaDelimitedDoubles(XTextBox.Text);
            List<double> yCoordinates = Program.ExtractCommaDelimitedDoubles(YTextBox.Text);

            xCoordinates.Reverse();
            yCoordinates.Reverse();

            string xPath = string.Empty;
            string yPath = string.Empty;

            foreach(double xCoordinate in xCoordinates)
            {
                xPath += xCoordinate.ToString("N3") + ", ";
            }

            foreach (double yCoordinate in yCoordinates)
            {
                yPath += yCoordinate.ToString("N3") + ", ";
            }

            XTextBox.Text = xPath;
            YTextBox.Text = yPath;
        }

        private void AlwaysThrowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            WowAutomater.Rogue.ThrowFlag = AlwaysThrowCheckBox.Checked;
        }

        private void StealthCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            WowAutomater.Rogue.StealthFlag = StealthCheckBox.Checked;
        }

        private void EvasionPercentaceNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WowAutomater.Rogue.Evasion.PlayerHealthPercentage = (int)EvasionPercentaceNumericInput.Value;
        }

        private void XReviveButtonLocationNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WowAutomater.XReviveButtonLocation = (double)XReviveButtonLocationNumericInput.Value * 100;
        }

        private void YReviveButtonLocationNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WowAutomater.YReviveButtonLocation = (double)YReviveButtonLocationNumericInput.Value * 100;
        }

        private void ReviveButtonLocationTestButton_Click(object sender, EventArgs e)
        {
            m_InputSimulator.Mouse.MoveMouseTo((double)XReviveButtonLocationNumericInput.Value * 100,
                                               (double)YReviveButtonLocationNumericInput.Value * 100);
        }

        private void PassiveHumanoidCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            WowAutomater.Druid.Passive = PassiveHumanoidCheckBox.Checked;
        }

        private void RegenerateVitalsNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WowAutomater.RegenerateVitalsHealthPercentage = (double)RegenerateVitalsNumericInput.Value;
        }

        private void AutoLootLabelCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            WowAutomater.AutoLoot = AutoLootLabelCheckbox.Checked;
        }

        private void RuptureFirstCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            WowAutomater.Rogue.RuptureFirst = RuptureFirstCheckBox.Checked;
        }

    }
}
