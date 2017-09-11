using System;
using System.Windows.Forms;
using System.IO;
//****Gvitech.CityMaker****
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.Common;

namespace CalculateTopOnTerrain
{
    public partial class MainForm : Form
    {
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号
        
        private ICalculateTop calculateTop = null;

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

            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "CalculateTopOnTerrain.html";
            } 

            // 注册地形
            string tmpTedPath = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\terrain\terrain.ted");
            this.axRenderControl1.Terrain.RegisterTerrain(tmpTedPath, "");
            this.axRenderControl1.Terrain.FlyTo(gviTerrainActionCode.gviJumpToTerrain);

            // 默认选buffer
            this.toolStripAreaType.SelectedIndex = 0;
            this.axRenderControl1.RcKeyUp += new Gvitech.CityMaker.Controls._IRenderControlEvents_RcKeyUpEventHandler(this.axRenderControl1_RcKeyUp);
        }

        private bool axRenderControl1_RcKeyUp(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcKeyUpEvent e)
        {
            if (e.@char == 27)  //ESC键
            {
                calculateTop.Reset();

                this.toolStripAreaType.Enabled = true;
                this.toolStripButtonStartAnalysis.Enabled = true;
            }
            return default(bool);
        }

        private void toolStripButtonStartAnalysis_Click(object sender, EventArgs e)
        {
            this.toolStripAreaType.Enabled = false;
            this.toolStripButtonStartAnalysis.Enabled = false;
            this.axRenderControl1.Focus();
            if (this.toolStripAreaType.SelectedIndex == 0)
            {
                calculateTop = new BufferCalculateTop(axRenderControl1);
                calculateTop.CalculateTop();
            }
            else if (this.toolStripAreaType.SelectedIndex == 1)
            {
                calculateTop = new PolygonCalculateTop(axRenderControl1);
                calculateTop.CalculateTop();
            }
        }

        private void toolStripAreaType_Click(object sender, EventArgs e)
        {

        }

        private bool axRenderControl1_RcLButtonDown(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcLButtonDownEvent e)
        {
            return default(bool);
        }      
    }
}
