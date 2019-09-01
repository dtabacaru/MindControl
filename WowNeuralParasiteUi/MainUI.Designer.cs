namespace WoWNeuralParasiteUI
{
    partial class MainUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainUI));
            this.ModeDropDown = new System.Windows.Forms.ComboBox();
            this.OptionTabs = new System.Windows.Forms.TabControl();
            this.InfoTab = new System.Windows.Forms.TabPage();
            this.DataTextBox = new System.Windows.Forms.RichTextBox();
            this.PathsTab = new System.Windows.Forms.TabPage();
            this.SplitDistanceLabel = new System.Windows.Forms.Label();
            this.SplitDistanceNumericInput = new System.Windows.Forms.NumericUpDown();
            this.SaveFileButton = new System.Windows.Forms.Button();
            this.PathTypeDropDown = new System.Windows.Forms.ComboBox();
            this.RecordButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.LoadFileButton = new System.Windows.Forms.Button();
            this.YTextBox = new System.Windows.Forms.TextBox();
            this.YLabel = new System.Windows.Forms.Label();
            this.XTextBox = new System.Windows.Forms.TextBox();
            this.XLabel = new System.Windows.Forms.Label();
            this.Automater = new System.Windows.Forms.TabPage();
            this.ClosestPointDistanceLabel = new System.Windows.Forms.Label();
            this.ClosestPointDistanceNumericInput = new System.Windows.Forms.NumericUpDown();
            this.RegisterDelayLabel = new System.Windows.Forms.Label();
            this.RegisterDelayNumericInput = new System.Windows.Forms.NumericUpDown();
            this.ClassTabs = new System.Windows.Forms.TabControl();
            this.WarriorTab = new System.Windows.Forms.TabPage();
            this.PaladinTab = new System.Windows.Forms.TabPage();
            this.RogueTab = new System.Windows.Forms.TabPage();
            this.RuptureCPLabel = new System.Windows.Forms.Label();
            this.SliceNDiceCPLabel = new System.Windows.Forms.Label();
            this.EvisceratePercentageLabel = new System.Windows.Forms.Label();
            this.EvisceratePercentageNumericInput = new System.Windows.Forms.NumericUpDown();
            this.RuptureCPNumericInput = new System.Windows.Forms.NumericUpDown();
            this.SliceNDiceCPNumericInput = new System.Windows.Forms.NumericUpDown();
            this.StaleStealthNumericInput = new System.Windows.Forms.NumericUpDown();
            this.StaleStealthLabel = new System.Windows.Forms.Label();
            this.StealthlevelLabel = new System.Windows.Forms.Label();
            this.StealthLevelNumericInput = new System.Windows.Forms.NumericUpDown();
            this.PriestTab = new System.Windows.Forms.TabPage();
            this.MageTab = new System.Windows.Forms.TabPage();
            this.WarlockTab = new System.Windows.Forms.TabPage();
            this.HunterTab = new System.Windows.Forms.TabPage();
            this.ShamanTab = new System.Windows.Forms.TabPage();
            this.DruidTab = new System.Windows.Forms.TabPage();
            this.PositionToleranceNumericInput = new System.Windows.Forms.NumericUpDown();
            this.PositionToleranceLabel = new System.Windows.Forms.Label();
            this.TurnToleranceNumericInput = new System.Windows.Forms.NumericUpDown();
            this.TurnToleranceLabel = new System.Windows.Forms.Label();
            this.ShowInfoButton = new System.Windows.Forms.Button();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.OptionTabs.SuspendLayout();
            this.InfoTab.SuspendLayout();
            this.PathsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitDistanceNumericInput)).BeginInit();
            this.Automater.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ClosestPointDistanceNumericInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterDelayNumericInput)).BeginInit();
            this.ClassTabs.SuspendLayout();
            this.RogueTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EvisceratePercentageNumericInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RuptureCPNumericInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SliceNDiceCPNumericInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StaleStealthNumericInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StealthLevelNumericInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionToleranceNumericInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TurnToleranceNumericInput)).BeginInit();
            this.SuspendLayout();
            // 
            // ModeDropDown
            // 
            this.ModeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModeDropDown.FormattingEnabled = true;
            this.ModeDropDown.Items.AddRange(new object[] {
            "Find and kill",
            "Attack only"});
            this.ModeDropDown.Location = new System.Drawing.Point(0, 0);
            this.ModeDropDown.Name = "ModeDropDown";
            this.ModeDropDown.Size = new System.Drawing.Size(220, 21);
            this.ModeDropDown.TabIndex = 34;
            this.ModeDropDown.SelectedIndexChanged += new System.EventHandler(this.ModeDropDown_SelectedIndexChanged);
            // 
            // OptionTabs
            // 
            this.OptionTabs.Controls.Add(this.InfoTab);
            this.OptionTabs.Controls.Add(this.PathsTab);
            this.OptionTabs.Controls.Add(this.Automater);
            this.OptionTabs.Location = new System.Drawing.Point(0, 63);
            this.OptionTabs.Name = "OptionTabs";
            this.OptionTabs.SelectedIndex = 0;
            this.OptionTabs.Size = new System.Drawing.Size(222, 565);
            this.OptionTabs.TabIndex = 37;
            this.OptionTabs.Visible = false;
            this.OptionTabs.SelectedIndexChanged += new System.EventHandler(this.OptionTabs_SelectedIndexChanged);
            // 
            // InfoTab
            // 
            this.InfoTab.Controls.Add(this.DataTextBox);
            this.InfoTab.Location = new System.Drawing.Point(4, 22);
            this.InfoTab.Name = "InfoTab";
            this.InfoTab.Padding = new System.Windows.Forms.Padding(3);
            this.InfoTab.Size = new System.Drawing.Size(214, 539);
            this.InfoTab.TabIndex = 0;
            this.InfoTab.Text = "Info";
            this.InfoTab.UseVisualStyleBackColor = true;
            // 
            // DataTextBox
            // 
            this.DataTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DataTextBox.Location = new System.Drawing.Point(7, 7);
            this.DataTextBox.Name = "DataTextBox";
            this.DataTextBox.ReadOnly = true;
            this.DataTextBox.Size = new System.Drawing.Size(197, 526);
            this.DataTextBox.TabIndex = 0;
            this.DataTextBox.Text = "";
            // 
            // PathsTab
            // 
            this.PathsTab.Controls.Add(this.SplitDistanceLabel);
            this.PathsTab.Controls.Add(this.SplitDistanceNumericInput);
            this.PathsTab.Controls.Add(this.SaveFileButton);
            this.PathsTab.Controls.Add(this.PathTypeDropDown);
            this.PathsTab.Controls.Add(this.RecordButton);
            this.PathsTab.Controls.Add(this.OKButton);
            this.PathsTab.Controls.Add(this.LoadFileButton);
            this.PathsTab.Controls.Add(this.YTextBox);
            this.PathsTab.Controls.Add(this.YLabel);
            this.PathsTab.Controls.Add(this.XTextBox);
            this.PathsTab.Controls.Add(this.XLabel);
            this.PathsTab.Location = new System.Drawing.Point(4, 22);
            this.PathsTab.Name = "PathsTab";
            this.PathsTab.Padding = new System.Windows.Forms.Padding(3);
            this.PathsTab.Size = new System.Drawing.Size(214, 539);
            this.PathsTab.TabIndex = 1;
            this.PathsTab.Text = "Paths";
            this.PathsTab.UseVisualStyleBackColor = true;
            // 
            // SplitDistanceLabel
            // 
            this.SplitDistanceLabel.AutoSize = true;
            this.SplitDistanceLabel.Location = new System.Drawing.Point(8, 424);
            this.SplitDistanceLabel.Name = "SplitDistanceLabel";
            this.SplitDistanceLabel.Size = new System.Drawing.Size(70, 13);
            this.SplitDistanceLabel.TabIndex = 42;
            this.SplitDistanceLabel.Text = "Split distance";
            // 
            // SplitDistanceNumericInput
            // 
            this.SplitDistanceNumericInput.DecimalPlaces = 2;
            this.SplitDistanceNumericInput.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.SplitDistanceNumericInput.Location = new System.Drawing.Point(130, 422);
            this.SplitDistanceNumericInput.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.SplitDistanceNumericInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.SplitDistanceNumericInput.Name = "SplitDistanceNumericInput";
            this.SplitDistanceNumericInput.Size = new System.Drawing.Size(76, 20);
            this.SplitDistanceNumericInput.TabIndex = 41;
            this.SplitDistanceNumericInput.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.SplitDistanceNumericInput.ValueChanged += new System.EventHandler(this.SplitDistanceNumericInput_ValueChanged);
            // 
            // SaveFileButton
            // 
            this.SaveFileButton.Location = new System.Drawing.Point(6, 370);
            this.SaveFileButton.Name = "SaveFileButton";
            this.SaveFileButton.Size = new System.Drawing.Size(200, 25);
            this.SaveFileButton.TabIndex = 40;
            this.SaveFileButton.Text = "Save Path To File";
            this.SaveFileButton.UseVisualStyleBackColor = true;
            this.SaveFileButton.Click += new System.EventHandler(this.SaveFileButton_Click);
            // 
            // PathTypeDropDown
            // 
            this.PathTypeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PathTypeDropDown.FormattingEnabled = true;
            this.PathTypeDropDown.Items.AddRange(new object[] {
            "Target",
            "Revive",
            "Sell"});
            this.PathTypeDropDown.Location = new System.Drawing.Point(6, 6);
            this.PathTypeDropDown.Name = "PathTypeDropDown";
            this.PathTypeDropDown.Size = new System.Drawing.Size(200, 21);
            this.PathTypeDropDown.TabIndex = 39;
            this.PathTypeDropDown.SelectedIndexChanged += new System.EventHandler(this.PathTypeDropDown_SelectedIndexChanged);
            // 
            // RecordButton
            // 
            this.RecordButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.RecordButton.Location = new System.Drawing.Point(6, 448);
            this.RecordButton.Name = "RecordButton";
            this.RecordButton.Size = new System.Drawing.Size(200, 25);
            this.RecordButton.TabIndex = 38;
            this.RecordButton.Text = "Record Path";
            this.RecordButton.UseVisualStyleBackColor = false;
            this.RecordButton.Click += new System.EventHandler(this.RecordButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OKButton.Location = new System.Drawing.Point(6, 496);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(200, 37);
            this.OKButton.TabIndex = 37;
            this.OKButton.Text = "Apply Path";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // LoadFileButton
            // 
            this.LoadFileButton.Location = new System.Drawing.Point(6, 339);
            this.LoadFileButton.Name = "LoadFileButton";
            this.LoadFileButton.Size = new System.Drawing.Size(200, 25);
            this.LoadFileButton.TabIndex = 36;
            this.LoadFileButton.Text = "Load Path From File";
            this.LoadFileButton.UseVisualStyleBackColor = true;
            this.LoadFileButton.Click += new System.EventHandler(this.LoadFileButton_Click);
            // 
            // YTextBox
            // 
            this.YTextBox.Location = new System.Drawing.Point(6, 191);
            this.YTextBox.Multiline = true;
            this.YTextBox.Name = "YTextBox";
            this.YTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.YTextBox.Size = new System.Drawing.Size(200, 125);
            this.YTextBox.TabIndex = 35;
            // 
            // YLabel
            // 
            this.YLabel.AutoSize = true;
            this.YLabel.Location = new System.Drawing.Point(6, 175);
            this.YLabel.Name = "YLabel";
            this.YLabel.Size = new System.Drawing.Size(14, 13);
            this.YLabel.TabIndex = 34;
            this.YLabel.Text = "Y";
            // 
            // XTextBox
            // 
            this.XTextBox.Location = new System.Drawing.Point(6, 46);
            this.XTextBox.Multiline = true;
            this.XTextBox.Name = "XTextBox";
            this.XTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.XTextBox.Size = new System.Drawing.Size(200, 125);
            this.XTextBox.TabIndex = 33;
            // 
            // XLabel
            // 
            this.XLabel.AutoSize = true;
            this.XLabel.Location = new System.Drawing.Point(6, 30);
            this.XLabel.Name = "XLabel";
            this.XLabel.Size = new System.Drawing.Size(14, 13);
            this.XLabel.TabIndex = 32;
            this.XLabel.Text = "X";
            // 
            // Automater
            // 
            this.Automater.Controls.Add(this.ClosestPointDistanceLabel);
            this.Automater.Controls.Add(this.ClosestPointDistanceNumericInput);
            this.Automater.Controls.Add(this.RegisterDelayLabel);
            this.Automater.Controls.Add(this.RegisterDelayNumericInput);
            this.Automater.Controls.Add(this.ClassTabs);
            this.Automater.Controls.Add(this.PositionToleranceNumericInput);
            this.Automater.Controls.Add(this.PositionToleranceLabel);
            this.Automater.Controls.Add(this.TurnToleranceNumericInput);
            this.Automater.Controls.Add(this.TurnToleranceLabel);
            this.Automater.Location = new System.Drawing.Point(4, 22);
            this.Automater.Name = "Automater";
            this.Automater.Size = new System.Drawing.Size(214, 539);
            this.Automater.TabIndex = 2;
            this.Automater.Text = "Automater";
            this.Automater.UseVisualStyleBackColor = true;
            // 
            // ClosestPointDistanceLabel
            // 
            this.ClosestPointDistanceLabel.AutoSize = true;
            this.ClosestPointDistanceLabel.Location = new System.Drawing.Point(3, 87);
            this.ClosestPointDistanceLabel.Name = "ClosestPointDistanceLabel";
            this.ClosestPointDistanceLabel.Size = new System.Drawing.Size(150, 13);
            this.ClosestPointDistanceLabel.TabIndex = 10;
            this.ClosestPointDistanceLabel.Text = "Recompute waypoint distance";
            // 
            // ClosestPointDistanceNumericInput
            // 
            this.ClosestPointDistanceNumericInput.DecimalPlaces = 2;
            this.ClosestPointDistanceNumericInput.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ClosestPointDistanceNumericInput.Location = new System.Drawing.Point(154, 85);
            this.ClosestPointDistanceNumericInput.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.ClosestPointDistanceNumericInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ClosestPointDistanceNumericInput.Name = "ClosestPointDistanceNumericInput";
            this.ClosestPointDistanceNumericInput.Size = new System.Drawing.Size(54, 20);
            this.ClosestPointDistanceNumericInput.TabIndex = 9;
            this.ClosestPointDistanceNumericInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ClosestPointDistanceNumericInput.ValueChanged += new System.EventHandler(this.ClosestPointDistanceNumericInput_ValueChanged);
            // 
            // RegisterDelayLabel
            // 
            this.RegisterDelayLabel.AutoSize = true;
            this.RegisterDelayLabel.Location = new System.Drawing.Point(3, 61);
            this.RegisterDelayLabel.Name = "RegisterDelayLabel";
            this.RegisterDelayLabel.Size = new System.Drawing.Size(100, 13);
            this.RegisterDelayLabel.TabIndex = 8;
            this.RegisterDelayLabel.Text = "Register delay (sec)";
            // 
            // RegisterDelayNumericInput
            // 
            this.RegisterDelayNumericInput.DecimalPlaces = 2;
            this.RegisterDelayNumericInput.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.RegisterDelayNumericInput.Location = new System.Drawing.Point(154, 59);
            this.RegisterDelayNumericInput.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.RegisterDelayNumericInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.RegisterDelayNumericInput.Name = "RegisterDelayNumericInput";
            this.RegisterDelayNumericInput.Size = new System.Drawing.Size(54, 20);
            this.RegisterDelayNumericInput.TabIndex = 7;
            this.RegisterDelayNumericInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.RegisterDelayNumericInput.ValueChanged += new System.EventHandler(this.RegisterDelayNumericInput_ValueChanged);
            // 
            // ClassTabs
            // 
            this.ClassTabs.Controls.Add(this.WarriorTab);
            this.ClassTabs.Controls.Add(this.PaladinTab);
            this.ClassTabs.Controls.Add(this.RogueTab);
            this.ClassTabs.Controls.Add(this.PriestTab);
            this.ClassTabs.Controls.Add(this.MageTab);
            this.ClassTabs.Controls.Add(this.WarlockTab);
            this.ClassTabs.Controls.Add(this.HunterTab);
            this.ClassTabs.Controls.Add(this.ShamanTab);
            this.ClassTabs.Controls.Add(this.DruidTab);
            this.ClassTabs.Location = new System.Drawing.Point(0, 156);
            this.ClassTabs.Name = "ClassTabs";
            this.ClassTabs.SelectedIndex = 0;
            this.ClassTabs.Size = new System.Drawing.Size(214, 380);
            this.ClassTabs.TabIndex = 6;
            // 
            // WarriorTab
            // 
            this.WarriorTab.Location = new System.Drawing.Point(4, 22);
            this.WarriorTab.Name = "WarriorTab";
            this.WarriorTab.Padding = new System.Windows.Forms.Padding(3);
            this.WarriorTab.Size = new System.Drawing.Size(206, 354);
            this.WarriorTab.TabIndex = 0;
            this.WarriorTab.Text = "Warrior";
            this.WarriorTab.UseVisualStyleBackColor = true;
            // 
            // PaladinTab
            // 
            this.PaladinTab.Location = new System.Drawing.Point(4, 22);
            this.PaladinTab.Name = "PaladinTab";
            this.PaladinTab.Padding = new System.Windows.Forms.Padding(3);
            this.PaladinTab.Size = new System.Drawing.Size(206, 354);
            this.PaladinTab.TabIndex = 1;
            this.PaladinTab.Text = "Paladin";
            this.PaladinTab.UseVisualStyleBackColor = true;
            // 
            // RogueTab
            // 
            this.RogueTab.Controls.Add(this.RuptureCPLabel);
            this.RogueTab.Controls.Add(this.SliceNDiceCPLabel);
            this.RogueTab.Controls.Add(this.EvisceratePercentageLabel);
            this.RogueTab.Controls.Add(this.EvisceratePercentageNumericInput);
            this.RogueTab.Controls.Add(this.RuptureCPNumericInput);
            this.RogueTab.Controls.Add(this.SliceNDiceCPNumericInput);
            this.RogueTab.Controls.Add(this.StaleStealthNumericInput);
            this.RogueTab.Controls.Add(this.StaleStealthLabel);
            this.RogueTab.Controls.Add(this.StealthlevelLabel);
            this.RogueTab.Controls.Add(this.StealthLevelNumericInput);
            this.RogueTab.Location = new System.Drawing.Point(4, 22);
            this.RogueTab.Name = "RogueTab";
            this.RogueTab.Size = new System.Drawing.Size(206, 354);
            this.RogueTab.TabIndex = 2;
            this.RogueTab.Text = "Rogue";
            this.RogueTab.UseVisualStyleBackColor = true;
            // 
            // RuptureCPLabel
            // 
            this.RuptureCPLabel.AutoSize = true;
            this.RuptureCPLabel.Location = new System.Drawing.Point(4, 88);
            this.RuptureCPLabel.Name = "RuptureCPLabel";
            this.RuptureCPLabel.Size = new System.Drawing.Size(111, 13);
            this.RuptureCPLabel.TabIndex = 9;
            this.RuptureCPLabel.Text = "Rupture combo points";
            // 
            // SliceNDiceCPLabel
            // 
            this.SliceNDiceCPLabel.AutoSize = true;
            this.SliceNDiceCPLabel.Location = new System.Drawing.Point(4, 62);
            this.SliceNDiceCPLabel.Name = "SliceNDiceCPLabel";
            this.SliceNDiceCPLabel.Size = new System.Drawing.Size(140, 13);
            this.SliceNDiceCPLabel.TabIndex = 8;
            this.SliceNDiceCPLabel.Text = "Slice and dice combo points";
            // 
            // EvisceratePercentageLabel
            // 
            this.EvisceratePercentageLabel.AutoSize = true;
            this.EvisceratePercentageLabel.Location = new System.Drawing.Point(4, 114);
            this.EvisceratePercentageLabel.Name = "EvisceratePercentageLabel";
            this.EvisceratePercentageLabel.Size = new System.Drawing.Size(114, 13);
            this.EvisceratePercentageLabel.TabIndex = 7;
            this.EvisceratePercentageLabel.Text = "Eviscerate percentage";
            // 
            // EvisceratePercentageNumericInput
            // 
            this.EvisceratePercentageNumericInput.Location = new System.Drawing.Point(146, 112);
            this.EvisceratePercentageNumericInput.Name = "EvisceratePercentageNumericInput";
            this.EvisceratePercentageNumericInput.Size = new System.Drawing.Size(54, 20);
            this.EvisceratePercentageNumericInput.TabIndex = 6;
            this.EvisceratePercentageNumericInput.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.EvisceratePercentageNumericInput.ValueChanged += new System.EventHandler(this.EvisceratePercentageNumericInput_ValueChanged);
            // 
            // RuptureCPNumericInput
            // 
            this.RuptureCPNumericInput.Location = new System.Drawing.Point(146, 86);
            this.RuptureCPNumericInput.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.RuptureCPNumericInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.RuptureCPNumericInput.Name = "RuptureCPNumericInput";
            this.RuptureCPNumericInput.Size = new System.Drawing.Size(54, 20);
            this.RuptureCPNumericInput.TabIndex = 5;
            this.RuptureCPNumericInput.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.RuptureCPNumericInput.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // SliceNDiceCPNumericInput
            // 
            this.SliceNDiceCPNumericInput.Location = new System.Drawing.Point(146, 60);
            this.SliceNDiceCPNumericInput.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.SliceNDiceCPNumericInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SliceNDiceCPNumericInput.Name = "SliceNDiceCPNumericInput";
            this.SliceNDiceCPNumericInput.Size = new System.Drawing.Size(54, 20);
            this.SliceNDiceCPNumericInput.TabIndex = 4;
            this.SliceNDiceCPNumericInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SliceNDiceCPNumericInput.ValueChanged += new System.EventHandler(this.SliceNDiceCPNumericInput_ValueChanged);
            // 
            // StaleStealthNumericInput
            // 
            this.StaleStealthNumericInput.Location = new System.Drawing.Point(146, 34);
            this.StaleStealthNumericInput.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.StaleStealthNumericInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.StaleStealthNumericInput.Name = "StaleStealthNumericInput";
            this.StaleStealthNumericInput.Size = new System.Drawing.Size(54, 20);
            this.StaleStealthNumericInput.TabIndex = 3;
            this.StaleStealthNumericInput.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.StaleStealthNumericInput.ValueChanged += new System.EventHandler(this.StaleStealthNumericInput_ValueChanged);
            // 
            // StaleStealthLabel
            // 
            this.StaleStealthLabel.AutoSize = true;
            this.StaleStealthLabel.Location = new System.Drawing.Point(4, 36);
            this.StaleStealthLabel.Name = "StaleStealthLabel";
            this.StaleStealthLabel.Size = new System.Drawing.Size(131, 13);
            this.StaleStealthLabel.TabIndex = 2;
            this.StaleStealthLabel.Text = "Remove stealth after (sec)";
            // 
            // StealthlevelLabel
            // 
            this.StealthlevelLabel.AutoSize = true;
            this.StealthlevelLabel.Location = new System.Drawing.Point(4, 10);
            this.StealthlevelLabel.Name = "StealthlevelLabel";
            this.StealthlevelLabel.Size = new System.Drawing.Size(85, 13);
            this.StealthlevelLabel.TabIndex = 1;
            this.StealthlevelLabel.Text = "Stealth find level";
            // 
            // StealthLevelNumericInput
            // 
            this.StealthLevelNumericInput.Location = new System.Drawing.Point(146, 8);
            this.StealthLevelNumericInput.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.StealthLevelNumericInput.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.StealthLevelNumericInput.Name = "StealthLevelNumericInput";
            this.StealthLevelNumericInput.Size = new System.Drawing.Size(54, 20);
            this.StealthLevelNumericInput.TabIndex = 0;
            this.StealthLevelNumericInput.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.StealthLevelNumericInput.ValueChanged += new System.EventHandler(this.StealthlevelNumericInput_ValueChanged);
            // 
            // PriestTab
            // 
            this.PriestTab.Location = new System.Drawing.Point(4, 22);
            this.PriestTab.Name = "PriestTab";
            this.PriestTab.Size = new System.Drawing.Size(206, 354);
            this.PriestTab.TabIndex = 3;
            this.PriestTab.Text = "Priest";
            this.PriestTab.UseVisualStyleBackColor = true;
            // 
            // MageTab
            // 
            this.MageTab.Location = new System.Drawing.Point(4, 22);
            this.MageTab.Name = "MageTab";
            this.MageTab.Size = new System.Drawing.Size(206, 354);
            this.MageTab.TabIndex = 4;
            this.MageTab.Text = "Mage";
            this.MageTab.UseVisualStyleBackColor = true;
            // 
            // WarlockTab
            // 
            this.WarlockTab.Location = new System.Drawing.Point(4, 22);
            this.WarlockTab.Name = "WarlockTab";
            this.WarlockTab.Size = new System.Drawing.Size(206, 354);
            this.WarlockTab.TabIndex = 5;
            this.WarlockTab.Text = "Warlock";
            this.WarlockTab.UseVisualStyleBackColor = true;
            // 
            // HunterTab
            // 
            this.HunterTab.Location = new System.Drawing.Point(4, 22);
            this.HunterTab.Name = "HunterTab";
            this.HunterTab.Size = new System.Drawing.Size(206, 354);
            this.HunterTab.TabIndex = 6;
            this.HunterTab.Text = "Hunter";
            this.HunterTab.UseVisualStyleBackColor = true;
            // 
            // ShamanTab
            // 
            this.ShamanTab.Location = new System.Drawing.Point(4, 22);
            this.ShamanTab.Name = "ShamanTab";
            this.ShamanTab.Size = new System.Drawing.Size(206, 354);
            this.ShamanTab.TabIndex = 7;
            this.ShamanTab.Text = "Shaman";
            this.ShamanTab.UseVisualStyleBackColor = true;
            // 
            // DruidTab
            // 
            this.DruidTab.Location = new System.Drawing.Point(4, 22);
            this.DruidTab.Name = "DruidTab";
            this.DruidTab.Size = new System.Drawing.Size(206, 354);
            this.DruidTab.TabIndex = 8;
            this.DruidTab.Text = "Druid";
            this.DruidTab.UseVisualStyleBackColor = true;
            // 
            // PositionToleranceNumericInput
            // 
            this.PositionToleranceNumericInput.DecimalPlaces = 2;
            this.PositionToleranceNumericInput.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.PositionToleranceNumericInput.Location = new System.Drawing.Point(154, 33);
            this.PositionToleranceNumericInput.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.PositionToleranceNumericInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.PositionToleranceNumericInput.Name = "PositionToleranceNumericInput";
            this.PositionToleranceNumericInput.Size = new System.Drawing.Size(54, 20);
            this.PositionToleranceNumericInput.TabIndex = 5;
            this.PositionToleranceNumericInput.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // PositionToleranceLabel
            // 
            this.PositionToleranceLabel.AutoSize = true;
            this.PositionToleranceLabel.Location = new System.Drawing.Point(3, 35);
            this.PositionToleranceLabel.Name = "PositionToleranceLabel";
            this.PositionToleranceLabel.Size = new System.Drawing.Size(91, 13);
            this.PositionToleranceLabel.TabIndex = 4;
            this.PositionToleranceLabel.Text = "Position tolerance";
            // 
            // TurnToleranceNumericInput
            // 
            this.TurnToleranceNumericInput.DecimalPlaces = 2;
            this.TurnToleranceNumericInput.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.TurnToleranceNumericInput.Location = new System.Drawing.Point(154, 7);
            this.TurnToleranceNumericInput.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.TurnToleranceNumericInput.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.TurnToleranceNumericInput.Name = "TurnToleranceNumericInput";
            this.TurnToleranceNumericInput.Size = new System.Drawing.Size(54, 20);
            this.TurnToleranceNumericInput.TabIndex = 3;
            this.TurnToleranceNumericInput.Value = new decimal(new int[] {
            7,
            0,
            0,
            131072});
            this.TurnToleranceNumericInput.ValueChanged += new System.EventHandler(this.TurnToleranceNumericInput_ValueChanged);
            // 
            // TurnToleranceLabel
            // 
            this.TurnToleranceLabel.AutoSize = true;
            this.TurnToleranceLabel.Location = new System.Drawing.Point(3, 9);
            this.TurnToleranceLabel.Name = "TurnToleranceLabel";
            this.TurnToleranceLabel.Size = new System.Drawing.Size(100, 13);
            this.TurnToleranceLabel.TabIndex = 2;
            this.TurnToleranceLabel.Text = "Turn tolerance (rad)";
            // 
            // ShowInfoButton
            // 
            this.ShowInfoButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ShowInfoButton.BackgroundImage = global::WoWNeuralParasiteUI.Properties.Resources.show;
            this.ShowInfoButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ShowInfoButton.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ButtonFace;
            this.ShowInfoButton.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ButtonFace;
            this.ShowInfoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ShowInfoButton.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ShowInfoButton.Location = new System.Drawing.Point(188, 26);
            this.ShowInfoButton.Name = "ShowInfoButton";
            this.ShowInfoButton.Size = new System.Drawing.Size(30, 30);
            this.ShowInfoButton.TabIndex = 36;
            this.ShowInfoButton.Text = "V";
            this.ShowInfoButton.UseVisualStyleBackColor = false;
            this.ShowInfoButton.Click += new System.EventHandler(this.ShowInfoButton_Click);
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLabel.Location = new System.Drawing.Point(3, 30);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 24);
            this.StatusLabel.TabIndex = 38;
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 61);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.OptionTabs);
            this.Controls.Add(this.ShowInfoButton);
            this.Controls.Add(this.ModeDropDown);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "MainUI";
            this.Text = "ClassicWowNeuralParasite";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainUI_FormClosing);
            this.OptionTabs.ResumeLayout(false);
            this.InfoTab.ResumeLayout(false);
            this.PathsTab.ResumeLayout(false);
            this.PathsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitDistanceNumericInput)).EndInit();
            this.Automater.ResumeLayout(false);
            this.Automater.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ClosestPointDistanceNumericInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterDelayNumericInput)).EndInit();
            this.ClassTabs.ResumeLayout(false);
            this.RogueTab.ResumeLayout(false);
            this.RogueTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EvisceratePercentageNumericInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RuptureCPNumericInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SliceNDiceCPNumericInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StaleStealthNumericInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StealthLevelNumericInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionToleranceNumericInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TurnToleranceNumericInput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox ModeDropDown;
        private System.Windows.Forms.Button ShowInfoButton;
        private System.Windows.Forms.TabControl OptionTabs;
        private System.Windows.Forms.TabPage InfoTab;
        private System.Windows.Forms.TabPage PathsTab;
        private System.Windows.Forms.TabPage Automater;
        private System.Windows.Forms.Button SaveFileButton;
        private System.Windows.Forms.ComboBox PathTypeDropDown;
        private System.Windows.Forms.Button RecordButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button LoadFileButton;
        private System.Windows.Forms.TextBox YTextBox;
        private System.Windows.Forms.Label YLabel;
        private System.Windows.Forms.TextBox XTextBox;
        private System.Windows.Forms.Label XLabel;
        private System.Windows.Forms.Label StealthlevelLabel;
        private System.Windows.Forms.NumericUpDown StealthLevelNumericInput;
        private System.Windows.Forms.NumericUpDown TurnToleranceNumericInput;
        private System.Windows.Forms.Label TurnToleranceLabel;
        private System.Windows.Forms.NumericUpDown PositionToleranceNumericInput;
        private System.Windows.Forms.Label PositionToleranceLabel;
        private System.Windows.Forms.TabControl ClassTabs;
        private System.Windows.Forms.TabPage WarriorTab;
        private System.Windows.Forms.TabPage PaladinTab;
        private System.Windows.Forms.TabPage RogueTab;
        private System.Windows.Forms.TabPage PriestTab;
        private System.Windows.Forms.TabPage MageTab;
        private System.Windows.Forms.TabPage WarlockTab;
        private System.Windows.Forms.TabPage HunterTab;
        private System.Windows.Forms.TabPage ShamanTab;
        private System.Windows.Forms.TabPage DruidTab;
        private System.Windows.Forms.NumericUpDown StaleStealthNumericInput;
        private System.Windows.Forms.Label StaleStealthLabel;
        private System.Windows.Forms.Label RegisterDelayLabel;
        private System.Windows.Forms.NumericUpDown RegisterDelayNumericInput;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Label SplitDistanceLabel;
        private System.Windows.Forms.NumericUpDown SplitDistanceNumericInput;
        private System.Windows.Forms.NumericUpDown EvisceratePercentageNumericInput;
        private System.Windows.Forms.NumericUpDown RuptureCPNumericInput;
        private System.Windows.Forms.NumericUpDown SliceNDiceCPNumericInput;
        private System.Windows.Forms.Label RuptureCPLabel;
        private System.Windows.Forms.Label SliceNDiceCPLabel;
        private System.Windows.Forms.Label EvisceratePercentageLabel;
        private System.Windows.Forms.Label ClosestPointDistanceLabel;
        private System.Windows.Forms.NumericUpDown ClosestPointDistanceNumericInput;
        private System.Windows.Forms.RichTextBox DataTextBox;
    }
}

