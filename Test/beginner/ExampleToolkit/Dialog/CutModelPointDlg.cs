using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Runtime.InteropServices;

using Gvitech.CityMaker.FdeCore;

using CityMakerBuilder.WorkSpace;
using DataInteropCommon.Dialog;
using DataInteropCommon.PublicClass;
using DevExpress.XtraTreeList.Nodes;
using Gvitech.CityMaker.FdeGeometry;
using System.IO;
using ExampleToolkit.ProcessClass;
using CityMakerBuilder.AddIn.WinForm;



namespace ExampleToolkit.Dialog
{
    public partial class CutModelPointDlg : ToolKitCommon.Dialog.ToolKitParentDlg
    {
        private IFeatureClass shpFc = null;
        private int polygonIndex = -1;
        private IMultiPolygon multiPolygon = null;

        //被拖入的目标数据集目录树节点
        private TreeListNode _DragTargetCatalogNode = null;

        //目标数据是否为拖拽数据
        private bool _isDraggedTargetData = false;



        public CutModelPointDlg()
        {
            InitializeComponent();

            this._toolHelpContent = "\n    用矢量数据切割模型\n\n    用polygon类型的矢量切割模型，计算结果存储在新的目标数据集内。";
        }

        private void te_InputData_DragDrop(object sender, DragEventArgs e)
        {
            TreeListNode treeNode = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
            if (treeNode == null)
                return;

            TreeNodeData nodeData = CatalogTreeServices.GetNodeData(treeNode);
            if (nodeData == null)
                return;

            if (nodeData.NodeType == TreeNodeType.NT_DATASOURCE)
            {
                List<ToolKitDataSourceNodeData> dsLst = GetDataSourceItemsFromTreeNode(nodeData);

                if (dsLst.Count != 1)
                    return;
                //弹出要素选择对话框
                FeatureDataSetSelectDlg mySelectFrm = new FeatureDataSetSelectDlg(dsLst, true, false);
                if (mySelectFrm.ShowDialog(this) == DialogResult.OK)
                {
                    if (mySelectFrm.SelectFdsItems.Count != 1)
                        return;
                    ToolKitFeatureDataSetNodeData oneItem = mySelectFrm.SelectFdsItems[0];
                    te_InputData.Text = oneItem.ToString();
                    te_InputData.Tag = oneItem;
                }
            }
            if (nodeData.NodeType == TreeNodeType.NT_DATASET)
            {
                ToolKitFeatureDataSetNodeData myItem = GetDataSetItemFromTreeNode(nodeData);
                if (myItem == null)
                    return;
                te_InputData.Text = myItem.ToString();
                te_InputData.Tag = myItem;
            }           
        }

