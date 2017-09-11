// Copyright 2012 CityMaker SDK
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
//author	yuanying
//date	2013/05/28
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
using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.Common;
using Gvitech;
using System.Drawing;

namespace HighlightHelper
{
    public enum HelperType 
    {
        NULL,
        CircleRegion,
        SectorRegion,
        PolygonRegion
    }

    public partial class MainForm : Form
    {
        RenderControl g = null;
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号
        private Hashtable fcMap = null;  //IFeatureClass, List<string> 存储dataset里featureclass及对应的空间列名

        private bool isDrawing = false;
        private int mouseClicks = 0;

        private HelperType type = HelperType.NULL;

        private IGeometryFactory geoFactory = new GeometryFactory();
        private IPoint fde_point1 = null;
        private IPoint fde_point2 = null;
        private IPolygon polygon = null;
        private IRenderPolygon rpolygon = null;

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
                this.helpProvider1.HelpNamespace = "HighlightHelper.html";
            }
        }

        public MainForm()
        {
            InitializeComponent();
            init();

            this.colorBox.Text = g.HighlightHelper.Color.ToString("X");
            this.trackBarOpacity.Value = Utils.HexNumberToColor(g.HighlightHelper.Color.ToString("X")).A;
            this.numMaxZ.Value = (decimal)g.HighlightHelper.MaxZ;
            this.numMinZ.Value = (decimal)g.HighlightHelper.MinZ;
            g.HighlightHelper.VisibleMask = 1;  
        }

        /// <summary>
        /// 设置hightlight的color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangeColor_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = Utils.HexNumberToColor(colorBox.Text);
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                uint olec = (uint)(this.colorDialog1.Color.A << 24 | this.colorDialog1.Color.R << 16 | this.colorDialog1.Color.G << 8 | this.colorDialog1.Color.B);
                this.colorBox.Text = olec.ToString("X");
                g.HighlightHelper.Color = olec;
            }
        }

        /// <summary>
        /// 设置颜色透明度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarOpacity_Scroll(object sender, EventArgs e)
        {
            int a = this.trackBarOpacity.Value;
            Color highlightHelperColor = Utils.HexNumberToColor(g.HighlightHelper.Color.ToString("X"));
            g.HighlightHelper.Color = Utils.ToArgb(a, highlightHelperColor.R, highlightHelperColor.G, highlightHelperColor.B);
            this.colorBox.Text = g.HighlightHelper.Color.ToString("X");
        }

        /// <summary>
        /// 画圆区
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetCircleRegion_Click(object sender, EventArgs e)
        {
            type = HelperType.CircleRegion;

            this.label7.Text = "请鼠标点击选择圆心";
            this.btnSetCircleRegion.Enabled = false;
            this.btnSetSectorRegion.Enabled = false;
            this.btnSetRegion.Enabled = false;
            isDrawing = true;

            g.HighlightHelper.SetRegion(null);  //清空之前的高亮区

            g.InteractMode = gviInteractMode.gviInteractSelect;
            g.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectAll;
            g.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;
            g.RcMouseClickSelect += new _IRenderControlEvents_RcMouseClickSelectEventHandler(g_RcMouseClickSelect);
        }

        /// <summary>
        /// 画扇区 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetSectorRegion_Click(object sender, EventArgs e)
        {
            type = HelperType.SectorRegion;

            this.label7.Text = "请鼠标点击选择观察点";
            this.btnSetCircleRegion.Enabled = false;
            this.btnSetSectorRegion.Enabled = false;
            this.btnSetRegion.Enabled = false;
            isDrawing = true;

            g.HighlightHelper.SetRegion(null);  //清空之前的高亮区

            g.InteractMode = gviInteractMode.gviInteractSelect;
            g.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectAll;
            g.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;
            g.RcMouseClickSelect += new _IRenderControlEvents_RcMouseClickSelectEventHandler(g_RcMouseClickSelect);
        }

        /// <summary>
        /// 设置选择区域。
        /// 目前只支持IPolygon，后面会支持ClosedTrimesh和IMultiPolygon。
        /// 允许传入null，相当于把region清空，清空之后即使VisibleMask还是1也没有任何高亮区域。 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetRegion_Click(object sender, EventArgs e)
        {
            type = HelperType.PolygonRegion;

            this.label7.Text = "请鼠标点击画Polygon";
            this.btnSetCircleRegion.Enabled = false;
            this.btnSetSectorRegion.Enabled = false;
            this.btnSetRegion.Enabled = false;
            isDrawing = true;

            g.HighlightHelper.SetRegion(null);  //清空之前的高亮区

            g.InteractMode = gviInteractMode.gviInteractEdit;
            g.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectAll;
            g.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;
            g.RcObjectEditing += new _IRenderControlEvents_RcObjectEditingEventHandler(g_RcObjectEditing);
            g.RcObjectEditFinish += new _IRenderControlEvents_RcObjectEditFinishEventHandler(g_RcObjectEditFinish);

            if (rpolygon != null)
                g.ObjectManager.DeleteObject(rpolygon.Guid);

            polygon = geoFactory.CreateGeometry(gviGeometryType.gviGeometryPolygon, gviVertexAttribute.gviVertexAttributeZ) as IPolygon;
            rpolygon = g.ObjectManager.CreateRenderPolygon(polygon, null, rootId);
            g.ObjectEditor.StartEditRenderGeometry(rpolygon, gviGeoEditType.gviGeoEditCreator);                   
        }

        /// <summary>
        /// 鼠标点击
        /// </summary>
        /// <param name="PickResult"></param>
        /// <param name="IntersectPoint"></param>
        /// <param name="Mask"></param>
        /// <param name="EventSender"></param>
        void g_RcMouseClickSelect(IPickResult PickResult, IPoint IntersectPoint, gviModKeyMask Mask, gviMouseSelectMode EventSender)
        {
            if (IntersectPoint == null)
                return;
            if (isDrawing == true)
            {
                if (EventSender.Equals(gviMouseSelectMode.gviMouseSelectClick))
                {
                    mouseClicks++;
                    if (mouseClicks % 2 == 1)
                    {
                        switch (type) 
                        {
                            case HelperType.SectorRegion:
                                {
                                    this.label7.Text = "请鼠标点击选择目标点";
                                    fde_point1 = IntersectPoint;
                                    g.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick | gviMouseSelectMode.gviMouseSelectMove;
                                }
                                break;
                            case HelperType.CircleRegion:
                                {
                                    this.label7.Text = "请鼠标点击选择最外环点";
                                    fde_point1 = IntersectPoint;
                                    g.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick | gviMouseSelectMode.gviMouseSelectMove;
                                }
                                break;
                        }                        
                    }
                    else
                    {
                        isDrawing = false;
                        this.label7.Text = "画结束";
                        this.btnSetSectorRegion.Enabled = true;
                        this.btnSetCircleRegion.Enabled = true;
                        this.btnSetRegion.Enabled = true;

                        g.InteractMode = gviInteractMode.gviInteractNormal;
                        g.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectNone;
                        g.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;
                        g.RcMouseClickSelect -= new _IRenderControlEvents_RcMouseClickSelectEventHandler(g_RcMouseClickSelect);
                    }
                }
                else if (EventSender.Equals(gviMouseSelectMode.gviMouseSelectMove))
                {
                    switch (type)
                    {
                        case HelperType.SectorRegion:
                            {
                                fde_point2 = IntersectPoint;
                                g.HighlightHelper.SetSectorRegion(fde_point1, fde_point2, double.Parse(this.numHorizontalAngle.Value.ToString()));
                            }
                            break;
                        case HelperType.CircleRegion:
                            {
                                fde_point2 = IntersectPoint;
                                ILine fde_line = geoFactory.CreateGeometry(gviGeometryType.gviGeometryLine,
                                    gviVertexAttribute.gviVertexAttributeZ) as ILine;
                                fde_line.StartPoint = fde_point1;
                                fde_line.EndPoint = fde_point2;
                                this.centerRadius.Text = fde_line.Length.ToString();
                                g.HighlightHelper.SetCircleRegion(fde_point1, fde_line.Length);
                            }
                            break;
                    }
                }
            }
        }

        void g_RcObjectEditFinish()
        {
            isDrawing = false;
            this.label7.Text = "画结束";
            this.btnSetSectorRegion.Enabled = true;
            this.btnSetCircleRegion.Enabled = true;
            this.btnSetRegion.Enabled = true;

            g.InteractMode = gviInteractMode.gviInteractNormal;
            g.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectNone;
            g.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;

            g.RcObjectEditing -= new _IRenderControlEvents_RcObjectEditingEventHandler(g_RcObjectEditing);
            g.RcObjectEditFinish -= new _IRenderControlEvents_RcObjectEditFinishEventHandler(g_RcObjectEditFinish);
        }

        void g_RcObjectEditing(IGeometry geo)
        {
            g.HighlightHelper.SetRegion(geo);
        }

        /// <summary>
        /// 改变MinZ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numMinZ_ValueChanged(object sender, EventArgs e)
        {
            g.HighlightHelper.MinZ = double.Parse(this.numMinZ.Value.ToString());
        }

        /// <summary>
        /// 改变MaxZ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numMaxZ_ValueChanged(object sender, EventArgs e)
        {
            g.HighlightHelper.MaxZ = double.Parse(this.numMaxZ.Value.ToString());
        }

    }


}
