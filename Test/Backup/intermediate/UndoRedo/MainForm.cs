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
using Gvitech.CityMaker.FdeUndoRedo;
using Gvitech.CityMaker.FdeGeometry;

namespace UndoRedo
{
    public partial class MainForm : Form
    {
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号
        private Hashtable fcMap = null;  //IFeatureClass, List<string> 存储dataset里featureclass及对应的空间列名   
        private Hashtable fcuidMap = null; //FeatureClassUID,FeatureClass
        private int curSelectFid = -1;
        private IFeatureClass curSelectFc = null;
        private CommandManager cmdManager = null;
        private string backUpFile = "";

        private System.Guid rootId = new System.Guid();

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

            // 加载FDB场景
            try
            {
                ILicenseServer license = new LicenseServer();
                license.SetHost("192.168.2.155", 8588, "");

                IConnectionInfo ci = new ConnectionInfo();
                ci.ConnectionType = gviConnectionType.gviConnectionCms7Http;
                ci.Server = "192.168.2.132";
                ci.Port = 8030;
                ci.Database = "cbpdata";
                IDataSourceFactory dsFactory = new DataSourceFactory();
                IDataSource ds = dsFactory.OpenDataSource(ci);
                string[] setnames = (string[])ds.GetFeatureDatasetNames();
                if (setnames.Length == 0)
                    return;
                IFeatureDataSet dataset = ds.OpenFeatureDataset(setnames[0]);

                //因为此处只加载的该要素数据集，故初始化ICommandManager一次即可
                ICommandManagerFactory cmdFac = new CommandManagerFactory();
                backUpFile = Application.StartupPath + "\\" + Guid.NewGuid().ToString() + ".fdb";
                cmdManager = cmdFac.CreateCommandManager(dataset, backUpFile) as CommandManager;

                string[] fcnames = (string[])dataset.GetNamesByType(gviDataSetType.gviDataSetFeatureClassTable);
                if (fcnames.Length == 0)
                    return;
                fcMap = new Hashtable(fcnames.Length);
                fcuidMap = new Hashtable(fcnames.Length);
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
                    fcuidMap.Add(fc.GuidString, fc);
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

            this.axRenderControl1.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectAll;
            this.axRenderControl1.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;
            this.axRenderControl1.RcMouseClickSelect += new Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEventHandler(axRenderControl1_RcMouseClickSelect);

            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "UndoRedo.html";
            }

        }

        void axRenderControl1_RcMouseClickSelect(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEvent e)
        {
            if (e.eventSender == gviMouseSelectMode.gviMouseSelectClick)
            {
                this.axRenderControl1.FeatureManager.UnhighlightAll();
                IPickResult pr = e.pickResult;
                if (pr != null && pr.Type == gviObjectType.gviObjectFeatureLayer)
                {
                    IFeatureLayerPickResult flpr = pr as IFeatureLayerPickResult;
                    curSelectFid = flpr.FeatureId;
                    IFeatureLayer fl = flpr.FeatureLayer;
                    curSelectFc = fcuidMap[fl.FeatureClassId.ToString()] as IFeatureClass;
                    this.axRenderControl1.FeatureManager.HighlightFeature(curSelectFc, curSelectFid, 0xffff0064);
                    this.tsb_Delete.Enabled = true;
                    this.tsb_Update.Enabled = true;
                    this.tsb_Insert.Enabled = true;
                }
                else
                {
                    this.tsb_Delete.Enabled = false;
                    this.tsb_Update.Enabled = false;
                    this.tsb_Insert.Enabled = false;
                }
            }
        }

        private void tsb_Pan_Click(object sender, EventArgs e)
        {
            this.axRenderControl1.InteractMode = gviInteractMode.gviInteractNormal;
        }

        private void tsb_Select_Click(object sender, EventArgs e)
        {
            this.axRenderControl1.InteractMode = gviInteractMode.gviInteractSelect;
        }

        private void tsb_Delete_Click(object sender, EventArgs e)
        {
            this.cmdManager.StartCommand();

            this.cmdManager.DeleteFeature(curSelectFc as IObjectClass, curSelectFid);
            this.axRenderControl1.FeatureManager.DeleteFeature(curSelectFc, curSelectFid);

            this.tsb_Redo.Enabled = this.cmdManager.CanRedo;
            this.tsb_Undo.Enabled = this.cmdManager.CanUndo;
            this.tsb_Delete.Enabled = false;
        }

        private void tsb_Update_Click(object sender, EventArgs e)
        {
            this.cmdManager.StartCommand();

            //获取当前选中要素，将其放大一倍，作为新的行进行更新
            IRowBuffer row = curSelectFc.GetRow(curSelectFid);
            int geoPos = curSelectFc.GetFields().IndexOf("Geometry");
            if (geoPos != -1)
            {
                IModelPoint geo = row.GetValue(geoPos) as IModelPoint;
                geo.SelfScale(2, 2, 2);
                row.SetValue(geoPos, geo);
            }

            this.cmdManager.UpdateFeature(curSelectFc as IObjectClass, row);
            this.axRenderControl1.FeatureManager.EditFeature(curSelectFc, row);

            this.tsb_Redo.Enabled = this.cmdManager.CanRedo;
            this.tsb_Undo.Enabled = this.cmdManager.CanUndo;
            this.tsb_Update.Enabled = false;
        }

