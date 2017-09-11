using System.Windows.Forms;
using System.IO;
//****Gvitech.CityMaker****
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.Common;
using Gvitech.CityMaker.FdeGeometry;
using System;

namespace Ocean
{
    public partial class MainForm : Form
    {
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号
        private System.Guid rootId = new System.Guid();

        IGeometryFactory geoFactory = new GeometryFactory();
        ICRSFactory crsFactory = new CRSFactory();
        ICoordinateReferenceSystem crs = null;
        IRenderGeometry currentRenderGeometry = null;
        IGeometry currentGeometry = null;

        CheckBox enableOceanEffect;  //是否开启动态海水
        ToolStripControlHost host;

        public MainForm()
        {
            InitializeComponent();

            // 初始化RenderControl控件
            IPropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", gviRenderSystem.gviRenderOpenGL);
            this.axRenderControl1.Initialize(true, ps);

            rootId = this.axRenderControl1.ObjectManager.GetProjectTree().RootID;

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
            crs = crsFactory.CreateFromWKT(this.axRenderControl1.GetTerrainCrsWKT(tmpTedPath, ""));
            this.axRenderControl1.Terrain.FlyTo(gviTerrainActionCode.gviFlyToTerrain);

            // 注册事件
            this.axRenderControl1.RcObjectEditing += new Gvitech.CityMaker.Controls._IRenderControlEvents_RcObjectEditingEventHandler(axRenderControl1_RcObjectEditing);
            this.axRenderControl1.RcObjectEditFinish += new System.EventHandler(axRenderControl1_RcObjectEditFinish);

            this.toolStripTextBoxWindSpeed.Text = this.axRenderControl1.Terrain.OceanWindSpeed.ToString();
            this.toolStripTextBoxWindDirection.Text = this.axRenderControl1.Terrain.OceanWindDirection.ToString();

            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "Ocean.html";
            }    
        }

        private void toolStripButtonLoadTerrain_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "地形文件(*.ted)|*.ted";
            if (flag > -1)
            {
                od.InitialDirectory = Application.StartupPath.Substring(0, flag) + @"Samples\Media\terrain";
            }
            od.RestoreDirectory = true;
            if (DialogResult.OK == od.ShowDialog())
            {
                string wkt = this.axRenderControl1.GetTerrainCrsWKT(od.FileName, "");
                this.axRenderControl1.Reset2(wkt);
                this.axRenderControl1.Terrain.RegisterTerrain(od.FileName, "");
                if (this.axRenderControl1.Terrain.IsPlanarTerrain)
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
                crs = crsFactory.CreateFromWKT(wkt);
                this.axRenderControl1.Terrain.FlyTo(gviTerrainActionCode.gviFlyToTerrain);
            }
        }

        private void toolStripButtonSetOceanRegion_Click(object sender, System.EventArgs e)
        {
            currentGeometry = geoFactory.CreateGeometry(gviGeometryType.gviGeometryPolygon, gviVertexAttribute.gviVertexAttributeZ) as IPolygon;
            currentGeometry.SpatialCRS = crs as ISpatialCRS;

            ISurfaceSymbol sfbottom = new SurfaceSymbol();
            sfbottom.Color = 0x550000FF;
            currentRenderGeometry = this.axRenderControl1.ObjectManager.CreateRenderPolygon(currentGeometry as IPolygon, sfbottom, rootId);
            (currentRenderGeometry as IRenderPolygon).HeightStyle = gviHeightStyle.gviHeightOnTerrain;
            this.axRenderControl1.InteractMode = gviInteractMode.gviInteractEdit;
            this.axRenderControl1.ObjectEditor.StartEditRenderGeometry(currentRenderGeometry, gviGeoEditType.gviGeoEditCreator);
        }

        void axRenderControl1_RcObjectEditFinish(object sender, System.EventArgs e)
        {
            IMultiPolygon multiPolygon = currentRenderGeometry.GetFdeGeometry() as IMultiPolygon;
            if (multiPolygon == null)
            {
                multiPolygon = geoFactory.CreateGeometry(gviGeometryType.gviGeometryMultiPolygon, gviVertexAttribute.gviVertexAttributeZ) as IMultiPolygon;
                IPolygon polygon = currentRenderGeometry.GetFdeGeometry() as IPolygon;
                multiPolygon.AddPolygon(polygon);
            }
            this.axRenderControl1.Terrain.SetOceanRegion(multiPolygon);
            currentRenderGeometry.VisibleMask = gviViewportMask.gviViewNone;
            currentRenderGeometry.ViewingDistance = 10000;
            this.axRenderControl1.Camera.FlyToObject(currentRenderGeometry.Guid, gviActionCode.gviActionFollowAbove);
            this.axRenderControl1.InteractMode = gviInteractMode.gviInteractNormal;
        }

        void axRenderControl1_RcObjectEditing(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcObjectEditingEvent e)
        {
            currentGeometry = e.geometry;
        }


        private void toolStripTextBoxDeep_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void toolStripButtonDeleteHole_Click(object sender, EventArgs e)
        {
            this.axRenderControl1.Terrain.SetOceanRegion(null);
            if (this.currentRenderGeometry != null)
            {
                this.axRenderControl1.ObjectManager.DeleteObject(currentRenderGeometry.Guid);
                this.currentRenderGeometry = null;
            }            
        }

        private void toolStripTextBoxWindSpeed_TextChanged(object sender, EventArgs e)
        {
            this.axRenderControl1.Terrain.OceanWindSpeed = Double.Parse(this.toolStripTextBoxWindSpeed.Text);
        }

        private void toolStripTextBoxWindDirection_TextChanged(object sender, EventArgs e)
        {
            this.axRenderControl1.Terrain.OceanWindDirection = Double.Parse(this.toolStripTextBoxWindDirection.Text);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            enableOceanEffect = new CheckBox();
            enableOceanEffect.Text = "是否开启海水特效";
            enableOceanEffect.Checked = this.axRenderControl1.Terrain.EnableOceanEffect;
            enableOceanEffect.CheckedChanged += new EventHandler(enableOceanEffect_CheckedChanged);
            host = new ToolStripControlHost(enableOceanEffect);
            toolStrip1.Items.Add(host);
        }

        void enableOceanEffect_CheckedChanged(object sender, EventArgs e)
        {
            this.axRenderControl1.Terrain.EnableOceanEffect = this.enableOceanEffect.Checked;
        }

    }
}
