﻿// Copyright 2012 CityMaker SDK
// 
// All rights reserved under the copyright laws of the China
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See Sample at <your CityMaker install location>/CityMaker SDK/Samples.
// 
//author	gs
//date	2011/09/26
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
//****Gvitech.CityMaker****
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.FdeCore;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.Common;
using Gvitech.CityMaker.Resource;

namespace SightlineAnalysis
{
    public partial class MainForm : Form
    {
        RenderControl g = null;
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号
        private Hashtable fcMap = null;  //IFeatureClass, List<string> 存储dataset里featureclass及对应的空间列名

        private bool flagx = false;
        private IPolyline polyline = null;
        private IRenderPolyline renderPolyline = null;
        private IGeometryFactory geoFactory = null;

        private Dictionary<string, RowObject> rowMap = new Dictionary<string, RowObject>();

        private System.Guid rootId = new System.Guid();

        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            // 初始化RenderControl控件
            IPropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", gviRenderSystem.gviRenderOpenGL);
            this.axRenderControl1.Initialize(true, ps);
            g = this.axRenderControl1.GetOcx() as RenderControl;

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

            #region 加载FDB场景
            try
            {
                IConnectionInfo ci = new ConnectionInfo();
                ci.ConnectionType = gviConnectionType.gviConnectionFireBird2x;
                string tmpFDBPath = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\SDKDEMO.FDB");
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

                    if (!hasfly)
                    {
                        IFieldInfoCollection fieldinfos = fc.GetFields();
                        IFieldInfo fieldinfo = fieldinfos.Get(fieldinfos.IndexOf(geoName));
                        IGeometryDef geometryDef = fieldinfo.GeometryDef;
                        IEnvelope env = geometryDef.Envelope;
                        if (env == null || (env.MaxX == 0.0 && env.MaxY == 0.0 && env.MaxZ == 0.0 &&
                            env.MinX == 0.0 && env.MinY == 0.0 && env.MinZ == 0.0))
                            continue;
                        IEulerAngle angle = new EulerAngle();
                        angle.Set(0, -20, 0);
                        this.axRenderControl1.Camera.LookAt(env.Center, 500, angle);
                    }
                    hasfly = true;
                }
            }
            #endregion 加载FDB场景
            
            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "SightlineAnalysis.html";
            }

            this.btnFlyToSourcePoint.Enabled = false;
            this.btnFlyToTargetPoint.Enabled = false;
        }

        public MainForm()
        {
            InitializeComponent();
            init();
        }

        #region 构造线段
        /// <summary>
        /// 开始绘制线段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_createLine_Click(object sender, EventArgs e)
        {
            this.label7.Text = "请在三维窗口中点两个点构造一条线";
            g.FeatureManager.UnhighlightAll();
            flagx = true;
            if (renderPolyline != null)
            {
                g.ObjectManager.DeleteObject(renderPolyline.Guid);
                renderPolyline = null;
            }
            if (polyline != null)
            {
                polyline = null;
            }
            if (geoFactory == null)
            {
                geoFactory = new GeometryFactory();
            }
            if (polyline == null)
            {
                polyline = (IPolyline)geoFactory.CreateGeometry(gviGeometryType.gviGeometryPolyline, gviVertexAttribute.gviVertexAttributeZ);
            }
            
            g.InteractMode = gviInteractMode.gviInteractSelect;
            g.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectAll;
            g.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;
            g.RcMouseClickSelect += new _IRenderControlEvents_RcMouseClickSelectEventHandler(g_RcMouseClickSelect);
        }

        /// <summary>
        /// 鼠标点击 拾取线段点
        /// </summary>
        /// <param name="PickResult"></param>
        /// <param name="IntersectPoint"></param>
        /// <param name="Mask"></param>
        /// <param name="EventSender"></param>
        void g_RcMouseClickSelect(IPickResult PickResult, IPoint IntersectPoint, gviModKeyMask Mask, gviMouseSelectMode EventSender)
        {
            if (IntersectPoint == null)
                return;

            if (renderPolyline == null)
            {
                renderPolyline = g.ObjectManager.CreateRenderPolyline(polyline, null, rootId);
            }

            if (polyline.PointCount < 2)
            {
                polyline.AppendPoint(IntersectPoint);
            }
            if (polyline.PointCount == 2)
            {
                g.InteractMode = gviInteractMode.gviInteractNormal;
                g.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectNone;
                g.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;
                g.RcMouseClickSelect -= new _IRenderControlEvents_RcMouseClickSelectEventHandler(g_RcMouseClickSelect);
                IPoint p = polyline.StartPoint;
                this.startX.Text = p.X.ToString();
                this.startY.Text = p.Y.ToString();
                this.startZ.Text = p.Z.ToString();
                p = polyline.EndPoint;
                this.endX.Text = p.X.ToString();
                this.endY.Text = p.Y.ToString();
                this.endZ.Text = p.Z.ToString();
                flagx = false;
                this.label7.Text = "线构造成功!请点击“通视分析”按钮";
                this.btnFlyToSourcePoint.Enabled = true;
                this.btnFlyToTargetPoint.Enabled = true;
            }
            renderPolyline.SetFdeGeometry(polyline);
        }
        #endregion

        #region 通视分析核心功能
        private void btn_Analize_Click(object sender, EventArgs e)
        {
            if (polyline != null)
            {
                SelectFeaturesFromBaseLyr(polyline);
            }
        }

        public void SelectFeaturesFromBaseLyr(IPolyline polyLine)
        {
            btn_analyse.Enabled = false;
            btn_createLine.Enabled = false;
            this.btnFlyToSourcePoint.Enabled = false;
            this.btnFlyToTargetPoint.Enabled = false;

            IFdeCursor cursor = null;
            try
            {              
                this.dataGridView1.Rows.Clear();
                g.FeatureManager.UnhighlightAll();
                
                IRowBuffer row = null;
                int index = 0;
                List<IRowBuffer> list = new List<IRowBuffer>();
                this.Text = "开始粗查询";
                foreach (IFeatureClass fc in fcMap.Keys)
                {
                    ISpatialFilter filter = new SpatialFilterClass();
                    filter.Geometry = polyline;
                    filter.SpatialRel = gviSpatialRel.gviSpatialRelEnvelope;
                    filter.GeometryField = "Geometry";
                    cursor = fc.Search(filter, false);
                    while ((row = cursor.NextRow()) != null)
                    {
                        list.Add(row);
                    }
                    this.Text = "开始细查询";
                    foreach (IRowBuffer r in list)
                    {
                        index++;
                        int geometryIndex = -1;
                        geometryIndex = r.FieldIndex("Geometry");
                        if (geometryIndex != -1)
                        {
                            IModelPoint modelPoint = r.GetValue(geometryIndex) as IModelPoint;
                            IModel model = (fc.FeatureDataSet as IResourceManager).GetModel(modelPoint.ModelName);
                            IGeometryConvertor gc = new GeometryConvertor();
                            this.Text = "正在计算第" + index.ToString() + "个IMultiTriMesh是否与线相交";
                            IMultiTriMesh triMesh = gc.ModelPointToTriMesh(model, modelPoint, false);

                            if (triMesh != null)
                            {
                                ILine l = geoFactory.CreateGeometry(gviGeometryType.gviGeometryLine, gviVertexAttribute.gviVertexAttributeZ) as ILine;
                                l.StartPoint = polyline.StartPoint;
                                l.EndPoint = polyline.EndPoint;
                                IVector3 v3 = triMesh.LineSegmentIntersect(l);
                                if (v3 != null)
                                {
                                    string fid = "";
                                    string fName = "";
                                    string groupId = "";
                                    IEnvelope env = null;
                                    int fidPos = r.FieldIndex(fc.FidFieldName);
                                    g.FeatureManager.HighlightFeature(fc, int.Parse(r.GetValue(fidPos).ToString()), 0xffff00ff);
                                    for (int i = 0; i < r.FieldCount; i++)
                                    {
                                        string fieldName = r.Fields.Get(i).Name;
                                        if (r.Fields.Get(i).Name == "oid")
                                        {
                                            fid = r.GetValue(i).ToString();
                                        }
                                        else if (r.Fields.Get(i).Name == "groupid")
                                        {
                                            groupId = r.GetValue(i).ToString();
                                        }
                                        else if (r.Fields.Get(i).Name == "Name")
                                        {
                                            fName = r.GetValue(i).ToString();
                                        }
                                        else if (r.Fields.Get(i).Name == "Geometry")
                                        {
                                            IGeometry geometry = r.GetValue(i) as IModelPoint;
                                            env = geometry.Envelope;
                                        }
                                    }
                                    RowObject ro = new RowObject() { FID = fid, GroupId = groupId, Name = fName, FeatureClass = fc, Envelop = env };
                                    if (!rowMap.ContainsKey(ro.FID))
                                    {
                                        rowMap.Add(ro.FID, ro);
                                    }
                                }
                            }
                        }
                    } // end of foreach (IRowBuffer r in list)
                } // end of foreach (IFeatureClass fc in fcMap.Keys)
                this.Text = "通视分析完成！";
                LoadGridView();
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name.Equals("UnauthorizedAccessException"))
                    MessageBox.Show("需要标准runtime授权");
                else
                    MessageBox.Show(ex.Message);
            }
            finally
            {
                this.btn_analyse.Enabled = true;
                this.btn_createLine.Enabled = true;
                this.btnFlyToSourcePoint.Enabled = true;
                this.btnFlyToTargetPoint.Enabled = true;

                if (cursor != null)
                {
                    Marshal.ReleaseComObject(cursor);
                    cursor = null;
               }
            }
        }

        private void LoadGridView()
        {
            this.label7.Text = string.Format("共有{0}个障碍物", rowMap.Count);
            this.dataGridView1.Rows.Clear();
            if (rowMap != null)
            {
                this.dataGridView1.RowCount = rowMap.Count;
                for (int i = 0; i < rowMap.Count; i++)
                {
                    this.dataGridView1.Rows[i].Cells["FID"].Value = rowMap.Values.ToArray()[i].FID;
                    this.dataGridView1.Rows[i].Cells["FID"].Tag = rowMap.Values.ToArray()[i].FeatureClass;
                    this.dataGridView1.Rows[i].Cells["NameColumn"].Value = rowMap.Values.ToArray()[i].Name;
                    this.dataGridView1.Rows[i].Cells["GroupID"].Value = rowMap.Values.ToArray()[i].GroupId;
                }
            }
        }
        #endregion
              
       
        private void startX_TextChanged(object sender, EventArgs e)
        {
            if (polyline == null || renderPolyline == null)
            {
                return;
            }
            else
            {
                if (flagx == false)
                {
                    IPoint fde_point = geoFactory.CreateGeometry(gviGeometryType.gviGeometryPoint,
                           gviVertexAttribute.gviVertexAttributeZ) as IPoint;
                    fde_point.SetCoords(double.Parse(this.startX.Text), double.Parse(this.startY.Text), double.Parse(this.startZ.Text), 0, 0);
                    polyline.UpdatePoint(0, fde_point);

                    fde_point = geoFactory.CreateGeometry(gviGeometryType.gviGeometryPoint,
                           gviVertexAttribute.gviVertexAttributeZ) as IPoint;
                    fde_point.SetCoords(double.Parse(this.endX.Text), double.Parse(this.endY.Text), double.Parse(this.endZ.Text), 0, 0);
                    polyline.UpdatePoint(1, fde_point);
                    renderPolyline.SetFdeGeometry(polyline);
                }
            }
        }

        /// <summary>
        /// 单击表格记录进行变色和定位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            foreach (string key in rowMap.Keys)
            {
                RowObject r = rowMap[key];
                int fid = int.Parse(r.FID);
                IFeatureClass fc = r.FeatureClass as IFeatureClass;
                g.FeatureManager.HighlightFeature(fc, fid, 0xffff00ff);
            }
            DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
            if (row != null)
            {
                string fid = row.Cells["FID"].Value.ToString();
                IFeatureClass fc = row.Cells["FID"].Tag as IFeatureClass;
                g.FeatureManager.HighlightFeature(fc, int.Parse(fid), 0xff0000ff);
                IEnvelope evn = rowMap[fid].Envelop;
                IEulerAngle angle = new EulerAngle();
                angle.Set(0, -30, 0);
                g.Camera.LookAt(evn.Center, 40, angle);
            }
        }

        /// <summary>
        /// 飞入观察点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlyToSourcePoint_Click(object sender, EventArgs e)
        {
            if (this.startX.Text != "" && this.startY.Text != "" && this.startZ.Text != ""
                    && this.endX.Text != "" && this.endY.Text != "" && this.endZ.Text != "")
            {                
                IVector3 position1 = new Vector3();
                position1.Set(double.Parse(startX.Text), double.Parse(startY.Text), double.Parse(startZ.Text));
                IVector3 position2 = new Vector3();
                position2.Set(double.Parse(endX.Text), double.Parse(endY.Text), double.Parse(endZ.Text));
                IEulerAngle angle = new EulerAngle();
                angle = g.Camera.GetAimingAngles(position1, position2);
                g.Camera.LookAt(position1, 0, angle);
            }
        }

        /// <summary>
        /// 飞入目标点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlyToTargetPoint_Click(object sender, EventArgs e)
        {
            if (this.startX.Text != "" && this.startY.Text != "" && this.startZ.Text != ""
                    && this.endX.Text != "" && this.endY.Text != "" && this.endZ.Text != "")
            {
                IVector3 position1 = new Vector3();
                position1.Set(double.Parse(endX.Text), double.Parse(endY.Text), double.Parse(endZ.Text));
                IVector3 position2 = new Vector3();
                position2.Set(double.Parse(startX.Text), double.Parse(startY.Text), double.Parse(startZ.Text));
                IEulerAngle angle = new EulerAngle();
                angle = g.Camera.GetAimingAngles(position1, position2);
                g.Camera.LookAt(position1, 0, angle);
            }
        }
    }

    class LogicLayerNodeInfo
    {
        public int layerId;
        public string layerName;

        public LogicLayerNodeInfo(int layerId, string layerName)
        {
            this.layerId = layerId;
            this.layerName = layerName;
        }
    }

    class RowObject
    {
        public string FID
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string GroupId
        {
            get;
            set;
        }
        public object FeatureClass
        {
            get;
            set;
        }
        public IEnvelope Envelop
        {
            get;
            set;
        }
    }
}
