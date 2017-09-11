using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Runtime.InteropServices;
using CityMakerBuilder.WorkSpace;
using Gvitech.CityMaker.FdeCore;
using DevExpress.XtraTreeList.Nodes;
using CityMakerBuilder.AddIn.WinForm;
using DataInteropCommon.PublicClass;
using DataInteropCommon.Dialog;
using Gvitech.CityMaker.Resource;

namespace ExampleToolkit.Dialog
{
    public partial class AddImageAsResourceDlg : ToolKitCommon.Dialog.ToolKitParentDlg
    {

        //GridView绑定数据源
        private DataTable _dt = null;

        public AddImageAsResourceDlg()
        {
            InitializeComponent();

            _dt = new DataTable();
            DataColumn col_DataPath = new DataColumn("DataPath", typeof(System.String));
            DataColumn col_DataItem = new DataColumn("DataItem", typeof(System.Object));
            _dt.Columns.AddRange(new DataColumn[] { col_DataPath, col_DataItem });
            this.gridControl1.DataSource = _dt;

            this._toolHelpContent = "\n    往资源库里加贴图\n\n    可往输入数据集中添加贴图，操作不可逆。";
        }

        private void sbtn_BrowseData_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "FDB File(*.FDB)|*.FDB";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string[] fdbFiles = dlg.FileNames;
                List<ToolKitDataSourceNodeData> dsLst = new List<ToolKitDataSourceNodeData>();
                //遍历数据源
                foreach (string filename in fdbFiles)
                {
                    IDataSource ds = null;
                    IFeatureDataSet fds = null;
                    try
                    {
                        //打开数据源
                        IConnectionInfo conn = new ConnectionInfo();
                        conn.ConnectionType = gviConnectionType.gviConnectionFireBird2x;
                        conn.Database = filename;
                        ds = new DataSourceFactory().OpenDataSource(conn);
                        if (ds != null)
                        {
                            string sDsName = System.IO.Path.GetFileNameWithoutExtension(filename);
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
                                    //this.lstbDatasets.Items.Add(myItem);
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
                }

                if (dsLst.Count < 1)
                    return;
                //弹出要素选择对话框
                FeatureDataSetSelectDlg mySelectFrm = new FeatureDataSetSelectDlg(dsLst, false, false);
                if (mySelectFrm.ShowDialog(this) == DialogResult.OK)
                {
                    foreach (ToolKitFeatureDataSetNodeData oneItem in mySelectFrm.SelectFdsItems)
                    {
                        _dt.Rows.Add(new object[] { oneItem.ToString(), oneItem });
                    }
                }
            }

            if (this.gv_InputData.SelectedRowsCount < 1)
            {
                this.gv_InputData.SelectRow(this.gv_InputData.FocusedRowHandle);
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
                FeatureDataSetSelectDlg mySelectFrm = new FeatureDataSetSelectDlg(dsLst, false, false);
                if (mySelectFrm.ShowDialog(this) == DialogResult.OK)
                {
                    foreach (ToolKitFeatureDataSetNodeData oneItem in mySelectFrm.SelectFdsItems)
                    {
                        _dt.Rows.Add(new object[] { oneItem.ToString(), oneItem });
                    }
                }
            }
            if (nodeData.NodeType == TreeNodeType.NT_DATASET)
            {
                ToolKitFeatureDataSetNodeData myItem = GetDataSetItemFromTreeNode(nodeData);
                if (myItem != null)
                {
                    _dt.Rows.Add(new object[] { myItem.ToString(), myItem });
                }
            }
            if (this.gv_InputData.SelectedRowsCount < 1)
            {
                this.gv_InputData.SelectRow(this.gv_InputData.FocusedRowHandle);
            }
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

        private void gv_InputData_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (this.gv_InputData.SelectedRowsCount == 1)
            {
                this.sbtn_Delete.Enabled = true;
                this.sbtn_Prev.Enabled = true;
                this.sbtn_Next.Enabled = true;
                if (this.gv_InputData.IsFirstRow)
                    this.sbtn_Prev.Enabled = false;
                if (this.gv_InputData.IsLastRow)
                    this.sbtn_Next.Enabled = false;
            }
            else if (this.gv_InputData.SelectedRowsCount > 1)
            {
                this.sbtn_Delete.Enabled = true;
                this.sbtn_Prev.Enabled = false;
                this.sbtn_Next.Enabled = false;
            }
            else
            {
                this.sbtn_Delete.Enabled = false;
                this.sbtn_Prev.Enabled = false;
                this.sbtn_Next.Enabled = false;
            }
        }

        private void sbtn_Delete_Click(object sender, EventArgs e)
        {
            this.gv_InputData.DeleteSelectedRows();
            this.gv_InputData.ClearSelection();
            this.gv_InputData.SelectRow(this.gv_InputData.FocusedRowHandle);
        }

