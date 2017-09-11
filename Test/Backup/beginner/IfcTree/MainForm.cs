using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Xml;
//****Gvitech.CityMaker****
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.FdeCore;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.Common;
using Gvitech.CityMaker.FdeGeometry;


namespace IfcTree
{
    public partial class MainForm : Form
    {
        // 以下为解析xml中用到的节点属性名
        public const String ID = "ID";
        public const String NAME = "Name";

        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号        

        private TreeNode root = null; //存储树根节点  
        private IFeatureLayer layer = null;

        private System.Guid rootId = new System.Guid();
        private IFeatureClass fc = null;

        private ArrayList properties = null;
        private ITableLabel tableLabel = null;

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

            this.axRenderControl1.Camera.FlyTime = 0;

            // 加载FDB场景
            try
            {
                IConnectionInfo ci = new ConnectionInfo();
                ci.ConnectionType = gviConnectionType.gviConnectionFireBird2x;
                string tmpFDBPath = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\IFC.FDB");
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
                fc = dataset.OpenFeatureClass(fcnames[0]);
                IFieldInfoCollection fieldinfos = fc.GetFields();
                for (int i = 0; i < fieldinfos.Count; i++)
                {
                    IFieldInfo fieldinfo = fieldinfos.Get(i);
                    if (null == fieldinfo)
                        continue;
                    IGeometryDef geometryDef = fieldinfo.GeometryDef;
                    if (null == geometryDef)
                        continue;
                    ISimpleGeometryRender geoRender = new SimpleGeometryRender();
                    geoRender.RenderGroupField = "ParentObjectId";
                    layer = this.axRenderControl1.ObjectManager.CreateFeatureLayer(fc, fieldinfo.Name, null, geoRender, rootId);
                    IEnvelope env = geometryDef.Envelope;
                    if (env == null || (env.MaxX == 0.0 && env.MaxY == 0.0 && env.MaxZ == 0.0 &&
                        env.MinX == 0.0 && env.MinY == 0.0 && env.MinZ == 0.0))
                        continue;
                    IEulerAngle angle = new EulerAngle();
                    angle.Set(0, -20, 0);
                    this.axRenderControl1.Camera.LookAt(env.Center, 100, angle);

                    IPropertySet cusData = fc.CustomData;
                    string[] key = cusData.GetAllKeys();
                    string strXML = (string)cusData.GetProperty(key[0]);
                    ShowIFCTree(strXML);
                    break;
                }
            }
            catch (COMException ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return;
            }

