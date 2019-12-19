using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WowApi;
using WowAutomater;

namespace MindControlUI
{
    public partial class MainUI : Form
    {
        private string AutomaterStatusString = string.Empty;
        private System.Windows.Forms.Timer m_ApiDataUpdateTimer = new System.Windows.Forms.Timer();
        private bool m_InfoSHown = false;
        private bool m_Recording = false;
        private Color m_RedColor = Color.FromArgb(50, 255, 0, 0);
        private Color m_GreenColor = Color.FromArgb(50, 0, 255, 0);
        private InputSimulator m_InputSimulator = new InputSimulator();
        private AutomaterWebInterface m_WebInterface = new AutomaterWebInterface();
        private bool m_WebInterfaceRunning = false;
        private volatile bool m_SendEmails = true;
        private volatile bool m_SendingEmail = false;

        List<Waypoint> m_TargetPathWaypoints = new List<Waypoint>();
        List<Waypoint> m_RevivePathWaypoints = new List<Waypoint>();
        List<Waypoint> m_SellPathWaypoints = new List<Waypoint>();
        List<Waypoint> m_WalkPathWaypoints = new List<Waypoint>();

        public MainUI()
        {
            InitializeComponent();

            Size = new Size(236, 100);

            if (File.Exists("Config.ftw"))
                ReadConfigString(File.ReadAllText("Config.ftw"));

            if (File.Exists("targetpath.txt"))
            {
                ReadPathFromFile("targetpath.txt", m_TargetPathWaypoints);
                Automater.SetPathWaypoints(m_TargetPathWaypoints);
            } 
            else
                File.WriteAllText("targetpath.txt", ";;");

            if (File.Exists("revivepath.txt"))
            {
                ReadPathFromFile("revivepath.txt", m_RevivePathWaypoints);
                Automater.SetReviveWaypoints(m_RevivePathWaypoints);
            }
            else
                File.WriteAllText("revivepath.txt", ";;");

            if (File.Exists("shoppath.txt"))
            {
                ReadPathFromFile("shoppath.txt", m_SellPathWaypoints);
                Automater.SetShopWaypoints(m_SellPathWaypoints);
            }
            else
                File.WriteAllText("shoppath.txt", ";;");

            if (File.Exists("walkpath.txt"))
            {
                ReadPathFromFile("walkpath.txt", m_WalkPathWaypoints);
                Automater.SetWalkWaypoints(m_WalkPathWaypoints);
            }
            else
                File.WriteAllText("walkpath.txt", ";;");

            RecordWowPath.RecordPathEvent += Automater_RecordPathEvent;
            RecordWowPath.StopEvent += Automater_StopEvent;

            PathTypeDropDown.SelectedIndex = 0;

            ModeDropDown.SelectedIndex = 0;
            m_ApiDataUpdateTimer.Interval = 50;
            m_ApiDataUpdateTimer.Tick += ApiDataUpdateTimer_Tick;
            m_ApiDataUpdateTimer.Enabled = true;

            Automater.AutomaterStatusEvent += AutomaterStatusEvent;

            Api.UpdateEvent += Api_UpdateEvent;

            Task.Run(() =>
            {
                try
                {
                    Automater.Run();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            });

        }

        private void SendEmail()
        {
            if (!m_SendingEmail && 
                m_SendEmails && 
                WhisperEmailToTextBox.Text != string.Empty &&
                SMTPNameTextBox.Text != string.Empty &&
                SMTPPasswordTextBox.Text != string.Empty &&
                SMTPServerTextBox.Text != string.Empty) 
            {
                m_SendingEmail = true;

                Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(1000);

                    try
                    {
                        using (SmtpClient client = new SmtpClient(SMTPServerTextBox.Text, 587)
                        {
                            EnableSsl = true,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(SMTPNameTextBox.Text, SMTPPasswordTextBox.Text),
                        })

                        using (var mm = new MailMessage())
                        {
                            mm.To.Add(WhisperEmailToTextBox.Text);
                            mm.From = new MailAddress(SMTPNameTextBox.Text);
                            mm.Subject = "AutomaterUi: Whisper Alert";

                            Rectangle bounds = new Rectangle(0, 500, 500, 500);

                            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                            {
                                using (Graphics g = Graphics.FromImage(bitmap))
                                {
                                    g.CopyFromScreen(new Point(0, 500), Point.Empty, bounds.Size);
                                }

                                using (var memoryStream = new MemoryStream())
                                {
                                    bitmap.Save(memoryStream, ImageFormat.Png);
                                    memoryStream.Position = 0;

                                    var imageResource = new LinkedResource(memoryStream, "image/png") { ContentId = "added-image-id" };
                                    var alternateView = AlternateView.CreateAlternateViewFromString(mm.Body, mm.BodyEncoding, MediaTypeNames.Text.Html);

                                    alternateView.LinkedResources.Add(imageResource);
                                    mm.AlternateViews.Add(alternateView);

                                    client.Send(mm);
                                }
                            }

                        }// using (var mm = new MailMessage())
                    }// try
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    m_SendingEmail = false;
                });
            }
        }

