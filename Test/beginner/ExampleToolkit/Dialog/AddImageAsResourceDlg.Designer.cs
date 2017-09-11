namespace ExampleToolkit.Dialog
{
    partial class AddImageAsResourceDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddImageAsResourceDlg));
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.sbtn_Next = new DevExpress.XtraEditors.SimpleButton();
            this.sbtn_Prev = new DevExpress.XtraEditors.SimpleButton();
            this.sbtn_Delete = new DevExpress.XtraEditors.SimpleButton();
            this.sbtn_BrowseData = new DevExpress.XtraEditors.SimpleButton();
            this.te_InputData = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gv_InputData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.col_DataPath = new DevExpress.XtraGrid.Columns.GridColumn();
            this.col_DataItem = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.te_CopyrightImage = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.sbtn_BrowseCopyrightImage = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataOpLayoutControl)).BeginInit();
            this.dataOpLayoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataOplayoutControlGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_InputData.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_InputData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_CopyrightImage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            this.SuspendLayout();
            // 
            // sbtnOK
            // 
            this.sbtnOK.Click += new System.EventHandler(this.sbtnOK_Click);
            // 
            // sbtnCancel
            // 
            this.sbtnCancel.Click += new System.EventHandler(this.sbtnCancel_Click);
            // 
            // dataOpLayoutControl
            // 
            this.dataOpLayoutControl.Controls.Add(this.sbtn_BrowseCopyrightImage);
            this.dataOpLayoutControl.Controls.Add(this.te_CopyrightImage);
            this.dataOpLayoutControl.Controls.Add(this.sbtn_Next);
            this.dataOpLayoutControl.Controls.Add(this.gridControl1);
            this.dataOpLayoutControl.Controls.Add(this.sbtn_Prev);
            this.dataOpLayoutControl.Controls.Add(this.te_InputData);
            this.dataOpLayoutControl.Controls.Add(this.sbtn_BrowseData);
            this.dataOpLayoutControl.Controls.Add(this.sbtn_Delete);
            // 
            // dataOplayoutControlGroup
            // 
            this.dataOplayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.emptySpaceItem2,
            this.layoutControlGroup1,
            this.layoutControlGroup2});
            // 
            // imageList_Icon
            // 
            this.imageList_Icon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_Icon.ImageStream")));
            this.imageList_Icon.Images.SetKeyName(0, "Normal.png");
            this.imageList_Icon.Images.SetKeyName(1, "Warning.png");
            this.imageList_Icon.Images.SetKeyName(2, "Error.png");
            this.imageList_Icon.Images.SetKeyName(3, "DeleteError.png");
            this.imageList_Icon.Images.SetKeyName(4, "Delete.png");
            this.imageList_Icon.Images.SetKeyName(5, "GenericBlackArrowUp16.png");
            this.imageList_Icon.Images.SetKeyName(6, "GenericBlackArrowDown16.png");
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            this.emptySpaceItem1.Location = new System.Drawing.Point(332, 104);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(33, 77);
            this.emptySpaceItem1.Text = "emptySpaceItem1";
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // sbtn_Next
            // 
            this.sbtn_Next.Enabled = false;
            this.sbtn_Next.ImageIndex = 6;
            this.sbtn_Next.ImageList = this.imageList_Icon;
            this.sbtn_Next.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.sbtn_Next.Location = new System.Drawing.Point(342, 108);
            this.sbtn_Next.Name = "sbtn_Next";
            this.sbtn_Next.Size = new System.Drawing.Size(29, 22);
            this.sbtn_Next.StyleController = this.dataOpLayoutControl;
            this.sbtn_Next.TabIndex = 8;
            this.sbtn_Next.Click += new System.EventHandler(this.sbtn_Next_Click);
            // 
            // sbtn_Prev
            // 
            this.sbtn_Prev.Enabled = false;
            this.sbtn_Prev.ImageIndex = 5;
            this.sbtn_Prev.ImageList = this.imageList_Icon;
            this.sbtn_Prev.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.sbtn_Prev.Location = new System.Drawing.Point(342, 82);
            this.sbtn_Prev.Name = "sbtn_Prev";
            this.sbtn_Prev.Size = new System.Drawing.Size(29, 22);
            this.sbtn_Prev.StyleController = this.dataOpLayoutControl;
            this.sbtn_Prev.TabIndex = 7;
            this.sbtn_Prev.Click += new System.EventHandler(this.sbtn_Prev_Click);
            // 
            // sbtn_Delete
            // 
            this.sbtn_Delete.Enabled = false;
            this.sbtn_Delete.ImageIndex = 4;
            this.sbtn_Delete.ImageList = this.imageList_Icon;
            this.sbtn_Delete.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.sbtn_Delete.Location = new System.Drawing.Point(342, 56);
            this.sbtn_Delete.Name = "sbtn_Delete";
            this.sbtn_Delete.Size = new System.Drawing.Size(29, 22);
            this.sbtn_Delete.StyleController = this.dataOpLayoutControl;
            this.sbtn_Delete.TabIndex = 6;
            this.sbtn_Delete.Click += new System.EventHandler(this.sbtn_Delete_Click);
            // 
            // sbtn_BrowseData
            // 
            this.sbtn_BrowseData.Location = new System.Drawing.Point(342, 30);
            this.sbtn_BrowseData.Name = "sbtn_BrowseData";
            this.sbtn_BrowseData.Size = new System.Drawing.Size(29, 22);
            this.sbtn_BrowseData.StyleController = this.dataOpLayoutControl;
            this.sbtn_BrowseData.TabIndex = 5;
            this.sbtn_BrowseData.Text = "...";
            this.sbtn_BrowseData.Click += new System.EventHandler(this.sbtn_BrowseData_Click);
            // 
            // te_InputData
            // 
            this.te_InputData.AllowDrop = true;
            this.te_InputData.Location = new System.Drawing.Point(10, 30);
            this.te_InputData.Name = "te_InputData";
            this.te_InputData.Properties.ReadOnly = true;
            this.te_InputData.Size = new System.Drawing.Size(328, 20);
            this.te_InputData.StyleController = this.dataOpLayoutControl;
            this.te_InputData.TabIndex = 4;
            this.te_InputData.DragDrop += new System.Windows.Forms.DragEventHandler(this.te_InputData_DragDrop);
            this.te_InputData.DragEnter += new System.Windows.Forms.DragEventHandler(this.te_InputData_DragEnter);
            this.te_InputData.Enter += new System.EventHandler(this.te_InputData_Enter);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.te_InputData;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(332, 26);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.sbtn_BrowseData;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.layoutControlItem2.Location = new System.Drawing.Point(332, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(33, 26);
            this.layoutControlItem2.Text = "layoutControlItem2";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(10, 56);
            this.gridControl1.MainView = this.gv_InputData;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(328, 151);
            this.gridControl1.TabIndex = 9;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_InputData});
            // 
            // gv_InputData
            // 
            this.gv_InputData.Appearance.SelectedRow.BackColor = System.Drawing.Color.CornflowerBlue;
            this.gv_InputData.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gv_InputData.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.col_DataPath,
            this.col_DataItem});
            this.gv_InputData.GridControl = this.gridControl1;
            this.gv_InputData.Name = "gv_InputData";
            this.gv_InputData.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gv_InputData.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gv_InputData.OptionsBehavior.Editable = false;
            this.gv_InputData.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gv_InputData.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gv_InputData.OptionsSelection.MultiSelect = true;
            this.gv_InputData.OptionsView.ShowColumnHeaders = false;
            this.gv_InputData.OptionsView.ShowGroupPanel = false;
            this.gv_InputData.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
            this.gv_InputData.OptionsView.ShowIndicator = false;
            this.gv_InputData.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_InputData_RowStyle);
            this.gv_InputData.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gv_InputData_SelectionChanged);
            this.gv_InputData.RowCountChanged += new System.EventHandler(this.gv_InputData_RowCountChanged);
            // 
            // col_DataPath
            // 
            this.col_DataPath.Caption = "数据路径";
            this.col_DataPath.FieldName = "DataPath";
            this.col_DataPath.Name = "col_DataPath";
            this.col_DataPath.Visible = true;
            this.col_DataPath.VisibleIndex = 0;
            // 
            // col_DataItem
            // 
            this.col_DataItem.Caption = "数据项";
            this.col_DataItem.FieldName = "DataItem";
            this.col_DataItem.Name = "col_DataItem";
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.gridControl1;
            this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(332, 155);
            this.layoutControlItem3.Text = "layoutControlItem3";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextToControlDistance = 0;
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.sbtn_Delete;
            this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
            this.layoutControlItem4.Location = new System.Drawing.Point(332, 26);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(33, 26);
            this.layoutControlItem4.Text = "layoutControlItem4";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextToControlDistance = 0;
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 279);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(381, 93);
            this.emptySpaceItem2.Text = "emptySpaceItem2";
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.sbtn_Prev;
            this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
            this.layoutControlItem5.Location = new System.Drawing.Point(332, 52);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(33, 26);
            this.layoutControlItem5.Text = "layoutControlItem5";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextToControlDistance = 0;
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.sbtn_Next;
            this.layoutControlItem6.CustomizationFormText = "layoutControlItem6";
            this.layoutControlItem6.Location = new System.Drawing.Point(332, 78);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(33, 26);
            this.layoutControlItem6.Text = "layoutControlItem6";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextToControlDistance = 0;
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "输入数据集";
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlGroup1.Size = new System.Drawing.Size(381, 217);
            this.layoutControlGroup1.Text = "输入数据集";
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.CustomizationFormText = "加密类型";
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem7,
            this.layoutControlItem8});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 217);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlGroup2.Size = new System.Drawing.Size(381, 62);
            this.layoutControlGroup2.Text = "版权图片";
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.te_CopyrightImage;
            this.layoutControlItem7.CustomizationFormText = "layoutControlItem7";
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(332, 26);
            this.layoutControlItem7.Text = "layoutControlItem7";
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextToControlDistance = 0;
            this.layoutControlItem7.TextVisible = false;
            // 
            // te_CopyrightImage
            // 
            this.te_CopyrightImage.Location = new System.Drawing.Point(10, 247);
            this.te_CopyrightImage.Name = "te_CopyrightImage";
            this.te_CopyrightImage.Properties.ReadOnly = true;
            this.te_CopyrightImage.Size = new System.Drawing.Size(328, 20);
            this.te_CopyrightImage.StyleController = this.dataOpLayoutControl;
            this.te_CopyrightImage.TabIndex = 6;
            this.te_CopyrightImage.Enter += new System.EventHandler(this.te_CopyrightImage_Enter);
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.sbtn_BrowseCopyrightImage;
            this.layoutControlItem8.CustomizationFormText = "layoutControlItem8";
            this.layoutControlItem8.Location = new System.Drawing.Point(332, 0);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(33, 26);
            this.layoutControlItem8.Text = "layoutControlItem8";
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextToControlDistance = 0;
            this.layoutControlItem8.TextVisible = false;
            // 
            // sbtn_BrowseCopyrightImage
            // 
            this.sbtn_BrowseCopyrightImage.Location = new System.Drawing.Point(342, 247);
            this.sbtn_BrowseCopyrightImage.Name = "sbtn_BrowseCopyrightImage";
            this.sbtn_BrowseCopyrightImage.Size = new System.Drawing.Size(29, 22);
            this.sbtn_BrowseCopyrightImage.StyleController = this.dataOpLayoutControl;
            this.sbtn_BrowseCopyrightImage.TabIndex = 6;
            this.sbtn_BrowseCopyrightImage.Text = "...";
            this.sbtn_BrowseCopyrightImage.Click += new System.EventHandler(this.sbtn_BrowseCopyrightImage_Click);
            // 
            // WatermarkImageDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 414);
            this.Name = "WatermarkImageDlg";
            this.Text = "把贴图加到资源库中";
            ((System.ComponentModel.ISupportInitialize)(this.dataOpLayoutControl)).EndInit();
            this.dataOpLayoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataOplayoutControlGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_InputData.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_InputData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_CopyrightImage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.SimpleButton sbtn_Next;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_InputData;
        private DevExpress.XtraEditors.SimpleButton sbtn_Prev;
        private DevExpress.XtraEditors.TextEdit te_InputData;
        private DevExpress.XtraEditors.SimpleButton sbtn_BrowseData;
        private DevExpress.XtraEditors.SimpleButton sbtn_Delete;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraGrid.Columns.GridColumn col_DataPath;
        private DevExpress.XtraGrid.Columns.GridColumn col_DataItem;
        private DevExpress.XtraEditors.SimpleButton sbtn_BrowseCopyrightImage;
        private DevExpress.XtraEditors.TextEdit te_CopyrightImage;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
    }
}