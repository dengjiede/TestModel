using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CityMakerBuilder.WorkSpace;
using Gvitech.CityMaker.RenderControl;
using CityMakerBuilder.AddIn.Core;
using System.IO;
using System.Runtime.InteropServices;

namespace CityMakerBuilder.AddIn.Example
{
    public partial class TerrainSettingForm : DevExpress.XtraEditors.XtraForm
    {
        //写到当前用户缓存目录 C:\Documents and Settings\Administrator\Application Data
        private string _tmpPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                               "CityMaker Builder");
        private string tedName = "";
        private string tedfilepath = "";
        private string tedpassword = "";
        private string wkt = "";
        private bool bwkt = false;
        public TerrainSettingForm()
        {
            InitializeComponent();
            if (!Directory.Exists(_tmpPath))
            {
                Directory.CreateDirectory(_tmpPath);
            }
            //设置CustomCombox下拉框对应的历史文件路径
            this.cbTedFile.FileName = _tmpPath + "\\TedAddress.db";

            //如果设置过地形， 则显示旧地形
            if (ExampleProcess.Instance().TedPath != "")
                this.cbTedFile.cmbContent.Text = ExampleProcess.Instance().TedPath;
        }
        /// <summary>
        /// 创建工程时调用
        /// </summary>
        /// <param name="bReadWkt"></param>
        public TerrainSettingForm(bool bReadWkt)
            : this()
        {
            bwkt = bReadWkt;
        }
        public string WKT
        {
            get { return this.wkt; }
        }
        public string TedName
        {
            get { return this.tedName; }
        }
        public string TedPath
        {
            get { return this.tedfilepath; }
        }
        public string TedPwd
        {
            get { return this.tedpassword; }
        }
        /// <summary>
        /// 查找地形文件
        /// </summary>
        private void btnFindTedFile_Click(object sender, EventArgs e)
        {
            TerrainServerForm tsForm = new TerrainServerForm();
            if (tsForm.ShowDialog() == DialogResult.OK)
            {
                this.cbTedFile.cmbContent.Text = tsForm.TedPath;
                this.cbTedFile.cmbContent.Focus();  //触发控件保存路径事件
            }
        }
        private string TrimPath(string path)
        {
            string s = "";
            try
            {
                int len1 = path.LastIndexOf(".");
                int len2 = path.LastIndexOf("\\");
                s = path.Substring(len2 + 1, len1 - len2 - 1);
            }
            catch (Exception ex)
            { }
            return s;
        }
        void sbtn_OK_Click(object sender, System.EventArgs e)
        {
            try
            {
                tedfilepath = this.cbTedFile.cmbContent.Text;
                if (!tedfilepath.Equals(""))
                {
                    tedName = File.Exists(tedfilepath) ? TrimPath(tedfilepath) : tedfilepath;
                    if (tedName.Equals(""))
                    {
                        XtraMessageBox.Show(StringParser.Parse("${res:View__FDE_INVALID_LACAL_PATH}"));
                        this.cbTedFile.Focus();
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                    tedpassword = this.tedPwd.Text;
                    if (!bwkt)
                    {
                        bool msg = RenderControlServices.Instance().AxRenderControl.Terrain.RegisterTerrain(tedfilepath, tedpassword);
                        int rcError = RenderControlServices.Instance().AxRenderControl.GetLastError();
                        if (rcError != 0)
                        {
                            XtraMessageBox.Show(Logger.GetRenderCtrlError(rcError));
                            this.DialogResult = DialogResult.None;
                            return;
                        }
                        if (!msg)
                        {
                            XtraMessageBox.Show(StringParser.Parse("${res:project_alert_loadTerrainFailed}"));
                            this.DialogResult = DialogResult.None;
                        }
                        else
                        {
                            this.DialogResult = DialogResult.OK;
                            RenderControlServices.Instance().AxRenderControl.Terrain.FlyTo();
                            ExampleProcess.Instance().TedPath = tedfilepath;
                            ExampleProcess.Instance().TedPwd = tedpassword;
                        }
                    }
                    else
                    {
                        wkt = RenderControlServices.Instance().AxRenderControl.GetTerrainCrsWKT(layerInfo: tedfilepath, password: tedpassword);
                        int rcError = RenderControlServices.Instance().AxRenderControl.GetLastError();
                        if (rcError != 0)
                        {
                            XtraMessageBox.Show(Logger.GetRenderCtrlError(rcError));
                            this.DialogResult = DialogResult.None;
                        }
                        else
                            this.DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.None;
                }
            }
            catch(COMException comEx)
            {
                
            }
            catch (System.Exception ex)
            {
               
            }
        }
        void sbtn_Cancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}