        private void Api_UpdateEvent(object sender, EventArgs ea)
        {
            if (Api.PlayerData.Found && Api.PlayerData.Whisper)
                SendEmail();
        }

        private void Automater_StopEvent(object sender, EventArgs ea)
        {
            if (InvokeRequired)
            {
                InvokeWithoutDisposedException(new MethodInvoker(() => { Automater_StopEvent(sender, ea); }));
                return;
            }

            LoadFileButton.Enabled = true;
            SaveFileButton.Enabled = true;
            ApplyPathButton.Enabled = true;
            PathTypeDropDown.Enabled = true;

            RecordButton.Text = "Record Path";
            RecordButton.BackColor = System.Drawing.Color.FromArgb(255, 128, 255, 128);

            m_Recording = false;
            MapBox.Invalidate();
        }

        private void Automater_RecordPathEvent(object sender, RecordPathEventArgs wea)
        {
            if (InvokeRequired)
            {
                InvokeWithoutDisposedException(new MethodInvoker(() => { Automater_RecordPathEvent(sender, wea); }));
                return;
            }

            switch (PathTypeDropDown.SelectedIndex)
            {
                case 0:
                    m_TargetPathWaypoints.Add(wea.CurrentWaypoint);
                    break;
                case 1:
                    m_RevivePathWaypoints.Add(wea.CurrentWaypoint);
                    break;
                case 2:
                    m_SellPathWaypoints.Add(wea.CurrentWaypoint);
                    break;
                case 3:
                    m_WalkPathWaypoints.Add(wea.CurrentWaypoint);
                    break;
            }

            MapBox.Invalidate();
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
                    Automater.RegisterDelay = Convert.ToDouble(configValue);
                    RegisterDelayNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Recompute waypoint distance":
                    WaypointFollower.ClosestPointDistance = Convert.ToDouble(configValue);
                    ClosestPointDistanceNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Skin after loot":
                    Automater.SkinLoot = Convert.ToBoolean(configValue);
                    SkinLootCheckbox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "X revive button location":
                    Automater.XReviveButtonLocation = Convert.ToDouble(configValue) * 100;
                    XReviveButtonLocationNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Y revive button location":
                    Automater.YReviveButtonLocation = Convert.ToDouble(configValue) * 100;
                    YReviveButtonLocationNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Regenerate health %":
                    Automater.RegenerateVitalsHealthPercentage = Convert.ToDouble(configValue);
                    RegenerateVitalsNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Auto loot":
                    Automater.AutoLoot = Convert.ToBoolean(configValue);
                    AutoLootLabelCheckbox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Send whisper e-mail":
                    m_SendEmails = Convert.ToBoolean(configValue);
                    WhisperEmailCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Whisper e-mail to":
                    WhisperEmailToTextBox.Text = configValue;
                    break;
                case "SMTP account name":
                    SMTPNameTextBox.Text = configValue;
                    break;
                case "SMTP account password":
                    SMTPPasswordTextBox.Text = configValue;
                    break;
                case "SMTP server":
                    SMTPServerTextBox.Text = configValue;
                    break;
                case "Remote user":
                    RemoteUserTextBox.Text = configValue;
                    break;
                case "Remote server":
                    RemoteServerTextBox.Text = configValue;
                    break;
                case "Find target rate":
                    WaypointFollower.Jitterizer.Rate = Convert.ToDouble(configValue);
                    FindTargetRateNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Find target wait time":
                    WaypointFollower.Jitterizer.WaitTime = Convert.ToDouble(configValue);
                    FindTargetWaitTimeNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Find target jump":
                    WaypointFollower.Jitterizer.Jump = Convert.ToBoolean(configValue);
                    FindTargetJumpCheckBox.Checked = Convert.ToBoolean(configValue); ;
                    break;
                case "Find target left":
                    WaypointFollower.Jitterizer.Left = Convert.ToBoolean(configValue);
                    FindTargetLeftCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Find target right":
                    WaypointFollower.Jitterizer.Right = Convert.ToBoolean(configValue);
                    FindTargetRightCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Find target leftright":
                    WaypointFollower.Jitterizer.LeftRight = Convert.ToBoolean(configValue);
                    FindTargetLeftRightCheckBox.Checked =  Convert.ToBoolean(configValue);
                    break;
                case "Find target rightleft":
                    WaypointFollower.Jitterizer.RightLeft = Convert.ToBoolean(configValue);
                    FindTargetRightLeftCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Kill target rate":
                    Automater.PlayerJitterizer.Rate = Convert.ToDouble(configValue);
                    KillTargetRateNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Kill target wait time":
                    Automater.PlayerJitterizer.WaitTime = Convert.ToDouble(configValue);
                    KillTargetWaitTimeNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Kill target jump":
                    Automater.PlayerJitterizer.Jump = Convert.ToBoolean(configValue);
                    KillTargetJumpCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Kill target updown":
                    Automater.PlayerJitterizer.UpDown = Convert.ToBoolean(configValue);
                    KillTargetUpDownCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Kill target downup":
                    Automater.PlayerJitterizer.DownUp = Convert.ToBoolean(configValue);
                    KillTargetDownUpCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Kill target leftright":
                    Automater.PlayerJitterizer.LeftRight = Convert.ToBoolean(configValue);
                    KillTargetLeftRightCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Kill target rightleft":
                    Automater.PlayerJitterizer.RightLeft = Convert.ToBoolean(configValue);
                    KillTargetRightLeftCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Kill target clockwise":
                    Automater.PlayerJitterizer.Clockwise = Convert.ToBoolean(configValue);
                    KillTargetClockwiseCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Kill target counter clockwise":
                    Automater.PlayerJitterizer.CounterClockwise = Convert.ToBoolean(configValue);
                    KillTargetCounterClockwiseCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "First seal":
                    
                    switch (configValue)
                    {
                        case "None":
                            Automater.Paladin.FirstSeal = FirstSealType.None;
                            FirstSealNoneButton.Checked = true;
                            break;
                        case "Crusader":
                            Automater.Paladin.FirstSeal = FirstSealType.Crusader;
                            FirstSealCrusaderButton.Checked = true;
                            break;
                        case "Justice":
                            Automater.Paladin.FirstSeal = FirstSealType.Justice;
                            FirstSealJusticeButton.Checked = true;
                            break;
                        default:
                            throw new Exception("Unknown line in config: " + config);
                    }

                    break;
                case "Stealth":
                    Automater.Rogue.StealthFlag = Convert.ToBoolean(configValue);
                    StealthCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Stealth forever":
                    Automater.Rogue.AlwaysStealth = Convert.ToBoolean(configValue);
                    StealthForeverCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Remove stealth after (sec)":
                    Automater.Rogue.StaleStealthTimer.Interval = Convert.ToDouble(configValue) * 1000;
                    StaleStealthNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Always throw":
                    Automater.Rogue.ThrowFlag = Convert.ToBoolean(configValue);
                    AlwaysThrowCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Slice and dice combo points":
                    Automater.Rogue.SliceAndDice.MinimumComboPointsCost = Convert.ToUInt16(configValue);
                    Automater.Rogue.SliceAndDice.MaximumComboPointsCost = Convert.ToUInt16(configValue);
                    SliceNDiceCPNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Rupture combo points":
                    Automater.Rogue.Rupture.MinimumComboPointsCost = Convert.ToUInt16(configValue);
                    RuptureCPNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Eviscerate percentage":
                    Automater.Rogue.Eviscerate.TargetHealthPercentage = Convert.ToDouble(configValue);
                    EvisceratePercentageNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Evasion percentage":
                    Automater.Rogue.Evasion.PlayerHealthPercentage = Convert.ToDouble(configValue);
                    EvasionPercentaceNumericInput.Value = Convert.ToDecimal(configValue);
                    break;
                case "Rupture first":
                    Automater.Rogue.RuptureFirst = Convert.ToBoolean(configValue);
                    RuptureFirstCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Apply poison":
                    Automater.Rogue.ApplyPoison = Convert.ToBoolean(configValue);
                    ApplyPoisonCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Don't throw around players":
                    Automater.Rogue.DontThrow = Convert.ToBoolean(configValue);
                    DontThrowCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                case "Throw after (sec)":
                    Automater.Rogue.FriendlyTimer.Interval = Convert.ToDouble(configValue) * 1000;
                    ThrowAfterNumericInput.Value = Convert.ToDecimal(configValue); break;
                case "Passive humanoid":
                    Automater.Druid.Passive = Convert.ToBoolean(configValue);
                    PassiveHumanoidCheckBox.Checked = Convert.ToBoolean(configValue);
                    break;
                default:
                    throw new Exception("Unknown line in config: " + config);
            }
        }

        /*
        private void ReadPaths()
        {
            string allpaths = File.ReadAllText("targetpath.txt");
            string[] paths = allpaths.Split(';');

            Automater.SetPathCoordinates(Program.ExtractCommaDelimitedDoubles(paths[0]),
                                     Program.ExtractCommaDelimitedDoubles(paths[1]));


            allpaths = File.ReadAllText("revivepath.txt");
            paths = allpaths.Split(';');

            Automater.SetReviveCoordinates(Program.ExtractCommaDelimitedDoubles(paths[0]),
                                     Program.ExtractCommaDelimitedDoubles(paths[1]));


            allpaths = File.ReadAllText("shoppath.txt");
            paths = allpaths.Split(';');

            Automater.SetShopCoordinates(Program.ExtractCommaDelimitedDoubles(paths[0]),
                                     Program.ExtractCommaDelimitedDoubles(paths[1]));

            allpaths = File.ReadAllText("walkpath.txt");
            paths = allpaths.Split(';');

            Automater.SetWalkCoordinates(Program.ExtractCommaDelimitedDoubles(paths[0]),
                                     Program.ExtractCommaDelimitedDoubles(paths[1]));

        }
        */

        private bool m_LastFound = true;

        private void ApiDataUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeWithoutDisposedException(new MethodInvoker(() => { ApiDataUpdateTimer_Tick(sender, e); }));
                return;
            }

            DataTextBox.Text = Api.PlayerData.ToString();
            StatusLabel.Text = AutomaterStatusString;

            if (Api.PlayerData.Found && !m_LastFound)
            {
                RecordButton.Enabled = true;
                InfoTab.BackColor = m_GreenColor;
            }
            else if(!Api.PlayerData.Found && m_LastFound)
            {
                RecordButton.Enabled = false;
                InfoTab.BackColor = m_RedColor;
            }

            m_LastFound = Api.PlayerData.Found;
            MapBox.Invalidate();
        }

        private void AutomaterStatusEvent(object sender, AutomaterActionEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeWithoutDisposedException(new MethodInvoker(() => { AutomaterStatusEvent(sender, e); }));
                return;
            }

            AutomaterStatusString = e.CurrentAction.ToString();
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
                    Automater.AutomaterActionMode = ActionMode.FindTarget;
                    break;
                case 1:
                    Automater.AutomaterActionMode = ActionMode.AutoAttack;
                    break;
                case 2:
                    Automater.AutomaterActionMode = ActionMode.AutoWalk;
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

        int selectedIndex = 0;

        private void PathTypeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (PathTypeDropDown.SelectedIndex)
            {
                case 0:
                    ReadPathFromFile("targetpath.txt", m_TargetPathWaypoints);
                    selectedIndex = 0;
                    if (m_TargetPathWaypoints.Count > 0)
                        MapBox.Image = new Bitmap(Helper.ResizeImage(new Bitmap("MapImages\\" + m_TargetPathWaypoints[0].MapId.ToString() + ".jpg"), 200, 140));
                    break;
                case 1:
                    ReadPathFromFile("revivepath.txt", m_RevivePathWaypoints);
                    selectedIndex = 1;
                    if (m_RevivePathWaypoints.Count > 0)
                        MapBox.Image = new Bitmap(Helper.ResizeImage(new Bitmap("MapImages\\" + m_RevivePathWaypoints[0].MapId.ToString() + ".jpg"), 200, 140));
                    break;
                case 2:
                    ReadPathFromFile("shoppath.txt", m_SellPathWaypoints);
                    selectedIndex = 2;
                    if (m_SellPathWaypoints.Count > 0)
                        MapBox.Image = new Bitmap(Helper.ResizeImage(new Bitmap("MapImages\\" + m_SellPathWaypoints[0].MapId.ToString() + ".jpg"), 200, 140));
                    break;
                case 3:
                    ReadPathFromFile("walkpath.txt", m_WalkPathWaypoints);
                    selectedIndex = 3;
                    if (m_WalkPathWaypoints.Count > 0)
                        MapBox.Image = new Bitmap(Helper.ResizeImage(new Bitmap("MapImages\\" + m_WalkPathWaypoints[0].MapId.ToString() + ".jpg"), 200, 140));
                    break;
            }
            
            MapBox.Invalidate();
        }

        private void ReadPathFromFile(string filePath, List<Waypoint> waypointList)
        {
            waypointList.Clear();
            string path = File.ReadAllText(filePath);

            string[] waypointsComponents = path.Split(';');

            List<uint> mapIds = Program.ExtractCommaDelimitedUInts(waypointsComponents[0].Trim());
            List<double> xs = Program.ExtractCommaDelimitedDoubles(waypointsComponents[1].Trim());
            List<double> ys = Program.ExtractCommaDelimitedDoubles(waypointsComponents[2].Trim());

            for (int i = 0; i < mapIds.Count; i++)
            {
                waypointList.Add(new Waypoint(mapIds[i], xs[i], ys[i]));
            }

        }

        private void SavePathToFile(string filePath, List<Waypoint> waypointList)
        {
            string mapIdsString = string.Empty;
            string xsString = string.Empty;
            string ysString = string.Empty;

            for(int i = 0; i < waypointList.Count; i++)
            {
                mapIdsString += waypointList[i].MapId + ",";
                xsString += waypointList[i].X + ",";
                ysString += waypointList[i].Y + ",";
            }

            File.WriteAllText(filePath, mapIdsString + ";" + xsString + ";" + ysString);
        }

        private void ApplyPathButton_Click(object sender, EventArgs e)
        {
            switch (PathTypeDropDown.SelectedIndex)
            {
                case 0:
                    Automater.SetPathWaypoints(m_TargetPathWaypoints);
                    SavePathToFile("targetpath.txt", m_TargetPathWaypoints);
                    break;
                case 1:
                    Automater.SetReviveWaypoints(m_RevivePathWaypoints);
                    SavePathToFile("revivepath.txt", m_RevivePathWaypoints);
                    break;
                case 2:
                    Automater.SetShopWaypoints(m_SellPathWaypoints);
                    SavePathToFile("shoppath.txt", m_SellPathWaypoints);
                    break;
                case 3:
                    Automater.SetWalkWaypoints(m_WalkPathWaypoints);
                    SavePathToFile("walkpath.txt", m_WalkPathWaypoints);
                    break;
            }
        }

        private void RecordButton_Click(object sender, EventArgs e)
        {
            if (m_Recording)
            {
                RecordWowPath.StopRecordingPath();
            }
            else
            {
                LoadFileButton.Enabled = false;
                SaveFileButton.Enabled = false;
                ApplyPathButton.Enabled = false;
                PathTypeDropDown.Enabled = false;

                RecordButton.Text = "Stop Recording";
                RecordButton.BackColor = System.Drawing.Color.FromArgb(255, 255, 128, 128);

                switch (PathTypeDropDown.SelectedIndex)
                {
                    case 0:
                        m_TargetPathWaypoints.Clear();
                        break;
                    case 1:
                        m_RevivePathWaypoints.Clear();
                        break;
                    case 2:
                        m_SellPathWaypoints.Clear();
                        break;
                    case 3:
                        m_WalkPathWaypoints.Clear();
                        break;
                }

                MapBox.Image = new Bitmap(Helper.ResizeImage(new Bitmap("MapImages\\" + Api.PlayerData.MapId.ToString() + ".jpg"), 200, 140));

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
            sfd.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (sfd.FileName != "")
            {
                switch (PathTypeDropDown.SelectedIndex)
                {
                    case 0:
                        SavePathToFile(sfd.FileName, m_TargetPathWaypoints);
                        break;
                    case 1:
                        SavePathToFile(sfd.FileName, m_RevivePathWaypoints);
                        break;
                    case 2:
                        SavePathToFile(sfd.FileName, m_SellPathWaypoints);
                        break;
                    case 3:
                        SavePathToFile(sfd.FileName, m_WalkPathWaypoints);
                        break;
                }

            }
        }

        private void LoadFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text |*.txt";
            ofd.Title = "Open path";
            ofd.ShowDialog();

            if (ofd.FileName != "")
            {
                switch (PathTypeDropDown.SelectedIndex)
                {
                    case 0:
                        ReadPathFromFile(ofd.FileName, m_TargetPathWaypoints);
                        if (m_TargetPathWaypoints.Count > 0)
                            MapBox.Image = new Bitmap(Helper.ResizeImage(new Bitmap("MapImages\\" + m_TargetPathWaypoints[0].MapId.ToString() + ".jpg"), 200, 140));
                        break;
                    case 1:
                        ReadPathFromFile(ofd.FileName, m_RevivePathWaypoints);
                        if (m_RevivePathWaypoints.Count > 0)
                            MapBox.Image = new Bitmap(Helper.ResizeImage(new Bitmap("MapImages\\" + m_RevivePathWaypoints[0].MapId.ToString() + ".jpg"), 200, 140));
                        break;
                    case 2:
                        ReadPathFromFile(ofd.FileName, m_SellPathWaypoints);
                        if (m_SellPathWaypoints.Count > 0)
                            MapBox.Image = new Bitmap(Helper.ResizeImage(new Bitmap("MapImages\\" + m_SellPathWaypoints[0].MapId.ToString() + ".jpg"), 200, 140));
                        break;
                    case 3:
                        ReadPathFromFile(ofd.FileName, m_WalkPathWaypoints);
                        if (m_WalkPathWaypoints.Count > 0)
                            MapBox.Image = new Bitmap(Helper.ResizeImage(new Bitmap("MapImages\\" + m_WalkPathWaypoints[0].MapId.ToString() + ".jpg"), 200, 140));
                        break;
                }

                MapBox.Invalidate();
            }
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
                Api.PlayerData.Class > PlayerClassType.None && 
                Api.PlayerData.Class <= PlayerClassType.LastPlayerClass)
            {
                ClassTabs.SelectedIndex = (int)Api.PlayerData.Class - 1;
            }
        }

        private void StaleStealthNumericInput_ValueChanged(object sender, EventArgs e)
        {
            Automater.Rogue.StaleStealthTimer.Interval = (double)(StaleStealthNumericInput.Value * 1000);
        }

        private void RegisterDelayNumericInput_ValueChanged(object sender, EventArgs e)
        {
            Automater.RegisterDelay = (double)RegisterDelayNumericInput.Value;
        }

        private void SplitDistanceNumericInput_ValueChanged(object sender, EventArgs e)
        {
            RecordWowPath.SplitDistance = (double)SplitDistanceNumericInput.Value;
        }

        private void SliceNDiceCPNumericInput_ValueChanged(object sender, EventArgs e)
        {
            Automater.Rogue.SliceAndDice.MaximumComboPointsCost = (uint)SliceNDiceCPNumericInput.Value ;
            Automater.Rogue.SliceAndDice.MinimumComboPointsCost = (uint)SliceNDiceCPNumericInput.Value;
        }

        private void RuptureCPNumericInput_ValueChanged(object sender, EventArgs e)
        {
            Automater.Rogue.Rupture.MinimumComboPointsCost = (uint)RuptureCPNumericInput.Value;
        }

        private void EvisceratePercentageNumericInput_ValueChanged(object sender, EventArgs e)
        {
            Automater.Rogue.Eviscerate.TargetHealthPercentage = (uint)EvisceratePercentageNumericInput.Value;
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
            Automater.AutomaterStatusEvent -= AutomaterStatusEvent;

            Automater.Stop();
        }

        private void SkinLootCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.SkinLoot = SkinLootCheckbox.Checked;
        }

        private void ReversePathButton_Click(object sender, EventArgs e)
        {
            switch (PathTypeDropDown.SelectedIndex)
            {
                case 0:
                    m_TargetPathWaypoints.Reverse();
                    break;
                case 1:
                    m_RevivePathWaypoints.Reverse();
                    break;
                case 2:
                    m_SellPathWaypoints.Reverse();
                    break;
                case 3:
                    m_WalkPathWaypoints.Reverse();
                    break;
            }

            MapBox.Invalidate();
        }

        private void AlwaysThrowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.Rogue.ThrowFlag = AlwaysThrowCheckBox.Checked;
        }

        private void StealthCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.Rogue.StealthFlag = StealthCheckBox.Checked;
        }

        private void EvasionPercentaceNumericInput_ValueChanged(object sender, EventArgs e)
        {
            Automater.Rogue.Evasion.PlayerHealthPercentage = (int)EvasionPercentaceNumericInput.Value;
        }

        private void XReviveButtonLocationNumericInput_ValueChanged(object sender, EventArgs e)
        {
            Automater.XReviveButtonLocation = (double)XReviveButtonLocationNumericInput.Value * 100;
        }

        private void YReviveButtonLocationNumericInput_ValueChanged(object sender, EventArgs e)
        {
            Automater.YReviveButtonLocation = (double)YReviveButtonLocationNumericInput.Value * 100;
        }

        private void ReviveButtonLocationTestButton_Click(object sender, EventArgs e)
        {
            m_InputSimulator.Mouse.MoveMouseTo((double)XReviveButtonLocationNumericInput.Value * 100,
                                               (double)YReviveButtonLocationNumericInput.Value * 100);
        }

        private void PassiveHumanoidCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.Druid.Passive = PassiveHumanoidCheckBox.Checked;
        }

        private void RegenerateVitalsNumericInput_ValueChanged(object sender, EventArgs e)
        {
            Automater.RegenerateVitalsHealthPercentage = (double)RegenerateVitalsNumericInput.Value;
        }

        private void AutoLootLabelCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.AutoLoot = AutoLootLabelCheckbox.Checked;
        }

        private void RuptureFirstCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.Rogue.RuptureFirst = RuptureFirstCheckBox.Checked;
        }

        private void FindTargetRateNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WaypointFollower.Jitterizer.Rate = (double)FindTargetRateNumericInput.Value;
        }

        private void FindTargetWaitTimeNumericInput_ValueChanged(object sender, EventArgs e)
        {
            WaypointFollower.Jitterizer.WaitTime = (double)FindTargetWaitTimeNumericInput.Value;
        }

        private void FindTargetJumpCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            WaypointFollower.Jitterizer.Jump = FindTargetJumpCheckBox.Checked;
        }

        private void FindTargetLeftCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            WaypointFollower.Jitterizer.Left = FindTargetLeftCheckBox.Checked;
        }

        private void FindTargetRightCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            WaypointFollower.Jitterizer.Right = FindTargetRightCheckBox.Checked;
        }

        private void FindTargetLeftRightCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            WaypointFollower.Jitterizer.LeftRight = FindTargetLeftRightCheckBox.Checked;
        }

        private void FindTargetRightLeftCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            WaypointFollower.Jitterizer.RightLeft = FindTargetRightLeftCheckBox.Checked;
        }

        private void KillTargetRateNumericInput_ValueChanged(object sender, EventArgs e)
        {
            Automater.PlayerJitterizer.Rate = (double)KillTargetRateNumericInput.Value;
        }

        private void KillTargetWaitTimeNumericInput_ValueChanged(object sender, EventArgs e)
        {
            Automater.PlayerJitterizer.WaitTime = (double)KillTargetWaitTimeNumericInput.Value;
        }

        private void KillTargetJumpCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.PlayerJitterizer.Jump = KillTargetJumpCheckBox.Checked;
        }

        private void KillTargetUpDownCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.PlayerJitterizer.UpDown = KillTargetUpDownCheckBox.Checked;
        }

        private void KillTargetDownUpCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.PlayerJitterizer.DownUp = KillTargetDownUpCheckBox.Checked;
        }

        private void KillTargetLeftRightCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.PlayerJitterizer.LeftRight = KillTargetLeftRightCheckBox.Checked;
        }

        private void KillTargetRightLeftCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.PlayerJitterizer.RightLeft = KillTargetRightLeftCheckBox.Checked;
        }

        private void KillTargetClockwiseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.PlayerJitterizer.Clockwise = KillTargetClockwiseCheckBox.Checked;
        }

        private void KillTargetCounterClockwiseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.PlayerJitterizer.CounterClockwise = KillTargetCounterClockwiseCheckBox.Checked;
        }

        private void StealthForeverCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.Rogue.AlwaysStealth = StealthForeverCheckBox.Checked;
        }

        private void WebInterfaceToggleButton_Click(object sender, EventArgs e)
        {
            if(!m_WebInterfaceRunning)
            {
                m_WebInterface.Start();
                m_WebInterfaceRunning = true;
                WebInterfaceToggleButton.Text = "Stop web interface";
                WebInterfaceToggleButton.BackColor = System.Drawing.Color.FromArgb(255, 255, 128, 128);
            }
            else
            {
                m_WebInterface.Stop();
                m_WebInterfaceRunning = false;
                WebInterfaceToggleButton.Text = "Start web interface";
                WebInterfaceToggleButton.BackColor = System.Drawing.Color.FromArgb(255, 128, 255, 128);
            }
        }

        private void WhisperEmailCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_SendEmails = WhisperEmailCheckBox.Checked;
        }

        private void FirstSealCrusaderButton_CheckedChanged(object sender, EventArgs e)
        {
            Automater.Paladin.FirstSeal = FirstSealType.Crusader;
        }

        private void FirstSealNoneButton_CheckedChanged(object sender, EventArgs e)
        {
            Automater.Paladin.FirstSeal = FirstSealType.None;
        }

        private void FirstSealJusticeButton_CheckedChanged(object sender, EventArgs e)
        {
            Automater.Paladin.FirstSeal = FirstSealType.Justice;
        }

        private const int LOGIN_LENGTH = 34;


        private const int READ_BUFFER_LENGTH = 8192;
        private const int PORT = 8002;
        private const int SLEEP_TIME = 10; // ms

        private const int LOGIN_BYTE_1 = 31;
        private const int LOGIN_BYTE_2 = 41;
        private const byte RELAY_BYTE = 59;
        private const byte LOGIN_BYTE = 26;
        private const byte START_BYTE = 53;
        private const byte STOP_BYTE = 58;
        private const byte SCREEN_BYTE = 97;
        private const byte BAD_LOGIN_BYTE = 93;
        private const byte BOOP_BYTE = 23;


        private void ConnectToRemote(string host, string user)
        {
            byte[] loginBytes = new byte[LOGIN_LENGTH];

            loginBytes[0] = LOGIN_BYTE_1;
            loginBytes[1] = LOGIN_BYTE_2;

            Array.Copy(ASCIIEncoding.ASCII.GetBytes(user), 0, loginBytes, 2, user.Length);

            try
            {
                using (TcpClient tcpClient = new TcpClient(host, PORT))
                {
                    tcpClient.NoDelay = true;

                    using (NetworkStream ns = tcpClient.GetStream())
                    {
                        ns.Write(loginBytes, 0, loginBytes.Length);

                        byte[] dataBuffer = new byte[READ_BUFFER_LENGTH];

                        while (true)
                        {
                            while(tcpClient.Available == 0 && !m_StopRemote && tcpClient.Connected)
                            {
                                if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                                {
                                    byte[] buff = new byte[1];
                                    if (tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                                    {
                                        m_StopRemote = true;
                                    }
                                }

                                System.Threading.Thread.Sleep(10);
                            }

                            if (m_StopRemote || !tcpClient.Connected)
                                break;

                            byte command = (byte)ns.ReadByte();

                            switch (command)
                            {
                                case BAD_LOGIN_BYTE:
                                    {
                                        throw new Exception("User not found");
                                    }

                                case LOGIN_BYTE:
                                    {
                                        ns.WriteByte(LOGIN_BYTE);

                                        SetRemoteWebInterfaceButtonStyle(true);

                                        break;
                                    }

                                case START_BYTE:
                                    {
                                        ns.WriteByte(START_BYTE);

                                        Automater.RemoteStart();

                                        break;
                                    }

                                case STOP_BYTE:
                                    {
                                        ns.WriteByte(STOP_BYTE);

                                        Automater.RemoteStop();

                                        break;
                                    }

                                case SCREEN_BYTE:
                                    {
                                        ns.WriteByte(SCREEN_BYTE);

                                        ns.ReadByte();

                                        byte[] jpegBytes = GetScreenJpegBytes();

                                        int jpegSize = jpegBytes.Length;

                                        byte[] sizeBytes = BitConverter.GetBytes(jpegSize);

                                        ns.Write(sizeBytes, 0, sizeBytes.Length);

                                        ns.ReadByte();

                                        ns.Write(jpegBytes, 0, jpegBytes.Length);

                                        break;
                                    }

                                case RELAY_BYTE:
                                    {
                                        ns.WriteByte(RELAY_BYTE);

                                        int bread = ns.Read(dataBuffer, 0, dataBuffer.Length);
                                        byte[] dataRead = new byte[bread];
                                        Array.Copy(dataBuffer, dataRead, bread);

                                        string relayString = ASCIIEncoding.ASCII.GetString(dataRead);

                                        Automater.SetRelayString(relayString);

                                        break;
                                    }

                                case BOOP_BYTE:
                                    {
                                        ns.WriteByte(BOOP_BYTE);

                                        break;
                                    }

                                default:
                                    {
                                        throw new Exception("BAD COMMAND");
                                    }
                            }
                        }
                    }
                }
            }
            catch(Exception err)
            {
                SetRemoteWebInterfaceButtonStyle(false);
                m_RemoteConnected = false;
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            remotewait.Set();
        }

        private void SetRemoteWebInterfaceButtonStyle(bool connected)
        {
            if (InvokeRequired)
            {
                InvokeWithoutDisposedException(new MethodInvoker(() => { SetRemoteWebInterfaceButtonStyle(connected); }));
                return;
            }

            RemoteWebInterfaceButton.Enabled = true;

            if(connected)
            {
                RemoteWebInterfaceButton.Text = "Disconnect from remote web interface";
                RemoteWebInterfaceButton.BackColor = System.Drawing.Color.FromArgb(255, 255, 128, 128);
            }
            else
            {
                RemoteWebInterfaceButton.Text = "Connect to remote web interface";
                RemoteWebInterfaceButton.BackColor = System.Drawing.Color.FromArgb(255, 128, 255, 128);
            }

            
        }

        private byte[] GetScreenJpegBytes()
        {
            Rectangle bounds = new Rectangle(0, 0, 1920, 1080);
            byte[] jpegImage;

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(0, 0), Point.Empty, bounds.Size);
                }

                using (var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Jpeg);

                    jpegImage = memoryStream.ToArray();
                }
            }

            return jpegImage;
        }

        private bool m_RemoteConnected = false;
        private EventWaitHandle remotewait = new EventWaitHandle(false, EventResetMode.AutoReset);
        private volatile bool m_StopRemote = false;

        private void RemoteWebInterfaceButton_Click(object sender, EventArgs e)
        {
            if (RemoteServerTextBox.Text == string.Empty || RemoteUserTextBox.Text == string.Empty)
                return;

            RemoteWebInterfaceButton.Enabled = false;

            if(!m_RemoteConnected)
            {
                m_StopRemote = false;
                m_RemoteConnected = true;

                Task.Run(() =>
                {
                    try
                    {
                        ConnectToRemote(RemoteServerTextBox.Text, RemoteUserTextBox.Text);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }
            else
            {
                m_StopRemote = true;
                remotewait.WaitOne();
                SetRemoteWebInterfaceButtonStyle(false);
                m_RemoteConnected = false;
            }

        }

        private void MapBox_Click(object sender, EventArgs e)
        {
            PathViewer pv = null;

            switch (PathTypeDropDown.SelectedIndex)
            {
                case 0:
                    pv = new PathViewer(m_TargetPathWaypoints, "Target Path");
                    break;
                case 1:
                    pv = new PathViewer(m_RevivePathWaypoints, "Revive Path");
                    break;
                case 2:
                    pv = new PathViewer(m_SellPathWaypoints, "Shop Path");
                    break;
                case 3:
                    pv = new PathViewer(m_WalkPathWaypoints, "Walk Path");
                    break;
            }

            pv.Show();
        }

        private void MapBox_Paint(object sender, PaintEventArgs e)
        {
            switch (selectedIndex)
            {
                case 0:
                    DrawWaypoints(e, m_TargetPathWaypoints);
                    break;
                case 1:
                    DrawWaypoints(e, m_RevivePathWaypoints);
                    break;
                case 2:
                    DrawWaypoints(e, m_SellPathWaypoints);
                    break;
                case 3:
                    DrawWaypoints(e, m_WalkPathWaypoints);
                    break;
            }
        }

        Pen m_PathPen = new Pen(Brushes.Black, 1);
        Pen m_StartPen = new Pen(Brushes.LightGreen, 2);
        Pen m_EndPen = new Pen(Brushes.Crimson, 2);
        Pen m_PlayerPen = new Pen(Brushes.Cyan, 2);

        private void DrawWaypoints(PaintEventArgs e, List<Waypoint> waypoints)
        {
            if (waypoints.Count < 2)
                return;

            for (int i = 0; i < waypoints.Count; i++)
            {
                e.Graphics.DrawEllipse(m_PathPen, (float)(waypoints[i].X * 2), (float)(waypoints[i].Y * 1.4), 1f, 1f);
            }

            e.Graphics.DrawEllipse(m_StartPen, (float)(waypoints[0].X * 2), (float)(waypoints[0].Y * 1.4), 1f, 1f);
            e.Graphics.DrawEllipse(m_EndPen, (float)(waypoints[waypoints.Count - 1].X * 2), (float)(waypoints[waypoints.Count - 1].Y * 1.4), 1f, 1f);

            if (waypoints[0].MapId == Api.PlayerData.MapId)
                e.Graphics.DrawEllipse(m_PlayerPen, (float)(Api.PlayerData.PlayerXPosition * 2), (float)(Api.PlayerData.PlayerYPosition * 1.4), 1f, 1f);

        }

        private void ApplyPoisonCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.Rogue.ApplyPoison = ApplyPoisonCheckBox.Checked;
        }

        private void DontThrowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Automater.Rogue.DontThrow = DontThrowCheckBox.Checked;
        }

        private void ThrowAfterNumericInput_ValueChanged(object sender, EventArgs e)
        {
            Automater.Rogue.FriendlyTimer.Interval = (double)(ThrowAfterNumericInput.Value*1000);
        }
    }
}
