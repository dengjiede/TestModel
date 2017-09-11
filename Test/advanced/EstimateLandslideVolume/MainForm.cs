using System.Windows.Forms;
using System.IO;
//****Gvitech.CityMaker****
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.Common;
using Gvitech.CityMaker.FdeGeometry;
using System;
using System.Collections.Generic;
using Gvitech.CityMaker.FdeCore;
using System.Collections;
using System.Runtime.InteropServices;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.Resource;


namespace EstimateLandslideVolume
{
    public partial class MainForm : Form
    {
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号
        private Hashtable fcMap = null;  //IFeatureClass, List<string> 存储dataset里featureclass及对应的空间列名
        private IEnvelope env;//加载数据时，初始化的矩形范围

        IGeometryFactory geoFactory = new GeometryFactory();
        ICRSFactory crsFactory = new CRSFactory();
        ICoordinateReferenceSystem crs = null;
        IRenderGeometry currentRenderGeometry = null;
        IGeometry currentGeometry = null;

        IRenderGeometry intersectGeo = null;
        IFeatureClass __fc = null;
        IModel modelRet = null;

        private System.Guid rootId = System.Guid.Empty;

        public MainForm()
        {
            InitializeComponent();            

            enableRightPanel(false);

            // 初始化RenderControl控件
            IPropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", gviRenderSystem.gviRenderOpenGL);
            this.axRenderControl1.Initialize(true, ps);

            rootId = this.axRenderControl1.ObjectManager.GetProjectTree().RootID;
            this.axRenderControl1.Camera.FlyTime = 1;
            this.axRenderControl1.ObjectManager.GetReferencePlane().PlaneHeight = 900;

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

            #region 加载FDB场景
            try
            {
                IConnectionInfo ci = new ConnectionInfo();
                ci.ConnectionType = gviConnectionType.gviConnectionFireBird2x;
                string tmpFDBPath = @"C:\TestData\FDB\尼泊尔.FDB";
                ci.Database = tmpFDBPath;
                IDataSourceFactory dsFactory = new DataSourceFactory();
                IDataSource ds = dsFactory.OpenDataSource(ci);
                string[] setnames = (string[])ds.GetFeatureDatasetNames();
                if (setnames.Length == 0)
                    return;
                IFeatureDataSet dataset = ds.OpenFeatureDataset(setnames[0]);
                string[] fcnames = (string[])dataset.GetNamesByType(gviDataSetType.gviDataSetFeatureClassTable);
                if (fcnames.Length == 0)
                    return;
                fcMap = new Hashtable(fcnames.Length);
                foreach (string name in fcnames)
                {
                    IFeatureClass fc = dataset.OpenFeatureClass(name);
                    // 找到空间列字段
                    List<string> geoNames = new List<string>();
                    IFieldInfoCollection fieldinfos = fc.GetFields();
                    for (int i = 0; i < fieldinfos.Count; i++)
                    {
                        IFieldInfo fieldinfo = fieldinfos.Get(i);
                        if (null == fieldinfo)
                            continue;
                        IGeometryDef geometryDef = fieldinfo.GeometryDef;
                        if (null == geometryDef)
                            continue;
                        geoNames.Add(fieldinfo.Name);
                    }
                    fcMap.Add(fc, geoNames);
                }
            }
            catch (COMException ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return;
            }

            // CreateFeautureLayer
            bool hasfly = false;
            foreach (IFeatureClass fc in fcMap.Keys)
            {
                List<string> geoNames = (List<string>)fcMap[fc];
                foreach (string geoName in geoNames)
                {
                    if (!geoName.Equals("Geometry"))
                        continue;

                    IFeatureLayer featureLayer = this.axRenderControl1.ObjectManager.CreateFeatureLayer(
                    fc, geoName, null, null, rootId);
                    __fc = fc;

                    if (!hasfly)
                    {
                        IFieldInfoCollection fieldinfos = fc.GetFields();
                        IFieldInfo fieldinfo = fieldinfos.Get(fieldinfos.IndexOf(geoName));
                        IGeometryDef geometryDef = fieldinfo.GeometryDef;
                        env = geometryDef.Envelope;
                        if (env == null || (env.MaxX == 0.0 && env.MaxY == 0.0 && env.MaxZ == 0.0 &&
                            env.MinX == 0.0 && env.MinY == 0.0 && env.MinZ == 0.0))
                            continue;
                        IEulerAngle angle = new EulerAngle();
                        angle.Set(0, -20, 0);
                        IPoint p = (new GeometryFactory()).CreatePoint(gviVertexAttribute.gviVertexAttributeZ);
                        p.SpatialCRS = fc.FeatureDataSet.SpatialReference;
                        p.Position = env.Center;
                        this.axRenderControl1.Camera.LookAt2(p, 1000, angle);
                    }
                    hasfly = true;
                }
            }
            #endregion

            // 注册事件
            this.axRenderControl1.RcObjectEditing += new Gvitech.CityMaker.Controls._IRenderControlEvents_RcObjectEditingEventHandler(axRenderControl1_RcObjectEditing);
            this.axRenderControl1.RcObjectEditFinish += new System.EventHandler(axRenderControl1_RcObjectEditFinish);


            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "EstimateLandslideVolume.html";
            }    
        }



