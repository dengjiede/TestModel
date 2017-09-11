namespace ClipPlane
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.axRenderControl1 = new Gvitech.CityMaker.Controls.AxRenderControl();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripInteractModeSetting = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripClipModeSetting = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLineColorComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbClipPlaneEnable = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCreateClipPlaneOperation = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.textBoxBoxCenterX = new System.Windows.Forms.TextBox();
            this.textBoxBoxCenterY = new System.Windows.Forms.TextBox();
            this.textBoxBoxCenterZ = new System.Windows.Forms.TextBox();
            this.textBoxBoxSizeX = new System.Windows.Forms.TextBox();
            this.textBoxBoxSizeY = new System.Windows.Forms.TextBox();
            this.textBoxBoxSizeZ = new System.Windows.Forms.TextBox();
            this.textBoxAngleHeading = new System.Windows.Forms.TextBox();
            this.textBoxAngleRoll = new System.Windows.Forms.TextBox();
            this.textBoxAngleTilt = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxPositionX = new System.Windows.Forms.TextBox();
            this.textBoxPositionY = new System.Windows.Forms.TextBox();
            this.textBoxPositionZ = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.axRenderControl1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // axRenderControl1
            // 
            this.axRenderControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axRenderControl1.Enabled = true;
            this.axRenderControl1.Location = new System.Drawing.Point(108, 33);
            this.axRenderControl1.Name = "axRenderControl1";
            this.axRenderControl1.Size = new System.Drawing.Size(510, 550);
            this.axRenderControl1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripInteractModeSetting,
            this.toolStripLabel3,
            this.toolStripClipModeSetting,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.toolStripLineColorComboBox});
            this.toolStrip1.Location = new System.Drawing.Point(105, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(516, 29);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(92, 26);
            this.toolStripLabel1.Text = "交互模式设置：";
            // 
            // toolStripInteractModeSetting
            // 
            this.toolStripInteractModeSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripInteractModeSetting.Items.AddRange(new object[] {
            "裁剪面交互模式",
            "鼠标拾取模式",
            "普通漫游模式"});
            this.toolStripInteractModeSetting.Name = "toolStripInteractModeSetting";
            this.toolStripInteractModeSetting.Size = new System.Drawing.Size(121, 29);
            this.toolStripInteractModeSetting.SelectedIndexChanged += new System.EventHandler(this.toolStripInteractModeSetting_SelectedIndexChanged);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(80, 26);
            this.toolStripLabel3.Text = "裁剪面模式：";
            // 
            // toolStripClipModeSetting
            // 
            this.toolStripClipModeSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripClipModeSetting.Items.AddRange(new object[] {
            "任意面",
            "长方体"});
            this.toolStripClipModeSetting.Name = "toolStripClipModeSetting";
            this.toolStripClipModeSetting.Size = new System.Drawing.Size(100, 29);
            this.toolStripClipModeSetting.SelectedIndexChanged += new System.EventHandler(this.toolStripClipModeSetting_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 29);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(68, 26);
            this.toolStripLabel2.Text = "交线颜色：";
            // 
            // toolStripLineColorComboBox
            // 
            this.toolStripLineColorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripLineColorComboBox.Items.AddRange(new object[] {
            "白色",
            "红色",
            "黄色",
            "蓝色"});
            this.toolStripLineColorComboBox.Name = "toolStripLineColorComboBox";
            this.toolStripLineColorComboBox.Size = new System.Drawing.Size(75, 25);
            this.toolStripLineColorComboBox.SelectedIndexChanged += new System.EventHandler(this.toolStripLineColorComboBox_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.axRenderControl1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(821, 586);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // cbClipPlaneEnable
            // 
            this.cbClipPlaneEnable.AutoSize = true;
            this.cbClipPlaneEnable.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbClipPlaneEnable.Location = new System.Drawing.Point(6, 101);
            this.cbClipPlaneEnable.Name = "cbClipPlaneEnable";
            this.cbClipPlaneEnable.Size = new System.Drawing.Size(99, 21);
            this.cbClipPlaneEnable.TabIndex = 2;
            this.cbClipPlaneEnable.Text = "是否参与裁剪";
            this.cbClipPlaneEnable.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbClipPlaneEnable);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 30);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(105, 556);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 89);
            this.label1.TabIndex = 3;
            this.label1.Text = "“交互模式设置”设置为“鼠标拾取模式”，然后点选场景，可设置某个FeatureLayer是否参与裁剪";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnCreateClipPlaneOperation);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(621, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 30);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            // 
            // btnCreateClipPlaneOperation
            // 
            this.btnCreateClipPlaneOperation.Location = new System.Drawing.Point(23, 0);
            this.btnCreateClipPlaneOperation.Name = "btnCreateClipPlaneOperation";
            this.btnCreateClipPlaneOperation.Size = new System.Drawing.Size(140, 31);
            this.btnCreateClipPlaneOperation.TabIndex = 0;
            this.btnCreateClipPlaneOperation.Text = "获取当前裁剪面参数";
            this.btnCreateClipPlaneOperation.UseVisualStyleBackColor = true;
            this.btnCreateClipPlaneOperation.Click += new System.EventHandler(this.btnCreateClipPlaneOperation_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.02062F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.97938F));
            this.tableLayoutPanel2.Controls.Add(this.textBoxPositionZ, 1, 11);
            this.tableLayoutPanel2.Controls.Add(this.textBoxPositionY, 1, 10);
            this.tableLayoutPanel2.Controls.Add(this.textBoxPositionX, 1, 9);
            this.tableLayoutPanel2.Controls.Add(this.textBoxAngleTilt, 1, 8);
            this.tableLayoutPanel2.Controls.Add(this.textBoxAngleRoll, 1, 7);
            this.tableLayoutPanel2.Controls.Add(this.textBoxAngleHeading, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.textBoxBoxSizeZ, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.textBoxBoxSizeY, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.textBoxBoxSizeX, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.textBoxBoxCenterZ, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBoxBoxCenterY, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 7);
            this.tableLayoutPanel2.Controls.Add(this.label10, 0, 8);
            this.tableLayoutPanel2.Controls.Add(this.btnSave, 0, 12);
            this.tableLayoutPanel2.Controls.Add(this.textBoxBoxCenterX, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label11, 0, 9);
            this.tableLayoutPanel2.Controls.Add(this.label12, 0, 10);
            this.tableLayoutPanel2.Controls.Add(this.label13, 0, 11);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(624, 33);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 13;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(194, 524);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 31);
            this.label2.TabIndex = 0;
            this.label2.Text = "BoxCenter-X";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 31);
            this.label3.TabIndex = 1;
            this.label3.Text = "BoxCenter-Y";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(3, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 31);
            this.label4.TabIndex = 2;
            this.label4.Text = "BoxCenter-Z";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(3, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 31);
            this.label5.TabIndex = 3;
            this.label5.Text = "BoxSize-X";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(3, 160);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 31);
            this.label6.TabIndex = 4;
            this.label6.Text = "BoxSize-Y";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(3, 200);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 31);
            this.label7.TabIndex = 5;
            this.label7.Text = "BoxSize-Z";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(3, 240);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 31);
            this.label8.TabIndex = 6;
            this.label8.Text = "Angle - heading";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(3, 280);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 31);
            this.label9.TabIndex = 9;
            this.label9.Text = "Angle - roll";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(3, 320);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 31);
            this.label10.TabIndex = 10;
            this.label10.Text = "Angle - tilt";
            // 
            // btnSave
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.btnSave, 2);
            this.btnSave.Location = new System.Drawing.Point(3, 483);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(104, 38);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // textBoxBoxCenterX
            // 
            this.textBoxBoxCenterX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxBoxCenterX.Location = new System.Drawing.Point(68, 3);
            this.textBoxBoxCenterX.Name = "textBoxBoxCenterX";
            this.textBoxBoxCenterX.Size = new System.Drawing.Size(123, 21);
            this.textBoxBoxCenterX.TabIndex = 12;
            // 
            // textBoxBoxCenterY
            // 
            this.textBoxBoxCenterY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxBoxCenterY.Location = new System.Drawing.Point(68, 43);
            this.textBoxBoxCenterY.Name = "textBoxBoxCenterY";
            this.textBoxBoxCenterY.Size = new System.Drawing.Size(123, 21);
            this.textBoxBoxCenterY.TabIndex = 13;
            // 
            // textBoxBoxCenterZ
            // 
            this.textBoxBoxCenterZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxBoxCenterZ.Location = new System.Drawing.Point(68, 83);
            this.textBoxBoxCenterZ.Name = "textBoxBoxCenterZ";
            this.textBoxBoxCenterZ.Size = new System.Drawing.Size(123, 21);
            this.textBoxBoxCenterZ.TabIndex = 14;
            // 
            // textBoxBoxSizeX
            // 
            this.textBoxBoxSizeX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxBoxSizeX.Location = new System.Drawing.Point(68, 123);
            this.textBoxBoxSizeX.Name = "textBoxBoxSizeX";
            this.textBoxBoxSizeX.Size = new System.Drawing.Size(123, 21);
            this.textBoxBoxSizeX.TabIndex = 15;
            // 
            // textBoxBoxSizeY
            // 
            this.textBoxBoxSizeY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxBoxSizeY.Location = new System.Drawing.Point(68, 163);
            this.textBoxBoxSizeY.Name = "textBoxBoxSizeY";
            this.textBoxBoxSizeY.Size = new System.Drawing.Size(123, 21);
            this.textBoxBoxSizeY.TabIndex = 16;
            // 
            // textBoxBoxSizeZ
            // 
            this.textBoxBoxSizeZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxBoxSizeZ.Location = new System.Drawing.Point(68, 203);
            this.textBoxBoxSizeZ.Name = "textBoxBoxSizeZ";
            this.textBoxBoxSizeZ.Size = new System.Drawing.Size(123, 21);
            this.textBoxBoxSizeZ.TabIndex = 17;
            // 
            // textBoxAngleHeading
            // 
            this.textBoxAngleHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAngleHeading.Location = new System.Drawing.Point(68, 243);
            this.textBoxAngleHeading.Name = "textBoxAngleHeading";
            this.textBoxAngleHeading.Size = new System.Drawing.Size(123, 21);
            this.textBoxAngleHeading.TabIndex = 18;
            // 
            // textBoxAngleRoll
            // 
            this.textBoxAngleRoll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAngleRoll.Location = new System.Drawing.Point(68, 283);
            this.textBoxAngleRoll.Name = "textBoxAngleRoll";
            this.textBoxAngleRoll.Size = new System.Drawing.Size(123, 21);
            this.textBoxAngleRoll.TabIndex = 19;
            // 
            // textBoxAngleTilt
            // 
            this.textBoxAngleTilt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAngleTilt.Location = new System.Drawing.Point(68, 323);
            this.textBoxAngleTilt.Name = "textBoxAngleTilt";
            this.textBoxAngleTilt.Size = new System.Drawing.Size(123, 21);
            this.textBoxAngleTilt.TabIndex = 20;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(3, 360);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 32);
            this.label11.TabIndex = 21;
            this.label11.Text = "Position-X";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(3, 400);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 32);
            this.label12.TabIndex = 22;
            this.label12.Text = "Position-Y";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(3, 440);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 32);
            this.label13.TabIndex = 23;
            this.label13.Text = "Position-Z";
            // 
            // textBoxPositionX
            // 
            this.textBoxPositionX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPositionX.Location = new System.Drawing.Point(68, 363);
            this.textBoxPositionX.Name = "textBoxPositionX";
            this.textBoxPositionX.Size = new System.Drawing.Size(123, 21);
            this.textBoxPositionX.TabIndex = 24;
            // 
            // textBoxPositionY
            // 
            this.textBoxPositionY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPositionY.Location = new System.Drawing.Point(68, 403);
            this.textBoxPositionY.Name = "textBoxPositionY";
            this.textBoxPositionY.Size = new System.Drawing.Size(123, 21);
            this.textBoxPositionY.TabIndex = 25;
            // 
            // textBoxPositionZ
            // 
            this.textBoxPositionZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPositionZ.Location = new System.Drawing.Point(68, 443);
            this.textBoxPositionZ.Name = "textBoxPositionZ";
            this.textBoxPositionZ.Size = new System.Drawing.Size(123, 21);
            this.textBoxPositionZ.TabIndex = 26;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 586);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ClipPlane";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.axRenderControl1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Gvitech.CityMaker.Controls.AxRenderControl axRenderControl1;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox toolStripInteractModeSetting;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox cbClipPlaneEnable;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox toolStripLineColorComboBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox toolStripClipModeSetting;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCreateClipPlaneOperation;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox textBoxAngleTilt;
        private System.Windows.Forms.TextBox textBoxAngleRoll;
        private System.Windows.Forms.TextBox textBoxAngleHeading;
        private System.Windows.Forms.TextBox textBoxBoxSizeZ;
        private System.Windows.Forms.TextBox textBoxBoxSizeY;
        private System.Windows.Forms.TextBox textBoxBoxSizeX;
        private System.Windows.Forms.TextBox textBoxBoxCenterZ;
        private System.Windows.Forms.TextBox textBoxBoxCenterY;
        private System.Windows.Forms.TextBox textBoxBoxCenterX;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxPositionZ;
        private System.Windows.Forms.TextBox textBoxPositionY;
        private System.Windows.Forms.TextBox textBoxPositionX;

    }
}

