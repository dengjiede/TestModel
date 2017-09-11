using CityMakerBuilder.AddIn.Core;
namespace CityMakerBuilder.AddIn.Example
{
    partial class TerrainServerForm
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.gc_File = new DevExpress.XtraEditors.GroupControl();
            this.sbtn_FromFile = new DevExpress.XtraEditors.SimpleButton();
            this.gc_Service = new DevExpress.XtraEditors.GroupControl();
            this.sbtn_OK = new DevExpress.XtraEditors.SimpleButton();
            this.te_DSet = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.te_DSrc = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.te_Port = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.te_IP = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gc_File)).BeginInit();
            this.gc_File.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gc_Service)).BeginInit();
            this.gc_Service.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.te_DSet.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_DSrc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_Port.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_IP.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.gc_File);
            this.panelControl1.Controls.Add(this.gc_Service);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(316, 293);
            this.panelControl1.TabIndex = 0;
            // 
            // gc_File
            // 
            this.gc_File.Controls.Add(this.sbtn_FromFile);
            this.gc_File.Location = new System.Drawing.Point(0, 193);
            this.gc_File.Name = "gc_File";
            this.gc_File.Size = new System.Drawing.Size(316, 100);
            this.gc_File.TabIndex = 11;
            this.gc_File.Text = StringParser.Parse("${res:project_dlg_TerrainFile}");
            // 
            // sbtn_FromFile
            // 
            this.sbtn_FromFile.Location = new System.Drawing.Point(55, 45);
            this.sbtn_FromFile.Name = "sbtn_FromFile";
            this.sbtn_FromFile.Size = new System.Drawing.Size(200, 27);
            this.sbtn_FromFile.TabIndex = 10;
            this.sbtn_FromFile.Text = StringParser.Parse("${res:project_dlg_FromFile}");
            this.sbtn_FromFile.Click+=new System.EventHandler(sbtn_FromFile_Click);
            // 
            // gc_Service
            // 
            this.gc_Service.Controls.Add(this.sbtn_OK);
            this.gc_Service.Controls.Add(this.te_DSet);
            this.gc_Service.Controls.Add(this.labelControl4);
            this.gc_Service.Controls.Add(this.te_DSrc);
            this.gc_Service.Controls.Add(this.labelControl3);
            this.gc_Service.Controls.Add(this.te_Port);
            this.gc_Service.Controls.Add(this.labelControl2);
            this.gc_Service.Controls.Add(this.te_IP);
            this.gc_Service.Controls.Add(this.labelControl1);
            this.gc_Service.Location = new System.Drawing.Point(0, 0);
            this.gc_Service.Name = "gc_Service";
            this.gc_Service.Size = new System.Drawing.Size(316, 189);
            this.gc_Service.TabIndex = 10;
            this.gc_Service.Text = StringParser.Parse("${res:project_dlg_TerrainService}");
            // 
            // sbtn_OK
            // 
            this.sbtn_OK.Location = new System.Drawing.Point(196, 157);
            this.sbtn_OK.Name = "sbtn_OK";
            this.sbtn_OK.Size = new System.Drawing.Size(75, 27);
            this.sbtn_OK.TabIndex = 17;
            this.sbtn_OK.Text = StringParser.Parse("${res:project_dlg_OK}");
            this.sbtn_OK.Click+=new System.EventHandler(sbtn_OK_Click);
            // 
            // te_DSet
            // 
            this.te_DSet.Location = new System.Drawing.Point(91, 131);
            this.te_DSet.Name = "te_DSet";
            this.te_DSet.Size = new System.Drawing.Size(204, 20);
            this.te_DSet.TabIndex = 16;
            this.te_DSet.TextChanged+=new System.EventHandler(te_DSet_TextChanged);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(20, 134);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(149, 14);
            this.labelControl4.TabIndex = 15;
            this.labelControl4.Text = StringParser.Parse("${res:project_dlg_dataset}");
            // 
            // te_DSrc
            // 
            this.te_DSrc.Location = new System.Drawing.Point(91, 96);
            this.te_DSrc.Name = "te_DSrc";
            this.te_DSrc.Size = new System.Drawing.Size(204, 20);
            this.te_DSrc.TabIndex = 14;
            this.te_DSrc.TextChanged+=new System.EventHandler(te_DSrc_TextChanged);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(20, 99);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(171, 14);
            this.labelControl3.TabIndex = 13;
            this.labelControl3.Text = StringParser.Parse("${res:project_dlg_DataSource}");
            // 
            // te_Port
            // 
            this.te_Port.Location = new System.Drawing.Point(91, 62);
            this.te_Port.Name = "te_Port";
            this.te_Port.Size = new System.Drawing.Size(204, 20);
            this.te_Port.TabIndex = 12;
            this.te_Port.TextChanged+=new System.EventHandler(te_Port_TextChanged);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(20, 65);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(131, 14);
            this.labelControl2.TabIndex = 11;
            this.labelControl2.Text = StringParser.Parse("${res:project_dlg_port}");
            // 
            // te_IP
            // 
            this.te_IP.Location = new System.Drawing.Point(91, 29);
            this.te_IP.Name = "te_IP";
            this.te_IP.Size = new System.Drawing.Size(204, 20);
            this.te_IP.TabIndex = 10;
            this.te_IP.TextChanged+=new System.EventHandler(te_IP_TextChanged);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(20, 32);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(184, 14);
            this.labelControl1.TabIndex = 9;
            this.labelControl1.Text = StringParser.Parse("${res:project_dlg_serverAddress}");
            // 
            // TerrainServerForm
            // 
            this.ShowInTaskbar = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 293);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(322, 319);
            this.MinimumSize = new System.Drawing.Size(316, 293);
            this.Name = "TerrainServerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = StringParser.Parse("${res:project_dlg_TedSetting}");
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gc_File)).EndInit();
            this.gc_File.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gc_Service)).EndInit();
            this.gc_Service.ResumeLayout(false);
            this.gc_Service.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.te_DSet.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_DSrc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_Port.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_IP.Properties)).EndInit();
            this.ResumeLayout(false);

        }

       
        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.GroupControl gc_File;
        private DevExpress.XtraEditors.SimpleButton sbtn_FromFile;
        private DevExpress.XtraEditors.GroupControl gc_Service;
        private DevExpress.XtraEditors.SimpleButton sbtn_OK;
        private DevExpress.XtraEditors.TextEdit te_DSet;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit te_DSrc;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit te_Port;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit te_IP;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}