        private void te_InputData_DragEnter(object sender, DragEventArgs e)
        {
            TreeListNode treeNode = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
            if (treeNode == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            TreeNodeData nodeData = CatalogTreeServices.GetNodeData(treeNode);
            if (nodeData == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            if (nodeData.NodeType != TreeNodeType.NT_DATASOURCE && nodeData.NodeType != TreeNodeType.NT_DATASET)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Copy;
        }

        private void te_InputData_Enter(object sender, EventArgs e)
        {
            this.rtbHelpContent.Text = "\n    输入数据\n\n    输入数据可以直接打开本地的FDB，也可以从资源目录中直接拖拽，支持拖拽数据源和数据集。";
        }

        private void te_SHP_Enter(object sender, EventArgs e)
        {
            this.rtbHelpContent.Text = "\n    矢量数据\n\n    用于切割模型的polygon类型矢量数据，支持从本地文件夹拖拽。";
        }

        private void te_OutputData_DragDrop(object sender, DragEventArgs e)
        {
            TreeListNode treeNode = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
            if (treeNode == null)
                return;

            TreeNodeData nodeData = CatalogTreeServices.GetNodeData(treeNode);
            if (nodeData == null)
                return;

            this._DragTargetCatalogNode = treeNode;
            if (nodeData.NodeType == TreeNodeType.NT_DATASOURCE)
            {
                List<ToolKitDataSourceNodeData> dsLst = GetDataSourceItemsFromTreeNode(nodeData);

                if (dsLst.Count != 1)
                    return;
                //弹出要素选择对话框
                FeatureDataSetSelectDlg mySelectFrm = new FeatureDataSetSelectDlg(dsLst, true, false);
                if (mySelectFrm.ShowDialog(this) == DialogResult.OK)
                {
                    if (mySelectFrm.SelectFdsItems.Count != 1)
                        return;
                    ToolKitFeatureDataSetNodeData oneItem = mySelectFrm.SelectFdsItems[0];
                    te_OutputData.Text = oneItem.ToString();
                    te_OutputData.Tag = oneItem;
                    this._isDraggedTargetData = true;
                }
            }
            if (nodeData.NodeType == TreeNodeType.NT_DATASET)
            {
                ToolKitFeatureDataSetNodeData myItem = GetDataSetItemFromTreeNode(nodeData);
                if (myItem == null)
                    return;
                te_OutputData.Text = myItem.ToString();
                te_OutputData.Tag = myItem;
                this._isDraggedTargetData = true;
            }   
        }

        private void te_OutputData_DragEnter(object sender, DragEventArgs e)
        {
            TreeListNode treeNode = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
            if (treeNode == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            TreeNodeData nodeData = CatalogTreeServices.GetNodeData(treeNode);
            if (nodeData == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            if (nodeData.NodeType != TreeNodeType.NT_DATASOURCE && nodeData.NodeType != TreeNodeType.NT_DATASET)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Copy;
        }

        private void te_OutputData_Enter(object sender, EventArgs e)
        {
            this.rtbHelpContent.Text = "\n    目标数据集\n\n    可以直接打开本地的FDB，也可以从资源目录中直接拖拽，支持拖拽数据源和数据集。\n\n    不能与输入数据共用同一个数据集。";
        }

        private void sbtn_BrowseInputData_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "FDB File(*.FDB)|*.FDB";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fdbFile = dlg.FileName;
                List<ToolKitDataSourceNodeData> dsLst = new List<ToolKitDataSourceNodeData>();
                IDataSource ds = null;
                IFeatureDataSet fds = null;
                try
                {
                    //打开数据源
                    IConnectionInfo conn = new ConnectionInfo();
                    conn.ConnectionType = gviConnectionType.gviConnectionFireBird2x;
                    conn.Database = fdbFile;
                    ds = new DataSourceFactory().OpenDataSource(conn);
                    if (ds != null)
                    {
                        string sDsName = System.IO.Path.GetFileNameWithoutExtension(fdbFile);
                        ToolKitDataSourceNodeData dsItem = new ToolKitDataSourceNodeData(sDsName);
                        dsItem.ConnectionInfoStr = conn.ToConnectionString();
                        string[] fdsNames = ds.GetFeatureDatasetNames();
                        //遍历数据集
                        foreach (string sFdsName in fdsNames)
                        {
                            //打开数据集
                            fds = ds.OpenFeatureDataset(sFdsName);
                            if (fds != null)
                            {
                                ToolKitFeatureDataSetNodeData myItem = new ToolKitFeatureDataSetNodeData(sFdsName);
                                myItem.PrjWKT = fds.SpatialReference.AsWKT();
                                myItem.ConnectionInfoStr = conn.ToConnectionString();
                                myItem.DataSourceName = sDsName;
                                myItem.ParentDataSourceItem = dsItem;
                                dsItem.ChildFdsItems.Add(myItem);
                                //释放COM组件
                                Marshal.ReleaseComObject(fds);
                                fds = null;
                            }
                        }
                        if (dsItem.ChildFdsItems.Count > 0)
                            dsLst.Add(dsItem);
                        //释放COM组件
                        Marshal.ReleaseComObject(ds);
                        ds = null;
                    }
                }
                catch (COMException ex)
                {
                    //释放COM组件
                    if (ds != null)
                    {
                        Marshal.ReleaseComObject(ds);
                        ds = null;
                    }
                    if (fds != null)
                    {
                        Marshal.ReleaseComObject(fds);
                        fds = null;
                    }
                    XtraMessageBox.Show(this, Logger.GetErrorMessage(ex));
                }
                catch (Exception ex)
                {
                    //释放COM组件
                    if (ds != null)
                    {
                        Marshal.ReleaseComObject(ds);
                        ds = null;
                    }
                    if (fds != null)
                    {
                        Marshal.ReleaseComObject(fds);
                        fds = null;
                    }
                    XtraMessageBox.Show(this, ex.Message);
                }


                if (dsLst.Count < 1)
                    return;
                //弹出要素选择对话框
                FeatureDataSetSelectDlg mySelectFrm = new FeatureDataSetSelectDlg(dsLst, true, false);
                if (mySelectFrm.ShowDialog(this) == DialogResult.OK)
                {
                    if (mySelectFrm.SelectFdsItems.Count != 1)
                        return;
                    ToolKitFeatureDataSetNodeData oneItem = mySelectFrm.SelectFdsItems[0];
                    te_InputData.Text = oneItem.ToString();
                    te_InputData.Tag = oneItem;
                }
            }
        }

        private void sbtn_BrowseSHP_Click(object sender, EventArgs e)
        {
            IDataSource ds = null;
            IFeatureDataSet fds = null;
            try
            {
                bool b = false;
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.DefaultExt = "*.shp";
                dlg.Multiselect = false;
                dlg.Filter = "shp文件|*.shp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string shpfile = dlg.FileName;
                    IDataSourceFactory dsf = new DataSourceFactory();
                    IConnectionInfo conn = new ConnectionInfo();
                    conn.ConnectionType = gviConnectionType.gviConnectionShapeFile;
                    conn.Database = shpfile;
                    ds = dsf.OpenDataSource(conn);
                    if (ds != null)
                    {
                        foreach (string fdsn in ds.GetFeatureDatasetNames())
                        {
                            fds = ds.OpenFeatureDataset(fdsn);
                            //判断坐标系，目前只有PCS和Unknow可用
                            if (!CheckCoordinateReferenceSystem(fds))
                            {
                                XtraMessageBox.Show("坐标系错误，必须是投影坐标系或者Unknow");
                                sbtn_BrowseSHP.Focus();
                                return;
                            }
                            foreach (string fn in fds.GetNamesByType(gviDataSetType.gviDataSetFeatureClassTable))
                            {
                                shpFc = fds.OpenFeatureClass(fn);
                                if (shpFc != null)
                                {
                                    IFieldInfoCollection fields = shpFc.GetFields();
                                    for (int i = 0; i < fields.Count; i++)
                                    {
                                        IFieldInfo field = fields.Get(i);
                                        if (field.FieldType == gviFieldType.gviFieldGeometry)
                                        {
                                            if (field.GeometryDef != null)
                                            {
                                                if (field.GeometryDef.GeometryColumnType == gviGeometryColumnType.gviGeometryColumnPolygon)
                                                {
                                                    b = true;
                                                    polygonIndex = i;
                                                }
                                                else
                                                    b = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!b)
                    {
                        XtraMessageBox.Show("空间列类型不匹配");
                        sbtn_BrowseSHP.Focus();
                        return;
                    }
                    //遍历shp
                    IFdeCursor shpCursor = null;
                    try
                    {
                        shpCursor = shpFc.Search(null, true);
                        IRowBuffer shpRow = null;
                        while ((shpRow = shpCursor.NextRow()) != null)
                        {
                            IGeometry geo = shpRow.GetValue(polygonIndex) as IGeometry;
                            if (multiPolygon == null)
                                multiPolygon = new GeometryFactory().CreateGeometry(gviGeometryType.gviGeometryMultiPolygon, geo.VertexAttribute) as IMultiPolygon;
                            multiPolygon.AddGeometry(geo);
                        }
                    }
                    catch (COMException ex)
                    {
                        if (shpCursor != null)
                        {
                            Marshal.ReleaseComObject(shpCursor);
                            shpCursor = null;
                        }
                        if (multiPolygon != null)
                        {
                            Marshal.ReleaseComObject(multiPolygon);
                            multiPolygon = null;
                        }
                        XtraMessageBox.Show(this, ex.Message);
                    }
                    catch (Exception ex)
                    {
                        if (shpCursor != null)
                        {
                            Marshal.ReleaseComObject(shpCursor);
                            shpCursor = null;
                        }
                        if (multiPolygon != null)
                        {
                            Marshal.ReleaseComObject(multiPolygon);
                            multiPolygon = null;
                        }
                        XtraMessageBox.Show(this, ex.Message);
                    }
                    this.te_SHP.Text = dlg.FileName;
                }
            }
            catch (System.Exception ex)
            {
                Logger.WriteException(ex);
            }
            finally
            {
                if (fds != null)
                {
                    Marshal.ReleaseComObject(fds);
                    fds = null;
                }
                if (ds != null)
                {
                    Marshal.ReleaseComObject(ds);
                    ds = null;
                }
            }
        }

        private void sbtn_BrowseOutputData_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "FDB File(*.FDB)|*.FDB";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fdbFile = dlg.FileName;
                List<ToolKitDataSourceNodeData> dsLst = new List<ToolKitDataSourceNodeData>();
                IDataSource ds = null;
                IFeatureDataSet fds = null;
                try
                {
                    //打开数据源
                    IConnectionInfo conn = new ConnectionInfo();
                    conn.ConnectionType = gviConnectionType.gviConnectionFireBird2x;
                    conn.Database = fdbFile;
                    ds = new DataSourceFactory().OpenDataSource(conn);
                    if (ds != null)
                    {
                        string sDsName = System.IO.Path.GetFileNameWithoutExtension(fdbFile);
                        ToolKitDataSourceNodeData dsItem = new ToolKitDataSourceNodeData(sDsName);
                        dsItem.ConnectionInfoStr = conn.ToConnectionString();
                        string[] fdsNames = ds.GetFeatureDatasetNames();
                        //遍历数据集
                        foreach (string sFdsName in fdsNames)
                        {
                            //打开数据集
                            fds = ds.OpenFeatureDataset(sFdsName);
                            if (fds != null)
                            {
                                ToolKitFeatureDataSetNodeData myItem = new ToolKitFeatureDataSetNodeData(sFdsName);
                                myItem.PrjWKT = fds.SpatialReference.AsWKT();
                                myItem.ConnectionInfoStr = conn.ToConnectionString();
                                myItem.DataSourceName = sDsName;
                                myItem.ParentDataSourceItem = dsItem;
                                dsItem.ChildFdsItems.Add(myItem);
                                //释放COM组件
                                Marshal.ReleaseComObject(fds);
                                fds = null;
                            }
                        }
                        if (dsItem.ChildFdsItems.Count > 0)
                            dsLst.Add(dsItem);
                        //释放COM组件
                        Marshal.ReleaseComObject(ds);
                        ds = null;
                    }
                }
                catch (COMException ex)
                {
                    //释放COM组件
                    if (ds != null)
                    {
                        Marshal.ReleaseComObject(ds);
                        ds = null;
                    }
                    if (fds != null)
                    {
                        Marshal.ReleaseComObject(fds);
                        fds = null;
                    }
                    XtraMessageBox.Show(this, Logger.GetErrorMessage(ex));
                }
                catch (Exception ex)
                {
                    //释放COM组件
                    if (ds != null)
                    {
                        Marshal.ReleaseComObject(ds);
                        ds = null;
                    }
                    if (fds != null)
                    {
                        Marshal.ReleaseComObject(fds);
                        fds = null;
                    }
                    XtraMessageBox.Show(this, ex.Message);
                }


                if (dsLst.Count < 1)
                    return;
                //弹出要素选择对话框
                FeatureDataSetSelectDlg mySelectFrm = new FeatureDataSetSelectDlg(dsLst, true, false);
                if (mySelectFrm.ShowDialog(this) == DialogResult.OK)
                {
                    if (mySelectFrm.SelectFdsItems.Count != 1)
                        return;
                    ToolKitFeatureDataSetNodeData oneItem = mySelectFrm.SelectFdsItems[0];
                    te_OutputData.Text = oneItem.ToString();
                    te_OutputData.Tag = oneItem;
                }
            }
        }

        private void sbtnOK_Click(object sender, EventArgs e)
        {
            //有效性判断
            if (string.IsNullOrEmpty(this.te_InputData.Text.Trim()) || this.te_InputData.Tag == null)
            {
                XtraMessageBox.Show(this, "请选择源数据集！");
                return;
            }
            ToolKitFeatureDataSetNodeData inputItem = this.te_InputData.Tag as ToolKitFeatureDataSetNodeData;
            if (inputItem == null)
            {
                XtraMessageBox.Show(this, "源数据集无效！");
                return;
            }
            if (string.IsNullOrEmpty(this.te_OutputData.Text.Trim()) || this.te_OutputData.Tag == null)
            {
                XtraMessageBox.Show(this, "请选择目标数据集！");
                return;
            }
            ToolKitFeatureDataSetNodeData outputItem = this.te_OutputData.Tag as ToolKitFeatureDataSetNodeData;
            if (outputItem == null)
            {
                XtraMessageBox.Show(this, "目标数据集无效！");
                return;
            }
            if (inputItem.ConnectionInfoStr == outputItem.ConnectionInfoStr && inputItem.DataSetName == outputItem.DataSetName)
            {
                XtraMessageBox.Show(this, "源与目标数据集相同！");
                return;
            }
            if (multiPolygon == null || multiPolygon.GeometryCount == 0)
            {
                XtraMessageBox.Show(this, "shp数据无效！");
                return;
            }
            //设置新建数据集坐标系
            outputItem.PrjWKT = inputItem.PrjWKT;

            CutModelPointProcess dataProcess = new CutModelPointProcess(inputItem, outputItem, this.te_SHP.Text);
            ProgressDlg dlg = new ProgressDlg(dataProcess, this._isDraggedTargetData ? this._DragTargetCatalogNode : null);
            dlg.StartPosition = FormStartPosition.Manual;
            dlg.Location = new Point(MainFrmService.MainFrm.Width - dlg.Width - 10,
                                    MainFrmService.MainFrm.Height - dlg.Height - 10);
            dlg.RunWorker();
            dlg.Show(MainFrmService.MainFrm);

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void sbtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// 当拖进的TreeNodeData为NT_DATASOURCE类型时，获取DataSourceItem
        /// </summary>
        /// <param name="nodeData">TreeNodeData节点</param>
        /// <returns></returns>
        private List<ToolKitDataSourceNodeData> GetDataSourceItemsFromTreeNode(TreeNodeData nodeData)
        {
            IDataSource ds = null;
            IFeatureDataSet fds = null;
            List<ToolKitDataSourceNodeData> dsLst = new List<ToolKitDataSourceNodeData>();
            try
            {
                //打开数据源
                IConnectionInfo conn = new ConnectionInfo();
                conn.FromConnectionString(nodeData.ConnectionInfoStr);
                ds = new DataSourceFactory().OpenDataSource(conn);
                if (ds != null)
                {
                    ToolKitDataSourceNodeData dsItem = new ToolKitDataSourceNodeData(nodeData.DataSourceName);
                    dsItem.ConnectionInfoStr = nodeData.ConnectionInfoStr;

                    string[] fdsNames = ds.GetFeatureDatasetNames();
                    //遍历数据集
                    foreach (string sFdsName in fdsNames)
                    {
                        //打开数据集
                        fds = ds.OpenFeatureDataset(sFdsName);
                        if (fds != null)
                        {
                            ToolKitFeatureDataSetNodeData myItem = new ToolKitFeatureDataSetNodeData(sFdsName);
                            myItem.PrjWKT = fds.SpatialReference.AsWKT();
                            myItem.ConnectionInfoStr = nodeData.ConnectionInfoStr;
                            myItem.DataSourceName = nodeData.DataSourceName;
                            myItem.ParentDataSourceItem = dsItem;
                            dsItem.ChildFdsItems.Add(myItem);
                            //释放COM组件
                            Marshal.ReleaseComObject(fds);
                            fds = null;
                        }
                    }
                    dsLst.Add(dsItem);
                    //释放COM组件
                    Marshal.ReleaseComObject(ds);
                    ds = null;
                }
                return dsLst;
            }
            catch (COMException ex)
            {
                //释放COM组件
                if (ds != null)
                {
                    Marshal.ReleaseComObject(ds);
                    ds = null;
                }
                if (fds != null)
                {
                    Marshal.ReleaseComObject(fds);
                    fds = null;
                }
                XtraMessageBox.Show(this, Logger.GetErrorMessage(ex));
                return new List<ToolKitDataSourceNodeData>();
            }
            catch (Exception ex)
            {
                //释放COM组件
                if (ds != null)
                {
                    Marshal.ReleaseComObject(ds);
                    ds = null;
                }
                if (fds != null)
                {
                    Marshal.ReleaseComObject(fds);
                    fds = null;
                }
                XtraMessageBox.Show(this, ex.Message);
                return new List<ToolKitDataSourceNodeData>();
            }
        }

        /// <summary>
        /// 当拖进的TreeNodeData为NT_DATASET类型时，获取DataSetItem
        /// </summary>
        /// <param name="nodeData">TreeNodeData节点</param>
        /// <returns></returns>
        private ToolKitFeatureDataSetNodeData GetDataSetItemFromTreeNode(TreeNodeData nodeData)
        {
            IDataSource ds = null;
            IFeatureDataSet fds = null;
            try
            {
                ToolKitFeatureDataSetNodeData myItem = null;
                //打开数据源,获取坐标系
                IConnectionInfo conn = new ConnectionInfo();
                conn.FromConnectionString(nodeData.ConnectionInfoStr);
                ds = new DataSourceFactory().OpenDataSource(conn);
                if (ds != null)
                {
                    ToolKitDataSourceNodeData dsItem = new ToolKitDataSourceNodeData(nodeData.DataSourceName);
                    dsItem.ConnectionInfoStr = nodeData.ConnectionInfoStr;

                    fds = ds.OpenFeatureDataset(nodeData.DataSetName);
                    if (fds != null)
                    {
                        myItem = new ToolKitFeatureDataSetNodeData(nodeData.DataSetName);
                        myItem.PrjWKT = fds.SpatialReference.AsWKT();
                        myItem.ConnectionInfoStr = nodeData.ConnectionInfoStr;
                        myItem.DataSourceName = nodeData.DataSourceName;
                        myItem.ParentDataSourceItem = dsItem;
                        dsItem.ChildFdsItems.Add(myItem);
                        //释放COM组件
                        Marshal.ReleaseComObject(fds);
                        fds = null;
                    }
                    //释放COM组件
                    Marshal.ReleaseComObject(ds);
                    ds = null;
                }
                return myItem;
            }
            catch (COMException ex)
            {
                //释放COM组件
                if (ds != null)
                {
                    Marshal.ReleaseComObject(ds);
                    ds = null;
                }
                if (fds != null)
                {
                    Marshal.ReleaseComObject(fds);
                    fds = null;
                }
                XtraMessageBox.Show(this, Logger.GetErrorMessage(ex));
                return null;
            }
            catch (Exception ex)
            {
                //释放COM组件
                if (ds != null)
                {
                    Marshal.ReleaseComObject(ds);
                    ds = null;
                }
                if (fds != null)
                {
                    Marshal.ReleaseComObject(fds);
                    fds = null;
                }
                XtraMessageBox.Show(this, ex.Message);
                return null;
            }
        }

        private bool CheckCoordinateReferenceSystem(IFeatureDataSet fds)
        {
            bool b = false;
            try
            {
                ICoordinateReferenceSystem crs = fds.SpatialReference as ICoordinateReferenceSystem;
                if (crs.CrsType == gviCoordinateReferenceSystemType.gviCrsProject || crs.CrsType == gviCoordinateReferenceSystemType.gviCrsUnknown)
                    b = true;
                else
                    b = false;
            }
            catch (System.Exception ex)
            {

            }
            return b;
        }

        private String TrimPath(String path)
        {
            string s = "";
            int len1 = path.LastIndexOf(".");
            int len2 = path.LastIndexOf("\\");
            if (len1 != -1 && len2 != -1)
                s = path.Substring(len2 + 1, len1 - len2 - 1);
            return s;
        }

        

    }
}