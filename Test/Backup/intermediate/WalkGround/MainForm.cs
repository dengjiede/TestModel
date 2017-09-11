using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
//****Gvitech.CityMaker****
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.FdeCore;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.Common;
using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.Resource;

namespace WalkGround
{
    public partial class MainForm : Form
    {
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号
        private Hashtable fcMap = null;  //IFeatureClass, List<string> 存储dataset里featureclass及对应的空间列名        
        private IFeatureClass _featureClass = null;  // 要素类
        private int[] oidarray;

        private System.Guid rootId = new System.Guid();

        public MainForm()
        {
            InitializeComponent();

            // 初始化RenderControl控件
            IPropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", gviRenderSystem.gviRenderOpenGL);
            this.axRenderControl1.Initialize(true, ps);

            rootId = this.axRenderControl1.ObjectManager.GetProjectTree().RootID;
            this.axRenderControl1.Camera.FlyTime = 1;

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

            string tmpTdbPath = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\sdk.tdb");
            I3DTileLayer tilelayer = this.axRenderControl1.ObjectManager.Create3DTileLayer(tmpTdbPath, "", rootId);
            if (tilelayer != null)
                this.axRenderControl1.Camera.FlyToObject(tilelayer.Guid, gviActionCode.gviActionFlyTo);
            else
                this.Text = "tilelayer create failed!";

            /*
            // 从空间列类型为ModelPoint的FeatureClass中创建WalkGround
            IConnectionInfo ci = new ConnectionInfo();
            ci.ConnectionType = gviConnectionType.gviConnectionFireBird2x;
            string tmpFDBPath = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\SDKROAD.FDB");
            ci.Database = tmpFDBPath;
            IDataSourceFactory dsFactory = new DataSourceFactory();
            IDataSource ds = dsFactory.OpenDataSource(ci);
            string[] setnames = (string[])ds.GetFeatureDatasetNames();
            IFeatureDataSet dataset = ds.OpenFeatureDataset(setnames[0]);
            string[] fcnames = dataset.GetNamesByType(gviDataSetType.gviDataSetFeatureClassTable);
            IFeatureClass fc = dataset.OpenFeatureClass(fcnames[0]);
            IWalkGround wg = this.axRenderControl1.ObjectManager.CreateWalkGroundFromFDB(fc, "Geometry");
            // 相机定位至第一条记录处
            IRowBuffer row = fc.GetRow(1);
            int index = row.FieldIndex("Geometry");
            IModelPoint mp = row.GetValue(index) as IModelPoint;
            IEulerAngle angle = new EulerAngle();
            angle.Set(0, -90, 0);
            this.axRenderControl1.Camera.LookAt(mp.Position, 50, angle);
            if (wg == null)
                this.Text = "walk ground create failed!";
            */

            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "WalkGround.html";
            }   
        }

        private void extrudePolygonToWalkGroundToolStripButton_Click(object sender, EventArgs e)
        {
            IConnectionInfo ci = new ConnectionInfo();
            ci.ConnectionType = gviConnectionType.gviConnectionFireBird2x;
            string tmpFDBPath = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\SDKROAD.FDB");
            ci.Database = tmpFDBPath;
            try
            {
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
                    _featureClass = fc;
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

                    //获取oid数组
                    oidarray = new int[_featureClass.GetCount(null)];
                    int cc=0;
                    IFdeCursor cursor = null;
                    cursor = fc.Search(null, false);
                    if (cursor != null)
                    {
                        IRowBuffer fdeRow = null;
                        while ((fdeRow = cursor.NextRow()) != null)
                        {
                            object v = fdeRow.GetValue(0);  // 从库中读取值
                            oidarray[cc] = (int)v;
                            cc++;
                        }
                        Marshal.ReleaseComObject(cursor);
                        cursor = null;
                    }

                    IResourceManager rm = dataset as IResourceManager;
                    foreach (int fid in oidarray)
                    {
                        IRowBuffer rowGC = _featureClass.GetRow(fid);
                        int nPose = rowGC.FieldIndex("Geometry");
                        if (nPose == -1)
                        {
                            MessageBox.Show("不存在Geometry列");
                            break;
                        }
                        
                        IPolygon polygonGC = null;
                        if (rowGC != null)
                        {
                            // 获取polygon
                            nPose = rowGC.FieldIndex("Geometry");
                            IGeometry geo = rowGC.GetValue(nPose) as IGeometry;
                            if (geo.GeometryType == gviGeometryType.gviGeometryPolygon)
                                polygonGC = geo as IPolygon;

                            //PolygonToModelPoint
                            IGeometryConvertor gc = new GeometryConvertor();                           
                            IModelPoint mp = null;
                            IModel model = null;
                            if (!gc.PolygonToModelPoint(polygonGC, out model, out mp))
                            {
                                MessageBox.Show("拉体块出错！");
                                break;
                            }
                            //写到缓存目录
                            IInternalTool tool = this.axRenderControl1.GetOcx() as IInternalTool;
                            string filename = tool.GetRuntimeTempPath() + "\\Gvitech\\" + fid + ".osg";
                            model.WriteFile(filename, null);
                            mp.ModelName = filename;

                            //创建WalkGround
                            IWalkGround wg = this.axRenderControl1.ObjectManager.CreateWalkGround(mp);
                            if (wg == null)
                                MessageBox.Show("fid=" + fid + "创建walkground失败");
                        }
                    }//end foreach
                   
                }
            }
            catch (COMException ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return;
            }

            // CreateFeautureLayer
            //bool hasfly = false;
            //foreach (IFeatureClass fc in fcMap.Keys)
            //{
            //    List<string> geoNames = (List<string>)fcMap[fc];
            //    foreach (string geoName in geoNames)
            //    {
            //        IFeatureLayer featureLayer = this.axRenderControl1.ObjectManager.CreateFeatureLayer(
            //        fc, geoName, null, null, rootId);

            //        IFieldInfoCollection fieldinfos = fc.GetFields();
            //        IFieldInfo fieldinfo = fieldinfos.Get(fieldinfos.IndexOf(geoName));
            //        IGeometryDef geometryDef = fieldinfo.GeometryDef;
            //        IEnvelope env = geometryDef.Envelope;
            //        if (env == null || (env.MaxX == 0.0 && env.MaxY == 0.0 && env.MaxZ == 0.0 &&
            //            env.MinX == 0.0 && env.MinY == 0.0 && env.MinZ == 0.0))
            //            continue;

            //        // 相机飞入
            //        if (!hasfly)
            //        {
            //            IEulerAngle angle = new EulerAngle();
            //            angle.Set(0, -20, 0);
            //            this.axRenderControl1.Camera.LookAt(env.Center, 1000, angle);
            //        }
            //        hasfly = true;
            //    }
            //}           
        }

    }
}