            this.axRenderControl1.RcMouseClickSelect += new Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEventHandler(axRenderControl1_RcMouseClickSelect);
            this.axRenderControl1.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;
            this.axRenderControl1.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectFeatureLayer;

            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "IfcTree.html";
            }
        }

        void axRenderControl1_RcMouseClickSelect(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEvent e)
        {
            this.axRenderControl1.FeatureManager.UnhighlightAll();
            IPickResult pr = e.pickResult;
            if (pr == null)
                return;                      

            if (e.eventSender == gviMouseSelectMode.gviMouseSelectClick)
            {
                if (e.pickResult != null)
                {
                    if (pr.Type == gviObjectType.gviObjectFeatureLayer)
                    {
                        IFeatureLayerPickResult flpr = pr as IFeatureLayerPickResult;
                        int fid = flpr.FeatureId;
                        IFeatureLayer fl = flpr.FeatureLayer;
                        this.axRenderControl1.FeatureManager.HighlightFeature(fc, fid, 0xffff0000);
                        
                        if (properties != null)
                            properties.Clear();
                        properties = new ArrayList();
                        IRowBuffer row = fc.GetRow(fid);
                        TraverseProperty(row);

                        CreateTableLabel(e.intersectPoint);
                    }
                }
            }
        }

        private void CreateTableLabel(IPoint pos)
        {
            if (properties.Count == 0)
                return;

            if (tableLabel != null)
                this.axRenderControl1.ObjectManager.DeleteObject(tableLabel.Guid);

            // 创建一个有3行2列的TableLabel
            tableLabel = this.axRenderControl1.ObjectManager.CreateTableLabel(properties.Count, 2, rootId);
            // 设定表头文字
            tableLabel.TitleText = "属性信息展示";

            int i = 0;
            IEnumerator enu = properties.GetEnumerator();
            while(enu.MoveNext())
            {
                string property = (string)enu.Current;
                string[] strs = property.Split('#');
                tableLabel.SetRecord(i, 0, strs[0]);
                tableLabel.SetRecord(i, 1, strs[1]);
                i++;
            }            

            //标牌的位置            
            tableLabel.Position = pos;

            // 列宽度
            tableLabel.SetColumnWidth(0, 180);
            tableLabel.SetColumnWidth(1, 200);

            #region 设置样式
            // 表的边框颜色
            tableLabel.BorderColor = 0xffffffff;
            // 表的边框的宽度
            tableLabel.BorderWidth = 2;
            // 表的背景色
            tableLabel.TableBackgroundColor = 4290707456;

            // 标题背景色
            tableLabel.TitleBackgroundColor = 0xff000000;

            // 第一列文本样式
            TextAttribute headerTextAttribute = new TextAttribute();
            headerTextAttribute.TextColor = 0xffffffff;
            headerTextAttribute.OutlineColor = 0xff000000;
            headerTextAttribute.Font = "黑体";
            headerTextAttribute.Bold = true;
            headerTextAttribute.MultilineJustification = gviMultilineJustification.gviMultilineLeft;
            tableLabel.SetColumnTextAttribute(0, headerTextAttribute);

            // 第二列文本样式
            TextAttribute contentTextAttribute = new TextAttribute();
            contentTextAttribute.TextColor = 4293256677;
            contentTextAttribute.OutlineColor = 0xff000000;
            contentTextAttribute.Font = "黑体";
            contentTextAttribute.Bold = false;
            contentTextAttribute.MultilineJustification = gviMultilineJustification.gviMultilineLeft;
            tableLabel.SetColumnTextAttribute(1, contentTextAttribute);

            // 标题文本样式
            TextAttribute capitalTextAttribute = new TextAttribute();
            capitalTextAttribute.TextColor = 0xffffffff;
            capitalTextAttribute.OutlineColor = 4279834905;
            capitalTextAttribute.Font = "华文新魏";
            capitalTextAttribute.TextSize = 14;
            capitalTextAttribute.MultilineJustification = gviMultilineJustification.gviMultilineCenter;
            capitalTextAttribute.Bold = true;
            tableLabel.TitleTextAttribute = capitalTextAttribute;  
            #endregion
        }

        private void TraverseProperty(IRowBuffer row)
        {
            IFieldInfoCollection col = row.Fields;
            for (int i = 0; i < col.Count; i++)
            {
                IFieldInfo info = col.Get(i);
                switch (info.Name)
                {
                    case "ObjectId":
                    case "ObjectName":
                    case "ClassName":
                    case "GlobalId":
                    case "ObjectType":
                    case "Description":
                        properties.Add(info.Name + "#" + row.GetValue(row.FieldIndex(info.Name)));
                        break;
                    case "Properties":
                        {
                            int bufferPos = row.FieldIndex("Properties");
                            IBinaryBuffer buffer = row.GetValue(bufferPos) as IBinaryBuffer;
                            string strContent = buffer.AsString();
                            TraverseXml(strContent);
                        }
                        break;
                }
            }
        }

        private void TraverseXml(String lc)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(lc);

                //遍历所有子节点
                LoadXml2TreeList(xmlDoc.DocumentElement.ChildNodes, "");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private void LoadXml2TreeList(XmlNodeList list, string parentPropertyName)
        {
            foreach (XmlNode nd in list)
            {
                if (nd.HasChildNodes)
                {
                    LoadXml2TreeList(nd.ChildNodes, nd.Name);
                }
                else
                {
                    string key = parentPropertyName + "/" + nd.Name;
                    properties.Add(key + "#" + nd.Attributes[0].Value);
                }
            }
        }

        #region 解析逻辑树xml加载到界面控件
        private void ShowIFCTree(String lc)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(lc);

                //读取DocumentElement节点
                XmlElement element = xmlDoc.DocumentElement;
                root = AddNode(element, this.treeView1.Nodes);

                //遍历所有子节点
                LoadXml2TreeList(xmlDoc.DocumentElement.ChildNodes, root.Nodes);

                this.treeView1.ExpandAll();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 从xml导入逻辑图层树
        /// </summary>
        private int LoadXml2TreeList(XmlNodeList list, TreeNodeCollection pNodes)
        {
            int nCount = 0;
            foreach (XmlNode nd in list)
            {
                if (nd.HasChildNodes)
                {
                    TreeNode node = AddNode(nd, pNodes);

                    layer.SetGroupVisibleMask((int)node.Tag, gviViewportMask.gviViewAllNormalView);

                    int childNodeCount = LoadXml2TreeList(nd.ChildNodes, node.Nodes);
                    node.Text = node.Text + "(" + childNodeCount + ")";
                }
                else
                {
                    //叶子节点不上树
                    nCount++;
                }
            }
            return nCount;
        }

        private TreeNode AddNode(XmlNode xmlNode, TreeNodeCollection col)
        {
            int id = 0;
            String name = null;
            if (!int.TryParse(xmlNode.Attributes[ID].Value, out id))
            {
                return null;
            }
            name = xmlNode.Attributes[NAME].Value;

            TreeNode node = new TreeNode(name, 0, 0);
            node.Tag = id;
            node.Checked = true;
            col.Add(node);

            return node;
        }

        #endregion

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                TravelTreeNode(e.Node, e.Node.Checked ? gviViewportMask.gviViewAllNormalView : gviViewportMask.gviViewNone);
            }
        }

        private void TravelTreeNode(TreeNode node, gviViewportMask mask)
        {
            if (mask == gviViewportMask.gviViewAllNormalView)
                node.Checked = true;
            else
                node.Checked = false;

            if (node.Nodes.Count == 0)
            {
                layer.SetGroupVisibleMask((int)node.Tag, mask);                
                return;
            }

            foreach (TreeNode n in node.Nodes)
            {
                TravelTreeNode(n, mask);
            }
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            TreeNode nnn = this.treeView1.GetNodeAt(e.X, e.Y);
            if (nnn == null)
                UnHighlightAll(root);
            else
            {
                //判断的目的在于：当选中复选框时仅仅控制显示隐藏，而不改变颜色
                if (nnn.Bounds.Contains(e.X, e.Y))
                {
                    UnHighlightAll(root);
                    TravelTreeNode(nnn);
                }
            }
        }

        private void TravelTreeNode(TreeNode node)
        {
            if (node.Nodes.Count == 0)
            {
                if (!layer.GetEnableGroupColor((int)node.Tag))
                    layer.SetEnableGroupColor((int)node.Tag, true);
                layer.SetGroupColor((int)node.Tag, 0x88ffff00);
                return;
            }

            foreach (TreeNode n in node.Nodes)
            {
                TravelTreeNode(n);
            }
        }

        private void UnHighlightAll(TreeNode node)
        {
            if (node.Nodes.Count == 0)
            {
                layer.SetEnableGroupColor((int)node.Tag, false);
                return;
            }

            foreach (TreeNode n in node.Nodes)
            {
                UnHighlightAll(n);
            }
        }

    }

}