        private void toolStripButtonCreatePolygon_Click(object sender, System.EventArgs e)
        {
            if (currentRenderGeometry != null)
            {
                this.axRenderControl1.ObjectManager.DeleteObject(currentRenderGeometry.Guid);
                currentRenderGeometry = null;
            }
            if (intersectGeo != null)
            {
                this.axRenderControl1.ObjectManager.DeleteObject(intersectGeo.Guid);
                intersectGeo = null;
            }
            this.txtVolume.Text = "";

            currentGeometry = geoFactory.CreateGeometry(gviGeometryType.gviGeometryPolygon, gviVertexAttribute.gviVertexAttributeZ) as IPolygon;
            currentGeometry.SpatialCRS = crs as ISpatialCRS;
            ISurfaceSymbol sf = new SurfaceSymbol();
            sf.Color = 0x55ffff80;
            ICurveSymbol cs = new CurveSymbol();
            cs.Color = 0x55ffff80;
            sf.BoundarySymbol = cs;
            currentRenderGeometry = this.axRenderControl1.ObjectManager.CreateRenderPolygon(currentGeometry as IPolygon, sf, rootId);
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
            this.txtVolume.Enabled = able;
        }

        private void btnOnProcess_Click(object sender, EventArgs e)
        {
            double height = 0.0;
            double sampling = 0.0;
            try
            {
                height = double.Parse(this.numHeight.Value.ToString());
                sampling = double.Parse(this.numSampling.Value.ToString());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("请检查拉升高度和采样密度");
                return;
            }            

            IGeometryConvertor gv = new GeometryConvertor();
            ITriMesh tm = gv.ExtrudePolygonToTriMesh((IPolygon)currentGeometry, height, true);
            IMultiTriMesh mltm = geoFactory.CreateGeometry(gviGeometryType.gviGeometryMultiTrimesh, gviVertexAttribute.gviVertexAttributeZ) as IMultiTriMesh;
            mltm.AddTriMesh(tm);

            IModelPoint mp = null;
            IModel model = null;
            gv.TriMeshToModelPoint(mltm, out model, out mp);

            //model.WriteFile(@"C:\TestData\FDB\a.osg", null);
            //mp.ModelName = @"C:\TestData\FDB\a.osg";
            //mp.ModelEnvelope = model.Envelope;
            //this.axRenderControl1.ObjectManager.CreateRenderModelPoint(mp, null, rootId);

            //IEnvelope env = model.Envelope;
            //IVector3 offset = new Vector3();
            //offset.Set(-mp.X, -mp.Y, -mp.Z);
            //env.ExpandByVector(offset);

            model.MultiplyMatrix(mp.AsMatrix());  //转成绝对坐标      

            //this.axRenderControl1.RefreshModel(null, @"C:\TestData\FDB\slope.osg");
            //model.WriteFile(@"C:\TestData\FDB\slope.osg", null);
            //mp.SetCoords(0, 0, 0, 0, 0);
            //mp.ModelEnvelope = model.Envelope;
            //mp.ModelName = @"C:\TestData\FDB\slope.osg";
            //pullingGeo = this.axRenderControl1.ObjectManager.CreateRenderModelPoint(mp, null, rootId);
            //this.axRenderControl1.Camera.FlyToObject(pullingGeo.Guid, gviActionCode.gviActionFlyTo);

            ITools tools = new Tools();
            double vRet = 0;
            modelRet = tools.EstimateLandslideVolumeTool(__fc, "Geometry", model, sampling, ref vRet);
            modelRet.WriteFile(@"C:\TestData\FDB\ret.osg", null);
            this.txtVolume.Text = vRet.ToString();

            this.axRenderControl1.RefreshModel(null, @"C:\TestData\FDB\ret.osg");
            IModelPoint mp2 = (new GeometryFactory()).CreateGeometry(gviGeometryType.gviGeometryModelPoint, gviVertexAttribute.gviVertexAttributeZ) as IModelPoint;
            mp2.SetCoords(0, 0, 0, 0, 0);
            mp2.ModelEnvelope = modelRet.Envelope;
            mp2.ModelName = @"C:\TestData\FDB\ret.osg";
            intersectGeo = this.axRenderControl1.ObjectManager.CreateRenderModelPoint(mp2, null, rootId);
            this.axRenderControl1.Camera.FlyToObject(intersectGeo.Guid, gviActionCode.gviActionFlyTo);
        }        

    }
}
