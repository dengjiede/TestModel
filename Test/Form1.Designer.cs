namespace Test
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.axArcReaderControl1 = new ESRI.ArcGIS.PublisherControls.AxArcReaderControl();
            this.axbigPageLayoutControl = new ESRI.ArcGIS.Controls.AxPageLayoutControl();
            this.axLicenseControl1 = new ESRI.ArcGIS.Controls.AxLicenseControl();
            ((System.ComponentModel.ISupportInitialize)(this.axArcReaderControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axbigPageLayoutControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // axArcReaderControl1
            // 
            this.axArcReaderControl1.Location = new System.Drawing.Point(12, 21);
            this.axArcReaderControl1.Name = "axArcReaderControl1";
            this.axArcReaderControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axArcReaderControl1.OcxState")));
            this.axArcReaderControl1.Size = new System.Drawing.Size(305, 305);
            this.axArcReaderControl1.TabIndex = 0;
            // 
            // axbigPageLayoutControl
            // 
            this.axbigPageLayoutControl.Location = new System.Drawing.Point(323, 21);
            this.axbigPageLayoutControl.Name = "axbigPageLayoutControl";
            this.axbigPageLayoutControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axbigPageLayoutControl.OcxState")));
            this.axbigPageLayoutControl.Size = new System.Drawing.Size(265, 265);
            this.axbigPageLayoutControl.TabIndex = 1;
            // 
            // axLicenseControl1
            // 
            this.axLicenseControl1.Enabled = true;
            this.axLicenseControl1.Location = new System.Drawing.Point(627, -2);
            this.axLicenseControl1.Name = "axLicenseControl1";
            this.axLicenseControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axLicenseControl1.OcxState")));
            this.axLicenseControl1.Size = new System.Drawing.Size(32, 32);
            this.axLicenseControl1.TabIndex = 2;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(656, 492);
            this.Controls.Add(this.axLicenseControl1);
            this.Controls.Add(this.axbigPageLayoutControl);
            this.Controls.Add(this.axArcReaderControl1);
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.axArcReaderControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axbigPageLayoutControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private ESRI.ArcGIS.PublisherControls.AxArcReaderControl axArcReaderControl1;
        private ESRI.ArcGIS.Controls.AxPageLayoutControl axbigPageLayoutControl;
        private ESRI.ArcGIS.Controls.AxLicenseControl axLicenseControl1;
    }
}

        