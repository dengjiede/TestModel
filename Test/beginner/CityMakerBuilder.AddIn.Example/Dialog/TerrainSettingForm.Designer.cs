using CityMakerBuilder.AddIn.Core;
namespace CityMakerBuilder.AddIn.Example
{
    partial class TerrainSettingForm
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
            this.btnFindTedFile = new DevExpress.XtraEditors.SimpleButton();
            this.cbTedFile = new CityMakerBuilder.UserControls.CommonControls.CustomCombox();
            this.tedPwd = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.sbtn_OK = new DevExpress.XtraEditors.SimpleButton();
            this.sbtn_Cancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.tedPwd.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFindTedFile
            // 
            this.btnFindTedFile.Location = new System.Drawing.Point(379, 11);
            this.btnFindTedFile.Name = "btnFindTedFile";
            this.btnFindTedFile.Size = new System.Drawing.Size(38, 23);
            this.btnFindTedFile.TabIndex = 8;
            this.btnFindTedFile.Text = "...";
            this.btnFindTedFile.Click += new System.EventHandler(this.btnFindTedFile_Click);
            // 
            // cbTedFile
            // 
            this.cbTedFile.FileName = null;
            this.cbTedFile.Location = new System.Drawing.Point(107, 11);
            this.cbTedFile.Name = "cbTedFile";
            this.cbTedFile.Size = new System.Drawing.Size(266, 23);
            this.cbTedFile.TabIndex = 9;
            // 
            // tedPwd
            // 
            this.tedPwd.Location = new System.Drawing.Point(107, 45);
            this.tedPwd.Name = "tedPwd";
            this.tedPwd.Properties.PasswordChar = '*';
            this.tedPwd.Size = new System.Drawing.Size(266, 20);
            this.tedPwd.TabIndex = 7;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(13, 48);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(154, 14);
            this.labelControl5.TabIndex = 6;
            this.labelControl5.Text = StringParser.Parse("${res:project_dlg_TedPwd}");
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(13, 15);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(147, 14);
            this.labelControl4.TabIndex = 5;
            this.labelControl4.Text = StringParser.Parse("${res:project_dlg_TedFile}");
            // 
            // sbtn_OK
            // 
            this.sbtn_OK.Location = new System.Drawing.Point(92, 75);
            this.sbtn_OK.Name = "sbtn_OK";
            this.sbtn_OK.Size = new System.Drawing.Size(75, 23);
            this.sbtn_OK.TabIndex = 10;
            this.sbtn_OK.Text = StringParser.Parse("${res:project_dlg_OK}");
            this.sbtn_OK.Click += new System.EventHandler(this.sbtn_OK_Click);
            // 
            // sbtn_Cancel
            // 
            this.sbtn_Cancel.Location = new System.Drawing.Point(263, 75);
            this.sbtn_Cancel.Name = "sbtn_Cancel";
            this.sbtn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.sbtn_Cancel.TabIndex = 11;
            this.sbtn_Cancel.Text = StringParser.Parse("${res:project_dlg_Cancel}");
            this.sbtn_Cancel.Click += new System.EventHandler(this.sbtn_Cancel_Click);
            // 
            // TerrainSettingForm
            // 
            this.MaximizeBox = false;
            this.ShowInTaskbar = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 104);
            this.Controls.Add(this.sbtn_Cancel);
            this.Controls.Add(this.sbtn_OK);
            this.Controls.Add(this.btnFindTedFile);
            this.Controls.Add(this.cbTedFile);
            this.Controls.Add(this.tedPwd);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.labelControl4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(432, 130);
            this.MinimumSize = new System.Drawing.Size(426, 130);
            this.Name = "TerrainSettingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = StringParser.Parse("${res:project_dlg_TerrainLayer}");
            ((System.ComponentModel.ISupportInitialize)(this.tedPwd.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private DevExpress.XtraEditors.SimpleButton btnFindTedFile;
        private UserControls.CommonControls.CustomCombox cbTedFile;
        private DevExpress.XtraEditors.TextEdit tedPwd;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.SimpleButton sbtn_OK;
        private DevExpress.XtraEditors.SimpleButton sbtn_Cancel;
    }
}