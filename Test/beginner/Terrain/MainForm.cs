using System;
using System.Windows.Forms;
using System.IO;
//****Gvitech.CityMaker****
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.Common;

namespace Terrain
{
    public partial class MainForm : Form
    {
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号

        private const String HIDEDEM = "关闭DEM";
        private const String SHOWDEM = "开启DEM";
        private const String TEDVISIBLE = "显示地形";
        private const String TEDNOTVISIBLE = "隐藏地形";

        TrackBar opacityTrack;  //地形透明度滑动条
        ToolStripControlHost host;

        public MainForm()
        {
            InitializeComponent();

            // 初始化RenderControl控件
            IPropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", gviRenderSystem.gviRenderOpenGL);
            this.axRenderControl1.Initialize(true, ps);

            // 设置天空盒
            flag = Application.StartupPath.LastIndexOf("Samples");
            if (flag > -1)
            {
                string tmpSkyboxPath = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\skybox");
                ISkyBox skybox = this.axRenderControl1.ObjectManager.GetSkyBox(0);
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageBack, tmpSkyboxPath + "\\1_BK.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageBottom, tmpSkyboxPath + "\\1_DN.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageFront, tmpSkyboxPath + "\\1_FR.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageLeft, tmpSkyboxPath + "\\1_LF.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageRight, tmpSkyboxPath + "\\1_RT.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageTop, tmpSkyboxPath + "\\1_UP.jpg");   
            }
            else
            {
                MessageBox.Show("请不要随意更改SDK目录名");
                return;
            }

            // 注册地形
            string tmpTedPath = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\terrain\terrain.ted");
            this.axRenderControl1.Terrain.RegisterTerrain(tmpTedPath, "");
            this.axRenderControl1.Terrain.FlyTo(gviTerrainActionCode.gviFlyToTerrain);

            this.toolStripGetElevationType.SelectedIndex = 0;

            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "Terrain.html";
            }  
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            opacityTrack = new TrackBar();
            opacityTrack.Minimum = 1;
            opacityTrack.Maximum = 100;
            opacityTrack.TickFrequency = 1;
            opacityTrack.Value = 100;
            opacityTrack.ValueChanged += new EventHandler(track_ValueChanged);
            host = new ToolStripControlHost(opacityTrack);
            toolStrip1.Items.Add(host);
        }

        void track_ValueChanged(object sender, EventArgs e)
        {
            double opacity = opacityTrack.Value / 100.0;
            this.axRenderControl1.Terrain.Opacity = opacity;
        }

        private void toolStripDEM_Click(object sender, EventArgs e)
        {
            if (this.toolStripDEM.Text == HIDEDEM)
            {
                this.axRenderControl1.Terrain.DemAvailable = false;
                this.toolStripDEM.Text = SHOWDEM;
            }
            else if (this.toolStripDEM.Text == SHOWDEM)
            {
                this.axRenderControl1.Terrain.DemAvailable = true;
                this.toolStripDEM.Text = HIDEDEM;
            }
        }

        private void toolStripVisible_Click(object sender, EventArgs e)
        {
            if (this.toolStripVisible.Text == TEDNOTVISIBLE)
            {
                this.axRenderControl1.Terrain.VisibleMask = gviViewportMask.gviViewNone;
                this.toolStripVisible.Text = TEDVISIBLE;
            }
            else if (this.toolStripVisible.Text == TEDVISIBLE)
            {
                this.axRenderControl1.Terrain.VisibleMask = gviViewportMask.gviViewAllNormalView;
                this.toolStripVisible.Text = TEDNOTVISIBLE;
            }
        }

        private void toolStripGetElevation_Click(object sender, EventArgs e)
        {
            // 注册三维控件选择事件
            this.axRenderControl1.RcMouseClickSelect += new Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEventHandler(axRenderControl1_RcMouseClickSelect);
            this.axRenderControl1.InteractMode = gviInteractMode.gviInteractSelect;
            this.axRenderControl1.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;
            this.axRenderControl1.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectTerrain;

            this.toolStripGetElevationType.Enabled = false;
            this.toolStripGetElevation.Enabled = false;
            this.axRenderControl1.Focus();
        }

        void axRenderControl1_RcMouseClickSelect(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEvent e)
        {
            if (e.pickResult.Type == gviObjectType.gviObjectTerrain)
            {
                gviGetElevationType mode = gviGetElevationType.gviGetElevationFromMemory;
                switch (this.toolStripGetElevationType.Text)
                {
                    case "GetElevationFromMemory":
                        mode = gviGetElevationType.gviGetElevationFromMemory;
                        break;
                    case "GetElevationFromDatabase":
                        mode = gviGetElevationType.gviGetElevationFromDatabase;
                        break;
                }
                double elevatation = this.axRenderControl1.Terrain.GetElevation(e.intersectPoint.X, e.intersectPoint.Y, mode);
                MessageBox.Show("当前位置高程为:" + elevatation.ToString());
            }
        }

        private void toolStripWKT_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.axRenderControl1.Terrain.CrsWKT);
        }

        private bool axRenderControl1_RcKeyUp(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcKeyUpEvent e)
        {
            if (e.@char == 27)  //ESC键
            {
                this.axRenderControl1.RcMouseClickSelect -= new Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEventHandler(axRenderControl1_RcMouseClickSelect);
                this.axRenderControl1.InteractMode = gviInteractMode.gviInteractNormal;

                this.toolStripGetElevationType.Enabled = true;
                this.toolStripGetElevation.Enabled = true;
            }
            return default(bool);
        }

        /// <summary>
        /// 取消方向键对控件焦点的控制。
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