        private void tsb_Insert_Click(object sender, EventArgs e)
        {
            this.cmdManager.StartCommand();

            //克隆当前选中要素，将z值提高10米，作为新的行进行插入
            IRowBuffer row = curSelectFc.GetRow(curSelectFid).Clone(false);
            int geoPos = curSelectFc.GetFields().IndexOf("Geometry");
            if (geoPos != -1)
            {
                IModelPoint geo = row.GetValue(geoPos) as IModelPoint;
                geo.Z = geo.Z + 10;
                row.SetValue(geoPos, geo);
            }
            row.SetNull(0);

            this.cmdManager.InsertFeature(curSelectFc as IObjectClass, row);
            this.axRenderControl1.FeatureManager.CreateFeature(curSelectFc, row);

            this.tsb_Redo.Enabled = this.cmdManager.CanRedo;
            this.tsb_Undo.Enabled = this.cmdManager.CanUndo;
            this.tsb_Insert.Enabled = false;
        }

        private void tsb_Undo_Click(object sender, EventArgs e)
        {
            if (this.axRenderControl1.ObjectEditor.IsEditing)
            {
                this.axRenderControl1.ObjectEditor.FinishEdit();
            }
            this.axRenderControl1.PauseRendering(false);
            this.cmdManager.UndoStart +=new _ICommandManagerEvents_UndoStartEventHandler(cmdManager_UndoStart);

            this.cmdManager.Undo();

            this.cmdManager.UndoStart -= new _ICommandManagerEvents_UndoStartEventHandler(cmdManager_UndoStart);
            this.axRenderControl1.ResumeRendering();
            this.tsb_Redo.Enabled = this.cmdManager.CanRedo;
            this.tsb_Undo.Enabled = this.cmdManager.CanUndo;
        }

        private void tsb_Redo_Click(object sender, EventArgs e)
        {
            if (this.axRenderControl1.ObjectEditor.IsEditing)
            {
                this.axRenderControl1.ObjectEditor.FinishEdit();
            }
            this.axRenderControl1.PauseRendering(false);
            this.cmdManager.RedoStart +=new _ICommandManagerEvents_RedoStartEventHandler(cmdManager_RedoStart);

            this.cmdManager.Redo();

            this.cmdManager.RedoStart -= new _ICommandManagerEvents_RedoStartEventHandler(cmdManager_RedoStart);
            this.axRenderControl1.ResumeRendering();
            this.tsb_Redo.Enabled = this.cmdManager.CanRedo;
            this.tsb_Undo.Enabled = this.cmdManager.CanUndo;
        }

        private void cmdManager_UndoStart(IUndoRedoResultCollection Coll)
        {
            ExecuteUndoRedo(Coll);
        }

        private void cmdManager_RedoStart(IUndoRedoResultCollection Coll)
        {
            ExecuteUndoRedo(Coll);
        }

        private void ExecuteUndoRedo(IUndoRedoResultCollection Coll)
        {
            if (Coll != null)
            {
                int nCount = Coll.Count;
                if (nCount == 0)
                    return;
                for (int i = 0; i < nCount; ++i)
                {
                    IUndoRedoResult result = Coll[i] as IUndoRedoResult;
                    if (result == null)
                        continue;
                    IObjectClass oc = result.ObjectClass;
                    IRowBufferCollection Rows = result.RowBuffers;
                    List<int> insert_updata_Ids = new List<int>();
                    if (Rows != null)
                    {
                        for (int j = 0; j < Rows.Count;j ++ )
                        {
                            IRowBuffer row = Rows.Get(j);
                            if (row.IsNull(0))
                                continue;
                            insert_updata_Ids.Add(Convert.ToInt32(row.GetValue(0)));
                        }
                    }

                    gviCommandType type = result.Type;
                    switch (type)
                    {
                        case gviCommandType.gviCommandInsert:  //Insert
                            {
                                this.axRenderControl1.FeatureManager.CreateFeatures(oc as IFeatureClass, Rows);
                                this.axRenderControl1.FeatureManager.UnhighlightAll();
                                this.axRenderControl1.FeatureManager.HighlightFeatures(oc as IFeatureClass, insert_updata_Ids.ToArray(), 0xffff0064);
                            }
                            break;
                        case gviCommandType.gviCommandDelete:  //Delete
                            {
                                int[] OidArray = result.FidArray;
                                this.axRenderControl1.FeatureManager.DeleteFeatures(oc as IFeatureClass, OidArray);
                            }
                            break;
                        case gviCommandType.gviCommandUpdate:  //Update
                            {
                                this.axRenderControl1.FeatureManager.EditFeatures(oc as IFeatureClass, Rows);
                                this.axRenderControl1.FeatureManager.UnhighlightAll();
                                this.axRenderControl1.FeatureManager.HighlightFeatures(oc as IFeatureClass, insert_updata_Ids.ToArray(), 0xffff0064);
                            }
                            break;
                    }
                    if (Rows != null)
                    {
                        Rows.Clear();
                        Marshal.ReleaseComObject(Rows);
                    }
                    Marshal.ReleaseComObject(oc);
                    Marshal.ReleaseComObject(result);
                }
                Marshal.ReleaseComObject(Coll);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (cmdManager != null)
            {
                Marshal.ReleaseComObject(cmdManager);
                cmdManager = null;
            }
            try
            {
                File.Delete(backUpFile);
            }
            catch { }
        }

    }
}
