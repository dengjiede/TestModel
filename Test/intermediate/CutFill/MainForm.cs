﻿using System.Windows.Forms;
using System.IO;
//****Gvitech.CityMaker****
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.Common;
using Gvitech.CityMaker.FdeGeometry;
using System;
using System.Collections.Generic;

namespace CutFill
{
    public partial class MainForm : Form
    {
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号

        IGeometryFactory geoFactory = new GeometryFactory();
        ICRSFactory crsFactory = new CRSFactory();
        ICoordinateReferenceSystem crs = null;
        IRenderGeometry currentRenderGeometry = null;
        IGeometry currentGeometry = null;

        IRenderMultiPolygon cutrmp;
        IRenderMultiPolygon fillrmp;

        TerrainAnalyseCallBack _cb = new TerrainAnalyseCallBack();

        private System.Guid rootId = new System.Guid();

        public MainForm()
        {
            InitializeComponent();

            enableRightPanel(false);

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

            //注册回调
            _cb.onProcessing = this.OnProcessing;

            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "CutFill.html";
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
                if (crsFactory.CreateFromWKT(wkt).IsGeographic())
                {
                    MessageBox.Show("球面不能进行填挖方分析，经纬度不能计算体积，面积");
                    return;
                }
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

        private void toolStripButtonCreateCutFill_Click(object sender, System.EventArgs e)
        {
            if (currentRenderGeometry != null)
            {
                this.axRenderControl1.ObjectManager.DeleteObject(currentRenderGeometry.Guid);
                currentRenderGeometry = null;
            }
            if (cutrmp != null)
            {
                this.axRenderControl1.ObjectManager.DeleteObject(cutrmp.Guid);
                cutrmp = null;
            }
            if (fillrmp != null)
            {
                this.axRenderControl1.ObjectManager.DeleteObject(fillrmp.Guid);
                fillrmp = null;
            }

            currentGeometry = geoFactory.CreateGeometry(gviGeometryType.gviGeometryPolygon, gviVertexAttribute.gviVertexAttributeZ) as IPolygon;
            currentGeometry.SpatialCRS = crs as ISpatialCRS;
            ISurfaceSymbol sf = new SurfaceSymbol();
            sf.Color = 0x55ffff80;
            ICurveSymbol cs = new CurveSymbol();
            cs.Color = 0x55ffff80;
            sf.BoundarySymbol = cs;
            currentRenderGeometry = this.axRenderControl1.ObjectManager.CreateRenderPolygon(currentGeometry as IPolygon, sf, rootId);
            (currentRenderGeometry as IRenderPolygon).HeightStyle = gviHeightStyle.gviHeightOnTerrain;
            this.axRenderControl1.InteractMode = gviInteractMode.gviInteractEdit;
            this.axRenderControl1.ObjectEditor.StartEditRenderGeometry(currentRenderGeometry, gviGeoEditType.gviGeoEditCreator);
        }

        void axRenderControl1_RcObjectEditFinish(object sender, System.EventArgs e)
        {
            enableRightPanel(true);
            this.axRenderControl1.InteractMode = gviInteractMode.gviInteractNormal;
        }

        void axRenderControl1_RcObjectEditing(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcObjectEditingEvent e)
        {
            currentGeometry = e.geometry;
        }

        void enableRightPanel(bool able)
        {
            this.numHeight.Enabled = able;
            this.numSampling.Enabled = able;
            this.btnOnProcess.Enabled = able;
            this.txtCutVolume.Enabled = able;
            this.txtFillVolume.Enabled = able;
        }

        private void DoFillCut()
        {
            TerrainAnalyse ta = new TerrainAnalyse();
            ta.OnProcessing = _cb;
            IMultiPolygon CutmPolygon = geoFactory.CreateGeometry(gviGeometryType.gviGeometryMultiPolygon, gviVertexAttribute.gviVertexAttributeZ) as IMultiPolygon;
            IMultiPolygon FillmPolygon = geoFactory.CreateGeometry(gviGeometryType.gviGeometryMultiPolygon, gviVertexAttribute.gviVertexAttributeZ) as IMultiPolygon;
            double CutVolume = 0;
            double FillVolume = 0;
            ta.CalculateCutFill(currentRenderGeometry.GetFdeGeometry() as IPolygon, (double)numSampling.Value, (double)numHeight.Value, ref CutmPolygon, ref FillmPolygon, ref CutVolume, ref FillVolume);
            if (CutVolume != 0)
            {
                ISurfaceSymbol sf = new SurfaceSymbol();
                sf.Color = 0xbbFF0000;
                ICurveSymbol cs = new CurveSymbol();
                cs.Color = 0xcc0000cc;
                sf.BoundarySymbol = cs;
                cutrmp = this.axRenderControl1.ObjectManager.CreateRenderMultiPolygon(CutmPolygon, sf, rootId);
                cutrmp.HeightStyle = gviHeightStyle.gviHeightOnTerrain;
                txtCutVolume.Text = Math.Round(CutVolume, 4).ToString();
            }
            if (FillVolume != 0)
            {
                ISurfaceSymbol sf = new SurfaceSymbol();
                sf.Color = 0xbb0000FF;
                ICurveSymbol cs = new CurveSymbol();
                cs.Color = 0xcc0000cc;
                sf.BoundarySymbol = cs;
                fillrmp = this.axRenderControl1.ObjectManager.CreateRenderMultiPolygon(FillmPolygon, sf, rootId);
                fillrmp.HeightStyle = gviHeightStyle.gviHeightOnTerrain;
                txtFillVolume.Text = Math.Round(FillVolume, 4).ToString();
            }
        }

        /// <summary>
        /// 开挖
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOnProcess_Click(object sender, EventArgs e)
        {
            if (cutrmp != null)
            {
                this.axRenderControl1.ObjectManager.DeleteObject(cutrmp.Guid);
                cutrmp = null;
            }
            if (fillrmp != null)
            {
                this.axRenderControl1.ObjectManager.DeleteObject(fillrmp.Guid);
                fillrmp = null;
            }
            DoFillCut();
        }

        public double[] OnProcessing(gviTerrainAnalyseOperation op, double[] ptArray)
        {
            List<double> ret = new List<double>();
            if (ptArray != null)
            {
                for (int i = 0; i < ptArray.Length / 2; i++)
                {
                    double x = ptArray[2 * i];
                    double y = ptArray[2 * i + 1];
                    double z = this.axRenderControl1.Terrain.GetElevation(x, y, gviGetElevationType.gviGetElevationFromDatabase);
                    ret.Add(z);
                }
            }
            return ret.ToArray();
        }

    }
}