        private void sbtn_Prev_Click(object sender, EventArgs e)
        {
            int iFocuseRowHandel = this.gv_InputData.FocusedRowHandle;
            if (iFocuseRowHandel > 0)
            {
                DataRow focuseDatatRow = this.gv_InputData.GetFocusedDataRow();
                object[] focuseRowItems = focuseDatatRow.ItemArray;
                DataRow prevDataRow = this.gv_InputData.GetDataRow(iFocuseRowHandel - 1);
                object[] prevRowItems = prevDataRow.ItemArray;
                prevDataRow.ItemArray = focuseRowItems;
                focuseDatatRow.ItemArray = prevRowItems;

                this.gv_InputData.FocusedRowHandle = iFocuseRowHandel - 1;
                this.gv_InputData.ClearSelection();
                this.gv_InputData.SelectRow(this.gv_InputData.FocusedRowHandle);
            }
        }

        private void sbtn_Next_Click(object sender, EventArgs e)
        {
            int iFocuseRowHandel = this.gv_InputData.FocusedRowHandle;
            if (iFocuseRowHandel < this.gv_InputData.RowCount - 1)
            {
                DataRow focuseDatatRow = this.gv_InputData.GetFocusedDataRow();
                object[] focuseRowItems = focuseDatatRow.ItemArray;
                DataRow nextDataRow = this.gv_InputData.GetDataRow(iFocuseRowHandel + 1);
                object[] nextRowItems = nextDataRow.ItemArray;
                nextDataRow.ItemArray = focuseRowItems;
                focuseDatatRow.ItemArray = nextRowItems;

                this.gv_InputData.FocusedRowHandle = iFocuseRowHandel + 1;
                this.gv_InputData.ClearSelection();
                this.gv_InputData.SelectRow(this.gv_InputData.FocusedRowHandle);
            }
        }

        private void gv_InputData_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle == this.gv_InputData.FocusedRowHandle)
            {
                e.Appearance.BackColor = Color.CornflowerBlue;
            }
        }

        private void sbtn_BrowseCopyrightImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "授权图片(*.jpg;*.bmp;*.png;*.tif)|*.jpg;*.bmp;*.png;*.tif";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.te_CopyrightImage.Text = dlg.FileName;
            }
        }

        private void sbtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void te_InputData_Enter(object sender, EventArgs e)
        {
            this.rtbHelpContent.Text = "\n    输入数据\n\n    输入数据可以直接打开本地的FDB，也可以从资源目录中直接拖拽，支持拖拽数据源和数据集。";
        }

        private void te_CopyrightImage_Enter(object sender, EventArgs e)
        {
            this.rtbHelpContent.Text = "\n    授权图片\n\n    用于加到资源库中的图片。";
        }

        private void sbtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.te_CopyrightImage.Text.Trim()))
            {
                XtraMessageBox.Show(this, "请选择授权图片！");
                return;
            }

            if (this.gv_InputData.RowCount < 1)
            {
                XtraMessageBox.Show(this, "请选择输入数据集！");
                return;
            }

            List<ToolKitFeatureDataSetNodeData> lstSource = new List<ToolKitFeatureDataSetNodeData>();
            for (int i = 0; i < this.gv_InputData.RowCount; i++)
            {
                ToolKitFeatureDataSetNodeData myItem = this.gv_InputData.GetDataRow(i)["DataItem"] as ToolKitFeatureDataSetNodeData;
                lstSource.Add(myItem);
            }

            IDataSource ds = null;
            IFeatureDataSet fds = null;
            try
            {
                //首先获取水印贴图
                IResourceFactory resFac = new ResourceFactory();
                IImage toAddImage = resFac.CreateImageFromFile(this.te_CopyrightImage.Text.Trim());
                //打开数据集
                ToolKitFeatureDataSetNodeData myFdsItem = lstSource[0];
                ds = new DataSourceFactory().OpenDataSourceByString(myFdsItem.ConnectionInfoStr);
                if (ds == null)
                    XtraMessageBox.Show("数据源打开失败");
                fds = ds.OpenFeatureDataset(myFdsItem.DataSetName);
                if (fds == null)
                    XtraMessageBox.Show("数据集打开失败");
                IResourceManager fdsResource = fds as IResourceManager;
                Guid imageGuid = new Guid();
                fdsResource.AddImage(imageGuid.ToString(), toAddImage);
                XtraMessageBox.Show("添加成功");
            }
            catch (System.Exception ex)
            {
            	
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
            
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void gv_InputData_RowCountChanged(object sender, EventArgs e)
        {
            if (this.gv_InputData.SelectedRowsCount == 1)
            {
                this.sbtn_Delete.Enabled = true;
                this.sbtn_Prev.Enabled = true;
                this.sbtn_Next.Enabled = true;
                if (this.gv_InputData.IsFirstRow)
                    this.sbtn_Prev.Enabled = false;
                if (this.gv_InputData.IsLastRow)
                    this.sbtn_Next.Enabled = false;
            }
            else if (this.gv_InputData.SelectedRowsCount > 1)
            {
                this.sbtn_Delete.Enabled = true;
                this.sbtn_Prev.Enabled = false;
                this.sbtn_Next.Enabled = false;
            }
            else
            {
                this.sbtn_Delete.Enabled = false;
                this.sbtn_Prev.Enabled = false;
                this.sbtn_Next.Enabled = false;
            }
        }
    }
}