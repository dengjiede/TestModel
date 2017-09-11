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

namespace ClipPlane
{
    public partial class MainForm : Form
    {
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号
        private Hashtable fcMap = null;  //IFeatureClass, List<string> 存储dataset里featureclass及对应的空间列名

        private System.Guid rootId = new System.Guid();
        
        private IClipPlaneOperation operation = null;
        private IVector3 boxCenter = new Vector3();
        private IVector3 boxSize = new Vector3();
        private IVector3 position = new Vector3();
        private IEulerAngle angle = new EulerAngle();

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

            // 加载FDB场景
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
                        this.axRenderControl1.Camera.LookAt(env.Center, 1000, angle);
                    }
                    hasfly = true;
                }
            }

            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "FeatureSelect.html";
            }    

            // 注册控件拾取事件
            this.axRenderControl1.RcMouseClickSelect += new Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEventHandler(axRenderControl1_RcMouseClickSelect);

            // 设置控件默认值
            this.toolStripInteractModeSetting.SelectedIndex = 0;
            this.toolStripLineColorComboBox.SelectedIndex = 0;
            this.toolStripClipModeSetting.SelectedIndex = 0;
            this.axRenderControl1.InteractMode = gviInteractMode.gviInteractClipPlane;
        }

        #region RenderControl事件
        void axRenderControl1_RcMouseClickSelect(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEvent e)
        {
            IPickResult pr = e.pickResult;
            if (pr == null)
            {
                this.axRenderControl1.FeatureManager.UnhighlightAll();
                return;
            }

            if (e.eventSender == gviMouseSelectMode.gviMouseSelectClick)
            {
                if (e.pickResult != null)
                {
                    if (pr.Type == gviObjectType.gviObjectFeatureLayer)
                    {
                        IFeatureLayerPickResult flpr = pr as IFeatureLayerPickResult;
                        int fid = flpr.FeatureId;
                        IFeatureLayer fl = flpr.FeatureLayer;

                        foreach (IFeatureClass fc in fcMap.Keys)
                        {
                            if (fc.Guid.Equals(fl.FeatureClassId))
                            {
                                if (cbClipPlaneEnable.Checked)
                                {
                                    fl.AttributeMask = gviAttributeMask.gviAttributeClipPlane | gviAttributeMask.gviAttributeCollision | gviAttributeMask.gviAttributeHighlight;
                                    MessageBox.Show(string.Format("名称为{0}的FeatureLayer的AttributeMask已设置为参与裁剪", fc.Name));
                                }
                                else
                                {
                                    fl.AttributeMask = gviAttributeMask.gviAttributeCollision | gviAttributeMask.gviAttributeHighlight;
                                    MessageBox.Show(string.Format("名称为{0}的FeatureLayer的AttributeMask已设置为不参与裁剪", fc.Name));
                                }
                                break;
                            }
                        }                                               
                    }
                }
            }
        }
        #endregion

        private void toolStripInteractModeSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox cb = (sender as ToolStripComboBox);
            switch (cb.Text)
            {
                case "裁剪面交互模式":
                    {
                        this.axRenderControl1.InteractMode = gviInteractMode.gviInteractClipPlane;
                        this.toolStripClipModeSetting.Enabled = true;
                        //this.toolStripClipModeSetting.SelectedIndex = 0;
                    }
                    break;
                case "鼠标拾取模式":
                    {
                        this.axRenderControl1.InteractMode = gviInteractMode.gviInteractSelect;
                        this.axRenderControl1.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;
                        this.axRenderControl1.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectFeatureLayer;
                        this.toolStripClipModeSetting.Enabled = false;
                    }
                    break;
                case "普通漫游模式":
                    {
                        this.axRenderControl1.InteractMode = gviInteractMode.gviInteractNormal;
                        this.toolStripClipModeSetting.Enabled = false;
                    }
                    break;               
            }
        }

        private void toolStripLineColorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox cb = (sender as ToolStripComboBox);
            switch (cb.Text)
            {
                case "白色":
                    {
                        this.axRenderControl1.SetRenderParam(gviRenderControlParameters.gviRenderParamClipPlaneLineColor, 0xffffffff);
                    }
                    break;
                case "红色":
                    {
                        this.axRenderControl1.SetRenderParam(gviRenderControlParameters.gviRenderParamClipPlaneLineColor, 0xffff0000);
                    }
                    break;
                case "黄色":
                    {
                        this.axRenderControl1.SetRenderParam(gviRenderControlParameters.gviRenderParamClipPlaneLineColor, 0xffffff00);
                    }
                    break;
                case "蓝色":
                    {
                        this.axRenderControl1.SetRenderParam(gviRenderControlParameters.gviRenderParamClipPlaneLineColor, 0xff0000ff);
                    }
                    break;
            }
        }

        private void toolStripClipModeSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox cb = (sender as ToolStripComboBox);
            switch (cb.Text)
            {
                case "任意面":
                    {
                        this.axRenderControl1.ClipMode = gviClipMode.gviClipCustomePlane;
                    }
                    break;
                case "长方体":
                    {
                        this.axRenderControl1.ClipMode = gviClipMode.gviClipBox;
                    }
                    break;                
            }
        }

        private void btnCreateClipPlaneOperation_Click(object sender, EventArgs e)
        {
            if (operation != null)
                this.axRenderControl1.ObjectManager.DeleteObject(operation.Guid);

            clear();
            operation = this.axRenderControl1.ObjectManager.CreateClipPlaneOperation(rootId);
            if (operation.ClipPlaneOperationType == gviClipPlaneOperation.gviBoxClipOperation)
            {
                this.textBoxPositionX.Enabled = false;
                this.textBoxPositionY.Enabled = false;
                this.textBoxPositionZ.Enabled = false;

                operation.GetBoxClip(out boxCenter, out boxSize, out angle);
                this.textBoxBoxCenterX.Text = boxCenter.X.ToString();
                this.textBoxBoxCenterY.Text = boxCenter.Y.ToString();
                this.textBoxBoxCenterZ.Text = boxCenter.Z.ToString();
                this.textBoxBoxSizeX.Text = boxSize.X.ToString();
                this.textBoxBoxSizeY.Text = boxSize.Y.ToString();
                this.textBoxBoxSizeZ.Text = boxSize.Z.ToString();
                this.textBoxAngleHeading.Text = angle.Heading.ToString();
                this.textBoxAngleRoll.Text = angle.Roll.ToString();
                this.textBoxAngleTilt.Text = angle.Tilt.ToString();
            }
            else
            {
                this.textBoxBoxCenterX.Enabled = false;
                this.textBoxBoxCenterY.Enabled = false;
                this.textBoxBoxCenterZ.Enabled = false;
                this.textBoxBoxSizeX.Enabled = false;
                this.textBoxBoxSizeY.Enabled = false;
                this.textBoxBoxSizeZ.Enabled = false;

                operation.GetSingleClip(out position, out angle);
                this.textBoxPositionX.Text = position.X.ToString();
                this.textBoxPositionY.Text = position.Y.ToString();
                this.textBoxPositionZ.Text = position.Z.ToString();
                this.textBoxAngleHeading.Text = angle.Heading.ToString();
                this.textBoxAngleRoll.Text = angle.Roll.ToString();
                this.textBoxAngleTilt.Text = angle.Tilt.ToString();
            }
        }

        void clear()
        {
            this.textBoxBoxCenterX.Text = "";
            this.textBoxBoxCenterY.Text = "";
            this.textBoxBoxCenterZ.Text = "";
            this.textBoxBoxSizeX.Text = "";
            this.textBoxBoxSizeY.Text = "";
            this.textBoxBoxSizeZ.Text = "";
            this.textBoxAngleHeading.Text = "";
            this.textBoxAngleRoll.Text = "";
            this.textBoxAngleTilt.Text = "";
            this.textBoxPositionX.Text = "";
            this.textBoxPositionY.Text = "";
            this.textBoxPositionZ.Text = "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (operation == null)
            {
                MessageBox.Show("没有当前操作的operation对象");
                return;
            }

            if (operation.ClipPlaneOperationType == gviClipPlaneOperation.gviBoxClipOperation)
            {
                boxCenter.Set(double.Parse(this.textBoxBoxCenterX.Text), double.Parse(this.textBoxBoxCenterY.Text), double.Parse(this.textBoxBoxCenterZ.Text));
                boxSize.Set(double.Parse(this.textBoxBoxSizeX.Text), double.Parse(this.textBoxBoxSizeY.Text), double.Parse(this.textBoxBoxSizeZ.Text));
                angle.Set(double.Parse(this.textBoxAngleHeading.Text), double.Parse(this.textBoxAngleTilt.Text), double.Parse(this.textBoxAngleRoll.Text));
                operation.SetBoxClip(boxCenter, boxSize, angle);
            }
            else
            {
                position.Set(double.Parse(this.textBoxPositionX.Text), double.Parse(this.textBoxPositionY.Text), double.Parse(this.textBoxPositionZ.Text));
                angle.Set(double.Parse(this.textBoxAngleHeading.Text), double.Parse(this.textBoxAngleTilt.Text), double.Parse(this.textBoxAngleRoll.Text));
                operation.SetSingleClip(position, angle);
            }
            operation.Execute();
        }

    }
}
